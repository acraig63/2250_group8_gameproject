using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    // Attached to the HUDCanvas in BlackwaterFlagship.unity.
    // Keeps the canvas alive across all Blackwater sub-rooms, hides it
    // during battle, and destroys it when the player leaves Level 5.
    public class HudPersist : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            bool inBlackwater = scene.name.StartsWith("Blackwater");
            bool inBattle     = scene.name == "Battle" || scene.name == "pirateBattleScene";

            if (!inBlackwater && !inBattle)
                Destroy(gameObject);          // player left Level 5 entirely
            else
                gameObject.SetActive(!inBattle); // hide during battle, show in Blackwater
        }
    }
}
