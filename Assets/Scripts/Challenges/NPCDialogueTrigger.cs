using UnityEngine;
using DefaultNamespace;
using System.Collections.Generic;

public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("Quiz Settings")]
    public int questionLevel = 1;

    [Header("References")]
    public QuizUIManager quizUIManager;

    private bool hasBeenDefeated = false;
    private bool isPlayerInside = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenDefeated || isPlayerInside) return;
        if (!other.CompareTag("Player")) return;

        // Fallback: find QuizUIManager at runtime if the Inspector reference was lost
        if (quizUIManager == null)
            quizUIManager = FindObjectOfType<QuizUIManager>();
        if (quizUIManager == null) return;

        MultipleChoiceQuestion question = GetRandomQuestion();
        if (question == null) return;

        isPlayerInside = true;
        quizUIManager.ShowQuiz(question, this);
    }

    /// <summary>
    /// Called by QuizUIManager when the quiz panel closes so the player can
    /// re-trigger the encounter without needing to physically exit and re-enter
    /// the collider (important after a wrong answer).
    /// </summary>
    public void OnQuizClosed()
    {
        isPlayerInside = false;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    public void DefeatNPC()
    {
        hasBeenDefeated = true;
        Destroy(gameObject);
    }

    private MultipleChoiceQuestion GetRandomQuestion()
    {
        List<MultipleChoiceQuestion> questions = GetQuestionsForLevel(questionLevel);

        if (questions == null || questions.Count == 0)
            return null;

        int index = Random.Range(0, questions.Count);
        return questions[index];
    }

    private List<MultipleChoiceQuestion> GetQuestionsForLevel(int level)
    {
        switch (level)
        {
            case 1: return QuestionBank.GetLevel1Questions();
            case 2: return QuestionBank.GetLevel2Questions();
            case 3: return QuestionBank.GetLevel3Questions();
            case 4: return QuestionBank.GetLevel4Questions();
            case 5: return QuestionBank.GetLevel5Questions();
            default: return QuestionBank.GetLevel1Questions();
        }
    }
}