namespace Challenges;

using System.Collections.Generic;

// Sub class of CodingQuestion class that represents a multiple choice question
// Defines how the question works
public class MultipleChoiceQuestion : CodingQuestion
{
    // List of possible choices
    public List<string> options;

    // Constructor to initialize a multiple-choice question
    public MultipleChoiceQuestion(
        string question,
        List<string> options,
        string correctAnswer,
        int xp,
        int difficulty,
        string hint
    ) : base(question, correctAnswer, xp, difficulty, hint)
    {
        // Store the choices
        this.options = options;
    }

    // Overrides the abstract method from CodingQuestion
    // Checks if the player's answer is correct
    public override bool Evaluate(string playerAnswer)
    {
        return playerAnswer == correctAnswer;
    }
}