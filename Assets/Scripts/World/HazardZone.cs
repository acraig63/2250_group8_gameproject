using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Attached automatically by IslandSceneBuilder to every tile whose
    /// TileType is Hazard (quicksand zone).
    ///
    /// Deals damage-over-time while the player stands on the tile.
    /// Detects the player by the "Player" tag and calls TakeDamage via
    /// SendMessage — no hard reference to PlayerManager needed, so this
    /// compiles independently of the player subsystem.
    ///
    /// Inspector tweaks:
    ///   damagePerTick  — HP removed each interval (default 5).
    ///   damageInterval — seconds between ticks (default 0.5 s).
    ///   flashColor     — tint applied to this tile briefly after a hit.
    /// </summary>
    public class HazardZone : MonoBehaviour
    {
        [Header("Damage")]
        [SerializeField] private int   damagePerTick  = 5;
        [SerializeField] private float damageInterval = 0.5f;

        [Header("Visual Feedback")]
        [SerializeField] private Color flashColor    = new Color(1f, 0.25f, 0.25f, 0.65f);
        [SerializeField] private float flashDuration = 0.12f;

        private bool           _playerInside;
        private float          _damageTimer;
        private float          _flashTimer;
        private bool           _flashing;
        private GameObject     _playerObject;
        private SpriteRenderer _sr;
        private Color          _originalColor;

        void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            if (_sr != null) _originalColor = _sr.color;
        }

        void Update()
        {
            if (_playerInside && _playerObject != null)
            {
                _damageTimer += Time.deltaTime;
                if (_damageTimer >= damageInterval)
                {
                    _damageTimer = 0f;
                    DealDamage();
                }
            }

            if (_flashing)
            {
                _flashTimer += Time.deltaTime;
                if (_flashTimer >= flashDuration)
                {
                    _flashing = false;
                    if (_sr != null) _sr.color = _originalColor;
                }
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _playerObject = other.gameObject;
            _playerInside = true;
            _damageTimer  = damageInterval;
            Debug.Log($"HazardZone ({name}): player entered quicksand.");
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            _playerInside = false;
            _damageTimer  = 0f;
            _playerObject = null;
            if (_sr != null) _sr.color = _originalColor;
            _flashing = false;
        }

        private void DealDamage()
        {
            if (_playerObject == null) return;

            // SendMessage calls TakeDamage on whatever component owns it —
            // works without a hard reference to PlayerManager.
            _playerObject.SendMessage("TakeDamage", damagePerTick, SendMessageOptions.DontRequireReceiver);
            Debug.Log($"HazardZone: sent TakeDamage({damagePerTick}) to player.");

            if (_sr != null) { _sr.color = flashColor; _flashTimer = 0f; _flashing = true; }
        }

        public bool IsPlayerInside()             => _playerInside;
        public void SetDamagePerTick(int damage) => damagePerTick = Mathf.Max(0, damage);
    }
}
