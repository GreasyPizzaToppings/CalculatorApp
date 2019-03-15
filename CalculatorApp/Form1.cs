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
            //Misc Buttons

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
            EventHandler[] eventMethodNames = { BtnClear_Click, Btn0_Click, BtnDecimalPoint_Click, BtnEquals_Click, Btn1_Click, Btn2_Click, Btn3_Click, BtnPlus_Click, Btn4_Click, Btn5_Click, Btn6_Click, BtnMinus_Click, Btn7_Click, Btn8_Click, Btn9_Click, BtnMultiply_Click, BtnSquareRoot_Click, BtnMemoryClear_Click, BtnMemoryPlus_Click, BtnDivide_Click, BtnExponent_Click, BtnMemoryRecall_Click, BtnMemoryMinus_Click, BtnBackspace_Click };
            string[] buttonSymbols = { "&C", "&0", "&.", "&=", "&1", "&2", "&3", "&+", "&4", "&5", "&6", "&-", "&7", "&8", "&9", "×", "√", "MC", "M+", "÷", "&^", "MR", "M-", "⌫" };

            //initialise button instance
            Button btnOperators = new Button();

            int btnX = 3;
            int btnY = 458;
            int btnHeight = 60; //One Unit = 30 pixels.
            int btnWidth = Convert.ToInt16(btnHeight * 1.25); //keeping with ratio in design

            //create the buttons
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

            //Assign each button to each click event
            for (int b = 0; b < buttonList.Count; b++)
            {
                Controls.Add(buttonList[b]); //add each button to the form

                buttonList[b].Click += new EventHandler(eventMethodNames[b]);
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

        //Add in the other keyboard shortcuts for accesskeys that were not applicable.
        private void CalculatorGUI_KeyUp(object sender, KeyEventArgs e)
        {   
            switch (e.KeyData)
            {
                case Keys.R: //'r' is the shortcut for roots
                    mainDisplay.Text += '√';
                    break;
                case (Keys.OemQuestion): // '/' is the shortcut for divide. Divide does not work, but oemquestion does.
                    mainDisplay.Text += '÷';
                    break;
                case (Keys.Shift | Keys.D8): //'*' is the shortcut for multiply
                    mainDisplay.Text += '×';
                    break;
                case Keys.Back:
                    Backspace();
                    break;
            }
        }

        private void CalculatorGUI_Resize(object sender, EventArgs e)
        {
            //Fixed values for the size of the calculator. Stops resizing, apart from fullscreen.
            ActiveForm.Width = 325;
            ActiveForm.Height = 560;
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
        void BtnDecimalPoint_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons

            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += ".";
        }

        void Btn0_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons

            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "0";
        }
        void Btn1_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons

            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "1";
        }
        void Btn2_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons

            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "2";
        }
        void Btn3_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons

            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "3";
        }
        void Btn4_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons

            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "4";
        }
        void Btn5_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons

            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "5";
        }
        void Btn6_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons

            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "6";
        }
        void Btn7_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons

            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "7";
        }
        void Btn8_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons

            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "8";
        }
        void Btn9_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons

            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "9";
        }
        #endregion

        #region Calculator Operator Buttons

        void BtnPlus_Click(object sender, EventArgs e)
        {
            //Reset all other operator colours, but make itself inverted
            SetOperatorButtonColours();
            InvertOperatorButtonColour(7); //Plus index is 7

            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "+";
        }

        void BtnMinus_Click(object sender, EventArgs e)
        {
            //Reset all other operator colours, but make itself inverted
            SetOperatorButtonColours();
            InvertOperatorButtonColour(11); //Minus index is 11
            
            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "-";
        }
        void BtnMultiply_Click(object sender, EventArgs e)
        {
            //Reset all other operator colours, but make itself inverted
            SetOperatorButtonColours();
            InvertOperatorButtonColour(15); //Multiply index is 15

            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "×";
        }
        void BtnSquareRoot_Click(object sender, EventArgs e)
        {
            //Reset all other operator colours, but make itself inverted
            SetOperatorButtonColours();
            InvertOperatorButtonColour(16); //Square root index is 16

            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "√";
        }
        void BtnDivide_Click(object sender, EventArgs e)
        {
            //Reset all other operator colours, but make itself inverted
            SetOperatorButtonColours();
            InvertOperatorButtonColour(19); //Divide index is 19

            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "÷";
        }
        
        void BtnExponent_Click(object sender, EventArgs e)
        {
            //Reset all other operator colours, but make itself inverted
            SetOperatorButtonColours();
            InvertOperatorButtonColour(20); //Exponent index is 20

            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "^";
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
            SetOperatorButtonColours(); //Reset any inverted operator buttons
            calculator.MemoryRecall();
        }
        void BtnMemoryMinus_Click(object sender, EventArgs e)
        {
            SetOperatorButtonColours(); //Reset any inverted operator buttons
            calculator.MemoryMinus();
        }
        #endregion

        //Other calculator user buttons
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
    }
}