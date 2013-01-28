using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NeuroSky.ThinkGear
{
    public partial class UserAgeInputGUI : Form
    {
        public event EventHandler ConfirmButtonClicked = delegate { };
        public event EventHandler CancelButtonClicked = delegate { };

        private string fileName;
        private int age;
        
        public UserAgeInputGUI()
        {
            InitializeComponent();
        }

        private void heartAgeInputConfirmButton_Click(object sender, EventArgs e)
        {
            //both of inputs are not empty
            if (this.heartAgeInputTextBox.Text != String.Empty && this.fileNameInputTextBox.Text != String.Empty)
            {               
                this.age = Convert.ToInt32(this.heartAgeInputTextBox.Text);
                this.fileName = this.fileNameInputTextBox.Text;
                if (this.age < 10 || this.age > 150)//suggestion: age should be between 10 to 150
                {
                    MessageBox.Show("Input age is invalid!");
                }
                else
                { 
                    this.Hide();
                    ConfirmButtonClicked(this, EventArgs.Empty);
                }
                
            }
            else if (this.heartAgeInputTextBox.Text == String.Empty || this.fileNameInputTextBox.Text == String.Empty)
            {
                MessageBox.Show("Please input your real age and file name!");
            }
        }

        private void heartAgeInputCancelButton_Click(object sender, EventArgs e)
        {
            this.heartAgeInputTextBox.Text = "";
            this.fileNameInputTextBox.Text = "";
            this.Hide();
        }

        public string getFilename()
        {
            return fileName;
        }

        public int getAge()
        {
            return age;
        }
        //check the age input is number or not
        private void heartAgeInputTextBox_TextChanged(object sender, EventArgs e)
        {
            string txt = heartAgeInputTextBox.Text;
            int i = txt.Length;
            if (i < 1)
                return;
            for (int m = 0; m < i; m++)
            {
                string str = txt.Substring(m, 1);
                if (!char.IsNumber(Convert.ToChar(str)))//not a valide number
                {
                    MessageBox.Show("Please input numbers!");//show message
                    heartAgeInputTextBox.Text = "";//clean textbox
                }
            }
        }

        

       
    }
}
