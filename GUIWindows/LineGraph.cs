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





namespace NeuroSky.MindView {

    public class LineGraph : System.Windows.Forms.UserControl {

        private System.ComponentModel.Container components = null;
        public System.Windows.Forms.HScrollBar hScrollBar;
        private System.Windows.Forms.Timer maxFrameRateTimer;

        //TODO: Implement list of a list for multiple data graphing
        public List<List<DataPair>> dataPoints;

        public List<DataPair> data0;          //store the data
        public DataPair[] data0noDC;          //store the DC removed data into this array

        public int frameHeight = 240;
        public int frameWidth = 760;
        public int samplingRate = 10;

        public double xAxisMax, xAxisMin; //In seconds 
        public double yAxisMax, yAxisMin;
        public int timeStampIndex;

        public bool RecordDataFlag = false;
        public bool SaveDataFlag = false;
        public string DataFolderString = @"C:\Data";
        public string FolderNameString = "Test";
        public string FileNameString = "FileName";
        public string FileHeaderString = "Test, Test";
        public string graphName = "ECG";

        public bool hScrollBarInUse = false;

        private Size defaultSize;
        private int scrollBarTop;

        private int numberOfPoints;

        private int DCOffsetCounter;

        public bool DCRemovalEnabled = false;

        private double averageValue;

        private Thread saveDataThread;

        public event EventHandler DataSavingFinished = delegate { };


        public void Add(DataPair p) 
        {
            lock (data0)
            {
                data0.Add(p);
            }

            /*
            if(DCRemovalEnabled)
            {
                removeDCOffset();
            }
            */
        }

        public void Clear() {
            lock (data0)
            {
                data0.Clear();
            }

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


        public void removeDCOffset()
        {
            DCOffsetCounter++;

            //run this function every 0.5 seconds
            if(DCOffsetCounter >= 0)
            {
                lock(data0)
                {
                    if(data0.Count > numberOfPoints)
                    {
                        data0noDC = new DataPair[numberOfPoints];
                        data0.GetRange(data0.Count - numberOfPoints, numberOfPoints).CopyTo(data0noDC);
                    } else
                    {
                        data0noDC = new DataPair[data0.Count];
                        data0.CopyTo(data0noDC);
                    }
                }

                //loop through all the values to calculate the average
                averageValue = 0;
                for(int i = 0; i < data0noDC.Length; i++)
                {
                    averageValue = averageValue + data0noDC[i].data;
                }
                averageValue = (averageValue / data0noDC.Length);

                //loop through all the values and subtract the average
                for(int i = 0; i < data0noDC.Length; i++)
                {
                    data0noDC[i].data = data0noDC[i].data - averageValue;
                }

                //reset the counter
                DCOffsetCounter = 0;
            }
        }


        protected override void OnPaint(PaintEventArgs pe)
        {

            Graphics drawingSurface = pe.Graphics;
            Pen myPen = new Pen(Color.Blue, 1);
            Rectangle rect = this.ClientRectangle;

            frameWidth = rect.Right - rect.Left;
            frameHeight = rect.Bottom - rect.Top;

            numberOfPoints = (int)Math.Abs(((xAxisMax - xAxisMin) * samplingRate)) + 1;
            double timeStampOffset = 0;

            lock (data0)
            {
                /*Makes sures that the graph has at least two points to graph a line*/
                if ((data0 != null) && (data0.Count > 1))
                {
                    /*If recording is not enable trim the excess values*/
                    if (!this.RecordDataFlag)
                    {
                        if (this.SaveDataFlag)
                        {
                            if (saveDataThread == null || !saveDataThread.IsAlive)
                            {
                                saveDataThread = new Thread(this.SaveData);
                                saveDataThread.Start();
                            }
                        }
                        else
                        {
                            hScrollBar.Visible = false;
                            if (data0.Count > numberOfPoints * 3)     //shift the buffer every once in a while (more efficient)
                            {
                                data0.RemoveRange(0, data0.Count - numberOfPoints);
                            }
                        }
                    }

                    if(DCRemovalEnabled)
                    {
                        removeDCOffset();
                        timeStampOffset = DrawGraph(data0noDC, drawingSurface, myPen);
                    } else
                    {
                        if(data0.Count > numberOfPoints)
                        {
                            timeStampOffset = DrawGraph(data0.GetRange(data0.Count - numberOfPoints, numberOfPoints).ToArray(), drawingSurface, myPen);
                        } else
                        {
                            timeStampOffset = DrawGraph(data0.ToArray(), drawingSurface, myPen);
                        }
                    }
                }
            }

            //TODO: Draw Axis
            //if (timeStampOffset < 0) timeStampOffset = 0;
            DrawXAxis(drawingSurface);
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


        /**
         * Labels the x-axis with frequency bins
         */
        private void DrawXAxis(Graphics drawingSurface) {
            Pen myPen = new Pen(Color.Black);
            SolidBrush myBrush = new SolidBrush(Color.Black);
            System.Drawing.Font myFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);

            Point pt;
            float X;
            float Xoffset = 0;

            float YtickLength = 15;
            float Y1 = frameHeight;
            float Y2 = Y1 - YtickLength;

            double stepSize = 0;
            int numGroups = 0;


            // Set number of labels to create
            if (xAxisMax >= 100)
            {
                stepSize = 20;
            }
            else if (xAxisMax >= 50)
            {
                stepSize = 10;
            }
            else if (xAxisMax >= 10)
            {
                stepSize = 2;
            }
            else if (xAxisMax >= 5)
            {
                stepSize = 1;
            }
            else if (xAxisMax >= 2)
            {
                stepSize = 0.5;
            }
            else if (xAxisMax >= 1)
            {
                stepSize = 0.2;
            }
            else
            {
                stepSize = 0.1;
            }

            numGroups = (int)(xAxisMax / stepSize);


            // Write the labels
            for (int i = 1; i < numGroups; i++)
            {
                X = (float)(i * stepSize) + Xoffset;
                pt = Point2Pixel(X, 0);
                drawingSurface.DrawLine(myPen, pt.X, Y1, pt.X, Y2);
                drawingSurface.DrawString(X.ToString(), myFont, myBrush, frameWidth - pt.X, Y2 - 2);
            }

            myPen.Dispose();
            myBrush.Dispose();
        }

        /**
         * Labels the y-axis with amplitudes
         */
        private void DrawYAxis(Graphics drawingSurface) {
            Pen myPen = new Pen(Color.Black);
            
            int X = frameWidth;
            int Xwide = -10;
            int numLines = 10;
            double Ystep = Math.Ceiling((double)(frameHeight) / (double)(numLines));

            for (int i = numLines - 1; i > 0; i--)
            {
                drawingSurface.DrawLine(myPen, X, frameHeight - ((int)Ystep * i), (X + Xwide), frameHeight - ((int)Ystep * i));
            }

            myPen.Dispose();
            SolidBrush myBrush2 = new SolidBrush(Color.Black);
            System.Drawing.Font myFont2 = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            drawingSurface.DrawString(yAxisMax.ToString(), myFont2, myBrush2, X+(Xwide*5), 2);
            drawingSurface.DrawString(yAxisMin.ToString(), myFont2, myBrush2, X+(Xwide*5), frameHeight - 20);
            myBrush2.Dispose();
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

            this.Size = defaultSize;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.scrollBarTop = frameHeight - this.hScrollBar.Height;
            this.hScrollBar.Location = new System.Drawing.Point(0, scrollBarTop);
            this.DoubleBuffered = true;

            data0 = new List<DataPair>();
            data0noDC = new DataPair[512];

            /*Setting up the timer for the max frame rate*/
            maxFrameRateTimer = new System.Windows.Forms.Timer();
            maxFrameRateTimer.Interval = 40; //In milliseconds
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
            this.BackColor = Color.White;

           
            this.hScrollBar.Width = this.Width;
            this.hScrollBar.Visible = false;
            this.hScrollBar.Maximum = 1;
            this.hScrollBar.Minimum = 0;
            this.hScrollBar.Value = 0;
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


    }/*End of LineGraph Class*/

}


public struct DataPair {
    public double timeStamp;
    public double data;

    public DataPair(double t, double d) {
        timeStamp = t;
        data = d;
    }
}