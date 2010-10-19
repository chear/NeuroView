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
        
        // configuration properties
        public bool blinkDetectionEnabled;

        // TODO: Deprecate this public variable, since it's not used anymore
        public volatile bool ScanConnectEnable = true;

        private List<Connection> portsToConnect;        // ports that an application wishes to connect to
        private List<Connection> activePortsList;       // ports that are currently connected
        private List<Device> deviceList;                // devices that are currently connected

        private const int DEFAULT_BAUD_RATE = 115200;

        private Thread findThread;
        private Thread readThread;

        private volatile bool ReadThreadEnable = true;
        private volatile bool FindThreadEnable = true;

        private volatile bool IsFinding = false;

        private const int REMOVE_PORT_TIMER = 1000; //In milliseconds
        private const int READ_TIMEOUT = 10000;

        public Connector() {
            mindSetPorts = new List<Connection>();
            portsToConnect = new List<Connection>();

            activePortsList = new List<Connection>();
            deviceList = new List<Device>();

            StartThreads(false);
        }

        ~Connector() {
            Close();
        }

        /**
         * Provides a collection of Connections that have ThinkGear devices. Note that this is
         * *not* a collection of active Connections.
         */
        public Connection[] AvailableConnections {
            get { return mindSetPorts.ToArray(); }
        }

        /**
         * Indicates whether the AvailableConnections collection is in the middle of refreshing. 
         * While this property returns "true", the contents of the AvailableConnections
         * collection are invalid.
         */
        public bool IsRefreshing {
            get { return IsFinding && portsToConnect.Count > 0; }
        }

        /**
         * Indicates whether the Connector is in the middle of performing a connect-scan.
         */
        public bool IsScanning {
            get { return !IsFinding && portsToConnect.Count > 0; }
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
            lock(portsToConnect){ portsToConnect.Add(new Connection(portName)); }

            StartThreads(false);
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

            lock(portsToConnect) {
                foreach(string port in ports) {
                    Connection c = new Connection(port);

                    if(!portsToConnect.Contains(c))
                        portsToConnect.Add(c);
                }
            }

            StartThreads(false);
        }

        public void ConnectScan(string initialPort) {
            Regex r = new Regex("COM[1-9][0-9]*");

            // scrub the port
            string portName = r.Match(initialPort).ToString();

            lock(portsToConnect) {
                if(portName.Length != 0)
                    portsToConnect.Add(new Connection(portName));

                string[] ports = FindAvailablePorts();

                foreach(string port in ports) {
                    Connection c = new Connection(port);

                    if(!portsToConnect.Contains(c))
                        portsToConnect.Add(c);
                }
            }

            StartThreads(false);
        }

        /**
         * Performs cleanup of the ThinkGear Connector instance.
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
            Connection[] ports = activePortsList.ToArray();

            foreach(Connection port in ports)
                Disconnect(port);
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

        /**
         * Overloaded Disconnect method that takes a Connection instance.
         */
        private void Disconnect(Connection c) {
            int deviceIndex = deviceList.FindIndex(f => (f.PortName == c.PortName));

            Device d = null;

            if(deviceIndex != -1)
                d = deviceList[deviceIndex];

            DisconnectionCleanup(c, d);
        }

        /**
         * Handle all the cleanup when disconnecting a Connection and Device
         */
        private void DisconnectionCleanup(Connection c, Device d) {
            if(c != null) {
                lock(activePortsList) { activePortsList.Remove(c); }
                c.Close();
            }

            if(d != null) {
                lock(deviceList) { deviceList.Remove(d); }

                DeviceDisconnected(this, new DeviceEventArgs(d));
            }
        }

        /**
         * Send a collection of bytes to the port specified by portName.
         */
        public void Send(string portName, byte[] byteArray) {
            Connection tempConnection = new Connection(portName);

            int index = -1;

            lock(activePortsList) {
                // Check to make sure the portName exists in the activePortsList
                index = activePortsList.FindIndex(f => (f.PortName == tempConnection.PortName));
            }

            if(index < 0)
                return;

            activePortsList[index].Write(byteArray, 0, byteArray.Length);
        }

        /**
         * Refreshes the DeviceList
         */
        public void RefreshAvailableConnections() {
            Find();
        }

        // TODO: Deprecate this method (replaced by RefreshAvailableConnections and ConnectScan methods).
        public void Find() {
            string[] ports = FindAvailablePorts();

            foreach(string port in ports)
                lock(portsToConnect) { portsToConnect.Add(new Connection(port)); }

            StartThreads(true);
        }

        // TODO: Deprecate this method (replaced by IsRefreshing and IsScanning properties)
        public bool FindThreadIsAlive() {
            return findThread.IsAlive;
        }

        /**
         * Fire off the read and find threads, handling all error conditions (e.g. if no
         * Thread instance exists, or if the Thread exists but has exited).
         * 
         * The isFind parameter dictates whether the threads will be started in "Find" mode
         * (e.g. no connection initialized to a ThinkGear device) or "Connect/Scan" mode
         * (a successful search will result in a connection to a ThinkGear device).
         */
        private void StartThreads(bool isFind) {
            IsFinding = isFind;
            ReadThreadEnable = true;
            FindThreadEnable = true;

            if(findThread == null || !findThread.IsAlive) {
                findThread = new Thread(FindThread);
                findThread.Start();
            }

            if(readThread == null || !readThread.IsAlive) {
                readThread = new Thread(ReadThread);
                readThread.Start();
            }
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

            // now sort the COM port names
            ports.Sort((x, y) => {
                // parse the number after "COM"
                int xNum = Int32.Parse(x.Substring(3));
                int yNum = Int32.Parse(y.Substring(3));

                return xNum.CompareTo(yNum);
            });

            return ports.ToArray();
        }

        /**
         * This thread has different behavior depending on whether it was invoked from a
         * Find or a ConnectScan/Connect. IsFinding is set to 'true' for a Find, 'false'
         * otherwise.
         * 
         * Messages for a ConnectScan / Connect:
         *    * DeviceConnected will be broadcasted if the attempt was successful (though not
         *      by this thread)
         *    * DeviceConnectFail will be broadcasted if the attempt failed
         *    
         * Messages for a Find:
         *    * DeviceValidating will be broadcasted for every validation attempt
         *    * DeviceFound will be broadcasted at the end if a device was found
         *    * DeviceNotFound will be broadcasted at the end if no device was found
         *    
         * For both Find and ConnectScan/Connect, devices are added to the mindSetPorts 
         * collection.
         * 
         * For only ConnectScan/Connect, devices are added to the activePortsList collection 
         * for the ReadThread to handle.
         */
        private void FindThread() {
            while(FindThreadEnable) {
                Connection[] ports;

                lock(portsToConnect) {
                    ports = portsToConnect.ToArray();
                }

                if(ports.Length > 0) {
                    if(IsFinding)
                        lock(mindSetPorts) { mindSetPorts.Clear(); }

                    foreach(Connection tempPort in ports) {
#if DEBUG
                        Console.WriteLine("MVC scanning " + tempPort.PortName);
#endif

                        // Always notifies which port it is validating.
                        DeviceValidating(this, new ConnectionEventArgs(tempPort));

                        try {
                            tempPort.Open();

                            // sleep the thread to wait for some data to come in before we
                            // try to read data from the port
                            Thread.Sleep(2000);
                        }
                        catch(Exception e) {
                            Console.WriteLine("Caught exception on port open: " + e.Message);
                        }

                        if(tempPort.IsOpen) {
                            Packet returnPacket = new Packet();

                            try {
                                returnPacket = tempPort.ReadPacket();
                            }
                            catch(Exception e) {
                                Console.WriteLine("Caught at port scan: " + e);
                            }

                            if(returnPacket.DataRowArray != null && returnPacket.DataRowArray.Length > 0) {
                                // found a valid ThinkGear device, so add it to the list of
                                // valid ports
                                if(IsFinding) {
                                    lock(mindSetPorts) { mindSetPorts.Add(tempPort); }
                                }

                                // If this is a ConnectScan, then go ahead and connect directly
                                // to the port. No need to worry about sending messages at this
                                // point.
                                if(!IsFinding) {
                                    // clear the portsToConnect list. this fixes the bug where
                                    // subsequent Connect/ConnectScan attempts would re-search
                                    // ports
                                    lock(portsToConnect) { portsToConnect.Clear(); }

                                    // notify ReadThread that we should start reading from this port
                                    lock(activePortsList) { activePortsList.Add(tempPort); }

                                    // increase the timeout to some arbitrarily large number
                                    // so that we're not always tripping TimeoutException (e.g. under
                                    // low battery conditions)
                                    tempPort.ReadTimeout = READ_TIMEOUT;

                                    return;
                                }
                            }

                            tempPort.Close();
                        }

                        lock(portsToConnect) { portsToConnect.Remove(tempPort); }
                    }

                    // If the Connect/ConnectScan failed, then broadcast a DeviceConnectFail message
                    if(!IsFinding && mindSetPorts.Count == 0) {
                        DeviceConnectFail(this, EventArgs.Empty);
                    }
                    // Otherwise, indicate whether the Find call was successful or not
                    else {
                        if(mindSetPorts.Count > 0)
                            DeviceFound(this, new PortEventArgs(mindSetPorts[0].PortName));
                        else
                            DeviceNotFound(this, EventArgs.Empty);
                    }

                    IsFinding = false;
                }

                Thread.Sleep(50);
            }
        }

        /**
         * ReadThread is responsible for transmitting headset data to all active connections.
         * It is also responsible for disconnecting any connections that have died.
         */
        private void ReadThread() {
            while(ReadThreadEnable) {
                Connection[] ports;

                lock(activePortsList) {
                    ports = activePortsList.ToArray();
                }

                foreach (Connection port in ports) {
                    Packet returnPacket = new Packet();

                    try {
                        returnPacket = port.ReadPacket();
                    }
                    catch(Exception e) {
                        Console.WriteLine("Caught exception on read: " + e);

                        // catch the case where the port gets removed from APL
                        // in the Disconnect call, but ReadThread is still processing
                        // old cached data. this is to prevent Disconnect() being called
                        // twice.
                        if(activePortsList.Contains(port))
                            Disconnect(port);

                        continue;
                    }

                    // Pass the data to the devices.
                    DeliverPacket(returnPacket);
                }

                Thread.Sleep(20);
            }
        }

        // Delivers a packet to a device
        private void DeliverPacket(Packet packet) {
            Device tempDevice = new Device(packet.PortName, packet.HeadsetID);

            // Searches the device list and see the device is listed
            if(!deviceList.Contains(tempDevice)) {
                lock(deviceList) { deviceList.Add(tempDevice); }

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

            private const int INITIAL_READ_TIMEOUT = 4000; //in milliseconds

            private const byte SYNC_BYTE = 0xAA;
            private const byte EXCODE_BYTE = 0x55;
            private const byte MULTI_BYTE = 0x80;

            private DateTime packetLastReceived;
            public double TotalTimeoutTime = 0; //In milliseconds

            private BlinkDetector blinkDetector;
            private byte poorSignal = 200;

            private bool blinkPacketFound = false;

            private byte[] buffer;
            private int payloadBytesRemaining = 0;
            private byte[] payloadBuffer;

            public enum ParserStatuses {
                Invalid,
                Sync0,
                Sync1,
                PayloadLength,
                Payload,
                Checksum
            }

            public struct ParserState {
                public ParserStatuses packetState;
                public int payloadLength;
                public ushort cumulativeChecksum;
            }

            public ParserState parserState;

            public Connection() : this(" ") {
            }

            public Connection(String portName) {
                buffer = new byte[1024];

                BaudRate = 115200;
                ReadTimeout = INITIAL_READ_TIMEOUT;

                PortName = portName;
                blinkDetector = new BlinkDetector();

                parserState = new ParserState();
                parserState.packetState = ParserStatuses.Sync0;
                parserState.payloadLength = 0;
                parserState.cumulativeChecksum = 0;

                packetLastReceived = DateTime.UtcNow;
            }

            public Packet ReadPacket() {
                int bytesRead = 0;
                ushort computedChecksum = 0;

                List<DataRow> dataRows = new List<DataRow>(256);

                bytesRead = Read(buffer, 0, 1024);

                for(int i = 0; i < bytesRead; i++) {
                    switch(parserState.packetState) {
                        case ParserStatuses.Sync0:
                            if(buffer[i] == SYNC_BYTE)
                                parserState.packetState = ParserStatuses.Sync1;

                            break;
                        case ParserStatuses.Sync1:
                            if(buffer[i] == SYNC_BYTE)
                                parserState.packetState = ParserStatuses.PayloadLength;
                            else
                                ResetParser();

                            break;
                        case ParserStatuses.PayloadLength:
                            parserState.payloadLength = buffer[i];
                            
                            // handle the case where the parser returns a 0 for the payload
                            // length. we skip payload processing entirely and go right to the
                            // checksum
                            parserState.packetState = parserState.payloadLength == 0 ? ParserStatuses.Checksum : ParserStatuses.Payload;
                            
                            parserState.cumulativeChecksum = 0;

                            payloadBytesRemaining = buffer[i];

                            if(payloadBuffer != null)
                                payloadBuffer = null;

                            payloadBuffer = new byte[parserState.payloadLength];

                            break;
                        case ParserStatuses.Payload:
                            parserState.cumulativeChecksum += buffer[i];
                            payloadBuffer[parserState.payloadLength - payloadBytesRemaining] = buffer[i];

                            if(--payloadBytesRemaining <= 0)
                                parserState.packetState = ParserStatuses.Checksum;

                            break;
                        case ParserStatuses.Checksum:
                            computedChecksum = (ushort)(~parserState.cumulativeChecksum & 0xFF);

                            if(computedChecksum == (buffer[i] & 0xFF)) {
                                DataRow[] parsedDataRows = ParsePayload(payloadBuffer);

                                for(int j = 0; j < parsedDataRows.Length; j++)
                                    dataRows.Add(parsedDataRows[j]);

                                payloadBuffer = null;

                                packetLastReceived = DateTime.UtcNow;
                            }

                            ResetParser();

                            break;
                    }
                }

                return PackagePacket(dataRows.ToArray(), PortName);
            }

            private void ResetParser() {
                parserState.packetState = ParserStatuses.Sync0;
                parserState.payloadLength = 0;
                parserState.cumulativeChecksum = 0;
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

                    try {
                        Array.Copy(payload, i, tempDataRow.Data, 0, numBytes);
                    }
                    catch(ArgumentException ae) {
                        Console.WriteLine("Argument exception!");
                    }

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

                    // if blink packet is received from the headset then skip embedded blink detection
                    if (tempDataRow.Type == Code.Blink) {
                      blinkPacketFound = true;               
                    }
                    // check if a blink was detected every time a raw packet is received
                    if(!blinkPacketFound && tempDataRow.Type == Code.Raw) {
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
