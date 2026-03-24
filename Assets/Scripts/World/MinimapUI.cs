using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    /// <summary>
    /// Attach this to your Canvas GameObject.
    /// Displays the minimap RenderTexture in a small panel at the top-left of the screen.
    /// Also shows a small dot representing the player's position on the minimap.
    ///
    /// Setup steps (do once in Unity Editor):
    ///   1. Create a Canvas in your scene if one doesn't exist
    ///      (GameObject → UI → Canvas). Set "Render Mode" to "Screen Space - Overlay"
    ///   2. Attach this script to the Canvas GameObject
    ///   3. Drag your MinimapRenderTexture asset into the "minimapRenderTexture" field
    ///   4. Drag your Player GameObject into the "playerTransform" field
    ///   5. Press Play — the minimap panel will build itself automatically
    /// </summary>
    public class MinimapUI : MonoBehaviour
    {
        [Header("Render Texture")]
        [Tooltip("The same RenderTexture assigned to your MinimapCamera's Camera component.")]
        public RenderTexture minimapRenderTexture;

        [Header("Player Reference")]
        [Tooltip("Drag your Player GameObject here for the player dot indicator.")]
        public Transform playerTransform;

        [Header("Minimap Appearance")]
        [Tooltip("Size of the minimap panel in pixels.")]
        public int minimapSize = 200;

        [Tooltip("Thickness of the border around the minimap panel.")]
        public int borderThickness = 3;

        [Tooltip("Padding from the top-left corner of the screen.")]
        public int screenPadding = 15;

        [Tooltip("Color of the border around the minimap.")]
        public Color borderColor = new Color(0.6f, 0.4f, 0.1f, 1f); // Pirate gold

        [Tooltip("Color of the player dot on the minimap.")]
        public Color playerDotColor = Color.red;

        [Tooltip("Size of the player dot in pixels.")]
        public int playerDotSize = 8;

        // Internal UI references — built at runtime so no manual wiring needed
        private RectTransform _borderPanel;
        private RectTransform _minimapPanel;
        private RawImage _minimapImage;
        private RectTransform _playerDot;

        void Start()
        {
            BuildMinimapUI();
        }

        /// <summary>
        /// Constructs all minimap UI elements at runtime.
        /// Called once on Start — no prefabs or manual scene wiring required.
        /// </summary>
        private void BuildMinimapUI()
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("MinimapUI: This script must be attached to a Canvas GameObject.");
                return;
            }

            // ------------------------------------------------------------------
            // 1. Border panel — slightly larger than the minimap, gives a frame
            // ------------------------------------------------------------------
            GameObject borderObj = new GameObject("MinimapBorder");
            borderObj.transform.SetParent(transform, false);

            _borderPanel = borderObj.AddComponent<RectTransform>();
            Image borderImage = borderObj.AddComponent<Image>();
            borderImage.color = borderColor;

            int totalSize = minimapSize + (borderThickness * 2);
            _borderPanel.sizeDelta = new Vector2(totalSize, totalSize);

            // Anchor to top-left corner
            _borderPanel.anchorMin = new Vector2(0, 1);
            _borderPanel.anchorMax = new Vector2(0, 1);
            _borderPanel.pivot     = new Vector2(0, 1);
            _borderPanel.anchoredPosition = new Vector2(screenPadding, -screenPadding);

            // ------------------------------------------------------------------
            // 2. Minimap display panel — child of border, inset by borderThickness
            // ------------------------------------------------------------------
            GameObject minimapObj = new GameObject("MinimapDisplay");
            minimapObj.transform.SetParent(borderObj.transform, false);

            _minimapPanel = minimapObj.AddComponent<RectTransform>();
            _minimapImage = minimapObj.AddComponent<RawImage>();

            _minimapPanel.sizeDelta = new Vector2(minimapSize, minimapSize);
            _minimapPanel.anchorMin = new Vector2(0.5f, 0.5f);
            _minimapPanel.anchorMax = new Vector2(0.5f, 0.5f);
            _minimapPanel.pivot     = new Vector2(0.5f, 0.5f);
            _minimapPanel.anchoredPosition = Vector2.zero;

            // Hook up the render texture if one was provided
            if (minimapRenderTexture != null)
            {
                _minimapImage.texture = minimapRenderTexture;
            }
            else
            {
                // Fallback: dark background so the panel is at least visible
                _minimapImage.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);
                Debug.LogWarning("MinimapUI: No RenderTexture assigned. " +
                                 "Drag MinimapRenderTexture into the minimapRenderTexture field.");
            }

            // ------------------------------------------------------------------
            // 3. Player dot — small circle centered on the minimap
            // ------------------------------------------------------------------
            GameObject dotObj = new GameObject("PlayerDot");
            dotObj.transform.SetParent(minimapObj.transform, false);

            _playerDot = dotObj.AddComponent<RectTransform>();
            Image dotImage = dotObj.AddComponent<Image>();
            dotImage.color = playerDotColor;

            _playerDot.sizeDelta = new Vector2(playerDotSize, playerDotSize);

            // Since the minimap camera always centers on the player, the dot
            // always stays in the middle of the minimap panel
            _playerDot.anchorMin = new Vector2(0.5f, 0.5f);
            _playerDot.anchorMax = new Vector2(0.5f, 0.5f);
            _playerDot.pivot     = new Vector2(0.5f, 0.5f);
            _playerDot.anchoredPosition = Vector2.zero;

            Debug.Log("MinimapUI: Minimap panel built successfully.");
        }

        void Update()
        {
            // Player dot always stays centered since the camera follows the player —
            // nothing to update here unless you later want to add other map markers
        }

        /// <summary>
        /// Call this to show or hide the minimap (e.g., when opening the full map screen).
        /// </summary>
        public void SetVisible(bool visible)
        {
            if (_borderPanel != null)
                _borderPanel.gameObject.SetActive(visible);
        }
    }
}
