using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CalculatorApp
{
    /// This class holds all the calculation functions that a normal calculator has. 
    /// It is where the input from the user gets split into chunks and individually calculated/processed.
    /// The other class called CalculatorGUI is only for setting up the visual design and layout of the calculator.

    class Calculator
    {

        public void SetupCalculator(string expression)
        {
            //Check for invalid inputs
            if (IsValid(expression) == false)
            {
                return; //Could not set up successfully, then exit.
            }

            //Obtain cleaned operators and numbers needed for calculation(s)
            string[] numbers = ObtainWorkingNumbers(expression);
            string[] operators = ObtainWorkingOperators(expression);

            Console.WriteLine("numbers to be used for calculation:");
            foreach (string number in numbers) Console.WriteLine("number: " + number);

            //Once input has been properly processed/setup for calculation, begin calculating.
            string answer = ComputeExpression(numbers, operators);

            //Display answer and input to user
            SetLabels(expression, answer);
        }

        private string ComputeExpression(string[] numbers, string[] operators)
        {
            string answer = ""; //keep answer in terms of string for display purposes

            //Calculate each chunk and return as number, and update number array

            Chunk chunk = new Chunk();

            do
            {
                //Create chunk and update arrays
                chunk = chunk.GetChunk(numbers, operators);

                //remove blank spaces
                numbers = numbers.Where(z => !string.IsNullOrWhiteSpace(z)).ToArray();
                operators = operators.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();


                //compute the chunk by calling the appropriate method
                switch (chunk.calculatorOperator)
                {
                    case "+":
                        chunk.number1 = Add(chunk.number1, chunk.number2);
                        break;
                    case "-":
                        chunk.number1 = Subtract(chunk.number1, chunk.number2);
                        break;
                    case "÷":
                        chunk.number1 = Divide(chunk.number1, chunk.number2);
                        break;
                    case "×":
                        chunk.number1 = Multiply(chunk.number1, chunk.number2);
                        break;
                    case "^":
                        chunk.number1 = Exponent(chunk.number1, chunk.number2);
                        break;
                }


            } while (chunk.IsFull());
        
            answer = chunk.number1; //result is always the first number in the partially filled chunk

            return answer;
        }

        private bool IsValid(string expression)
        {
            //Error message not required for this invalid case
            if (expression.Length == 0) return false;

            //if no digits, then invalid
            Regex digits = new Regex(@"[0-9]");
            if (digits.Matches(expression).Count == 0)
            {
                DisplayError("No Digits In Input");
                return false;
            }

            //Check for expressions starting with '×','÷','^' as they are invalid. Others are ok.
            string startSymbol = expression.ElementAt(0).ToString();
            Regex badStartingSymbols = new Regex(@"[\÷×^]");

            if (badStartingSymbols.Matches(startSymbol).Count >= 1)
            {
                DisplayError("Illegal starting symbol of " + startSymbol);
                return false;
            }

            //Check for expressions ending with operators (not including '.')
            string endingSymbol = expression.ElementAt(expression.Length - 1).ToString();
            Regex okEndingSymbols = new Regex(@"[0-9.]");

            if (okEndingSymbols.Matches(endingSymbol).Count == 0)
            {
                DisplayError("Illegal ending symbol of " + endingSymbol);
                return false;
            }

            //Check for one of the 19 possible invalid 2-symbol combinations.
            string[] invalidOperatorCombos = { "÷÷", "÷×", "÷^", "×÷", "××", "×^", "^÷", "^×", "^^", "√÷", "√×", "√^", "-÷", "-×", "-^", "+÷", "+×", "+^", ".." };

            foreach (string combo in invalidOperatorCombos)
            {
                if (expression.Contains(combo))
                {
                    DisplayError("Illegal symbol combo of " + combo);
                    return false;
                }
            }

            //Disallow negative roots as they are non-real and complex.
            for (int charIndex = 0; charIndex <= expression.Length - 1; charIndex++)
            {
                if (expression.ElementAt(charIndex) == '√')
                {
                    //check next character for negatives
                    int nextChar = charIndex + 1;
                    if (expression.ElementAt(nextChar) == '-')
                    {
                        DisplayError("Non-real number from √");
                        return false;
                    }

                }
            }


            //check for numbers with two non-consecutive decimal points
            string[] numbers = ObtainWorkingNumbers(expression);
            foreach (string number in numbers)
            {
                var decimalCount = number.Count(x => x == '.');
                if (decimalCount >= 2)
                {
                    DisplayError("Illegal quantity of decimal points in a number");
                    return false;
                }
            }


            //It is presumed to be valid if all checks passed
            return true;
        }


        private string[] ObtainWorkingNumbers(string expression)
        {

            //Extract array of numbers, and remove empty elements of size nConsecutiveOperators - 1
            string[] numbers = expression.Split('-', '+', '÷', '×', '^', '√');
            numbers = numbers.Where(z => !string.IsNullOrWhiteSpace(z)).ToArray();


            //obtain first whole number, including any operators
            string startingOperator = "";
            bool firstDigitDetected = false;
            int charNumber = 0;
            Regex numberParts = new Regex(@"[0-9.]");

            while (firstDigitDetected == false)
            {
                char compareChar = expression.ElementAt(charNumber);

                //if the character is not a number
                if (numberParts.Matches(compareChar.ToString()).Count == 0)
                {
                    startingOperator += expression.ElementAt(charNumber);
                    charNumber++; //search the next character

                }
                else
                {
                    firstDigitDetected = true;
                    //now add the whole first number including any legal operators to the array. 
                    numbers[0] = startingOperator + numbers[0];
                }
            }

            //Numbers still need some of the information contained within non-simplified operators 
            string[] baseOperators = ObtainBaseOperators(expression);

            //Add operators that are actually part of the number to the numbers in the array. 
            for (int operatorIndex = 0; operatorIndex <= (baseOperators.Length - 1); operatorIndex++)
            {
                string operatorSequence = baseOperators[operatorIndex];

                //check if a sequence of 2 or more
                if (operatorSequence.Length > 1)
                {
                    //Part of number is the whole sequence minus the first symbol
                    string partOfNumber = operatorSequence;
                    partOfNumber = partOfNumber.Remove(0, 1) + "";


                    //Extract real operator and update array
                    string realOperator = operatorSequence.ElementAt(0).ToString();

                    baseOperators[operatorIndex] = realOperator;

                    //add one to index number, because we want to skip the starting number
                    numbers[operatorIndex + 1] = partOfNumber + numbers[operatorIndex + 1];
                }
            }

            //Remove unnecessary pluses or minuses at the front
            numbers = SimplifyNumberSign(numbers);

            //Remove the '√' sign from the number by evaluation to make them truly workable
            numbers = EvaluateRoots(numbers);

            return numbers;
        }

        private string[] SimplifyNumberSign(string[] numberArray)


        {
            //obtaining the operator sequence
            Regex numberParts = new Regex(@"[0-9.√]");

            //Search through all the number entries
            for (int numberIndex = 0; numberIndex <= numberArray.Length - 1; numberIndex++)
            {
                string minusSequence = "";
                string number = numberArray[numberIndex];
                string baseNumber = "";

                foreach (Match match in numberParts.Matches(number))
                {
                    baseNumber += match;
                }

                //Search through all the parts of a number entry
                foreach (char character in number)
                {

                    //obtain the minuses
                    if (character == '-')
                    {
                        minusSequence += character;
                    }


                }

                //remove unnecessary minuses
                if (minusSequence.Length % 2 == 0) minusSequence = "";
                else minusSequence = "-";

                //update number 
                numberArray[numberIndex] = minusSequence + baseNumber;
            }


            return numberArray;
        }

        private string[] EvaluateRoots(string[] numberArray)
        {
            for (int numberIndex = 0; numberIndex <= numberArray.Length - 1; numberIndex++)
            {
                string number = numberArray[numberIndex];
                if (number.Contains('√'))
                {
                    string decimalNumber = Convert.ToString(SquareRoot(number));
                    numberArray[numberIndex] = decimalNumber;
                }
            }

            return numberArray;
        }

        private string[] ObtainBaseOperators(string expression)
        {
            string[] operators = expression.Split('1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.');

            //remove first element, as it is part of the number
            operators[0] = "";

            //remove blank elements
            operators = operators.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();


            return operators;
        }

        private string[] ObtainWorkingOperators(string expression)
        {
            string[] operators = expression.Split('1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.');

            //remove first element, as it is part of the number
            operators[0] = "";

            //remove blank elements
            operators = operators.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();



            //Refine operators, and remove the ones that are actually part of the number
            for (int operatorIndex = 0; operatorIndex <= (operators.Length - 1); operatorIndex++)
            {
                string operatorSequence = operators[operatorIndex];

                //check if a sequence of 2 or more
                if (operatorSequence.Length > 1)
                {
                    
                    //Extract real operator and update array
                    string realOperator = operatorSequence.ElementAt(0).ToString();

                    operators[operatorIndex] = realOperator;
                }
            }

            return operators;
        }



        private void DisplayError(string reason)
        {
            CalculatorGUI.mainDisplay.Text = "Error: " + reason;
        }

        private void SetLabels(string expression, string answer)
        {
            //Show the user their calculated expression
            CalculatorGUI.postCalculatedExpression.Text = "= " + expression;

            //Display answer
            CalculatorGUI.mainDisplay.Text = "= " + answer;

            //Once done, work with the precalculatorDisplay, if they did not press equals

        }



        #region Calculator Functions
        private string Add(string firstNumber, string secondNumber)
        {
            return "";
        }
        private string Subtract(string firstNumber, string secondNumber)
        {
            return "";
        }

        private string Multiply(string firstNumber, string secondNumber)
        {
            return "";
        }

        private string Divide(string numerator, string denominator)
        {



            return "";
        }

        private string Exponent(string baseNumber, string power)
        {

            return "";
        }

        private double SquareRoot(string number)
        {
            //Obtain number of roots in the number
            Regex root = new Regex(@"[√]");
            int rootCount = root.Matches(number).Count;

            if (rootCount == 0) return 0.00;

            bool isNegative = false;

            string nonNegativeNumber = "";

            if (number.Contains("-"))
            {
                isNegative = true;
                //Then number to be rooted will be everything but the first two chars
                nonNegativeNumber = number.Remove(0, 1) + "";
            }
            else
            {
                nonNegativeNumber = number;
            }



            double workingNumber = Convert.ToDouble(nonNegativeNumber.Remove(0, rootCount) + "");

            while (rootCount >= 1)
            {
                //Calculate root value
                workingNumber = Math.Sqrt(workingNumber);
                rootCount--;
            }

            if (isNegative) workingNumber = workingNumber * -1; //Add negative back on.

            double result = workingNumber;

            return result;
        }


        #endregion
    }

    class Chunk
    {
        public string number1 = "";
        public string calculatorOperator = "";
        public string number2 = "";

        public bool IsFull()
        {
            if (number1.Length >= 1 && calculatorOperator.Length >= 1 && number2.Length >= 1)
            {
                return true;
            }
            else return false;
        }

        public Chunk GetChunk(string[] numberArray, string[] operatorArray)
        {
            Chunk chunk = new Chunk();


            int numNumbers = numberArray.Length;
            int numOperators = operatorArray.Length; //Number of operators in an expression will always be (numNumbers - 1)

            //check if there are enough 'ingredients' for a chunk be made
            if (numNumbers >= 2)
            {
                //create chunk
                int n = 0;
                chunk.number1 = numberArray[n];
                chunk.calculatorOperator = operatorArray[n];
                chunk.number2 = numberArray[n + 1];

                //update arrays
                numberArray[n] = "";
                numberArray[n + 1] = "";
                operatorArray[n] = "";
            }

            else
            {
                chunk.number1 = numberArray[0];
            }

            return chunk;
        }


    }
}