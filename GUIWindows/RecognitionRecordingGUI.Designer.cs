namespace NeuroSky.ThinkGear
{
    partial class RecognitionRecordingGUI
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
            this.trainingStepLabel = new System.Windows.Forms.Label();
            this.trainingStepIndicatorLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // trainingStepLabel
            // 
            this.trainingStepLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.trainingStepLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trainingStepLabel.Location = new System.Drawing.Point(75, 9);
            this.trainingStepLabel.Name = "trainingStepLabel";
            this.trainingStepLabel.Size = new System.Drawing.Size(332, 80);
            this.trainingStepLabel.TabIndex = 0;
            this.trainingStepLabel.Text = "Recording Status";
            this.trainingStepLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // trainingStepIndicatorLabel
            // 
            this.trainingStepIndicatorLabel.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.trainingStepIndicatorLabel.Location = new System.Drawing.Point(118, 131);
            this.trainingStepIndicatorLabel.Name = "trainingStepIndicatorLabel";
            this.trainingStepIndicatorLabel.Size = new System.Drawing.Size(256, 46);
            this.trainingStepIndicatorLabel.TabIndex = 1;
            this.trainingStepIndicatorLabel.Text = "Please touch your sensors!";
            this.trainingStepIndicatorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RecognitionRecordingGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 222);
            this.ControlBox = false;
            this.Controls.Add(this.trainingStepIndicatorLabel);
            this.Controls.Add(this.trainingStepLabel);
            this.Name = "RecognitionRecordingGUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RecognitionRecordingGUI";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label trainingStepLabel;
        private System.Windows.Forms.Label trainingStepIndicatorLabel;
    }
}