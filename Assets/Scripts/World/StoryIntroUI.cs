using UnityEngine;
using UnityEngine.UI;
<<<<<<< HEAD
=======
using UnityEngine.SceneManagement;
>>>>>>> mike-level

namespace DefaultNamespace
{
    /// <summary>
<<<<<<< HEAD
    /// Displays a pirate-themed story intro panel when the SmugglersIsland scene loads.
    /// Fades in → holds → fades out. Skippable with any key or screen tap.
    ///
    /// Attach to any Canvas in the scene. All UI elements are built at runtime —
    /// no prefab or manual child-wiring required.
    ///
    /// Inspector overrides:
    ///   introText     — body text shown in the panel.
    ///   holdDuration  — seconds at full opacity before auto-dismiss.
    ///   fadeDuration  — seconds for each fade transition.
=======
    /// Displays a pirate-themed story intro panel when a scene loads.
    /// Fades in → holds → fades out. Skippable with any key or screen tap.
    ///
    /// - Title is taken from the current scene name automatically.
    /// - Skipped entirely when returning from a won battle (BattleData.ReturningFromBattle).
    /// - Shows a "You were defeated..." message when restarting after a loss.
    ///
    /// Attach to any Canvas in the scene.
>>>>>>> mike-level
    /// </summary>
    public class StoryIntroUI : MonoBehaviour
    {
        [Header("Story Text")]
        [TextArea(4, 12)]
        [SerializeField] private string introText =
            "Ye have washed ashore at Smuggler's Island...\n\n" +
            "Three territories divide this wretched cove:\n" +
            "the open beach where ye first land,\n" +
            "the walled pirate camp patrolled by the Camp Leader,\n" +
            "and the dense jungle hiding secrets to the north.\n\n" +
            "Defeat the Camp Leader to claim Key 1 and Map 1.\n" +
            "Beware the quicksand fields — many have sunk without trace.\n\n" +
            "A hidden passage waits at the jungle's northern edge.\n" +
            "Find it and ye may escape without another fight.\n\n" +
            "Good luck, sailor.";

        [Header("Timing")]
        [SerializeField] private float holdDuration = 7f;
        [SerializeField] private float fadeDuration = 1.2f;

        [Header("Colours")]
        [SerializeField] private Color panelColor  = new Color(0.05f, 0.07f, 0.12f, 0.92f);
        [SerializeField] private Color textColor   = new Color(0.88f, 0.80f, 0.60f, 1f);
        [SerializeField] private Color titleColor  = new Color(1.00f, 0.84f, 0.15f, 1f);
        [SerializeField] private Color promptColor = new Color(0.70f, 0.65f, 0.50f, 1f);
<<<<<<< HEAD
=======
        [SerializeField] private Color deathColor  = new Color(0.90f, 0.20f, 0.20f, 1f);
>>>>>>> mike-level

        [Header("Font Sizes")]
        [SerializeField] private int titleSize  = 28;
        [SerializeField] private int bodySize   = 16;
        [SerializeField] private int promptSize = 13;

        private enum Phase { FadeIn, Hold, FadeOut, Done }
        private Phase  _phase;
        private float  _timer;
        private bool   _skipped;

        private CanvasGroup _group;
        private GameObject  _panel;
        private Text        _promptText;

        void Start()
        {
<<<<<<< HEAD
=======
            // Skip intro entirely when returning from a won battle
            if (BattleData.ReturningFromBattle)
            {
                BattleData.ReturningFromBattle = false;
                FinishIntro();
                return;
            }

>>>>>>> mike-level
            BuildUI();
            _phase = Phase.FadeIn;
            _timer = 0f;
            if (_group != null) _group.alpha = 0f;
        }

        void Update()
        {
            if (_phase == Phase.Done) return;

            if (!_skipped && (Input.anyKeyDown ||
                (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
            { _skipped = true; _phase = Phase.FadeOut; _timer = 0f; return; }

            _timer += Time.deltaTime;

            switch (_phase)
            {
                case Phase.FadeIn:
                    SetAlpha(Mathf.Clamp01(_timer / fadeDuration));
                    if (_timer >= fadeDuration) Next();
                    break;
                case Phase.Hold:
                    if (_promptText != null)
                    {
                        float p = 0.5f + 0.5f * Mathf.Sin(Time.time * 2.5f);
<<<<<<< HEAD
                        Color c = _promptText.color; _promptText.color = new Color(c.r,c.g,c.b,p);
=======
                        Color c = _promptText.color;
                        _promptText.color = new Color(c.r, c.g, c.b, p);
>>>>>>> mike-level
                    }
                    if (_timer >= holdDuration) Next();
                    break;
                case Phase.FadeOut:
                    SetAlpha(1f - Mathf.Clamp01(_timer / fadeDuration));
                    if (_timer >= fadeDuration) Next();
                    break;
            }
        }

        private void Next()
        {
            _timer = 0f;
            switch (_phase)
            {
                case Phase.FadeIn:  _phase = Phase.Hold;    break;
                case Phase.Hold:    _phase = Phase.FadeOut; break;
                case Phase.FadeOut:
                    _phase = Phase.Done;
<<<<<<< HEAD
                    if (_panel != null) _panel.SetActive(false);
                    // Activate MinimapCamera FIRST so its Awake() creates the
                    // RenderTexture before MinimapUI.Start() tries to read it.
                    foreach (MinimapCamera mc in FindObjectsOfType<MinimapCamera>(true))
                    { mc.gameObject.SetActive(true); break; }
                    // Then activate the minimap UI canvas.
                    foreach (Canvas c in FindObjectsOfType<Canvas>(true))
                        if (c.gameObject.name == "MinimapCanvas")
                        { c.gameObject.SetActive(true); break; }
=======
                    FinishIntro();
>>>>>>> mike-level
                    break;
            }
        }

<<<<<<< HEAD
=======
        /// <summary>Hides the panel and activates minimap — called on finish or skip.</summary>
        private void FinishIntro()
        {
            if (_panel != null) _panel.SetActive(false);

            foreach (MinimapCamera mc in FindObjectsOfType<MinimapCamera>(true))
            { mc.gameObject.SetActive(true); break; }

            foreach (Canvas c in FindObjectsOfType<Canvas>(true))
                if (c.gameObject.name == "MinimapCanvas")
                { c.gameObject.SetActive(true); break; }
        }

>>>>>>> mike-level
        private void SetAlpha(float a) { if (_group != null) _group.alpha = a; }

        private void BuildUI()
        {
            if (GetComponent<Canvas>() == null)
            { Debug.LogError("StoryIntroUI must be on a Canvas."); return; }

            if (GetComponent<CanvasScaler>() == null)
            {
                var s = gameObject.AddComponent<CanvasScaler>();
                s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                s.referenceResolution = new Vector2(1920, 1080);
            }

            _panel = new GameObject("StoryIntroPanel");
            _panel.transform.SetParent(transform, false);
            _group = _panel.AddComponent<CanvasGroup>();
            _group.blocksRaycasts = false;

            var rt = _panel.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            _panel.AddComponent<Image>().color = panelColor;

<<<<<<< HEAD
            MakeText("Title",  "— Smuggler's Island —", titleSize, titleColor,
                     new Vector2(0.1f,0.78f), new Vector2(0.9f,0.92f), TextAnchor.UpperCenter);
            MakeText("Sep",    "────────────────────────", 13,
                     new Color(titleColor.r,titleColor.g,titleColor.b,0.4f),
                     new Vector2(0.15f,0.73f), new Vector2(0.85f,0.80f), TextAnchor.UpperCenter);
            MakeText("Body",   introText, bodySize, textColor,
                     new Vector2(0.12f,0.12f), new Vector2(0.88f,0.72f), TextAnchor.UpperLeft);
            _promptText = MakeText("Prompt", "[ Press any key to continue ]", promptSize, promptColor,
                     new Vector2(0.1f,0.04f), new Vector2(0.9f,0.12f), TextAnchor.LowerCenter);
        }

        private Text MakeText(string name, string content, int size, Color color,
                               Vector2 aMin, Vector2 aMax, TextAnchor align)
        {
            var go = new GameObject(name);
            go.transform.SetParent(_panel.transform, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = aMin; rt.anchorMax = aMax;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
=======
            // FIX: use current scene name instead of hardcoded "Smuggler's Island"
            string sceneName = SceneManager.GetActiveScene().name;
            string titleLine = $"— {FormatSceneName(sceneName)} —";

            // If player was defeated, show death message above the title
            bool diedAndRestarted = BattleData.DiedInBattle;
            BattleData.DiedInBattle = false;

            if (diedAndRestarted)
            {
                MakeText("Death", "⚔ You were defeated and must start again...", 18, deathColor,
                         new Vector2(0.1f, 0.88f), new Vector2(0.9f, 0.96f), TextAnchor.UpperCenter);
            }

            MakeText("Title",  titleLine, titleSize, titleColor,
                     new Vector2(0.1f, 0.78f), new Vector2(0.9f, 0.92f), TextAnchor.UpperCenter);
            MakeText("Sep",    "────────────────────────", 13,
                     new Color(titleColor.r, titleColor.g, titleColor.b, 0.4f),
                     new Vector2(0.15f, 0.73f), new Vector2(0.85f, 0.80f), TextAnchor.UpperCenter);
            MakeText("Body",   introText, bodySize, textColor,
                     new Vector2(0.12f, 0.12f), new Vector2(0.88f, 0.72f), TextAnchor.UpperLeft);
            _promptText = MakeText("Prompt", "[ Press any key to continue ]", promptSize, promptColor,
                     new Vector2(0.1f, 0.04f), new Vector2(0.9f, 0.12f), TextAnchor.LowerCenter);
        }

        /// <summary>
        /// Converts a camelCase or PascalCase scene name into a readable title.
        /// e.g. "SmugglersIsland" → "Smugglers Island"
        ///      "StormbreakerIsland" → "Stormbreaker Island"
        /// </summary>
        private string FormatSceneName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                if (i > 0 && char.IsUpper(name[i]) && !char.IsUpper(name[i - 1]))
                    sb.Append(' ');
                sb.Append(name[i]);
            }
            return sb.ToString();
        }

        private Text MakeText(string objName, string content, int size, Color color,
                               Vector2 aMin, Vector2 aMax, TextAnchor align)
        {
            var go = new GameObject(objName);
            go.transform.SetParent(_panel.transform, false);
            var r = go.AddComponent<RectTransform>();
            r.anchorMin = aMin; r.anchorMax = aMax;
            r.offsetMin = r.offsetMax = Vector2.zero;
>>>>>>> mike-level
            var t = go.AddComponent<Text>();
            t.text = content; t.fontSize = size; t.color = color;
            t.alignment = align;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow   = VerticalWrapMode.Overflow;
            return t;
        }

        public void SkipIntro() { _skipped = true; _phase = Phase.FadeOut; _timer = 0f; }
        public bool IsRunning() => _phase != Phase.Done;
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> mike-level
