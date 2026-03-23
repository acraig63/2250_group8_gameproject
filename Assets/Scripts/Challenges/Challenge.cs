namespace DefaultNamespace
{
    using System.Collections.Generic;
using UnityEngine;

// Challenge represents a gameplay event where the player must answer questions
// It controls running the questions and tracking completion
    public class Challenge
    {
        // Unique identifier for the challenge 
        public string challengeId;

        // List of questions included in this challenge
        // Uses CodingQuestion so it can support different question types using polymorphism
        public List<CodingQuestion> questions;

        // Total XP reward for completing the challenge 
        public int xpReward;

        // Tracks whether the challenge has been completed
        public bool isCompleted;

        // Constructor initializes the challenge with an ID
        public Challenge(string id)
        {
            challengeId = id;

            // Initialize empty list of questions
            questions = new List<CodingQuestion>();

            // Challenge starts as incomplete
            isCompleted = false;
        }

        // Adds a question to the challenge
        public void AddQuestion(CodingQuestion question)
        {
            questions.Add(question);
        }

        // Starts the challenge and runs through all questions
        public void StartChallenge()
        {
            // Debug message to show challenge has started
            Debug.Log("Challenge Started: " + challengeId);

            // Loop through each question in the challenge
            foreach (CodingQuestion q in questions)
            {
                // Display the question text
                Debug.Log("Question: " + q.GetQuestionText());

                // This is temporary for now but simulate player answer 
                // This will later be replaced by real player input 
                string simulatedAnswer = q.correctAnswer;

                // Evaluate the player's answer using polymorphism
                if (q.Evaluate(simulatedAnswer))
                {
                    Debug.Log("Correct!");
                }
                else
                {
                    Debug.Log("Incorrect!");
                }
            }

            // Mark the challenge as completed after all questions are processed
            isCompleted = true;

            // Debug message to show challenge completion
            Debug.Log("Challenge Completed!");
        }
    }
}