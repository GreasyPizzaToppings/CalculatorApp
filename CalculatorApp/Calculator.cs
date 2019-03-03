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
        //split input into small chunks, and then calculate individually via other methods
        public void Calculate(string expression, Label postCalculatedExpression, Label mainDisplay, Label preCalculatedExpression)
        {
            //reset label text from before
            mainDisplay.Text = "";

            //-----step 1: Filter invalid input expressions-----
            if (IsValid(expression) == false)
            {
                mainDisplay.Text = "Syntax Error!";
                return;
            }


            //-----step 2: Produce and refine arrays of operators and number values-----

            //Extract array of numbers, and remove empty elements of size nConsecutiveOperators - 1
            string[] numbers = expression.Split('-', '+', '÷', '×', '^', '√');
            numbers = numbers.Where(z => !string.IsNullOrWhiteSpace(z)).ToArray();

            string startingOperator = "";
            bool firstDigitDetected = false;
            int charNumber = 0;
            Regex numberParts = new Regex(@"[0-9.]"); 

            //obtain first whole number like -++3.11
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

            //check for numbers with two non-consecutive decimal points
            foreach (string number in numbers)
            {
                var decimalCount = number.Count(x => x == '.');
                if(decimalCount >= 2) 
                {
                    mainDisplay.Text = "Syntax Error";
                    return;
                }
            }

            //-----Extract array of operators

            //split by non-operators
            string[] operators = expression.Split('1','2','3','4','5','6','7','8','9','0', '.');

            //remove first element, as it is part of the number
            operators[0] = "";

            //remove blank elements
            operators = operators.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            //Further refine number and operator list
            for (int operatorIndex = 0; operatorIndex <= (operators.Length - 1); operatorIndex++)
            {
                string operatorSequence = operators[operatorIndex];

                //check if a sequence of 2 or more
                if (operatorSequence.Length > 1)
                {
                    //Part of number is the whole sequence minus the first symbol
                    string partOfNumber = operatorSequence;
                    partOfNumber = partOfNumber.Remove(0, 1) + "";

                    //Extract real operator and update array
                    string realOperator = operatorSequence.ElementAt(0).ToString();
                    operators[operatorIndex] = realOperator;
                    
                    //add one to index number, because we want to skip the starting number
                    numbers[operatorIndex + 1] = partOfNumber + numbers[operatorIndex + 1];
                }
            }

            #region Print Array Contents

            Console.WriteLine("Array contents after refining");

            //For debugging/developing purposes. Remove in final version
            foreach (string operatorSymbol in operators)
            {
                Console.WriteLine("Detected operator: " + operatorSymbol);
            }

            foreach (string number in numbers)
            {
                Console.WriteLine("Detected number: " + number);
            }
            #endregion


            //-----Step 3: Simplify the numbers as much as possible, to make them workable.-----
            //E.g. refine -+---7.11 to 7.11 so they can be used later when chunked.






            //-----step 4: Break expression into a workable chunk of the form Number, Operator, Number-----

            //Create chunk and update arrays
            Chunk chunk = new Chunk();
            chunk = GetChunk(numbers, operators);

            numbers = numbers.Where(z => !string.IsNullOrWhiteSpace(z)).ToArray();
            operators = operators.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();


            Console.WriteLine("first chunk is: {0}, {1}, {2}", chunk.number1, chunk.calculatorOperator, chunk.number2);

            //if no full chunks are made, the answer requires no computation
            if (chunk.IsFull() == false)
            {
                //show answer
                mainDisplay.Text = "= " + numbers[0];
                postCalculatedExpression.Text = "= " + numbers[0];
                return;
            }

            //-----Step 4: Calculate each chunk and return as number, and update number array------





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
        }


        private Chunk GetChunk(string[] numberArray, string[] operatorArray)
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
                chunk.number2 = numberArray[n+1];

                //update arrays and remove empty elements
                numberArray[n] = "";
                numberArray[n + 1] = "";
                operatorArray[n] = "";
            }

            return chunk;
        }

        private bool IsValid(string expression)
        {
            if (expression.Length == 0) return false;

            //if no digits, then invalid
            Regex digits = new Regex(@"[0-9]");
            if (digits.Matches(expression).Count == 0)
            {
                return false;
            }

            //Check for expressions starting with '×','÷','^' as they are invalid. Others are ok.
            string startSymbol = expression.ElementAt(0).ToString();
            Regex badStartingSymbols = new Regex(@"[\÷×^]");

            if (badStartingSymbols.Matches(startSymbol).Count >= 1)
            {
                return false;
            }

            //Check for expressions ending with operators (not including '.')
            string endingSymbol = expression.ElementAt(expression.Length - 1).ToString();
            Regex okEndingSymbols = new Regex(@"[0-9.]");

            if (okEndingSymbols.Matches(endingSymbol).Count == 0)
            {
                return false;
            }

            //Check for one of the 19 possible invalid 2-symbol combinations.
            string[] invalidOperatorCombos = { "÷÷", "÷×", "÷^", "×÷", "××", "×^", "^÷", "^×", "^^", "√÷", "√×", "√^", "-÷", "-×", "-^", "+÷", "+×", "+^", ".." };

            foreach (string combo in invalidOperatorCombos)
            {
                if (expression.Contains(combo))
                {
                    return false;
                }
            }

            //It is presumed to be valid if all checks passed
            return true;
        }

        //Maybe give the job of refining arrays of operators and numbers to a method.

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
}

//Steps for calculating that need to be implemented
//1. Refine the list of numbers and operators to be accurate and as simple as possible. 
//This involves assigning only the first operator in a sequence to the operator list, and all the rest to 
//a part of the number. E.g. 2*-++7 Gets split into numbers: "2", "-++7". Operators: "*".
//2. Simplify the list of numbers to their base form so that they can be used later. From the example,
//it will be refined as numbers: "2", "-7".
//3. Now we build the chunks of number, operator, number (NON). So we will get ("2", "*", "-++7").
//4. Call the appropriate method by identifying the operator, and pass in each number. 
//5. Return the result (-14), and as it is just a number, we add it to the first available slot in the 
//number array like before. Remove empty spots in the number array.
//6. Repeat the process of building and calculating chunks until there is one number left. 
//7. Return that number, and you are done. 