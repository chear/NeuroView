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
using NeuroSky.ThinkGear.Parser;

namespace NeuroSky.NeuroView
{
    public class Form1 : System.Windows.Forms.Form
    {

        public GraphPanel attGraphPanel;
        public GraphPanel medGraphPanel;
        public GraphPanel rawGraphPanel;
        private TextBox portText;
        private Label fileLabel;
        private Label statusLabel;
        private Label poorSignalLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button openButton;

        private Connector tg_Connector;

        private System.ComponentModel.Container components = null;

        public int timeStampIndex = 0;

        public Form1()
        {
            tg_Connector = new Connector();
            tg_Connector.DeviceConnected += new EventHandler(OnDeviceConnected);
            tg_Connector.DeviceFound += new EventHandler(OnDeviceFound);
            tg_Connector.DeviceNotFound += new EventHandler(OnDeviceNotFound);
            tg_Connector.DeviceConnectFail += new EventHandler(OnDeviceNotFound);

            InitializeComponent();

            attGraphPanel.Label.Text = "Attention";
            attGraphPanel.LineGraph.samplingRate = 1;
            attGraphPanel.LineGraph.xAxisMax = 4;
            attGraphPanel.LineGraph.xAxisMin = 0;
            attGraphPanel.LineGraph.yAxisMax = 105;
            attGraphPanel.LineGraph.yAxisMin = -5;
            attGraphPanel.LineGraph.FileNameString = "Attention.csv";
            attGraphPanel.LineGraph.FileHeaderString = "TimeStamp, Attention";

            medGraphPanel.Label.Text = "Meditation";
            medGraphPanel.LineGraph.samplingRate = 1;
            medGraphPanel.LineGraph.xAxisMax = 4;
            medGraphPanel.LineGraph.xAxisMin = 0;
            medGraphPanel.LineGraph.yAxisMax = 105;
            medGraphPanel.LineGraph.yAxisMin = -5;
            medGraphPanel.LineGraph.FileNameString = "Meditation.csv";
            medGraphPanel.LineGraph.FileHeaderString = "TimeStamp, Meditation";


            rawGraphPanel.Label.Text = "EEG (Time Domain)";
            rawGraphPanel.LineGraph.samplingRate = 512;
            rawGraphPanel.LineGraph.xAxisMax = 4;
            rawGraphPanel.LineGraph.xAxisMin = 0;
            rawGraphPanel.LineGraph.yAxisMax = 2047;
            rawGraphPanel.LineGraph.yAxisMin = -2048;
            rawGraphPanel.LineGraph.FileNameString = "EEG (Time Domain).csv";
            rawGraphPanel.LineGraph.FileHeaderString = "TimeStamp, Raw";

#if false

            rawGraphPanel.LineGraph.samplingRate = 10;
            rawGraphPanel.LineGraph.xAxisMax = 2;
            rawGraphPanel.LineGraph.xAxisMin = 0;
            rawGraphPanel.LineGraph.yAxisMax = 1;
            rawGraphPanel.LineGraph.yAxisMin = -1;

#endif
            disconnectButton.Visible = false;
            disconnectButton.Enabled = false;

            stopButton.Visible = false;
            stopButton.Enabled = false;

            fileLabel.TextAlign = ContentAlignment.MiddleCenter;
            fileLabel.Visible = false;

        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.openButton = new System.Windows.Forms.Button();
            this.portText = new TextBox();
            this.statusLabel = new Label();
            this.poorSignalLabel = new Label();
            this.fileLabel = new Label();
            this.attGraphPanel = new NeuroSky.NeuroView.GraphPanel();
            this.medGraphPanel = new NeuroSky.NeuroView.GraphPanel();
            this.rawGraphPanel = new NeuroSky.NeuroView.GraphPanel();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(100, 15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(80, 24);
            this.button1.TabIndex = 1;
            this.button1.Text = "Connect";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(700, 510);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(80, 24);
            this.button2.TabIndex = 1;
            this.button2.Text = "Clear";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(360, 15);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(80, 24);
            this.button3.TabIndex = 1;
            this.button3.Text = "Record";
            this.button3.Click += new System.EventHandler(this.button3_Click);
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
            this.stopButton.Location = new System.Drawing.Point(360, 15);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(80, 24);
            this.stopButton.TabIndex = 1;
            this.stopButton.Text = "Stop";
            this.stopButton.Click += new System.EventHandler(this.stop_Click);
            // 
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(15, 510);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(80, 24);
            this.openButton.TabIndex = 1;
            this.openButton.Text = "Open";
            this.openButton.Click += new System.EventHandler(this.open_Click);
            // 
            // portText
            // 
            this.portText.Location = new System.Drawing.Point(10, 17);
            this.portText.Name = "portText";
            this.portText.Size = new System.Drawing.Size(80, 24);
            this.portText.Text = "Auto";
            // 
            // fileLabel
            // 
            this.fileLabel.Location = new System.Drawing.Point(0, 15);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(800, 24);
            this.fileLabel.Text = "None";
            // 
            // statusLabel
            // 
            this.statusLabel.Location = new System.Drawing.Point(110, 515);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(400, 24);
            this.statusLabel.Text = "Type COM port to connect (Ex. COM1) and Press Connect";
            // 
            // poorSignalLabel
            // 
            this.poorSignalLabel.Location = new System.Drawing.Point(740, 23);
            this.poorSignalLabel.Name = "poorSignalLabel";
            this.poorSignalLabel.Size = new System.Drawing.Size(50, 24);
            this.poorSignalLabel.Text = "PQ:";
            // 
            // attGraphPanel
            // 
            this.attGraphPanel.Location = new System.Drawing.Point(0, 50);
            this.attGraphPanel.Name = "attGraphPanel";
            this.attGraphPanel.Size = new System.Drawing.Size(800, 150);
            this.attGraphPanel.TabIndex = 0;
            // 
            // medGraphPanel
            // 
            this.medGraphPanel.Location = new System.Drawing.Point(0, 199);
            this.medGraphPanel.Name = "medGraphPanel";
            this.medGraphPanel.Size = new System.Drawing.Size(800, 150);
            this.medGraphPanel.TabIndex = 0;
            // 
            // rawGraphPanel
            // 
            this.rawGraphPanel.Location = new System.Drawing.Point(0, 348);
            this.rawGraphPanel.Name = "rawGraphPanel";
            this.rawGraphPanel.Size = new System.Drawing.Size(800, 150);
            this.rawGraphPanel.TabIndex = 0;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(800, 550);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.openButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.disconnectButton);
            this.Controls.Add(this.portText);
            this.Controls.Add(this.fileLabel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.poorSignalLabel);
            this.Controls.Add(this.attGraphPanel);
            this.Controls.Add(this.medGraphPanel);
            this.Controls.Add(this.rawGraphPanel);
            this.Name = "Form1";
            this.Text = "NeuroView Awesome";
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new Form1());
        }

        public int i = 0;

        /*Connect Button Clicked*/
        private void button1_Click(object sender, System.EventArgs e)
        {
#if true
            this.button1.Enabled = false;

            rawGraphPanel.LineGraph.Clear();
            medGraphPanel.LineGraph.Clear();
            attGraphPanel.LineGraph.Clear();

            string portName = portText.Text.ToUpper();
            portName = portName.Trim();

            if (portName != "AUTO")
            {
                Regex r = new Regex("COM[1-9][0-9]*");
                portName = r.Match(portName).ToString();
                tg_Connector.Connect(portName);
            }
            else
            {
                tg_Connector.Find();
            }

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

        private void disconnect_Click(object sender, System.EventArgs e)
        {
            tg_Connector.Disconnect();
            updateConnectButton(false);
        }

        private void open_Click(object sender, System.EventArgs e)
        {
            FolderBrowserDialog fdlg = new FolderBrowserDialog();
            fdlg.Description = "Choose the folder that includes the csv files:";
            fdlg.ShowNewFolderButton = false;

            if(fdlg.ShowDialog() == DialogResult.OK)
            {
                string attPath = System.IO.Path.Combine(fdlg.SelectedPath, attGraphPanel.LineGraph.FileNameString);
                string medPath = System.IO.Path.Combine(fdlg.SelectedPath, medGraphPanel.LineGraph.FileNameString);
                if (System.IO.File.Exists(attPath) && System.IO.File.Exists(medPath))
                {

                    Form1 newForm = new Form1();

                    OpenAddData(newForm, fdlg.SelectedPath);

                    newForm.fileLabel.Text = fdlg.SelectedPath;
                    newForm.fileLabel.Visible = true;
                    newForm.button1.Visible = false;
                    newForm.portText.Visible = false;
                    newForm.button3.Visible = false;
                    newForm.Show();
                }
                else
                {
                    Console.WriteLine("Invalid Path");
                }
            }
   
        }

        /*Opens and adds data to the form*/
        private void OpenAddData(Form1 tempForm, string dataPath)
        {
            string attPath = System.IO.Path.Combine(dataPath, attGraphPanel.LineGraph.FileNameString);
            string medPath = System.IO.Path.Combine(dataPath, medGraphPanel.LineGraph.FileNameString);
            string rawPath = System.IO.Path.Combine(dataPath, rawGraphPanel.LineGraph.FileNameString);

            tempForm.attGraphPanel.LineGraph.RecordDataFlag = true;
            tempForm.medGraphPanel.LineGraph.RecordDataFlag = true;
            tempForm.rawGraphPanel.LineGraph.RecordDataFlag = true;

            if (File.Exists(attPath))
            {
                DataPair[] tempDataPairArray = ParseCSVFile(attPath);

                foreach (DataPair d in tempDataPairArray)
                {
                    tempForm.attGraphPanel.LineGraph.Add(d);
                }
            }

            if (File.Exists(medPath))
            {
                DataPair[] tempDataPairArray = ParseCSVFile(medPath);

                foreach (DataPair d in tempDataPairArray)
                {
                    tempForm.medGraphPanel.LineGraph.Add(d);
                }
            }

            if (File.Exists(rawPath))
            {
                DataPair[] tempDataPairArray = ParseCSVFile(rawPath);

                foreach (DataPair d in tempDataPairArray)
                {
                    tempForm.rawGraphPanel.LineGraph.Add(d);
                }
            }

            tempForm.rawGraphPanel.Invalidate();
            tempForm.medGraphPanel.Invalidate();
            tempForm.attGraphPanel.Invalidate();

        }

        private DataPair[] ParseCSVFile(string filePath)
        {
            List<DataPair> tempDataPairList = new List<DataPair>();

            using (StreamReader sr = new StreamReader(filePath))
            {
                /*Initialize the string and reads the first header line.*/
                string line = sr.ReadLine();

                while ((line = sr.ReadLine()) != null)
                {
                    string[] tempString = line.Split(new char[] { ',' });

                    tempDataPairList.Add(new DataPair(Convert.ToDouble(tempString[0]), Convert.ToDouble(tempString[1])));
                }

                return tempDataPairList.ToArray();
            }

        }

        /*Clear Button Clicked*/
        private void button2_Click(object sender, System.EventArgs e)
        {
            rawGraphPanel.LineGraph.Clear();
            medGraphPanel.LineGraph.Clear();
            attGraphPanel.LineGraph.Clear();

            i = 0;
            timeStampIndex = 0;

            rawGraphPanel.LineGraph.Invalidate();
            medGraphPanel.LineGraph.Invalidate();
            attGraphPanel.LineGraph.Invalidate();
        }

        /*Record Button Clicked*/
        private void button3_Click(object sender, System.EventArgs e)
        {
            button3.Enabled = false;
            button3.Visible = false;
            
            /*Clear Block*/
            rawGraphPanel.LineGraph.Clear();
            medGraphPanel.LineGraph.Clear();
            attGraphPanel.LineGraph.Clear();
            i = 0;
            timeStampIndex = 0;

            /*Turn on recording*/
            rawGraphPanel.LineGraph.RecordDataFlag = true;
            medGraphPanel.LineGraph.RecordDataFlag = true;
            attGraphPanel.LineGraph.RecordDataFlag = true;

            /*Specify the folder to be saved*/
            string tempFolderName = DateTime.Now.ToString();
            tempFolderName = tempFolderName.Replace(':', ' ');
            tempFolderName = tempFolderName.Replace('/', '.');
            Console.WriteLine(tempFolderName);


            rawGraphPanel.LineGraph.FolderNameString = tempFolderName;
            medGraphPanel.LineGraph.FolderNameString = tempFolderName;
            attGraphPanel.LineGraph.FolderNameString = tempFolderName;

            stopButton.Enabled = true;
            stopButton.Visible = true;

        }

        private void stop_Click(object sender, System.EventArgs e)
        {
            stopButton.Enabled = false;
            stopButton.Visible = false;

            rawGraphPanel.LineGraph.RecordDataFlag = false;
            medGraphPanel.LineGraph.RecordDataFlag = false;
            attGraphPanel.LineGraph.RecordDataFlag = false;

            rawGraphPanel.LineGraph.SaveDataFlag = true;
            medGraphPanel.LineGraph.SaveDataFlag = true;
            attGraphPanel.LineGraph.SaveDataFlag = true;

            rawGraphPanel.LineGraph.Invalidate();
            medGraphPanel.LineGraph.Invalidate();
            attGraphPanel.LineGraph.Invalidate();

            button3.Enabled = true;
            button3.Visible = true;
        }

        void OnDeviceNotFound(object sender, EventArgs e)
        {
            updateConnectButton(false);
            updateStatusLabel("Unable to connect.");
        }

        void OnDeviceFound(object sender, EventArgs e)
        {
            string tempPortName = tg_Connector.mindSetPorts[0].PortName;
            updateStatusLabel("Device found and Connecting to " + tempPortName + ".");
            tg_Connector.Connect(tempPortName);
            
        }

        void OnDeviceConnected(object sender, EventArgs e)
        {
            Connector.DeviceEventArgs de = (Connector.DeviceEventArgs)e;

            updateStatusLabel("Connected to a headset on " + de.Device.PortName + ".");

            de.Device.DataReceived += new EventHandler(OnDataReceived);
            updateConnectButton(true);

        }

        delegate void updateConnectButtonDelegate(bool connected);

        private void updateConnectButton(bool connected)
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
                    this.button1.Enabled = false;
                    this.button1.Visible = false;

                    this.disconnectButton.Enabled = true;
                    this.disconnectButton.Visible = true;
                }
                else
                {
                    this.disconnectButton.Enabled = false;
                    this.disconnectButton.Visible = false;

                    this.button1.Enabled = true;
                    this.button1.Visible = true;
                }

            }

        }

        delegate void updatePQLabelDelegate(string tempText);

        private void updatePQLabel(string tempText)
        {
            if (this.InvokeRequired)
            {
                updatePQLabelDelegate del = new updatePQLabelDelegate(updatePQLabel);
                this.Invoke(del, new object[] { tempText });
            }
            else
            {
                this.poorSignalLabel.Text = tempText;
            }
        }

        delegate void updateStatusLabelDelegate(string tempText);

        private void updateStatusLabel(string tempText)
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


        void OnDataReceived(object sender, EventArgs e)
        {
            Device d = (Device)sender;
            Device.DataEventArgs de = (Device.DataEventArgs)e;

            DataRow[] tempDataRowArray = de.DataRowArray;
            Parsed parsedData = new Parsed();

            MindSetParser mindSetParser = new MindSetParser();

            parsedData = mindSetParser.Read(de.DataRowArray);
#if true
            foreach (TimeStampData tsd in parsedData.Raw)
            {
                rawGraphPanel.LineGraph.Add(new DataPair((timeStampIndex / (double)rawGraphPanel.LineGraph.samplingRate), tsd.Value));
                timeStampIndex++;
                i++;
            }
#endif

#if true
            foreach (TimeStampData tsd in parsedData.Attention)
            {
                attGraphPanel.LineGraph.Add(new DataPair((timeStampIndex / (double)rawGraphPanel.LineGraph.samplingRate), tsd.Value));
            }

            foreach (TimeStampData tsd in parsedData.Meditation)
            {
                medGraphPanel.LineGraph.Add(new DataPair((timeStampIndex / (double)rawGraphPanel.LineGraph.samplingRate), tsd.Value));
            }

            foreach (TimeStampData tsd in parsedData.PoorSignalQuality)
            {
                updatePQLabel("PQ: " + tsd.Value);
            }
#endif
            if (i > 20)
            {
                i = 0;
                rawGraphPanel.LineGraph.Invalidate();
                attGraphPanel.LineGraph.Invalidate();
                medGraphPanel.LineGraph.Invalidate();
            }

#if false
            foreach(PowerEEGData ped in parsedData.PowerEEGData)
            {
                Console.WriteLine("Time: " + ped.TimeStamp +
                                  "\nDelta: " + ped.Delta +
                                  "\nTheta: " + ped.Theta +
                                  "\nAlpha1: " + ped.Alpha1 +
                                  "\nAlpha2: " + ped.Alpha2 +
                                  "\nBeta1: " + ped.Beta1 +
                                  "\nBeta2: " + ped.Beta2 +
                                  "\nGamma1: " + ped.Gamma1 +
                                  "\nGamma2: " + ped.Gamma2);
            }
#endif
        }

    }
}
