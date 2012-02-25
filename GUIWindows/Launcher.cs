using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Text.RegularExpressions;

using NeuroSky.ThinkGear;
using NeuroSky.ThinkGear.Parser;

using NeuroSky.ThinkGear.Algorithms;

namespace NeuroSky.MindView {
    public partial class Launcher : Form {


        private string debugOutFile;
        private System.IO.StreamWriter debugStream;
        
        
        
        
        private Connector connector;

        MainForm mainForm;
        Device device;

        private byte[] bytesToSend;     //bytes to send for EGO
        private int rawCounter;     //counter for delay of EGO output
        private int delay;          //delay for lead on/lead off

        
        
        private double[] filt_coeff = new double[37] {-0.000361406858290222
,-0.0102963905383887
,-0.00974465467098966
,-0.0131901643978840
,-0.0159210938061222
,-0.0174394061276396
,-0.0170616802221640
,-0.0141921510529872
,-0.00839609561864024
,0.000513546985671733
,0.0123928768841560
,0.0267747773346622
,0.0428560196628620
,0.0595742449913909
,0.0756798102482777
,0.0898999203474164
,0.101042683657107
,0.108152684780980
,0.110593117284196
,0.108152684780980
,0.101042683657107
,0.0898999203474164
,0.0756798102482777
,0.0595742449913909
,0.0428560196628620
,0.0267747773346622
,0.0123928768841560
,0.000513546985671733
,-0.00839609561864024
,-0.0141921510529872
,-0.0170616802221640
,-0.0174394061276396
,-0.0159210938061222
,-0.0131901643978840
,-0.00974465467098966
,-0.0102963905383887
,-0.000361406858290222};

        /*

        private double[] filt_coeff = new double[37] {-0.0984735459831233
,-0.00828522750775057
,-0.00697077950439262
,-0.00573459401060683
,-0.00625762021468554
,-0.0101510197638870
,-0.0176380595801390
,-0.0280336010681272
,-0.0388458665735298
,-0.0467295191965702
,-0.0479543970396153
,-0.0395351235900068
,-0.0201408570963116
,0.00926645436943145
,0.0453614614743012
,0.0828753377498285
,0.115717558856825
,0.138129363346965
,0.146099433918104
,0.138129363346965
,0.115717558856825
,0.0828753377498285
,0.0453614614743012
,0.00926645436943145
,-0.0201408570963116
,-0.0395351235900068
,-0.0479543970396153
,-0.0467295191965702
,-0.0388458665735298
,-0.0280336010681272
,-0.0176380595801390
,-0.0101510197638870
,-0.00625762021468554
,-0.00573459401060683
,-0.00697077950439262
,-0.00828522750775057
,-0.0984735459831233};
*/











        private int BUFFER_SIZE = 37;

        //hold the filtered data
        private double filtered; 

        //hold the raw data in a continuously updating buffer
        private double[] eegBuffer; 
        private double[] tempeegBuffer;

        //counter to track when we should plot
        private int bufferCounter;

        public Launcher() {


            debugOutFile = "debug.txt";
            this.debugStream = new System.IO.StreamWriter(debugOutFile, true);

            
            
            
            
            
            mainForm = new MainForm();

            connector = new Connector();
            connector.DeviceConnected += new EventHandler(OnDeviceConnected);
            connector.DeviceFound += new EventHandler(OnDeviceFound);
            connector.DeviceNotFound += new EventHandler(OnDeviceNotFound);
            connector.DeviceConnectFail += new EventHandler(OnDeviceNotFound);
            connector.DeviceDisconnected += new EventHandler(OnDeviceDisconnected);
            connector.DeviceValidating += new EventHandler(OnDeviceValidating);

            mainForm.ConnectButtonClicked += new EventHandler(OnConnectButtonClicked);
            mainForm.DisconnectButtonClicked += new EventHandler(OnDisconnectButtonClicked);
            mainForm.Disposed += new EventHandler(OnMainFormDisposed);

            InitializeComponent();

            this.MaximumSize = new Size(391, 361);
            this.MinimumSize = this.MaximumSize;

            rawCounter = 0;     //initially zero
            delay = 512 * 5;    //5 seconds delay

            bufferCounter = 0;
            
            //hold the raw data in a continuously updating buffer
            eegBuffer = new double[BUFFER_SIZE];
            tempeegBuffer = new double[BUFFER_SIZE];
            
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.Run(new Launcher());

        }

        private void button1_Click(object sender, EventArgs e) {
            //UpdateConnectButton(true);
            //UpdateStatusLabel("Searching for MindSet...");

            //mainForm.updateConnectButton(true);
            //mainForm.updateStatusLabel("Searching for MindSet...");

            //Comment this line out if you want the splash screen to wait for good connection.
            UpdateVisibility(false);

            //connector.RefreshAvailableConnections();

        }

        void OnDeviceNotFound(object sender, EventArgs e) {
            UpdateConnectButton(false);
            mainForm.updateConnectButton(false);
            mainForm.updateStatusLabel("Unable to connect.");
        }

        void OnDeviceFound(object sender, EventArgs e) {
            Connector.PortEventArgs de = (Connector.PortEventArgs)e;

            string tempPortName = de.PortName;
            mainForm.updateStatusLabel("Device found on " + tempPortName + ". Connecting...");

            connector.Connect(tempPortName);

        }

        void OnDeviceValidating(object sender, EventArgs e) {
            Connector.ConnectionEventArgs ce = (Connector.ConnectionEventArgs)e;

            mainForm.updateStatusLabel("Validating " + ce.Connection.PortName + ".");
        }

        void OnDeviceConnected(object sender, EventArgs e) {
            Connector.DeviceEventArgs de = (Connector.DeviceEventArgs)e;

            //save the device
            device = de.Device;

            mainForm.updateStatusLabel("Connected to a device on " + de.Device.PortName + ".");

            de.Device.DataReceived += new EventHandler(OnDataReceived);
            mainForm.updateConnectButton(true);

            UpdateVisibility(false);
            Console.WriteLine("Done");
        }

        void OnDeviceDisconnected(object sender, EventArgs e) {
            Connector.DeviceEventArgs de = (Connector.DeviceEventArgs)e;

            mainForm.updateStatusLabel("Disconnected from a device on " + de.Device.PortName + ".");

            mainForm.updateConnectButton(false);

        }


        void OnDataReceived(object sender, EventArgs e) {
            Device d = (Device)sender;
            Device.DataEventArgs de = (Device.DataEventArgs)e;

            ThinkGear.DataRow[] tempDataRowArray = de.DataRowArray;

            TGParser thinkGearParser = new TGParser();
            thinkGearParser.Read(de.DataRowArray);

            // Pass off data for recording
            if(mainForm.recordFlag == true) {
                //save the datalog.txt
                mainForm.recordData(de.DataRowArray);
            }


            /* Loop through new parsed data */
            for(int i = 0; i < thinkGearParser.ParsedData.Length; i++) {
                //send the configuration bytes to the chip. this happens immediately and only once
                if(thinkGearParser.ParsedData[i].ContainsKey("BMDConfig")) {
                    if(bytesToSend == null) {
                        bytesToSend = new byte[8] { 0xAA, 0xAA, 0x04, 0x03, 0x40, 0xF9, 0x00, (byte)thinkGearParser.ParsedData[i]["BMDConfig"] };
                        connector.Send(device.PortName, bytesToSend);
                    }
                }

                //save the poorsignal value. this is always updated
                if(thinkGearParser.ParsedData[i].ContainsKey("PoorSignal")) {
                    mainForm.poorQuality = thinkGearParser.ParsedData[i]["PoorSignal"];
                }


                if(thinkGearParser.ParsedData[i].ContainsKey("Raw")) {

                    //if signal is good
                    if(mainForm.poorQuality == 200) {
                        rawCounter++;

                        //update the buffer with the latest eeg value
                        Array.Copy(eegBuffer, 1, tempeegBuffer, 0, BUFFER_SIZE - 1);
                        tempeegBuffer[BUFFER_SIZE - 1] = (double)thinkGearParser.ParsedData[i]["Raw"];
                        Array.Copy(tempeegBuffer, eegBuffer, BUFFER_SIZE);
                        bufferCounter++;

                        //if "delay" seconds have passed
                        if((rawCounter >= delay) && (bufferCounter >= BUFFER_SIZE)) {

                            //filter the data
                            filtered = applyFilter(eegBuffer);
                            mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), filtered));
                            mainForm.rawGraphPanel.LineGraph.timeStampIndex++;

                            //reset the counter
                            bufferCounter = BUFFER_SIZE - 1;
                        } else {
                            //just plot zero
                            mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), 0));
                            mainForm.rawGraphPanel.LineGraph.timeStampIndex++;
                        }

                    } else {
                        //otherwise signal is bad, plot zero. reset counter
                        rawCounter = 0;
                        bufferCounter = 0;
                        Array.Clear(eegBuffer, 0, eegBuffer.Length);

                        mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), 0));
                        mainForm.rawGraphPanel.LineGraph.timeStampIndex++;
                    }
                }


                if(thinkGearParser.ParsedData[i].ContainsKey("HeartRate")) {
                    //if the "delay" number of seconds have passed, pass the heartrate value
                    if(rawCounter >= delay) {
                        mainForm.ASICHBValue = thinkGearParser.ParsedData[i]["HeartRate"];
                        mainForm.updateAverageHeartBeatValue(thinkGearParser.ParsedData[i]["HeartRate"]);
                        mainForm.updateRealTimeHeartBeatValue(thinkGearParser.ParsedData[i]["HeartRate"]);
                    }
                        //otherwise just pass a value of 0 to make it think its poor signal
                    else {
                        mainForm.updateAverageHeartBeatValue(0);
                        mainForm.updateRealTimeHeartBeatValue(0);

                        //but still pass the correct heartbeat value for ecglog.txt
                        mainForm.ASICHBValue = thinkGearParser.ParsedData[i]["HeartRate"];
                    }
                }

                /* End "Check for the data flag for each panel..." */
            }
        }

        void OnConnectButtonClicked(object sender, EventArgs e) {
            string portName = mainForm.portText.Text.ToUpper();
            portName = portName.Trim();

            if(portName == "AUTO") {
                connector.RefreshAvailableConnections();
                mainForm.updateStatusLabel("Searching for MindSet...");
                return;
            }

            int portNumber = 0;

            try {
                portNumber = Convert.ToInt16(portName);
            } catch(FormatException fe) {
                Console.WriteLine(fe.Message);
            }

            if(portNumber > 0) {
                portName = "COM" + portNumber;
            }

            Regex r = new Regex("COM[1-9][0-9]*");
            portName = r.Match(portName).ToString();
            Console.WriteLine("Connecting to xx" + portName + "xx");

            if(portName != "") {
                connector.Connect(portName);
                mainForm.updateStatusLabel("Connecting to " + portName);
                return;
            }

            MessageBox.Show("You must enter a valid COM port name. (Ex. COM1 or 1)\nYou may also type in 'Auto' for auto-connection.", "Invalid Input Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //mainForm.updateConnectButton(false);
            mainForm.updateConnectButton(false);
            return;

        }

        void OnDisconnectButtonClicked(object sender, EventArgs e) {
            connector.Disconnect();
            mainForm.updateConnectButton(false);

            //make the byteToSend null so it will be resent when pressing connect
            bytesToSend = null;
        }


        delegate void UpdateVisibilityDelegate(bool enable);
        public void UpdateVisibility(bool enable) {
            if(this.InvokeRequired) {
                UpdateVisibilityDelegate del = new UpdateVisibilityDelegate(UpdateVisibility);
                this.Invoke(del, new object[] { enable });
            } else {
                if(enable) {
                    if(!this.Visible) {
                        this.Show();
                        mainForm.Hide();
                    }
                } else {
                    if(this.Visible) {
                        this.Hide();
                        mainForm.Show();
                    }
                }
            }
        }


        delegate void UpdateConnectButtonDelegate(bool connected);
        public void UpdateConnectButton(bool connected) {
            if(this.InvokeRequired) {
                UpdateConnectButtonDelegate del = new UpdateConnectButtonDelegate(UpdateConnectButton);
                this.Invoke(del, new object[] { connected });
            } else {
                if(connected) {
                    this.button1.Enabled = false;
                } else {

                    this.button1.Enabled = true;
                }

            }

        }

        void OnMainFormDisposed(object sender, EventArgs e) {
            this.Dispose();
        }

        private void Launcher_Load(object sender, EventArgs e) {

        }


        //apply filter based on multiply add technique
        private double applyFilter(double[] data) {
            int length = data.Length;
            double result = new double();

            for(int i = 0; i < length; i++) {
                result += data[i] * filt_coeff[i];
            }
            return result;
        }
    }
}
