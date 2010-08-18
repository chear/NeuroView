using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuroSky.ThinkGear.Parser
{
    // TODO: Really have to rename this guy. Yikes. Recommend "ParsedData".
    public struct Parsed
    {
        public TimeStampData[] PoorSignalQuality;
        public TimeStampData[] Attention;
        public TimeStampData[] Meditation;

        public TimeStampData[] DampenedAttention;
        public TimeStampData[] DampenedMeditation;

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
                    case(Code.PoorSignal):
                        tempPoorSignal.Add(new TimeStampData(d.Time, (double)d.Data[0]));
                        break;
                    case(Code.Attention):
                        tempAttention.Add(new TimeStampData(d.Time, (double)d.Data[0]));
                        break;
                    case(Code.Meditation):
                        tempMeditation.Add(new TimeStampData(d.Time, (double)d.Data[0]));
                        break;
                    case(Code.DampenedAtt):
                        tempDampenedAttention.Add(new TimeStampData(d.Time, (double)d.Data[0]));
                        break;
                    case (Code.DampenedMed):
                        tempDampenedMeditation.Add(new TimeStampData(d.Time, (double)d.Data[0]));
                        break;
                    case(Code.Raw):
                        tempRaw.Add(new TimeStampData(d.Time, (short)((d.Data[0]<<8) + d.Data[1])));
                        break;
                    case(Code.EMGPower):                                                
                        tempEmgPower.Add(new TimeStampData(d.Time, BitConverter.ToSingle(ReverseBytes(d.Data), 0)));
                        break;
                    case(Code.EEGPowerInt):
                        tempPowerEEGData.Add(new PowerEEGData(d.Time, d.Data));
                        break;
                    case(Code.DongleStatus):
                        tempDongleStatus.Add(new TimeStampData(d.Time, (double)d.Data[0]));
                        break;
                    case(Code.HeadsetDisconnect):
                        tempHeadsetDisconnect.Add(new TimeStampData(d.Time, (int)((d.Data[0]<<8) + d.Data[1])));
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
            tempParsed.EMGPower = tempEmgPower.ToArray();
            return tempParsed;
        }

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
}
