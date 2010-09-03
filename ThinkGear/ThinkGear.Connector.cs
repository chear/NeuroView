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
        Battery             = 0x01,
        PoorSignal          = 0x02,
        Attention           = 0x04,
        Meditation          = 0x05,
        DampenedAtt         = 0x14,
        DampenedMed         = 0x15,
        Blink               = 0x16,
        HeadsetID           = 0x7F,
        Raw                 = 0x80,
        EEGPowerFloat       = 0x81,
        EEGPowerInt         = 0x83,
        RawMS               = 0x90,
        Accelerometer       = 0x91,
		EMGPower            = 0x94,
        Offhead             = 0xC0,
        HeadsetConnect      = 0xD0,
        HeadsetNotFound     = 0xD1,
        HeadsetDisconnect   = 0xD2,
        RequestDenied       = 0xD3,
        DongleStatus        = 0xD4
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

        public volatile bool ScanConnectEnable = true;

        private List<Connection> portsToConnect;
        private List<Connection> activePortsList;
        private List<Device> deviceList;

        private const int DEFAULT_BAUD_RATE = 115200;

        private Thread findThread;
        private Thread readThread;

        private volatile bool ReadThreadEnable = true;
        private volatile bool FindThreadEnable = true;

        private volatile bool IsScanning = false;

        private const int REMOVE_PORT_TIMER = 1000; //In milliseconds

        public Connector() {
            mindSetPorts = new List<Connection>();
            portsToConnect = new List<Connection>();

            activePortsList = new List<Connection>();
            deviceList = new List<Device>();

            findThread = new Thread(FindThread);
            readThread = new Thread(ReadThread);

            readThread.Start();
            findThread.Start();
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
            lock(portsToConnect){
                portsToConnect.Add(new Connection(portName));
            }

            ReadThreadEnable = true;
            FindThreadEnable = true;

            if(!findThread.IsAlive)
                findThread.Start();

            if(!readThread.IsAlive) 
                readThread.Start();
        }

        public void Send(string portName, byte[] byteArray) {
            Connection tempConnection = new Connection(portName);

            int index = -1;

            lock (activePortsList) {
                // Check to make sure the portName exists in the activePortsList
                index = activePortsList.FindIndex(f => (f.PortName == tempConnection.PortName));
            }

            if(index < 0)
                return;

            activePortsList[index].Write(byteArray, 0, byteArray.Length);
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
            string[] ports = FindAvailablePorts();

            foreach(string port in ports) {
                lock(portsToConnect) {
                    portsToConnect.Add(new Connection(port));
                }
            }

            ReadThreadEnable = true;
            FindThreadEnable = true;

            if(!findThread.IsAlive)
                findThread.Start();

            if(!readThread.IsAlive)
                readThread.Start();
        }

        /**
         * Performs cleanup of the ThinkGear Connector instance.
         * 
         * TODO: Move this stuff into a destructor, so that an application that uses the Connector doesn't have to explicitly call this.
         */
        public void Close() {
            ReadThreadEnable = false;
            FindThreadEnable = false;

            this.Disconnect();
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
            Connection[] ports = activePortsList.ToArray();

            foreach(Connection port in ports){
                Disconnect(port);
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

                Connection c = null;

                // perform cleanup
                if(connectionIndex != -1)
                    c = activePortsList[connectionIndex];

                DisconnectionCleanup(c, d);
            }
        }

        private void Disconnect(Connection c) {
            int deviceIndex = deviceList.FindIndex(f => (f.PortName == c.PortName));

            Device d = null;

            if(deviceIndex != -1)
                d = deviceList[deviceIndex];

            DisconnectionCleanup(c, d);
        }

        private void DisconnectionCleanup(Connection c, Device d) {
            if(c != null) {
                c.Close();

                lock(activePortsList) {
                    activePortsList.Remove(c);
                }
            }

            if(d != null) {
                lock(deviceList) {
                    deviceList.Remove(d);
                }

                DeviceDisconnected(this, new DeviceEventArgs(d));
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
         * 
         * TODO: Re-implement functionality
         */
        public void RefreshAvailableConnections() {
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
        /*
        public bool IsScanning {
            get { return ScanConnectEnable == true && findThread.IsAlive; }
        }
         */


        // TODO: Deprecate this method (replaced by RefreshAvailableConnections and ConnectScan methods).
        public void Find() {
            IsScanning = true;

            string[] ports = FindAvailablePorts();

            foreach(string port in ports) {
                lock(portsToConnect) {
                    portsToConnect.Add(new Connection(port));
                }
            }

            IsScanning = true;

            ReadThreadEnable = true;
            FindThreadEnable = true;

            if(!findThread.IsAlive)
                findThread.Start();

            if(!readThread.IsAlive)
                readThread.Start();
        }

        // TODO: Deprecate this method (replaced by IsRefreshing and IsScanning properties)
        public bool FindThreadIsAlive() {
            return findThread.IsAlive;
        }

        /**
         * This method scans through all the serial ports listed by the system, and then
         * performs some scrubbing to make sure that they're "clean" port names (i.e. are of the
         * form "COM#").
         */
        private string[] FindAvailablePorts() {
            string[] rawNames = SerialPort.GetPortNames();

            List<string> ports = new List<string>();

            Regex r = new Regex("COM[1-9][0-9]*");

            // iterate over each of the raw COM port names and then
            // scrub them
            foreach(string rawName in rawNames){
                string portName = r.Match(rawName).ToString();

                if(portName.Length != 0) {
                    ports.Add(portName);
                }
            }

            return ports.ToArray();
        }

        private void FindThread() {
            while(FindThreadEnable) {
                Connection[] ports = portsToConnect.ToArray();

                if(ports.Length > 0) {
                    foreach(Connection tempPort in ports) {
                        if(IsScanning)
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

                            if(returnPacket.DataRowArray.Length > 0) {
                                lock(mindSetPorts) {
                                    mindSetPorts.Add(tempPort);
                                }

                                //Connects to the First MindSet it found.
                                if(!IsScanning) {
                                    lock(activePortsList) {
                                        activePortsList.Add(tempPort);
                                    }

                                    return;
                                }
                            }

                            tempPort.Close();
                        }

                        lock(portsToConnect) {
                            portsToConnect.Remove(tempPort);
                        }
                    }

                    if(!IsScanning && mindSetPorts.Count == 0) {
                        DeviceConnectFail(this, EventArgs.Empty);
                    }
                    else {
                        if(mindSetPorts.Count > 0)
                            DeviceFound(this, new PortEventArgs(mindSetPorts[0].PortName));
                        else
                            DeviceNotFound(this, EventArgs.Empty);
                    }

                    IsScanning = false;
                }

                Thread.Sleep(500);
            }
        }

        private void ReadThread() {
            bool allReturnNull = true;

            // Exits if readThreadEnable is false. 
            while(ReadThreadEnable) {
                Connection[] ports = activePortsList.ToArray();

                foreach(Connection port in ports) {
                    Packet returnPacket = new Packet();

                    try {
                        returnPacket = port.ReadPacket();
                    }
                    catch(Exception e) {
                        Console.WriteLine("Caught exception " + e.Message);
                        Disconnect(port);
                        continue;
                    }

                    // Checks if it received any packet from any of the connections.
                    if(returnPacket.DataRowArray.Length > 0)
                        allReturnNull = false;

                    // Pass the data to the devices.
                    DeliverPacket(returnPacket);

                    // Check the TotalTimeout and add to the remove list if is not receiving
                    if(port.TotalTimeoutTime > 1000) {
                        Disconnect(port);
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

        // Delivers a packet to a device
        private void DeliverPacket(Packet packet) {
            Device tempDevice = new Device(packet.PortName, packet.HeadsetID);

            // Searches the device list and see the device is listed
            if(!deviceList.Contains(tempDevice)) {
                lock(deviceList) {
                    deviceList.Add(tempDevice);
                }

                DeviceConnected(this, new DeviceEventArgs(tempDevice));
            }

            // Finds the index for the device
            int index = deviceList.FindIndex(f => (f.PortName == tempDevice.PortName) && (f.HeadsetID == tempDevice.HeadsetID));

            if(index < 0)
                return;

            (deviceList[index]).Deliver(packet.DataRowArray);
        }

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

                    switch(state) {
                        // Waiting for the first SYNC_BYTE
                        case (ParserState.Sync0):
                            if(tempByte[0] == SYNC_BYTE) {
                                state = ParserState.Sync1;
                            }
                            break;

                        // Waiting for the second SYNC_BYTE
                        case (ParserState.Sync1):
                            if(tempByte[0] == SYNC_BYTE) {
                                state = ParserState.PayloadLength;
                            }
                            else {
                                state = ParserState.Sync0;
                            }
                            break;

                        // Waiting for payload length
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

                        // Waiting for Payload bytes
                        case (ParserState.Payload):
                            payload.Add(tempByte[0]);
                            payloadSum += tempByte[0];
                            if(payload.Count >= payloadLength) {
                                state = ParserState.Checksum;
                            }
                            break;

                        // Waiting for checksum byte
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
            }

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
            }

            private DataRow[] ParsePayload(byte[] payload) {
                List<DataRow> tempDataRowList = new List<DataRow>();
                DataRow tempDataRow = new DataRow();
                int i = 0;
                int extendedCodeLevel = 0;
                byte code = 0;
                int numBytes = 0;

                // Parse all bytes from the payload[]
                while(i < payload.Length) {
                    // Parse possible Extended CODE bytes
                    while(payload[i] == EXCODE_BYTE) {
                        extendedCodeLevel++;
                        i++;
                    }

                    // Parse CODE
                    code = payload[i++];

                    // Parse value length
                    numBytes = code >= 0x80 ? payload[i++] : 1;

                    // Copies the Code to the tempDataRow
                    tempDataRow.Type = (Code)code;

                    double currentTime = (DateTime.UtcNow - UNIXSTARTTIME).TotalSeconds;

                    // Gets the current time and inserts it into the tempDataRow
                    tempDataRow.Time = currentTime;

                    // Copies the data into the DataRow
                    tempDataRow.Data = new byte[numBytes];

                    Array.Copy(payload, i, tempDataRow.Data, 0, numBytes);

                    i += numBytes;

                    // Appends the data row to list
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

        public class PortEventArgs: EventArgs {
            public string PortName;

            public PortEventArgs(string portName) {
                this.PortName = portName;
            }
        }
    }
}