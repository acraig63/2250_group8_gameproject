using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace DefaultNamespace
{
    // Displays a full-screen Game Over overlay when all characters have been disabled.
    // Auto-returns to the opening screen after 10 seconds.
    public static class GameOverHandler
    {
        public static void Show()
        {
            var go = new GameObject("GameOverHost");
            Object.DontDestroyOnLoad(go);
            go.AddComponent<GameOverHost>();
        }

        private class GameOverHost : MonoBehaviour
        {
            private void Awake() => BuildUI();

            private void BuildUI()
            {
                var canvasGO = new GameObject("GameOverCanvas");
                canvasGO.transform.SetParent(transform, false);

                var canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 200;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();

                // Dark overlay
                var bgGO = new GameObject("Background");
                bgGO.transform.SetParent(canvasGO.transform, false);
                bgGO.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.85f);
                StretchFull(bgGO);

                // "GAME OVER" title
                var titleGO = CreateText(canvasGO, "Title", "GAME OVER",
                    72, new Color(0.8f, 0f, 0f), FontStyles.Bold);
                AnchorBand(titleGO, 0.6f, 0.8f);

                // Subtitle
                var subGO = CreateText(canvasGO, "Subtitle",
                    "All crew members have fallen!", 36, Color.white, FontStyles.Normal);
                AnchorBand(subGO, 0.45f, 0.6f);

                // Return button
                var btnGO = new GameObject("ReturnButton");
                btnGO.transform.SetParent(canvasGO.transform, false);
                var btnImg = btnGO.AddComponent<Image>();
                btnImg.color = new Color(0.55f, 0f, 0f);
                var btn = btnGO.AddComponent<Button>();
                btn.targetGraphic = btnImg;

                RectTransform btnRT = btnGO.GetComponent<RectTransform>();
                btnRT.anchorMin        = new Vector2(0.35f, 0.3f);
                btnRT.anchorMax        = new Vector2(0.65f, 0.42f);
                btnRT.sizeDelta        = Vector2.zero;
                btnRT.anchoredPosition = Vector2.zero;

                var btnTextGO = CreateText(btnGO, "BtnText",
                    "Return to Main Menu", 24, Color.white, FontStyles.Normal);
                StretchFull(btnTextGO);

                btn.onClick.AddListener(ReturnToMenu);
                StartCoroutine(AutoReturn(10f));
            }

            private void ReturnToMenu()
            {
                CharacterDisableManager.ResetAll();
                SceneManager.LoadScene("OpeningScreen");
                Destroy(gameObject);
            }

            private IEnumerator AutoReturn(float delay)
            {
                yield return new WaitForSeconds(delay);
                ReturnToMenu();
            }

            private static GameObject CreateText(GameObject parent, string name,
                string text, float size, Color color, FontStyles style)
            {
                var go = new GameObject(name);
                go.transform.SetParent(parent.transform, false);
                var tmp = go.AddComponent<TextMeshProUGUI>();
                tmp.text      = text;
                tmp.fontSize  = size;
                tmp.color     = color;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.fontStyle = style;
                return go;
            }

            private static void StretchFull(GameObject go)
            {
                var rt = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
                rt.anchorMin        = Vector2.zero;
                rt.anchorMax        = Vector2.one;
                rt.sizeDelta        = Vector2.zero;
                rt.anchoredPosition = Vector2.zero;
            }

            private static void AnchorBand(GameObject go, float yMin, float yMax)
            {
                var rt = go.GetComponent<RectTransform>();
                rt.anchorMin        = new Vector2(0f, yMin);
                rt.anchorMax        = new Vector2(1f, yMax);
                rt.sizeDelta        = Vector2.zero;
                rt.anchoredPosition = Vector2.zero;
            }
        }
    }
}
