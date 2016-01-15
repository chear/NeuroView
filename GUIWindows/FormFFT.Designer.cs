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
            this.graphPanel = new NeuroSky.MindView.GraphPanelFFT();
            this.SuspendLayout();
            // 
            // graphPanel
            // 
            this.graphPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.graphPanel.Location = new System.Drawing.Point(12, 12);
            this.graphPanel.Name = "graphPanel";
            this.graphPanel.Size = new System.Drawing.Size(760, 240);
            this.graphPanel.TabIndex = 0;
            this.graphPanel.XAxisMax = 0D;
            this.graphPanel.XAxisMin = 0D;
            this.graphPanel.YAxisMax = 0D;
            this.graphPanel.YAxisMin = 0D;
            // 
            // FormFFT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 277);
            this.Controls.Add(this.graphPanel);
            this.Name = "FormFFT";
            this.Text = "FFT Chat";
            this.Load += new System.EventHandler(this.FormFFT_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private GraphPanelFFT graphPanel;


    }
}