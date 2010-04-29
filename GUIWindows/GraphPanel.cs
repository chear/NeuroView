using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

using System.Windows.Forms;

namespace NeuroSky.NeuroView
{
    public class GraphPanel: Panel
    {
        public LineGraph LineGraph;
        public Label Label;

        private System.ComponentModel.Container components = null;

        public GraphPanel()
        {
            InitializeComponent();
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
            this.LineGraph = new NeuroSky.NeuroView.LineGraph();
            this.Label = new System.Windows.Forms.Label();
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
            // label
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
            // GraphPanel
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Size = new System.Drawing.Size(800, 200);
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

            base.OnSizeChanged(e);
            
        }
        
    }
}
