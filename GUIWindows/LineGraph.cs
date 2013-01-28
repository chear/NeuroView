//NEW VERSION

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

        public List<DataPair> data0;            //store the data
        public List<DataPair> runningBuffer;    //continuously updated buffer

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

        public int numberOfPoints;

        private double conversionFactor = 0.0183;
        private int    gain             = 128;

        private int DCOffsetCounter;
        public bool DCRemovalEnabled;
        private double DCOffset = 0;
        private int roundPrecision = 300;    //number of digits precision. ie 500 means round to +/- 500

        private Thread saveDataThread;

        public event EventHandler DataSavingFinished = delegate { };


        public void Add(DataPair p) {
            lock(data0) {
                data0.Add(p);
            }

            //if DC removal is enabled, keep a continuous log of raw data and
            //use it to calculate the DC offset
            if(DCRemovalEnabled) {
                runningBuffer.Add(p);

                //shift the buffer every once in a while (more efficient)
                if(runningBuffer.Count > numberOfPoints * 3) {    
                    runningBuffer.RemoveRange(0, runningBuffer.Count - numberOfPoints);
                }

                calculateDCOffset();
            }
        }
            
        //remove all the elements from data0            
        public void Clear() {
            lock (data0)
            {
                data0.Clear();
            }
            timeStampIndex = 0;

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


        public void calculateDCOffset() {
            DCOffsetCounter++;

            //run this function every .5 seconds
            if(DCOffsetCounter >= samplingRate / 16) {
                DCOffset = 0;
                lock(runningBuffer) {
                    if(runningBuffer.Count > numberOfPoints) {
                        for(int i = runningBuffer.Count - numberOfPoints; i < runningBuffer.Count; i++) {
                            DCOffset = DCOffset + runningBuffer[i].data;
                        }

                    } else {
                        for(int i = 0; i < runningBuffer.Count; i++) {
                            DCOffset = DCOffset + runningBuffer[i].data;
                        }
                    }
                }

                //calculate the DCOffset
                DCOffset = customRound(DCOffset / numberOfPoints);

                //reset the counter
                DCOffsetCounter = 0;
            }
        }

        private double customRound(double value)
        {
            value = Math.Round(value / roundPrecision);
            value = (value * roundPrecision);
            return value;
        }


        protected override void OnPaint(PaintEventArgs pe)
        {

            Graphics drawingSurface = pe.Graphics;
            Pen myPen = new Pen(Color.Black, 2);
            Rectangle rect = this.ClientRectangle;

            frameWidth = rect.Right - rect.Left;
            frameHeight = rect.Bottom - rect.Top;

            numberOfPoints = (int)Math.Abs(((xAxisMax - xAxisMin) * samplingRate));
            double timeStampOffset = 0;

            DrawXAxis(drawingSurface);
            DrawXAxisMinor(drawingSurface);
            DrawYAxis(drawingSurface);
            DrawYAxisMinor(drawingSurface);

            lock(data0) {
                /*Makes sures that the graph has at least two points to graph a line*/
                if((data0 != null) && (data0.Count > 1)) {
                    /*If recording is not enable trim the excess values*/
                    if(!this.RecordDataFlag) {
                        if(this.SaveDataFlag) {
                            if(saveDataThread == null || !saveDataThread.IsAlive) {
                                saveDataThread = new Thread(this.SaveData);
                                saveDataThread.Start();
                            }
                        } else {
                            hScrollBar.Visible = false;
                            if(data0.Count > numberOfPoints * 3)     //shift the buffer every once in a while (more efficient)
                            {
                                data0.RemoveRange(0, data0.Count - numberOfPoints);
                            }
                        }
                    }

                    if(data0.Count > numberOfPoints) {
                        timeStampOffset = DrawGraph(data0.GetRange(data0.Count - numberOfPoints, numberOfPoints).ToArray(), drawingSurface, myPen);
                    } else {
                        timeStampOffset = DrawGraph(data0.ToArray(), drawingSurface, myPen);
                    }

                }
            }

 

            myPen.Dispose();

        }

        private double DrawGraph(DataPair[] data, Graphics drawingSurface, Pen myPen) {
            int graphStartIndex = 0;
            double timeStampOffset = -1;
            int d = 0;

            /*If there is more points enable the scrollbar*/
            if(data.Length > numberOfPoints) {
                hScrollBar.Visible = true;
                frameHeight = frameHeight - hScrollBar.Height;
                hScrollBar.Maximum = data.Length - numberOfPoints + 9;

                if(!hScrollBarInUse) {
                    hScrollBar.Value = data.Length - numberOfPoints;
                }

                graphStartIndex = hScrollBar.Value;
            } else {
                graphStartIndex = 0;
            }

            int numberOfPointsToGraph = 0;

            if(numberOfPoints > data.Length) {
                numberOfPointsToGraph = data.Length;
            } else {
                numberOfPointsToGraph = numberOfPoints;
            }

            timeStampOffset = data[graphStartIndex].timeStamp;

            Point[] graphPoints = new Point[numberOfPointsToGraph];
            for(int i = 0; i < numberOfPointsToGraph; i++) {
                d = i + graphStartIndex;

                try {
                    if(DCRemovalEnabled) {
                        graphPoints[i] = Point2Pixel((data[d].timeStamp - timeStampOffset), data[d].data - DCOffset);
                    } else {
                        graphPoints[i] = Point2Pixel((data[d].timeStamp - timeStampOffset), data[d].data);
                    }
                } catch(Exception e) {
                    Console.WriteLine("LineGraph: " + e.Message);
                }

            }

            drawingSurface.DrawLines(myPen, graphPoints);

            return timeStampOffset;
        }

        //Labels the x-axis with minor grid lines
        private void DrawXAxis(Graphics drawingSurface) {
            Pen myPen = new Pen(Color.DeepPink, 2.5f);
            SolidBrush myBrush = new SolidBrush(Color.Black);
            System.Drawing.Font myFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);

            Point pt;
            float X;
            float Xoffset = 0;

            //define the "tick length" to be the entire windows
            float YtickLength = frameHeight;
            float Y1 = frameHeight;
            float Y2 = 0;

            int tickDistance = 200;     //distance between each tick, in msec

            int numGroups = (int)((xAxisMax - xAxisMin) * (1000 / tickDistance)) + 1;   //number of lines to draw
            double stepSize = (double)(xAxisMax - xAxisMin) / (double)(numGroups-1);

            // Write the labels
            for(int i = 0; i <= numGroups; i++) {
                X = (float)(i * stepSize) + Xoffset;
                pt = Point2Pixel(X, 0);
                try {
                    //shift the major gridline slightly so that it's visible on the edge
                    if(i == numGroups - 1) {
                        drawingSurface.DrawLine(myPen, pt.X - 2, Y1, pt.X - 2, Y2);
                    } else {
                        drawingSurface.DrawLine(myPen, pt.X, Y1, pt.X, Y2);
                    }
                } catch(Exception e) {
                    
                }
            }

            myPen.Dispose();
            myBrush.Dispose();
        }

        //Labels the x-axis with major grid lines
        private void DrawXAxisMinor(Graphics drawingSurface) {
            Pen myPen = new Pen(Color.DeepPink,1);
            SolidBrush myBrush = new SolidBrush(Color.Black);
            System.Drawing.Font myFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);

            Point pt;
            float X;
            float Xoffset = 0;

            //define the "tick length" to be the entire windows
            float YtickLength = frameHeight;
            float Y1 = frameHeight;
            float Y2 = 0;

            int tickDistance = 40;     //distance between each tick, in msec

            int numGroups = (int)((xAxisMax - xAxisMin) * (1000/tickDistance));
            double stepSize = (double)(xAxisMax - xAxisMin) / (double)numGroups;

            // Write the labels
            for(int i = 1; i < numGroups; i++) {
                X = (float)(i * stepSize) + Xoffset;
                pt = Point2Pixel(X, 0);
                drawingSurface.DrawLine(myPen, pt.X, Y1, pt.X, Y2);
            }

            myPen.Dispose();
            myBrush.Dispose();
        }

        //Labels the y-axis with major grid lines
        private void DrawYAxis(Graphics drawingSurface) {
            Pen myPen = new Pen(Color.DeepPink, 2.5f);

            double mVolts_step = .5 * gain;     //number of mVolts between each line * gain
            int X = frameWidth;
            int Xwide = -frameWidth;

            int start = (int)((Math.Floor(toVoltage(Math.Abs(yAxisMax)) / mVolts_step) * mVolts_step) / conversionFactor);     //this is the location of the first tick
            if(yAxisMax < 0) {
                start *= -1;
            }

            int numLines = (int)(Math.Floor(toVoltage(yAxisMax - yAxisMin)) / mVolts_step);

            for(int i = numLines; i >= 0; i--) {
                Point tempPoint = Point2Pixel(0, (double)start - (mVolts_step / conversionFactor) * (double)i);
                try {
                    //shift the major gridline slightly so that it's visible on the edge
                    if(i == numLines) {
                        drawingSurface.DrawLine(myPen, X, tempPoint.Y - 2, (X + Xwide), tempPoint.Y - 2);
                    } else {
                        drawingSurface.DrawLine(myPen, X, tempPoint.Y, (X + Xwide), tempPoint.Y);
                    }
                } catch(Exception e) {

                }
            }

            myPen.Dispose();
            SolidBrush myBrush2 = new SolidBrush(Color.Black);
            System.Drawing.Font myFont2 = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            myBrush2.Dispose();
        }


        //Labels the y-axis with minor grid lines
        private void DrawYAxisMinor(Graphics drawingSurface) {
            Pen myPen = new Pen(Color.DeepPink, 1);

            double mVolts_step = .1 * gain;     //number of mVolts between each line * gain
            int X = frameWidth;
            int Xwide = -frameWidth;

            int start = (int)((Math.Floor(toVoltage(Math.Abs(yAxisMax)) / mVolts_step) * mVolts_step) / conversionFactor);     //this is the location of the first tick
            if(yAxisMax < 0) {
                start *= -1;
            }

            int numLines = (int)(Math.Floor(toVoltage(yAxisMax - yAxisMin)) / mVolts_step);

            for(int i = numLines; i >= 0; i--) {
                Point tempPoint = Point2Pixel(0, (double)start - (mVolts_step / conversionFactor) * (double)i);
                try {
                    drawingSurface.DrawLine(myPen, X, tempPoint.Y, (X + Xwide), tempPoint.Y);
                } catch(Exception e) {

                }
            }

            myPen.Dispose();
            SolidBrush myBrush2 = new SolidBrush(Color.Black);
            System.Drawing.Font myFont2 = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
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
            runningBuffer = new List<DataPair>();

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

            temp.X = (int)Math.Abs((xValue - xAxisMin) / (xAxisMax - xAxisMin) * frameWidth);
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

        //convert ADC to voltage
        public double toVoltage(double ADC) {
            return ADC * conversionFactor;
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