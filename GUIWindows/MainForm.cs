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
        
        private EnergyLevel energyLevel;
        
        public bool runFatigueMeter = false;   //fatigue meter is off by default
        private int fatigueResult;              //output of the EnergyLevel algorithm
        private int fatigueTime;                //holds a record of how many seconds have passed since the RR recording began
        
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
        
        private string currentPath;
        
        public event EventHandler ConnectButtonClicked = delegate { };
        public event EventHandler DisconnectButtonClicked = delegate { };

        public CheckBox soundCheckBox;
        private Button startFatigueButton;
        private Label fatigueLabelIndicator;
        public Label fatigueLabel;
        private Button stopFatigueButton;
        private PictureBox energyPictureBox;
        public SoundPlayer player;

        private Bitmap emptyImage;
        private Bitmap lowImage;
        private Bitmap mediumImage;
        private Label HRVLabelIndicator;
        public Label HRVLabel;
        private Bitmap fullImage;

        public MainForm() {

            saveFileGUI = new SaveFileGUI();
            saveFileGUI.SaveButtonClicked += new EventHandler(OnSaveButtonClicked);
            saveFileGUI.DiscardButtonClicked += new EventHandler(OnDiscardButtonClicked);
            saveFileGUI.BrowseButtonClicked += new EventHandler(OnBrowseButtonClicked);
            saveFileGUI.StartPosition = FormStartPosition.Manual;

            energyLevel = new EnergyLevel();
            
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
            System.IO.Stream s = a.GetManifestResourceStream("NeuroSky.ThinkGear.Resources.heartbeep.wav");
            player = new SoundPlayer(s);

            System.IO.Stream emptyStream = a.GetManifestResourceStream("NeuroSky.ThinkGear.Resources.empty.gif");
            emptyImage = new Bitmap(emptyStream);

            System.IO.Stream lowStream = a.GetManifestResourceStream("NeuroSky.ThinkGear.Resources.low.gif");
            lowImage = new Bitmap(lowStream);

            System.IO.Stream mediumStream = a.GetManifestResourceStream("NeuroSky.ThinkGear.Resources.medium.gif");
            mediumImage = new Bitmap(mediumStream);

            System.IO.Stream fullStream = a.GetManifestResourceStream("NeuroSky.ThinkGear.Resources.full.gif");
            fullImage = new Bitmap(fullStream);
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
            this.startFatigueButton = new System.Windows.Forms.Button();
            this.fatigueLabelIndicator = new System.Windows.Forms.Label();
            this.fatigueLabel = new System.Windows.Forms.Label();
            this.stopFatigueButton = new System.Windows.Forms.Button();
            this.energyPictureBox = new System.Windows.Forms.PictureBox();
            this.HRVLabelIndicator = new System.Windows.Forms.Label();
            this.HRVLabel = new System.Windows.Forms.Label();
            this.rawGraphPanel = new NeuroSky.MindView.GraphPanel();
            ((System.ComponentModel.ISupportInitialize)(this.energyPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // connectButton
            // 
            this.connectButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectButton.Location = new System.Drawing.Point(134, 11);
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
            this.disconnectButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.realtimeHeartRateLabel.Location = new System.Drawing.Point(974, 7);
            this.realtimeHeartRateLabel.Name = "realtimeHeartRateLabel";
            this.realtimeHeartRateLabel.Size = new System.Drawing.Size(92, 24);
            this.realtimeHeartRateLabel.TabIndex = 5;
            this.realtimeHeartRateLabel.Text = "0";
            this.realtimeHeartRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.realtimeHeartRateLabel.Click += new System.EventHandler(this.realtimeHeartRateLabel_Click);
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
            this.averageHeartRateLabel.Location = new System.Drawing.Point(974, 37);
            this.averageHeartRateLabel.Name = "averageHeartRateLabel";
            this.averageHeartRateLabel.Size = new System.Drawing.Size(92, 24);
            this.averageHeartRateLabel.TabIndex = 14;
            this.averageHeartRateLabel.Text = "0";
            this.averageHeartRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.averageHeartRateLabel.Click += new System.EventHandler(this.averageHeartRateLabel_Click);
            // 
            // realtimeHeartRateLabelIndicator
            // 
            this.realtimeHeartRateLabelIndicator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.realtimeHeartRateLabelIndicator.Location = new System.Drawing.Point(824, 9);
            this.realtimeHeartRateLabelIndicator.Name = "realtimeHeartRateLabelIndicator";
            this.realtimeHeartRateLabelIndicator.Size = new System.Drawing.Size(132, 19);
            this.realtimeHeartRateLabelIndicator.TabIndex = 15;
            this.realtimeHeartRateLabelIndicator.Text = "Real Time Heart Rate:";
            this.realtimeHeartRateLabelIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.realtimeHeartRateLabelIndicator.Click += new System.EventHandler(this.realtimeHeartRateLabelIndicator_Click);
            // 
            // averageHeartRateLabelIndicator
            // 
            this.averageHeartRateLabelIndicator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.averageHeartRateLabelIndicator.Location = new System.Drawing.Point(824, 40);
            this.averageHeartRateLabelIndicator.Name = "averageHeartRateLabelIndicator";
            this.averageHeartRateLabelIndicator.Size = new System.Drawing.Size(132, 19);
            this.averageHeartRateLabelIndicator.TabIndex = 16;
            this.averageHeartRateLabelIndicator.Text = "Average Heart Rate:";
            this.averageHeartRateLabelIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.averageHeartRateLabelIndicator.Click += new System.EventHandler(this.averageHeartRateLabelIndicator_Click);
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
            // startFatigueButton
            // 
            this.startFatigueButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startFatigueButton.Location = new System.Drawing.Point(733, 513);
            this.startFatigueButton.Name = "startFatigueButton";
            this.startFatigueButton.Size = new System.Drawing.Size(100, 24);
            this.startFatigueButton.TabIndex = 18;
            this.startFatigueButton.Text = "Energy Level";
            this.startFatigueButton.UseVisualStyleBackColor = true;
            this.startFatigueButton.Click += new System.EventHandler(this.fatigueButton_Click);
            // 
            // fatigueLabelIndicator
            // 
            this.fatigueLabelIndicator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fatigueLabelIndicator.Location = new System.Drawing.Point(585, 61);
            this.fatigueLabelIndicator.Name = "fatigueLabelIndicator";
            this.fatigueLabelIndicator.Size = new System.Drawing.Size(84, 19);
            this.fatigueLabelIndicator.TabIndex = 20;
            this.fatigueLabelIndicator.Text = "Energy Level:";
            this.fatigueLabelIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.fatigueLabelIndicator.Visible = false;
            // 
            // fatigueLabel
            // 
            this.fatigueLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fatigueLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.fatigueLabel.Location = new System.Drawing.Point(687, 59);
            this.fatigueLabel.Name = "fatigueLabel";
            this.fatigueLabel.Size = new System.Drawing.Size(83, 24);
            this.fatigueLabel.TabIndex = 19;
            this.fatigueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fatigueLabel.Visible = false;
            // 
            // stopFatigueButton
            // 
            this.stopFatigueButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopFatigueButton.Location = new System.Drawing.Point(733, 512);
            this.stopFatigueButton.Name = "stopFatigueButton";
            this.stopFatigueButton.Size = new System.Drawing.Size(100, 24);
            this.stopFatigueButton.TabIndex = 21;
            this.stopFatigueButton.Text = "Stop";
            this.stopFatigueButton.UseVisualStyleBackColor = true;
            this.stopFatigueButton.Visible = false;
            this.stopFatigueButton.Click += new System.EventHandler(this.stopFatigueMeter_Click);
            // 
            // energyPictureBox
            // 
            this.energyPictureBox.BackColor = System.Drawing.SystemColors.Control;
            this.energyPictureBox.ErrorImage = null;
            this.energyPictureBox.InitialImage = global::NeuroSky.ThinkGear.Properties.Resources.full;
            this.energyPictureBox.Location = new System.Drawing.Point(594, 7);
            this.energyPictureBox.Name = "energyPictureBox";
            this.energyPictureBox.Size = new System.Drawing.Size(115, 49);
            this.energyPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.energyPictureBox.TabIndex = 22;
            this.energyPictureBox.TabStop = false;
            this.energyPictureBox.Visible = false;
            // 
            // HRVLabelIndicator
            // 
            this.HRVLabelIndicator.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HRVLabelIndicator.Location = new System.Drawing.Point(824, 64);
            this.HRVLabelIndicator.Name = "HRVLabelIndicator";
            this.HRVLabelIndicator.Size = new System.Drawing.Size(132, 19);
            this.HRVLabelIndicator.TabIndex = 24;
            this.HRVLabelIndicator.Text = "R-R interval:";
            this.HRVLabelIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.HRVLabelIndicator.Click += new System.EventHandler(this.HRVLabelIndicator_Click);
            // 
            // HRVLabel
            // 
            this.HRVLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HRVLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.HRVLabel.Location = new System.Drawing.Point(974, 64);
            this.HRVLabel.Name = "HRVLabel";
            this.HRVLabel.Size = new System.Drawing.Size(92, 24);
            this.HRVLabel.TabIndex = 23;
            this.HRVLabel.Text = "0";
            this.HRVLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HRVLabel.Click += new System.EventHandler(this.HRVLabel_Click);
            // 
            // rawGraphPanel
            // 
            this.rawGraphPanel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rawGraphPanel.Location = new System.Drawing.Point(0, 96);
            this.rawGraphPanel.Name = "rawGraphPanel";
            this.rawGraphPanel.samplingRate = 10;
            this.rawGraphPanel.Size = new System.Drawing.Size(1079, 410);
            this.rawGraphPanel.TabIndex = 0;
            this.rawGraphPanel.xAxisMax = 0D;
            this.rawGraphPanel.xAxisMin = 0D;
            this.rawGraphPanel.yAxisMax = 0D;
            this.rawGraphPanel.yAxisMin = 0D;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(1078, 562);
            this.Controls.Add(this.HRVLabelIndicator);
            this.Controls.Add(this.HRVLabel);
            this.Controls.Add(this.energyPictureBox);
            this.Controls.Add(this.stopFatigueButton);
            this.Controls.Add(this.fatigueLabelIndicator);
            this.Controls.Add(this.fatigueLabel);
            this.Controls.Add(this.startFatigueButton);
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
            this.Text = "CardioChip PC Starter Software 2.1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.energyPictureBox)).EndInit();
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


        //if there has been an R peak, play a "beep"
        public void playBeep() {
            if(soundCheckBox.Checked) {
                player.Play();
            }
        }

        /*Clear Button Clicked*/
        private void clearButton_Click(object sender, System.EventArgs e) {
            rawGraphPanel.LineGraph.Clear();

            timeStampIndex = 0;

            rawGraphPanel.LineGraph.Invalidate();
        }


     

        //fatigue button clicked. set up stuff
        private void fatigueButton_Click(object sender, EventArgs e) {
            updateFatigueLevelLabel("Calculating...");
            toggleFatigueLevelLabel(true);
            toggleFatigueLevelLabelIndicator(true);
            toggleRecordButton(false);
            toggleEnergyPictureBox(false);
            
            fatigueResult = 0;

            //reset the meter
            fatigueResult = energyLevel.addInterval(0, 0);

            //disable the button
            toggleFatigueStartButton(false);
            toggleFatigueStopButton(true);

            //update the status bar
            updateStatusLabel("Recording...please hold position for approximately 1 minute...");

            //start the fatigue meter
            runFatigueMeter = true;
        }


        //stop fatigue button clicked
        private void stopFatigueMeter_Click(object sender, EventArgs e) {
            runFatigueMeter = false;

            //reset the meter
            fatigueResult = energyLevel.addInterval(0, 0);

            outputFatigueResults(fatigueResult);
        }




        //calculate the fatigue value based on RR interval
        public void calculateFatigue(int RRvalue) {
            //if the Fatigue Meter button has been pressed
            if(runFatigueMeter) {
                //if the energy level is less than 1
                if(fatigueResult < 1) {

                    fatigueResult = energyLevel.addInterval((int)((RRvalue * 1000.0) / 512.0), (byte)poorQuality);

                } else {
                    //else energy level is now greater than 0. or, the user has pressed the stop button. write it out to a text file
                    runFatigueMeter = false;
                    outputFatigueResults(fatigueResult);
                }

            }
        }
        



        //save the output of the fatigue meter
        public void outputFatigueResults(int fatigueLevel) {


            if(fatigueLevel > 0) {    //if we actually got a fatigue level
                updateFatigueLevelLabel(fatigueLevel.ToString());
                updateStatusLabel("Energy recording complete.");

                if(fatigueLevel < 25) {
                    setEnergyPictureBox(emptyImage);
                } else if(fatigueLevel < 50) {
                    setEnergyPictureBox(lowImage);
                } else if(fatigueLevel < 75) {
                    setEnergyPictureBox(mediumImage);
                } else if(fatigueLevel <= 100) {
                    setEnergyPictureBox(fullImage);
                }
                toggleEnergyPictureBox(true);

            } else {    //fatigue result = -1, because the user stopped it early
                updateFatigueLevelLabel("");
                toggleFatigueLevelLabelIndicator(true);
                toggleFatigueLevelLabel(true);
                updateStatusLabel("Data recording ended early. Please try recording again.");
            }
            
            //stop the fatigue meter
            runFatigueMeter = false;

            //re-enable the button
            toggleFatigueStartButton(true);
            toggleFatigueStopButton(false);
            toggleRecordButton(true);

        }


        /*Record Button Clicked*/
        private void recordButton_Click(object sender, System.EventArgs e) {
            recordButton.Enabled = false;
            recordButton.Visible = false;
            fatigueStartButton(false);

            updateStatusLabel("Recording...");

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
            dataLogOutFile = Path.Combine(currentPath, dataLogOutFile);
            
            ECGLogOutFile = "ECGLog-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Hour.ToString() + "-"
            + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".txt";
            ECGLogOutFile = Path.Combine(currentPath, ECGLogOutFile);

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
            updateStatusLabel("Recording complete.");

            recordFlag = false;

            stopButton.Enabled = false;
            stopButton.Visible = false;

            fatigueStartButton(true);

            recordButton.Enabled = true;
            recordButton.Visible = true;

            RecordStopTime = DateTime.Now;

            dataLogStream.Close();
            ECGLogStream.Close();

            saveFileGUI.updateDataLogTextBox(Path.GetFileName(dataLogOutFile));
            saveFileGUI.updateECGLogTextBox(Path.GetFileName(ECGLogOutFile));
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

                System.IO.File.Copy(dataLogOutFile, System.IO.Path.Combine(saveFileGUI.folderPathTextBox.Text, saveFileGUI.dataLogTextBox.Text), true);
                System.IO.File.Copy(ECGLogOutFile, System.IO.Path.Combine(saveFileGUI.folderPathTextBox.Text, saveFileGUI.sleepLogTextBox.Text), true);

                System.IO.File.Delete(dataLogOutFile);
                System.IO.File.Delete(ECGLogOutFile);

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
                    updateAverageHeartRateLabel(Math.Round(avgHBValue).ToString() + " bpm");
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
                    updateAverageHeartRateLabel(Math.Round(avgHBValue).ToString() + " bpm");
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
                    updateRealTimeHeartRateLabel(Math.Round(realTimeHBValue).ToString() + " bpm");
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
                    updateRealTimeHeartRateLabel(Math.Round(realTimeHBValue).ToString() + " bpm");
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

        //show/hide the engergy picture box
        delegate void ToggleEnergyPictureBoxDelegate(bool visible);
        public void toggleEnergyPictureBox(bool visible) {
            if(this.InvokeRequired) {
                ToggleEnergyPictureBoxDelegate del = new ToggleEnergyPictureBoxDelegate(toggleEnergyPictureBox);
                this.Invoke(del, new object[] { visible });
            } else {
                this.energyPictureBox.Visible = visible;
            }
        }

        //show a certain image on the picture box
        delegate void SetEnergyPictureBoxDelegate(Bitmap image);
        public void setEnergyPictureBox(Bitmap image) {
            if(this.InvokeRequired) {
                SetEnergyPictureBoxDelegate del = new SetEnergyPictureBoxDelegate(setEnergyPictureBox);
                this.Invoke(del, new object[] { image });
            } else {
                this.energyPictureBox.Image = image;
            }
        }


        //update the fatigue start button
        delegate void ToggleFatigueStartButtonDelegate(bool visible);
        public void toggleFatigueStartButton(bool visible) {
            if(this.InvokeRequired) {
                ToggleFatigueStartButtonDelegate del = new ToggleFatigueStartButtonDelegate(toggleFatigueStartButton);
                this.Invoke(del, new object[] { visible });
            } else {
                this.startFatigueButton.Visible = visible;
            }
        }

        //disable/enable the fatigue start button
        delegate void FatigueStartButtonDelegate(bool enabled);
        public void fatigueStartButton(bool enabled) {
            if(this.InvokeRequired) {
                FatigueStartButtonDelegate del = new FatigueStartButtonDelegate(fatigueStartButton);
                this.Invoke(del, new object[] { enabled });
            } else {
                this.startFatigueButton.Enabled = enabled;
            }
        }

        //update the fatigue stop button
        delegate void ToggleFatigueStopButtonDelegate(bool visible);
        public void toggleFatigueStopButton(bool visible) {
            if(this.InvokeRequired) {
                ToggleFatigueStopButtonDelegate del = new ToggleFatigueStopButtonDelegate(toggleFatigueStopButton);
                this.Invoke(del, new object[] { visible });
            } else {
                this.stopFatigueButton.Visible = visible;
            }
        }


        //update the fatigue level
        delegate void UpdateFatigueLabelDelegate(string tempString);
        public void updateFatigueLevelLabel(string tempString) {
            if((!this.Disposing) && (!this.IsDisposed)) {
                if(this.InvokeRequired) {
                    //try catch necessary for handling case when form is disposing
                    try {
                        UpdateFatigueLabelDelegate del = new UpdateFatigueLabelDelegate(updateFatigueLevelLabel);
                        this.Invoke(del, new object[] { tempString });
                    } catch(Exception e) {
                        Console.WriteLine("Caught exception at updateFatigueLevelLabel: " + e.Message);
                    }

                } else {

                    //if the status is "calculating...", change the font to something smaller
                    if(String.Equals("Calculating...", tempString)) {
                        fatigueLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    } else {
                        fatigueLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    }

                    this.fatigueLabel.Text = tempString;
                }
            }
        }


        //make the fatigue label visible/invisible
        delegate void ToggleFatigueLabelDelegate(bool visible);
        public void toggleFatigueLevelLabel(bool visible) {
            if(this.InvokeRequired) {
                //try catch necessary for handling case when form is disposing
                try {
                    ToggleFatigueLabelDelegate del = new ToggleFatigueLabelDelegate(toggleFatigueLevelLabel);
                    this.Invoke(del, new object[] { visible });
                } catch(Exception e) {
                    Console.WriteLine("Caught exception at toggleFatigueLevelLabel: " + e.Message);
                }

            } else {
                this.fatigueLabel.Visible = visible;
            }
        }


        //make the fatigue label indicator visible/invisible
        delegate void ToggleFatigueLabelIndicatorDelegate(bool visible);
        public void toggleFatigueLevelLabelIndicator(bool visible) {
            if(this.InvokeRequired) {
                //try catch necessary for handling case when form is disposing
                try {
                    ToggleFatigueLabelIndicatorDelegate del = new ToggleFatigueLabelIndicatorDelegate(toggleFatigueLevelLabelIndicator);
                    this.Invoke(del, new object[] { visible });
                } catch(Exception e) {
                    Console.WriteLine("Caught exception at toggleFatigueLevelLabelIndicator: " + e.Message);
                }

            } else {
                this.fatigueLabelIndicator.Visible = visible;
            }
        }

        
        //make the record button enabled/disabled
        delegate void ToggleRecordButtonDelegate(bool enabled);
        public void toggleRecordButton(bool enabled) {
            if(this.InvokeRequired) {
                //try catch necessary for handling case when form is disposing
                try {
                    ToggleRecordButtonDelegate del = new ToggleRecordButtonDelegate(toggleRecordButton);
                    this.Invoke(del, new object[] { enabled });
                } catch(Exception e) {
                    Console.WriteLine("Caught exception at toggleRecordButton: " + e.Message);
                }

            } else {
                this.recordButton.Enabled = enabled;
            }
        }


        //update the realtime heart rate label status
        delegate void UpdateRealTimeHeartRateLabelDelegate(string tempString);
        public void updateRealTimeHeartRateLabel(string tempString) {
            if((!this.Disposing) && (!this.IsDisposed)) {
                if(this.InvokeRequired) {
                    //try catch necessary for handling case when form is disposing
                    try {
                        UpdateRealTimeHeartRateLabelDelegate del = new UpdateRealTimeHeartRateLabelDelegate(updateRealTimeHeartRateLabel);
                        this.Invoke(del, new object[] { tempString });
                    } catch(Exception e) {
                        Console.WriteLine("Caught exception at updateRealTimeHeartRateLabel: " + e.Message);
                    }

                } else {
                    this.realtimeHeartRateLabel.Text = tempString;
                }
            }
        }

        //update the average heart rate label status
        delegate void UpdateAverageHeartRateLabelDelegate(string tempString);
        public void updateAverageHeartRateLabel(string tempString) {
            if((!this.Disposing) && (!this.IsDisposed)) {
                if(this.InvokeRequired) {
                    //try catch necessary for handling case when form is disposing
                    try {
                        UpdateAverageHeartRateLabelDelegate del = new UpdateAverageHeartRateLabelDelegate(updateAverageHeartRateLabel);
                        this.Invoke(del, new object[] { tempString });
                    } catch(Exception e) {
                        Console.WriteLine("Caught exception at updateAverageHeartRateLabel: " + e.Message);
                    }

                } else {
                    this.averageHeartRateLabel.Text = tempString;
                }
            }
        }


        //update the HRV label
        delegate void UpdateHRVLabelDelegate(string tempString);
        public void updateHRVLabel(string tempString) {
            if((!this.Disposing) && (!this.IsDisposed)) {
                if(this.InvokeRequired) {
                    try {
                        UpdateHRVLabelDelegate del = new UpdateHRVLabelDelegate(updateHRVLabel);
                        this.Invoke(del, new object[] { tempString });
                    } catch(Exception e) {
                        Console.WriteLine("caught exception at UpdateHRVLabel: " + e.Message);
                    }
                } else {
                    this.HRVLabel.Text = tempString;
                }
            }
        }



        delegate void UpdateStatusLabelDelegate(string tempText);
        public void updateStatusLabel(string tempText) {
            if((!this.Disposing) && (!this.IsDisposed)) {
                if(this.InvokeRequired) {
                    UpdateStatusLabelDelegate del = new UpdateStatusLabelDelegate(updateStatusLabel);
                    this.Invoke(del, new object[] { tempText });
                } else {
                    this.statusLabel.Text = tempText;
                }
            }
        }


        
        protected override void OnSizeChanged(EventArgs e) {

            realtimeHeartRateLabelIndicator.Location = new System.Drawing.Point(this.Width - 262, realtimeHeartRateLabelIndicator.Location.Y);
            realtimeHeartRateLabel.Location = new System.Drawing.Point(this.Width - 112, realtimeHeartRateLabel.Location.Y);

            averageHeartRateLabelIndicator.Location = new System.Drawing.Point(this.Width - 262, realtimeHeartRateLabelIndicator.Location.Y + 30);
            averageHeartRateLabel.Location = new System.Drawing.Point(this.Width - 112, realtimeHeartRateLabel.Location.Y + 30);

            HRVLabelIndicator.Location = new System.Drawing.Point(this.Width - 262, averageHeartRateLabelIndicator.Location.Y + 30);
            HRVLabel.Location = new System.Drawing.Point(this.Width - 112, averageHeartRateLabel.Location.Y + 30);

            energyPictureBox.Location = new System.Drawing.Point(this.Width - 492, energyPictureBox.Location.Y);

            fatigueLabelIndicator.Location = new System.Drawing.Point(this.Width - 501, energyPictureBox.Location.Y + energyPictureBox.Height + 8);
            fatigueLabel.Location = new System.Drawing.Point(this.Width - 399, energyPictureBox.Location.Y + energyPictureBox.Height + 6);

            statusLabel.Location = new System.Drawing.Point(statusLabel.Location.X, this.Height - 54);

            recordButton.Location = new System.Drawing.Point(this.Width - 240, this.Height - 73);
            stopButton.Location = new System.Drawing.Point(this.Width - 240, this.Height - 73);

            clearButton.Location = new System.Drawing.Point(this.Width - 140, this.Height - 73);

            startFatigueButton.Location = new System.Drawing.Point(this.Width - 360, this.Height - 73);
            stopFatigueButton.Location = new System.Drawing.Point(this.Width - 360, this.Height - 73);

            rawGraphPanel.Location = new Point(rawGraphPanel.Location.X, HRVLabelIndicator.Location.Y + HRVLabelIndicator.Height + 9);
            rawGraphPanel.Height = (int)(recordButton.Location.Y - rawGraphPanel.Location.Y - 15);
            rawGraphPanel.Width = this.Width - 10;

            base.OnSizeChanged(e);
        }

        private void MainForm_Load(object sender, EventArgs e) {

        }

        private void realtimeHeartRateLabel_Click(object sender, EventArgs e) {

        }

        private void averageHeartRateLabel_Click(object sender, EventArgs e) {

        }

        private void HRVLabel_Click(object sender, EventArgs e) {

        }

        private void averageHeartRateLabelIndicator_Click(object sender, EventArgs e) {

        }

        private void realtimeHeartRateLabelIndicator_Click(object sender, EventArgs e) {

        }

        private void HRVLabelIndicator_Click(object sender, EventArgs e) {

        }
        
     

       

    }
    /*End of MainForm*/
}
