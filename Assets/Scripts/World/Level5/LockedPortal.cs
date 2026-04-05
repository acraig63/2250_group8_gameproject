using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    /// <summary>
    /// Blocks entry to Captain's Quarters until all 5 key pieces are collected.
    /// Uses reflection to set Portal's private static spawn fields — Portal has no
    /// public SetPendingSpawn method (confirmed by reading Portal.cs).
    /// </summary>
    public class LockedPortal : MonoBehaviour
    {
        public string  targetScene    = "BlackwaterCaptainsQuarters";
        public Vector2 spawnPosition  = new Vector2(12f, 3f);

        private SpriteRenderer _sr;

        private void Start()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        public static GameObject Create(Vector3 position, string target, Vector2 spawn)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            Sprite spr = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

            GameObject go = new GameObject("LockedPortal_" + target);
            go.transform.position   = position;
            go.transform.localScale = new Vector3(2f, 3f, 1f);   // door shape

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = spr;
            sr.color        = new Color(0.4f, 0.2f, 0.05f);      // dark brown
            sr.sortingOrder = 6;

            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            LockedPortal portal = go.AddComponent<LockedPortal>();
            portal.targetScene   = target;
            portal.spawnPosition = spawn;

            return go;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player") && other.GetComponent<PlayerController>() == null)
                return;

            if (BlackwaterState.collectedKeyPieces.Count >= 5)
            {
                if (_sr != null) _sr.color = Color.green;
                SetPortalPendingSpawn(spawnPosition);
                SceneManager.LoadScene(targetScene);
            }
            else
            {
                int have = BlackwaterState.collectedKeyPieces.Count;
                int need = 5 - have;
                PopupMessage.Show("Locked. Need " + need + " more key pieces. (" + have + "/5)", 3f);

                // Push player back away from door
                Vector3 pushDir = (other.transform.position - transform.position).normalized;
                other.transform.position += pushDir * 2f;
            }
        }

        private void Update()
        {
            if (_sr != null &&
                BlackwaterState.collectedKeyPieces.Count >= 5 &&
                _sr.color != Color.green)
            {
                _sr.color = Color.green;
            }
        }

        /// <summary>
        /// Sets Portal's private static pending-spawn fields via reflection.
        /// Portal.cs does not expose a public SetPendingSpawn method.
        /// </summary>
        private static void SetPortalPendingSpawn(Vector2 spawn)
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Static;
            Type portalType = typeof(Portal);

            FieldInfo pendingSpawnField   = portalType.GetField("_pendingSpawn",    flags);
            FieldInfo hasPendingSpawnField = portalType.GetField("_hasPendingSpawn", flags);

            if (pendingSpawnField != null)
                pendingSpawnField.SetValue(null, spawn);
            else
                Debug.LogWarning("LockedPortal: Portal._pendingSpawn field not found via reflection.");

            if (hasPendingSpawnField != null)
                hasPendingSpawnField.SetValue(null, true);
            else
                Debug.LogWarning("LockedPortal: Portal._hasPendingSpawn field not found via reflection.");
        }
    }
}
