using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace CalculatorApp
{
    /// This class holds all the calculation functions that a normal calculator has. 
    /// It is where the input from the user gets split into chunks and individually calculated/processed.
    /// The other class called CalculatorGUI is only for setting up the visual design and layout of the calculator.

    class Calculator
    {
        //Set the global memory value for memory operations
        private double memoryValue = 0.00;

        //Start working with the given expression, and try and calculate with it. 
        //Flag parameter indicates if this is a pre or post-equals-press expression.
        public void Start(string expression, bool preEqualsPress)
        {
            //Exponent numbers in input can only be the result of a previous calcuation. Return immediately.
            if (expression.Contains("E")) return; 

            //Check for invalid inputs, only if user presses equals
            if (IsValid(expression, preEqualsPress) == false)
            {
                return; //Could not set up successfully, then exit.
            }

            //Obtain cleaned operators and numbers needed for calculation(s) 
            string[] numbers = ObtainWorkingNumbers(expression);
            string[] operators = ObtainWorkingOperators(expression);

            //Begin calculating and display result
            string answer = ComputeExpression(numbers, operators);
            
            //Simplify any long answers for ease of display
            answer = Exponent(answer, "1");

            SetLabels(expression, answer, preEqualsPress);
        }

        private string ComputeExpression(string[] numbers, string[] operators)
        {
            //Calculate expression by building 'chunks' of the form number, operator, number. 
            Chunk chunk = new Chunk();

            do
            {
                chunk = chunk.GetChunk(numbers, operators);

                if (chunk.IsFull() == false) break;

                //Evaluate by calling the appropriate method
                switch (chunk.calculatorOperator)
                {
                    case "+":
                        numbers[0] = Add(chunk.number1, chunk.number2);
                        break;
                    case "-":
                        numbers[0] = Subtract(chunk.number1, chunk.number2);
                        break;
                    case "÷":
                        numbers[0] = Divide(chunk.number1, chunk.number2);
                        break;
                    case "×":
                        numbers[0] = Multiply(chunk.number1, chunk.number2);
                        break;
                    case "^":
                        numbers[0] = Exponent(chunk.number1, chunk.number2);
                        break;
                }

                //Remove empty entries in arrays
                numbers = numbers.Where(z => !string.IsNullOrWhiteSpace(z)).ToArray();
                operators = operators.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            } while (chunk.IsFull());

            //result is always the first number in the partially filled chunk
            string answer = chunk.number1;
            return answer;
        }

        #region Numerical Calculator Functions
        private string Add(string firstNumber, string secondNumber)
        {
            double total = Convert.ToDouble(firstNumber) + Convert.ToDouble(secondNumber);
            string textAnswer = total.ToString();
            return textAnswer;
        }
        private string Subtract(string firstNumber, string secondNumber)
        {
            double total = Convert.ToDouble(firstNumber) - Convert.ToDouble(secondNumber);
            string textAnswer = total.ToString();
            return textAnswer;
        }

        private string Multiply(string firstNumber, string secondNumber)
        {
            double total = Convert.ToDouble(firstNumber) * Convert.ToDouble(secondNumber);
            string textAnswer = total.ToString();
            return textAnswer;
        }

        private string Divide(string numerator, string denominator)
        {
            double total = Convert.ToDouble(numerator) / Convert.ToDouble(denominator);
            string textAnswer = total.ToString();
            return textAnswer;
        }

        private string Exponent(string baseNumber, string power)
        {
            double workingBase = Convert.ToDouble(baseNumber);
            double workingExponent = Convert.ToDouble(power);

            double answer = Math.Pow(workingBase, workingExponent);

            string textAnswer = answer.ToString();
            return textAnswer;
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

        #region Memory Calculator Functions

        public void MemoryClear()
        {
            memoryValue = 0; //clearing the value is the same as setting it to zero.
        }

        public void MemoryPlus()
        {
            string inputText = CalculatorGUI.mainDisplay.Text;

            //Try and add the current value in the display to the memory
            try
            {
                //Remove any equals signs from previous calculations, as it should be able to add previously calculated values to memory
                if (inputText.Contains('=') == true)
                {
                    inputText = inputText.Remove(0, 2) + null; //remove '= ' 
                }

                double valueToSubtract = Convert.ToDouble(inputText);
                memoryValue += valueToSubtract; //Finally add value to memory
            }
            catch
            {
                //Display an error if their input was not nothing.
                if (inputText.Length >= 1) DisplayError("Invalid Numeric value to subtract from Memory", false);
            }
        }

        public void MemoryMinus()
        {
            string inputText = CalculatorGUI.mainDisplay.Text;

            //Try and add the current value in the display to the memory
            try
            {
                //Remove any equals signs from previous calculations, as it should be able to add previously calculated values to memory
                if (inputText.Contains('=') == true)
                {
                    inputText = inputText.Remove(0, 2) + null; //remove '= ' 
                }

                double valueToSubtract = Convert.ToDouble(inputText);
                memoryValue -= valueToSubtract; //Finally add value to memory
            }
            catch
            {
                //Display an error if their input was not nothing.
                if (inputText.Length >= 1) DisplayError("Invalid Numeric value to subtract from Memory", false);
            }
        }

        public void MemoryRecall()
        {
            if (memoryValue == 0) return; //Avoid adding a zero to their input.

            //Print out exponent number manually. Avoid putting 'E' into the input expression.
            if (memoryValue.ToString().Contains('E'))
            {
                 
                string[] numberParts = memoryValue.ToString().Split('E');
                string baseNumber = numberParts[0];
                //remove decimal point if applicable
                if (baseNumber.Contains('.'))
                {
                    baseNumber = baseNumber.Remove(baseNumber.IndexOf('.'), 1) + "";
                }

                int exponent = Convert.ToInt32(numberParts[1]);

                string fullNumber = "";

                if (exponent < 0) //Negative exponents have n-1 zeroes to the right of the decimal point before base num 
                {
                    //Give the start of the number the correct sign.
                    if (Convert.ToInt16(baseNumber) < 0)
                    {
                        fullNumber = "-0.";
                        baseNumber = baseNumber.Remove(0, 1) + "";
                    }
                    else fullNumber = "0.";
                    
                    //Add in the zeroes
                    for (int j = 1; j < exponent * -1; j++) { fullNumber += "0"; }
                    fullNumber += baseNumber;
                }

                else //zeroes go the right, like normal
                {
                    fullNumber += baseNumber;
                    for (int i = 0; i < exponent; i++) { fullNumber += "0"; }
                }

                //Display full number with no Exponent.
                CalculatorGUI.mainDisplay.Text += fullNumber;
                return;
            }

        
            CalculatorGUI.mainDisplay.Text += memoryValue.ToString();
        }
        
        #endregion


        #region Routines To Obtain Workable Input 

        //Check for errors in input. Only display then if equals has been pressed by user
        private bool IsValid(string expression, bool preEqualsPress)
        {
            //Error message not required for this invalid case
            if (expression.Length == 0) return false;

            //if no digits, then invalid
            Regex digits = new Regex(@"[0-9]");
            if (digits.Matches(expression).Count == 0)
            {
                DisplayError("No Digits In Input", preEqualsPress);
                return false;
            }

            //Check for expressions starting with '×','÷','^' as they are invalid. Others are ok.
            string startSymbol = expression.ElementAt(0).ToString();
            Regex badStartingSymbols = new Regex(@"[\÷×^]");

            if (badStartingSymbols.Matches(startSymbol).Count >= 1)
            {
                DisplayError("Illegal starting symbol of " + startSymbol, preEqualsPress);
                return false;
            }

            //Check for expressions ending with operators (not including '.')
            string endingSymbol = expression.ElementAt(expression.Length - 1).ToString();
            Regex okEndingSymbols = new Regex(@"[0-9.]");

            if (okEndingSymbols.Matches(endingSymbol).Count == 0)
            {
                DisplayError("Illegal ending symbol of " + endingSymbol, preEqualsPress);
                return false;
            }

            //Check for possible invalid 2-symbol combinations.
            string[] invalidOperatorCombos = { "÷÷", "÷×", "÷^", "×÷", "××", "×^", "^÷", "^×", "^^", "√÷", "√×", "√^", "-÷", "-×", "-^", "+÷", "+×", "+^", "..", "0√", "1√", "2√", "3√", "4√", "5√", "6√", "7√", "8√", "9√" };

            foreach (string combo in invalidOperatorCombos)
            {
                if (expression.Contains(combo))
                {
                    DisplayError("Illegal symbol combo of " + combo, preEqualsPress);
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
                        DisplayError("Non-real number from √", preEqualsPress);
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
                    DisplayError("Illegal quantity of decimal points in a number", preEqualsPress);
                    return false;
                }
            }

            return true;
        }

        //Clean numbers
        private string[] ObtainWorkingNumbers(string expression)
        {
            //Extract numbers. Only split by - and + if the previous character was not an E
            string[] numbers = expression.Split('-', '+', '÷', '×', '^', '√');
            
            //Remove empty elements of size nConsecutiveOperators - 1
            numbers = numbers.Where(z => !string.IsNullOrWhiteSpace(z)).ToArray();


            //obtain first whole number, including any operators
            string startingOperator = "";
            bool firstDigitDetected = false;
            int charNumber = 0;
            Regex numberParts = new Regex(@"[0-9.E]");

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
            numbers = SimplifyArrayNumbers(numbers);

            //Remove the '√' sign from the number by evaluation to make them truly workable
            numbers = EvaluateRoots(numbers);

            return numbers;
        }

        private string[] SimplifyArrayNumbers(string[] numberArray)
        {
            //obtaining the operator sequence
            Regex numberParts = new Regex(@"[0-9.√E]");

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

        //Clean operators
        private string[] ObtainBaseOperators(string expression)
        {
            //new lines. Testing
            string[] stringSeparators = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "."};
            string[] operators = expression.Split(stringSeparators, StringSplitOptions.None);

            //remove first element, as it is part of the number
            operators[0] = "";

            //remove blank elements
            operators = operators.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            return operators;
        }

        private string[] ObtainWorkingOperators(string expression)
        {
            //new lines. Testing
            string[] stringSeparators = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "."};
            string[] operators = expression.Split(stringSeparators, StringSplitOptions.None);

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
        #endregion

        #region Display Functions
        private void DisplayError(string reason, bool preEqualsPress)
        {
            if (!preEqualsPress) CalculatorGUI.mainDisplay.Text = "Error: " + reason;
            
            //Avoid showing errors for this label, as there will be plenty as user is completing their expression
            else CalculatorGUI.preCalculatedExpression.Text = ""; 
        }

        private void SetLabels(string expression, string answer, bool preEqualsPress)
        {
            if (preEqualsPress) //Then only set the precalculation label. 
            {
                CalculatorGUI.preCalculatedExpression.Text = "= " + answer;
                return; 
            }

            //Expressions more than the maxDigits causes the text to change vertical alignment and be invisible
            int maxDigits = 29;
            if (expression.Length > maxDigits)
            {
                int excessDigits = expression.Length - maxDigits;

                //remove excess digits to make it fit in the display
                string alteredExpression = expression;
                alteredExpression = alteredExpression.Remove(alteredExpression.Length - (excessDigits + 1), excessDigits) + "";

                //Set label, and show user their input has been truncated
                CalculatorGUI.postCalculatedExpression.Text = "= " + alteredExpression + "...";
            }

            else CalculatorGUI.postCalculatedExpression.Text = "= " + expression;

            //Display answer
            CalculatorGUI.mainDisplay.Text = "= " + answer;
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

            //check if there are enough 'ingredients' for a chunk be made
            if (numNumbers >= 2)
            {
                //create chunk
                int n = 0;
                chunk.number1 = numberArray[n];
                chunk.calculatorOperator = operatorArray[n];
                chunk.number2 = numberArray[n + 1];

                //update arrays, only if equals has been pressed

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