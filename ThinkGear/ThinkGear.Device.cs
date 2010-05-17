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
        
        public List<DataRow> DataRowList;

        public event EventHandler DataReceived = delegate { };
        public DateTime lastUpdate; //In milliseconds.
        public double DataReceivedRate = 20; //In milliseconds

        public Device()
        {
            InitializeComponent();
        }

        public Device(string portName, int headsetId)
        {
            InitializeComponent();

            PortName   = portName;
            HeadsetID = headsetId;
            lastUpdate = DateTime.UtcNow;
        }

        private void InitializeComponent()
        {
            PortName = " ";
            HeadsetID = 0;

            DataRowList = new List<DataRow>();
            
            lastUpdate = DateTime.UtcNow;

        }


        public void Deliver(DataRow[] dataRowArray)
        {
            foreach (DataRow d in dataRowArray)
            {
                DataRowList.Add(d);
            }


            if ((DateTime.UtcNow - lastUpdate).TotalMilliseconds > DataReceivedRate)
            {
                //TODO: Make the DataReceived Event be triggered only when certain data is received.
                DataReceived(this, new DataEventArgs(DataRowList.ToArray()));

                DataRowList.Clear();

                lastUpdate = DateTime.UtcNow;
            }

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

