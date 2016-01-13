using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NeuroSky.MindView
{
    public partial class RecognitionRecordingGUI : Form
    {
        //set a timer to count
        private int timer = -1;
        //trigger to initial
        private int trigger = 0;

        public double poorSignal;

        public RecognitionRecordingGUI()
        {
            InitializeComponent();
        }

        //update the status of training step
        delegate void UpdateTrainingStepDelegate(string tempString);
        public void updateTrainingStepIndicator(string tempString)
        {
            if ((!this.Disposing) && (!this.IsDisposed))
            {
                if (this.InvokeRequired)
                {
                    try
                    {
                        UpdateTrainingStepDelegate del = new UpdateTrainingStepDelegate(updateTrainingStepIndicator);
                        this.Invoke(del, new object[] { tempString});
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("caught exception at UpdateTrainingStepIndicator: " + e.Message);
                    }
                }
                else
                {
                    //show the step of training
                    //Console.WriteLine("Training step: " + tempString);


                    if (String.Equals(tempString,"MSG_EKG_TRAINED")) //finish training
                    {
                        this.trainingStepIndicatorLabel.Text = "Record Finished!";
                        //this.returnButton.Visible = true;
                        this.Hide();
                        MessageBox.Show("Record Finished!");
                    }
                    else if (String.Equals(tempString, "MSG_EKG_TRAIN_TOUCH"))//need touch sensors
                    {
                        this.trainingStepLabel.Text = " ";
                        this.trainingStepIndicatorLabel.Text = "Please Touch the Sensors!";
                    }
                    else if (tempString.Equals("1") || tempString.Equals("2") || tempString.Equals("3"))
                    {
                        //update the training step and tell user to release sensors
                        this.trainingStepIndicatorLabel.Text = "Training Step: " + tempString + " is finished";
                        this.trainingStepLabel.Text = "Please Release Sensors And Wait For Three Seconds Then Touch the Sensors!";
                        //set trigger
                        trigger = -1;//have finished one training step
                        //reset timer
                        timer = 0;
                        //updateMessage();
                    }
                    
                    
                }
            }
        }

        private void updateMessage()
        {
            while (trigger == -1)
            {
                if (timer == 0 || timer == 1)
                {

                    if (poorSignal == 0)//have released sensors
                    {
                        //tell user algo is initializing
                        this.trainingStepIndicatorLabel.Text = "Initializing...";
                        //when poorsignal is 0 for 2 seconds, tell user to touch sensors again
                        timer++;
                    }
                }
                else if (timer == 2)
                {
                    //touch sensors again
                    this.trainingStepIndicatorLabel.Text = "Please Touch the Sensors!";
                    trigger = 0;
                    timer = 0;
                    break;
                }
                
            }
        }

       
        protected override void OnSizeChanged(EventArgs e)
        {
            trainingStepIndicatorLabel.Location = new System.Drawing.Point((this.Width-trainingStepIndicatorLabel.Width)/2,(this.Height-trainingStepIndicatorLabel.Height)/2);
            trainingStepLabel.Location = new System.Drawing.Point((this.Width - trainingStepLabel.Width) / 2, (this.Height - trainingStepLabel.Height)/3);        
        }
    }
}
