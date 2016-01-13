using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NeuroSky.MindView
{
    public partial class GraphPanelFFT : UserControl
    {
        public List<DataPair> data0;
        public int timeStampIndex;

        public double xAxisMax, xAxisMin; //In seconds 
        public double yAxisMax, yAxisMin;

        private Size defaultSize;
        public int frameHeight = 240;
        public int frameWidth = 760;

        private double conversionFactor = 0.0183;
        private int gain = 128;

        public GraphPanelFFT()
        {
            defaultSize = new Size(frameWidth, frameHeight);
            InitializeComponent();
            this.Size = defaultSize;
        }

        #region
        //get or set the xAxisMax value
        public double XAxisMax
        {
            get
            {
                return this.xAxisMax;
            }
            set
            {
                xAxisMax = value;        
            }
        }

        //get or set the xAxisMin value
        public double XAxisMin
        {
            get
            {
                return xAxisMin;                
            }
            set
            {
                xAxisMin = value;
            }
        }

        //get or set the yAxisMax value. the input should be mV, so conver to ADC
        public double YAxisMax
        {
            get
            {  
                return yAxisMax;
            }
            set
            {        
                    if (value > 0)
                    {
                        yAxisMax = (int)Math.Ceiling((value * gain) / conversionFactor);
                    }
                    else
                    {
                        yAxisMax = (int)Math.Floor((value * gain) / conversionFactor);
                    }
            }
        }


        //get or set the yAxisMin value
        public double YAxisMin
        {
            get
            {
                return yAxisMin;
            }
            set
            {
                    if (value > 0)
                    {
                        yAxisMin = (int)Math.Ceiling((value * gain) / conversionFactor);
                    }
                    else
                    {
                        yAxisMin = (int)Math.Floor((value * gain) / conversionFactor);
                    }
            }
        }
        #endregion

        /// <summary>
        /// clear the drawing
        /// </summary>
        public void Clear()
        {
            lock (data0)
            {
                data0.Clear();
            }
            timeStampIndex = 0;
        }

        /// <summary>
        /// this function use to add data to 
        /// </summary>
        /// <param name="p"></param>
        public void Add(DataPair p)
        {
            lock (data0)
            {
                data0.Add(p);
            }
        }
       
        //overide
        protected override void OnSizeChanged(EventArgs e)
        {
            /*Update Location and dimension*/           
            base.OnSizeChanged(e);
        }

        //overide
        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics drawingSurface = pe.Graphics;
            Pen myPen = new Pen(Color.Black, 2);
            Rectangle rect = this.ClientRectangle;

            myPen.Dispose();
        }

        private double DrawGraph(DataPair[] data, Graphics drawingSurface, Pen myPen)
        {
            int graphStartIndex = 0;
            double timeStampOffset = -1;
            int d = 0;

            /*If there is more points enable the scrollbar*/
            //if (data.Length > numberOfPoints)
            //{
            //    hScrollBar.Visible = true;
            //    frameHeight = frameHeight - hScrollBar.Height;
            //    hScrollBar.Maximum = data.Length - numberOfPoints + 9;

            //    if (!hScrollBarInUse)
            //    {
            //        hScrollBar.Value = data.Length - numberOfPoints;
            //    }

            //    graphStartIndex = hScrollBar.Value;
            //}
            //else
            //{
            //    graphStartIndex = 0;
            //}

            //int numberOfPointsToGraph = 0;

            //if (numberOfPoints > data.Length)
            //{
            //    numberOfPointsToGraph = data.Length;
            //}
            //else
            //{
            //    numberOfPointsToGraph = numberOfPoints;
            //}

            //timeStampOffset = data[graphStartIndex].timeStamp;

            //Point[] graphPoints = new Point[numberOfPointsToGraph];
            //for (int i = 0; i < numberOfPointsToGraph; i++)
            //{
            //    d = i + graphStartIndex;

            //    try
            //    {
            //        if (DCRemovalEnabled)
            //        {
            //            graphPoints[i] = Point2Pixel((data[d].timeStamp - timeStampOffset), data[d].data - DCOffset);
            //        }
            //        else
            //        {
            //            graphPoints[i] = Point2Pixel((data[d].timeStamp - timeStampOffset), data[d].data);
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine("LineGraph: " + e.Message);
            //    }

            //}

            //drawingSurface.DrawLines(myPen, graphPoints);

            return timeStampOffset;
        }

        private Point Point2Pixel(double xValue, double yValue) {
            Point temp = new Point();

            temp.X = (int)Math.Abs((xValue - xAxisMin) / (xAxisMax - xAxisMin) * frameWidth);
            temp.Y = (int)((yValue - yAxisMin) / (yAxisMax - yAxisMin) * frameHeight);
            temp.Y = frameHeight - temp.Y;

            return temp;
        }
    }
}
