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
            UpdateConnectButton(true);
            UpdateStatusLabel("Searching for MindSet...");

            mainForm.updateConnectButton(true);
            mainForm.updateStatusLabel("Searching for MindSet...");
            
            //Comment this line out if you want the splash screen to wait for good connection.
            UpdateVisibility(false);

            connector.RefreshAvailableConnections();
            
        }

        void OnDeviceNotFound(object sender, EventArgs e)
        {
            UpdateConnectButton(false);
            mainForm.updateConnectButton(false);
            mainForm.updateStatusLabel("Unable to connect.");
            UpdateStatusLabel("Unable to connect. Make sure the headset is turned on and paired.");
        }

        void OnDeviceFound(object sender, EventArgs e)
        {
            Connector.PortEventArgs de = (Connector.PortEventArgs)e;

            string tempPortName = de.PortName;
            UpdateStatusLabel("Device found on " + tempPortName + ". Connecting...");
            mainForm.updateStatusLabel("Device found on " + tempPortName + ". Connecting...");

            connector.Connect(tempPortName);

        }

        void OnDeviceValidating(object sender, EventArgs e)
        {
            Connector.ConnectionEventArgs ce = (Connector.ConnectionEventArgs)e;

            UpdateStatusLabel("Validating " + ce.Connection.PortName + ".");

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
            Parsed parsedData = new Parsed();

            MindSetParser mindSetParser = new MindSetParser();

            parsedData = mindSetParser.Read(de.DataRowArray);

            foreach (TimeStampData tsd in parsedData.Raw)
            {
                mainForm.rawGraphPanel.LineGraph.Add(new DataPair((mainForm.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), tsd.Value));
                mainForm.timeStampIndex++;
            }

            foreach (TimeStampData tsd in parsedData.Attention)
            {
                if (mainForm.attGraphPanel.LineGraph.data0.Count == 0) mainForm.attGraphPanel.LineGraph.Add(new DataPair(0, 0));
                mainForm.attGraphPanel.LineGraph.Add(new DataPair((mainForm.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), tsd.Value));
            }

            foreach (TimeStampData tsd in parsedData.Meditation)
            {
                if (mainForm.medGraphPanel.LineGraph.data0.Count == 0) mainForm.medGraphPanel.LineGraph.Add(new DataPair(0, 0));
                mainForm.medGraphPanel.LineGraph.Add(new DataPair((mainForm.timeStampIndex / (double)mainForm.rawGraphPanel.LineGraph.samplingRate), tsd.Value));
            }

            foreach (TimeStampData tsd in parsedData.PoorSignalQuality)
            {
                mainForm.updatePQLabel("PQ: " + tsd.Value);
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

        delegate void UpdateStatusLabelDelegate(string tempText);

        public void UpdateStatusLabel(string tempText)
        {
            if (this.InvokeRequired)
            {
                UpdateStatusLabelDelegate del = new UpdateStatusLabelDelegate(UpdateStatusLabel);
                this.Invoke(del, new object[] { tempText });
            }
            else
            {
                this.textBox2.Text = tempText;
            }
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
        

    }
}
