using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace DefaultNamespace
{
    // Replicates the exact health bar structure used in TwistedGardens:
    // HealthBarUI component + Slider on a HealthBar object, with a sibling HealthBar Text label.
    public class Level5ProgressionHealthBar : MonoBehaviour
    {
        private static Level5ProgressionHealthBar _instance;

        public static void EnsureExists()
        {
            if (_instance != null) return;
            GameObject go = new GameObject("Level5HealthBarCanvas");
            _instance = go.AddComponent<Level5ProgressionHealthBar>();
            DontDestroyOnLoad(go);
            _instance.BuildUI(go);
            SceneManager.sceneLoaded += _instance.OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (_instance == this) _instance = null;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            bool isBattle = scene.name == "Battle" || scene.name == "pirateBattleScene";
            gameObject.SetActive(!isBattle);
        }

        private void BuildUI(GameObject root)
        {
            // Root canvas — same sortingOrder as TwistedGardens (10)
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();

            // ── HealthBar Text ──────────────────────────────────────────────
            // Sibling of the slider at canvas level, anchored top-right.
            // Matches TwistedGardens: anchorMin/Max (1,1), pos (-5,-15),
            // size (200,50), pivot (1,1), right-aligned, fontSize 12, white.
            GameObject textGO = new GameObject("HealthBar Text");
            textGO.transform.SetParent(root.transform, false);
            TextMeshProUGUI healthText = textGO.AddComponent<TextMeshProUGUI>();
            healthText.text      = "HP: 100 / 100";
            healthText.fontSize  = 12;
            healthText.color     = Color.white;
            healthText.alignment = TextAlignmentOptions.Right;
            RectTransform textRT = textGO.GetComponent<RectTransform>();
            textRT.anchorMin        = new Vector2(1f, 1f);
            textRT.anchorMax        = new Vector2(1f, 1f);
            textRT.pivot            = new Vector2(1f, 1f);
            textRT.anchoredPosition = new Vector2(-5f, -15f);
            textRT.sizeDelta        = new Vector2(200f, 50f);

            // ── HealthBar (Slider + HealthBarUI) ───────────────────────────
            // Matches TwistedGardens: anchorMin/Max (1,1), pos (-84.9,-10),
            // size (160,20), pivot (0.5,0.5). No background Image on this object.
            GameObject sliderGO = new GameObject("HealthBar");
            sliderGO.transform.SetParent(root.transform, false);
            RectTransform sliderRT = sliderGO.AddComponent<RectTransform>();
            sliderRT.anchorMin        = new Vector2(1f, 1f);
            sliderRT.anchorMax        = new Vector2(1f, 1f);
            sliderRT.pivot            = new Vector2(0.5f, 0.5f);
            sliderRT.anchoredPosition = new Vector2(-84.9f, -10f);
            sliderRT.sizeDelta        = new Vector2(160f, 20f);

            // Background child — white Image, anchors (0,0.25)/(1,0.75)
            GameObject bgGO = new GameObject("Background");
            bgGO.transform.SetParent(sliderGO.transform, false);
            Image bgImage = bgGO.AddComponent<Image>();
            bgImage.color = Color.white;
            RectTransform bgRT = bgGO.GetComponent<RectTransform>();
            bgRT.anchorMin        = new Vector2(0f, 0.25f);
            bgRT.anchorMax        = new Vector2(1f, 0.75f);
            bgRT.pivot            = new Vector2(0.5f, 0.5f);
            bgRT.anchoredPosition = Vector2.zero;
            bgRT.sizeDelta        = Vector2.zero;

            // Fill Area child — anchors (0,0.25)/(1,0.75)
            GameObject fillAreaGO = new GameObject("Fill Area");
            fillAreaGO.transform.SetParent(sliderGO.transform, false);
            RectTransform fillAreaRT = fillAreaGO.AddComponent<RectTransform>();
            fillAreaRT.anchorMin        = new Vector2(0f, 0.25f);
            fillAreaRT.anchorMax        = new Vector2(1f, 0.75f);
            fillAreaRT.pivot            = new Vector2(0.5f, 0.5f);
            fillAreaRT.anchoredPosition = Vector2.zero;
            fillAreaRT.sizeDelta        = Vector2.zero;

            // Fill child — bright green matching TwistedGardens exactly, anchors (0,0)/(0,0)
            GameObject fillGO = new GameObject("Fill");
            fillGO.transform.SetParent(fillAreaGO.transform, false);
            Image fillImage = fillGO.AddComponent<Image>();
            fillImage.color = new Color(0f, 1f, 0.09392524f, 1f);
            RectTransform fillRT = fillGO.GetComponent<RectTransform>();
            fillRT.anchorMin        = new Vector2(0f, 0f);
            fillRT.anchorMax        = new Vector2(0f, 0f);
            fillRT.pivot            = new Vector2(0.5f, 0.5f);
            fillRT.anchoredPosition = Vector2.zero;
            fillRT.sizeDelta        = Vector2.zero;

            // Slider on HealthBar — same settings as TwistedGardens
            Slider slider = sliderGO.AddComponent<Slider>();
            slider.fillRect     = fillRT;
            slider.direction    = Slider.Direction.LeftToRight;
            slider.minValue     = 0f;
            slider.maxValue     = 1f;
            slider.value        = 1f;
            slider.interactable = false;

            // HealthBarUI drives the update — same component TwistedGardens uses
            HealthBarUI hbUI = sliderGO.AddComponent<HealthBarUI>();
            hbUI.slider     = slider;
            hbUI.healthText = healthText;
        }
    }
}
