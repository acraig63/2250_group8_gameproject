using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Placed at the end of the gauntlet corridor in BlackwaterLowerDeck.
    /// Requires Speed Boots to activate. Grants: full heal, hazard immunity,
    /// and the 5th key piece ("maze_key").
    /// </summary>
    public class GauntletReward : MonoBehaviour
    {
        private bool _collected = false;

        public static GameObject Create(Vector3 position)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            Sprite spr = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

            GameObject go = new GameObject("GauntletReward");
            go.transform.position   = position;
            go.transform.localScale = new Vector3(2f, 2f, 1f);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = spr;
            sr.color        = new Color(0f, 1f, 0.5f);   // green-gold
            sr.sortingOrder = 5;

            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size      = new Vector2(3f, 3f);

            go.AddComponent<GauntletReward>();
            return go;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_collected) return;

            if (!other.CompareTag("Player") && other.GetComponent<PlayerController>() == null)
                return;

            // Already collected in a prior visit (hasSavedState not needed here — immunity tracks it)
            if (BlackwaterState.hasHazardImmunity) return;

            if (!BlackwaterState.hasSpeedBoots)
            {
                PopupMessage.Show("You need Speed Boots to survive the gauntlet!", 3f);
                return;
            }

            _collected = true;

            // Full heal
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
                pc.SetHealth(pc.MaxHealth);

            // Grant hazard immunity
            BlackwaterState.hasHazardImmunity = true;
            HazardImmunityManager.SetImmune(true);

            // 5th and final key piece
            if (!BlackwaterState.collectedKeyPieces.Contains("maze_key"))
                BlackwaterState.collectedKeyPieces.Add("maze_key");

            int count = BlackwaterState.collectedKeyPieces.Count;
            PopupMessage.Show("Final key piece! Full heal and hazard immunity granted! (" + count + "/5)", 4f);

            if (count >= 5)
                PopupMessage.Show("All key pieces collected! Find the Captain's Quarters!", 5f);

            Destroy(gameObject);
        }
    }
}
