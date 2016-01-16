using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Math;

namespace NeuroSky.MindView
{
    public partial class FormFFT : Form
    {
        //public GraphPanelFFT graphPanel;
        public FormFFT()
        {
            InitializeComponent();

            //graphPanel.samplingRate = 512;
            //graphPanel.XAxisMax = 512;
            //graphPanel.XAxisMin = 0;
            //graphPanel.YAxisMax = 2;
            //graphPanel.YAxisMin = -2;
        }

        private void FormFFT_Load(object sender, EventArgs e)
        {
            /// Only for function test.
            string filePath = Environment.CurrentDirectory +"\\raw\\FFT.txt";
            using (FileStream fsRead = new FileStream(filePath, FileMode.Open))
            {
                StreamReader sr = new StreamReader(fsRead);
                while (sr.ReadLine() != null)
                {
                    string line= sr.ReadLine();
                    double fft = Double.Parse(line);
                    Console.WriteLine("line:"+line);
                    Complex d = new Complex(32760,0);
                    barGraph.Add(d);
                }

            }
        }
    }
}
