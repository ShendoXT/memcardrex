namespace MemcardRex.Windows.GUI
{
    partial class pocketStationInfo
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
            okButton = new System.Windows.Forms.Button();
            serialLabel = new System.Windows.Forms.Label();
            serialTextbox = new System.Windows.Forms.TextBox();
            versionTextbox = new System.Windows.Forms.TextBox();
            versionLabel = new System.Windows.Forms.Label();
            dateTextbox = new System.Windows.Forms.TextBox();
            dateLabel = new System.Windows.Forms.Label();
            checksumTextbox = new System.Windows.Forms.TextBox();
            checksumLabel = new System.Windows.Forms.Label();
            saveButton = new System.Windows.Forms.Button();
            remarkTextbox = new System.Windows.Forms.TextBox();
            remarkLabel = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // okButton
            // 
            okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            okButton.Location = new System.Drawing.Point(192, 148);
            okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.TabIndex = 0;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // serialLabel
            // 
            serialLabel.AutoSize = true;
            serialLabel.Location = new System.Drawing.Point(5, 14);
            serialLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            serialLabel.Name = "serialLabel";
            serialLabel.Size = new System.Drawing.Size(113, 15);
            serialLabel.TabIndex = 1;
            serialLabel.Text = "PocketStation serial:";
            // 
            // serialTextbox
            // 
            serialTextbox.BackColor = System.Drawing.SystemColors.Window;
            serialTextbox.Location = new System.Drawing.Point(131, 9);
            serialTextbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            serialTextbox.Name = "serialTextbox";
            serialTextbox.ReadOnly = true;
            serialTextbox.Size = new System.Drawing.Size(135, 23);
            serialTextbox.TabIndex = 2;
            // 
            // versionTextbox
            // 
            versionTextbox.BackColor = System.Drawing.SystemColors.Window;
            versionTextbox.Location = new System.Drawing.Point(131, 37);
            versionTextbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            versionTextbox.Name = "versionTextbox";
            versionTextbox.ReadOnly = true;
            versionTextbox.Size = new System.Drawing.Size(135, 23);
            versionTextbox.TabIndex = 4;
            versionTextbox.Visible = false;
            // 
            // versionLabel
            // 
            versionLabel.AutoSize = true;
            versionLabel.Location = new System.Drawing.Point(5, 42);
            versionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            versionLabel.Name = "versionLabel";
            versionLabel.Size = new System.Drawing.Size(76, 15);
            versionLabel.TabIndex = 3;
            versionLabel.Text = "BIOS version:";
            versionLabel.Visible = false;
            // 
            // dateTextbox
            // 
            dateTextbox.BackColor = System.Drawing.SystemColors.Window;
            dateTextbox.Location = new System.Drawing.Point(131, 65);
            dateTextbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dateTextbox.Name = "dateTextbox";
            dateTextbox.ReadOnly = true;
            dateTextbox.Size = new System.Drawing.Size(135, 23);
            dateTextbox.TabIndex = 6;
            dateTextbox.Visible = false;
            // 
            // dateLabel
            // 
            dateLabel.AutoSize = true;
            dateLabel.Location = new System.Drawing.Point(5, 69);
            dateLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            dateLabel.Name = "dateLabel";
            dateLabel.Size = new System.Drawing.Size(61, 15);
            dateLabel.TabIndex = 5;
            dateLabel.Text = "BIOS date:";
            dateLabel.Visible = false;
            // 
            // checksumTextbox
            // 
            checksumTextbox.BackColor = System.Drawing.SystemColors.Window;
            checksumTextbox.Location = new System.Drawing.Point(131, 92);
            checksumTextbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checksumTextbox.Name = "checksumTextbox";
            checksumTextbox.ReadOnly = true;
            checksumTextbox.Size = new System.Drawing.Size(135, 23);
            checksumTextbox.TabIndex = 8;
            checksumTextbox.Visible = false;
            // 
            // checksumLabel
            // 
            checksumLabel.AutoSize = true;
            checksumLabel.Location = new System.Drawing.Point(5, 97);
            checksumLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            checksumLabel.Name = "checksumLabel";
            checksumLabel.Size = new System.Drawing.Size(92, 15);
            checksumLabel.TabIndex = 7;
            checksumLabel.Text = "BIOS checksum:";
            checksumLabel.Visible = false;
            // 
            // saveButton
            // 
            saveButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            saveButton.Location = new System.Drawing.Point(4, 148);
            saveButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            saveButton.Name = "saveButton";
            saveButton.Size = new System.Drawing.Size(75, 23);
            saveButton.TabIndex = 9;
            saveButton.Text = "Save BIOS";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Visible = false;
            saveButton.Click += saveButton_Click;
            // 
            // remarkTextbox
            // 
            remarkTextbox.BackColor = System.Drawing.SystemColors.Window;
            remarkTextbox.Location = new System.Drawing.Point(131, 120);
            remarkTextbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            remarkTextbox.Name = "remarkTextbox";
            remarkTextbox.ReadOnly = true;
            remarkTextbox.Size = new System.Drawing.Size(135, 23);
            remarkTextbox.TabIndex = 11;
            remarkTextbox.Visible = false;
            // 
            // remarkLabel
            // 
            remarkLabel.AutoSize = true;
            remarkLabel.Location = new System.Drawing.Point(5, 125);
            remarkLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            remarkLabel.Name = "remarkLabel";
            remarkLabel.Size = new System.Drawing.Size(75, 15);
            remarkLabel.TabIndex = 10;
            remarkLabel.Text = "BIOS remark:";
            remarkLabel.Visible = false;
            // 
            // pocketStationInfo
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(272, 176);
            Controls.Add(remarkTextbox);
            Controls.Add(remarkLabel);
            Controls.Add(saveButton);
            Controls.Add(checksumTextbox);
            Controls.Add(checksumLabel);
            Controls.Add(dateTextbox);
            Controls.Add(dateLabel);
            Controls.Add(versionTextbox);
            Controls.Add(versionLabel);
            Controls.Add(serialTextbox);
            Controls.Add(serialLabel);
            Controls.Add(okButton);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "pocketStationInfo";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "pocketStationInfo";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label serialLabel;
        private System.Windows.Forms.TextBox serialTextbox;
        private System.Windows.Forms.TextBox versionTextbox;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.TextBox dateTextbox;
        private System.Windows.Forms.Label dateLabel;
        private System.Windows.Forms.TextBox checksumTextbox;
        private System.Windows.Forms.Label checksumLabel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TextBox remarkTextbox;
        private System.Windows.Forms.Label remarkLabel;
    }
}