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
<<<<<<< HEAD
=======
                ),

                new MultipleChoiceQuestion(
                    "Which keyword declares a variable that cannot change?",
                    new List<string> { "const", "var", "static", "readonly" },
                    "const",
                    10,
                    1,
                    "Constant values do not change"
                ),

                new MultipleChoiceQuestion(
                    "Which type stores text?",
                    new List<string> { "string", "int", "bool", "char[]" },
                    "string",
                    10,
                    1,
                    "Used for words"
                ),

                new MultipleChoiceQuestion(
                    "Which keyword creates an object?",
                    new List<string> { "new", "make", "create", "object" },
                    "new",
                    10,
                    1,
                    "Used with constructors"
                ),

                new MultipleChoiceQuestion(
                    "Which symbol starts a code block?",
                    new List<string> { "{", "(", "[", "<" },
                    "{",
                    10,
                    1,
                    "Used for scope"
                ),

                new MultipleChoiceQuestion(
                    "Which type stores true/false?",
                    new List<string> { "bool", "int", "float", "string" },
                    "bool",
                    10,
                    1,
                    "Binary values"
                ),

                new MultipleChoiceQuestion(
                    "Which keyword defines a method with no return value?",
                    new List<string> { "void", "func", "method", "define" },
                    "void",
                    10,
                    1,
                    "Used when no value is returned"
                ),

                new MultipleChoiceQuestion(
                    "Which operator assigns a value?",
                    new List<string> { "=", "==", "+", ":" },
                    "=",
                    10,
                    1,
                    "Single equals assigns"
                ),

                new MultipleChoiceQuestion(
                    "Which type holds decimal numbers?",
                    new List<string> { "double", "int", "bool", "string" },
                    "double",
                    10,
                    1,
                    "Floating point"
                ),

                new MultipleChoiceQuestion(
                    "Which keyword refers to the current object?",
                    new List<string> { "this", "self", "current", "base" },
                    "this",
                    10,
                    1,
                    "Inside class methods"
                ),

                new MultipleChoiceQuestion(
                    "Which symbol accesses members of an object?",
                    new List<string> { ".", ":", ";", "#" },
                    ".",
                    10,
                    1,
                    "Object.property"
>>>>>>> mike-level
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
<<<<<<< HEAD
=======
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint x = 10; Debug.Log(x - 3);",
                    new List<string> { "7", "13", "10", "Error" },
                    "7",
                    15,
                    2,
                    "Subtraction"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint x = 4; x += 2; Debug.Log(x);",
                    new List<string> { "6", "8", "4", "Error" },
                    "6",
                    15,
                    2,
                    "+= adds"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint x = 6; x--; Debug.Log(x);",
                    new List<string> { "5", "6", "7", "Error" },
                    "5",
                    15,
                    2,
                    "x-- subtracts 1"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint x = 2; Debug.Log(x * 3);",
                    new List<string> { "6", "5", "3", "Error" },
                    "6",
                    15,
                    2,
                    "Multiplication"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nstring s = \"Hi\"; Debug.Log(s + \"!\");",
                    new List<string> { "Hi!", "Hi", "!", "Error" },
                    "Hi!",
                    15,
                    2,
                    "String concatenation"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint x = 9 / 3; Debug.Log(x);",
                    new List<string> { "3", "9", "6", "Error" },
                    "3",
                    15,
                    2,
                    "Division"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nbool b = true; Debug.Log(b);",
                    new List<string> { "true", "false", "1", "Error" },
                    "true",
                    15,
                    2,
                    "Boolean prints"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint x = 1; x = x + 5; Debug.Log(x);",
                    new List<string> { "6", "5", "1", "Error" },
                    "6",
                    15,
                    2,
                    "Addition"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nDebug.Log(3 + 4);",
                    new List<string> { "7", "34", "3", "Error" },
                    "7",
                    15,
                    2,
                    "Integer addition"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nDebug.Log(\"A\" + \"B\");",
                    new List<string> { "AB", "A B", "Error", "A" },
                    "AB",
                    15,
                    2,
                    "Strings join"
>>>>>>> mike-level
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
<<<<<<< HEAD
=======
                ),

                new MultipleChoiceQuestion(
                    "How many times does this loop run?\nfor(int i=0;i<5;i++)",
                    new List<string> { "5", "4", "6", "Error" },
                    "5",
                    20,
                    3,
                    "0 to 4"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nfor(int i=1;i<4;i++) Debug.Log(i);",
                    new List<string> { "1 2 3", "0 1 2", "1 2 3 4", "Error" },
                    "1 2 3",
                    20,
                    3,
                    "Starts at 1"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint x=0; for(int i=0;i<2;i++) x++; Debug.Log(x);",
                    new List<string> { "2", "1", "0", "Error" },
                    "2",
                    20,
                    3,
                    "Increment twice"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nfor(int i=0;i<1;i++) Debug.Log(\"Hi\");",
                    new List<string> { "Hi", "Nothing", "Error", "Hi Hi" },
                    "Hi",
                    20,
                    3,
                    "Runs once"
                ),

                new MultipleChoiceQuestion(
                    "How many iterations?\nfor(int i=2;i<5;i++)",
                    new List<string> { "3", "2", "5", "Error" },
                    "3",
                    20,
                    3,
                    "2,3,4"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint s=0; for(int i=0;i<4;i++) s+=1; Debug.Log(s);",
                    new List<string> { "4", "3", "1", "Error" },
                    "4",
                    20,
                    3,
                    "Adds 1 four times"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nfor(int i=0;i<3;i++) Debug.Log(\"A\");",
                    new List<string> { "AAA", "A A A", "Error", "AA" },
                    "AAA",
                    20,
                    3,
                    "Runs 3 times"
                ),

                new MultipleChoiceQuestion(
                    "Loop start value?\nfor(int i=5;i<8;i++)",
                    new List<string> { "5", "0", "8", "Error" },
                    "5",
                    20,
                    3,
                    "Initial value"
                ),

                new MultipleChoiceQuestion(
                    "How many prints?\nfor(int i=0;i<=2;i++)",
                    new List<string> { "3", "2", "1", "Error" },
                    "3",
                    20,
                    3,
                    "0,1,2"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint s=1; for(int i=0;i<2;i++) s*=2; Debug.Log(s);",
                    new List<string> { "4", "2", "3", "Error" },
                    "4",
                    20,
                    3,
                    "1→2→4"
>>>>>>> mike-level
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
<<<<<<< HEAD
=======
                ),

                new MultipleChoiceQuestion(
                    "What is wrong?\nint x == 5;",
                    new List<string> { "Wrong operator", "Missing ;", "Correct", "Wrong variable" },
                    "Wrong operator",
                    25,
                    4,
                    "== compares"
                ),

                new MultipleChoiceQuestion(
                    "Error?\nDebug.Log(x)",
                    new List<string> { "Missing semicolon", "Correct", "Wrong function", "Variable issue" },
                    "Missing semicolon",
                    25,
                    4,
                    "Needs ;"
                ),

                new MultipleChoiceQuestion(
                    "Issue?\nbool x = 5;",
                    new List<string> { "Wrong type", "Correct", "Missing ;", "Wrong name" },
                    "Wrong type",
                    25,
                    4,
                    "bool stores true or false"
                ),

                new MultipleChoiceQuestion(
                    "Problem?\nstring s = 5;",
                    new List<string> { "Wrong type", "Correct", "Missing ;", "Wrong keyword" },
                    "Wrong type",
                    25,
                    4,
                    "string holds text"
                ),

                new MultipleChoiceQuestion(
                    "Error?\nfor(int i=0;i<5;i++) {",
                    new List<string> { "Missing brace", "Correct", "Wrong loop", "Wrong variable" },
                    "Missing brace",
                    25,
                    4,
                    "Needs closing }"
                ),

                new MultipleChoiceQuestion(
                    "Issue?\nDebug.Log Hello;",
                    new List<string> { "Missing quotes", "Correct", "Wrong keyword", "Missing ;" },
                    "Missing quotes",
                    25,
                    4,
                    "Strings need quotes"
                ),

                new MultipleChoiceQuestion(
                    "Error?\nint 5 = x;",
                    new List<string> { "Invalid variable name", "Correct", "Missing ;", "Wrong type" },
                    "Invalid variable name",
                    25,
                    4,
                    "Variable names cannot start with number"
                ),

                new MultipleChoiceQuestion(
                    "Issue?\nif(x = 5)",
                    new List<string> { "Assignment instead of comparison", "Correct", "Missing ;", "Wrong keyword" },
                    "Assignment instead of comparison",
                    25,
                    4,
                    "Use =="
                ),

                new MultipleChoiceQuestion(
                    "Error?\nDebug.Log(\"Hi\"",
                    new List<string> { "Missing bracket", "Correct", "Missing ;", "Wrong keyword" },
                    "Missing bracket",
                    25,
                    4,
                    "Needs )"
                ),

                new MultipleChoiceQuestion(
                    "Issue?\nint x;",
                    new List<string> { "Correct", "Missing type", "Missing value", "Wrong keyword" },
                    "Correct",
                    25,
                    4,
                    "Declaration is valid"
>>>>>>> mike-level
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
<<<<<<< HEAD
=======
                ),

                new MultipleChoiceQuestion(
                    "Output?\nint x=1; x+=3; Debug.Log(x);",
                    new List<string> { "4", "3", "1", "Error" },
                    "4",
                    30,
                    5,
                    "Adds 3"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint x=5; if(x>2) x++; Debug.Log(x);",
                    new List<string> { "6", "5", "7", "Error" },
                    "6",
                    30,
                    5,
                    "Condition true"
                ),

                new MultipleChoiceQuestion(
                    "Output?\nint x=3; if(x<2) x=0; Debug.Log(x);",
                    new List<string> { "3", "0", "2", "Error" },
                    "3",
                    30,
                    5,
                    "Condition false"
                ),

                new MultipleChoiceQuestion(
                    "Result?\nint x=1; for(int i=0;i<2;i++) x+=2; Debug.Log(x);",
                    new List<string> { "5", "3", "2", "Error" },
                    "5",
                    30,
                    5,
                    "1→3→5"
                ),

                new MultipleChoiceQuestion(
                    "Output?\nint x=2; x*=3; Debug.Log(x);",
                    new List<string> { "6", "5", "3", "Error" },
                    "6",
                    30,
                    5,
                    "Multiply"
                ),

                new MultipleChoiceQuestion(
                    "Result?\nint x=4; if(x==4) x+=1; Debug.Log(x);",
                    new List<string> { "5", "4", "6", "Error" },
                    "5",
                    30,
                    5,
                    "Condition true"
                ),

                new MultipleChoiceQuestion(
                    "Output?\nint x=10; x-=3; Debug.Log(x);",
                    new List<string> { "7", "13", "10", "Error" },
                    "7",
                    30,
                    5,
                    "Subtract"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint x=1; for(int i=0;i<3;i++) x+=1; Debug.Log(x);",
                    new List<string> { "4", "3", "1", "Error" },
                    "4",
                    30,
                    5,
                    "Increment 3 times"
                ),

                new MultipleChoiceQuestion(
                    "Output?\nint x=8; if(x>5) x/=2; Debug.Log(x);",
                    new List<string> { "4", "8", "2", "Error" },
                    "4",
                    30,
                    5,
                    "Divide"
                ),

                new MultipleChoiceQuestion(
                    "Result?\nint x=0; for(int i=0;i<5;i++) x+=i; Debug.Log(x);",
                    new List<string> { "10", "15", "5", "Error" },
                    "10",
                    30,
                    5,
                    "0+1+2+3+4"
                )
            };
        }

        // Level 6, Boss Questions
        public static List<MultipleChoiceQuestion> GetLevel6Questions()
        {
            return new List<MultipleChoiceQuestion>
            {
                new MultipleChoiceQuestion(
                    "What prints?\nint x = 1;\nfor(int i = 0; i < 3; i++)\n{\n    x += i;\n}\nDebug.Log(x);",
                    new List<string> { "4", "3", "6", "1" },
                    "4",
                    40,
                    6,
                    "x becomes 1+0+1+2"
                ),

                new MultipleChoiceQuestion(
                    "What is the output?\nint x = 0;\nfor(int i = 1; i <= 3; i++)\n{\n    x += i * 2;\n}\nDebug.Log(x);",
                    new List<string> { "6", "12", "9", "3" },
                    "12",
                    40,
                    6,
                    "Adds 2 + 4 + 6"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint x = 5;\nif(x > 2)\n{\n    x *= 2;\n}\nif(x > 8)\n{\n    x -= 3;\n}\nDebug.Log(x);",
                    new List<string> { "10", "7", "13", "5" },
                    "7",
                    40,
                    6,
                    "5→10→7"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint sum = 0;\nfor(int i = 0; i < 4; i++)\n{\n    if(i % 2 == 0)\n        sum += i;\n}\nDebug.Log(sum);",
                    new List<string> { "2", "4", "6", "0" },
                    "2",
                    40,
                    6,
                    "Only 0 and 2 are added"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint x = 3;\nfor(int i = 0; i < 2; i++)\n{\n    x *= x;\n}\nDebug.Log(x);",
                    new List<string> { "9", "27", "81", "6561" },
                    "81",
                    40,
                    6,
                    "3→9→81"
                ),

                new MultipleChoiceQuestion(
                    "What is the output?\nint x = 0;\nfor(int i = 0; i < 3; i++)\n{\n    for(int j = 0; j < 2; j++)\n    {\n        x++;\n    }\n}\nDebug.Log(x);",
                    new List<string> { "5", "6", "3", "2" },
                    "6",
                    40,
                    6,
                    "Outer loop 3 times, inner loop 2 times"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint x = 8;\nwhile(x > 1)\n{\n    x /= 2;\n}\nDebug.Log(x);",
                    new List<string> { "8", "4", "2", "1" },
                    "1",
                    40,
                    6,
                    "8→4→2→1"
                ),

                new MultipleChoiceQuestion(
                    "What is wrong with this code?\nfor(int i = 0; i < 3; i++)\n    Debug.Log(i)\n    Debug.Log(\"done\");",
                    new List<string>
                    {
                        "Missing semicolon after Debug.Log(i)",
                        "Loop variable not declared",
                        "Nothing is wrong",
                        "Wrong use of braces"
                    },
                    "Missing semicolon after Debug.Log(i)",
                    40,
                    6,
                    "C# still needs semicolons"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint x = 2;\nint y = 3;\nif(x < y && y < 5)\n{\n    x += y;\n}\nDebug.Log(x);",
                    new List<string> { "2", "3", "5", "6" },
                    "5",
                    40,
                    6,
                    "Both conditions are true"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint x = 0;\nfor(int i = 1; i <= 4; i++)\n{\n    if(i == 3)\n        x += 5;\n    else\n        x += 1;\n}\nDebug.Log(x);",
                    new List<string> { "8", "7", "9", "6" },
                    "8",
                    40,
                    6,
                    "1+1+5+1"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint x = 10;\nfor(int i = 0; i < 2; i++)\n{\n    x -= 3;\n}\nif(x == 4)\n    x *= 2;\nDebug.Log(x);",
                    new List<string> { "4", "8", "14", "10" },
                    "8",
                    40,
                    6,
                    "10→7→4→8"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nstring s = \"A\";\nfor(int i = 0; i < 3; i++)\n{\n    s += \"B\";\n}\nDebug.Log(s);",
                    new List<string> { "AB", "ABB", "ABBB", "BBBB" },
                    "ABBB",
                    40,
                    6,
                    "Starts with A and adds B three times"
                ),

                new MultipleChoiceQuestion(
                    "What prints?\nint x = 1;\nfor(int i = 1; i <= 3; i++)\n{\n    x *= i;\n}\nDebug.Log(x);",
                    new List<string> { "3", "6", "9", "12" },
                    "6",
                    40,
                    6,
                    "1×1×2×3"
>>>>>>> mike-level
                )
            };
        }
    }
}