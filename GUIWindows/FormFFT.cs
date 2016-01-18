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
using System.Threading;

namespace NeuroSky.MindView
{
    public enum ReadType
    {
        RawArray = 1,   //BarGraph read raw.
        FFTArray = 2    // BarGraph read FFT array from 'Eric'
    };

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

            rawGraphPanel.samplingRate = 512;
            rawGraphPanel.xAxisMax = 20;
            rawGraphPanel.xAxisMin = 0;
            rawGraphPanel.yAxisMax = 50;
            rawGraphPanel.yAxisMin = 0;
            rawGraphPanel.Text = "ECG";
            rawGraphPanel.EnableValueDisplay();
            rawGraphPanel.OptimizeScrollBar();
            rawGraphPanel.DataSavingFinished += new EventHandler(OnDataSavingFinished);
            if (rawGraphPanel.PlotType == PlotType.Line)
            {
                rawGraphPanel.LineGraph.DCRemovalEnabled = false;
            }
            else if (rawGraphPanel.PlotType == PlotType.Bar)
            {
                rawGraphPanel.BarGraph.pwrSpecWindow = 1;
                rawGraphPanel.BarGraph.BarReadType = ReadType.RawArray;
            }
        }

        //when data saving is done
        void OnDataSavingFinished(object sender, EventArgs e)
        {
            //no need for this currently
        }

        private void FormFFT_Load(object sender, EventArgs e)
        {
             
        }


        delegate void UpdateDataForGraphicDelegate(double raw);
        private void UpdateDataForGraphic(double raw)
        {
             
            if(this.InvokeRequired)             
            {
                UpdateDataForGraphicDelegate del = new UpdateDataForGraphicDelegate(UpdateDataForGraphic);
                this.Invoke(del, new object[] { raw });
            } else {
               
                if (rawGraphPanel.BarGraph.BarReadType == ReadType.RawArray)
                {
                    Console.WriteLine("update BarGraph data:" + raw);  

                    /// TODO: Input 'Raw' from ThinkGear SDK or text ,then BarGraph.cs should be caculate the FFT with 
                    /// AForge.Math.FourierTransform.FFT(Complex[] , AForge.Math.FourierTransform.Direction.Forward).
                    Complex d = new Complex(raw, 0);
                    rawGraphPanel.BarGraph.Add(d);
                }
                else if (rawGraphPanel.BarGraph.BarReadType == ReadType.FFTArray)
                {
                    
                    float f = Convert.ToSingle(raw);
                    /// TODO: Input data for FFT array,
                    rawGraphPanel.BarGraph.AddFFT(f);
                    Console.WriteLine("update float data:" + f+",raw:"+raw);  
                }
            }        
        }

        Thread thread;
        public string filePath;
        private void button1_Click(object sender, EventArgs e)
        {
            rawGraphPanel.Clear();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text Files (.txt)|*.txt";
            openFileDialog1.FilterIndex = 1;
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;                
                thread = new Thread(new ThreadStart(DataLoading));
                thread.Start();           
            }
        }


        private void DataLoading()
        {
            /// Only for function test.
            //string filePath = Environment.CurrentDirectory + "\\raw\\stand_sin_ecg_30hz.txt";
            using (FileStream fsRead = new FileStream(filePath, FileMode.Open))
            {
                List<string> strArray = new List<string>();
                
                StreamReader sr = new StreamReader(fsRead);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line != null)
                    {                       
                        double fft = Double.Parse(line);
                        UpdateDataForGraphic(fft);
                        strArray.Add(line);                        
                    }
                }
                Console.WriteLine("reading finish!");
                /// test code for 'AForge.Math.FourierTransform.FFT' function
                //try
                //{
                //    Complex[] array = new Complex[128];
                //    for (int i = 0; i < 128; i++)
                //    {
                //        double raw = Double.Parse(strArray[i]);
                //        array[i] = new Complex(raw, 0);
                //    }
                //    List<float> pwr = new List<float>();
                //    // Perform forward FFT
                //    AForge.Math.FourierTransform.FFT(array, AForge.Math.FourierTransform.Direction.Forward);
                //    // Translate to power (a.^2)/numPts
                //    for (int i = 0; i < array.Length; i++)
                //    {
                //        // Squared magnitude / FFT Length * 2
                //        // the "* 2" is to recover the half of power lost by the calculation
                //        // See http://www.aforgenet.com/forum/viewtopic.php?f=2&t=1418
                //        pwr.Add((float)((array[i].SquaredMagnitude) / array.Length * 2));
                //        // Or log10 of power
                //        //pwr.Add((float)(Math.Log10((dataRaw[i].SquaredMagnitude)/dataRaw.Length*2)));
                //    }
                //    foreach (float f in pwr)
                //    {
                //        Console.WriteLine("fft value:" + f);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //}
            }
        }
    }
}
