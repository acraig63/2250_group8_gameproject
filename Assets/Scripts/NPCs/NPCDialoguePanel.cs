using System.Collections;
using UnityEngine;
using TMPro;

public class NPCDialoguePanel : MonoBehaviour
{
    public static NPCDialoguePanel Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text   text;
    [SerializeField] private Canvas canvas;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(string message, float duration = 4f)
    {
        //StopAllCoroutines();
        gameObject.SetActive(true);
        StartCoroutine(ShowFor(message, duration));
    }

    public void ShowPersistent(string message, string prompt = "")
    {
        StopAllCoroutines();
        text.text = message;
        Debug.Log($"ShowPersistent called, canvas.enabled={canvas.enabled}, panel active={panel.activeSelf}");
        canvas.enabled = true;  // use canvas instead of panel
        panel.SetActive(true);
    }

    public void Hide()
    {
        if (this == null) return;
        StopAllCoroutines();
        canvas.enabled = false;
        //panel.SetActive(false);
    }

    private IEnumerator ShowFor(string message, float duration)
    {
        text.text = message;
        canvas.enabled = true;
        yield return new WaitForSeconds(duration);
        canvas.enabled = false;
    }
}