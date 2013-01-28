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
    public partial class RecognitionGUI : Form
    {
        public RecognitionGUI()
        {
            InitializeComponent();
        }
        //update the Identification result
        delegate void UpdateIdentificationResultDelegate(string tempString);
        public void updateIdentificationResultIndicator(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    try
                    {
                        UpdateIdentificationResultDelegate del = new UpdateIdentificationResultDelegate(updateIdentificationResultIndicator);
                        this.Invoke(del, new object[] { tempString });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("caught exception at UpdateIdentificationResultIndicator: " + e.Message);
                    }
                }
                else
                {
                    //show the result of identification
                    //Console.WriteLine("The result is: " + tempString);
                    this.identificationResultsIndicatorLabel.Text = tempString;
                    this.returnButton.Visible = true;
                }
            }
        }
        private void returnButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
       
    }
}
