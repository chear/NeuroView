using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

using System.Windows.Forms;

namespace NeuroSky.MindView
{
    // Possible types of device
    public enum DeviceType
    {
        MindSet = 0,            // Original MindSet
        Circlet = 1,            // Circlet, 1 Channel, headband form factor
        Bandana = 2,            // Bandana, 2 Channel on MSB, nothing else
        BandanaDeluxeP300 = 3,  // Bandana, 3 Channel on MSB, 3 Accel, 2 Offhead, Sleep Firmware  
        BandanaDeluxeMed = 4    // Bandana, 3 Channel on MSB, Meditation FirmWare
    };

    // Possible types of data
    public enum DataType
    {
        EEG = 1,        // Raw EEG
        Accel = 2,      // Accelerometer
        Offhead = 3,    // Offhead score
        Attn = 4,       // eSense Attention
        Med = 5,        // eSense Meditation
        Pwr = 6         // Power of EEG signal
    };

    // Possible plotting types
    public enum PlotType
    {
        Line = 1,       // Line graphs
        Bar = 2         // Bar graphs
    }

    public class GraphPanel: Panel
    {
        public LineGraph LineGraph;
        public BarGraph BarGraph;
        public Label Label;
        public Label ValueLabel;
        public DeviceType DeviceType;
        public DataType DataType;
        public PlotType PlotType;

        private System.ComponentModel.Container components = null;
        private Timer ValueUpdateTimer;

        public event EventHandler DataSavingFinished = delegate { };

        // Defaults to a line plot
        public GraphPanel() : this(PlotType.Line)
        {
        }

        public GraphPanel(PlotType plotType)
        {
            InitializeComponent(plotType);

            if (this.PlotType == PlotType.Line) {
                LineGraph.DataSavingFinished += new EventHandler(LineGraph_DataSavingFinished);
            }

            ValueUpdateTimer = new Timer();
            ValueUpdateTimer.Interval = 200; //In milliseconds.
            ValueUpdateTimer.Tick += new EventHandler(ValueUpdateTimer_Tick);
        }

        public void EnableValueDisplay()
        {
            ValueUpdateTimer.Start();
        }

        void LineGraph_DataSavingFinished(object sender, EventArgs e)
        {
            DataSavingFinished(this, EventArgs.Empty);
        }

        public void ValueUpdateTimer_Tick(object sender, EventArgs e)
        {
            // Only do this if it is a line plot
            if (this.PlotType == PlotType.Line) {
                int lastIndex = this.LineGraph.data0.Count - 1;
                if (lastIndex >= 0)
                {
                    this.ValueLabel.Text = this.LineGraph.data0[lastIndex].data.ToString();
                }
            }
        }

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
        
        // Set parameters relating to device and data type
        public void SetTypes(DeviceType deviceType, DataType dataType)
        {
            // Catch DeviceType exceptions
            switch (deviceType)
            {
                case DeviceType.MindSet:
                    break;
                case DeviceType.Circlet:
                    break;
                case DeviceType.Bandana:
                    break;
                case DeviceType.BandanaDeluxeP300:
                    break;
                case DeviceType.BandanaDeluxeMed:
                    break;
                default:
                    // Default to Bandana
                    // Rerun this function
                    SetTypes(DeviceType.Bandana, dataType);
                    break;
            }

            // Save out the device type
            this.DeviceType = deviceType;


            // Set parameters for the device and data types
            switch (dataType)
            {
                // EEG
                case DataType.EEG:
                    switch (deviceType)
                    {
                        case DeviceType.MindSet:
                            LineGraph.samplingRate = 512;
                            LineGraph.yAxisMax = 2047;
                            LineGraph.yAxisMin = -2048;
                            break;
                        case DeviceType.Circlet:
                            LineGraph.samplingRate = 512;
                            LineGraph.yAxisMax = 2047;
                            LineGraph.yAxisMin = -2048;
                            break;
                        case DeviceType.Bandana:
                            LineGraph.samplingRate = 256;
                            LineGraph.yAxisMax = 2047;
                            LineGraph.yAxisMin = -2048;
                            break;
                        case DeviceType.BandanaDeluxeP300:
                            LineGraph.samplingRate = 256;
                            LineGraph.yAxisMax = 2047;
                            LineGraph.yAxisMin = -2048;
                            break;
                        case DeviceType.BandanaDeluxeMed:
                            LineGraph.samplingRate = 128;
                            LineGraph.yAxisMax = 2047;
                            LineGraph.yAxisMin = -2048;
                            break;
                        default:
                            SetTypes(DeviceType.Bandana, DataType.EEG);
                            break;
                    }
                    break;

                // Accelerometer
                case DataType.Accel:
                    switch (deviceType)
                    {
                        case DeviceType.BandanaDeluxeP300:
                            LineGraph.samplingRate = 85;
                            LineGraph.yAxisMax = 4100;
                            LineGraph.yAxisMin = 1000;
                            break;
                        default:
                            SetTypes(DeviceType.Bandana, DataType.EEG);
                            break;
                    }
                    break;

                // Offhead
                case DataType.Offhead:
                    switch (deviceType)
                    {
                        case DeviceType.BandanaDeluxeP300:
                            LineGraph.samplingRate = 1;
                            LineGraph.yAxisMax = 4100;
                            LineGraph.yAxisMin = 0;
                            break;
                        default:
                            SetTypes(DeviceType.Bandana, DataType.EEG);
                            break;
                    }
                    break;

                // eSense Attention
                case DataType.Attn:
                    switch (deviceType)
                    {
                        case DeviceType.MindSet:
                            LineGraph.samplingRate = 1;
                            LineGraph.yAxisMax = 105;
                            LineGraph.yAxisMin = -5;
                            break;
                        default:
                            SetTypes(DeviceType.Bandana, DataType.EEG);
                            break;
                    }
                    break;

                // eSense Meditation
                case DataType.Med:
                    switch (deviceType)
                    {
                        case DeviceType.MindSet:
                            LineGraph.samplingRate = 1;
                            LineGraph.yAxisMax = 105;
                            LineGraph.yAxisMin = -5;
                            break;
                        default:
                            SetTypes(DeviceType.Bandana, DataType.EEG);
                            break;
                    }
                    break;

                case DataType.Pwr:
                    switch (deviceType)
                    {
                        case DeviceType.MindSet:
                            BarGraph.samplingRate = 512;
                            break;
                        case DeviceType.Circlet:
                            BarGraph.samplingRate = 512;
                            break;
                        case DeviceType.Bandana:
                            BarGraph.samplingRate = 256;
                            break;
                        case DeviceType.BandanaDeluxeP300:
                            BarGraph.samplingRate = 256;
                            break;
                        case DeviceType.BandanaDeluxeMed:
                            BarGraph.samplingRate = 128;
                            break;
                        default:
                            SetTypes(DeviceType.Bandana, DataType.EEG);
                            break;
                    }
                    break;

                // Default
                // Bailout to just a Bandana spitting out EEG data
                default:
                    SetTypes(DeviceType.Bandana, DataType.EEG);
                    break;
            }

            // Save out the data type
            this.DataType = dataType;


            // Set other default settings for the line and bar plots
            if (this.PlotType == PlotType.Line) {
                LineGraph.xAxisMax = 4;
                LineGraph.xAxisMin = 0;
                LineGraph.OptimizeScrollBar();
            } else if (this.PlotType == PlotType.Bar) {
                BarGraph.xAxisMax = 60;
                BarGraph.xAxisMin = 1;
                BarGraph.yAxisMax = 25;
                BarGraph.yAxisMin = 0;
                BarGraph.pwrSpecWindow = 1;
            }
            EnableValueDisplay();
        }
        // End "public void SetType(...)"

        #region Windows Form Designer generated code
        private void InitializeComponent(PlotType plotType)
        {
            if (plotType == PlotType.Line) {
                this.LineGraph = new NeuroSky.MindView.LineGraph();
                this.PlotType = PlotType.Line;
            } else if (plotType == PlotType.Bar) {
                this.BarGraph = new NeuroSky.MindView.BarGraph();
                this.PlotType = PlotType.Bar;
            }
            this.Label = new System.Windows.Forms.Label();
            this.ValueLabel = new System.Windows.Forms.Label(); 
            this.SuspendLayout();
            // 
            // lineGraph
            // 
            if (plotType == PlotType.Line) {
                this.BorderStyle = BorderStyle.FixedSingle;
                this.LineGraph.Location = new System.Drawing.Point(100, 0);
                this.LineGraph.Name = "lineGraph";
                this.LineGraph.Size = new System.Drawing.Size(700, 200);
                this.LineGraph.TabIndex = 0;
            }
            //
            // BarGraph
            //
            if (plotType == PlotType.Bar) {
                this.BorderStyle = BorderStyle.FixedSingle;
                this.BarGraph.Location = new System.Drawing.Point(100, 0);
                this.BarGraph.Name = "barGraph";
                this.BarGraph.Size = new System.Drawing.Size(700, 200);
                this.BarGraph.TabIndex = 0;
            }
            // 
            // Label
            // 
            this.Label.Location = new System.Drawing.Point(0, 0);
            this.Label.Name = "label";
            this.Label.Size = new System.Drawing.Size(101, 200);
            this.Label.TextAlign = ContentAlignment.TopCenter;
            this.Label.TabIndex = 0;
            this.Label.Font = new Font(Label.Font, FontStyle.Bold);
            this.Label.Text = "Label";
            this.Label.BorderStyle = BorderStyle.FixedSingle;
            // 
            // ValueLabel
            // 
            this.ValueLabel.Location = new System.Drawing.Point(10,20);
            this.ValueLabel.Text = " ";
            this.ValueLabel.Size = new System.Drawing.Size(80, 20);
            this.ValueLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // GraphPanel
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Size = new System.Drawing.Size(800, 200);
            this.Controls.Add(this.ValueLabel);
            this.Controls.Add(this.Label);
            if (plotType == PlotType.Line) {
                this.Controls.Add(this.LineGraph);
            } else if (plotType == PlotType.Bar) {
                this.Controls.Add(this.BarGraph);
            }
            this.ResumeLayout(false);

        }
        #endregion

        protected override void OnSizeChanged(EventArgs e)
        {
            //Adjust the Height
            this.Label.Height = this.Size.Height;
            if (this.PlotType == PlotType.Line) {
                this.LineGraph.Height = this.Size.Height;
            } else if (this.PlotType == PlotType.Bar) {
                this.BarGraph.Height = this.Size.Height;
            }

            //Adjust the Width
            this.Label.Width = 101;
            if (this.PlotType == PlotType.Line) {
                this.LineGraph.Width = this.Size.Width - 100;
            } else if (this.PlotType == PlotType.Bar) {
                this.BarGraph.Width = this.Size.Width - 100;
            }

            //Adjust location of ValueLabel
            this.ValueLabel.Left = 10;
            //this.ValueLabel.Top = this.Size.Height*2/3;
            this.ValueLabel.Top = 20;

            base.OnSizeChanged(e);
            
        }
        
    }
}
