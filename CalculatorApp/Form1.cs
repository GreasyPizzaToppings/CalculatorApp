using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace CalculatorApp
{
    public partial class CalculatorGUI : Form
    {

        List<Button> buttonList = new List<Button>(); //make list for operator buttons
        public static Label postCalculatedExpression, mainDisplay, preCalculatedExpression; //declare screen labels
        bool finishedCalculation = false; //Needed for display clearing purposes
        Calculator calculator = new Calculator(); //the calculation class object

        public CalculatorGUI()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            //Initialise all the buttons and colour code them. 
            InitialiseCalculatorButtons();
            SetButtonColours();

            //make the input/output displays
            InitialiseCalculatorDisplays();
        }

        public void ClearDisplays()
        {
            //clear all 3 displays
            postCalculatedExpression.Text = "";
            mainDisplay.Text = "";
            preCalculatedExpression.Text = "";
        }

        #region Colour Changing
        public void SetDigitButtonColours()
        {
            //Set blue colour for digit Buttons and decimal point
            int[] digitIndexes = { 1, 2, 4, 5, 6, 8, 9, 10, 12, 13, 14 };
            foreach (int index in digitIndexes)
            {
                buttonList[index].BackColor = Color.DarkGray;
            }
        }

        public void SetOperatorButtonColours()
        {
            //Set black background, white text for operator buttons
            int[] operatorIndexes = { 7, 11, 15, 16, 19, 20 };
            foreach (int index in operatorIndexes)
            {
                buttonList[index].BackColor = Color.Black;
                buttonList[index].ForeColor = Color.White;
            }
        }

        public void InvertOperatorButtonColour(int btnIndex)
        {
            buttonList[btnIndex].BackColor = Color.White;
            buttonList[btnIndex].ForeColor = Color.Black;
        }

        public void SetMemoryButtonColours()
        {
            int[] memoryIndexes = { 17, 18, 21, 22 };
            foreach (int index in memoryIndexes)
            {
                buttonList[index].BackColor = Color.DarkOrange;
            }
        }

        public void SetOtherButtonColours()
        {
            //Clear button & Backspace, respectively
            buttonList[0].BackColor = Color.CornflowerBlue;
            buttonList[23].BackColor = Color.CornflowerBlue;

            //Equals Button
            buttonList[3].BackColor = Color.Yellow;
        }
        #endregion
        #region Setup Calculator GUI
        public void InitialiseCalculatorDisplays()
        {
            //displays fit perfectly inside the border that is drawn
            int labelWidth = 298;
            int labelHeight = 32;

            const int startingX = 5;
            const int startingY = 7;
            const int gap = 3;

            Color textColor = Color.Black;
            //make custom light green
            Color displayColor = ColorTranslator.FromHtml("#b4eac7");

            postCalculatedExpression = new Label
            {
                Size = new Size(labelWidth, labelHeight),
                ForeColor = textColor,
                BackColor = displayColor,
                Location = new Point(startingX, startingY),
                Font = new Font("Arial", 12),
                TextAlign = ContentAlignment.MiddleLeft
                
            };

            Controls.Add(postCalculatedExpression);

            mainDisplay = new Label
            {
                Size = new Size(labelWidth, Convert.ToInt16(labelHeight * 1.95)),
                ForeColor = textColor,
                BackColor = displayColor,
                Location = new Point(startingX, startingY + labelHeight + gap),
                Font = new Font("Arial", 15),
                TextAlign = ContentAlignment.MiddleLeft

            };

            mainDisplay.TextChanged += new EventHandler(MainDisplay_TextChanged);
            Controls.Add(mainDisplay);

            preCalculatedExpression = new Label
            {
                Size = new Size(labelWidth, labelHeight),
                ForeColor = textColor,
                BackColor = displayColor,
                Location = new Point(startingX, startingY + Convert.ToInt16(labelHeight * 2.86) + gap),
                Font = new Font("Arial", 12),
                TextAlign = ContentAlignment.MiddleLeft,
            };

            Controls.Add(preCalculatedExpression);
        }

        public void InitialiseCalculatorButtons()
        {
            string[] buttonSymbols = { "C", "0", ".", "=", "1", "2", "3", "+", "4", "5", "6", "-", "7", "8", "9", "×", "√", "MC", "M+", "÷", "^", "MR", "M-", "⌫" };
            Button btnOperators = new Button(); //The button object to be used to create all buttons

            int btnX = 3;
            int btnY = 458;
            int btnHeight = 60; //One Unit = 30 pixels.
            int btnWidth = Convert.ToInt16(btnHeight * 1.25); //keeping with ratio in design

            //Create all buttons
            for (int buttonCount = 0; buttonCount < buttonSymbols.Length; buttonCount++)
            {
                btnOperators = new Button
                {
                    //Specify button parameters
                    Location = new Point(btnX, btnY),
                    Size = new Size(btnWidth, btnHeight),
                    Font = new Font("Arial", 18),
                    FlatStyle = FlatStyle.Popup,
                    //Add the correct symbol to each button
                    Text = buttonSymbols[buttonCount]
                };

                //Add button to list
                buttonList.Add(btnOperators);

                //check for grid alignment
                if (btnX >= (3 * btnWidth))
                {
                    btnX = 3; //reset and align grid upwards
                    btnY -= (btnHeight + 2);
                }
                else btnX += btnWidth + 1;
            }

            //Add all buttons to form
            foreach (Button calculatorButton in buttonList) Controls.Add(calculatorButton);

            //Assign all number buttons to the same event
            int[] digitIndexes = { 1, 2, 4, 5, 6, 8, 9, 10, 12, 13, 14 };
            foreach (int digitIndex in digitIndexes) buttonList[digitIndex].Click += new EventHandler(BtnNumericals_Click);

            //Assign all operator buttons to the same event
            int[] operatorIndexes = { 7, 11, 15, 16, 19, 20 };
            foreach (int operatorIndex in operatorIndexes) buttonList[operatorIndex].Click += new EventHandler(BtnOperators_Click);

            //Assign the other buttons to their correct event
            for (int buttonIndex = 0; buttonIndex < buttonList.Count; buttonIndex++)
            {
                switch (buttonIndex)
                {
                    case 0: //The clear button
                        buttonList[buttonIndex].Click += new EventHandler(BtnClear_Click);
                        break;
                    case 3: //The equals button
                        buttonList[buttonIndex].Click += new EventHandler(BtnEquals_Click);
                        break;
                    case 17: //The Memory Clear button
                        buttonList[buttonIndex].Click += new EventHandler(BtnMemoryClear_Click);
                        break;
                    case 18: //The Memory Plus button
                        buttonList[buttonIndex].Click += new EventHandler(BtnMemoryPlus_Click);
                        break;
                    case 21: //The Memory Recall button
                        buttonList[buttonIndex].Click += new EventHandler(BtnMemoryRecall_Click);
                        break;
                    case 22: //The Memory Minus button
                        buttonList[buttonIndex].Click += new EventHandler(BtnMemoryMinus_Click);
                        break;
                    case 23: //The Backspace button
                        buttonList[buttonIndex].Click += new EventHandler(BtnBackspace_Click);
                        break;
                }
            }
        }

        public void SetButtonColours()
        {
            //Call all the button colour changing methods from one place for initialisation
            SetDigitButtonColours();
            SetOperatorButtonColours();
            SetMemoryButtonColours();
            SetOtherButtonColours();
        }
        #endregion
        #region GUI Events
        private void CalculatorGUI_Resize(object sender, EventArgs e)
        {
            //Fixed values for the size of the calculator. Stops resizing, apart from fullscreen.
            try
            {
                ActiveForm.Width = 325;
                ActiveForm.Height = 560;
            }
            catch { } //Moving one window to the side and then clicking on the calculator to fill the other half of the screen will cause errors. 
        }

        private void CalculatorGUI_Paint(object sender, PaintEventArgs e) //Draws the border around the displays
        {
            Graphics drawArea = e.Graphics;
            Pen borderPen = new Pen(Color.Black, 3);

            Rectangle borderRectangle = new Rectangle(3, 5, 301, 130);
            drawArea.DrawRectangle(borderPen, borderRectangle);

            //draw the line separating the main label from the postcalculation label
            drawArea.DrawLine(borderPen, new Point(3, 40), new Point(303, 40));
        }

        private void MainDisplay_TextChanged(object sender, EventArgs e)
        {
            //Limit input length to 60 character.
            int maxLength = 60;
            if (mainDisplay.Text.Length >= maxLength)
            {
                mainDisplay.Text = mainDisplay.Text.Remove(maxLength - 1, 1) + "";
            }

            //Try and pre-calculate their current expression, if the user were to press equals, but not after they have already pressed equals
            if (mainDisplay.Text.Contains('=') == false)
            {
                string expression = mainDisplay.Text;
                calculator.Start(expression, true);
            }
        }
        #endregion
        #region Button Events

        #region Numerical Input Buttons
        void BtnNumericals_Click(object sender, EventArgs e) //Includes 0-9 and .
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons
            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            
            //Display the correct numerical character 
            Button numericalsButton = sender as Button;
            mainDisplay.Text += numericalsButton.Text; 
        }
        #endregion

        #region Calculator Operator Buttons
        void BtnOperators_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset all other operator colours
            //Obtain correct index to invert the correct operator button
            int operatorIndex = 0;
            Button operatorButton = sender as Button;
            switch (operatorButton.Text)
            {
                case "+":
                    operatorIndex = 7;
                    break;
                case "-":
                    operatorIndex = 11;
                    break;
                case "×":
                    operatorIndex = 15;
                    break;
                case "√":
                    operatorIndex = 16;
                    break;
                case "÷":
                    operatorIndex = 19;
                    break;
                case "^":
                    operatorIndex = 20;
                    break;
            }

            InvertOperatorButtonColour(operatorIndex); 

            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }

            //Display the correct operator on screen
            mainDisplay.Text += operatorButton.Text;
        }
        #endregion

        #region Calculator Memory Buttons
        void BtnMemoryClear_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons
            calculator.MemoryClear(); 
        }
        void BtnMemoryPlus_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons
            calculator.MemoryPlus();
        }
        void BtnMemoryRecall_Click(object sender, EventArgs e)
        {
            finishedCalculation = false;
            SetOperatorButtonColours(); //Reset any inverted operator buttons
            calculator.MemoryRecall();
        }
        void BtnMemoryMinus_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons
            calculator.MemoryMinus();
        }
        #endregion

        #region Other Calculator Buttons
        void BtnClear_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons
            ClearDisplays();
        }

        void BtnEquals_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons
            finishedCalculation = false;

            //remove any initial '= ' characters from previous calculations being shown
            string expression = mainDisplay.Text;
            if (expression.Length >= 2 && expression.ElementAt(0) == '=') expression = expression.Remove(0, 2) + null;

            //send input to be processed and calculated
            calculator.Start(expression, false);

            //Reset the precalculation label, as the mainDisplay is now being used for the same purpose.
            preCalculatedExpression.Text = "";

            finishedCalculation = true;
        }

        void BtnBackspace_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons
            Backspace();
        }

        void Backspace()
        {
            //delete the last character if there is input already
            if (mainDisplay.Text.Length >= 1)
            {
                mainDisplay.Text = mainDisplay.Text.Remove((mainDisplay.Text.Length - 1), 1) + "";
            }
        }
        #endregion
        #endregion
    }
}