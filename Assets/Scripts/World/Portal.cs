using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class Portal : MonoBehaviour
    {
        [Tooltip("Exact scene name to load (must be registered in Build Settings).")]
        public string targetScene;

        [Tooltip("World position where the player will appear in the destination scene.")]
        public Vector2 spawnPosition;

        private static Vector2 _pendingSpawn;
        private static bool    _hasPendingSpawn;
        private static bool _justTeleported;

        void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"[Portal] Trigger entered by {other.gameObject.name}, justTeleported={_justTeleported}, targetScene={targetScene}");
            if (!other.CompareTag("Player")) return;
            if (_justTeleported) return;

            _pendingSpawn    = spawnPosition;
            _hasPendingSpawn = true;

            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene.StartsWith("Blackwater") || targetScene.StartsWith("Blackwater"))
                BlackwaterState.SavePlayerState();

            Debug.Log($"[Portal] Loading scene '{targetScene}', spawn at {spawnPosition}");
            SceneManager.LoadScene(targetScene);
        }

        void OnTriggerExit2D(Collider2D other)
        {
            Debug.Log($"[Portal] Trigger exited by {other.gameObject.name}, resetting _justTeleported");
            if (other.CompareTag("Player"))
                _justTeleported = false;
        }

        public static void ApplyPendingSpawn()
        {
            if (!_hasPendingSpawn) return;
            _hasPendingSpawn = false;
            _justTeleported  = true;

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(_pendingSpawn.x, _pendingSpawn.y, 0f);
                Debug.Log($"[Portal] Player repositioned to {_pendingSpawn}");
            }
            else
            {
                Debug.LogWarning("[Portal] ApplyPendingSpawn: no GameObject tagged 'Player' found.");
            }
        }

        public static bool HasPendingSpawn => _hasPendingSpawn;
    }
}
