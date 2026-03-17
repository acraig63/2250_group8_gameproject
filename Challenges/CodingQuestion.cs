namespace Challenges;

public abstract class CodingQuestion
{
    public string questionText;
    public string correctAnswer;
    public int xpValue;
    public int difficulty;
    public string hintText;

    public CodingQuestion(string question, string answer, int xp, int difficulty, string hint)
    {
        this.questionText = question;
        this.correctAnswer = answer;
        this.xpValue = xp;
        this.difficulty = difficulty;
        this.hintText = hint;
    }

    public abstract bool Evaluate(string playerAnswer);

    public string GetQuestionText()
    {
        return questionText;
    }

    public string GetHint()
    {
        return hintText;
    }
}