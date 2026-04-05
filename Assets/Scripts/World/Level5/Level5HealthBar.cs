using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace DefaultNamespace
{
    /// <summary>
    /// Persistent health bar for Level 5 (Blackwater) scenes.
    /// Mirrors HealthBarUI but survives scene loads via DontDestroyOnLoad.
    /// Reads PlayerController.GetHealth() / MaxHealth each frame.
    /// Hides itself during Battle scenes to avoid HUD overlap.
    /// </summary>
    public class Level5HealthBar : MonoBehaviour
    {
        private static Level5HealthBar _instance;

        // UI references built at runtime
        private Slider          _slider;
        private Image           _fillImage;
        private TextMeshProUGUI _healthText;

        // Cached player reference – refreshed when null (scene changes)
        private PlayerController _player;

        // ── Public entry point ─────────────────────────────────────────────────

        public static void EnsureExists()
        {
            if (_instance != null) return;

            GameObject go = new GameObject("Level5HealthBarCanvas");
            _instance = go.AddComponent<Level5HealthBar>();
            DontDestroyOnLoad(go);
            _instance.BuildUI(go);

            SceneManager.sceneLoaded += _instance.OnSceneLoaded;
        }

        // ── Lifecycle ──────────────────────────────────────────────────────────

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (_instance == this) _instance = null;
        }

        private void Update()
        {
            if (_player == null)
                _player = FindObjectOfType<PlayerController>();

            if (_player == null) return;

            int   current = _player.GetHealth();
            int   max     = _player.MaxHealth;
            float ratio   = max > 0 ? (float)current / max : 0f;

            _slider.value = ratio;

            if (_fillImage != null)
            {
                if (ratio > 0.6f)       _fillImage.color = new Color(0.2f, 0.8f, 0.2f); // green
                else if (ratio > 0.3f)  _fillImage.color = new Color(1.0f, 0.8f, 0.0f); // yellow
                else                    _fillImage.color = new Color(0.9f, 0.2f, 0.2f); // red
            }

            if (_healthText != null)
                _healthText.text = $"HP: {current} / {max}";
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Invalidate cached player reference on each scene load
            _player = null;

            bool isBattle = scene.name == "Battle" || scene.name == "pirateBattleScene";
            gameObject.SetActive(!isBattle);
        }

        // ── UI construction ────────────────────────────────────────────────────

        private void BuildUI(GameObject root)
        {
            // Canvas
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();

            // Slider container (top-right, matching HealthBarUI layout in SmugglersIsland)
            GameObject sliderGO = new GameObject("HealthSlider");
            sliderGO.transform.SetParent(root.transform, false);

            RectTransform sliderRect = sliderGO.AddComponent<RectTransform>();
            sliderRect.anchorMin        = new Vector2(1f, 1f);
            sliderRect.anchorMax        = new Vector2(1f, 1f);
            sliderRect.pivot            = new Vector2(0.5f, 0.5f);
            sliderRect.anchoredPosition = new Vector2(-84.9f, -10f);
            sliderRect.sizeDelta        = new Vector2(160f, 20f);

            // Background image
            Image bgImage = sliderGO.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            // Fill area
            GameObject fillAreaGO = new GameObject("Fill Area");
            fillAreaGO.transform.SetParent(sliderGO.transform, false);
            RectTransform fillAreaRect = fillAreaGO.AddComponent<RectTransform>();
            fillAreaRect.anchorMin        = Vector2.zero;
            fillAreaRect.anchorMax        = Vector2.one;
            fillAreaRect.sizeDelta        = new Vector2(-10f, -4f);
            fillAreaRect.anchoredPosition = new Vector2(-5f, 0f);

            GameObject fillGO = new GameObject("Fill");
            fillGO.transform.SetParent(fillAreaGO.transform, false);
            _fillImage = fillGO.AddComponent<Image>();
            _fillImage.color = new Color(0.2f, 0.8f, 0.2f); // starts green
            RectTransform fillRect = fillGO.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;

            // Wire Slider component
            _slider = sliderGO.AddComponent<Slider>();
            _slider.fillRect      = fillRect;
            _slider.direction     = Slider.Direction.LeftToRight;
            _slider.minValue      = 0f;
            _slider.maxValue      = 1f;
            _slider.value         = 1f;
            _slider.interactable  = false;

            // HP text label (overlaid on the slider)
            GameObject textGO = new GameObject("HPText");
            textGO.transform.SetParent(sliderGO.transform, false);
            _healthText = textGO.AddComponent<TextMeshProUGUI>();
            _healthText.text      = "HP: 100 / 100";
            _healthText.fontSize  = 12;
            _healthText.color     = Color.white;
            _healthText.alignment = TextAlignmentOptions.Center;
            RectTransform textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin        = Vector2.zero;
            textRect.anchorMax        = Vector2.one;
            textRect.sizeDelta        = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
        }
    }
}
