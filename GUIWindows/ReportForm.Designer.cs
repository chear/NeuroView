namespace NeuroSky.MindView
{
    partial class ReportForm
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
            this.attGraphPanel = new NeuroSky.MindView.GraphPanel();
            this.medGraphPanel = new NeuroSky.MindView.GraphPanel();
            this.infoLabel = new System.Windows.Forms.Label();
            // 
            // attGraphPanel
            // 
            this.attGraphPanel.Location = new System.Drawing.Point(0, 50);
            this.attGraphPanel.Name = "attGraphPanel";
            this.attGraphPanel.Size = new System.Drawing.Size(800, 150);
            this.attGraphPanel.TabIndex = 0;
            // 
            // medGraphPanel
            // 
            this.medGraphPanel.Location = new System.Drawing.Point(0, 199);
            this.medGraphPanel.Name = "medGraphPanel";
            this.medGraphPanel.Size = new System.Drawing.Size(800, 150);
            this.medGraphPanel.TabIndex = 0;

            this.infoLabel.Location = new System.Drawing.Point(0, 15);
            this.infoLabel.Name = "fileLabel";
            this.infoLabel.Size = new System.Drawing.Size(800, 24);
            this.infoLabel.Text = "None";

            this.Controls.Add(this.attGraphPanel);
            this.Controls.Add(this.medGraphPanel);
            this.Controls.Add(this.infoLabel);

            this.ClientSize = new System.Drawing.Size(800, 400);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "ReportForm";
        }

        #endregion
    }
}