using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    /// <summary>
    /// Attach to the MinimapCanvas GameObject (named exactly "MinimapCanvas").
    /// Displays the MinimapCamera's RenderTexture in a gold-bordered panel at the
    /// top-left of the screen. No overlay dots — the camera view is the map.
    ///
    /// Start the GameObject INACTIVE — StoryIntroUI activates it after the intro.
    /// </summary>
    public class MinimapUI : MonoBehaviour
    {
        public static MinimapUI Instance;

        [Header("Render Texture")]
        public RenderTexture minimapRenderTexture;

        [Header("Appearance")]
        public int   minimapSize     = 200;
        public int   borderThickness = 3;
        public int   screenPadding   = 15;
        public Color borderColor     = new Color(0.6f, 0.4f, 0.1f, 1f);

        private RectTransform _borderPanel;
        private RawImage      _minimapImage;

        // Destroy(gameObject) is deferred to end-of-frame — Start() still runs
        // on the same frame. This flag gates Start/Update so they are no-ops
        // when Awake() determined this instance should be destroyed.
        private bool _initialized;

        void Awake()
        {
            // Only valid in SmugglersIsland — destroy immediately in any other scene
            // so this Canvas's GraphicRaycaster cannot block input elsewhere.
            if (SceneManager.GetActiveScene().name != "SmugglersIsland")
            {
                Destroy(gameObject);
                return;
            }

            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            _initialized = true;
        }

        void Start()
        {
            if (!_initialized) return;

            // Grab the runtime RT from MinimapCamera BEFORE building the UI so
            // BuildMinimapUI() receives a valid texture instead of the stale
            // Inspector reference (which may be null if the asset was recreated).
            if (MinimapCamera.Instance != null && MinimapCamera.Instance.renderTexture != null)
                minimapRenderTexture = MinimapCamera.Instance.renderTexture;

            BuildMinimapUI();
        }

        private void BuildMinimapUI()
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("MinimapUI: must be attached to a Canvas GameObject.");
                return;
            }

            // --- Border panel (gold frame) ---
            GameObject borderObj = new GameObject("MinimapBorder");
            borderObj.transform.SetParent(transform, false);
            _borderPanel               = borderObj.AddComponent<RectTransform>();
            borderObj.AddComponent<Image>().color = borderColor;
            int total = minimapSize + borderThickness * 2;
            _borderPanel.sizeDelta        = new Vector2(total, total);
            _borderPanel.anchorMin        = new Vector2(0, 1);
            _borderPanel.anchorMax        = new Vector2(0, 1);
            _borderPanel.pivot            = new Vector2(0, 1);
            _borderPanel.anchoredPosition = new Vector2(screenPadding, -screenPadding);

            // --- Minimap display ---
            GameObject mapObj = new GameObject("MinimapDisplay");
            mapObj.transform.SetParent(borderObj.transform, false);
            RectTransform mapRT    = mapObj.AddComponent<RectTransform>();
            _minimapImage          = mapObj.AddComponent<RawImage>();
            mapRT.sizeDelta        = new Vector2(minimapSize, minimapSize);
            mapRT.anchorMin        = new Vector2(0.5f, 0.5f);
            mapRT.anchorMax        = new Vector2(0.5f, 0.5f);
            mapRT.pivot            = new Vector2(0.5f, 0.5f);
            mapRT.anchoredPosition = Vector2.zero;

            if (minimapRenderTexture != null)
                _minimapImage.texture = minimapRenderTexture;
            else
                Debug.LogWarning("MinimapUI: no RenderTexture assigned.");
        }

        void Update()
        {
            if (!_initialized) return;

            // Self-heal: if the RawImage still has no texture (RT wasn't ready
            // during Start), keep trying until MinimapCamera provides one.
            if (_minimapImage != null && _minimapImage.texture == null
                && MinimapCamera.Instance != null
                && MinimapCamera.Instance.renderTexture != null)
            {
                minimapRenderTexture  = MinimapCamera.Instance.renderTexture;
                _minimapImage.texture = minimapRenderTexture;
            }
        }

        public void SetVisible(bool visible)
        {
            if (_borderPanel != null)
                _borderPanel.gameObject.SetActive(visible);
        }
    }
}
