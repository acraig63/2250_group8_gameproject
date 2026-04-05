using UnityEngine;
using TMPro;

public class DialogueUIManager : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI dialogueText;

    public void ShowDialogue(string[] lines)
    {
        panel.SetActive(true);
        dialogueText.text = string.Join("\n", lines);
    }

    public void HideDialogue()
    {
        panel.SetActive(false);
    }
}
