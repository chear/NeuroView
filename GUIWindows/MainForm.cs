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

namespace NeuroSky.MindView {
    public class MainForm : System.Windows.Forms.Form {
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
        private string directoryToSave; //= Directory.GetCurrentDirectory();
        //private string folderToSave = "\\Data\\";

        public event EventHandler ConnectButtonClicked = delegate { };
        public event EventHandler DisconnectButtonClicked = delegate { };


        public MainForm() {

            InitializeComponent();

            recordFlag = false;

            rawGraphPanel.samplingRate = 512;
            rawGraphPanel.xAxisMax = 4;
            rawGraphPanel.xAxisMin = 0;
            rawGraphPanel.yAxisMax = 32768;
            rawGraphPanel.yAxisMin = -32768;
            rawGraphPanel.Text = "ECG";
            rawGraphPanel.EnableValueDisplay();
            rawGraphPanel.OptimizeScrollBar();
            rawGraphPanel.DataSavingFinished += new EventHandler(OnDataSavingFinished);
            rawGraphPanel.LineGraph.DCRemovalEnabled = true;

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

            //set the directory to save data as the current directory\Data folder
            directoryToSave = string.Concat(Directory.GetCurrentDirectory(), "\\Data\\");

            this.MinimumSize = new System.Drawing.Size(711, 322);
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
            this.rawGraphPanel = new NeuroSky.MindView.GraphPanel();
            this.SuspendLayout();
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(136, 24);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(80, 24);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Connect";
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(831, 294);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(80, 24);
            this.clearButton.TabIndex = 1;
            this.clearButton.Text = "Clear Graph";
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // recordButton
            // 
            this.recordButton.Location = new System.Drawing.Point(701, 294);
            this.recordButton.Name = "recordButton";
            this.recordButton.Size = new System.Drawing.Size(80, 24);
            this.recordButton.TabIndex = 1;
            this.recordButton.Text = "Record";
            this.recordButton.Click += new System.EventHandler(this.recordButton_Click);
            // 
            // disconnectButton
            // 
            this.disconnectButton.Location = new System.Drawing.Point(136, 24);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(80, 24);
            this.disconnectButton.TabIndex = 1;
            this.disconnectButton.Text = "Disconnect";
            this.disconnectButton.Click += new System.EventHandler(this.disconnect_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(701, 294);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(80, 24);
            this.stopButton.TabIndex = 1;
            this.stopButton.Text = "Stop";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // portText
            // 
            this.portText.Location = new System.Drawing.Point(12, 26);
            this.portText.Name = "portText";
            this.portText.Size = new System.Drawing.Size(80, 20);
            this.portText.TabIndex = 2;
            this.portText.Text = "Auto";
            this.portText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.portText_KeyPress);
            // 
            // statusLabel
            // 
            this.statusLabel.Location = new System.Drawing.Point(12, 314);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(400, 24);
            this.statusLabel.TabIndex = 4;
            this.statusLabel.Text = "Type COM port to connect and press Connect";
            // 
            // realtimeHeartRateLabel
            // 
            this.realtimeHeartRateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.realtimeHeartRateLabel.Location = new System.Drawing.Point(876, 9);
            this.realtimeHeartRateLabel.Name = "realtimeHeartRateLabel";
            this.realtimeHeartRateLabel.Size = new System.Drawing.Size(50, 24);
            this.realtimeHeartRateLabel.TabIndex = 5;
            this.realtimeHeartRateLabel.Text = " ";
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
            this.averageHeartRateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.averageHeartRateLabel.Location = new System.Drawing.Point(876, 39);
            this.averageHeartRateLabel.Name = "averageHeartRateLabel";
            this.averageHeartRateLabel.Size = new System.Drawing.Size(50, 24);
            this.averageHeartRateLabel.TabIndex = 14;
            this.averageHeartRateLabel.Text = " ";
            // 
            // realtimeHeartRateLabelIndicator
            // 
            this.realtimeHeartRateLabelIndicator.AutoSize = true;
            this.realtimeHeartRateLabelIndicator.Location = new System.Drawing.Point(757, 14);
            this.realtimeHeartRateLabelIndicator.Name = "realtimeHeartRateLabelIndicator";
            this.realtimeHeartRateLabelIndicator.Size = new System.Drawing.Size(113, 13);
            this.realtimeHeartRateLabelIndicator.TabIndex = 15;
            this.realtimeHeartRateLabelIndicator.Text = "Real Time Heart Rate:";
            // 
            // averageHeartRateLabelIndicator
            // 
            this.averageHeartRateLabelIndicator.AutoSize = true;
            this.averageHeartRateLabelIndicator.Location = new System.Drawing.Point(765, 44);
            this.averageHeartRateLabelIndicator.Name = "averageHeartRateLabelIndicator";
            this.averageHeartRateLabelIndicator.Size = new System.Drawing.Size(105, 13);
            this.averageHeartRateLabelIndicator.TabIndex = 16;
            this.averageHeartRateLabelIndicator.Text = "Average Heart Rate:";
            // 
            // rawGraphPanel
            // 
            this.rawGraphPanel.Location = new System.Drawing.Point(0, 76);
            this.rawGraphPanel.Name = "rawGraphPanel";
            this.rawGraphPanel.samplingRate = 10;
            this.rawGraphPanel.Size = new System.Drawing.Size(938, 203);
            this.rawGraphPanel.TabIndex = 0;
            this.rawGraphPanel.xAxisMax = 0D;
            this.rawGraphPanel.xAxisMin = 0D;
            this.rawGraphPanel.yAxisMax = 0D;
            this.rawGraphPanel.yAxisMin = 0D;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(939, 337);
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


        /*Clear Button Clicked*/
        private void clearButton_Click(object sender, System.EventArgs e) {
            rawGraphPanel.LineGraph.Clear();

            timeStampIndex = 0;

            rawGraphPanel.LineGraph.Invalidate();
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
            dataLogOutFile = string.Concat(directoryToSave, dataLogOutFile);

            ECGLogOutFile = "ECGLog-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Hour.ToString() + "-"
            + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + ".txt";
            ECGLogOutFile = string.Concat(directoryToSave, ECGLogOutFile);

            //try making a new directory if it doesn't exist
            try {
                if(!Directory.Exists(directoryToSave)) {
                    Directory.CreateDirectory(directoryToSave);
                }
            } catch(Exception ex) {
                Console.WriteLine("caught exception at create directory: " + ex.Message);
            }

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

            realtimeHeartRateLabelIndicator.Location = new System.Drawing.Point(this.Width - 190, realtimeHeartRateLabelIndicator.Location.Y);
            realtimeHeartRateLabel.Location = new System.Drawing.Point(this.Width - 71, realtimeHeartRateLabel.Location.Y);

            averageHeartRateLabelIndicator.Location = new System.Drawing.Point(this.Width - 182, realtimeHeartRateLabelIndicator.Location.Y + 30);
            averageHeartRateLabel.Location = new System.Drawing.Point(this.Width - 71, realtimeHeartRateLabel.Location.Y + 30);

            statusLabel.Location = new System.Drawing.Point(statusLabel.Location.X, this.Height - 54);

            recordButton.Location = new System.Drawing.Point(this.Width - 247, this.Height - 73);
            stopButton.Location = new System.Drawing.Point(this.Width - 247, this.Height - 73);

            clearButton.Location = new System.Drawing.Point(this.Width - 117, this.Height - 73);

            rawGraphPanel.Location = new Point(rawGraphPanel.Location.X, connectButton.Location.Y + connectButton.Height + 18);
            rawGraphPanel.Height = (int)(recordButton.Location.Y - rawGraphPanel.Location.Y - 15);
            rawGraphPanel.Width = this.Width - 10;

            base.OnSizeChanged(e);
        }
    }
    /*End of MainForm*/
}
