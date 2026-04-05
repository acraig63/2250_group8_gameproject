using System.Collections;
using UnityEngine;
using TMPro;

public class NPCDialoguePanel : MonoBehaviour
{
    public static NPCDialoguePanel Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text   text;

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

    public void ShowPersistent(string message)
    {
        StopAllCoroutines();
        text.text = message;
        panel.SetActive(true);
    }

    public void Hide()
    {
        StopAllCoroutines();
        panel.SetActive(false);
    }

    private IEnumerator ShowFor(string message, float duration)
    {
        text.text = message;
        panel.SetActive(true);
        yield return new WaitForSeconds(duration);
        panel.SetActive(false);
    }
}