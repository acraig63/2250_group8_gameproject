using UnityEngine;

namespace DefaultNamespace
{
    public class MazeKeyDoor : MonoBehaviour
    {
        private BoxCollider2D _collider;
        private SpriteRenderer _sr;
        private bool _opened = false;

        private void Start()
        {
            _collider = GetComponent<BoxCollider2D>();
            _sr       = GetComponent<SpriteRenderer>();

            if (BlackwaterState.hasMazeKey)
                OpenInstantly();
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (_opened) return;

            bool isPlayer = col.gameObject.CompareTag("Player")
                         || col.gameObject.GetComponent<PlayerController>() != null;
            if (!isPlayer) return;

            if (BlackwaterState.hasMazeKey)
            {
                _opened = true;
                if (_collider != null) _collider.enabled = false;
                if (_sr != null) _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 0.2f);
                PopupMessage.Show("The way forward is open.", 2f);
            }
            else
            {
                int have = BlackwaterState.collectedKeyPieces.Count;
                int need = 4 - have;
                PopupMessage.Show("Blocked. Defeat " + need + " more enemies to pass. (" + have + "/4)", 3f);
            }
        }

        private void Update()
        {
            if (!_opened && BlackwaterState.hasMazeKey)
                OpenInstantly();
        }

        private void OpenInstantly()
        {
            _opened = true;
            if (_collider != null) _collider.enabled = false;
            if (_sr != null) _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 0.2f);
        }

        public static GameObject Create(Vector3 position)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            Sprite spr = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

            GameObject go = new GameObject("MazeKeyDoor");
            go.transform.position   = position;
            go.transform.localScale = new Vector3(3f, 3f, 1f);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = spr;
            sr.color        = new Color(0.5f, 0.25f, 0.1f);  // brown
            sr.sortingOrder = 3;

            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;

            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = false;  // solid

            go.AddComponent<MazeKeyDoor>();
            return go;
        }
    }
}
