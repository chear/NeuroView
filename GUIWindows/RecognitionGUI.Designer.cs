namespace NeuroSky.ThinkGear
{
    partial class RecognitionGUI
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
            this.identificationResultLabel = new System.Windows.Forms.Label();
            this.identificationResultsIndicatorLabel = new System.Windows.Forms.Label();
            this.returnButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // identificationResultLabel
            // 
            this.identificationResultLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.identificationResultLabel.AutoSize = true;
            this.identificationResultLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.identificationResultLabel.Location = new System.Drawing.Point(73, 24);
            this.identificationResultLabel.Name = "identificationResultLabel";
            this.identificationResultLabel.Size = new System.Drawing.Size(151, 16);
            this.identificationResultLabel.TabIndex = 0;
            this.identificationResultLabel.Text = "Recognition Results:";
            this.identificationResultLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // identificationResultsIndicatorLabel
            // 
            this.identificationResultsIndicatorLabel.AutoSize = true;
            this.identificationResultsIndicatorLabel.Location = new System.Drawing.Point(73, 80);
            this.identificationResultsIndicatorLabel.Name = "identificationResultsIndicatorLabel";
            this.identificationResultsIndicatorLabel.Size = new System.Drawing.Size(134, 13);
            this.identificationResultsIndicatorLabel.TabIndex = 1;
            this.identificationResultsIndicatorLabel.Text = "Please touch your sensors!";
            this.identificationResultsIndicatorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // returnButton
            // 
            this.returnButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.returnButton.Location = new System.Drawing.Point(96, 127);
            this.returnButton.Name = "returnButton";
            this.returnButton.Size = new System.Drawing.Size(75, 23);
            this.returnButton.TabIndex = 2;
            this.returnButton.Text = "Return";
            this.returnButton.UseVisualStyleBackColor = true;
            this.returnButton.Visible = false;
            this.returnButton.Click += new System.EventHandler(this.returnButton_Click);
            // 
            // RecognitionGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 162);
            this.ControlBox = false;
            this.Controls.Add(this.returnButton);
            this.Controls.Add(this.identificationResultsIndicatorLabel);
            this.Controls.Add(this.identificationResultLabel);
            this.Name = "RecognitionGUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Recognition";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label identificationResultLabel;
        private System.Windows.Forms.Label identificationResultsIndicatorLabel;
        private System.Windows.Forms.Button returnButton;
    }
}