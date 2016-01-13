using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NeuroSky.MindView
{
    public partial class FormFFT : Form
    {
        //public GraphPanelFFT graphPanel;
        public FormFFT()
        {
            InitializeComponent();

            this.graphPanel.XAxisMax = 0D;
            this.graphPanel.XAxisMin = 0D;
            this.graphPanel.YAxisMax = 0D;
            this.graphPanel.YAxisMin = 0D;
        }

        private void FormFFT_Load(object sender, EventArgs e)
        {
            /// Only for function test.
            string filePath = Environment.CurrentDirectory +"\\raw\\FFT.txt";
            using (FileStream fsRead = new FileStream(filePath, FileMode.Open))
            {
                Console.WriteLine("file path:"+filePath);
            }
        }
    }
}
