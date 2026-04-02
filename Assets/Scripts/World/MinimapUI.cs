using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    /// <summary>
    /// Attach to the MinimapCanvas GameObject (named exactly "MinimapCanvas").
    /// Displays the minimap RenderTexture in a panel at the top-left of the screen,
    /// with a yellow player dot (always centred — the minimap camera follows the player)
    /// and red dots for each NPC.
    ///
    /// Start the GameObject INACTIVE — StoryIntroUI activates it after the intro.
    ///
    /// Setup (do once in Unity Editor):
    ///   1. Create a Canvas, name it "MinimapCanvas"
    ///   2. Set Render Mode: Screen Space - Overlay, Sort Order: 20
    ///   3. Attach this script
    ///   4. Drag MinimapRenderTexture into "minimapRenderTexture"
    ///   5. Drag the MinimapCamera GameObject into "minimapCamera"
    ///   6. Set the Canvas GameObject inactive in the Hierarchy
    /// </summary>
    public class MinimapUI : MonoBehaviour
    {
        public static MinimapUI Instance;

        [Header("Render Texture")]
        public RenderTexture minimapRenderTexture;

        [Header("Camera Reference")]
        [Tooltip("Drag the MinimapCamera GameObject here.")]
        public Camera minimapCamera;

        [Header("Appearance")]
        public int   minimapSize     = 200;
        public int   borderThickness = 3;
        public int   screenPadding   = 15;
        public Color borderColor     = new Color(0.6f, 0.4f, 0.1f, 1f);

        [Header("Player Dot")]
        public Color playerDotColor = Color.yellow;
        public int   playerDotSize  = 10;

        [Header("NPC Dots")]
        public Color npcDotColor = Color.red;
        public int   npcDotSize  = 6;

        private RectTransform _borderPanel;
        private RectTransform _minimapPanel;
        private RawImage      _minimapImage;
        private RectTransform _playerDot;

        private Transform _playerTransform;

        private readonly List<(Transform npc, RectTransform dot)> _npcTrackers
            = new List<(Transform, RectTransform)>();
        private float _npcRefreshTimer;
        private const float NPC_REFRESH_INTERVAL = 2f;

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
            Debug.Log(">>> NEW CODE RUNNING: MinimapUI._initialized = " + _initialized
                      + " | scene = " + SceneManager.GetActiveScene().name);
        }

        void Start()
        {
            if (!_initialized)
            {
                Debug.Log(">>> MinimapUI.Start() skipped (not initialized) | scene = "
                          + SceneManager.GetActiveScene().name);
                return;
            }
            BuildMinimapUI();

            // Always pull the RenderTexture from MinimapCamera — it creates a
            // fresh one at runtime in Awake(). This overrides any stale Inspector
            // reference so the UI always shows what the camera is rendering.
            if (MinimapCamera.Instance != null)
            {
                minimapRenderTexture = MinimapCamera.Instance.renderTexture;
                if (_minimapImage != null)
                    _minimapImage.texture = minimapRenderTexture;
            }
            else
            {
                Debug.LogWarning("MinimapUI: MinimapCamera not found — ensure it is activated before MinimapCanvas.");
            }

            RefreshNPCTrackers();
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
            _borderPanel.sizeDelta          = new Vector2(total, total);
            _borderPanel.anchorMin          = new Vector2(0, 1);
            _borderPanel.anchorMax          = new Vector2(0, 1);
            _borderPanel.pivot              = new Vector2(0, 1);
            _borderPanel.anchoredPosition   = new Vector2(screenPadding, -screenPadding);

            // --- Minimap display ---
            GameObject mapObj = new GameObject("MinimapDisplay");
            mapObj.transform.SetParent(borderObj.transform, false);
            _minimapPanel              = mapObj.AddComponent<RectTransform>();
            _minimapImage              = mapObj.AddComponent<RawImage>();
            _minimapPanel.sizeDelta    = new Vector2(minimapSize, minimapSize);
            _minimapPanel.anchorMin    = new Vector2(0.5f, 0.5f);
            _minimapPanel.anchorMax    = new Vector2(0.5f, 0.5f);
            _minimapPanel.pivot        = new Vector2(0.5f, 0.5f);
            _minimapPanel.anchoredPosition = Vector2.zero;

            if (minimapRenderTexture != null)
                _minimapImage.texture = minimapRenderTexture;
            else
            {
                _minimapImage.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);
                Debug.LogWarning("MinimapUI: no RenderTexture assigned.");
            }

            // --- Player dot (yellow diamond, always centred) ---
            GameObject dotObj = new GameObject("PlayerDot");
            dotObj.transform.SetParent(mapObj.transform, false);
            _playerDot              = dotObj.AddComponent<RectTransform>();
            dotObj.AddComponent<Image>().color = playerDotColor;
            _playerDot.sizeDelta    = new Vector2(playerDotSize, playerDotSize);
            _playerDot.anchorMin    = new Vector2(0.5f, 0.5f);
            _playerDot.anchorMax    = new Vector2(0.5f, 0.5f);
            _playerDot.pivot        = new Vector2(0.5f, 0.5f);
            _playerDot.anchoredPosition = Vector2.zero;
            dotObj.transform.localRotation = Quaternion.Euler(0f, 0f, 45f); // diamond shape
        }

        void Update()
        {
            if (!_initialized) return;
            if (_playerTransform == null)
            {
                GameObject p = GameObject.FindWithTag("Player");
                if (p != null) _playerTransform = p.transform;
            }

            _npcRefreshTimer += Time.deltaTime;
            if (_npcRefreshTimer >= NPC_REFRESH_INTERVAL)
            {
                _npcRefreshTimer = 0f;
                RefreshNPCTrackers();
            }

            UpdateNPCDots();
        }

        private void RefreshNPCTrackers()
        {
            foreach (var (_, dot) in _npcTrackers)
                if (dot != null) Destroy(dot.gameObject);
            _npcTrackers.Clear();

            if (_minimapPanel == null) return;

            // TODO: NPCDialogueTrigger has no enemy/friendly distinction — all dots are red.
            // When an IsEnemy flag or EnemyNPC component is added, change dot color:
            //   enemy = Color.red, friendly = Color.green
            foreach (NPCDialogueTrigger trigger in FindObjectsOfType<NPCDialogueTrigger>())
            {
                GameObject npcGO = trigger.gameObject;
                GameObject dotObj = new GameObject("NPCDot_" + npcGO.name);
                dotObj.transform.SetParent(_minimapPanel, false);

                RectTransform rt = dotObj.AddComponent<RectTransform>();
                dotObj.AddComponent<Image>().color = npcDotColor;
                rt.sizeDelta          = new Vector2(npcDotSize, npcDotSize);
                rt.anchorMin          = new Vector2(0.5f, 0.5f);
                rt.anchorMax          = new Vector2(0.5f, 0.5f);
                rt.pivot              = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition   = Vector2.zero;
                _npcTrackers.Add((npcGO.transform, rt));
            }
        }

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
                    dot.anchoredPosition = new Vector2(
                        (vp.x - 0.5f) * minimapSize,
                        (vp.y - 0.5f) * minimapSize);
            }
        }

        public void SetVisible(bool visible)
        {
            if (_borderPanel != null)
                _borderPanel.gameObject.SetActive(visible);
        }
    }
}
