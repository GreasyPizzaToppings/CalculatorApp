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
            
            //Once input has been properly processed/setup for calculation, begin calculating.
            string answer = ComputeExpression(numbers, operators);

            //Display answer and input to user
            SetLabels(expression, answer);
        }

        private string ComputeExpression(string[] numbers, string[] operators)
        {
            string answer = ""; //keep answer in terms of string for display purposes

            #region Print Array Contents

            Console.WriteLine("Working numbers and operators include: ");

            //For debugging/developing purposes. Remove in final version
            foreach (string operatorSymbol in operators)
            {
                Console.WriteLine("Operator: " + operatorSymbol);
            }

            foreach (string number in numbers)
            {
                Console.WriteLine("Number: " + number);
            }
            #endregion


            //-----step 1: Break expression into a workable chunk of the form Number, Operator, Number-----

            //Create chunk and update arrays
            Chunk chunk = new Chunk();
            chunk = chunk.GetChunk(numbers, operators);

            numbers = numbers.Where(z => !string.IsNullOrWhiteSpace(z)).ToArray();
            operators = operators.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();


            Console.WriteLine("first chunk is: {0}, {1}, {2}", chunk.number1, chunk.calculatorOperator, chunk.number2);

            //if no full chunks are made, the answer requires no computation, and is always the first element in numbers[]
            if (chunk.IsFull() == false)
            {
                answer = numbers[0]; 
            }

            //-----Step 2: Calculate each chunk and return as number, and update number array------


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

            //Getting working numbers involves simplifying them.
            numbers = SimplifyNumbers(numbers);

            return numbers;
        }

        private string[] SimplifyNumbers(string[] numberArray)

        {
            //obtaining the operator sequence
            Regex numberParts = new Regex(@"[0-9.√]");

            //Search through all the number entries
            for (int numberIndex = 0; numberIndex <= numberArray.Length - 1; numberIndex++)
            {
                string minusSequence = "";
                string number = numberArray[numberIndex];
                string baseNumber = "";

                Console.WriteLine("starting number is: " + number);

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

                Console.WriteLine("detected minus sequence for the number is" + minusSequence);
                //update number //minus sequence + base or root number
                Console.WriteLine("Refined number is " + minusSequence + baseNumber);

                numberArray[numberIndex] = minusSequence + baseNumber;
            }


            return numberArray;
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

                    Console.WriteLine("The refined operator is " + realOperator);
                    operators[operatorIndex] = realOperator;
                }
            }

            return operators;
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
        private void Add()
        {

        }
        private void Subtract()
        {

        }

        private void Multiply()
        {

        }

        private void Divide()
        {

        }

        private void SquareRoot()
        {

        }

        private void Exponent()
        {

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

                //update arrays and remove empty elements
                numberArray[n] = "";
                numberArray[n + 1] = "";
                operatorArray[n] = "";
            }

            return chunk;
        }


    }
}

//Steps for calculating that need to be implemented
//while(chunks are full) {
//Build chunks by using GetChunk()
//Compute chunk by calling appropriate method, returning the result.
//Update number array with computed value from previous chunk.
//}
//return final value when chunks are not full.



