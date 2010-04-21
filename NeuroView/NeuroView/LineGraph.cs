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
        //public int frameWidth = 330;
        public int frameHeight = 240;
        public int frameWidth = 760;
        public int samplingRate = 10;

        public double xAxisMax, xAxisMin; //In seconds 
        public double yAxisMax, yAxisMin;

        public bool recordDataFlag = false;
        public bool hScrollBarInUse = false;

        private Size defaultSize;
        private int scrollBarTop;
        private int hScrollBarMaximum;

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

            if ((dataPoints != null) && (dataPoints.Count > 1))
            {
                if (!recordDataFlag)
                {
                    hScrollBar.Visible = false;
                    while (dataPoints.Count > numberOfPoints)
                    {
                        dataPoints.RemoveAt(0);
                        dataPoints.TrimExcess();
                    }
                    dataPoints.TrimExcess();

                }else 
                {
                    if (dataPoints.Count > numberOfPoints )
                    {
                        hScrollBar.Visible = true;
                        frameHeight = frameHeight - hScrollBar.Height;
                        hScrollBar.Maximum = dataPoints.Count - numberOfPoints + 9;
                        hScrollBarMaximum = hScrollBar.Maximum - numberOfPoints + 1;

                        graphStartIndex = dataPoints.Count - numberOfPoints;

                        if (!hScrollBarInUse)
                        {
                            if (hScrollBar.Maximum > (numberOfPoints - 1))
                            {
                                hScrollBar.Value = hScrollBarMaximum;
                            }
                        }else{
                            graphStartIndex -= (hScrollBarMaximum - hScrollBar.Value);
                        }
                    }
                    else
                    {
                        graphStartIndex = 0;
                    }

                    
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
                    graphPoints[i] = Point2Pixel( dataPoints[d].timeStamp, dataPoints[d].data );

                    Console.WriteLine("data: " + dataPoints[d].timeStamp + ", " + dataPoints[d].data +
                                      " graph: " + graphPoints[i].X + ", " + graphPoints[i].Y);
                }

                drawingSurface.DrawLines(myPen, graphPoints);
            }

            Console.WriteLine("StartPoint: " + graphStartIndex
                            + " FinishPoint: " + (d)
                            + " TotalDataPoints: " + dataPoints.Count
                            + " hScrollBarValue: " + hScrollBar.Value
                            + " hScrollBarMax: " + hScrollBar.Maximum);
           

            myPen.Dispose();
        }

        public LineGraph()
        {
            defaultSize = new Size(frameWidth, frameHeight);
            

            InitializeComponent();

            dataPoints = new List<DataPair>(10);
            numberOfPoints = 20;
        }

        private Point Point2Pixel( double xValue, double yValue )
        {
            Point temp = new Point();

            xValue = xValue - timeStampOffset;

            temp.X = (int)( (xValue - xAxisMin)/(xAxisMax - xAxisMin) * frameWidth );
            temp.Y = (int)( (yValue - yAxisMin)/(yAxisMax - yAxisMin) * frameHeight );

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

        private void hScrollBar_ValueChanged(object sender, System.EventArgs e)
        {

            hScrollBarInUse = true;

            if (hScrollBar.Value >= hScrollBar.Maximum - numberOfPoints + 1)
            {
                hScrollBarInUse = false;
            }
            else
            {
                this.Invalidate();
            }
        }


    }

}
