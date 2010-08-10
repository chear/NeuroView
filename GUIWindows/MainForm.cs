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

using System.Xml;

namespace NeuroSky.MindView
{
    public class MainForm : System.Windows.Forms.Form
    {

        public GraphPanel attGraphPanel;
        public GraphPanel medGraphPanel;
        public GraphPanel rawGraphPanel;
        public TextBox portText;
        private Label statusLabel;
        private Label poorSignalLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Button analyzeButton;

        private System.ComponentModel.Container components = null;

        public int timeStampIndex = 0;

        public string dataRootFolder = @"C:\Data";
        public string tempFolderName;
        public DateTime RecordStartTime;
        public DateTime RecordStopTime;

        public event EventHandler ConnectButtonClicked = delegate { };
        public event EventHandler DisconnectButtonClicked = delegate {};

        public MainForm()
        {

            InitializeComponent();

            attGraphPanel.Label.Text = "Attention";
            attGraphPanel.LineGraph.samplingRate = 1;
            attGraphPanel.LineGraph.xAxisMax = 8;
            attGraphPanel.LineGraph.xAxisMin = 0;
            attGraphPanel.LineGraph.yAxisMax = 105;
            attGraphPanel.LineGraph.yAxisMin = -5;
            attGraphPanel.LineGraph.FileNameString = "Attention.csv";
            attGraphPanel.LineGraph.FileHeaderString = "TimeStamp, Attention";
            attGraphPanel.EnableValueDisplay();
            attGraphPanel.DataSavingFinished += new EventHandler(OnDataSavingFinished);

            medGraphPanel.Label.Text = "Meditation";
            medGraphPanel.LineGraph.samplingRate = 1;
            medGraphPanel.LineGraph.xAxisMax = 8;
            medGraphPanel.LineGraph.xAxisMin = 0;
            medGraphPanel.LineGraph.yAxisMax = 105;
            medGraphPanel.LineGraph.yAxisMin = -5;
            medGraphPanel.LineGraph.FileNameString = "Meditation.csv";
            medGraphPanel.LineGraph.FileHeaderString = "TimeStamp, Meditation";
            medGraphPanel.EnableValueDisplay();
            medGraphPanel.DataSavingFinished += new EventHandler(OnDataSavingFinished);


            rawGraphPanel.Label.Text = "EEG (Time Domain)";
            rawGraphPanel.LineGraph.samplingRate = 512;
            rawGraphPanel.LineGraph.xAxisMax = 4;
            rawGraphPanel.LineGraph.xAxisMin = 0;
            rawGraphPanel.LineGraph.yAxisMax = 2047;
            rawGraphPanel.LineGraph.yAxisMin = -2048;
            rawGraphPanel.LineGraph.FileNameString = "EEG (Time Domain).csv";
            rawGraphPanel.LineGraph.FileHeaderString = "TimeStamp, Raw";
            rawGraphPanel.LineGraph.OptimizeScrollBar();
            rawGraphPanel.EnableValueDisplay();
            rawGraphPanel.DataSavingFinished += new EventHandler(OnDataSavingFinished);

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

            this.MinimumSize = new Size(800,  580);
            this.MaximumSize = new Size(3000,  580);

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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.openButton = new System.Windows.Forms.Button();
            this.analyzeButton = new System.Windows.Forms.Button();
            this.portText = new System.Windows.Forms.TextBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.poorSignalLabel = new System.Windows.Forms.Label();
            this.attGraphPanel = new NeuroSky.MindView.GraphPanel();
            this.medGraphPanel = new NeuroSky.MindView.GraphPanel();
            this.rawGraphPanel = new NeuroSky.MindView.GraphPanel();
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
            this.openButton.Text = "Open ...";
            this.openButton.Click += new System.EventHandler(this.open_Click);
            // 
            // analyzeButton
            // 
            this.analyzeButton.Location = new System.Drawing.Point(600, 510);
            this.analyzeButton.Name = "analyzeButton";
            this.analyzeButton.Size = new System.Drawing.Size(80, 24);
            this.analyzeButton.TabIndex = 1;
            this.analyzeButton.Text = "Analyze Data";
            this.analyzeButton.Click += new System.EventHandler(this.analyze_Click);
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
            this.statusLabel.Location = new System.Drawing.Point(110, 515);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(400, 24);
            this.statusLabel.TabIndex = 4;
            this.statusLabel.Text = "Type COM port to connect (Ex. COM1) and Press Connect";
            // 
            // poorSignalLabel
            // 
            this.poorSignalLabel.Location = new System.Drawing.Point(740, 23);
            this.poorSignalLabel.Name = "poorSignalLabel";
            this.poorSignalLabel.Size = new System.Drawing.Size(50, 24);
            this.poorSignalLabel.TabIndex = 5;
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
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(800, 550);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.openButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.disconnectButton);
            this.Controls.Add(this.analyzeButton);
            this.Controls.Add(this.portText);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.poorSignalLabel);
            this.Controls.Add(this.attGraphPanel);
            this.Controls.Add(this.medGraphPanel);
            this.Controls.Add(this.rawGraphPanel);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MindView Lite";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /*Connect Button Clicked*/
        private void button1_Click(object sender, System.EventArgs e)
        {
#if true
            ConnectButtonClicked(this, EventArgs.Empty);

            this.button1.Enabled = false;
            this.portText.Enabled = false;

            rawGraphPanel.LineGraph.Clear();
            medGraphPanel.LineGraph.Clear();
            attGraphPanel.LineGraph.Clear();
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
            DisconnectButtonClicked(this, EventArgs.Empty);
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

                    MainForm newForm = new MainForm();

                    OpenAddData(newForm, fdlg.SelectedPath);

                    newForm.Text = fdlg.SelectedPath;
                    newForm.button1.Visible = false;
                    newForm.portText.Visible = false;
                    newForm.button3.Visible = false;
                    newForm.openButton.Visible = false;
                    newForm.statusLabel.Visible = false;
                    newForm.attGraphPanel.ValueLabel.Visible = false;
                    newForm.medGraphPanel.ValueLabel.Visible = false;
                    newForm.rawGraphPanel.ValueLabel.Visible = false;
                    newForm.Show();
                }
                else
                {
                    Console.WriteLine("Invalid Path");
                }
            }
   
        }

        private void analyze_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fdlg = new FolderBrowserDialog();
            fdlg.Description = "Choose the folder that includes the csv files:";
            fdlg.ShowNewFolderButton = false;

            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                string attPath = System.IO.Path.Combine(fdlg.SelectedPath, attGraphPanel.LineGraph.FileNameString);
                string medPath = System.IO.Path.Combine(fdlg.SelectedPath, medGraphPanel.LineGraph.FileNameString);
                if (System.IO.File.Exists(attPath) && System.IO.File.Exists(medPath))
                {

                    ReportForm newForm = new ReportForm();

                    OpenAddData(newForm, fdlg.SelectedPath);
                    AddDataInfo(newForm, fdlg.SelectedPath);

                    newForm.Text = "Brainwave Report for " + fdlg.SelectedPath;
                    
                    newForm.attGraphPanel.LineGraph.FitAllData();
                    newForm.medGraphPanel.LineGraph.FitAllData();

                    newForm.Update();

                    newForm.Show();
                }
                else
                {
                    Console.WriteLine("Invalid Path");
                }
            }
        }

        private void AddDataInfo(ReportForm reportForm, string dataPath)
        {
            string txtPath = System.IO.Path.Combine(dataPath, "Info.xml");

            if (File.Exists(txtPath))
            {
                XmlTextReader reader = new XmlTextReader(txtPath);
                string tempStartTime = "None";
                string tempStopTime = "None";

                while(reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "StartTime":
                                reader.Read();
                                tempStartTime = reader.Value;
                                break;
                            case "StopTime":
                                reader.Read();
                                tempStopTime = reader.Value;
                                break;
                        }
                    }
                }


                reportForm.infoLabel.Text = "Start Time: " + tempStartTime + "     Stop Time: " + tempStopTime;

                Console.WriteLine("StartTime: " + tempStartTime + " StopTime: " + tempStopTime);
                
            }

        }

        /*Opens and adds data to the form*/
        private void OpenAddData(MainForm tempForm, string dataPath)
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

        /*Opens and adds data to the form*/
        private void OpenAddData(ReportForm tempForm, string dataPath)
        {
            string attPath = System.IO.Path.Combine(dataPath, attGraphPanel.LineGraph.FileNameString);
            string medPath = System.IO.Path.Combine(dataPath, medGraphPanel.LineGraph.FileNameString);

            tempForm.attGraphPanel.LineGraph.RecordDataFlag = true;
            tempForm.medGraphPanel.LineGraph.RecordDataFlag = true;

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
            timeStampIndex = 0;

            /*Turn on recording*/
            rawGraphPanel.LineGraph.RecordDataFlag = true;
            medGraphPanel.LineGraph.RecordDataFlag = true;
            attGraphPanel.LineGraph.RecordDataFlag = true;

            /*Specify the folder to be saved*/
            RecordStartTime = DateTime.Now;
            tempFolderName = RecordStartTime.ToString().Replace(':', ' ');
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
            rawGraphPanel.LineGraph.SaveDataFlag = true;
            medGraphPanel.LineGraph.SaveDataFlag = true;
            attGraphPanel.LineGraph.SaveDataFlag = true;

            rawGraphPanel.LineGraph.RecordDataFlag = false;
            medGraphPanel.LineGraph.RecordDataFlag = false;
            attGraphPanel.LineGraph.RecordDataFlag = false;

            stopButton.Enabled = false;
            stopButton.Visible = false;

            button3.Enabled = true;
            button3.Visible = true;

            RecordStopTime = DateTime.Now;
        }

        int numberFilesSaved = 0;

        void OnDataSavingFinished(object sender, EventArgs e)
        {
            GraphPanel tempGraphPanel = (GraphPanel)sender;

            numberFilesSaved++;

            if (numberFilesSaved == 3)
            {
                string tempPath = System.IO.Path.Combine(dataRootFolder, tempFolderName);

                MessageBox.Show("Data has been saved to the following directory:\n" + tempPath, "Data Saved",
                                 MessageBoxButtons.OK, MessageBoxIcon.Information);

                numberFilesSaved = 0;

                CreateInfoFile(tempPath);
            }
        }

        void CreateInfoFile( string dataFolderPath )
        {
            string txtPath = System.IO.Path.Combine(dataFolderPath, "Info.xml");

            if (!System.IO.File.Exists(txtPath))
            {
                XmlTextWriter xmlWriter = new XmlTextWriter(txtPath, System.Text.Encoding.UTF8);
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("DataInfo");
                xmlWriter.WriteStartElement("StartTime");
                xmlWriter.WriteString(RecordStartTime.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("StopTime");
                xmlWriter.WriteString(RecordStopTime.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();
                xmlWriter.Close();
                
            }
        }

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
                    this.button1.Enabled = false;
                    this.button1.Visible = false;

                    this.portText.Enabled = false;

                    this.disconnectButton.Enabled = true;
                    this.disconnectButton.Visible = true;
                }
                else
                {
                    this.disconnectButton.Enabled = false;
                    this.disconnectButton.Visible = false;

                    this.portText.Enabled = true;

                    this.button1.Enabled = true;
                    this.button1.Visible = true;
                }

            }

        }

        delegate void updatePQLabelDelegate(string tempText);

        public void updatePQLabel(string tempText)
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
            medGraphPanel.Width = this.Width - 10;
            attGraphPanel.Width = this.Width - 10;

            /*Update Locations*/


            base.OnSizeChanged(e);
        }


    }/*End of MainForm*/
}
