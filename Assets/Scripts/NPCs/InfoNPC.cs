using UnityEngine;

public class InfoNPC : MonoBehaviour
{
    public string[] dialogueLines;
    public DialogueUIManager dialogueUI;

    private bool playerInside = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        dialogueUI.ShowDialogue(dialogueLines);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        dialogueUI.HideDialogue();
    }
}