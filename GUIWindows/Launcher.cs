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

namespace NeuroSky.MindView
{
    public partial class Launcher : Form
    {
        private Connector connector;

        MainForm    mainForm;
        Device      device;

        private byte[]  byteToSend;     //byte to send for EGO
        private int     rawCounter;     //counter for delay of EGO output
        private int     delay;          //delay for lead on/lead off

        public Launcher()
        {
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

            this.MaximumSize = new Size(383, 327);
            this.MinimumSize = this.MaximumSize;

            rawCounter = 0;     //initially zero
            delay = 512 * 5;    //5 seconds delay
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new Launcher());
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //UpdateConnectButton(true);
            //UpdateStatusLabel("Searching for MindSet...");

            //mainForm.updateConnectButton(true);
            //mainForm.updateStatusLabel("Searching for MindSet...");
            
            //Comment this line out if you want the splash screen to wait for good connection.
            UpdateVisibility(false);

            //connector.RefreshAvailableConnections();
            
        }

        void OnDeviceNotFound(object sender, EventArgs e)
        {
            UpdateConnectButton(false);
            mainForm.updateConnectButton(false);
            mainForm.updateStatusLabel("Unable to connect.");
        }

        void OnDeviceFound(object sender, EventArgs e)
        {
            Connector.PortEventArgs de = (Connector.PortEventArgs)e;

            string tempPortName = de.PortName;
            mainForm.updateStatusLabel("Device found on " + tempPortName + ". Connecting...");

            connector.Connect(tempPortName);

        }

        void OnDeviceValidating(object sender, EventArgs e)
        {
            Connector.ConnectionEventArgs ce = (Connector.ConnectionEventArgs)e;

            mainForm.updateStatusLabel("Validating " + ce.Connection.PortName + ".");
        }

        void OnDeviceConnected(object sender, EventArgs e)
        {
            Connector.DeviceEventArgs de = (Connector.DeviceEventArgs)e;

            //save the device
            device = de.Device;

            mainForm.updateStatusLabel("Connected to a headset on " + de.Device.PortName + ".");

            de.Device.DataReceived += new EventHandler(OnDataReceived);
            mainForm.updateConnectButton(true);

            UpdateVisibility(false);
            Console.WriteLine("Done");
        }

        void OnDeviceDisconnected(object sender, EventArgs e)
        {
            Connector.DeviceEventArgs de = (Connector.DeviceEventArgs)e;

            mainForm.updateStatusLabel("Disconnected from a headset on " + de.Device.PortName + ".");

            mainForm.updateConnectButton(false);

        }


        void OnDataReceived(object sender, EventArgs e)
        {
            Device d = (Device)sender;
            Device.DataEventArgs de = (Device.DataEventArgs)e;

            ThinkGear.DataRow[] tempDataRowArray = de.DataRowArray;

            TGParser thinkGearParser = new TGParser();
            thinkGearParser.Read(de.DataRowArray);

            // Pass off data for recording
            if (mainForm.recordFlag == true)
            {
                //save the datalog.txt
                mainForm.recordData(de.DataRowArray);
            }
            
            
            /* Loop through new parsed data */
            for (int i = 0; i < thinkGearParser.ParsedData.Length; i++)
            {
                //send the configuration bytes to the chip. this happens immediately and only once
                if(thinkGearParser.ParsedData[i].ContainsKey("EGODebug2"))
                {
                    if(byteToSend == null)
                    {
                        byteToSend = new byte[8] { 0xAA, 0xAA, 0x04, 0x03, 0x40, 0xF9, 0x00, (byte)thinkGearParser.ParsedData[i]["EGODebug2"] };
                        connector.Send(device.PortName, byteToSend);
                    }
                }

                //save the poorsignal value. this is always updated
                if(thinkGearParser.ParsedData[i].ContainsKey("PoorSignal"))
                {
                    mainForm.poorQuality = thinkGearParser.ParsedData[i]["PoorSignal"];
                }


                if(thinkGearParser.ParsedData[i].ContainsKey("Raw"))
                {
                    //if signal is good
                    if(mainForm.poorQuality == 200)
                    {
                        rawCounter++;

                        //if "delay" seconds have passed, plot the data itself
                        if(rawCounter >= delay)
                        {
                            mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), thinkGearParser.ParsedData[i]["Raw"]));
                        } 
                        //otherwise plot zero (flatline)
                        else
                        {
                            mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), 0));
                        }

                        // Incremenet timer
                        mainForm.rawGraphPanel.LineGraph.timeStampIndex++;


                    } 
                    //otherwise signal is bad, plot zero. reset counter
                    else
                    {
                        rawCounter = 0;

                        mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), 0));
                        mainForm.rawGraphPanel.LineGraph.timeStampIndex++;
                    }
                }
                
                
                
                
                
                /*
                
                //look for raw data. if enough raw data has been accumulated, start parsing stuff
                //otherwise just throw away the data
                if(thinkGearParser.ParsedData[i].ContainsKey("Raw"))
                {
                    //if "delay" seconds have passed since lead on, start plotting stuff
                    if(rawCounter >= delay)
                    {
                        //send data to be graphed to the rawGraphPanel.
                        //if the poorQuality value is 200, send the actual data
                        if(mainForm.poorQuality == 200)
                        {
                            mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), thinkGearParser.ParsedData[i]["Raw"]));
                        }

                        //if poorquality is 0, that means it's lead off. just plot zero, then reset the counter and wait another "delay" number of seconds
                        else
                        {
                            mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), 0));
                            rawCounter = 0;
                        }

                        // Incremenet timer
                        mainForm.rawGraphPanel.LineGraph.timeStampIndex++;

                    } 
                    //otherwise, just send flatline (zero) and increment timer
                    else
                    {
                        mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), 0));
                        mainForm.rawGraphPanel.LineGraph.timeStampIndex++;

                        rawCounter = 0;
                    }
                }
                */
                


                if(thinkGearParser.ParsedData[i].ContainsKey("HeartRate"))
                {
                    //if the "delay" number of seconds have passed, pass the heartrate value
                    if(rawCounter >= delay)
                    {
                        mainForm.ASICHBValue = thinkGearParser.ParsedData[i]["HeartRate"];
                        mainForm.updateAverageHeartBeatValue(thinkGearParser.ParsedData[i]["HeartRate"]);
                        mainForm.updateRealTimeHeartBeatValue(thinkGearParser.ParsedData[i]["HeartRate"]);
                    } 
                    //otherwise just pass a value of 0 to make it think its poor signal
                    else
                    {
                        mainForm.updateAverageHeartBeatValue(0);
                        mainForm.updateRealTimeHeartBeatValue(0);

                        //but still pass the correct heartbeat value for ecglog.txt
                        mainForm.ASICHBValue = thinkGearParser.ParsedData[i]["HeartRate"];
                    }
                }

                /* End "Check for the data flag for each panel..." */            
            }
        }

        void OnConnectButtonClicked(object sender, EventArgs e)
        {
            string portName = mainForm.portText.Text.ToUpper();
            portName = portName.Trim();

            if (portName == "AUTO")
            {
                connector.RefreshAvailableConnections();
                mainForm.updateStatusLabel("Searching for MindSet...");
                return;
            }

            int portNumber = 0;

            try
            {
                portNumber = Convert.ToInt16(portName);
            }
            catch (FormatException fe)
            {
                Console.WriteLine(fe.Message);
            }

            if (portNumber > 0)
            {
                portName = "COM" + portNumber;
            }

            Regex r = new Regex("COM[1-9][0-9]*");
            portName = r.Match(portName).ToString();
            Console.WriteLine("Connecting to xx" + portName + "xx");

            if (portName != "")
            {
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

        void OnDisconnectButtonClicked(object sender, EventArgs e)
        {
            connector.Disconnect();
            mainForm.updateConnectButton(false);
        }

         
        delegate void UpdateVisibilityDelegate(bool enable);
        public void UpdateVisibility(bool enable)
        {
            if (this.InvokeRequired)
            {
                UpdateVisibilityDelegate del = new UpdateVisibilityDelegate(UpdateVisibility);
                this.Invoke(del, new object[] { enable });
            }
            else
            {
                if (enable)
                {
                    if (!this.Visible)
                    {
                        this.Show();
                        mainForm.Hide();
                    }
                }
                else
                {
                    if (this.Visible)
                    {
                        this.Hide();
                        mainForm.Show();
                    }
                }
            }
        }


        delegate void UpdateConnectButtonDelegate(bool connected);
        public void UpdateConnectButton(bool connected)
        {
            if (this.InvokeRequired)
            {
                UpdateConnectButtonDelegate del = new UpdateConnectButtonDelegate(UpdateConnectButton);
                this.Invoke(del, new object[] { connected });
            }
            else
            {
                if (connected)
                {
                    this.button1.Enabled = false;
                }
                else
                {

                    this.button1.Enabled = true;
                }

            }

        }

        void OnMainFormDisposed(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Launcher_Load(object sender, EventArgs e) {

        }
        

    }
}
