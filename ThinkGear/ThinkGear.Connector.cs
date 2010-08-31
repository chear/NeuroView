using System;
using System.Collections.Generic;

using System.Text;
using System.Text.RegularExpressions;

using System.IO;
using System.IO.Ports;

using System.Threading;

using NeuroSky.ThinkGear.Algorithms;

namespace NeuroSky.ThinkGear {

    /*
     * A packet of data from the headset
     */
    public struct Packet {
        public Type DeviceType;
        public string PortName;
        public int HeadsetID;
        public DataRow[] DataRowArray;
    }

    /*
     * The data content within the packet.
     */
    public struct DataRow {
        public double Time;
        public Code Type;
        public byte[] Data;
    }

    /*
     * The raw CODE used in the packet
     */
    public enum Code: byte {
        Battery       = 0x01,
        PoorSignal    = 0x02,
        Attention     = 0x04,
        Meditation    = 0x05,
        DampenedAtt   = 0x14,
        DampenedMed   = 0x15,
        Blink         = 0x16,
        HeadsetID     = 0x7F,
        Raw           = 0x80,
        EEGPowerFloat = 0x81,
        EEGPowerInt = 0x83,
		EMGPower    = 0x94,   
        HeadsetConnect = 0xD0,
        HeadsetNotFound = 0xD1,
        HeadsetDisconnect = 0xD2,
        RequestDenied = 0xD3,
        DongleStatus = 0xD4
    };

    // The main controller that connects the connections to a specific device.  
    // This is the controller who initiates the read.
    public class Connector {
        public List<Connection> mindSetPorts;

        // devicelist refresh events
        public event EventHandler DeviceFound = delegate { };
        public event EventHandler DeviceNotFound = delegate { };
        public event EventHandler DeviceValidating = delegate { };

        // device connection events
        public event EventHandler DeviceConnected = delegate { };
        public event EventHandler DeviceConnectFail = delegate { };
        public event EventHandler DeviceDisconnected = delegate { };

        public bool ScanConnectEnable = true;

        private List<string> availablePorts;
        private List<Connection> portsToConnect;
        private List<Connection> activePortsList;
        private List<Connection> removePortsList;
        private List<Device> deviceList;

        private const int DEFAULT_BAUD_RATE = 115200;

        private Thread findThread;
        private Thread readThread;
        private Thread addThread;

        private bool ReadThreadEnable = true;

        private const int REMOVE_PORT_TIMER = 1000; //In milliseconds

        public Connector() {
            availablePorts = new List<string>();
            mindSetPorts = new List<Connection>();
            portsToConnect = new List<Connection>();

            activePortsList = new List<Connection>();
            removePortsList = new List<Connection>();
            deviceList = new List<Device>();

            findThread = new Thread(FindThread);
            readThread = new Thread(ReadThread);
            addThread = new Thread(AddThread);

            readThread.Priority = ThreadPriority.Highest;

            readThread.Start();
        }

        ~Connector() {
            Close();
        }

        /**
         * Attempts to open a connection to a Device on the serial port named portName.
         * 
         * Calling this method results in one of two events being broadcasted:
         * 
         *      DeviceConnected - A connection was successfully opened on portName
         *      DeviceConnectFail - The connection attempt was unsuccessful
         */
        public void Connect(string portName) {
            Connection tempConnection = new Connection(portName);

            lock(portsToConnect) {
                portsToConnect.Add(tempConnection);
            }

            /*
            if(addThread == null || addThread.ThreadState == ThreadState.Stopped) {
                addThread = new Thread(AddThread);               
            }
             */
            if (!addThread.IsAlive) {
              addThread.Start();
            }

            if(!readThread.IsAlive) 
                readThread.Start();
        }

        public void Send(string portName, byte[] byteArray) {
            Connection tempConnection = new Connection(portName);

            lock (activePortsList) {
                // Check to make sure the portName exists in the activePortsList
                int index = activePortsList.FindIndex(f => (f.PortName == tempConnection.PortName));

                if (index < 0)return;

                activePortsList[index].Write(byteArray, 0, byteArray.Length);
            }
        }

        /**
         * Attempts to open a connection to the first Device seen by the Connector.
         * 
         * Calling this method results in one of two events being broadcasted:
         * 
         *      DeviceConnected - A connection was successfully opened
         *      DeviceConnectFail - The connection attempt was unsuccessful
         *      
         * TODO: Overload to take a preferred initial port to scan
         */
        public void ConnectScan() {
            ScanConnectEnable = true;
            Find();
        }

        /**
         * Performs cleanup of the ThinkGear Connector instance.
         * 
         * TODO: Move this stuff into a destructor, so that an application that uses the Connector doesn't have to explicitly call this.
         */
        public void Close() {
            this.Disconnect();

            ReadThreadEnable = false;

            Thread.Sleep(50);

            if(readThread != null) 
                readThread.Abort();

            if(findThread != null )
                findThread.Abort();

            if(addThread != null ) 
                addThread.Abort();
        }

        /**
         * Closes all open connections.
         * 
         * Calling this method will result in the following event being broadcasted for
         * each open device:
         * 
         *      DeviceDisconnected - The device was disconnected
         */
        public void Disconnect() {
            // iterate over all open connections
            foreach(Connection c in activePortsList) {
                // make sure the associated Device is present in the deviceList
                int deviceIndex = deviceList.FindIndex(f => (f.PortName == c.PortName));

                // go ahead and clean up
                if(deviceIndex != -1){
                    c.Close();

                    Device d = deviceList[deviceIndex];

                    deviceList.Remove(d);
                    activePortsList.Remove(c);
                    DeviceDisconnected(this, new DeviceEventArgs(d));
                }
            }
        }

        /**
         * Closes a specific connection
         * 
         * Calling this method will result in the following event being broadcasted for
         * a specific open device:
         * 
         *      DeviceDisconnected - The device was disconnected
         */
        public void Disconnect(Device d) {
            // check to see whether the device exists in aPL and dL
            if(deviceList.Contains(d)) {
                // make sure a Connection exists for this Device
                int connectionIndex = activePortsList.FindIndex(f => (f.PortName == d.PortName));

                // perform cleanup
                if(connectionIndex != -1) {
                    Connection c = activePortsList[connectionIndex];

                    c.Close();

                    deviceList.Remove(d);
                    activePortsList.Remove(c);

                    DeviceDisconnected(this, new DeviceEventArgs(d));
                }
            }
        }

        /**
         * Provides a collection of Connections that have ThinkGear devices. Note that this is
         * *not* a collection of active Connections.
         */
        public Connection[] AvailableConnections {
            get { return mindSetPorts.ToArray(); }
        }

        /**
         * Refreshes the DeviceList
         */
        public void RefreshAvailableConnections() {
            ScanConnectEnable = false;
            Find();
        }

        /**
         * Indicates whether the AvailableConnections collection is in the middle of refreshing. 
         * While this property returns "true", the contents of the AvailableConnections
         * collection are invalid.
         */
        public bool IsRefreshing {
            get { return ScanConnectEnable == false && findThread.IsAlive; }
        }

        /**
         * Indicates whether the Connector is in the middle of performing a connect-scan.
         */
        public bool IsScanning {
            get { return ScanConnectEnable == true && findThread.IsAlive; }
        }


        // TODO: Deprecate this method (replaced by RefreshAvailableConnections and ConnectScan methods).
        public void Find() {
            if(!findThread.IsAlive) {
                findThread.Start();
            }
        }

        // TODO: Deprecate this method (replaced by IsRefreshing and IsScanning properties)
        public bool FindThreadIsAlive() {
            return findThread.IsAlive;
        }

        // TODO: Make this method private, or refactor it into FindThread.
        public void FindAvailablePorts() {
            string[] rawNames = SerialPort.GetPortNames();

            availablePorts.Clear();

            Regex r = new Regex("COM[1-9][0-9]*");

            // iterate over each of the raw COM port names and then
            // scrub them
            for(int i = 0; i < rawNames.Length; i++) {
                string portName = r.Match(rawNames[i]).ToString();

                if(portName.Length != 0) {
                    availablePorts.Add(portName);
                }
            }
        }

        private void FindThread() {
            FindAvailablePorts();

            Connection tempPort;

            lock(mindSetPorts) {
                mindSetPorts.Clear();
            }

            foreach(string portName in availablePorts) {
                tempPort = new Connection();

                tempPort.PortName = portName;
                tempPort.BaudRate = DEFAULT_BAUD_RATE;

                DeviceValidating(this, new ConnectionEventArgs(tempPort));

                try {
                    tempPort.Open();
                    Thread.Sleep(100);
                }
                catch(Exception e) {
                    Console.WriteLine("Exception on port opening: " + e.Message);
                }

                if(tempPort.IsOpen) {
                    Packet returnPacket = tempPort.ReadPacket();

                    /*
                    Console.WriteLine("Received " + returnPacket.DataRowArray.Length + " DataRows at FindThread.");

                    for(int i = 0;i < returnPacket.DataRowArray.Length;i++) {
                        Console.Write(returnPacket.DataRowArray[i].Time + " : ["
                                   + (Code)(returnPacket.DataRowArray[i].Type) + "] : ");

                        for(int t = 0;t < returnPacket.DataRowArray[i].Data.Length;t++) {
                            Console.Write("0x" + returnPacket.DataRowArray[i].Data[t].ToString("X2") + " ");
                        }

                        Console.Write("\n");
                    }
                     */

                    if(returnPacket.DataRowArray.Length > 0) {
                        lock(mindSetPorts) {
                            mindSetPorts.Add(tempPort);
                        }

                        //Connects to the First MindSet it found.
                        if(ScanConnectEnable) {
                            lock(activePortsList) {
                                activePortsList.Add(tempPort);
                            }

                            return;
                        }
                    }

                    tempPort.Close();
                }
            }

            Thread.Sleep(1000);

            if(ScanConnectEnable && mindSetPorts.Count == 0) {
                DeviceConnectFail(this, EventArgs.Empty);
            }
            else {
                if(mindSetPorts.Count > 0)
                    DeviceFound(this, EventArgs.Empty);
                else
                    DeviceNotFound(this, EventArgs.Empty);
            }
        }

        private void ReadThread() {
            bool allReturnNull = true;

            // Exits if readThreadEnable is false. 
            while(ReadThreadEnable) {
                lock(activePortsList) {
                    try {
                        foreach(Connection port in activePortsList) {
                            Packet returnPacket = port.ReadPacket();

                            /* Checks if it received any packet from any of the connections.*/
                            if(returnPacket.DataRowArray.Length > 0)
                                allReturnNull = false;

                            /*
                            foreach (DataRow d in returnPacket.DataRowArray) {
                                Console.Write(d.Time + ": " + d.Type + ":");

                                foreach (byte b in d.Data) {
                                    Console.Write( " 0x" + b.ToString("X2"));
                                }
                                
                                Console.Write("\n");
                            }
                            */


                            /*Pass the data to the devices.*/
                            lock(deviceList) {
                                DeliverPacket(returnPacket);
                            }

                            // Check the TotalTimeout and add to the remove list if is not receiving
                            if(port.TotalTimeoutTime > 1000) {
                                lock(removePortsList) {
                                    removePortsList.Add(port);
                                }
                            }
                        }
                    }
                    // catch the exception where the collection was modified inside the loop.
                    // we can simply ignore and wait for the next iteration
                    catch(InvalidOperationException) {

                    }
                }

                // Sleep when DataRow null.
                // TODO: Make a dynamic sleep thread. (What does this mean? -HK)
                if(allReturnNull) {
                    Thread.Sleep(2);
                }

                allReturnNull = true;

            }

        }

        private void AddThread() {

            lock(portsToConnect) {
                foreach(Connection tempPort in portsToConnect) {
                    if(tempPort.IsOpen) 
                        break;

                    //Connect if it was opened before.
                    try {
                        tempPort.Open();
                        Thread.Sleep(100);
                    }
                    catch(Exception e) {
                        Console.WriteLine("tempPort.Open Exception: " + e.Message);
                    }

                    Packet returnPacket = tempPort.ReadPacket();

                    //If it can read valid packets add to activePortList
                    if( returnPacket.DataRowArray.Length > 0) {
                        lock(activePortsList) {
                            activePortsList.Add(tempPort);
                        }
                    }
                    else {
                        tempPort.Close();
                    }
                }

                portsToConnect.Clear();
            }

            lock(activePortsList) {
                if(activePortsList.Count < 1) {
                    DeviceConnectFail(this, EventArgs.Empty);
                }
            }

            Thread.Sleep(1000);
        }

        //Delivers a packet to a device
        private void DeliverPacket(Packet packet) {
            Device tempDevice = new Device(packet.PortName, packet.HeadsetID);

            /*Searches the device list and see the device is listed*/
            if(!deviceList.Contains(tempDevice)) {
                deviceList.Add(tempDevice);

                DeviceConnected(this, new DeviceEventArgs(tempDevice));
            }

            /*Finds the index for the device*/
            int index = deviceList.FindIndex(f => (f.PortName == tempDevice.PortName) && (f.HeadsetID == tempDevice.HeadsetID));

            if(index < 0)
                return;

            (deviceList[index]).Deliver(packet.DataRowArray);
        }/*End of DeliverPacket*/

        public class Connection: SerialPort {
            private DateTime UNIXSTARTTIME = new DateTime(1970, 1, 1, 0, 0, 0);

            private const int SERIALPORT_READ_TIMEOUT = 50; //in milliseconds
            private const int READ_PACKET_TIMEOUT = 2;      // in seconds

            private const byte SYNC_BYTE = 0xAA;
            private const byte EXCODE_BYTE = 0x55;
            private const byte MULTI_BYTE = 0x80;

            public DateTime StartTimeoutTime = new DateTime(1970, 1, 1, 0, 0, 0);
            public double TotalTimeoutTime = 0; //In milliseconds

            private BlinkDetector blinkDetector;
            private byte poorSignal = 200;

            public enum ParserState {
                Invalid,
                Sync0,
                Sync1,
                PayloadLength,
                Payload,
                Checksum
            };

            public Byte[] parserBuffer = new Byte[0];

            public Connection() : this(" ") {
            }

            public Connection(String portName) {
                parserBuffer = new byte[0];
                BaudRate = 115200;
                ReadTimeout = SERIALPORT_READ_TIMEOUT;

                PortName = portName;
                blinkDetector = new BlinkDetector();
            }

            public Packet ReadPacket() {
                byte[] tempByte = new byte[1];
                List<byte> receivedBytes = new List<byte>();
                List<DataRow> receivedDataRow = new List<DataRow>();

                ParserState state = ParserState.Sync0;
                int payloadLength = 0;
                int payloadSum = 0;
                int checkSum = 0;
                List<byte> payload = new List<byte>();
                DataRow[] tempDataRowArray = null;

                int bufferIterator = 0;

                DateTime readPacketTimeOut = DateTime.Now.AddSeconds(READ_PACKET_TIMEOUT);

                while(DateTime.Now < readPacketTimeOut && tempDataRowArray == null) {
                    try {
                        if(parserBuffer.Length > bufferIterator)
                            tempByte[0] = parserBuffer[bufferIterator++];
                        else
                            Read(tempByte, 0, 1);

                        receivedBytes.Add(tempByte[0]);
                    }
                    catch(TimeoutException te) {
                        //Console.WriteLine("serialPort.Read TimeoutException: " + te.Message);
                        if(StartTimeoutTime == UNIXSTARTTIME) {
                            StartTimeoutTime = DateTime.UtcNow;
                        }
                        else {
                            TotalTimeoutTime = (DateTime.UtcNow - StartTimeoutTime).TotalMilliseconds;
                        }

                        continue;
                    }
                    catch(IndexOutOfRangeException ie) {
                        Console.WriteLine("parserBuffer.Length: " + parserBuffer.Length);
                        Console.WriteLine("bufferIterator: " + bufferIterator);
                    }
                    catch(Exception e) {
                        Console.WriteLine("ReadPackets: " + e.Message);
                    }

                    switch(state) {
                        /*Waiting for the first SYNC_BYTE*/
                        case (ParserState.Sync0):
                            if(tempByte[0] == SYNC_BYTE) {
                                state = ParserState.Sync1;
                            }
                            break;

                        /*Waiting for the second SYNC_BYTE*/
                        case (ParserState.Sync1):
                            if(tempByte[0] == SYNC_BYTE) {
                                state = ParserState.PayloadLength;
                            }
                            else {
                                state = ParserState.Sync0;
                            }
                            break;

                        /* Waiting for payload length */
                        case (ParserState.PayloadLength):
                            payloadLength = tempByte[0];
                            if(payloadLength >= 170) {
                                state = ParserState.Sync0;
                            }
                            else {
                                payload.Clear();
                                payloadSum = 0;
                                state = ParserState.Payload;
                            }
                            break;

                        /* Waiting for Payload bytes */
                        case (ParserState.Payload):
                            payload.Add(tempByte[0]);
                            payloadSum += tempByte[0];
                            if(payload.Count >= payloadLength) {
                                state = ParserState.Checksum;
                            }
                            break;

                        /* Waiting for checksum byte */
                        case (ParserState.Checksum):
                            checkSum = tempByte[0];
                            state = ParserState.Sync0;

                            if(checkSum == ((~payloadSum) & 0xFF)) {
                                //Console.WriteLine("Parsing Payload");
                                tempDataRowArray = ParsePayload(payload.ToArray());
                                receivedBytes.Clear();
                                StartTimeoutTime = UNIXSTARTTIME;
                            }
                            break;
                    }

                    if(tempDataRowArray != null) {
                        foreach(DataRow tempDataRow in tempDataRowArray) {
                            receivedDataRow.Add(tempDataRow);
                        }
                    }
                }

                parserBuffer = receivedBytes.Count > 0 ? receivedBytes.ToArray() : new byte[0];

                return PackagePacket(receivedDataRow.ToArray(), PortName);

            }/*End of ReadPacket*/

            private Packet PackagePacket(DataRow[] dataRow, string portName) {
                Packet tempPacket = new Packet();
                List<DataRow> tempDataRowList = new List<DataRow>();

                tempPacket.PortName = portName;

                for(int i = 0;i < dataRow.Length;i++) {
                    if(dataRow[i].Type == Code.HeadsetID) {
                        tempPacket.HeadsetID = (int)dataRow[i].Data[0];
                    }
                    else {
                        tempDataRowList.Add(dataRow[i]);
                    }
                }

                tempPacket.DataRowArray = tempDataRowList.ToArray();

                return tempPacket;
            }/*End of PackagePacket*/

            private DataRow[] ParsePayload(byte[] payload) {
                List<DataRow> tempDataRowList = new List<DataRow>();
                DataRow tempDataRow = new DataRow();
                int i = 0;
                int extendedCodeLevel = 0;
                byte code = 0;
                int numBytes = 0;

                /* Parse all bytes from the payload[] */
                while(i < payload.Length) {
                    /* Parse possible Extended CODE bytes */
                    while(payload[i] == EXCODE_BYTE) {
                        extendedCodeLevel++;
                        i++;
                    }

                    /* Parse CODE */
                    code = payload[i++];

                    /* Parse value length */
                    numBytes = code >= 0x80 ? payload[i++] : 1;

                    /*Copies the Code to the tempDataRow*/
                    tempDataRow.Type = (Code)code;

                    double currentTime = (DateTime.UtcNow - UNIXSTARTTIME).TotalSeconds;

                    /*Gets the current time and inserts it into the tempDataRow*/
                    tempDataRow.Time = currentTime;

                    /*Copies the data into the DataRow*/
                    tempDataRow.Data = new byte[numBytes];

                    Array.Copy(payload, i, tempDataRow.Data, 0, numBytes);

                    i += numBytes;

                    /*Appends the data row to list*/
                    tempDataRowList.Add(tempDataRow);

                    /*
                     * Now let's apply some processing to figure out blinks
                     */

                    // look for a poorSignal data row. the blink detector needs
                    // this value
                    if(tempDataRow.Type == Code.PoorSignal)
                        poorSignal = tempDataRow.Data[0];

                    // check if a blink was detected every time a raw packet is received
                    if(tempDataRow.Type == Code.Raw) {
                        short rawValue = (short)((tempDataRow.Data[0] << 8) + tempDataRow.Data[1]);

                        byte blinkStrength = blinkDetector.Detect(poorSignal, rawValue);

                        if(blinkStrength > 0) {
                            DataRow d = new DataRow { Type = Code.Blink, 
                                                      Time = currentTime, 
                                                      Data = new byte[1] { blinkStrength } };
                            tempDataRowList.Add(d);
                        }
                    }
                }

                return tempDataRowList.ToArray();
            }
        }

        public class DeviceEventArgs: EventArgs {
            public Device Device;

            public DeviceEventArgs(Device device) {
                this.Device = device;
            }
        }

        public class ConnectionEventArgs: EventArgs {
            public Connection Connection;

            public ConnectionEventArgs(Connection connection) {
                this.Connection = connection;
            }
        }
    }
}
