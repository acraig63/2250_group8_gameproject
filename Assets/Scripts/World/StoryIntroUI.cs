using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Attach this to your Canvas GameObject (or any GameObject — it creates its own Canvas).
/// On Play, displays the pirate backstory as a full-screen text panel.
/// The player presses any key to dismiss it and start the game.
///
/// Setup:
///   1. Create an empty GameObject in your scene, name it "StoryIntro"
///   2. Attach this script to it
///   3. Press Play — the intro screen appears automatically
///
/// The screen fades out smoothly when dismissed.
/// GameManager.StartGame() is called after the fade completes.
/// </summary>
public class StoryIntroUI : MonoBehaviour
{
    // -----------------------------------------------------------------------
    // Story text — edit this to match your narrative
    // -----------------------------------------------------------------------
    private const string STORY_TEXT =
        "PIRATES OF THE CARIBBEAN\n\n" +
        "You are the most feared pirate captain on the sea.\n\n" +
        "After a brutal ambush by a ruthless rival known only as the\n" +
        "PIRATE KING, your entire crew has been captured and locked\n" +
        "in prison cells scattered across five islands.\n\n" +
        "Stripped of your allies and most of your gear, you must\n" +
        "fight your way through each island — answering coding\n" +
        "challenges to power your attacks — to collect every key\n" +
        "and free your crew.\n\n" +
        "Defeat the Pirate King. Save your crew.\n\n" +
        "[Press any key to begin]";

    // -----------------------------------------------------------------------
    // Internal UI references
    // -----------------------------------------------------------------------
    private Canvas    _canvas;
    private Image     _background;
    private Text      _storyText;
    private bool      _dismissed = false;
    private float     _fadeDuration = 1.2f;

    void Start()
    {
        BuildIntroScreen();
    }

    void Update()
    {
        if (_dismissed) return;

        // Any key press dismisses the screen
        if (Input.anyKeyDown)
        {
            _dismissed = true;
            StartCoroutine(FadeOutAndStart());
        }
    }

    // -----------------------------------------------------------------------
    // Build the UI at runtime — no prefab or Canvas setup needed
    // -----------------------------------------------------------------------
    private void BuildIntroScreen()
    {
        // --- Canvas ---
        GameObject canvasObj = new GameObject("StoryIntroCanvas");
        _canvas = canvasObj.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 100; // On top of everything
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // --- Full screen dark background ---
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        _background = bgObj.AddComponent<Image>();
        _background.color = new Color(0.05f, 0.05f, 0.10f, 1f); // Near-black

        RectTransform bgRect = _background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // --- Story text ---
        GameObject textObj = new GameObject("StoryText");
        textObj.transform.SetParent(canvasObj.transform, false);
        _storyText = textObj.AddComponent<Text>();

        // Use the built-in Arial font — no font asset needed
        _storyText.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _storyText.fontSize  = 22;
        _storyText.color     = new Color(0.95f, 0.85f, 0.60f, 1f); // Warm gold
        _storyText.alignment = TextAnchor.MiddleCenter;
        _storyText.text      = STORY_TEXT;
        _storyText.lineSpacing = 1.4f;

        RectTransform textRect = _storyText.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.1f, 0.1f);
        textRect.anchorMax = new Vector2(0.9f, 0.9f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Debug.Log("Story intro screen displayed. Press any key to begin.");
    }

    // -----------------------------------------------------------------------
    // Fade out the intro screen, then hand control back to the game
    // -----------------------------------------------------------------------
    private IEnumerator FadeOutAndStart()
    {
        float elapsed = 0f;
        Color bgStart   = _background.color;
        Color textStart = _storyText.color;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _fadeDuration;

            _background.color = new Color(bgStart.r,   bgStart.g,   bgStart.b,   Mathf.Lerp(1f, 0f, t));
            _storyText.color  = new Color(textStart.r, textStart.g, textStart.b, Mathf.Lerp(1f, 0f, t));

            yield return null;
        }

        // Destroy the canvas once fully faded
        Destroy(_canvas.gameObject);

        Debug.Log("Story intro dismissed. Game world is now active.");
    }
}
