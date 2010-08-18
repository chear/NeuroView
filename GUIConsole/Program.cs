using System;

using System.Collections.Generic;
using System.Text;
using System.Threading;

using System.IO;
using System.IO.Ports;

using NeuroSky.ThinkGear;
using NeuroSky.ThinkGear.Parser;

namespace testprogram {
    class Program {

        public static void Main(string[] args) {

            Console.WriteLine("Hello EEG!");

            Connector tg_Connector = new Connector();
            tg_Connector.DeviceConnected += new EventHandler(OnDeviceConnected);

            //tg_Connector.Find();
            //while(tg_Connector.FindThreadIsAlive()) { /*DO NOTHING*/}
            tg_Connector.Connect("COM4");

            Thread.Sleep(5000);

            //byte[] tempByte = { 0xC2 };
            byte[] connectByte = { 0xC0, 0x8E, 0xF2 };
            tg_Connector.Send("COM4", connectByte);

            Thread.Sleep(5000);

            byte[] tempByte1 = { 0xC1 };
            tg_Connector.Send("COM4", tempByte1);

            Thread.Sleep(5000);

            System.Console.WriteLine("Goodbye.");

            Environment.Exit(0);

        }


        static void OnDeviceConnected(object sender, EventArgs e) {
            Connector.DeviceEventArgs de = (Connector.DeviceEventArgs)e;

            Console.WriteLine("New Headset Created!!! " + de.Device.PortName);

            de.Device.DataReceived += new EventHandler(OnDataReceived);

        }

        static void OnDataReceived(object sender, EventArgs e) {
            Device d = (Device)sender;
            Device.DataEventArgs de = (Device.DataEventArgs)e;

            DataRow[] tempDataRowArray = de.DataRowArray;
            Parsed parsedData = new Parsed();

            //Console.WriteLine("PortName: " + d.PortName + " HeadSetID: " + d.HeadsetID);

            MindSetParser mindSetParser = new MindSetParser();

            parsedData = mindSetParser.Read(de.DataRowArray);

            foreach(TimeStampData tsd in parsedData.DongleStatus){
                Console.WriteLine("Time: " + tsd.TimeStamp + " Dongle Status: " + tsd.Value);
            }

            foreach (TimeStampData tsd in parsedData.HeadsetConnect) {
                Console.WriteLine("Time: " + tsd.TimeStamp + " Dongle Connect: 0x" + ((int)tsd.Value).ToString("X2"));
            }

            foreach (TimeStampData tsd in parsedData.HeadsetDisconnect) {
                Console.WriteLine("Time: " + tsd.TimeStamp + " Dongle Disconnect: 0x" + ((int)tsd.Value).ToString("X2"));
            }

#if false
            foreach (TimeStampData tsd in parsedData.Raw) {
                Console.WriteLine("Time: " + tsd.TimeStamp + " Raw Value: " + tsd.Value);
            }
#endif

#if false
            for (int i = 0; i < parsedData.Attention.Length; i++)
            {
                Console.WriteLine("Time: " + parsedData.Attention[i].TimeStamp +
                                  " Poor Signal Quality: " + parsedData.PoorSignalQuality[i].Value +
                                  " Attention: " +  parsedData.Attention[i].Value + 
                                  " Meditation: " + parsedData.Meditation[i].Value);
            }
#endif

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
    }
}
