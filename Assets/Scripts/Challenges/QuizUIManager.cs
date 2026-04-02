using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DefaultNamespace;

public class QuizUIManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject quizPanel;

    [Header("Text Elements")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI feedbackText;

    [Header("Answer Buttons (exactly 4)")]
    public Button[] answerButtons;   

    private MultipleChoiceQuestion activeQuestion;
    private NPCDialogueTrigger activeNPC;

    private void Awake()
    {
        if (quizPanel != null)
            quizPanel.SetActive(false);
    }


    public void ShowQuiz(MultipleChoiceQuestion question, NPCDialogueTrigger npc)
    {
        activeQuestion = question;
        activeNPC = npc;

        // Freeze the player while the quiz is open
        SetPlayerMovement(false);

        // Populate UI
        dialogueText.text = "Ah, you caught me! Answer this question correctly to defeat me…";
        questionText.text = question.questionText;
        feedbackText.text = "";

        // Set up each answer button
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < question.options.Count)
            {
                answerButtons[i].gameObject.SetActive(true);

                // Get the label inside the button
                TextMeshProUGUI label = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                    label.text = question.options[i];

                // Capture index for the lambda
                int capturedIndex = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(question.options[capturedIndex]));
            }
            else
            {
                // Hide unused buttons if fewer than 4 options
                answerButtons[i].gameObject.SetActive(false);
            }
        }

        quizPanel.SetActive(true);
    }


    private void OnAnswerSelected(string answer)
    {
        if (activeQuestion == null) return;

        bool correct = activeQuestion.Evaluate(answer);

        if (correct)
        {
            feedbackText.text = "✓ Correct! The smuggler has been defeated!";
            feedbackText.color = Color.green;

            // Defeat the NPC after a short delay so the player can read the feedback
            Invoke(nameof(HandleCorrectAnswer), 1.2f);
        }
        else
        {
            feedbackText.text = "✗ Wrong! Try again.";
            feedbackText.color = Color.red;

            //  player can bump into the NPC again to retry
            Invoke(nameof(ClosePanel), 1.0f);
        }
    }

    private void HandleCorrectAnswer()
    {
        if (activeNPC != null)
        {
            Debug.Log("Destroying NPC: " + activeNPC.name);
            activeNPC.DefeatNPC();
        }
        else
        {
            Debug.LogError("NPC is NULL when trying to destroy!");
        }

        ClosePanel();
    }

    private void ClosePanel()
    {
        quizPanel.SetActive(false);
        SetPlayerMovement(true);
        // Reset the NPC's player-inside flag so the quiz can be re-triggered
        // immediately after a wrong answer without needing to exit the collider
        if (activeNPC != null)
            activeNPC.OnQuizClosed();
        activeQuestion = null;
        activeNPC = null;
    }

    private void SetPlayerMovement(bool enabled)
    {
        PlayerController pc = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();

        if (pc != null)
        {
            pc.enabled = enabled;

            Rigidbody2D rb = pc.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }
    }
}