namespace Challenges;

using System.Collections.Generic;
using UnityEngine;

public class Challenge
{
    public string challengeId;
    public List<CodingQuestion> questions;
    public int xpReward;
    public bool isCompleted;

    public Challenge(string id)
    {
        challengeId = id;
        questions = new List<CodingQuestion>();
        isCompleted = false;
    }

    public void AddQuestion(CodingQuestion question)
    {
        questions.Add(question);
    }

    public void StartChallenge()
    {
        Debug.Log("Challenge Started: " + challengeId);

        foreach (CodingQuestion q in questions)
        {
            Debug.Log("Question: " + q.GetQuestionText());

            string simulatedAnswer = q.correctAnswer;

            if (q.Evaluate(simulatedAnswer))
            {
                Debug.Log("Correct!");
            }
            else
            {
                Debug.Log("Incorrect!");
            }
        }

        isCompleted = true;
        Debug.Log("Challenge Completed!");
    }
}