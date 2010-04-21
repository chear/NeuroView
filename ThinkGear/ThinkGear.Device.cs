using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.IO.Ports;


namespace NeuroSky.ThinkGear
{

    public class Device: IEquatable<Device>
    {
        public string PortName;
        public int HeadsetID;
        public double lastPacketReceived; //In milliseconds.

        public List<DataRow> DataRowList;

        public event EventHandler DataReceived = delegate { };

        public Device()
        {
            PortName = "";
            HeadsetID = 0;
            lastPacketReceived = 0;

        }

        public Device(string portName, int headsetId)
        {
            PortName   = portName;
            HeadsetID = headsetId;
            lastPacketReceived = 0;
        }

        public void Deliver(DataRow[] dataRowArray)
        {
            //TODO: Make the DataReceived Event be triggered only when certain data is received.
            DataReceived(this, new DataEventArgs(dataRowArray));
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

