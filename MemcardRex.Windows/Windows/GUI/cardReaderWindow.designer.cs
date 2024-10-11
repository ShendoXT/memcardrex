using System;
using System.ComponentModel;

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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.deviceLabel = new System.Windows.Forms.Label();
            this.spacerLabel = new System.Windows.Forms.Label();
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
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(4, 24);
            this.progressBar.Maximum = 1024;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(300, 16);
            this.progressBar.TabIndex = 5;
            // 
            // deviceLabel
            // 
            this.deviceLabel.AutoSize = true;
            this.deviceLabel.Location = new System.Drawing.Point(4, 5);
            this.deviceLabel.Name = "deviceLabel";
            this.deviceLabel.Size = new System.Drawing.Size(106, 13);
            this.deviceLabel.TabIndex = 6;
            this.deviceLabel.Text = "infoLabelPlaceholder";
            this.deviceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spacerLabel
            // 
            this.spacerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.spacerLabel.Location = new System.Drawing.Point(4, 46);
            this.spacerLabel.Name = "spacerLabel";
            this.spacerLabel.Size = new System.Drawing.Size(300, 2);
            this.spacerLabel.TabIndex = 9;
            // 
            // cardReaderWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(308, 81);
            this.Controls.Add(this.spacerLabel);
            this.Controls.Add(this.deviceLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.abortButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "cardReaderWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "cardReaderWindow";
            this.Load += new System.EventHandler(this.cardReaderWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Button abortButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label deviceLabel;
        private System.Windows.Forms.Label spacerLabel;
    }
}