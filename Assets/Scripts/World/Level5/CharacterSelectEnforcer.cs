using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace DefaultNamespace
{
    // Spawned by Level5DeathHandler when a character dies in Blackwater.
    // Persists via DontDestroyOnLoad, survives through OpeningScreen, waits for
    // CharacterCustomization to load, then greys out buttons for defeated characters.
    public class CharacterSelectEnforcer : MonoBehaviour
    {
        private static CharacterSelectEnforcer _instance;

        public static void Spawn()
        {
            if (_instance != null) return;
            var go = new GameObject("CharacterSelectEnforcer");
            _instance = go.AddComponent<CharacterSelectEnforcer>();
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
            if (scene.name == "CharacterCustomization")
                StartCoroutine(EnforceDisabledCharacters());
            else if (scene.name != "OpeningScreen")
                Destroy(gameObject); // character selected (StormbreakerIsland etc.) — done
            // OpeningScreen: survive and wait for CharacterCustomization
        }

        private IEnumerator EnforceDisabledCharacters()
        {
            yield return null; // let CharacterButton.Start() run

            CharacterButton[] buttons = FindObjectsOfType<CharacterButton>();
            CharacterDisableManager.RegisterTotal(buttons.Length);

            // Edge case: total was unknown until now (e.g. only 1 character died first run)
            if (CharacterDisableManager.AllDisabled())
            {
                GameOverHandler.Show();
                Destroy(gameObject);
                yield break;
            }

            foreach (CharacterButton cb in buttons)
            {
                if (!CharacterDisableManager.IsDisabled(cb.characterName)) continue;
                DisableButtonVisually(cb);
            }
        }

        private static void DisableButtonVisually(CharacterButton cb)
        {
            Button btn = cb.GetComponent<Button>();
            if (btn != null) btn.interactable = false;

            foreach (Image img in cb.GetComponentsInChildren<Image>(true))
                img.color = new Color(0.3f, 0.3f, 0.3f, 1f);

            var labelGO = new GameObject("DefeatedLabel");
            labelGO.transform.SetParent(cb.transform, false);

            var tmp = labelGO.AddComponent<TextMeshProUGUI>();
            tmp.text      = "DEFEATED";
            tmp.fontSize  = 20;
            tmp.color     = Color.red;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;

            RectTransform rt = labelGO.GetComponent<RectTransform>();
            rt.anchorMin        = Vector2.zero;
            rt.anchorMax        = Vector2.one;
            rt.sizeDelta        = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
        }
    }
}
