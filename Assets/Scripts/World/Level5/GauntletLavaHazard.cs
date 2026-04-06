using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Lava hazard tiles in the BlackwaterLowerDeck gauntlet corridor.
    ///
    /// Unlike HazardZone (which sends TakeDamage via SendMessage → PlayerHazardShield
    /// → DEF reduction applied), this component calls PlayerController.SetHealth()
    /// directly. DEF stats therefore do NOT reduce gauntlet damage.
    ///
    /// Immunity note: HazardImmunityManager.IsImmune IS respected here.
    /// Immunity is granted by GauntletReward at the END of the gauntlet, so the
    /// player never has immunity on the first traversal. On re-entry after collecting
    /// the reward, immunity protects them (intentional design — collect once, safe return).
    ///
    /// Damage math: 8 dmg / 0.5 s tick = 16 dmg/s.
    ///   With Speed Boots (1.5×): 30-unit corridor ≈ 4 s → ~64 total damage (survives).
    ///   Without Speed Boots   :  30-unit corridor ≈ 6 s → ~96 total damage (near-lethal).
    /// </summary>
    public class GauntletLavaHazard : MonoBehaviour
    {
        public int   damagePerTick = 8;
        public float tickInterval  = 0.5f;

        private float _lastDamageTime = 0f;

        private void OnTriggerStay2D(Collider2D other)
        {
            // Immunity check (granted after gauntlet is completed)
            if (HazardImmunityManager.IsImmune) return;

            // Tick-rate limiter
            if (Time.time - _lastDamageTime < tickInterval) return;

            // Player check
            bool isPlayer = other.CompareTag("Player")
                         || other.GetComponent<PlayerController>() != null;
            if (!isPlayer) return;

            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc == null) return;

            _lastDamageTime = Time.time;

            // Direct SetHealth call — bypasses PlayerHazardShield and DEF reduction
            pc.SetHealth(pc.GetHealth() - damagePerTick);
        }

        /// <summary>
        /// Creates a lava hazard tile at the given world position.
        /// Scale is 2×2 to cover the corridor width.
        /// </summary>
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
