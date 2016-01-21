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
    public partial class AddNewUserGUI : Form
    {
        public event EventHandler OnSaveNewUserNameButtonClicked = delegate { };
        private string newUserName;

        public AddNewUserGUI()
        {
            InitializeComponent();
        }


        //transfer user name to Launcher
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (this.userNameTextBox.Text != String.Empty)
            {
                this.newUserName = this.userNameTextBox.Text;
                
                this.Hide();
                OnSaveNewUserNameButtonClicked(this, EventArgs.Empty);
                
            }
            else if (this.userNameTextBox.Text == String.Empty)
            {
                MessageBox.Show("Please input your user name!");
            }
        }

        public String getNewUserName()
        {
            return newUserName;
          
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
  

 
    }
}