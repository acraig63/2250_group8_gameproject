namespace DefaultNamespace
{
    // base parent class for all types of questions
// abstract base class for all coding questions
    public abstract class CodingQuestion
    {
        // common properties shared by all subclasses
        public string questionText;
        public string correctAnswer;
        public int xpValue;
        public int difficulty;
        public string hintText;

        //constructor to initialize fields
        public CodingQuestion(string question, string answer, int xp, int difficulty, string hint)
        {
            this.questionText = question;
            this.correctAnswer = answer;
            this.xpValue = xp;
            this.difficulty = difficulty;
            this.hintText = hint;
        }

        // Abstract method that must be implemented by subclasses
        // Defines how answers are evaluated
        public abstract bool Evaluate(string playerAnswer);

        // Returns question text
        public string GetQuestionText()
        {
            return questionText;
        }

        // Returns question hint
        public string GetHint()
        {
            return hintText;
        }
    }
}