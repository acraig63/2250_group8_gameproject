using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    /// <summary>
    /// Attach this to your Canvas GameObject.
    /// Displays the minimap RenderTexture in a small panel at the top-left of the screen.
    /// Shows a yellow diamond for the player and red dots for each NPC.
    ///
    /// Setup steps (do once in Unity Editor):
    ///   1. Create a Canvas in your scene if one doesn't exist
    ///      (GameObject → UI → Canvas). Set "Render Mode" to "Screen Space - Overlay"
    ///   2. Attach this script to the Canvas GameObject
    ///   3. Drag your MinimapRenderTexture asset into the "minimapRenderTexture" field
    ///   4. Drag the MinimapCamera into the "minimapCamera" field
    ///   5. Press Play — the minimap panel will build itself automatically
    /// </summary>
    public class MinimapUI : MonoBehaviour
    {
        public static MinimapUI Instance;

        [Header("Render Texture")]
        [Tooltip("The same RenderTexture assigned to your MinimapCamera's Camera component.")]
        public RenderTexture minimapRenderTexture;

        [Header("Camera Reference")]
        [Tooltip("Drag the MinimapCamera GameObject here. Used to project world positions onto the minimap.")]
        public Camera minimapCamera;

        [Header("Minimap Appearance")]
        [Tooltip("Size of the minimap panel in pixels.")]
        public int minimapSize = 200;

        [Tooltip("Thickness of the border around the minimap panel.")]
        public int borderThickness = 3;

        [Tooltip("Padding from the top-left corner of the screen.")]
        public int screenPadding = 15;

        [Tooltip("Color of the border around the minimap.")]
        public Color borderColor = new Color(0.6f, 0.4f, 0.1f, 1f); // Pirate gold

        [Header("Player Indicator")]
        [Tooltip("Color of the player diamond on the minimap.")]
        public Color playerDotColor = Color.yellow;

        [Tooltip("Size of the player diamond in pixels.")]
        public int playerDotSize = 10;

        [Header("NPC Indicators")]
        [Tooltip("Color of NPC dots on the minimap.")]
        public Color npcDotColor = Color.red;

        [Tooltip("Size of each NPC dot in pixels.")]
        public int npcDotSize = 6;

        // Internal UI references — built at runtime so no manual wiring needed
        private RectTransform _borderPanel;
        private RectTransform _minimapPanel;
        private RawImage _minimapImage;
        private RectTransform _playerDot;

        // Player tracking — lazy lookup so scene transitions work
        private Transform _playerTransform;

        // NPC tracking: pairs a world-space Transform with its minimap dot rect
        private readonly List<(Transform npc, RectTransform dot)> _npcTrackers
            = new List<(Transform, RectTransform)>();
        private float _npcRefreshTimer;
        private const float NPC_REFRESH_INTERVAL = 2f;

        void Awake()
        {
            // Minimap is only valid in SmugglersIsland — destroy immediately in any other scene
            // so this canvas (which carries a GraphicRaycaster) cannot block input elsewhere.
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "SmugglersIsland")
            {
                Destroy(gameObject);
                return;
            }

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            BuildMinimapUI();
            RefreshNPCTrackers();
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

            if (minimapRenderTexture != null)
            {
                _minimapImage.texture = minimapRenderTexture;
            }
            else
            {
                _minimapImage.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);
                Debug.LogWarning("MinimapUI: No RenderTexture assigned. " +
                                 "Drag MinimapRenderTexture into the minimapRenderTexture field.");
            }

            // ------------------------------------------------------------------
            // 3. Player dot — yellow diamond (square rotated 45°)
            // ------------------------------------------------------------------
            GameObject dotObj = new GameObject("PlayerDot");
            dotObj.transform.SetParent(minimapObj.transform, false);

            _playerDot = dotObj.AddComponent<RectTransform>();
            Image dotImage = dotObj.AddComponent<Image>();
            dotImage.color = playerDotColor;

            _playerDot.sizeDelta = new Vector2(playerDotSize, playerDotSize);
            _playerDot.anchorMin = new Vector2(0.5f, 0.5f);
            _playerDot.anchorMax = new Vector2(0.5f, 0.5f);
            _playerDot.pivot     = new Vector2(0.5f, 0.5f);
            _playerDot.anchoredPosition = Vector2.zero;

            // Rotate 45° so the square looks like a diamond
            dotObj.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);

            Debug.Log("MinimapUI: Minimap panel built successfully.");
        }

        void Update()
        {
            // Lazy player lookup — handles scene transitions without re-running Start
            if (_playerTransform == null)
            {
                GameObject playerObj = GameObject.FindWithTag("Player");
                if (playerObj != null)
                    _playerTransform = playerObj.transform;
            }

            UpdatePlayerDot();

            // Periodically re-scan for new or destroyed NPC GameObjects
            _npcRefreshTimer += Time.deltaTime;
            if (_npcRefreshTimer >= NPC_REFRESH_INTERVAL)
            {
                _npcRefreshTimer = 0f;
                RefreshNPCTrackers();
            }

            UpdateNPCDots();
        }

        private void UpdatePlayerDot()
        {
            if (_playerDot == null || minimapCamera == null || _playerTransform == null)
                return;

            Vector3 vp = minimapCamera.WorldToViewportPoint(_playerTransform.position);
            bool inView = vp.z > 0f && vp.x >= 0f && vp.x <= 1f
                                     && vp.y >= 0f && vp.y <= 1f;
            _playerDot.gameObject.SetActive(inView);

            if (inView)
            {
                _playerDot.anchoredPosition = new Vector2(
                    (vp.x - 0.5f) * minimapSize,
                    (vp.y - 0.5f) * minimapSize
                );
            }
        }

        /// <summary>
        /// Destroys all existing NPC dots and rebuilds them from every active NPC.
        /// </summary>
        private void RefreshNPCTrackers()
        {
            foreach (var (_, dot) in _npcTrackers)
                if (dot != null) Destroy(dot.gameObject);
            _npcTrackers.Clear();

            if (_minimapPanel == null) return;

            NPCDialogueTrigger[] npcTriggers = FindObjectsOfType<NPCDialogueTrigger>();
            foreach (NPCDialogueTrigger trigger in npcTriggers)
            {
                GameObject npcGO = trigger.gameObject;
                GameObject dotObj = new GameObject("NPCDot_" + npcGO.name);
                dotObj.transform.SetParent(_minimapPanel, false);

                RectTransform rt = dotObj.AddComponent<RectTransform>();
                Image img        = dotObj.AddComponent<Image>();
                img.color        = npcDotColor;

                rt.sizeDelta        = new Vector2(npcDotSize, npcDotSize);
                rt.anchorMin        = new Vector2(0.5f, 0.5f);
                rt.anchorMax        = new Vector2(0.5f, 0.5f);
                rt.pivot            = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;

                _npcTrackers.Add((npcGO.transform, rt));
            }
        }

        /// <summary>
        /// Projects each tracked NPC's world position onto the minimap panel every frame.
        /// </summary>
        private void UpdateNPCDots()
        {
            if (minimapCamera == null) return;

            for (int i = _npcTrackers.Count - 1; i >= 0; i--)
            {
                var (npcTransform, dot) = _npcTrackers[i];

                if (npcTransform == null)
                {
                    if (dot != null) Destroy(dot.gameObject);
                    _npcTrackers.RemoveAt(i);
                    continue;
                }

                Vector3 vp = minimapCamera.WorldToViewportPoint(npcTransform.position);
                bool inView = vp.z > 0f && vp.x >= 0f && vp.x <= 1f
                                        && vp.y >= 0f && vp.y <= 1f;
                dot.gameObject.SetActive(inView);

                if (inView)
                {
                    dot.anchoredPosition = new Vector2(
                        (vp.x - 0.5f) * minimapSize,
                        (vp.y - 0.5f) * minimapSize
                    );
                }
            }
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
