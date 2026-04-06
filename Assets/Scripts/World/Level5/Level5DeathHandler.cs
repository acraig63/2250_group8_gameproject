using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    /// <summary>
    /// Persistent singleton that watches player health in Blackwater scenes.
    /// On death: resets all Blackwater state and returns to SmugglersIsland.
    /// Destroys itself when the player leaves all Blackwater scenes.
    /// </summary>
    public class Level5DeathHandler : MonoBehaviour
    {
        private static Level5DeathHandler _instance;
        private PlayerController _cachedPC;

        public static void EnsureExists()
        {
            if (_instance != null) return;
            new GameObject("Level5DeathHandler").AddComponent<Level5DeathHandler>();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (_instance == this) _instance = null;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Invalidate cached PC so we find the fresh instance in new scene.
            _cachedPC = null;

            // Self-destruct once the player has fully left Blackwater.
            bool isBlackwater = scene.name.StartsWith("Blackwater");
            bool isBattle     = scene.name == "Battle" || scene.name == "pirateBattleScene";
            if (!isBlackwater && !isBattle)
            {
                Destroy(gameObject);
                _instance = null;
            }
        }

        private void Update()
        {
            string scene = SceneManager.GetActiveScene().name;

            // Only monitor in overworld Blackwater scenes.
            if (!scene.StartsWith("Blackwater")) return;
            if (scene == "Battle" || scene == "pirateBattleScene") return;

            if (_cachedPC == null)
                _cachedPC = FindObjectOfType<PlayerController>();

            if (_cachedPC == null) return;

            if (_cachedPC.GetHealth() <= 0)
            {
                BlackwaterState.Reset();
                PopupMessage.Show("You have fallen... Returning to Smuggler's Island.", 3f);
                _cachedPC = null;
                StartCoroutine(DelayedSceneLoad());
            }
        }

        private IEnumerator DelayedSceneLoad()
        {
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("SmugglersIsland");
        }
    }
}
