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

        MainForm mainForm;

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
                // Check for the data flag for each panel
                if (thinkGearParser.ParsedData[i].ContainsKey("Raw"))
                {
                    mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.rawGraphPanel.LineGraph.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), thinkGearParser.ParsedData[i]["Raw"]));

                    // Incremenet timer
                    mainForm.rawGraphPanel.LineGraph.timeStampIndex++;
                }

                //save the poorsignal value
                if (thinkGearParser.ParsedData[i].ContainsKey("PoorSignal"))
                {
                    mainForm.poorQuality = thinkGearParser.ParsedData[i]["PoorSignal"];
                }

                if (thinkGearParser.ParsedData[i].ContainsKey("HeartRate"))
                {
                    mainForm.updateAverageHeartBeatValue(thinkGearParser.ParsedData[i]["HeartRate"]);

                    if (mainForm.poorQuality == 200)
                    {
                        mainForm.updateRealTimeHeartRateLabel(thinkGearParser.ParsedData[i]["HeartRate"].ToString());
                    }
                    else
                    {
                        //ZERO means poor signal
                        mainForm.updateRealTimeHeartRateLabel("0");
                        mainForm.updateAverageHeartRateLabel("0");
                        mainForm.poorQuality = thinkGearParser.ParsedData[i]["HeartRate"];
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
