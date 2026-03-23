namespace DefaultNamespace
{
    using System.Collections.Generic;

    public static class QuestionBank
    {
        // Level 1, Syntax
        public static List<MultipleChoiceQuestion> GetLevel1Questions()
        {
            return new List<MultipleChoiceQuestion>
            {
                new MultipleChoiceQuestion(
                    "Which is the correct way to declare an integer in C#?",
                    new List<string> { "int x;", "integer x;", "x int;", "num x;" },
                    "int x;",
                    10,
                    1,
                    "C# uses 'int'"
                ),

                new MultipleChoiceQuestion(
                    "Which symbol ends a statement in C#?",
                    new List<string> { ".", ";", ":", "," },
                    ";",
                    10,
                    1,
                    "Think of line termination"
                ),

                new MultipleChoiceQuestion(
                    "Which keyword creates a class in C#?",
                    new List<string> { "class", "object", "struct", "define" },
                    "class",
                    10,
                    1,
                    "Used to define types"
                )
            };
        }

        // Level 2, Output
        public static List<MultipleChoiceQuestion> GetLevel2Questions()
        {
            return new List<MultipleChoiceQuestion>
            {
                new MultipleChoiceQuestion(
                    "What does this print?\nint x = 5; x++; Debug.Log(x);",
                    new List<string> { "5", "6", "7", "Error" },
                    "6",
                    15,
                    2,
                    "x++ adds 1"
                ),

                new MultipleChoiceQuestion(
                    "What is the output?\nint a = 3; int b = 2; Debug.Log(a + b);",
                    new List<string> { "5", "6", "32", "Error" },
                    "5",
                    15,
                    2,
                    "Addition of ints"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nstring s = \"Hello\"; Debug.Log(s);",
                    new List<string> { "Hello", "null", "Error", "0" },
                    "Hello",
                    15,
                    2,
                    "Strings print directly"
                )
            };
        }

        // Level 3, Loop Tracing
        public static List<MultipleChoiceQuestion> GetLevel3Questions()
        {
            return new List<MultipleChoiceQuestion>
            {
                new MultipleChoiceQuestion(
                    "What does this print?\nfor(int i=0;i<3;i++) Debug.Log(i);",
                    new List<string> { "0 1 2", "1 2 3", "0 1 2 3", "Error" },
                    "0 1 2",
                    20,
                    3,
                    "Loop runs from 0 to 2"
                ),

                new MultipleChoiceQuestion(
                    "How many times does this loop run?\nfor(int i=1;i<=5;i++)",
                    new List<string> { "4", "5", "6", "Error" },
                    "5",
                    20,
                    3,
                    "Starts at 1 ends at 5"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint sum = 0;\nfor(int i=0;i<3;i++) sum += i;\nDebug.Log(sum);",
                    new List<string> { "3", "6", "0", "Error" },
                    "3",
                    20,
                    3,
                    "0+1+2 = 3"
                )
            };
        }

        // Level 4, Debugging
        public static List<MultipleChoiceQuestion> GetLevel4Questions()
        {
            return new List<MultipleChoiceQuestion>
            {
                new MultipleChoiceQuestion(
                    "What is wrong with this code?\nint x = \"5\";",
                    new List<string>
                    {
                        "Wrong type assignment",
                        "Missing semicolon",
                        "Syntax is correct",
                        "Variable not declared"
                    },
                    "Wrong type assignment",
                    25,
                    4,
                    "int cannot hold string"
                ),

                new MultipleChoiceQuestion(
                    "What is the issue?\nfor(int i=0;i<5;i--)",
                    new List<string>
                    {
                        "Infinite loop",
                        "Syntax error",
                        "Nothing wrong",
                        "Wrong variable"
                    },
                    "Infinite loop",
                    25,
                    4,
                    "i is decreasing but condition is <5"
                ),

                new MultipleChoiceQuestion(
                    "What is wrong?\nDebug.Log(\"Hello\")",
                    new List<string>
                    {
                        "Missing semicolon",
                        "Wrong function",
                        "Nothing wrong",
                        "Missing brackets"
                    },
                    "Missing semicolon",
                    25,
                    4,
                    "C# requires ;"
                )
            };
        }

        // Level 5, Multi Step Logic
        public static List<MultipleChoiceQuestion> GetLevel5Questions()
        {
            return new List<MultipleChoiceQuestion>
            {
                new MultipleChoiceQuestion(
                    "What does this print?\nint x = 2;\nfor(int i=0;i<3;i++) x *= 2;\nDebug.Log(x);",
                    new List<string> { "8", "16", "4", "Error" },
                    "16",
                    30,
                    5,
                    "2→4→8→16"
                ),

                new MultipleChoiceQuestion(
                    "What is the output?\nint x = 0;\nif(x == 0) x = 5;\nDebug.Log(x);",
                    new List<string> { "0", "5", "Error", "1" },
                    "5",
                    30,
                    5,
                    "Condition is true"
                ),

                new MultipleChoiceQuestion(
                    "What happens?\nint x = 5;\nif(x > 3) {\n x += 2;\n}\nDebug.Log(x);",
                    new List<string> { "5", "7", "3", "Error" },
                    "7",
                    30,
                    5,
                    "5 + 2 = 7"
                )
            };
        }
    }
}