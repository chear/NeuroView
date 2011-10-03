using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.Collections.Generic;

using System.IO;

using System.Text.RegularExpressions;

using System.Threading;

using NeuroSky.ThinkGear;

using System.Xml;

namespace NeuroSky.MindView
{
    public class MainForm : System.Windows.Forms.Form
    {
        public GraphPanel rawGraphPanel;
        public TextBox portText;
        private Label statusLabel;
        public Label heartRateLabel;
        private Label fileLabel;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button recordButton;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.Button stopButton;

        private System.ComponentModel.Container components = null;

        public int timeStampIndex = 0;
        public bool recordFlag;

        public DateTime RecordStartTime;
        private Label YMaxLabel;
        private Label YMinLabel;
        private Label XMaxLabel;
        private Label XMinLabel;
        private TextBox YMaxTextBox;
        private TextBox YMinTextBox;
        private TextBox XMaxTextBox;
        private TextBox XMinTextBox;
        public DateTime RecordStopTime;

        public double poorQuality;

        private string dataLogOutFile;
        private System.IO.StreamWriter dataLogStream;

        public event EventHandler ConnectButtonClicked = delegate { };
        public event EventHandler DisconnectButtonClicked = delegate {};



        public MainForm()
        {

            InitializeComponent();

            recordFlag = false;

            rawGraphPanel.LineGraph.samplingRate = 512;
            rawGraphPanel.LineGraph.xAxisMax = 4;
            rawGraphPanel.LineGraph.xAxisMin = 0;
            rawGraphPanel.LineGraph.yAxisMax = 32768;
            rawGraphPanel.LineGraph.yAxisMin = -32767;
            rawGraphPanel.LineGraph.OptimizeScrollBar();
            rawGraphPanel.EnableValueDisplay();
            rawGraphPanel.DataSavingFinished += new EventHandler(OnDataSavingFinished);

            YMaxTextBox.Text = rawGraphPanel.LineGraph.yAxisMax.ToString();
            YMinTextBox.Text = rawGraphPanel.LineGraph.yAxisMin.ToString();
            XMaxTextBox.Text = rawGraphPanel.LineGraph.xAxisMax.ToString();
            XMinTextBox.Text = rawGraphPanel.LineGraph.xAxisMin.ToString();

            disconnectButton.Visible = false;
            disconnectButton.Enabled = false;

            stopButton.Visible = false;
            stopButton.Enabled = false;

            this.MinimumSize = new Size(945,  381);
            this.MaximumSize = new Size(3000,  381);

        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            Console.WriteLine("Disposing MainForm.");
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.connectButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.recordButton = new System.Windows.Forms.Button();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.portText = new System.Windows.Forms.TextBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.heartRateLabel = new System.Windows.Forms.Label();
            this.fileLabel = new System.Windows.Forms.Label();
            this.YMaxLabel = new System.Windows.Forms.Label();
            this.YMinLabel = new System.Windows.Forms.Label();
            this.XMaxLabel = new System.Windows.Forms.Label();
            this.XMinLabel = new System.Windows.Forms.Label();
            this.YMaxTextBox = new System.Windows.Forms.TextBox();
            this.YMinTextBox = new System.Windows.Forms.TextBox();
            this.XMaxTextBox = new System.Windows.Forms.TextBox();
            this.XMinTextBox = new System.Windows.Forms.TextBox();
            this.rawGraphPanel = new NeuroSky.MindView.GraphPanel();
            this.SuspendLayout();
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(100, 15);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(80, 24);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Connect";
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(830, 299);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(80, 24);
            this.clearButton.TabIndex = 1;
            this.clearButton.Text = "Clear";
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // recordButton
            // 
            this.recordButton.Location = new System.Drawing.Point(700, 299);
            this.recordButton.Name = "recordButton";
            this.recordButton.Size = new System.Drawing.Size(80, 24);
            this.recordButton.TabIndex = 1;
            this.recordButton.Text = "Record";
            this.recordButton.Click += new System.EventHandler(this.recordButton_Click);
            // 
            // disconnectButton
            // 
            this.disconnectButton.Location = new System.Drawing.Point(100, 15);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(80, 24);
            this.disconnectButton.TabIndex = 1;
            this.disconnectButton.Text = "Disconnect";
            this.disconnectButton.Click += new System.EventHandler(this.disconnect_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(700, 299);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(80, 24);
            this.stopButton.TabIndex = 1;
            this.stopButton.Text = "Stop";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // portText
            // 
            this.portText.Location = new System.Drawing.Point(10, 17);
            this.portText.Name = "portText";
            this.portText.Size = new System.Drawing.Size(80, 20);
            this.portText.TabIndex = 2;
            this.portText.Text = "Auto";
            // 
            // statusLabel
            // 
            this.statusLabel.Location = new System.Drawing.Point(12, 314);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(400, 24);
            this.statusLabel.TabIndex = 4;
            this.statusLabel.Text = "Type COM port to connect (Ex. COM1) and Press Connect";
            // 
            // heartRateLabel
            // 
            this.heartRateLabel.Location = new System.Drawing.Point(792, 30);
            this.heartRateLabel.Name = "heartRateLabel";
            this.heartRateLabel.Size = new System.Drawing.Size(133, 24);
            this.heartRateLabel.TabIndex = 5;
            this.heartRateLabel.Text = "Heart Rate:";
            // 
            // fileLabel
            // 
            this.fileLabel.Location = new System.Drawing.Point(0, 15);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(800, 24);
            this.fileLabel.TabIndex = 3;
            this.fileLabel.Text = "None";
            // 
            // YMaxLabel
            // 
            this.YMaxLabel.AutoSize = true;
            this.YMaxLabel.Location = new System.Drawing.Point(7, 108);
            this.YMaxLabel.Name = "YMaxLabel";
            this.YMaxLabel.Size = new System.Drawing.Size(34, 13);
            this.YMaxLabel.TabIndex = 6;
            this.YMaxLabel.Text = "YMax";
            // 
            // YMinLabel
            // 
            this.YMinLabel.AutoSize = true;
            this.YMinLabel.Location = new System.Drawing.Point(7, 148);
            this.YMinLabel.Name = "YMinLabel";
            this.YMinLabel.Size = new System.Drawing.Size(31, 13);
            this.YMinLabel.TabIndex = 7;
            this.YMinLabel.Text = "YMin";
            // 
            // XMaxLabel
            // 
            this.XMaxLabel.AutoSize = true;
            this.XMaxLabel.Location = new System.Drawing.Point(7, 190);
            this.XMaxLabel.Name = "XMaxLabel";
            this.XMaxLabel.Size = new System.Drawing.Size(34, 13);
            this.XMaxLabel.TabIndex = 8;
            this.XMaxLabel.Text = "XMax";
            // 
            // XMinLabel
            // 
            this.XMinLabel.AutoSize = true;
            this.XMinLabel.Location = new System.Drawing.Point(7, 233);
            this.XMinLabel.Name = "XMinLabel";
            this.XMinLabel.Size = new System.Drawing.Size(31, 13);
            this.XMinLabel.TabIndex = 9;
            this.XMinLabel.Text = "XMin";
            // 
            // YMaxTextBox
            // 
            this.YMaxTextBox.Location = new System.Drawing.Point(47, 105);
            this.YMaxTextBox.Name = "YMaxTextBox";
            this.YMaxTextBox.Size = new System.Drawing.Size(43, 20);
            this.YMaxTextBox.TabIndex = 10;
            this.YMaxTextBox.Text = " ";
            this.YMaxTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.YMaxTextBox_KeyPress);
            // 
            // YMinTextBox
            // 
            this.YMinTextBox.Location = new System.Drawing.Point(47, 145);
            this.YMinTextBox.Name = "YMinTextBox";
            this.YMinTextBox.Size = new System.Drawing.Size(43, 20);
            this.YMinTextBox.TabIndex = 11;
            this.YMinTextBox.Text = " ";
            this.YMinTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.YMinTextBox_KeyPress);
            // 
            // XMaxTextBox
            // 
            this.XMaxTextBox.Location = new System.Drawing.Point(47, 187);
            this.XMaxTextBox.Name = "XMaxTextBox";
            this.XMaxTextBox.Size = new System.Drawing.Size(43, 20);
            this.XMaxTextBox.TabIndex = 12;
            this.XMaxTextBox.Text = " ";
            this.XMaxTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.XMaxTextBox_KeyPress);
            // 
            // XMinTextBox
            // 
            this.XMinTextBox.Location = new System.Drawing.Point(47, 230);
            this.XMinTextBox.Name = "XMinTextBox";
            this.XMinTextBox.Size = new System.Drawing.Size(43, 20);
            this.XMinTextBox.TabIndex = 13;
            this.XMinTextBox.Text = " ";
            this.XMinTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.XMinTextBox_KeyPress);
            // 
            // rawGraphPanel
            // 
            this.rawGraphPanel.Location = new System.Drawing.Point(0, 72);
            this.rawGraphPanel.Name = "rawGraphPanel";
            this.rawGraphPanel.Size = new System.Drawing.Size(938, 203);
            this.rawGraphPanel.TabIndex = 0;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(937, 347);
            this.Controls.Add(this.XMinTextBox);
            this.Controls.Add(this.XMaxTextBox);
            this.Controls.Add(this.YMinTextBox);
            this.Controls.Add(this.YMaxTextBox);
            this.Controls.Add(this.XMinLabel);
            this.Controls.Add(this.XMaxLabel);
            this.Controls.Add(this.YMinLabel);
            this.Controls.Add(this.YMaxLabel);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.recordButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.disconnectButton);
            this.Controls.Add(this.portText);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.heartRateLabel);
            this.Controls.Add(this.rawGraphPanel);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EGODemo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /*Connect Button Clicked*/
        private void connectButton_Click(object sender, System.EventArgs e)
        {
#if true
            ConnectButtonClicked(this, EventArgs.Empty);

            this.connectButton.Enabled = false;
            this.portText.Enabled = false;

            rawGraphPanel.LineGraph.Clear();
#else
            double tempData = 0;

            tempData = Math.Sin(2*Math.PI*1*i/rawGraphPanel.LineGraph.samplingRate);
            DataPair tempDataPair;
            tempDataPair.timeStamp = (float)i/rawGraphPanel.LineGraph.samplingRate;
            tempDataPair.data      = (float)tempData;


            rawGraphPanel.LineGraph.Add( tempDataPair );

            i++;

            rawGraphPanel.LineGraph.Invalidate();
#endif
        }

        //disconnect button clicked
        private void disconnect_Click(object sender, System.EventArgs e)
        {
            DisconnectButtonClicked(this, EventArgs.Empty);
        }


        /*Clear Button Clicked*/
        private void clearButton_Click(object sender, System.EventArgs e)
        {
            rawGraphPanel.LineGraph.Clear();

            timeStampIndex = 0;

            rawGraphPanel.LineGraph.Invalidate();
        }


        /*Record Button Clicked*/
        private void recordButton_Click(object sender, System.EventArgs e)
        {
            recordButton.Enabled = false;
            recordButton.Visible = false;

            //save linegraph data in a seperate file
            //rawGraphPanel.LineGraph.SaveDataFlag = false;
            //rawGraphPanel.LineGraph.RecordDataFlag = true;

            /*Clear Block*/
            rawGraphPanel.LineGraph.Clear();
            rawGraphPanel.LineGraph.timeStampIndex = 0;

            stopButton.Enabled = true;
            stopButton.Visible = true;

            // Create new file
            dataLogOutFile = "dataLog-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Hour.ToString() + "-"
            + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".txt";

            // Create new filestream, appendable
            this.dataLogStream = new System.IO.StreamWriter(dataLogOutFile, true);

            recordFlag = true;
        }

        //stop button clicked
        private void stopButton_Click(object sender, System.EventArgs e)
        {
            rawGraphPanel.LineGraph.SaveDataFlag = true;
            rawGraphPanel.LineGraph.RecordDataFlag = false;

            recordFlag = false;

            stopButton.Enabled = false;
            stopButton.Visible = false;

            recordButton.Enabled = true;
            recordButton.Visible = true;

            RecordStopTime = DateTime.Now;

            dataLogStream.Close();
        }

        /**
        * Record out the received data to a datalog file
        */
        public void recordData(ThinkGear.DataRow[] dataRowArray)
        {
            if (dataLogStream != null)
            {
                foreach (DataRow dr in dataRowArray)
                {
                    //write the timestamp
                    dataLogStream.Write(dr.Time.ToString("#0.000") + ": " + (dr.Type.GetHashCode().ToString("X").PadLeft(2, '0')) + " ");
                    
                    foreach (byte b in dr.Data)
                    {
                        dataLogStream.Write(b.ToString("X").PadLeft(2, '0') + " ");
                    }
                    dataLogStream.Write(dataLogStream.NewLine);
                }
            }
        }


        //when data saving is done
        void OnDataSavingFinished(object sender, EventArgs e)
        {
           //no need for this currently
        }


        //update the connect button status
        delegate void updateConnectButtonDelegate(bool connected);
        public void updateConnectButton(bool connected)
        {
            if (this.InvokeRequired)
            {
                updateConnectButtonDelegate del = new updateConnectButtonDelegate(updateConnectButton);
                this.Invoke(del, new object[] { connected });
            }
            else
            {
                if (connected)
                {
                    this.connectButton.Enabled = false;
                    this.connectButton.Visible = false;

                    this.portText.Enabled = false;

                    this.disconnectButton.Enabled = true;
                    this.disconnectButton.Visible = true;
                }
                else
                {
                    this.disconnectButton.Enabled = false;
                    this.disconnectButton.Visible = false;

                    this.portText.Enabled = true;

                    this.connectButton.Enabled = true;
                    this.connectButton.Visible = true;
                }

            }
        }

        //update the PQ Label status
        delegate void updateHeartRateLabelDelegate(string tempText);
        public void updateHeartRateLabel(string tempText)
        {
            if (this.InvokeRequired)
            {
                updateHeartRateLabelDelegate del = new updateHeartRateLabelDelegate(updateHeartRateLabel);
                this.Invoke(del, new object[] { tempText });
            }
            else
            {
                this.heartRateLabel.Text = tempText;
            }
        }


        delegate void updateStatusLabelDelegate(string tempText);
        public void updateStatusLabel(string tempText)
        {
            if (this.InvokeRequired)
            {
                updateStatusLabelDelegate del = new updateStatusLabelDelegate(updateStatusLabel);
                this.Invoke(del, new object[] { tempText });
            }
            else
            {
                this.statusLabel.Text = tempText;
            }
        }


        protected override void OnSizeChanged(EventArgs e)
        {

            /*Update dimension*/
            rawGraphPanel.Width = this.Width - 10;

            /*Update Locations*/

            base.OnSizeChanged(e);
        }


        private void YMaxTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            // If the key pressed was "Enter"
            if (e.KeyChar == (char)13)
            {
                //suppress the beep sounds by setting e.Handled = true
                e.Handled = true;
                //verify that the string entered in the box is actually a number
                try
                {
                    rawGraphPanel.LineGraph.yAxisMax = Int32.Parse(YMaxTextBox.Text);
                }
                catch (Exception exp)
                {
                    Console.WriteLine("error at YMax textbox: " + exp.Message);
                }
            }
        }


        private void YMinTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If the key pressed was "Enter"
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                //verify that the string entered in the box is actually a number
                try
                {
                    rawGraphPanel.LineGraph.yAxisMin = Int32.Parse(YMinTextBox.Text);
                }
                catch (Exception exp)
                {
                    Console.WriteLine("error at YMin textbox: " + exp.Message);
                }
            }
        }


        private void XMaxTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If the key pressed was "Enter"
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                //verify that the string entered in the box is actually a number
                try
                {
                    rawGraphPanel.LineGraph.xAxisMax = Int32.Parse(XMaxTextBox.Text);
                }
                catch (Exception exp)
                {
                    Console.WriteLine("error at XMax textbox: " + exp.Message);
                }
            }
        }


        private void XMinTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If the key pressed was "Enter"
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                //verify that the string entered in the box is actually a number
                try
                {
                    rawGraphPanel.LineGraph.xAxisMin = Int32.Parse(XMinTextBox.Text);
                }
                catch (Exception exp)
                {
                    Console.WriteLine("error at XMin textbox: " + exp.Message);
                }
            }
        }

    }
    /*End of MainForm*/
}
