#define ENABLE

using System;

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;

using System.Collections.Generic;

using System.Threading;



public struct DataPair {
    public double timeStamp;
    public double data;

    public DataPair(double t, double d) {
        timeStamp = t;
        data = d;
    }

}


namespace NeuroSky.MindView {

    public class LineGraph : System.Windows.Forms.UserControl {

        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.HScrollBar hScrollBar;
        private System.Windows.Forms.Timer maxFrameRateTimer;

        //TODO: Implement list of a list for multiple data graphing
        public List<List<DataPair>> dataPoints;

        public List<DataPair> data0;
        public List<DataPair> data1;
        public int frameHeight = 240;
        public int frameWidth = 760;
        public int samplingRate = 10;

        public double xAxisMax, xAxisMin; //In seconds 
        public double yAxisMax, yAxisMin;

        public bool RecordDataFlag = false;
        public bool SaveDataFlag = false;
        public string DataFolderString = @"C:\Data";
        public string FolderNameString = "Test";
        public string FileNameString = "FileName";
        public string FileHeaderString = "Test, Test";

        public bool hScrollBarInUse = false;

        private Size defaultSize;
        private int scrollBarTop;

        private int numberOfPoints;

        private Thread saveDataThread;

        public event EventHandler DataSavingFinished = delegate { };

        public void Add(DataPair p) {

            data0.Add(p);
        }

        public void Clear() {
            data0.Clear();

            //Initialize Horizontal Scrollbar
            hScrollBar.Visible = false;
            hScrollBar.Maximum = 1;
            hScrollBar.Minimum = 0;
            hScrollBar.Value = hScrollBar.Maximum - 1;
            hScrollBarInUse = false;
        }

        public void FitAllData() {
            xAxisMax = (data0.Count) / (double)samplingRate;
        }

        protected override void OnPaint(PaintEventArgs pe) {

            Graphics drawingSurface = pe.Graphics;
            Pen myPen = new Pen(Color.Blue, 1);
            Rectangle rect = this.ClientRectangle;

            frameWidth = rect.Right - rect.Left;
            frameHeight = rect.Bottom - rect.Top;

            numberOfPoints = (int)Math.Abs(((xAxisMax - xAxisMin) * samplingRate)) + 1;
            double timeStampOffset = 0;

            /*Makes sures that the graph has at least two points to graph a line*/
            if ((data0 != null) && (data0.Count > 1)) {
                /*If recording is not enable trim the excess values*/
                if (!this.RecordDataFlag) {
                    if (this.SaveDataFlag) {
                        if (saveDataThread == null || !saveDataThread.IsAlive) {
                            saveDataThread = new Thread(this.SaveData);
                            saveDataThread.Start();
                        }
                    }
                    else {
                        hScrollBar.Visible = false;
                        while (data0.Count > numberOfPoints) {
                            data0.RemoveAt(0);
                        }
                    }
                }

                timeStampOffset = DrawGraph(data0.ToArray(), drawingSurface, myPen);
            }

            //TODO: Draw Axis
            //if (timeStampOffset < 0) timeStampOffset = 0;
            DrawYAxis(drawingSurface);

            myPen.Dispose();

        }

        private double DrawGraph(DataPair[] data, Graphics drawingSurface, Pen myPen) {
            int graphStartIndex = 0;
            double timeStampOffset = -1;
            int d = 0;

            /*If there is more points enable the scrollbar*/
            if (data.Length > numberOfPoints) {
                hScrollBar.Visible = true;
                frameHeight = frameHeight - hScrollBar.Height;
                hScrollBar.Maximum = data.Length - numberOfPoints + 9;

                if (!hScrollBarInUse) {
                    hScrollBar.Value = data.Length - numberOfPoints;
                }

                graphStartIndex = hScrollBar.Value;
            }
            else {
                graphStartIndex = 0;
            }

            int numberOfPointsToGraph = 0;

            if (numberOfPoints > data.Length) {
                numberOfPointsToGraph = data.Length;
            }
            else {
                numberOfPointsToGraph = numberOfPoints;
            }

            timeStampOffset = data[graphStartIndex].timeStamp;

            Point[] graphPoints = new Point[numberOfPointsToGraph];
            for (int i = 0; i < numberOfPointsToGraph; i++) {
                d = i + graphStartIndex;

                try {
                    graphPoints[i] = Point2Pixel((data[d].timeStamp - timeStampOffset), data[d].data);

                }
                catch (Exception e) {
                    Console.WriteLine("LineGraph: " + e.Message);
                }

            }

            drawingSurface.DrawLines(myPen, graphPoints);

            return timeStampOffset;
        }

        private void DrawYAxis(Graphics drawingSurface) {
            Pen myPen = new Pen(Color.Black);

            int locationX = 20;

            //10 Lines
            int temp = (int)frameHeight/10;

            for (int i = 0; i < 10; i++) {
                drawingSurface.DrawLine(myPen, locationX, temp + temp*i, locationX + 10, temp + temp*i);
            }

            myPen.Dispose();
        }

        private void SaveData() {
            DataPair[] dataPointsCopy = data0.ToArray();
            //Console.WriteLine("Number of datapoints right before saving: " + dataPointsCopy.Length);

            SaveDataFlag = false;

            string newPath = System.IO.Path.Combine(DataFolderString, FolderNameString);

            System.IO.Directory.CreateDirectory(newPath);

            newPath = System.IO.Path.Combine(newPath, FileNameString);

            if (!System.IO.File.Exists(newPath)) {
                using (System.IO.StreamWriter fs = new System.IO.StreamWriter(newPath, false)) {
                    /*Write the header to the csv file*/
                    fs.WriteLine(FileHeaderString);

                    /*Write the data to the csv file*/
                    foreach (DataPair d in dataPointsCopy) {
                        fs.WriteLine(d.timeStamp + ", " + d.data);
                    }

                }
            }

            DataSavingFinished(this, EventArgs.Empty);
        }

        public LineGraph() {
            defaultSize = new Size(frameWidth, frameHeight);

            InitializeComponent();

            this.DoubleBuffered = true;

            data0 = new List<DataPair>();

            /*Setting up the timer for the max frame rate*/
            maxFrameRateTimer = new System.Windows.Forms.Timer();
            maxFrameRateTimer.Interval = 25; //In milliseconds
            maxFrameRateTimer.Tick += new EventHandler(MaxFrameRateTimer_Tick);
            maxFrameRateTimer.Start();


        }

        public void MaxFrameRateTimer_Tick(object sender, EventArgs e) {
            this.Invalidate();
        }

        private Point Point2Pixel(double xValue, double yValue) {
            Point temp = new Point();

            temp.X = (int)((xValue - xAxisMin) / (xAxisMax - xAxisMin) * frameWidth);
            temp.Y = (int)((yValue - yAxisMin) / (yAxisMax - yAxisMin) * frameHeight);
            temp.Y = frameHeight - temp.Y;

            return temp;

        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        private void InitializeComponent() {
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.SuspendLayout();

            this.Name = "LineGraph";
            this.Size = defaultSize;
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;

            scrollBarTop = frameHeight - this.hScrollBar.Height;
            this.hScrollBar.Location = new System.Drawing.Point(0, scrollBarTop);
            this.hScrollBar.Width = this.Width;
            this.hScrollBar.Visible = false;
            this.hScrollBar.Maximum = 1;
            this.hScrollBar.Minimum = 0;
            this.hScrollBar.Value = this.hScrollBar.Maximum - 1;
            this.hScrollBar.ValueChanged += new System.EventHandler(this.hScrollBar_ValueChanged);

            this.Controls.AddRange(new System.Windows.Forms.Control[] { this.hScrollBar });

            this.ResumeLayout(false);
        }
        #endregion

        public int scrollCounter = 0;

        private void hScrollBar_ValueChanged(object sender, System.EventArgs e) {
            //Console.WriteLine("HSCROLLBAR VALUE CHANGED. " + scrollCounter++ );

            if (hScrollBar.Value >= Math.Ceiling((data0.Count - numberOfPoints) * 0.99)) {
                hScrollBarInUse = false;
            }
            else {
                hScrollBarInUse = true;
            }

            this.Invalidate();
#if false

            hScrollBarInUse = true;

            if (hScrollBar.Value >= hScrollBar.Maximum - numberOfPoints + 1)
            {
                hScrollBarInUse = false;
            }
            else
            {
                this.Invalidate();
            }
#endif
        }

        protected override void OnSizeChanged(EventArgs e) {

            /*Update Location and dimension*/
            scrollBarTop = this.Height - this.hScrollBar.Height;
            this.hScrollBar.Location = new System.Drawing.Point(0, scrollBarTop);
            this.hScrollBar.Width = this.Width;

            base.OnSizeChanged(e);

        }

        public void OptimizeScrollBar() {
            numberOfPoints = (int)Math.Abs(((xAxisMax - xAxisMin) * samplingRate)) + 1;
            this.hScrollBar.LargeChange = numberOfPoints / 8;
        }

        public double Mean() {
            double sum = 0;
            int i = 0;

            foreach (DataPair d in data0) {
                i++;
                sum += d.data;
            }

            double mean = sum / i;

            return mean;
        }

        public double Max() {
            double max = 0;

            if (data0.Count > 0) {
                max = data0[0].data;

                foreach (DataPair d in data0) {
                    if (d.data > max) {
                        max = d.data;
                    }
                }
            }

            return max;
        }


    }/*End of LinGraph Class*/

}
