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

            this.rawGraphPanel = new NeuroSky.MindView.GraphPanel(PlotType.Bar);
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // rawGraphPanel
            // 
            this.rawGraphPanel.Location = new System.Drawing.Point(12, 43);
            this.rawGraphPanel.Name = "rawGraphPanel";
            this.rawGraphPanel.samplingRate = 10;
            this.rawGraphPanel.Size = new System.Drawing.Size(800, 200);
            this.rawGraphPanel.TabIndex = 0;
            this.rawGraphPanel.xAxisMax = 0D;
            this.rawGraphPanel.xAxisMin = 0D;
            this.rawGraphPanel.yAxisMax = 0D;
            this.rawGraphPanel.yAxisMin = 0D;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Load";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(94, 18);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(78, 16);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "Raw Input";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            //
            // FormFFT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            this.ClientSize = new System.Drawing.Size(830, 255);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.rawGraphPanel);
            this.Name = "FormFFT";
            this.Text = "FFT Chat";
            this.Load += new System.EventHandler(this.FormFFT_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GraphPanel rawGraphPanel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}