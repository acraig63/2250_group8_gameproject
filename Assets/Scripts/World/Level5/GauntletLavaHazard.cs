using UnityEngine;

namespace DefaultNamespace
{
    public class GauntletLavaHazard : MonoBehaviour
    {
        public int   damagePerTick = 8;
        public float tickInterval  = 0.5f;

        private float _lastDamageTime = 0f;

        private void OnTriggerStay2D(Collider2D other)
        {
            if (HazardImmunityManager.IsImmune) return;
            if (Time.time - _lastDamageTime < tickInterval) return;

            bool isPlayer = other.CompareTag("Player")
                         || other.GetComponent<PlayerController>() != null;
            if (!isPlayer) return;

            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc == null) return;

            _lastDamageTime = Time.time;
            pc.SetHealth(pc.GetHealth() - damagePerTick);
        }

        public static GameObject Create(Vector3 position)
        {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            Sprite spr = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

            GameObject go = new GameObject("LavaHazard");
            go.transform.position   = position;
            go.transform.localScale = new Vector3(2f, 2f, 1f);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = spr;
            sr.color        = new Color(1f, 0.2f, 0f, 0.6f);   // orange-red lava
            sr.sortingOrder = 1;

            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            go.AddComponent<GauntletLavaHazard>();
            return go;
        }
    }
}
