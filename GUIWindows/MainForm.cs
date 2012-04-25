using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Media;
using System.Threading;

using NeuroSky.ThinkGear;
using NeuroSky.ThinkGear.Algorithms;

namespace NeuroSky.MindView {
    public class MainForm : System.Windows.Forms.Form {
        private SaveFileGUI saveFileGUI;        //for saving the EEG data
        private SaveFileDialog saveHRMdialog;   //for saving the HRM file    

        private EnergyLevel energyLevel;
        private int fatigueCounter = 0;         //counter to keep track of how many RR values we have so far in the Fatigue Meter
        public int[] RRbuffer = new int[110];  //holds the 110 RR interval values before they are dumped into the algorithm
        public bool runFatigueMeter = false;   //fatigue meter is off by default
        private int fatigueResult;              //output of the EnergyLevel algorithm
        
        public GraphPanel rawGraphPanel;
        public TextBox portText;
        private Label statusLabel;
        public Label realtimeHeartRateLabel;
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
        public DateTime RecordStopTime;

        public Label averageHeartRateLabel;
        private Label realtimeHeartRateLabelIndicator;
        private Label averageHeartRateLabelIndicator;

        public double poorQuality;
        private int ADCValue;

        public double ASICHBValue;     //actual value coming out of the chip

        public double realTimeHBValue;
        private int realTimeHBBufferLength;
        private int realTimeHBCounter;
        private double[] realTimeHBValueBuffer;

        public double avgHBValue;
        private int avgHBBufferLength;
        private int avgHBCounter;
        private double[] avgHBValueBuffer;

        private string dataLogOutFile;
        private System.IO.StreamWriter dataLogStream;
        private string ECGLogOutFile;
        private System.IO.StreamWriter ECGLogStream;
        private string HRMoutFile;
        private System.IO.StreamWriter HRMstream;

        private string currentPath;
        
        public event EventHandler ConnectButtonClicked = delegate { };
        public event EventHandler DisconnectButtonClicked = delegate { };

        public CheckBox soundCheckBox;
        private Button fatigueButton;
        public SoundPlayer player;

        public MainForm() {

            saveFileGUI = new SaveFileGUI();
            saveFileGUI.SaveButtonClicked += new EventHandler(OnSaveButtonClicked);
            saveFileGUI.DiscardButtonClicked += new EventHandler(OnDiscardButtonClicked);
            saveFileGUI.BrowseButtonClicked += new EventHandler(OnBrowseButtonClicked);
            saveFileGUI.StartPosition = FormStartPosition.Manual;

            saveHRMdialog = new SaveFileDialog();
            saveHRMdialog.Filter = "HRM file|*.hrm";
            saveHRMdialog.FileOk += new CancelEventHandler(saveHRMdialog_FileOk);

            InitializeComponent();

            recordFlag = false;

            rawGraphPanel.samplingRate = 512;
            rawGraphPanel.xAxisMax = 4;
            rawGraphPanel.xAxisMin = 0;
            rawGraphPanel.yAxisMax = 2;
            rawGraphPanel.yAxisMin = -2;
            rawGraphPanel.Text = "ECG";
            rawGraphPanel.EnableValueDisplay();
            rawGraphPanel.OptimizeScrollBar();
            rawGraphPanel.DataSavingFinished += new EventHandler(OnDataSavingFinished);
            rawGraphPanel.LineGraph.DCRemovalEnabled = false;

            disconnectButton.Visible = false;
            disconnectButton.Enabled = false;

            stopButton.Visible = false;
            stopButton.Enabled = false;

            ASICHBValue = 0;

            avgHBValue = 0;
            avgHBValue = 0;
            avgHBCounter = 0;
            avgHBBufferLength = 30;
            avgHBValueBuffer = new double[avgHBBufferLength];

            realTimeHBValue = 0;
            realTimeHBCounter = 0;
            realTimeHBBufferLength = 4;
            realTimeHBValueBuffer = new double[realTimeHBBufferLength];

            currentPath = Directory.GetCurrentDirectory();

            this.MinimumSize = new System.Drawing.Size(711, 322);

            //grab the embedded audio file to play. note that player.Play() is a nonblocking function, so you can plot
            //and play audio and record data at the same time
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream s = a.GetManifestResourceStream("NeuroSky.ThinkGear.heartbeep.wav");
            player = new SoundPlayer(s);
        }


        




        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            Console.WriteLine("Disposing MainForm.");
            if(disposing) {
                if(components != null) {
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
        private void InitializeComponent() {
            this.connectButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.recordButton = new System.Windows.Forms.Button();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.portText = new System.Windows.Forms.TextBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.realtimeHeartRateLabel = new System.Windows.Forms.Label();
            this.fileLabel = new System.Windows.Forms.Label();
            this.averageHeartRateLabel = new System.Windows.Forms.Label();
            this.realtimeHeartRateLabelIndicator = new System.Windows.Forms.Label();
            this.averageHeartRateLabelIndicator = new System.Windows.Forms.Label();
            this.soundCheckBox = new System.Windows.Forms.CheckBox();
            this.fatigueButton = new System.Windows.Forms.Button();
            this.rawGraphPanel = new NeuroSky.MindView.GraphPanel();
            this.SuspendLayout();
            // 
            // connectButton
            // 
            this.connectButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectButton.Location = new System.Drawing.Point(134, 12);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(80, 24);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Connect";
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearButton.Location = new System.Drawing.Point(968, 512);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(80, 24);
            this.clearButton.TabIndex = 1;
            this.clearButton.Text = "Clear Graph";
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // recordButton
            // 
            this.recordButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.recordButton.Location = new System.Drawing.Point(862, 513);
            this.recordButton.Name = "recordButton";
            this.recordButton.Size = new System.Drawing.Size(80, 24);
            this.recordButton.TabIndex = 1;
            this.recordButton.Text = "Record";
            this.recordButton.Click += new System.EventHandler(this.recordButton_Click);
            // 
            // disconnectButton
            // 
            this.disconnectButton.Location = new System.Drawing.Point(134, 12);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(80, 24);
            this.disconnectButton.TabIndex = 1;
            this.disconnectButton.Text = "Disconnect";
            this.disconnectButton.Click += new System.EventHandler(this.disconnect_Click);
            // 
            // stopButton
            // 
            this.stopButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopButton.Location = new System.Drawing.Point(862, 513);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(80, 24);
            this.stopButton.TabIndex = 1;
            this.stopButton.Text = "Stop";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // portText
            // 
            this.portText.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.portText.Location = new System.Drawing.Point(10, 14);
            this.portText.Name = "portText";
            this.portText.Size = new System.Drawing.Size(80, 21);
            this.portText.TabIndex = 2;
            this.portText.Text = "Auto";
            this.portText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.portText_KeyPress);
            // 
            // statusLabel
            // 
            this.statusLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.Location = new System.Drawing.Point(12, 532);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(400, 24);
            this.statusLabel.TabIndex = 4;
            this.statusLabel.Text = "Type COM port to connect and press Connect";
            // 
            // realtimeHeartRateLabel
            // 
            this.realtimeHeartRateLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.realtimeHeartRateLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.realtimeHeartRateLabel.Location = new System.Drawing.Point(1019, 9);
            this.realtimeHeartRateLabel.Name = "realtimeHeartRateLabel";
            this.realtimeHeartRateLabel.Size = new System.Drawing.Size(51, 24);
            this.realtimeHeartRateLabel.TabIndex = 5;
            this.realtimeHeartRateLabel.Text = "0";
            this.realtimeHeartRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fileLabel
            // 
            this.fileLabel.Location = new System.Drawing.Point(0, 15);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(800, 24);
            this.fileLabel.TabIndex = 3;
            this.fileLabel.Text = "None";
            // 
            // averageHeartRateLabel
            // 
            this.averageHeartRateLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.averageHeartRateLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.averageHeartRateLabel.Location = new System.Drawing.Point(1019, 39);
            this.averageHeartRateLabel.Name = "averageHeartRateLabel";
            this.averageHeartRateLabel.Size = new System.Drawing.Size(51, 24);
            this.averageHeartRateLabel.TabIndex = 14;
            this.averageHeartRateLabel.Text = "0";
            this.averageHeartRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // realtimeHeartRateLabelIndicator
            // 
            this.realtimeHeartRateLabelIndicator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.realtimeHeartRateLabelIndicator.Location = new System.Drawing.Point(869, 11);
            this.realtimeHeartRateLabelIndicator.Name = "realtimeHeartRateLabelIndicator";
            this.realtimeHeartRateLabelIndicator.Size = new System.Drawing.Size(132, 19);
            this.realtimeHeartRateLabelIndicator.TabIndex = 15;
            this.realtimeHeartRateLabelIndicator.Text = "Real Time Heart Rate:";
            this.realtimeHeartRateLabelIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // averageHeartRateLabelIndicator
            // 
            this.averageHeartRateLabelIndicator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.averageHeartRateLabelIndicator.Location = new System.Drawing.Point(869, 42);
            this.averageHeartRateLabelIndicator.Name = "averageHeartRateLabelIndicator";
            this.averageHeartRateLabelIndicator.Size = new System.Drawing.Size(132, 19);
            this.averageHeartRateLabelIndicator.TabIndex = 16;
            this.averageHeartRateLabelIndicator.Text = "Average Heart Rate:";
            this.averageHeartRateLabelIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // soundCheckBox
            // 
            this.soundCheckBox.AutoSize = true;
            this.soundCheckBox.Font = new System.Drawing.Font("Arial", 9F);
            this.soundCheckBox.Location = new System.Drawing.Point(134, 46);
            this.soundCheckBox.Name = "soundCheckBox";
            this.soundCheckBox.Size = new System.Drawing.Size(104, 19);
            this.soundCheckBox.TabIndex = 17;
            this.soundCheckBox.Text = "Enable Sound";
            this.soundCheckBox.UseVisualStyleBackColor = true;
            // 
            // fatigueButton
            // 
            this.fatigueButton.Location = new System.Drawing.Point(734, 514);
            this.fatigueButton.Name = "fatigueButton";
            this.fatigueButton.Size = new System.Drawing.Size(100, 24);
            this.fatigueButton.TabIndex = 18;
            this.fatigueButton.Text = "Fatigue Meter";
            this.fatigueButton.UseVisualStyleBackColor = true;
            this.fatigueButton.Click += new System.EventHandler(this.fatigueButton_Click);
            // 
            // rawGraphPanel
            // 
            this.rawGraphPanel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rawGraphPanel.Location = new System.Drawing.Point(0, 76);
            this.rawGraphPanel.Name = "rawGraphPanel";
            this.rawGraphPanel.samplingRate = 10;
            this.rawGraphPanel.Size = new System.Drawing.Size(1079, 430);
            this.rawGraphPanel.TabIndex = 0;
            this.rawGraphPanel.xAxisMax = 0D;
            this.rawGraphPanel.xAxisMin = 0D;
            this.rawGraphPanel.yAxisMax = 0D;
            this.rawGraphPanel.yAxisMin = 0D;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(1078, 562);
            this.Controls.Add(this.fatigueButton);
            this.Controls.Add(this.soundCheckBox);
            this.Controls.Add(this.averageHeartRateLabelIndicator);
            this.Controls.Add(this.realtimeHeartRateLabelIndicator);
            this.Controls.Add(this.averageHeartRateLabel);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.recordButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.disconnectButton);
            this.Controls.Add(this.portText);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.realtimeHeartRateLabel);
            this.Controls.Add(this.rawGraphPanel);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BMD100 PC Starter Software";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void portText_KeyPress(object sender, KeyPressEventArgs e) {
            // If the key pressed was "Enter"
            if(e.KeyChar == (char)13) {
                //suppress the beep sounds by setting e.Handled = true
                e.Handled = true;
                connectButton_Click(sender, e);
            }
        }

        /*Connect Button Clicked*/
        private void connectButton_Click(object sender, System.EventArgs e) {
#if true
            this.connectButton.Enabled = false;
            this.portText.Enabled = false;

            rawGraphPanel.LineGraph.Clear();

            ConnectButtonClicked(this, EventArgs.Empty);
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
        private void disconnect_Click(object sender, System.EventArgs e) {
            DisconnectButtonClicked(this, EventArgs.Empty);
        }


        //check if there has been an R peak. if so, play a "beep"
        public void playBeep(int RRvalue, bool readyToPlay) {

            if(RRvalue > 0) {
                if((soundCheckBox.Checked) && (readyToPlay)) {
                    player.Play();
                }
            }
        }

        /*Clear Button Clicked*/
        private void clearButton_Click(object sender, System.EventArgs e) {
            rawGraphPanel.LineGraph.Clear();

            timeStampIndex = 0;

            rawGraphPanel.LineGraph.Invalidate();
        }

        //fatigue button clicked
        private void fatigueButton_Click(object sender, EventArgs e) {
            if(poorQuality == 200) {
                fatigueCounter = 0;
                Array.Clear(RRbuffer, 0, RRbuffer.Length);

                //disable the button
                updateFatigueButton(true);

                //set up the HRM file
                HRMoutFile = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Hour.ToString() + "-"
                + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".hrm";
                this.HRMstream = new System.IO.StreamWriter(HRMoutFile, true);

                //update the status bar
                updateStatusLabel("Recording...please hold position for approximately 2 minutes...");

                //start the fatigue meter
                runFatigueMeter = true;
            } else {
                //else, the fatigue meter was initialized while the fingers are not on the device. cancel this recording session
                runFatigueMeter = false;
                updateStatusLabel("Please place fingers on device and then start recording");
            }
        }

        //calculate the fatigue value based on RR interval
        public void calculateFatigue(int RRvalue) {
            //if the Fatigue Meter button has been pressed
            if(runFatigueMeter) {
                //if the return RR interval was between 150 and 800 points (292 msec to 1562 msec)
                if((RRvalue > 150) && (RRvalue < 800)) {

                    //if there are less than 110 RR values so far, add it to the buffer
                    if(fatigueCounter < 110) {
                        RRbuffer[fatigueCounter] = RRvalue;

                    } else {
                        //else there are now 110 values. or, the user has removed his hands from the device. write it out to a text file
                        runFatigueMeter = false;
                        outputFatigueResults(RRbuffer);
                    }

                    fatigueCounter++;
                }
            }
        }

        //save the output of the fatigue meter
        public void outputFatigueResults(int[] RRbuffer) {
            try {
                //write out the MILLISECOND values to the HRM file
                for(int k = 0; k < fatigueCounter; k++) {
                    HRMstream.WriteLine((int)((RRbuffer[k] * 1000.0) / 500.0));
                }
            } catch(Exception e) {
                Console.WriteLine("Unable to write HRM file: " + e.Message);
            }

            //stop the fatigue meter. close the file
            runFatigueMeter = false;
            HRMstream.Close();

            //re-enable the button
            updateFatigueButton(false);

            //update the status bar
            updateStatusLabel("Fatigue recording complete.");

            Thread showDialogThread = new Thread(new ThreadStart(showDialog));
            showDialogThread.SetApartmentState(ApartmentState.STA);
            showDialogThread.Start();     
        }

        //show the dialog box
        private void showDialog() {
            DialogResult res = saveHRMdialog.ShowDialog();

            //if the user pressed cancel, delete the file
            if(res == DialogResult.Cancel) {
                try {
                    System.IO.File.Delete(System.IO.Path.Combine(currentPath, HRMoutFile));
                } catch(Exception e) {
                    Console.WriteLine("canceled saving HRM file. couldn't delete the file: " + e.Message);
                }
            }
        }


        //save HRM file dialog box
        void saveHRMdialog_FileOk(object sender, CancelEventArgs e) {
            string fileName = saveHRMdialog.FileName;
            string path = Path.GetDirectoryName(fileName);
            
            try {
                if(!System.IO.Directory.Exists(path)) {
                    System.IO.Directory.CreateDirectory(path);
                }

                System.IO.File.Copy(System.IO.Path.Combine(currentPath, HRMoutFile), fileName, true);
                System.IO.File.Delete(System.IO.Path.Combine(currentPath, HRMoutFile));

            } catch(Exception ex) {
                MessageBox.Show("To save data in this directory, please exit the application and run as Administrator.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }


        /*Record Button Clicked*/
        private void recordButton_Click(object sender, System.EventArgs e) {
            recordButton.Enabled = false;
            recordButton.Visible = false;

            //save linegraph data in a seperate file
            //rawGraphPanel.LineGraph.SaveDataFlag = false;
            //rawGraphPanel.LineGraph.RecordDataFlag = true;

            /*Clear Block*/
            //rawGraphPanel.LineGraph.Clear();
            //rawGraphPanel.LineGraph.timeStampIndex = 0;

            stopButton.Enabled = true;
            stopButton.Visible = true;

            // Create new file and save to the "Data" folder
            dataLogOutFile = "dataLog-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Hour.ToString() + "-"
            + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".txt";
            
            ECGLogOutFile = "ECGLog-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Hour.ToString() + "-"
            + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".txt";
            
            // Create new filestream, appendable. write the headers
            this.dataLogStream = new System.IO.StreamWriter(dataLogOutFile, true);
            this.ECGLogStream = new System.IO.StreamWriter(ECGLogOutFile, true);

            if(dataLogStream != null) {
                this.dataLogStream.WriteLine("timestamp: CODE VALUEBYTE(S)");
            }

            if(ECGLogStream != null) {
                this.ECGLogStream.WriteLine("timestamp: ADC HeartRate4sAverage HeartRate30sAverage");
            }

            recordFlag = true;
        }


        //stop button clicked
        private void stopButton_Click(object sender, System.EventArgs e) {
            rawGraphPanel.LineGraph.SaveDataFlag = true;
            rawGraphPanel.LineGraph.RecordDataFlag = false;

            recordFlag = false;

            stopButton.Enabled = false;
            stopButton.Visible = false;

            recordButton.Enabled = true;
            recordButton.Visible = true;

            RecordStopTime = DateTime.Now;

            dataLogStream.Close();
            ECGLogStream.Close();

            saveFileGUI.updateDataLogTextBox(dataLogOutFile);
            saveFileGUI.updateECGLogTextBox(ECGLogOutFile);
            saveFileGUI.updatefolderPathTextBox(Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString());
            saveFileGUI.Location = new Point((this.Width - saveFileGUI.Width) / 2 + this.Location.X, (this.Height - saveFileGUI.Height) / 2 + this.Location.Y);

            saveFileGUI.Show();
        }

        //when the save button is clicked in the save dialog
        void OnSaveButtonClicked(object sender, EventArgs e) {

            try {
                if(!System.IO.Directory.Exists(saveFileGUI.folderPathTextBox.Text)) {
                    System.IO.Directory.CreateDirectory(saveFileGUI.folderPathTextBox.Text);
                }

                System.IO.File.Copy(System.IO.Path.Combine(currentPath, dataLogOutFile), System.IO.Path.Combine(saveFileGUI.folderPathTextBox.Text, saveFileGUI.dataLogTextBox.Text), true);
                System.IO.File.Copy(System.IO.Path.Combine(currentPath, ECGLogOutFile), System.IO.Path.Combine(saveFileGUI.folderPathTextBox.Text, saveFileGUI.sleepLogTextBox.Text), true);

                System.IO.File.Delete(System.IO.Path.Combine(currentPath, dataLogOutFile));
                System.IO.File.Delete(System.IO.Path.Combine(currentPath, ECGLogOutFile));

                saveFileGUI.Hide();
            } catch(Exception ex) {
                MessageBox.Show("To save data in this directory, please exit the application and run as Administrator.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }

        }

        //when the browse button is clicked in the save dialog
        void OnBrowseButtonClicked(object sender, EventArgs e) {

            //show the folder browser box
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if(folderBrowserDialog.ShowDialog() == DialogResult.OK) {
                saveFileGUI.updatefolderPathTextBox(folderBrowserDialog.SelectedPath);
            }
        }

        //when the browse button is clicked in the save dialog
        void OnDiscardButtonClicked(object sender, EventArgs e) {

            System.IO.File.Delete(System.IO.Path.Combine(currentPath, dataLogOutFile));
            System.IO.File.Delete(System.IO.Path.Combine(currentPath, ECGLogOutFile));

            saveFileGUI.Hide();
        }

        public void updateAverageHeartBeatValue(double heartBeatValue) {
            //if it's a good signal
            if(poorQuality == 200) {
                //if the buffer isnt full yet, add to the buffer. return the average heartbeat value based on however
                //much data is in the buffer currently
                if(avgHBCounter < avgHBBufferLength) {
                    avgHBValueBuffer[avgHBCounter] = heartBeatValue;
                    avgHBCounter++;

                    avgHBValue = 0;
                    for(int i = 0; i < avgHBCounter; i++) {
                        avgHBValue += avgHBValueBuffer[i];
                    }
                    avgHBValue = avgHBValue / avgHBCounter;

                    //update the label
                    updateAverageHeartRateLabel(Math.Round(avgHBValue).ToString());
                }
                    //else shift the buffer and add the most recent value to the end, calculate the average, and return the average value
                else {
                    //set the counter to the length of the index, to make sure that it doesn't become huge after looping so many times
                    avgHBCounter = avgHBBufferLength;

                    //shift the buffer
                    for(int i = 0; i < avgHBBufferLength - 1; i++) {
                        avgHBValueBuffer[i] = avgHBValueBuffer[i + 1];
                    }
                    avgHBValueBuffer[avgHBBufferLength - 1] = heartBeatValue;

                    //calculate the average
                    //reset
                    avgHBValue = 0;
                    for(int i = 0; i < avgHBBufferLength; i++) {
                        avgHBValue += (avgHBValueBuffer[i]);
                    }
                    avgHBValue = (avgHBValue / avgHBBufferLength);

                    //display the average heartbeat (rounded to nearest int)
                    updateAverageHeartRateLabel(Math.Round(avgHBValue).ToString());
                }
            }
                //else poor signal
            else {
                //set the counter back to zero. when its good signal again, it will refill the 30 seconds of new data before outputting average value
                avgHBCounter = 0;

                //for now, set the avgHBValue to zero
                avgHBValue = 0;

                //since poor signal, display 0 as average heartbeat
                updateAverageHeartRateLabel("0");
            }
        }

        public void updateRealTimeHeartBeatValue(double heartBeatValue) {
            //if it's a good signal
            if(poorQuality == 200) {
                //if the buffer isnt full yet, add to the buffer. return the average heartbeat value based on however
                //much data is in the buffer currently
                if(realTimeHBCounter < realTimeHBBufferLength) {
                    realTimeHBValueBuffer[realTimeHBCounter] = heartBeatValue;
                    realTimeHBCounter++;

                    realTimeHBValue = 0;
                    for(int i = 0; i < realTimeHBCounter; i++) {
                        realTimeHBValue += realTimeHBValueBuffer[i];
                    }
                    realTimeHBValue = realTimeHBValue / realTimeHBCounter;

                    //update the label
                    updateRealTimeHeartRateLabel(Math.Round(realTimeHBValue).ToString());
                }
                    //else shift the buffer and add the most recent value to the end, calculate the average, and return the average value
                else {
                    //set the counter to the length of the index, to make sure that it doesn't become huge after looping so many times
                    realTimeHBCounter = realTimeHBBufferLength;

                    //shift the buffer
                    for(int i = 0; i < realTimeHBBufferLength - 1; i++) {
                        realTimeHBValueBuffer[i] = realTimeHBValueBuffer[i + 1];
                    }
                    realTimeHBValueBuffer[realTimeHBBufferLength - 1] = heartBeatValue;

                    //calculate the average
                    //reset
                    realTimeHBValue = 0;
                    for(int i = 0; i < realTimeHBBufferLength; i++) {
                        realTimeHBValue += (realTimeHBValueBuffer[i]);
                    }
                    realTimeHBValue = (realTimeHBValue / realTimeHBBufferLength);

                    //display the average heartbeat (rounded to nearest int)
                    updateRealTimeHeartRateLabel(Math.Round(realTimeHBValue).ToString());
                }
            }
                //else poor signal
            else {
                //set the counter back to zero. when its good signal again, it will refill the 30 seconds of new data before outputting average value
                realTimeHBCounter = 0;

                //for now, set the avgHBValue to zero
                realTimeHBValue = 0;

                //since poor signal, display 0 as average heartbeat
                updateRealTimeHeartRateLabel("0");
            }
        }


        /**
        * Record out the received data to a dataLog file
        * Record the RAW ECG value and heartbeat to an ECGLog file
        */
        public void recordData(ThinkGear.DataRow[] dataRowArray) {
            if((dataLogStream != null) && (ECGLogStream != null)) {
                foreach(DataRow dr in dataRowArray) {
                    //suppress the EGODebug1, EGODebug2, and EGODebug3 outputs
                    if((dr.Type.GetHashCode() != 0x84) && (dr.Type.GetHashCode() != 0x08) && (dr.Type.GetHashCode() != 0x85)) {
                        //write the timestamp
                        dataLogStream.Write(dr.Time.ToString("#0.000") + ": " + (dr.Type.GetHashCode().ToString("X").PadLeft(2, '0')) + " ");

                        //print out the hex values into datalog
                        foreach(byte b in dr.Data) {
                            dataLogStream.Write(b.ToString("X").PadLeft(2, '0') + " ");
                        }
                        dataLogStream.Write(dataLogStream.NewLine);
                    }

                    //also print to the EGCLog
                    if(dr.Type.GetHashCode() == 0x80) {
                        //write the timestamp
                        ECGLogStream.Write(dr.Time.ToString("#0.000") + ": ");

                        //print out the EGC waveform, ASIC heartbeat value, and "real time" heartbeat value into EGCLog
                        if(dr.Type.GetHashCode() == 0x80) {
                            ADCValue = (short)((dr.Data[0] << 8) + dr.Data[1]);
                            ECGLogStream.Write(ADCValue.ToString().PadLeft(6, ' ') + " " + ASICHBValue.ToString().PadLeft(3, ' ') + " " + Math.Round(realTimeHBValue).ToString().PadLeft(3, ' '));
                            ECGLogStream.Write(ECGLogStream.NewLine);
                        }
                    }
                }
            }
        }


        //when data saving is done
        void OnDataSavingFinished(object sender, EventArgs e) {
            //no need for this currently
        }


        //update the connect button status
        delegate void updateConnectButtonDelegate(bool connected);
        public void updateConnectButton(bool connected) {
            if(this.InvokeRequired) {
                updateConnectButtonDelegate del = new updateConnectButtonDelegate(updateConnectButton);
                this.Invoke(del, new object[] { connected });
            } else {
                if(connected) {
                    this.connectButton.Enabled = false;
                    this.connectButton.Visible = false;

                    this.portText.Enabled = false;

                    this.disconnectButton.Enabled = true;
                    this.disconnectButton.Visible = true;
                } else {
                    this.disconnectButton.Enabled = false;
                    this.disconnectButton.Visible = false;

                    this.portText.Enabled = true;

                    this.connectButton.Enabled = true;
                    this.connectButton.Visible = true;
                }

            }
        }


        //update the fatigue button status
        delegate void UpdateFatigueButtonDelegate(bool enabled);
        public void updateFatigueButton(bool enabled) {
            if(this.InvokeRequired) {
                UpdateFatigueButtonDelegate del = new UpdateFatigueButtonDelegate(updateFatigueButton);
                this.Invoke(del, new object[] { enabled });
            } else {
                if(enabled) {
                    this.fatigueButton.Enabled = false;
                } else {
                    this.fatigueButton.Enabled = true;
                }
            }
        }


        //update the realtime heart rate label status
        delegate void updateRealTimeHeartRateLabelDelegate(string tempString);
        public void updateRealTimeHeartRateLabel(string tempString) {
            if(this.InvokeRequired) {
                //try catch necessary for handling case when form is disposing
                try {
                    updateRealTimeHeartRateLabelDelegate del = new updateRealTimeHeartRateLabelDelegate(updateRealTimeHeartRateLabel);
                    this.Invoke(del, new object[] { tempString });
                } catch(Exception e) {
                    Console.WriteLine("Caught exception at updateRealTimeHeartRateLabel: " + e.Message);
                }

            } else {
                this.realtimeHeartRateLabel.Text = tempString;
            }
        }

        //update the average heart rate label status
        delegate void updateAverageHeartRateLabelDelegate(string tempString);
        public void updateAverageHeartRateLabel(string tempString) {
            if(this.InvokeRequired) {
                //try catch necessary for handling case when form is disposing
                try {
                    updateAverageHeartRateLabelDelegate del = new updateAverageHeartRateLabelDelegate(updateAverageHeartRateLabel);
                    this.Invoke(del, new object[] { tempString });
                } catch(Exception e) {
                    Console.WriteLine("Caught exception at updateAverageHeartRateLabel: " + e.Message);
                }

            } else {
                this.averageHeartRateLabel.Text = tempString;
            }
        }


        delegate void updateStatusLabelDelegate(string tempText);
        public void updateStatusLabel(string tempText) {
            if(this.InvokeRequired) {
                updateStatusLabelDelegate del = new updateStatusLabelDelegate(updateStatusLabel);
                this.Invoke(del, new object[] { tempText });
            } else {
                this.statusLabel.Text = tempText;
            }
        }


        protected override void OnSizeChanged(EventArgs e) {

            realtimeHeartRateLabelIndicator.Location = new System.Drawing.Point(this.Width - 215, realtimeHeartRateLabelIndicator.Location.Y);
            realtimeHeartRateLabel.Location = new System.Drawing.Point(this.Width - 71, realtimeHeartRateLabel.Location.Y);

            averageHeartRateLabelIndicator.Location = new System.Drawing.Point(this.Width - 215, realtimeHeartRateLabelIndicator.Location.Y + 30);
            averageHeartRateLabel.Location = new System.Drawing.Point(this.Width - 71, realtimeHeartRateLabel.Location.Y + 30);

            statusLabel.Location = new System.Drawing.Point(statusLabel.Location.X, this.Height - 54);

            recordButton.Location = new System.Drawing.Point(this.Width - 240, this.Height - 73);
            stopButton.Location = new System.Drawing.Point(this.Width - 240, this.Height - 73);

            clearButton.Location = new System.Drawing.Point(this.Width - 140, this.Height - 73);

            fatigueButton.Location = new System.Drawing.Point(this.Width - 360, this.Height - 73);

            rawGraphPanel.Location = new Point(rawGraphPanel.Location.X, soundCheckBox.Location.Y + soundCheckBox.Height + 9);
            rawGraphPanel.Height = (int)(recordButton.Location.Y - rawGraphPanel.Location.Y - 15);
            rawGraphPanel.Width = this.Width - 10;

            base.OnSizeChanged(e);
        }

       

    }
    /*End of MainForm*/
}
