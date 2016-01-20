//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Drawing;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;

//namespace NeuroSky.MindView
//{
//    public partial class GraphPanelFFT : System.Windows.Forms.UserControl 
//    {
//        public double xAxisMax, xAxisMin; //In seconds 
//        public double yAxisMax, yAxisMin;

//        public GraphPanelFFT()
//        {
            
//        }

//        protected override void OnPaint(PaintEventArgs pe)
//        {

//            Graphics drawingSurface = pe.Graphics;
//            Pen myPen = new Pen(Color.Black, 2);
//            Rectangle rect = this.ClientRectangle;

//            /// drawing graphic
//            ///  drawingSurface.DrawLine(myPen, pt.X, Y1, pt.X, Y2);

//            myPen.Dispose();

//        }

//        private Point Point2Pixel(double xValue, double yValue)
//        {
//            Point temp = new Point();
//            temp.X = (int)Math.Abs((xValue - xAxisMin) / (xAxisMax - xAxisMin) * frameWidth);
//            temp.Y = (int)((yValue - yAxisMin) / (yAxisMax - yAxisMin) * frameHeight);
//            temp.Y = frameHeight - temp.Y;
//            return temp;
//        }
//    }
//}
