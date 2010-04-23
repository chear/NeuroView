using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
//using System.Data;

using NeuroSky.ThinkGear;
using NeuroSky.ThinkGear.Parser;

namespace NeuroSky.NeuroView
{
    public class Form1 : System.Windows.Forms.Form
    {

        public GraphPanel attGraphPanel;
        public GraphPanel medGraphPanel;
        public GraphPanel rawGraphPanel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;

        private Connector tg_Connector;

        private System.ComponentModel.Container components = null;

        public int timeStampIndex = 0;

        public Form1()
        {
            tg_Connector = new Connector();
            tg_Connector.DeviceConnected += new EventHandler(OnDeviceConnected);

            InitializeComponent();

            attGraphPanel.Label.Text = "attGraphPanel";
            attGraphPanel.LineGraph.samplingRate = 1;
            attGraphPanel.LineGraph.xAxisMax = 4;
            attGraphPanel.LineGraph.xAxisMin = 0;
            attGraphPanel.LineGraph.yAxisMax = 105;
            attGraphPanel.LineGraph.yAxisMin = -5;

            attGraphPanel.Label.Text = "attGraphPanel";
            medGraphPanel.LineGraph.samplingRate = 1;
            medGraphPanel.LineGraph.xAxisMax = 4;
            medGraphPanel.LineGraph.xAxisMin = 0;
            medGraphPanel.LineGraph.yAxisMax = 105;
            medGraphPanel.LineGraph.yAxisMin = -5;

            rawGraphPanel.LineGraph.samplingRate = 512;
            rawGraphPanel.LineGraph.xAxisMax = 4;
            rawGraphPanel.LineGraph.xAxisMin = 0;
            rawGraphPanel.LineGraph.yAxisMax = 2047;
            rawGraphPanel.LineGraph.yAxisMin = -2048;

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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.attGraphPanel = new NeuroSky.NeuroView.GraphPanel();
            this.medGraphPanel = new NeuroSky.NeuroView.GraphPanel();
            this.rawGraphPanel = new NeuroSky.NeuroView.GraphPanel();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(20, 660);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(80, 24);
            this.button1.TabIndex = 1;
            this.button1.Text = "Connect";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(140, 660);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(80, 24);
            this.button2.TabIndex = 1;
            this.button2.Text = "Clear";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(260, 660);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(80, 24);
            this.button3.TabIndex = 1;
            this.button3.Text = "Record";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // attGraphPanel
            // 
            this.attGraphPanel.Location = new System.Drawing.Point(0, 50);
            this.attGraphPanel.Name = "attGraphPanel";
            this.attGraphPanel.Size = new System.Drawing.Size(800, 200);
            this.attGraphPanel.TabIndex = 0;
            // 
            // medGraphPanel
            // 
            this.medGraphPanel.Location = new System.Drawing.Point(0, 250);
            this.medGraphPanel.Name = "medGraphPanel";
            this.medGraphPanel.Size = new System.Drawing.Size(800, 200);
            this.medGraphPanel.TabIndex = 0;
            // 
            // rawGraphPanel
            // 
            this.rawGraphPanel.Location = new System.Drawing.Point(0, 450);
            this.rawGraphPanel.Name = "rawGraphPanel";
            this.rawGraphPanel.Size = new System.Drawing.Size(800, 200);
            this.rawGraphPanel.TabIndex = 0;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(800, 740);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.attGraphPanel);
            this.Controls.Add(this.medGraphPanel);
            this.Controls.Add(this.rawGraphPanel);
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
            this.button1.Enabled = false;
            tg_Connector.Connect("COM4");

#if false
            double tempData = 0;

            tempData = Math.Sin(2*Math.PI*1*i/lineGraph0.samplingRate);
            DataPair tempDataPair;
            tempDataPair.timeStamp = (float)i/lineGraph0.samplingRate;
            tempDataPair.data      = (float)tempData;


            lineGraph0.Add( tempDataPair );

            i++;

            lineGraph0.Invalidate();
#endif
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            rawGraphPanel.LineGraph.Clear();
            medGraphPanel.LineGraph.Clear();
            attGraphPanel.LineGraph.Clear();

            i = 0;
            timeStampIndex = 0;

            rawGraphPanel.LineGraph.Invalidate();
            medGraphPanel.LineGraph.Invalidate();
            attGraphPanel.LineGraph.Invalidate();
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
#if false
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
#endif
        }

        void OnDeviceConnected(object sender, EventArgs e)
        {
            Connector.DeviceEventArgs de = (Connector.DeviceEventArgs)e;

            Console.WriteLine("New Headset Created!!! " + de.Device.PortName);

            de.Device.DataReceived += new EventHandler(OnDataReceived);

        }

        void OnDataReceived(object sender, EventArgs e)
        {
            Device d = (Device)sender;
            Device.DataEventArgs de = (Device.DataEventArgs)e;

            DataRow[] tempDataRowArray = de.DataRowArray;
            Parsed parsedData = new Parsed();

            MindSetParser mindSetParser = new MindSetParser();

            parsedData = mindSetParser.Read(de.DataRowArray);
#if true
            foreach (TimeStampData tsd in parsedData.Raw)
            {
                rawGraphPanel.LineGraph.Add(new DataPair((timeStampIndex / (double)rawGraphPanel.LineGraph.samplingRate), tsd.Value));
                timeStampIndex++;
                i++;
            }
#endif

#if true
            foreach (TimeStampData tsd in parsedData.Attention)
            {
                attGraphPanel.LineGraph.Add(new DataPair((timeStampIndex / (double)rawGraphPanel.LineGraph.samplingRate), tsd.Value));
            }

            foreach (TimeStampData tsd in parsedData.Meditation)
            {
                medGraphPanel.LineGraph.Add(new DataPair((timeStampIndex / (double)rawGraphPanel.LineGraph.samplingRate), tsd.Value));
            }
#endif
            if (i > 40)
            {
                i = 0;
                rawGraphPanel.LineGraph.Invalidate();
                attGraphPanel.LineGraph.Invalidate();
                medGraphPanel.LineGraph.Invalidate();
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
