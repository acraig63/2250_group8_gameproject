using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
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
            _cachedPC = null;

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

            if (!scene.StartsWith("Blackwater")) return;
            if (scene == "Battle" || scene == "pirateBattleScene") return;

            if (_cachedPC == null)
                _cachedPC = FindObjectOfType<PlayerController>();

            if (_cachedPC == null) return;

            if (_cachedPC.GetHealth() <= 0)
            {
                string dyingCharacter = CharacterSelectManager.selectedCharacter;
                CharacterDisableManager.DisableCharacter(dyingCharacter);
                BlackwaterState.Reset();
                _cachedPC = null;

                if (CharacterDisableManager.AllDisabled())
                {
                    PopupMessage.Show("All crew members have fallen!", 2f);
                    StartCoroutine(DelayedGameOver());
                }
                else
                {
                    PopupMessage.Show("You have fallen... Choose your next crew member.", 3f);
                    CharacterSelectEnforcer.Spawn();
                    StartCoroutine(DelayedSceneLoad());
                }
            }
        }

        private IEnumerator DelayedGameOver()
        {
            yield return new WaitForSeconds(2f);
            GameOverHandler.Show();
        }

        private IEnumerator DelayedSceneLoad()
        {
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("OpeningScreen");
        }
    }
}
