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
      tg_Connector.Connect("COM40");

      Thread.Sleep(60000);
      tg_Connector.Disconnect();
      Thread.Sleep(500);
      //tg_Connector.Connect("COM41");
      //Thread.Sleep(10000);

      /*
      foreach (NeuroSky.ThinkGear.Connector.Connection c in tg_Connector.mindSetPorts)
      {
          Console.WriteLine("Found" + c.PortName);
      }*/

      //Thread.Sleep(10000);
      tg_Connector.Close();
      Thread.Sleep(500);
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
#if false
            foreach (TimeStampData tsd in parsedData.DampenedMeditation)
            {
                Console.WriteLine("Time: " + tsd.TimeStamp + " Raw Value: " + tsd.Value);
            }
#endif

#if false
            for (int i = 0; i < parsedData.DampenedAttention.Length; i++)
            {
                Console.WriteLine("Time: " + parsedData.DampenedAttention[i].TimeStamp +
                                  " Poor Signal Quality: " + parsedData.PoorSignalQuality[i].Value +
                                  " Dampend Attention: " +  parsedData.DampenedAttention[i].Value + 
                                  " Dampened Meditation: " + parsedData.DampenedMeditation[i].Value);
            }
#endif
            for (int i = 0; i < parsedData.EMGPower.Length; i++) {
              Console.WriteLine("Time: " + parsedData.EMGPower[i].TimeStamp +
                                " Poor Signal Quality: " + parsedData.PoorSignalQuality[i].Value +
                                " emg power: " + string.Format("{0}", parsedData.EMGPower[i].Value));
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
  }
}
