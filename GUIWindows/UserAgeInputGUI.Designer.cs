namespace NeuroSky.ThinkGear
{
    partial class UserAgeInputGUI
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
            this.heartAgeInputGUITitleLabel = new System.Windows.Forms.Label();
            this.ageInputLabel = new System.Windows.Forms.Label();
            this.fileNameInputLabel = new System.Windows.Forms.Label();
            this.heartAgeInputTextBox = new System.Windows.Forms.TextBox();
            this.fileNameInputTextBox = new System.Windows.Forms.TextBox();
            this.heartAgeInputConfirmButton = new System.Windows.Forms.Button();
            this.heartAgeInputCancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // heartAgeInputGUITitleLabel
            // 
            this.heartAgeInputGUITitleLabel.AutoSize = true;
            this.heartAgeInputGUITitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.heartAgeInputGUITitleLabel.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.heartAgeInputGUITitleLabel.Location = new System.Drawing.Point(85, 19);
            this.heartAgeInputGUITitleLabel.Name = "heartAgeInputGUITitleLabel";
            this.heartAgeInputGUITitleLabel.Size = new System.Drawing.Size(264, 16);
            this.heartAgeInputGUITitleLabel.TabIndex = 0;
            this.heartAgeInputGUITitleLabel.Text = "Please input your age and file name! ";
            this.heartAgeInputGUITitleLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ageInputLabel
            // 
            this.ageInputLabel.AutoSize = true;
            this.ageInputLabel.Location = new System.Drawing.Point(74, 73);
            this.ageInputLabel.Name = "ageInputLabel";
            this.ageInputLabel.Size = new System.Drawing.Size(51, 13);
            this.ageInputLabel.TabIndex = 1;
            this.ageInputLabel.Text = "UserAge:";
            // 
            // fileNameInputLabel
            // 
            this.fileNameInputLabel.AutoSize = true;
            this.fileNameInputLabel.Location = new System.Drawing.Point(74, 139);
            this.fileNameInputLabel.Name = "fileNameInputLabel";
            this.fileNameInputLabel.Size = new System.Drawing.Size(54, 13);
            this.fileNameInputLabel.TabIndex = 2;
            this.fileNameInputLabel.Text = "FileName:";
            // 
            // heartAgeInputTextBox
            // 
            this.heartAgeInputTextBox.Location = new System.Drawing.Point(225, 73);
            this.heartAgeInputTextBox.MaxLength = 3;
            this.heartAgeInputTextBox.Name = "heartAgeInputTextBox";
            this.heartAgeInputTextBox.Size = new System.Drawing.Size(100, 20);
            this.heartAgeInputTextBox.TabIndex = 3;
            this.heartAgeInputTextBox.TextChanged += new System.EventHandler(this.heartAgeInputTextBox_TextChanged);
            // 
            // fileNameInputTextBox
            // 
            this.fileNameInputTextBox.Location = new System.Drawing.Point(225, 131);
            this.fileNameInputTextBox.Name = "fileNameInputTextBox";
            this.fileNameInputTextBox.Size = new System.Drawing.Size(100, 20);
            this.fileNameInputTextBox.TabIndex = 4;
            // 
            // heartAgeInputConfirmButton
            // 
            this.heartAgeInputConfirmButton.Location = new System.Drawing.Point(77, 202);
            this.heartAgeInputConfirmButton.Name = "heartAgeInputConfirmButton";
            this.heartAgeInputConfirmButton.Size = new System.Drawing.Size(75, 23);
            this.heartAgeInputConfirmButton.TabIndex = 5;
            this.heartAgeInputConfirmButton.Text = "Confirm";
            this.heartAgeInputConfirmButton.UseVisualStyleBackColor = true;
            this.heartAgeInputConfirmButton.Click += new System.EventHandler(this.heartAgeInputConfirmButton_Click);
            // 
            // heartAgeInputCancelButton
            // 
            this.heartAgeInputCancelButton.Location = new System.Drawing.Point(238, 202);
            this.heartAgeInputCancelButton.Name = "heartAgeInputCancelButton";
            this.heartAgeInputCancelButton.Size = new System.Drawing.Size(75, 23);
            this.heartAgeInputCancelButton.TabIndex = 6;
            this.heartAgeInputCancelButton.Text = "Cancel";
            this.heartAgeInputCancelButton.UseVisualStyleBackColor = true;
            this.heartAgeInputCancelButton.Click += new System.EventHandler(this.heartAgeInputCancelButton_Click);
            // 
            // UserAgeInputGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 262);
            this.ControlBox = false;
            this.Controls.Add(this.heartAgeInputCancelButton);
            this.Controls.Add(this.heartAgeInputConfirmButton);
            this.Controls.Add(this.fileNameInputTextBox);
            this.Controls.Add(this.heartAgeInputTextBox);
            this.Controls.Add(this.fileNameInputLabel);
            this.Controls.Add(this.ageInputLabel);
            this.Controls.Add(this.heartAgeInputGUITitleLabel);
            this.Name = "UserAgeInputGUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UserAgeInputGUI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label heartAgeInputGUITitleLabel;
        private System.Windows.Forms.Label ageInputLabel;
        private System.Windows.Forms.Label fileNameInputLabel;
        private System.Windows.Forms.TextBox heartAgeInputTextBox;
        private System.Windows.Forms.TextBox fileNameInputTextBox;
        private System.Windows.Forms.Button heartAgeInputConfirmButton;
        private System.Windows.Forms.Button heartAgeInputCancelButton;
    }
}