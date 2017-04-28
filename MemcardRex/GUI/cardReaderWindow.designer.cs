namespace MemcardRex
{
    partial class cardReaderWindow
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
            this.abortButton = new System.Windows.Forms.Button();
            this.mainProgressBar = new System.Windows.Forms.ProgressBar();
            this.backgroundReader = new System.ComponentModel.BackgroundWorker();
            this.infoLabel = new System.Windows.Forms.Label();
            this.spacerLabel = new System.Windows.Forms.Label();
            this.backgroundWriter = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // abortButton
            // 
            this.abortButton.Location = new System.Drawing.Point(228, 52);
            this.abortButton.Name = "abortButton";
            this.abortButton.Size = new System.Drawing.Size(76, 24);
            this.abortButton.TabIndex = 0;
            this.abortButton.Text = "Abort";
            this.abortButton.UseVisualStyleBackColor = true;
            this.abortButton.Click += new System.EventHandler(this.OKbutton_Click);
            // 
            // mainProgressBar
            // 
            this.mainProgressBar.Location = new System.Drawing.Point(4, 24);
            this.mainProgressBar.Maximum = 1024;
            this.mainProgressBar.Name = "mainProgressBar";
            this.mainProgressBar.Size = new System.Drawing.Size(300, 16);
            this.mainProgressBar.TabIndex = 5;
            // 
            // backgroundReader
            // 
            this.backgroundReader.WorkerReportsProgress = true;
            this.backgroundReader.WorkerSupportsCancellation = true;
            this.backgroundReader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundReader_DoWork);
            this.backgroundReader.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundReader_ProgressChanged);
            this.backgroundReader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundReader_RunWorkerCompleted);
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Location = new System.Drawing.Point(4, 5);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(106, 13);
            this.infoLabel.TabIndex = 6;
            this.infoLabel.Text = "infoLabelPlaceholder";
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spacerLabel
            // 
            this.spacerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.spacerLabel.Location = new System.Drawing.Point(4, 46);
            this.spacerLabel.Name = "spacerLabel";
            this.spacerLabel.Size = new System.Drawing.Size(300, 2);
            this.spacerLabel.TabIndex = 9;
            // 
            // backgroundWriter
            // 
            this.backgroundWriter.WorkerReportsProgress = true;
            this.backgroundWriter.WorkerSupportsCancellation = true;
            this.backgroundWriter.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWriter_DoWork);
            this.backgroundWriter.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWriter_ProgressChanged);
            this.backgroundWriter.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWriter_RunWorkerCompleted);
            // 
            // cardReaderWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(308, 81);
            this.ControlBox = false;
            this.Controls.Add(this.spacerLabel);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.mainProgressBar);
            this.Controls.Add(this.abortButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "cardReaderWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "cardReaderWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button abortButton;
        private System.Windows.Forms.ProgressBar mainProgressBar;
        private System.ComponentModel.BackgroundWorker backgroundReader;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Label spacerLabel;
        private System.ComponentModel.BackgroundWorker backgroundWriter;
    }
}