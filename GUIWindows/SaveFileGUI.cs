using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NeuroSky.ThinkGear {
    public partial class SaveFileGUI : Form {

        public event EventHandler SaveButtonClicked = delegate { };
        public event EventHandler DiscardButtonClicked = delegate { };
        public event EventHandler BrowseButtonClicked = delegate { };

        public SaveFileGUI() {
            InitializeComponent();
        }

        delegate void updateDataLogTextBoxDelegate(string tempText);
        public void updateDataLogTextBox(string tempText) {
            if(this.InvokeRequired) {
                updateDataLogTextBoxDelegate del = new updateDataLogTextBoxDelegate(updateDataLogTextBox);
                this.Invoke(del, new object[] { tempText });
            } else {
                this.dataLogTextBox.Text = tempText;
            }
        }

        delegate void updateECGLogTextBoxDelegate(string tempText);
        public void updateECGLogTextBox(string tempText) {
            if(this.InvokeRequired) {
                updateECGLogTextBoxDelegate del = new updateECGLogTextBoxDelegate(updateECGLogTextBox);
                this.Invoke(del, new object[] { tempText });
            } else {
                this.sleepLogTextBox.Text = tempText;
            }
        }

        delegate void updatefolderPathTextBoxDelegate(string tempText);
        public void updatefolderPathTextBox(string tempText) {
            if(this.InvokeRequired) {
                updatefolderPathTextBoxDelegate del = new updatefolderPathTextBoxDelegate(updatefolderPathTextBox);
                this.Invoke(del, new object[] { tempText });
            } else {
                this.folderPathTextBox.Text = tempText;
            }
        }

        private void saveButton_Click(object sender, EventArgs e) {
            SaveButtonClicked(this, EventArgs.Empty);
        }

        private void discardButton_Click(object sender, EventArgs e) {
            DiscardButtonClicked(this, EventArgs.Empty);
        }

        private void browseButton_Click(object sender, EventArgs e) {
            BrowseButtonClicked(this, EventArgs.Empty);
        }

        //if the user clicks the "X" button, treat it as discarding data. cancel the disposing of this window
        //so that this window can be opened again in the future
        private void formClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            DiscardButtonClicked(this, EventArgs.Empty);
        }


        
 
    }
}
