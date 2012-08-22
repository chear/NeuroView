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
    public partial class HeartAgeInputGUI : Form
    {
        public event EventHandler ConfirmButtonClicked = delegate { };
        public event EventHandler CancelButtonClicked = delegate { };

        private string fileName;
        private int age;
        
        public HeartAgeInputGUI()
        {
            InitializeComponent();
        }

        private void heartAgeInputConfirmButton_Click(object sender, EventArgs e)
        {
            if (this.heartAgeInputTextBox.Text != String.Empty && this.fileNameInputTextBox.Text != String.Empty)
            {
                this.age = Convert.ToInt32(this.heartAgeInputTextBox.Text);
                this.fileName = this.fileNameInputTextBox.Text;
                this.Hide();
                ConfirmButtonClicked(this, EventArgs.Empty);
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
        }

        public string getFilename()
        {
            return fileName;
        }

        public int getAge()
        {
            return age;
        }
    }
}
