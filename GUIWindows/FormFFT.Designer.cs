namespace NeuroSky.MindView
{
    partial class FormFFT
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.barGraph = new NeuroSky.MindView.BarGraph();
            this.SuspendLayout();
            // 
            // barGraph
            // 
            this.barGraph.BackColor = System.Drawing.Color.White;
            this.barGraph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.barGraph.Location = new System.Drawing.Point(12, 282);
            this.barGraph.Name = "barGraph";
            this.barGraph.Size = new System.Drawing.Size(760, 240);
            this.barGraph.TabIndex = 1;
            // 
            // FormFFT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 577);
            this.Controls.Add(this.barGraph);
            this.Name = "FormFFT";
            this.Text = "FFT Chat";
            this.Load += new System.EventHandler(this.FormFFT_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private BarGraph barGraph;


    }
}