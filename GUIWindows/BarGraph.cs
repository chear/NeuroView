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



namespace NeuroSky.MindView
{

    public class BarGraph : System.Windows.Forms.UserControl
    {

        private System.ComponentModel.Container components = null;
        public System.Windows.Forms.HScrollBar hScrollBar;
        private System.Windows.Forms.Timer maxFrameRateTimer;

        public List<AForge.Math.Complex> data0; // Storage of data points to compute with
        public float[] oldPwr;                  // Storage of old power spectrum

        public int frameHeight = 240;
        public int frameWidth = 760;
        public int samplingRate = 10;           // Hertz

        public double xAxisMax, xAxisMin;       // Hertz
        public double yAxisMax, yAxisMin;       // squared ADC units
        public double pwrSpecWindow;            // Number of seconds FFT should encompass
        public int numberOfBins;                // Integer number of frequency bins to graph
        public int binIndexHigh, binIndexLow;   // Bin index, integer
        public double binWidth;                 // Width of a bin in pixels

        public bool RecordDataFlag = false;
        public bool SaveDataFlag = false;
        public string DataFolderString = @"C:\Data";
        public string FolderNameString = "Test";
        public string FileNameString = "FileName";
        public string FileHeaderString = "Test, Test";

        public bool hScrollBarInUse = false;

        private Size defaultSize;
        private int scrollBarTop;

        public int numberOfPoints;         // Number of data points needed for computation

        private Thread saveDataThread;

        public event EventHandler DataSavingFinished = delegate { };


        /**
         * Add a data point to the storage bin
         */
        public void Add(AForge.Math.Complex d)
        {
            data0.Add(d);
        }


        /**
         * Clear the storage bin, reset the scroll bar? Do we even need the scrollbar?
         */
        public void Clear()
        {
            data0.Clear();

            //Initialize Horizontal Scrollbar
            hScrollBar.Visible = false;
            hScrollBar.Maximum = 1;
            hScrollBar.Minimum = 0;
            hScrollBar.Value = hScrollBar.Maximum - 1;
            hScrollBarInUse = false;
        }


        /**
         * When repainting the screen, perform these maintenance tasks and possibly redraw the graph
         */
        // May need to down sample how often the graph is refreshed, impose a "minNumOfNewPts" value?
        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics drawingSurface = pe.Graphics;
            SolidBrush myBrush = new SolidBrush(Color.Blue);
            Rectangle rect = this.ClientRectangle;

            // Set frame size
            frameWidth = rect.Right - rect.Left;
            frameHeight = rect.Bottom - rect.Top;

            // Update the bin indices
            ResetBinIndices();

            // Set the number of data points needed to compute power spectrum
            numberOfPoints = (int)(pwrSpecWindow * samplingRate);


            // Assure that there are enough data points to work with
            if ((data0 != null) && (data0.Count >= numberOfPoints+32))
            {
                // Trim excess values to minimize memory usage
                hScrollBar.Visible = false;
                while (data0.Count > numberOfPoints)
                {
                    data0.RemoveAt(0);
                }

                // If so, then go compute and disply the result
                oldPwr = new float[numberOfPoints];
                Array.Copy(ComputePwrSpec(data0.ToArray()), oldPwr, numberOfPoints);
                DrawGraph(oldPwr, drawingSurface, myBrush);

            } else if ((data0 != null) && (data0.Count >= numberOfPoints) && (oldPwr.Length >= numberOfPoints)) {
                // Draw the old power spectrum
                DrawGraph(oldPwr, drawingSurface, myBrush);
            }
            // End repainting sequence.
            
            // Label bins
            DrawXAxis(drawingSurface);

            // Label amplitude
            DrawYAxis(drawingSurface);

            // Draw helper lines
            // Green line at 20, stay above this
            // Red line at 3, stay below this
            //DrawHelperLines(drawingSurface);

            // Clean up brush
            myBrush.Dispose();
        }



        /**
         * Calculate the power spectrum
         */
        private float[] ComputePwrSpec(AForge.Math.Complex[] dataRaw)
        {
            // Variables
            List<float> pwr = new List<float>();

            // Perform forward FFT
            AForge.Math.FourierTransform.FFT(dataRaw, AForge.Math.FourierTransform.Direction.Forward);

            // Translate to power (a.^2)/numPts
            for (int i = 0; i < dataRaw.Length; i++)
            {
                // Squared magnitude / FFT Length * 2
                // the "* 2" is to recover the half of power lost by the calculation
                // See http://www.aforgenet.com/forum/viewtopic.php?f=2&t=1418
                pwr.Add((float)((dataRaw[i].SquaredMagnitude)/dataRaw.Length*2));

                // Or log10 of power
                //pwr.Add((float)(Math.Log10((dataRaw[i].SquaredMagnitude)/dataRaw.Length*2)));
            }

            // Return the power spectrum
            return pwr.ToArray();
        }



        /**
         * Draw the actual bar graph
         * 
         * Requires using ComputePwrSpec first to calculate the bin heights
         */
        private void DrawGraph(float[] data, Graphics drawingSurface, SolidBrush myBrush)
        {
            Point graphPoint = new Point();
            int d = 0;
            float X;
            float Y;
            float height;
            
            // Get the pixel locations and draw the bars
            for (int i = 0; i < numberOfBins; i++) {
                d = i + binIndexLow;

                try {
                    graphPoint = Point2Pixel(d, data[d]);
                }
                catch (Exception e) {
                    Console.WriteLine("BarGraph: " + e.Message);
                }

                // Set variables
                X = (i * (float)binWidth);
                Y = graphPoint.Y;
                height = (int)((data[d] - yAxisMin) / (yAxisMax - yAxisMin + 1) * frameHeight);

                // Draw the bar
                drawingSurface.FillRectangle(myBrush, X, Y, (float)binWidth, height);
            }
        }


        /**
         * Labels the x-axis with frequency bins
         */
        private void DrawXAxis(Graphics drawingSurface)
        {
            Pen myPen = new Pen(Color.Black);
            SolidBrush myBrush = new SolidBrush(Color.Black);
            System.Drawing.Font myFont = new System.Drawing.Font("Microsoft Sans Serif", 11F);

            float X;
            float Xnum;
            float Xoffset = 0;

            float Y1 = frameHeight - 0;
            float Y2 = Y1 - 15;

            int groupSize = 0;
            int numGroups = 0;


            // Set number of labels to create
            if (numberOfBins >= 100)
            {
                groupSize = 25;
            }
            else if (numberOfBins >= 50)
            {
                groupSize = 10;
            }
            else if (numberOfBins >= 30)
            {
                groupSize = 5;
            }
            else if (numberOfBins >= 20)
            {
                groupSize = 2;
            }
            else
            {
                groupSize = 1;
            }

            numGroups = (int)(numberOfBins / groupSize);
            //Xoffset = (groupSize - 1) * (float)binWidth;

            // Write the labels
            for (int i = 1; i <= numGroups; i++)
            {
                if (pwrSpecWindow == 4)
                {
                    X = 1;
                }
                Xnum = ((i * groupSize) + (int)xAxisMin - 1) / (float)pwrSpecWindow;
                X = ((((i * groupSize) - (float)pwrSpecWindow) * (float)binWidth) + Xoffset);
                drawingSurface.DrawLine(myPen, X, Y1, X, Y2);
                drawingSurface.DrawString(Xnum.ToString(), myFont, myBrush, X, Y2-2);
            }

            myPen.Dispose();
            myBrush.Dispose();
        }


        /**
         * Labels the y-axis with amplitudes
         */
        private void DrawYAxis(Graphics drawingSurface)
        {
            Pen myPen = new Pen(Color.Black);
            int X = 0;
            int Xwide = 10;
            int numLines = 10;
            double Ystep = Math.Ceiling((double)(frameHeight)/(double)(numLines));

            for (int i = numLines-1; i > 0; i--)
            {
                drawingSurface.DrawLine(myPen, X, frameHeight - ((int)Ystep * i), (X + Xwide), frameHeight - ((int)Ystep * i));
            }
            
            myPen.Dispose();
            SolidBrush myBrush2 = new SolidBrush(Color.Black);
            System.Drawing.Font myFont2 = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            drawingSurface.DrawString(yAxisMax.ToString(), myFont2, myBrush2, Xwide, 2);
            drawingSurface.DrawString(yAxisMin.ToString(), myFont2, myBrush2, Xwide, frameHeight - 20);
            myBrush2.Dispose();
        }


        /**
         * Draws two horizontal lines helpful for testing purpose.
         */
        private void DrawHelperLines(Graphics drawingSurface)
        {
            Pen myPenG = new Pen(Color.Green);
            Pen myPenR = new Pen(Color.Red);
            Point ptG = Point2Pixel(0, 20);
            Point ptR = Point2Pixel(0, 3);

            drawingSurface.DrawLine(myPenG, 0, ptG.Y, frameWidth, ptG.Y);
            drawingSurface.DrawLine(myPenR, 0, ptR.Y, frameWidth, ptR.Y);

            myPenG.Dispose();
            myPenR.Dispose();
        }


        /**
         * Recalculate values related to the bin indices
         */
        public void ResetBinIndices()
        {
            // Set the number of power spectrum bins that will be graphed.
            numberOfBins = (int)((xAxisMax - xAxisMin) * (pwrSpecWindow) + 1);
            if (numberOfBins > (int)(((pwrSpecWindow * samplingRate) / 2) - 1))
            {
                // Make sure the frequency bins graphed doesn't exceed the Nyquist frequency
                numberOfBins = (int)((pwrSpecWindow * samplingRate) / 2);
                //numberOfBins = (int)(((pwrSpecWindow * samplingRate) / 2) - 1);
            }

            // Set low bin index
            binIndexLow = (int)(xAxisMin * (pwrSpecWindow));

            // Set high bin index
            binIndexHigh = (int)(xAxisMax * (pwrSpecWindow));

            // Set bin width
            binWidth = Math.Floor((double)(frameWidth) / (double)(numberOfBins) * 10) / 10;
        }

        
        /**
         * Save out the data?
         */
        private void SaveData()
        {
#if false
            DataPair[] dataPointsCopy = data0.ToArray();
            //Console.WriteLine("Number of datapoints right before saving: " + dataPointsCopy.Length);

            SaveDataFlag = false;

            string newPath = System.IO.Path.Combine(DataFolderString, FolderNameString);

            System.IO.Directory.CreateDirectory(newPath);

            newPath = System.IO.Path.Combine(newPath, FileNameString);

            if (!System.IO.File.Exists(newPath))
            {
                using (System.IO.StreamWriter fs = new System.IO.StreamWriter(newPath, false))
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

            DataSavingFinished(this, EventArgs.Empty);
#endif
        }

        /**
         * On intialization
         */
        public BarGraph()
        {
            defaultSize = new Size(frameWidth, frameHeight);

            InitializeComponent();

            this.Size = defaultSize;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.scrollBarTop = frameHeight - this.hScrollBar.Height;
            this.hScrollBar.Location = new System.Drawing.Point(0, scrollBarTop);
            ResetBinIndices();
            this.DoubleBuffered = true;

            data0 = new List<AForge.Math.Complex>();
            oldPwr = new float[1];

            /*Setting up the timer for the max frame rate*/
            maxFrameRateTimer = new System.Windows.Forms.Timer();
            maxFrameRateTimer.Interval = 16; //In milliseconds
            maxFrameRateTimer.Tick += new EventHandler(MaxFrameRateTimer_Tick);
            maxFrameRateTimer.Start();


        }


        /**
         * Timer maxed out?
         */
        public void MaxFrameRateTimer_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }


        /**
         * Converts graph information into pixel locations for plotting graphics
         */
        private Point Point2Pixel(double xValue, double yValue)
        {
            Point temp = new Point();

            // Modified with extra "+1" because need space for rectangle in bar graph
            temp.X = (int)((xValue - xAxisMin) / (xAxisMax - xAxisMin + 1) * frameWidth);
            temp.Y = (int)((yValue - yAxisMin) / (yAxisMax - yAxisMin + 1) * frameHeight);
            temp.Y = frameHeight - temp.Y;

            return temp;

        }


        /**
         * Drop the resource and its parts
         */
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

            this.Name = "BarGraph";
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

        private void hScrollBar_ValueChanged(object sender, System.EventArgs e)
        {
            //Console.WriteLine("HSCROLLBAR VALUE CHANGED. " + scrollCounter++ );

            if (hScrollBar.Value >= Math.Ceiling((data0.Count - numberOfPoints) * 0.99))
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


        /**
         * Actions to take on resize
         */
        protected override void OnSizeChanged(EventArgs e)
        {

            /*Update Location and dimension*/
            scrollBarTop = this.Height - this.hScrollBar.Height;
            this.hScrollBar.Location = new System.Drawing.Point(0, scrollBarTop);
            this.hScrollBar.Width = this.Width;

            base.OnSizeChanged(e);

        }


        /**
         * Optimize scrollbar
         */
        public void OptimizeScrollBar()
        {
            /*
            numberOfPoints = (int)Math.Abs(((xAxisMax - xAxisMin) * samplingRate)) + 1;
            this.hScrollBar.LargeChange = numberOfPoints / 8;
             * */
        }

        


    }/*End of BarGraph Class*/

}
