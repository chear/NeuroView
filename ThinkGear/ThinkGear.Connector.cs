using System;
using System.Collections.Generic;

using System.Text;
using System.Text.RegularExpressions;

using System.IO;
using System.IO.Ports;

using System.Threading;

namespace NeuroSky.ThinkGear
{

    public struct DataRow
    {
        public double Time;
        public byte Type;
        public byte[] Data;

    }

    public struct Packet
    {
        public Type DeviceType;
        public string PortName;
        public int HeadsetID;
        public DataRow[] DataRowArray;
    }

    public enum DataType
    {
        Battery,
        PoorSignal,
        Attention,
        Meditation,
        Raw,
        Delta,
        Theta,
        Alpha1,
        Alpha2,
        Beta1,
        Beta2,
        Gamma1,
        Gamma2,
        HeadsetID
    };

    public enum Code : byte
    {
        Battery = 0x01,
        PoorSignal = 0x02,
        Attention = 0x04,
        Meditation = 0x05,
        Raw = 0x80,
        EEGPowerFloat = 0x81,
        EEGPowerInt = 0x83,
        HeadsetID = 0x7F,
    };

    // The main controller that connects the connections to a specific device.  This is the controller who initiates the read.
    public class Connector
    {
        public List<Connector.Connection> mindSetPorts;

        // devicelist refresh events
        public event EventHandler DeviceFound = delegate { };
        public event EventHandler DeviceNotFound = delegate { };
        public event EventHandler DeviceValidating = delegate { };

        // device connection events
        public event EventHandler DeviceConnected = delegate { };
        public event EventHandler DeviceConnectFail = delegate { };
        public event EventHandler DeviceDisconnected = delegate { };

        private List<string> availablePorts;
        private List<Connector.Connection> portsToConnect;

        private List<Connection> activePortsList;
        private List<Connection> removePortsList;
        private List<Device> deviceList;

        private int defaultBaudRate;

        private Thread findThread;
        private Thread readThread;
        private Thread addThread;
        private Thread removeThread;

        private bool ReadThreadEnable = true;
        private bool RemoveThreadEnable = true;
        public bool ScanConnectEnable = true;

        private const int REMOVE_PORT_TIMER = 1000; //In milliseconds

        public Connector()
        {

            availablePorts = new List<string>();
            mindSetPorts = new List<Connection>();
            portsToConnect = new List<Connection>();

            activePortsList = new List<Connection>();
            removePortsList = new List<Connection>();
            deviceList = new List<Device>();

            findThread = new Thread(FindThread);
            readThread = new Thread(ReadThread);
            removeThread = new Thread(RemoveThread);

            readThread.Priority = ThreadPriority.Highest;
            removeThread.Priority = ThreadPriority.Lowest;

            defaultBaudRate = 57600;

            readThread.Start();
            removeThread.Start();

        }

        public void Connect(string portName)
        {
            Connection tempConnection = new Connection(portName);

            lock (portsToConnect)
            {
                portsToConnect.Add(tempConnection);
            }

            if (!addThread.IsAlive)
            {
                addThread = new Thread(AddThread);
                addThread.Start();
            }
            if (!readThread.IsAlive) readThread.Start();
        }

        public void Close()
        {
            this.Disconnect();

            ReadThreadEnable = false;
            RemoveThreadEnable = false;

            Thread.Sleep(50);

            readThread.Abort();
            findThread.Abort();
            addThread.Abort();
            removeThread.Abort();
        }

        public void Disconnect()
        {
            lock (activePortsList)
            {
                foreach (Connection c in activePortsList)
                {
                    c.Close();
                }

                activePortsList.Clear();
                deviceList.Clear();
            }
        }


        public void Find()
        {
            defaultBaudRate = 57600;

            if (!findThread.IsAlive)
            {
                findThread = new Thread(FindThread);
                findThread.Start();
            }
        }

        public bool FindThreadIsAlive()
        {
            return findThread.IsAlive;
        }

        public void FindAvailablePorts()
        {
            string[] temp = SerialPort.GetPortNames();

            Regex r = new Regex("COM[1-9][0-9]*");

            Console.Write("Available ports on this computer is before rmatch: ");
            foreach(string portName in temp)
            {
                Console.Write(portName + " ");
            }
            Console.Write("\n");
#if false
            
            foreach (string portName in temp)
            {
                availablePorts.Add(r.Match(portName).ToString());
            }
#endif

            availablePorts.Clear();
            for (int i = 1; i < 100; i++)
            {
                availablePorts.Add("COM"+i);
            }

            Console.Write("Available ports on this computer is: ");
            foreach (string port in availablePorts)
            {
                Console.Write(port + " ");
            }
            Console.Write("\n");

        }

        private void FindThread()
        {
            FindAvailablePorts();

            Connection tempPort;
            lock (mindSetPorts)
            {
                mindSetPorts.Clear();
                foreach (string portName in availablePorts)
                {
                    tempPort = new Connection();

                    tempPort.PortName = portName;
                    tempPort.BaudRate = defaultBaudRate;

                    Console.WriteLine("Trying " + portName);
                    DeviceValidating(this, new ConnectionEventArgs(tempPort));

                    try
                    {
                        tempPort.Open();
                        Thread.Sleep(100);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("tempPort.Open Exception: " + e.Message);
                    }

                    if (tempPort.IsOpen)
                    {
                        Console.WriteLine("Openned and going to read packets from " + portName);

                        Packet returnPacket = tempPort.ReadPacket();

                        Console.WriteLine("Received " + returnPacket.DataRowArray.Length + " DataRows at FindThread.");

                        for (int i = 0; i < returnPacket.DataRowArray.Length; i++)
                        {
                            Console.Write(returnPacket.DataRowArray[i].Time + " : ["
                                       + (Code)(returnPacket.DataRowArray[i].Type) + "] : ");

                            for (int t = 0; t < returnPacket.DataRowArray[i].Data.Length; t++)
                            {
                                Console.Write("0x" + returnPacket.DataRowArray[i].Data[t].ToString("X2") + " ");
                            }

                            Console.Write("\n");
                        }

                        if (returnPacket.DataRowArray.Length > 0)
                        {

                            Console.WriteLine("Adding " + tempPort.PortName + " to mindSetPorts.");
                            mindSetPorts.Add(tempPort);

                            //Connects to the First MindSet it found.
                            if (ScanConnectEnable)
                            {
                                Console.WriteLine(tempPort.PortName + " validated in FindThread.");
                                lock (activePortsList) activePortsList.Add(tempPort);
                                return;
                            }
                        }

                        tempPort.Close();
                    }
                    else
                    {
                        Console.WriteLine(tempPort.PortName + " is not opened.");
                    }
                }

                Thread.Sleep(1000);

                if (mindSetPorts.Count > 0) 
                    DeviceFound(this, EventArgs.Empty);
                else 
                    DeviceNotFound(this, EventArgs.Empty);
            }
        }

        private void ReadThread()
        {
            //Console.WriteLine("Starting ReadThread. " + (DateTime.UtcNow - startTime).TotalSeconds);

            bool allReturnNull = true;

            //Exits if readThreadEnable is false. 
            while (ReadThreadEnable)
            {
                lock (activePortsList)
                {
                    foreach (Connection port in activePortsList)
                    {
                        Packet returnPacket = port.ReadPacket();

                        /* Checks if it received any packet from any of the connections.*/
                        if (returnPacket.DataRowArray.Length > 0) allReturnNull = false;

                        //debugFile.WriteLine("Length: " + returnPacket.DataRowArray.Length + " Right after the ReadPacket. " + (DateTime.UtcNow - startTime).TotalSeconds);

                        /*Pass the data to the devices.*/
                        lock (deviceList)
                        {
                            DeliverPacket(returnPacket);
                        }
             
                        //Check the TotalTimeout and add to the remove list if is not receiving
                        if (port.TotalTimeoutTime > 1)
                        {
                            lock (removePortsList)
                            {
                                removePortsList.Add(port);
                            }
                        }
                    }

                }

                //Sleep when DataRow null.
                //TODO: Make a dynamic sleep thread.
                if (allReturnNull)
                {
                    Thread.Sleep(2);
                    
                }

                allReturnNull = true;
                
            }

        }

        private void RemoveThread()
        {

            while (RemoveThreadEnable)
            {
                //Console.WriteLine("Searching for bad ports..............");
                lock (activePortsList)
                {
                    lock (removePortsList)
                    {
                        foreach (Connection port in removePortsList)
                        {
                            if (port.IsOpen)
                            {
                                try
                                {
                                    port.Close();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("RemoveThread: " + e.Message);
                                }
                            }
                            activePortsList.Remove(port);
                            Console.WriteLine(port.PortName + " Disconnected and removed.");

                            Device tempDevice = new Device();

                            lock (deviceList)
                            {
                                /*TODO: Currently it only finds one headset that is connected to that port.
                                        Need to change to get multiple headset.*/

                                /*Finds the index for the device that was connected to the port*/
                                int index = deviceList.FindIndex(f => (f.PortName == port.PortName));

                                /*Removes the Device from the list*/
                                if (index >= 0)
                                {
                                    tempDevice = deviceList[index];
                                    deviceList.RemoveAt(index);
                                    Console.WriteLine("Removed device at index: " + index);
                                }
                                
                            }

                            DeviceDisconnected(this, new DeviceEventArgs(tempDevice));

                        }
                        removePortsList.Clear();
                    }

                }

                Thread.Sleep(500);
            }
        }

        private void AddThread()
        {

            Console.Write("Connecting to the following port: ");
            lock (portsToConnect)
            {
                foreach (Connection port in portsToConnect)
                {
                    Console.Write(port.PortName + " ");
                }

                Console.Write("\n");

                foreach (Connection tempPort in portsToConnect)
                {
                    if (tempPort.IsOpen) break;

                    //Connect if it was opened before.
                    try
                    {
                        tempPort.Open();
                        Thread.Sleep(100);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("tempPort.Open Exception: " + e.Message);
                    }

                    Packet returnPacket = tempPort.ReadPacket();

                    //If it can read valid packets add to activePortList
                    if (returnPacket.DataRowArray.Length > 0)
                    {
                        Console.WriteLine(tempPort.PortName + " validated in AddThread.");
                        lock (activePortsList) activePortsList.Add(tempPort);
                    }
                    else
                    {
                        tempPort.Close();
                    }
                }

                portsToConnect.Clear();
            }

            lock (activePortsList)
            {
                Console.WriteLine("Number of Active Ports: " + activePortsList.Count);
                if (activePortsList.Count < 1)
                {
                    DeviceConnectFail(this, EventArgs.Empty);
                }
            }

            Console.WriteLine("Finished Add Thread.");

            Thread.Sleep(1000);

        }

        //Delivers a packet to a device
        private void DeliverPacket(Packet packet)
        {
            Device tempDevice = new Device(packet.PortName, packet.HeadsetID);

            /*Searches the device list and see the device is listed*/
            if (!deviceList.Contains(tempDevice))
            {
                deviceList.Add(tempDevice);

                DeviceConnected(this, new DeviceEventArgs(tempDevice));

            }

            /*Finds the index for the device*/
            int index = deviceList.FindIndex(f => (f.PortName == tempDevice.PortName) && (f.HeadsetID == tempDevice.HeadsetID));

            if (index < 0)
            {
                Console.WriteLine("Index not found:" + index);
                Console.WriteLine("MindSetList.Count: " + deviceList.Count);
                return;
            }

            (deviceList[index]).Deliver(packet.DataRowArray);


        }/*End of DeliverPacket*/


        public class DeviceEventArgs : EventArgs
        {
            public Device Device;

            public DeviceEventArgs(Device device)
            {
                this.Device = device;
            }
        }

        public class ConnectionEventArgs : EventArgs
        {
            public Connection Connection;

            public ConnectionEventArgs(Connection connection)
            {
                this.Connection = connection;
            }
        }


        public class Connection : SerialPort
        {
            private DateTime UNIXSTARTTIME = new DateTime(1970, 1, 1, 0, 0, 0);

            private const int SERIALPORT_READ_TIMEOUT = 50; //in milliseconds
            private const int READ_PACKET_TIMEOUT = 1;      // in seconds

            private const byte SYNC_BYTE = 0xAA;
            private const byte EXCODE_BYTE = 0x55;
            private const byte MULTI_BYTE = 0x80;

            public DateTime StartTimeoutTime = new DateTime(1970, 1, 1, 0, 0, 0);
            public double TotalTimeoutTime = 0; //In milliseconds

            public enum ParserState
            {
                Invalid = 0,
                Sync0 = 1,
                Sync1 = 2,
                PayloadLength = 3,
                Payload = 4,
                Checksum = 5
            };

            public Byte[] parserBuffer = new Byte[0];

            public Connection()
            {
                PortName = " ";
                parserBuffer = new Byte[0];
                this.BaudRate = 57600;
                this.ReadTimeout = SERIALPORT_READ_TIMEOUT;
            }

            public Connection(String portName)
            {
                parserBuffer = new Byte[0];
                this.BaudRate = 57600;
                this.ReadTimeout = SERIALPORT_READ_TIMEOUT;

                this.PortName = portName;
                
            }

            public Packet ReadPacket()
            {
                byte[] tempByte = new byte[1];
                List<byte> receivedBytes = new List<byte>();
                List<DataRow> receivedDataRow = new List<DataRow>();

                int state = (int)ParserState.Sync0;
                int payloadLength = 0;
                int payloadSum = 0;
                int checkSum = 0;
                List<byte> payload = new List<byte>();
                DataRow[] tempDataRowArray = null;

                int bufferIterator = 0;

                DateTime readPacketTimeOut = DateTime.Now.AddSeconds(READ_PACKET_TIMEOUT);

                while (DateTime.Now < readPacketTimeOut && tempDataRowArray == null)
                {
                    try
                    {
                        if (parserBuffer.Length > bufferIterator)
                        {
                            tempByte[0] = parserBuffer[bufferIterator++];
                        }
                        else
                        {
                            Read(tempByte, 0, 1);
                        }
#if false
                        if (parserBuffer.Length == 0 && bufferIterator == parserBuffer.Length)
                        {
                            Read(tempByte, 0, 1);
                        }
                        else
                        {
                            tempByte[0] = parserBuffer[bufferIterator++];
                        }
#endif
                        receivedBytes.Add(tempByte[0]);
                    }
                    catch (TimeoutException te)
                    {
                        //Console.WriteLine("serialPort.Read TimeoutException: " + te.Message);
                        if (StartTimeoutTime == UNIXSTARTTIME)
                        {
                            StartTimeoutTime = DateTime.UtcNow;
                        }
                        else
                        {
                            TotalTimeoutTime = (DateTime.UtcNow - StartTimeoutTime).TotalMilliseconds;
                        }

                        continue;
                    }
                    catch (IndexOutOfRangeException ie)
                    {
                        Console.WriteLine("parserBuffer.Length: " + parserBuffer.Length);
                        Console.WriteLine("bufferIterator: " + bufferIterator);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ReadPackets: " + e.Message);
                    }

                    switch (state)
                    {
                        /*Waiting for the first SYNC_BYTE*/
                        case ((int)ParserState.Sync0):
                            if (tempByte[0] == SYNC_BYTE)
                            {
                                state = (int)ParserState.Sync1;
                            }
                            break;

                        /*Waiting for the second SYNC_BYTE*/
                        case ((int)ParserState.Sync1):
                            if (tempByte[0] == SYNC_BYTE)
                            {
                                state = (int)ParserState.PayloadLength;
                            }
                            else
                            {
                                state = (int)ParserState.Sync0;
                            }
                            break;

                        /* Waiting for payload length */
                        case ((int)ParserState.PayloadLength):
                            payloadLength = tempByte[0];
                            if (payloadLength >= 170)
                            {
                                state = (int)ParserState.Sync0;
                            }
                            else
                            {
                                payload.Clear();
                                payloadSum = 0;
                                state = (int)ParserState.Payload;
                            }
                            break;

                        /* Waiting for Payload bytes */
                        case ((int)ParserState.Payload):
                            payload.Add(tempByte[0]);
                            payloadSum += tempByte[0];
                            if (payload.Count >= payloadLength)
                            {
                                state = (int)ParserState.Checksum;
                            }
                            break;

                        /* Waiting for checksum byte */
                        case ((int)ParserState.Checksum):
                            checkSum = tempByte[0];
                            state = (int)ParserState.Sync0;
                            if (checkSum == ((~payloadSum) & 0xFF))
                            {
                                //Console.WriteLine("Parsing Payload");
                                tempDataRowArray = ParsePayload(payload.ToArray());
                                receivedBytes.Clear();
                                StartTimeoutTime = UNIXSTARTTIME;
                            }
                            break;
                    }

                    if (tempDataRowArray != null)
                    {
                        foreach (DataRow tempDataRow in tempDataRowArray)
                        {
                            receivedDataRow.Add(tempDataRow);
                        }
                    }

                }

                if (receivedBytes.Count > 0)
                {
                    parserBuffer = receivedBytes.ToArray();
                }
                else { parserBuffer = new Byte[0]; }

                //if( receivedDataRow.Count < 1 ) debugFile.WriteLine("Did not receive any packet.");

                return PackagePacket(receivedDataRow.ToArray(), PortName);

            }/*End of ReadPacket*/

            private Packet PackagePacket(DataRow[] dataRow, string portName)
            {
                Packet tempPacket = new Packet();
                List<DataRow> tempDataRowList = new List<DataRow>();

                tempPacket.PortName = portName;

                for (int i = 0; i < dataRow.Length; i++)
                {
                    if (dataRow[i].Type == (int)DataType.HeadsetID)
                    {
                        tempPacket.HeadsetID = (int)dataRow[i].Data[0];
                    }
                    else
                    {
                        tempDataRowList.Add(dataRow[i]);
                    }
                }

                tempPacket.DataRowArray = tempDataRowList.ToArray();

                return tempPacket;
            }/*End of PackagePacket*/

            private DataRow[] ParsePayload(byte[] payload)
            {

                List<DataRow> tempDataRowList = new List<DataRow>();
                DataRow tempDataRow = new DataRow();
                int i = 0;
                int extendedCodeLevel = 0;
                byte code = 0;
                int numBytes = 0;

                /* Parse all bytes from the payload[] */
                while (i < payload.Length)
                {

                    /* Parse possible Extended CODE bytes */
                    while (payload[i] == EXCODE_BYTE)
                    {
                        extendedCodeLevel++;
                        i++;
                    }

                    /* Parse CODE */
                    code = payload[i++];

                    /* Parse value length */
                    if (code >= 0x80) numBytes = payload[i++];
                    else numBytes = 1;

                    /*Copies the Code to the tempDataRow*/
                    tempDataRow.Type = code;

                    /*Gets the current time and inserts it into the tempDataRow*/
                    tempDataRow.Time = (DateTime.UtcNow - UNIXSTARTTIME).TotalSeconds;

                    //debugFile.WriteLine("TimeStamp: " + (DateTime.UtcNow - startTime).TotalSeconds);

                    /*Copies the data into the DataRow*/
                    tempDataRow.Data = new byte[numBytes];
                    for (int t = 0; t < numBytes; t++)
                    {
                        tempDataRow.Data[t] = payload[i++];
                    }

                    /*Appends the data row to list*/
                    tempDataRowList.Add(tempDataRow);
                }

                return tempDataRowList.ToArray();
            }/*End of ParsePayload()*/



        }/*End of Connection Class*/



    }/*End of Connector Class*/
}
