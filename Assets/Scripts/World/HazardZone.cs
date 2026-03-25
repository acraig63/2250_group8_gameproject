using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Attached automatically by IslandSceneBuilder to every tile whose
    /// TileType is <see cref="TileType.Hazard"/> (quicksand zone).
    ///
    /// Deals damage-over-time while the player stands on the tile.
    /// Uses OnTriggerStay2D so it fires continuously without polling Update.
    ///
    /// Requirements on the player's GameObject:
    ///   • Rigidbody2D    (so Unity dispatches trigger events)
    ///   • PlayerManager  (holds the Player data model)
    ///
    /// Inspector tweaks:
    ///   • damagePerTick  — HP removed each interval (default 5).
    ///   • damageInterval — seconds between ticks (default 0.5 s).
    ///   • flashColor     — tint applied to this tile briefly after a hit.
    /// </summary>
    public class HazardZone : MonoBehaviour
    {
        [Header("Damage")]
        [SerializeField] private int   damagePerTick  = 5;
        [SerializeField] private float damageInterval = 0.5f;

        [Header("Visual Feedback")]
        [SerializeField] private Color flashColor    = new Color(1f, 0.25f, 0.25f, 0.65f);
        [SerializeField] private float flashDuration = 0.12f;

        // Internal state
        private bool           _playerInside;
        private float          _damageTimer;
        private float          _flashTimer;
        private bool           _flashing;
        private global::PlayerManager _playerManager;
        private SpriteRenderer _sr;
        private Color          _originalColor;

        void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            if (_sr != null) _originalColor = _sr.color;
        }

        void Update()
        {
            if (_playerInside && _playerManager != null)
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
            global::PlayerManager pm = other.GetComponent<global::PlayerManager>();
            if (pm == null) return;
            _playerManager = pm;
            _playerInside  = true;
            _damageTimer   = damageInterval; // immediate first hit
            Debug.Log($"HazardZone ({name}): player entered quicksand.");
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.GetComponent<global::PlayerManager>() == null) return;
            _playerInside  = false;
            _damageTimer   = 0f;
            _playerManager = null;
            if (_sr != null) _sr.color = _originalColor;
            _flashing = false;
        }

        private void DealDamage()
        {
            if (_playerManager?.player == null) return;
            if (!_playerManager.player.IsAlive()) return;

            _playerManager.player.TakeDamage(damagePerTick);
            Debug.Log($"HazardZone: -{damagePerTick} HP | remaining: {_playerManager.player.GetHealthNormalized()*100:F0}%");

            if (_sr != null) { _sr.color = flashColor; _flashTimer = 0f; _flashing = true; }

            if (!_playerManager.player.IsAlive())
            {
                Debug.Log("HazardZone: player defeated in quicksand.");
                _playerInside = false;
            }
        }

        public bool IsPlayerInside()              => _playerInside;
        public void SetDamagePerTick(int damage)  => damagePerTick = Mathf.Max(0, damage);
    }
}
