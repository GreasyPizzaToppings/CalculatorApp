using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CalculatorApp
{
    public partial class CalculatorGUI : Form
    {

        List<Button> buttonList = new List<Button>(); //make list for operator buttons
        public static Label postCalculatedExpression, mainDisplay, preCalculatedExpression; //declare screen labels

        Calculator calculator = new Calculator(); //the calculation class object
        bool finishedCalculation = false;


        public CalculatorGUI()
        {
            InitializeComponent();

            //Initialise all the buttons and colour code them. 
            InitialiseCalculatorButtons();
            ChangeButtonColours();

            //make the input/output labels
            InitialiseCalculatorDisplays();
        }

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
                Size = new Size(labelWidth, 60),
                ForeColor = textColor,
                BackColor = Color.NavajoWhite,
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
            string[] buttonSymbols = { "AC", "0", ".", "=", "1", "2", "3", "+", "4", "5", "6", "-", "7", "8", "9", "×", "√", "MC", "M+", "÷", "^", "MR", "M-", "⌫" };

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
                else
                {
                    btnX += btnWidth + 1;
                }
            }

            //Assign each button to each click event
            for (int b = 0; b < buttonList.Count; b++)
            {
                Controls.Add(buttonList[b]); //add each button to the form

                buttonList[b].Click += new EventHandler(eventMethodNames[b]);
            }
        }

        public void ChangeButtonColours()
        {
            //Set blue colour for digit Buttons and decimal point
            int[] digitIndexes = { 1, 2, 4, 5, 6, 8, 9, 10, 12, 13, 14 };
            foreach (int index in digitIndexes)
            {
                buttonList[index].BackColor = Color.DarkGray;
            }

            //Set black background, white text for operator buttons
            int[] operatorIndexes = { 7, 11, 15, 16, 19, 20 };
            foreach (int index in operatorIndexes)
            {
                buttonList[index].BackColor = Color.Black;
                buttonList[index].ForeColor = Color.White;
            }

            int[] memoryIndexes = { 17, 18, 21, 22 };
            foreach (int index in memoryIndexes)
            {
                buttonList[index].BackColor = Color.DarkOrange;
            }

            //Misc Buttons

            //Clear button & Backspace, respectively
            buttonList[0].BackColor = Color.CornflowerBlue;
            buttonList[23].BackColor = Color.CornflowerBlue;

            //Equals Button
            buttonList[3].BackColor = Color.Yellow;

        }
        #endregion

        #region GUI Events

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
            //set maximum character input limit one entire screen's worth of digits, which is about 60 long.
            int maxLength = 60;

            if (mainDisplay.Text.Length >= maxLength)
            {
                mainDisplay.Text = mainDisplay.Text.Remove(maxLength - 1, 1) + "";
            }
        }

        #endregion

        #region Button Events

        //Click Events for each operator
        void BtnClear_Click(object sender, EventArgs e)
        {
            ClearDisplays();
        }
        void Btn0_Click(object sender, EventArgs e)
        {
            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "0";
        }
        void BtnDecimalPoint_Click(object sender, EventArgs e)
        {
            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += ".";
        }

        void BtnEquals_Click(object sender, EventArgs e)
        {
            finishedCalculation = false;

            string expression = mainDisplay.Text;

            //send input to be processed and calculated
            calculator.Start(expression);
            finishedCalculation = true;

        }

        void Btn1_Click(object sender, EventArgs e)
        {
            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "1";


        }
        void Btn2_Click(object sender, EventArgs e)
        {
            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "2";
        }
        void Btn3_Click(object sender, EventArgs e)
        {
            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "3";
        }
        void BtnPlus_Click(object sender, EventArgs e)
        {
            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "+";
        }
        void Btn4_Click(object sender, EventArgs e)
        {
            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "4";
        }
        void Btn5_Click(object sender, EventArgs e)
        {
            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "5";
        }
        void Btn6_Click(object sender, EventArgs e)
        {
            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "6";
        }
        void BtnMinus_Click(object sender, EventArgs e)
        {
            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "-";
        }
        void Btn7_Click(object sender, EventArgs e)
        {
            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "7";
        }
        void Btn8_Click(object sender, EventArgs e)
        {
            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "8";
        }
        void Btn9_Click(object sender, EventArgs e)
        {
            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "9";
        }
        void BtnMultiply_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += "×";
        }
        void BtnSquareRoot_Click(object sender, EventArgs e)
        {
            if (finishedCalculation)
            {
                ClearDisplays();
                finishedCalculation = false; //once you input stuff, you start a new calculation
            }
            mainDisplay.Text += "√";
        }
        void BtnMemoryClear_Click(object sender, EventArgs e)
        {
            mainDisplay.Text = "Memory clear";
        }
        void BtnMemoryPlus_Click(object sender, EventArgs e)
        {
            mainDisplay.Text = "Memory plus";
        }
        void BtnDivide_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += "÷";
        }
        void BtnExponent_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += "^";
        }
        void BtnMemoryRecall_Click(object sender, EventArgs e)
        {
            mainDisplay.Text = "Memory recall";
        }
        void BtnMemoryMinus_Click(object sender, EventArgs e)
        {
            mainDisplay.Text = "Memory Minus";
        }

        void BtnBackspace_Click(object sender, EventArgs e)
        {
            //delete the last character if there is input already
            if (mainDisplay.Text.Length >= 1)
            {
                mainDisplay.Text = mainDisplay.Text.Remove((mainDisplay.Text.Length - 1), 1) + "";
            }

        }

        #endregion

        public void ClearDisplays()
        {
            //clear all 3 displays
            postCalculatedExpression.Text = "";
            mainDisplay.Text = "";
            preCalculatedExpression.Text = "";
        }

    }
}

//Potential features to incorporate once basic concepts are working:
//1. BEDMAS on operations, and to not just execute sequentially.
//2. Various colour schemes that can be selected.
//3. Make typing an available input into the calculator. 

//TODO:
//Implement memory functions
//Try a full range of inputs and test for errors
//Implement pre-calculation feature
//Make operator buttons change apperance when used.

//Problems:
//Pressing equals multiple times makes a second '=' appear in the postcalculation label