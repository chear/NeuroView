using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuroSky.ThinkGear.Parser
{
    
    /*
     * Old Parser code
     */
    // TODO:Really have to rename this guy. Yikes. Recommend "ParsedData".
    
    public struct Parsed
    {
        public TimeStampData[] PoorSignalQuality;
        public TimeStampData[] Attention;
        public TimeStampData[] Meditation;

        public TimeStampData[] DampenedAttention;
        public TimeStampData[] DampenedMeditation;

        public TimeStampData[] BlinkStrength;
        public TimeStampData[] EMGPower;
        public PowerEEGData[] PowerEEGData;

        public TimeStampData[] Raw;

        public TimeStampData[] DongleStatus;
        public TimeStampData[] HeadsetConnect;
        public TimeStampData[] HeadsetDisconnect;

    }

    
    public struct PowerEEGData
    {
        public double TimeStamp;
        public double Delta;
        public double Theta;
        public double Alpha1;
        public double Alpha2;
        public double Beta1;
        public double Beta2;
        public double Gamma1;
        public double Gamma2;

        public PowerEEGData(double timeStamp, byte[] byteArray)
        {
            this.TimeStamp = timeStamp;

            if (byteArray.Length == 24)
            {
                this.Delta = (uint)(byteArray[0] << 16) + byteArray[1] << 8 + byteArray[2];
                this.Theta = (uint)(byteArray[3] << 16) + byteArray[4] << 8 + byteArray[5];
                this.Alpha1 = (uint)(byteArray[6] << 16) + byteArray[7] << 8 + byteArray[8];
                this.Alpha2 = (uint)(byteArray[9] << 16) + byteArray[10] << 8 + byteArray[11];
                this.Beta1 = (uint)(byteArray[12] << 16) + byteArray[13] << 8 + byteArray[14];
                this.Beta2 = (uint)(byteArray[15] << 16) + byteArray[16] << 8 + byteArray[17];
                this.Gamma1 = (uint)(byteArray[18] << 16) + byteArray[19] << 8 + byteArray[20];
                this.Gamma2 = (uint)(byteArray[21] << 16) + byteArray[22] << 8 + byteArray[23];
            }
            else
            {
                Console.WriteLine("Got EEG Code but did not get the right amount of bytes.");
                this.Delta = 0;
                this.Theta = 0;
                this.Alpha1 = 0;
                this.Alpha2 = 0;
                this.Beta1 = 0;
                this.Beta2 = 0;
                this.Gamma1 = 0;
                this.Gamma2 = 0;
            }
        }
    }

    public struct TimeStampData
    {
        public double TimeStamp;
        public double Value;

        public TimeStampData(double timeStamp, double value)
        {
            this.TimeStamp = timeStamp;
            this.Value = value;
        }
    }

    public class MindSetParser
    {
        // TODO: Make this Read method a static method so that we don't have to instantiate an object to use it.
        public Parsed Read(DataRow[] dataRowArray)
        {
            Parsed tempParsed = new Parsed();

            List<TimeStampData> tempPoorSignal = new List<TimeStampData>();
            List<TimeStampData> tempAttention = new List<TimeStampData>();
            List<TimeStampData> tempMeditation = new List<TimeStampData>();
            List<TimeStampData> tempDampenedAttention = new List<TimeStampData>();
            List<TimeStampData> tempDampenedMeditation = new List<TimeStampData>();
            List<TimeStampData> tempBlinkStrength = new List<TimeStampData>();
            List<TimeStampData> tempEmgPower = new List<TimeStampData>();
            List<PowerEEGData> tempPowerEEGData = new List<PowerEEGData>();

            List<TimeStampData> tempRaw = new List<TimeStampData>();

            List<TimeStampData> tempDongleStatus = new List<TimeStampData>();
            List<TimeStampData> tempHeadsetDisconnect = new List<TimeStampData>();
            List<TimeStampData> tempHeadsetConnect = new List<TimeStampData>();

            foreach (DataRow d in dataRowArray)
            {
                switch (d.Type)
                {
                    case (Code.PoorSignal):
                        tempPoorSignal.Add(new TimeStampData(d.Time, (double)d.Data[0]));
                        break;
                    case (Code.Attention):
                        tempAttention.Add(new TimeStampData(d.Time, (double)d.Data[0]));
                        break;
                    case (Code.Meditation):
                        tempMeditation.Add(new TimeStampData(d.Time, (double)d.Data[0]));
                        break;
                    case (Code.DampenedAtt):
                        tempDampenedAttention.Add(new TimeStampData(d.Time, (double)d.Data[0]));
                        break;
                    case (Code.DampenedMed):
                        tempDampenedMeditation.Add(new TimeStampData(d.Time, (double)d.Data[0]));
                        break;
                    case (Code.Blink):
                        tempBlinkStrength.Add(new TimeStampData(d.Time, (int)d.Data[0]));
                        break;
                    case(Code.Raw):
                        tempRaw.Add(new TimeStampData(d.Time, (short)((d.Data[0] << 8) + d.Data[1])));
                        break;
                    case (Code.EMGPower):
                        tempEmgPower.Add(new TimeStampData(d.Time, BitConverter.ToSingle(ReverseBytes(d.Data), 0)));
                        break;
                    case (Code.EEGPowerInt):
                        tempPowerEEGData.Add(new PowerEEGData(d.Time, d.Data));
                        break;
                    case (Code.DongleStatus):
                        tempDongleStatus.Add(new TimeStampData(d.Time, (double)d.Data[0]));
                        break;
                    case (Code.HeadsetDisconnect):
                        tempHeadsetDisconnect.Add(new TimeStampData(d.Time, (int)((d.Data[0] << 8) + d.Data[1])));
                        break;
                    case (Code.HeadsetConnect):
                        tempHeadsetConnect.Add(new TimeStampData(d.Time, (int)((d.Data[0] << 8) + d.Data[1])));
                        break;

                }
            }

            tempParsed.PoorSignalQuality = tempPoorSignal.ToArray();
            tempParsed.Attention = tempAttention.ToArray();
            tempParsed.Meditation = tempMeditation.ToArray();
            tempParsed.Raw = tempRaw.ToArray();
            tempParsed.PowerEEGData = tempPowerEEGData.ToArray();

            tempParsed.DongleStatus = tempDongleStatus.ToArray();
            tempParsed.HeadsetDisconnect = tempHeadsetDisconnect.ToArray();
            tempParsed.HeadsetConnect = tempHeadsetConnect.ToArray();

            tempParsed.DampenedAttention = tempDampenedAttention.ToArray();
            tempParsed.DampenedMeditation = tempDampenedMeditation.ToArray();
            tempParsed.BlinkStrength = tempBlinkStrength.ToArray();
            tempParsed.EMGPower = tempEmgPower.ToArray();
            return tempParsed;
        }

        private byte[] ReverseBytes(byte[] inArray)
        {
            byte temp;
            int highCtr = inArray.Length - 1;

            for (int ctr = 0; ctr < inArray.Length / 2; ctr++)
            {
                temp = inArray[ctr];
                inArray[ctr] = inArray[highCtr];
                inArray[highCtr] = temp;
                highCtr -= 1;
            }
            return inArray;
        }
    }
    /*
     * End "Old Parser code..."
     */



    /*
     * New Parser Code
     */
    // New class name to reflect generality of this parser, able to expand capabilities in the future
    public class TGParser
    {
        // Variable containing parsed data
        public Dictionary<string, double>[] ParsedData = new Dictionary<string,double>[0];

        // Variable indicating what type of data is streaming in
        public short TGType = 1;

        public void Read(DataRow[] dataRowArray)
        {
            // Variable to pass back
            List<Dictionary<string,double>> parsedData = new List<Dictionary<string,double>>();

            // Loop through dataRowArray
            foreach (DataRow d in dataRowArray)
            {
                // Variable to fill up at each row of dataRowArray and empty out after each row is read
                Dictionary<string, double> parsedRow = new Dictionary<string, double>();

                // Get time value
                parsedRow.Add("Time", d.Time);

                //Console.WriteLine("New Row for {0} with {1} and {2}", d.Time, d.Type, (double)d.Data[0]);

                // Get payload values
                switch (d.Type)
                {
                    case(Code.PoorSignal):
                        parsedRow.Add("PoorSignal", (double)d.Data[0]);
                        break;
                    case(Code.Attention):
                        parsedRow.Add("Attention", (double)d.Data[0]);
                        break;
                    case(Code.Meditation):
                        parsedRow.Add("Meditation", (double)d.Data[0]);
                        break;
                    case(Code.DampenedAtt):
                        parsedRow.Add("DampenedAttn", (double)d.Data[0]);
                        break;
                    case (Code.DampenedMed):
                        parsedRow.Add("DampenedMed", (double)d.Data[0]);
                        break;
                    case(Code.Raw):
                        parsedRow.Add("Raw", (short)((d.Data[0] << 8) + d.Data[1]));
                        break;
                    case(Code.HeartRate):
                        parsedRow.Add("HeartRate", (double)d.Data[0]);
                        break;
                    case(Code.RawMSWithMultiTimeStamp):
                        parsedRow.Add("RawCh1", (short)((d.Data[0] << 8) + d.Data[1]));
                        if (d.Data.Length > 3) {
                            parsedRow.Add("RawCh2", (short)((d.Data[3] << 8) + d.Data[4]));
                        }
                        if (d.Data.Length > 6) {
                            parsedRow.Add("RawCh3", (short)((d.Data[6] << 8) + d.Data[7]));
                        }
                        if (d.Data.Length > 9) {
                            parsedRow.Add("RawCh4", (short)((d.Data[9] << 8) + d.Data[10]));
                        }
                        if (d.Data.Length > 12) {
                            parsedRow.Add("RawCh5", (short)((d.Data[12] << 8) + d.Data[13]));
                        }
                        if (d.Data.Length > 15) {
                            parsedRow.Add("RawCh6", (short)((d.Data[15] << 8) + d.Data[16]));
                        }
                        if (d.Data.Length > 18) {
                            parsedRow.Add("RawCh7", (short)((d.Data[18] << 8) + d.Data[19]));
                        }
                        //Console.WriteLine("\tEEG");
                        //Console.WriteLine("\t{0}\t{1}\t{2}", parsedRow["RawCh1"], parsedRow["RawCh2"], parsedRow["RawCh3"]);
                        break;

                    case (Code.RawMSWithSingleTimeStamp):
                        parsedRow.Add("RawCh1", (short)((d.Data[1] << 8) + d.Data[2]));
                        if (d.Data.Length > 3)
                        {
                            parsedRow.Add("RawCh2", (short)((d.Data[3] << 8) + d.Data[4]));
                        }
                        if (d.Data.Length > 5)
                        {
                            parsedRow.Add("RawCh3", (short)((d.Data[5] << 8) + d.Data[6]));
                        }
                        if (d.Data.Length > 7)
                        {
                            parsedRow.Add("RawCh4", (short)((d.Data[7] << 8) + d.Data[8]));
                        }
                        if (d.Data.Length > 9)
                        {
                            parsedRow.Add("RawCh5", (short)((d.Data[9] << 8) + d.Data[10]));
                        }
                        if (d.Data.Length > 11)
                        {
                            parsedRow.Add("RawCh6", (short)((d.Data[11] << 8) + d.Data[12]));
                        }
                        if (d.Data.Length > 13)
                        {
                            parsedRow.Add("RawCh7", (short)((d.Data[13] << 8) + d.Data[14]));
                        }
                        if (d.Data.Length > 15)
                        {
                            parsedRow.Add("RawCh8", (short)((d.Data[15] << 8) + d.Data[16]));
                        }
                        //Console.WriteLine("\tEEG");
                        //Console.WriteLine("\t{0}\t{1}\t{2}", parsedRow["RawCh1"], parsedRow["RawCh2"], parsedRow["RawCh3"]);
                        break;

                    case(Code.RawMSWithoutTimeStamp):
                        //see thinkcap_sdk wiki entry for full details on this code 0xB0 
                      
                        //Ch1
                        //if DataL is equal to 3
                        if (d.Data[1] == 3) {
                            //if the 5th bit of DataH is equal to 1
                            if ((d.Data[0] & 16) != 0) {
                                //dataL should be 2
                                d.Data[1] = 2;
                            }
                        }
                        //take the lower 2 bits of DataH
                        d.Data[0] = (byte)(d.Data[0] & 3);
                        parsedRow.Add("RawCh1", (short)((d.Data[0] << 8) + d.Data[1]));

                        //Ch2
                        if (d.Data.Length > 2) {
                            if (d.Data[3] == 3) {
                                if ((d.Data[2] & 16) != 0) {
                                    d.Data[3] = 2;
                                }
                            }
                            d.Data[2] = (byte)(d.Data[2] & 3);
                            parsedRow.Add("RawCh2", (short)((d.Data[2] << 8) + d.Data[3]));
                        }

                        //Ch3
                        if (d.Data.Length > 4) {
                            if (d.Data[5] == 3) {
                                if ((d.Data[4] & 16) != 0) {
                                    d.Data[5] = 2;
                                }
                            }
                            d.Data[4] = (byte)(d.Data[4] & 3);
                            parsedRow.Add("RawCh3", (short)((d.Data[4] << 8) + d.Data[5]));
                        }

                        //Ch4
                        if (d.Data.Length > 6) {
                            if (d.Data[7] == 3) {
                                if ((d.Data[6] & 16) != 0) {
                                    d.Data[7] = 2;
                                }
                            }
                            d.Data[6] = (byte)(d.Data[6] & 3);
                            parsedRow.Add("RawCh4", (short)((d.Data[6] << 8) + d.Data[7]));
                        }

                        //Ch5
                        if (d.Data.Length > 8) {
                            if (d.Data[9] == 3) {
                                if ((d.Data[8] & 16) != 0) {
                                    d.Data[9] = 2;
                                }
                            }
                            d.Data[8] = (byte)(d.Data[8] & 3);
                            parsedRow.Add("RawCh5", (short)((d.Data[8] << 8) + d.Data[9]));
                        }

                        //Ch6
                        if (d.Data.Length > 10) {
                            if (d.Data[11] == 3) {
                                if ((d.Data[10] & 16) != 0) {
                                    d.Data[11] = 2;
                                }
                            }
                            d.Data[10] = (byte)(d.Data[10] & 3);
                            parsedRow.Add("RawCh6", (short)((d.Data[10] << 8) + d.Data[11]));
                        }
                        
                        //Ch7
                        if (d.Data.Length > 12) {
                            if (d.Data[13] == 3) {
                                if ((d.Data[12] & 16) != 0) {
                                    d.Data[13] = 2;
                                }
                            }
                            d.Data[12] = (byte)(d.Data[12] & 3);
                            parsedRow.Add("RawCh7", (short)((d.Data[12] << 8) + d.Data[13]));
                        }

                        //Ch8
                        if (d.Data.Length > 14)
                        {
                            if (d.Data[15] == 3)
                            {
                                if ((d.Data[14] & 16) != 0)
                                {
                                    d.Data[15] = 2;
                                }
                            }
                            d.Data[14] = (byte)(d.Data[14] & 3);
                            parsedRow.Add("RawCh8", (short)((d.Data[14] << 8) + d.Data[15]));
                        }

                        break;

                    case(Code.Accelerometer):
                        parsedRow.Add("AccelCh1", (short)((d.Data[0] << 8) + d.Data[1]));
                        if (d.Data.Length > 2) {
                            parsedRow.Add("AccelCh2", (short)((d.Data[2] << 8) + d.Data[3]));
                        }
                        if (d.Data.Length > 4) {
                            parsedRow.Add("AccelCh3", (short)((d.Data[4] << 8) + d.Data[5]));
                        }
                        //Console.WriteLine("\tAccel");
                        //Console.WriteLine("\t{0}\t{1}\t{2}", parsedRow["AccelCh1"], parsedRow["AccelCh2"], parsedRow["AccelCh3"]);
                        break;
                    case(Code.EMGPower):
                        parsedRow.Add("EmgPower", BitConverter.ToSingle(ReverseBytes(d.Data), 0));
                        break;
                    case(Code.Offhead):
                        parsedRow.Add("OffheadCh1", (short)((d.Data[0] << 8) + d.Data[1]));
                        parsedRow.Add("OffheadCh3", (short)((d.Data[2] << 8) + d.Data[3]));
                        //Console.WriteLine("\tOffhead ------------");
                        //Console.WriteLine("\t{0}\t{1}", parsedRow["OffheadCh1"], parsedRow["OffheadCh3"]);
                        break;
                    case(Code.EEGPowerInt):
                        if (d.Data.Length == 24)
                        {
                            parsedRow.Add("EegPowerDelta", (uint)(d.Data[0] << 16) + d.Data[1] << 8 + d.Data[2]);
                            parsedRow.Add("EegPowerTheta", (uint)(d.Data[3] << 16) + d.Data[4] << 8 + d.Data[5]);
                            parsedRow.Add("EegPowerAlpha1", (uint)(d.Data[6] << 16) + d.Data[7] << 8 + d.Data[8]);
                            parsedRow.Add("EegPowerAlpha2", (uint)(d.Data[9] << 16) + d.Data[10] << 8 + d.Data[11]);
                            parsedRow.Add("EegPowerBeta1", (uint)(d.Data[12] << 16) + d.Data[13] << 8 + d.Data[14]);
                            parsedRow.Add("EegPowerBeta2", (uint)(d.Data[15] << 16) + d.Data[16] << 8 + d.Data[17]);
                            parsedRow.Add("EegPowerGamma1", (uint)(d.Data[18] << 16) + d.Data[19] << 8 + d.Data[20]);
                            parsedRow.Add("EegPowerGamma2", (uint)(d.Data[21] << 16) + d.Data[22] << 8 + d.Data[23]);
                        }
                        else
                        {
                            parsedRow.Add("EegPowerDelta", 0);
                            parsedRow.Add("EegPowerTheta", 0);
                            parsedRow.Add("EegPowerAlpha1", 0);
                            parsedRow.Add("EegPowerAlpha2", 0);
                            parsedRow.Add("EegPowerBeta1", 0);
                            parsedRow.Add("EegPowerBeta2", 0);
                            parsedRow.Add("EegPowerGamma1", 0);
                            parsedRow.Add("EegPowerGamma2", 0);
                        }
                        break;
                    case(Code.DongleStatus):
                        parsedRow.Add("DongleStatus", (double)d.Data[0]);
                        break;
                    case(Code.HeadsetDisconnect):
                        parsedRow.Add("HeadsetDisconnect", (int)((d.Data[0]<<8) + d.Data[1]));
                        break;
                    case (Code.HeadsetConnect):
                        parsedRow.Add("HeadsetConnect", (int)((d.Data[0] << 8) + d.Data[1]));
                        break;
                }
                // End "switch (d.Type)..."


                // Add this row to the growing List
                parsedData.Add(parsedRow);
            }
            // End "foreach (DataRow d in dataRowArray)..."


            // Assign the resulting array out to the class variable
            this.ParsedData = parsedData.ToArray();
   
        }
        // End "public void Read(DataRow[] dataRowArray)..."

        private byte[] ReverseBytes(byte[] inArray) {
          byte temp;
          int highCtr = inArray.Length - 1;

          for (int ctr = 0; ctr < inArray.Length / 2; ctr++) {
            temp = inArray[ctr];
            inArray[ctr] = inArray[highCtr];
            inArray[highCtr] = temp;
            highCtr -= 1;
          }
          return inArray;
        }
    }
    /*
     * End "New Parser code..."
     */
}
