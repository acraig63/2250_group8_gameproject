using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Collectible key fragment dropped by defeated NPCs.
    /// SpawnKeyPiece() is called by Level5NPCReward.OnDestroy().
    /// ResetPieceCounter() is called by BlackwaterState.Reset() on death.
    /// </summary>
    public class KeyPiecePickup : MonoBehaviour
    {
        public int pieceId;

        private static int _nextPieceId = 0;

        /// <summary>Resets the piece ID counter. Call from BlackwaterState.Reset().</summary>
        public static void ResetPieceCounter()
        {
            _nextPieceId = 0;
        }

        public static void SpawnKeyPiece(Vector3 position)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            Sprite spr = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

            GameObject go = new GameObject("KeyPiece_" + _nextPieceId);
            go.transform.position   = position;
            go.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = spr;
            sr.color        = new Color(1f, 0.85f, 0f);   // gold
            sr.sortingOrder = 5;

            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            KeyPiecePickup pickup = go.AddComponent<KeyPiecePickup>();
            pickup.pieceId = _nextPieceId;

            _nextPieceId++;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player") && other.GetComponent<PlayerController>() == null)
                return;

            string key = "piece_" + pieceId;
            if (BlackwaterState.collectedKeyPieces.Contains(key))
            {
                Destroy(gameObject);
                return;
            }

            BlackwaterState.collectedKeyPieces.Add(key);
            int count = BlackwaterState.collectedKeyPieces.Count;

            PopupMessage.Show("Key piece collected! (" + count + "/5)", 3f);

            if (count >= 5)
                PopupMessage.Show("All key pieces collected! Find the Captain's Quarters!", 5f);

            Destroy(gameObject);
        }
    }
}
