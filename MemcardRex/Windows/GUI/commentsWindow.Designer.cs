namespace MemcardRex
{
    partial class commentsWindow
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
            commentsTextBox = new System.Windows.Forms.TextBox();
            warningLabel = new System.Windows.Forms.Label();
            cancelButton = new System.Windows.Forms.Button();
            okButton = new System.Windows.Forms.Button();
            spacerLabel = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // commentsTextBox
            // 
            commentsTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            commentsTextBox.Location = new System.Drawing.Point(4, 4);
            commentsTextBox.MaxLength = 255;
            commentsTextBox.Multiline = true;
            commentsTextBox.Name = "commentsTextBox";
            commentsTextBox.Size = new System.Drawing.Size(301, 100);
            commentsTextBox.TabIndex = 0;
            // 
            // warningLabel
            // 
            warningLabel.AutoSize = true;
            warningLabel.Location = new System.Drawing.Point(4, 108);
            warningLabel.Name = "warningLabel";
            warningLabel.Size = new System.Drawing.Size(299, 15);
            warningLabel.TabIndex = 10;
            warningLabel.Text = "Comments are only supported by DexDrive (.gme) files.";
            // 
            // cancelButton
            // 
            cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            cancelButton.Location = new System.Drawing.Point(229, 134);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(76, 23);
            cancelButton.TabIndex = 2;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // okButton
            // 
            okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            okButton.Location = new System.Drawing.Point(149, 134);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(76, 23);
            okButton.TabIndex = 1;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // spacerLabel
            // 
            spacerLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            spacerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            spacerLabel.Location = new System.Drawing.Point(4, 128);
            spacerLabel.Name = "spacerLabel";
            spacerLabel.Size = new System.Drawing.Size(301, 2);
            spacerLabel.TabIndex = 11;
            // 
            // commentsWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(309, 161);
            Controls.Add(spacerLabel);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(warningLabel);
            Controls.Add(commentsTextBox);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "commentsWindow";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "commentsWindow";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox commentsTextBox;
        private System.Windows.Forms.Label warningLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label spacerLabel;
    }
}