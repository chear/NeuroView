using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
//using System.Data;

using NeuroSky.ThinkGear;
//using NeuroSky.ThinkGear.Parser;

namespace NeuroSky.NeuroView
{
    public class Form1 : System.Windows.Forms.Form
    {
        public LineGraph lineGraph0;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;

        private Connector tg_Connector;

        private System.ComponentModel.Container components = null;

        public Form1()
        {
            tg_Connector = new Connector();
            tg_Connector.DeviceConnected += new EventHandler(OnDeviceConnected);

            InitializeComponent();

        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lineGraph0 = new NeuroView.LineGraph();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lineGraph0
            // 
            this.lineGraph0.Location = new System.Drawing.Point(20, 20);
            this.lineGraph0.Name = "lineGraph0";
            this.lineGraph0.xAxisMax = 0.9;
            this.lineGraph0.xAxisMin = 0;
            this.lineGraph0.yAxisMax = 1;
            this.lineGraph0.yAxisMin = -1;
            this.lineGraph0.TabIndex = 0;
            this.lineGraph0.samplingRate = 10;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(20, 280);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(80, 24);
            this.button1.TabIndex = 1;
            this.button1.Text = "Connect";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(140, 280);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(80, 24);
            this.button2.TabIndex = 1;
            this.button2.Text = "Clear";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            //
            this.button3.Location = new System.Drawing.Point(260, 280);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(80, 24);
            this.button3.TabIndex = 1;
            this.button3.Text = "Record";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(800, 320);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.button1,
                                                                          this.button2,
																		  this.button3,
																		  this.lineGraph0});
            this.Name = "Form1";
            this.Text = "NeuroView Awesome";
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new Form1());
        }

        public int i = 0;

        private void button1_Click(object sender, System.EventArgs e)
        {
            double tempData = 0;
#if true
            tempData = Math.Sin(2*Math.PI*1*i/lineGraph0.samplingRate);
            DataPair tempDataPair;
            tempDataPair.timeStamp = (float)i/lineGraph0.samplingRate;
            tempDataPair.data      = (float)tempData;
#endif

            lineGraph0.Add( tempDataPair );

            i++;

            lineGraph0.Invalidate();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            i = 0;

            lineGraph0.Clear();

            lineGraph0.Invalidate();
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            if (!lineGraph0.recordDataFlag)
            {
                this.button3.Text = "Stop";
                lineGraph0.recordDataFlag = true;
            }
            else
            {
                this.button3.Text = "Record";
                lineGraph0.recordDataFlag = false;
            }

        }

        static void OnDeviceConnected(object sender, EventArgs e)
        {
            Connector.DeviceEventArgs de = (Connector.DeviceEventArgs)e;

            Console.WriteLine("New Headset Created!!! " + de.Device.PortName);

            de.Device.DataReceived += new EventHandler(OnDataReceived);

        }

        private void UpdateGraph()
        {

        }

        static void OnDataReceived(object sender, EventArgs e)
        {
            Device d = (Device)sender;
            Device.DataEventArgs de = (Device.DataEventArgs)e;

            DataRow[] tempDataRowArray = de.DataRowArray;
            Parsed parsedData = new Parsed();

            //Console.WriteLine("PortName: " + d.PortName + " HeadSetID: " + d.HeadsetID);

            MindSetParser mindSetParser = new MindSetParser();

            parsedData = mindSetParser.Read(de.DataRowArray);
#if true
            foreach (TimeStampData tsd in parsedData.Raw)
            {
                //Update
                //lineGraph0.Add(new DataPair(tsd.TimeStamp, tsd.Value));
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
