using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.IO.Ports;

using System.Windows.Forms;


namespace NeuroSky.ThinkGear
{

    public class Device: IEquatable<Device>
    {
        private DateTime UNIXSTARTTIME = new DateTime(1970, 1, 1, 0, 0, 0);

        public string PortName;
        public int HeadsetID;
        public double lastPacketReceived; //In milliseconds.

        public List<DataRow> DataRowList;

        public event EventHandler DataReceived = delegate { };

        private Timer watcherTimer;
        private DateTime startNoDataTime = new DateTime(1970, 1, 1, 0, 0, 0);
        public double TotalNoDataTime = 0; //In milliseconds

        public Device()
        {
            InitializeComponent();
        }

        public Device(string portName, int headsetId)
        {
            InitializeComponent();

            PortName   = portName;
            HeadsetID = headsetId;
            lastPacketReceived = 0;
        }

        private void InitializeComponent()
        {
            PortName = " ";
            HeadsetID = 0;
            lastPacketReceived = 0;

            startNoDataTime = DateTime.UtcNow;

            /*Setting up the timer for the max frame rate*/
            watcherTimer = new Timer();
            watcherTimer.Interval = 100; //In milliseconds
            watcherTimer.Tick += new EventHandler(watcherTimer_Tick);
            //watcherTimer.Start();

        }

        public void watcherTimer_Tick(object sender, EventArgs e)
        {
            TotalNoDataTime = (DateTime.UtcNow - startNoDataTime).TotalMilliseconds;
        }


        public void Deliver(DataRow[] dataRowArray)
        {
            //TODO: Make the DataReceived Event be triggered only when certain data is received.
            DataReceived(this, new DataEventArgs(dataRowArray));

            startNoDataTime = DateTime.UtcNow;
        }

        public bool Equals(Device other)
        {
            if (this.HeadsetID == other.HeadsetID && this.PortName == other.PortName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public class DataEventArgs : EventArgs
        {
            public DataRow[] DataRowArray;

            public DataEventArgs(DataRow[] dataRowArray)
            {
                this.DataRowArray = dataRowArray;
            }
        }

    }

}

