using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

using System.Windows.Forms;

namespace NeuroSky.MindView
{
    public class GraphPanel: Panel
    {
        public LineGraph LineGraph;
        public Label Label;
        public Label ValueLabel;

        private System.ComponentModel.Container components = null;
        private Timer ValueUpdateTimer;

        public event EventHandler DataSavingFinished = delegate { };

        public GraphPanel()
        {
            InitializeComponent();

            LineGraph.DataSavingFinished += new EventHandler(LineGraph_DataSavingFinished);

            ValueUpdateTimer = new Timer();
            ValueUpdateTimer.Interval = 200; //In milliseconds.
            ValueUpdateTimer.Tick += new EventHandler(ValueUpdateTimer_Tick);
        }

        public void EnableValueDisplay()
        {
            ValueUpdateTimer.Start();
        }

        void LineGraph_DataSavingFinished(object sender, EventArgs e)
        {
            DataSavingFinished(this, EventArgs.Empty);
        }

        public void ValueUpdateTimer_Tick(object sender, EventArgs e)
        {
            int lastIndex = this.LineGraph.data0.Count - 1;
            if (lastIndex >= 0)
            {
                this.ValueLabel.Text = this.LineGraph.data0[lastIndex].data.ToString();
            }
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

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.LineGraph = new NeuroSky.MindView.LineGraph();
            this.Label = new System.Windows.Forms.Label();
            this.ValueLabel = new System.Windows.Forms.Label(); 
            this.SuspendLayout();
            // 
            // lineGraph
            // 
            this.BorderStyle = BorderStyle.FixedSingle;
            this.LineGraph.Location = new System.Drawing.Point(100, 0);
            this.LineGraph.Name = "lineGraph";
            this.LineGraph.Size = new System.Drawing.Size(700, 200);
            this.LineGraph.TabIndex = 0;
            // 
            // Label
            // 
            this.Label.Location = new System.Drawing.Point(0, 0);
            this.Label.Name = "label";
            this.Label.Size = new System.Drawing.Size(101, 200);
            this.Label.TextAlign = ContentAlignment.MiddleCenter;
            this.Label.TabIndex = 0;
            this.Label.Font = new Font(Label.Font, FontStyle.Bold);
            this.Label.Text = "Label";
            this.Label.BorderStyle = BorderStyle.FixedSingle;
            // 
            // ValueLabel
            // 
            this.ValueLabel.Location = new System.Drawing.Point(10,100);
            this.ValueLabel.Text = " ";
            this.ValueLabel.Size = new System.Drawing.Size(80, 30);
            this.ValueLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // GraphPanel
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Size = new System.Drawing.Size(800, 200);
            this.Controls.Add(this.ValueLabel);
            this.Controls.Add(this.Label);
            this.Controls.Add(this.LineGraph);
            this.ResumeLayout(false);

        }
        #endregion

        protected override void OnSizeChanged(EventArgs e)
        {
            //Adjust the Height
            this.Label.Height = this.Size.Height;
            this.LineGraph.Height = this.Size.Height;

            //Adjust the Width
            this.Label.Width = 101;
            this.LineGraph.Width = this.Size.Width - 100;

            //Adjust location of ValueLabel
            this.ValueLabel.Left = 10;
            this.ValueLabel.Top = this.Size.Height*2/3;

            base.OnSizeChanged(e);
            
        }
        
    }
}
