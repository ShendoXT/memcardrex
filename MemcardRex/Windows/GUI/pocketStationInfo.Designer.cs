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
            this.okButton = new System.Windows.Forms.Button();
            this.serialLabel = new System.Windows.Forms.Label();
            this.serialTextbox = new System.Windows.Forms.TextBox();
            this.versionTextbox = new System.Windows.Forms.TextBox();
            this.versionLabel = new System.Windows.Forms.Label();
            this.dateTextbox = new System.Windows.Forms.TextBox();
            this.dateLabel = new System.Windows.Forms.Label();
            this.checksumTextbox = new System.Windows.Forms.TextBox();
            this.checksumLabel = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.remarkTextbox = new System.Windows.Forms.TextBox();
            this.remarkLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(152, 128);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // serialLabel
            // 
            this.serialLabel.AutoSize = true;
            this.serialLabel.Location = new System.Drawing.Point(4, 12);
            this.serialLabel.Name = "serialLabel";
            this.serialLabel.Size = new System.Drawing.Size(104, 13);
            this.serialLabel.TabIndex = 1;
            this.serialLabel.Text = "PocketStation serial:";
            // 
            // serialTextbox
            // 
            this.serialTextbox.BackColor = System.Drawing.SystemColors.Window;
            this.serialTextbox.Location = new System.Drawing.Point(112, 8);
            this.serialTextbox.Name = "serialTextbox";
            this.serialTextbox.ReadOnly = true;
            this.serialTextbox.Size = new System.Drawing.Size(116, 20);
            this.serialTextbox.TabIndex = 2;
            // 
            // versionTextbox
            // 
            this.versionTextbox.BackColor = System.Drawing.SystemColors.Window;
            this.versionTextbox.Location = new System.Drawing.Point(112, 32);
            this.versionTextbox.Name = "versionTextbox";
            this.versionTextbox.ReadOnly = true;
            this.versionTextbox.Size = new System.Drawing.Size(116, 20);
            this.versionTextbox.TabIndex = 4;
            this.versionTextbox.Visible = false;
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(4, 36);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(72, 13);
            this.versionLabel.TabIndex = 3;
            this.versionLabel.Text = "BIOS version:";
            this.versionLabel.Visible = false;
            // 
            // dateTextbox
            // 
            this.dateTextbox.BackColor = System.Drawing.SystemColors.Window;
            this.dateTextbox.Location = new System.Drawing.Point(112, 56);
            this.dateTextbox.Name = "dateTextbox";
            this.dateTextbox.ReadOnly = true;
            this.dateTextbox.Size = new System.Drawing.Size(116, 20);
            this.dateTextbox.TabIndex = 6;
            this.dateTextbox.Visible = false;
            // 
            // dateLabel
            // 
            this.dateLabel.AutoSize = true;
            this.dateLabel.Location = new System.Drawing.Point(4, 60);
            this.dateLabel.Name = "dateLabel";
            this.dateLabel.Size = new System.Drawing.Size(59, 13);
            this.dateLabel.TabIndex = 5;
            this.dateLabel.Text = "BIOS date:";
            this.dateLabel.Visible = false;
            // 
            // checksumTextbox
            // 
            this.checksumTextbox.BackColor = System.Drawing.SystemColors.Window;
            this.checksumTextbox.Location = new System.Drawing.Point(112, 80);
            this.checksumTextbox.Name = "checksumTextbox";
            this.checksumTextbox.ReadOnly = true;
            this.checksumTextbox.Size = new System.Drawing.Size(116, 20);
            this.checksumTextbox.TabIndex = 8;
            this.checksumTextbox.Visible = false;
            // 
            // checksumLabel
            // 
            this.checksumLabel.AutoSize = true;
            this.checksumLabel.Location = new System.Drawing.Point(4, 84);
            this.checksumLabel.Name = "checksumLabel";
            this.checksumLabel.Size = new System.Drawing.Size(87, 13);
            this.checksumLabel.TabIndex = 7;
            this.checksumLabel.Text = "BIOS checksum:";
            this.checksumLabel.Visible = false;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveButton.Location = new System.Drawing.Point(4, 128);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 9;
            this.saveButton.Text = "Save BIOS";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Visible = false;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // remarkTextbox
            // 
            this.remarkTextbox.BackColor = System.Drawing.SystemColors.Window;
            this.remarkTextbox.Location = new System.Drawing.Point(112, 104);
            this.remarkTextbox.Name = "remarkTextbox";
            this.remarkTextbox.ReadOnly = true;
            this.remarkTextbox.Size = new System.Drawing.Size(116, 20);
            this.remarkTextbox.TabIndex = 11;
            this.remarkTextbox.Visible = false;
            // 
            // remarkLabel
            // 
            this.remarkLabel.AutoSize = true;
            this.remarkLabel.Location = new System.Drawing.Point(4, 108);
            this.remarkLabel.Name = "remarkLabel";
            this.remarkLabel.Size = new System.Drawing.Size(70, 13);
            this.remarkLabel.TabIndex = 10;
            this.remarkLabel.Text = "BIOS remark:";
            this.remarkLabel.Visible = false;
            // 
            // pocketStationInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(233, 157);
            this.Controls.Add(this.remarkTextbox);
            this.Controls.Add(this.remarkLabel);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.checksumTextbox);
            this.Controls.Add(this.checksumLabel);
            this.Controls.Add(this.dateTextbox);
            this.Controls.Add(this.dateLabel);
            this.Controls.Add(this.versionTextbox);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.serialTextbox);
            this.Controls.Add(this.serialLabel);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "pocketStationInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "pocketStationInfo";
            this.ResumeLayout(false);
            this.PerformLayout();

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