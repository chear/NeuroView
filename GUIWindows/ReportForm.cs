using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NeuroSky.MindView
{
    public partial class ReportForm : Form
    {
        public GraphPanel attGraphPanel;
        public GraphPanel medGraphPanel;
        public Label infoLabel;

        public ReportForm()
        {
            InitializeComponent();

            attGraphPanel.Label.Text = "Attention";
            attGraphPanel.LineGraph.samplingRate = 1;
            attGraphPanel.LineGraph.xAxisMin = 0;
            attGraphPanel.LineGraph.yAxisMax = 105;
            attGraphPanel.LineGraph.yAxisMin = -5;

            medGraphPanel.Label.Text = "Meditation";
            medGraphPanel.LineGraph.samplingRate = 1;
            medGraphPanel.LineGraph.xAxisMin = 0;
            medGraphPanel.LineGraph.yAxisMax = 105;
            medGraphPanel.LineGraph.yAxisMin = -5;

           
        }

        public void Update()
        {
            int tempMax = 0;
            int tempMean = 0;

            tempMax = (int)attGraphPanel.LineGraph.Max();
            tempMean = (int)attGraphPanel.LineGraph.Mean();
            attGraphPanel.ValueLabel.Text = "Max: " + tempMax + "\n" +
                                            "Mean: " + tempMean + "\n";

            tempMax = (int)medGraphPanel.LineGraph.Max();
            tempMean = (int)medGraphPanel.LineGraph.Mean();
            medGraphPanel.ValueLabel.Text = "Max: " + tempMax + "\n" +
                                            "Mean: " + tempMean + "\n";

        }
    }
}
