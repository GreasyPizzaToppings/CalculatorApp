using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalculatorApp
{
    public partial class CalculatorGUI : Form
    {
        public CalculatorGUI()
        {
            InitializeComponent();

            //Initialise all the buttons and colour code them. 
            InitialiseCalculatorButtons();
            ChangeButtonColours();

            //make the input/output textboxes and labels
            //InitialiseCalculatorDisplays();
            DrawDisplayBorder();
        }

        List<Button> buttonList = new List<Button>(); //make list for operator buttons

        Label postCalculatedExpression, mainDisplay, preCalculatedResult; //declare screen labels

        public void InitialiseCalculatorDisplays()
        {
            
            int labelWidth = 295;
            int labelHeight = 30;

            postCalculatedExpression = new Label();
            postCalculatedExpression.Size = new Size(labelWidth, labelHeight);
            postCalculatedExpression.BackColor = Color.DarkGray;
            postCalculatedExpression.Location = new Point(5, 3);
            postCalculatedExpression.Font = new Font("Arial", 12);
            postCalculatedExpression.TextAlign = ContentAlignment.MiddleLeft;
            //test text
            postCalculatedExpression.Text = "24*3/2-1.003^3";
            Controls.Add(postCalculatedExpression);

            mainDisplay = new Label();
            mainDisplay.Size = new Size(labelWidth, labelHeight * 2);
            mainDisplay.BackColor = Color.DarkGray;
            mainDisplay.Location = new Point(5, 35);
            mainDisplay.Font = new Font("Arial", 15);
            mainDisplay.TextAlign = ContentAlignment.MiddleLeft;
            Controls.Add(mainDisplay);

            preCalculatedResult = new Label();
            preCalculatedResult.Size = new Size(labelWidth, labelHeight);
            preCalculatedResult.BackColor = Color.DarkGray;
            preCalculatedResult.Location = new Point(5, 97);
            preCalculatedResult.Font = new Font("Arial", 12);
            preCalculatedResult.TextAlign = ContentAlignment.MiddleLeft;
            //test text
            preCalculatedResult.Text = "552.1832";
            Controls.Add(preCalculatedResult);
        }

        public void DrawDisplayBorder()
        {
            //make picturebox for draw area
            PictureBox displayArea = new PictureBox();
            displayArea.Size = new Size(295, 124); //exactly as high as all 3 displays and the gap
            displayArea.Location = new Point(5, 3);
            displayArea.BackColor = Color.White;
            Controls.Add(displayArea);

            Graphics drawArea = displayArea.CreateGraphics();

            //make border
            Pen borderPen = new Pen(Color.Black);
            borderPen.Width = 50;
            Point xPoint = displayArea.Location;
            
            //y u no work?
            drawArea.DrawLine(borderPen, new Point(xPoint.X, xPoint.Y), new Point(xPoint.X + 50, xPoint.Y + 50));

            Rectangle borderRectangle = new Rectangle(5, 3, 295, 124);
            drawArea.DrawRectangle(borderPen, borderRectangle);
        }

        public void InitialiseCalculatorButtons()
        {
            EventHandler[] eventMethodNames = { BtnClear_Click, Btn0_Click, BtnDecimalPoint_Click, BtnEquals_Click, Btn1_Click, Btn2_Click, Btn3_Click, BtnPlus_Click, Btn4_Click, Btn5_Click, Btn6_Click, BtnMinus_Click, Btn7_Click, Btn8_Click, Btn9_Click, BtnMultiply_Click, BtnSquareRoot_Click, BtnMemoryClear_Click, BtnMemoryPlus_Click, BtnDivide_Click, BtnWedge_Click, BtnMemoryRecall_Click, BtnMemoryMinus_Click, BtnBackspace_Click };
            string[] buttonSymbols = { "AC", "0", ".", "=", "1", "2", "3", "+", "4", "5", "6", "-", "7", "8", "9", "×", "√", "MC", "M+", "÷", "^", "MR", "M-", "⌫" };

            //initialise button instance
            Button btnOperators = new Button();

            int btnX = 2;
            int btnY = 450;
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
                    btnX = 2; //reset and align grid upwards
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
            int[] digitIndexes = { 1, 2, 4, 5, 6, 8, 9, 10, 12, 13, 14};
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

            //Other buttons

            //Clear button & Backspace, respectively
            buttonList[0].BackColor = Color.CornflowerBlue;
            buttonList[23].BackColor = Color.CornflowerBlue;

            //Equals Button
            buttonList[3].BackColor = Color.Yellow;

        }

        #region Button Events
        //Click Events for each operator
        void BtnClear_Click(object sender, EventArgs e)
        {
            mainDisplay.Text = "";
        }
        void Btn0_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += "0";
        }
        void BtnDecimalPoint_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += ".";
        }
        void BtnEquals_Click(object sender, EventArgs e)
        {
            mainDisplay.Text = "=";
        }
        void Btn1_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += "1";
        }
        void Btn2_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += "2";
        }
        void Btn3_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += "3";
        }
        void BtnPlus_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += "+";
        }
        void Btn4_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += "4";
        }
        void Btn5_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += "5";
        }
        void Btn6_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += "6";
        }
        void BtnMinus_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += "-";
        }
        void Btn7_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += "7";
        }
        void Btn8_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += "8";
        }
        void Btn9_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += "9";
        }
        void BtnMultiply_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += "*";
        }
        void BtnSquareRoot_Click(object sender, EventArgs e)
        {
            mainDisplay.Text += "Square root";
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
            mainDisplay.Text += "/";
        }
        void BtnWedge_Click(object sender, EventArgs e)
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
            //could do the backspace function here
                    }
        #endregion
    }
}

//Make the GUI from code. No manually placing it for some fucking reason
//Make the Calculator or Calculating/Computing class and start working on operations
//Make the backspace method here.
//figure out how it is all going to work. I think it could split the whole expression up when you press equals
//checking for errors and then segregating the operands and their operators into an array. Sort it by bedmas
//rules and then call a method dealing with each one. 
//perhaps the labels for the main display can be inputted into by typing
//currently trying to get the border to draw around the display labels. It won't work for some reason...