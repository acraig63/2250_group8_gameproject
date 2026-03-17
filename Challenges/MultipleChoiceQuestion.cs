namespace Challenges;

using System.Collections.Generic;

public class MultipleChoiceQuestion : CodingQuestion
{
    public List<string> options;

    public MultipleChoiceQuestion(
        string question,
        List<string> options,
        string correctAnswer,
        int xp,
        int difficulty,
        string hint
    ) : base(question, correctAnswer, xp, difficulty, hint)
    {
        this.options = options;
    }

    public override bool Evaluate(string playerAnswer)
    {
        return playerAnswer == correctAnswer;
    }
}