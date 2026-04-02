using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    /// <summary>
    /// Attach to a trigger GameObject to transition to another scene and
    /// place the player at a specific world position in that scene.
    ///
    /// Usage:
    ///   1. Add BoxCollider2D (isTrigger = true) to the same GameObject.
    ///   2. Set targetScene  — the exact scene name (must be in Build Settings).
    ///   3. Set spawnPosition — world position the player will appear at.
    ///
    ///   In the destination scene's builder (e.g. BlackwaterSceneBuilder),
    ///   call  Portal.ApplyPendingSpawn()  at the top of Start() so the
    ///   player is repositioned as soon as the scene loads.
    /// </summary>
    public class Portal : MonoBehaviour
    {
        [Tooltip("Exact scene name to load (must be registered in Build Settings).")]
        public string targetScene;

        [Tooltip("World position where the player will appear in the destination scene.")]
        public Vector2 spawnPosition;

        // Static state: survives the scene load
        private static Vector2 _pendingSpawn;
        private static bool    _hasPendingSpawn;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            _pendingSpawn    = spawnPosition;
            _hasPendingSpawn = true;

            Debug.Log($"[Portal] Loading scene '{targetScene}', spawn at {spawnPosition}");
            SceneManager.LoadScene(targetScene);
        }

        /// <summary>
        /// Call this from the scene initialiser (Start or Awake) of the destination scene.
        /// If a pending spawn is waiting it moves the Player to that position and clears the flag.
        /// </summary>
        public static void ApplyPendingSpawn()
        {
            if (!_hasPendingSpawn) return;
            _hasPendingSpawn = false;

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

        /// <summary>Returns true when a spawn position is waiting to be applied.</summary>
        public static bool HasPendingSpawn => _hasPendingSpawn;
    }
}
