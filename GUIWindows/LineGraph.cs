#define ENABLE

using System;

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;

using System.Collections.Generic;



public struct DataPair {
    public double timeStamp;
    public double data;

    public DataPair(double t, double d) {
        timeStamp = t;
        data      = d;
    }

}


namespace NeuroSky.NeuroView {

    public class LineGraph : System.Windows.Forms.UserControl
    {

        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.HScrollBar hScrollBar;

        public List<DataPair> dataPoints;
        public int frameHeight = 240;
        public int frameWidth = 760;
        public int samplingRate = 10;

        public double xAxisMax, xAxisMin; //In seconds 
        public double yAxisMax, yAxisMin;

        public bool RecordDataFlag = false;
        public bool SaveDataFlag = false;
        public string DataFolderString = @"c:\Data";
        public string FolderNameString = "Test";
        public string FileNameString = "FileName";
        public string FileHeaderString = "Test, Test";

        public bool hScrollBarInUse = false;

        private Size defaultSize;
        private int scrollBarTop;
        //private int hScrollBarMaximum;

        private int numberOfPoints;
        private int graphStartIndex;
        private double timeStampOffset;

        public void Add(DataPair p)
        {
            
            dataPoints.Add(p);
        }

        public void Clear()
        {
            dataPoints.Clear();

            //Initialize Horizontal Scrollbar
            hScrollBar.Visible = false;
            hScrollBar.Maximum = 1;
            hScrollBar.Minimum = 0;
            hScrollBar.Value = hScrollBar.Maximum - 1;
            hScrollBarInUse = false;
      }

        protected override void OnPaint(PaintEventArgs pe)
        {

            Graphics drawingSurface = pe.Graphics;
            Pen myPen = new Pen(Color.Blue, 1);
            Rectangle rect = this.ClientRectangle;

            frameWidth = rect.Right - rect.Left;
            frameHeight = rect.Bottom - rect.Top;

            numberOfPoints = (int)Math.Abs(((xAxisMax - xAxisMin) * samplingRate)) + 1;

            int d = 0;
            
            /*Makes sures that the graph has at least two points to graph a line*/
            if ((dataPoints != null) && (dataPoints.Count > 1))
            {
                /*If recording is not enable trim the excess values*/
                if (!this.RecordDataFlag)
                {
                    if (this.SaveDataFlag)
                    {
                        this.SaveData();
                    }

                    hScrollBar.Visible = false;
                    while (dataPoints.Count > numberOfPoints)
                    {
                        dataPoints.RemoveAt(0);
                        dataPoints.TrimExcess();
                    }
                    dataPoints.TrimExcess();

                } 
                
                /*If there is more points enable the scrollbar*/
                if (dataPoints.Count > numberOfPoints )
                {
                    hScrollBar.Visible = true;
                    frameHeight = frameHeight - hScrollBar.Height;
                    hScrollBar.Maximum = dataPoints.Count - numberOfPoints + 9;

                    if (!hScrollBarInUse)
                    {
                        hScrollBar.Value = dataPoints.Count - numberOfPoints;
                    }

                    graphStartIndex = hScrollBar.Value;
               
#if false

                    Console.WriteLine("hScrollBar.Maximum: " + hScrollBar.Maximum +
                                      " hScrollBarMaximum: " + hScrollBarMaximum +
                                      " dataPoints.Count: " + dataPoints.Count +
                                      " numberOfPoints: " + numberOfPoints +
                                      " hScrollBar.Value: " + hScrollBar.Value);
#endif
                }
                else
                {
                    graphStartIndex = 0;
                }

                    
                

                int numberOfPointsToGraph = 0;

                if (numberOfPoints > dataPoints.Count)
                {
                    numberOfPointsToGraph = dataPoints.Count;
                }else{
                    numberOfPointsToGraph = numberOfPoints;
                }

                timeStampOffset = dataPoints[graphStartIndex].timeStamp;

                Point[] graphPoints = new Point[numberOfPointsToGraph];
                for (int i = 0; i < numberOfPointsToGraph; i++)
                {
                    d = i + graphStartIndex;

                    try
                    {
                        graphPoints[i] = Point2Pixel(dataPoints[d].timeStamp, dataPoints[d].data);

                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine("LineGraph: " + e.Message);
                        //Console.WriteLine("d: " + d);
#if false
                        Console.WriteLine("data: " + dataPoints[d].timeStamp + ", " + dataPoints[d].data +
                                      " graph: " + graphPoints[i].X + ", " + graphPoints[i].Y);
#endif
                    }

                }

                drawingSurface.DrawLines(myPen, graphPoints);
            }
#if false
            Console.WriteLine("StartPoint: " + graphStartIndex
                            + " FinishPoint: " + (d)
                            + " TotalDataPoints: " + dataPoints.Count
                            + " hScrollBarValue: " + hScrollBar.Value
                            + " hScrollBar.Maximum: " + hScrollBar.Maximum);
           
#endif
            myPen.Dispose();

        }

        private void SaveData()
        {

            string newPath = System.IO.Path.Combine(DataFolderString, FolderNameString);

            System.IO.Directory.CreateDirectory(newPath);

            newPath = System.IO.Path.Combine(newPath, FileNameString);

            DataPair[] dataPointsCopy = dataPoints.ToArray();

            if (!System.IO.File.Exists(newPath))
            {
                using( System.IO.StreamWriter fs = new System.IO.StreamWriter(newPath, false))
                {
                    /*Write the header to the csv file*/
                    fs.WriteLine(FileHeaderString);

                    /*Write the data to the csv file*/
                    foreach (DataPair d in dataPointsCopy)
                    {
                        fs.WriteLine(d.timeStamp + ", " + d.data);
                    }

                }
            }

            SaveDataFlag = false;
        }

        public LineGraph()
        {
            defaultSize = new Size(frameWidth, frameHeight);
            
            InitializeComponent();

            this.DoubleBuffered = true;

            dataPoints = new List<DataPair>(10);
            numberOfPoints = 512;

        }

        private Point Point2Pixel( double xValue, double yValue )
        {
            Point temp = new Point();

            xValue = xValue - timeStampOffset;

            temp.X = (int)( (xValue - xAxisMin)/(xAxisMax - xAxisMin) * frameWidth );
            temp.Y = (int)( (yValue - yAxisMin)/(yAxisMax - yAxisMin) * frameHeight );
            temp.Y = frameHeight - temp.Y;

            return temp;

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

        #region Component Designer generated code
        private void InitializeComponent()
        {
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

        private void hScrollBar_ValueChanged(object sender, System.EventArgs e)
        {
            //Console.WriteLine("HSCROLLBAR VALUE CHANGED. " + scrollCounter++ );

            if (hScrollBar.Value >= Math.Ceiling((dataPoints.Count - numberOfPoints)*0.9))
            {
                hScrollBarInUse = false;
            }
            else
            {
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

        protected override void OnSizeChanged(EventArgs e)
        {

            /*Update Location and dimension*/
            scrollBarTop = this.Height - this.hScrollBar.Height;
            this.hScrollBar.Location = new System.Drawing.Point(0, scrollBarTop);
            this.hScrollBar.Width = this.Width;

        }


    }/*End of LinGraph Class*/

}
