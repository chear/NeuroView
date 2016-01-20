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
        public int samplingRate = 10;

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
            DrawXAxis(drawingSurface);
            myPen.Dispose();
        }

        //Labels the x-axis with minor grid lines
        private void DrawXAxis(Graphics drawingSurface)
        {
            Pen myPen = new Pen(Color.Black, 0.5f);
            SolidBrush myBrush = new SolidBrush(Color.Black);
            System.Drawing.Font myFont = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);

            Point pt;
            float X;
            float Xoffset = 0;

            //define the "tick length" to be the entire windows
            float YtickLength = frameHeight;
            float Y1 = frameHeight;
            float Y2 = 0;
            int sampleRate = 512;
            int tickDistance = this.Width / sampleRate;     //distance between each tick, in msec

            int numGroups = (int)(xAxisMax - xAxisMin) + 1;   //number of lines to draw
            //double stepSize = (double)(this.Width /(numGroups-1));
            double stepSize = 10;

            /// Write the X Axis
            

            //// Write the labels
            for (int i = 0; i <= numGroups; i++)
            {
                X = (float)(i * stepSize) + Xoffset;
                pt = Point2Pixel(X, 0);
                try
                {
                    //shift the major gridline slightly so that it's visible on the edge
                    if (i == 0)
                    {
                        drawingSurface.DrawLine(myPen, pt.X+1, Y1, pt.X+1, 0);
                    }
                    else
                    {
                        drawingSurface.DrawLine(myPen, pt.X, Y1, pt.X, frameHeight - 5);
                    }
                }
                catch (Exception e)
                {

                }
            }

            myPen.Dispose();
            myBrush.Dispose();
        }

        private Point Point2Pixel(double xValue, double yValue)
        {
            Point temp = new Point();
            //temp.X = (int)Math.Abs((xValue - xAxisMin) / (xAxisMax - xAxisMin) * frameWidth);
            //temp.Y = (int)((yValue - yAxisMin) / (yAxisMax - yAxisMin) * frameHeight);            
            //temp.Y = frameHeight - temp.Y;
            temp.X = (int)xValue;
            temp.Y = frameHeight-(int)yValue;
            return temp;
        }
    }
}
