namespace MemcardRex
{
    partial class preferencesWindow
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.fontCombo = new System.Windows.Forms.ComboBox();
            this.fontLabel = new System.Windows.Forms.Label();
            this.gridCheckbox = new System.Windows.Forms.CheckBox();
            this.iconSizeLabel = new System.Windows.Forms.Label();
            this.iconSizeCombo = new System.Windows.Forms.ComboBox();
            this.interpolationCombo = new System.Windows.Forms.ComboBox();
            this.interpolationLabel = new System.Windows.Forms.Label();
            this.backupWarningCheckBox = new System.Windows.Forms.CheckBox();
            this.backupCheckbox = new System.Windows.Forms.CheckBox();
            this.dexDriveCombo = new System.Windows.Forms.ComboBox();
            this.hardwarePortLabel = new System.Windows.Forms.Label();
            this.spacerLabel = new System.Windows.Forms.Label();
            this.restorePositionCheckbox = new System.Windows.Forms.CheckBox();
            this.formatCombo = new System.Windows.Forms.ComboBox();
            this.formatLabel = new System.Windows.Forms.Label();
            this.backgroundCombo = new System.Windows.Forms.ComboBox();
            this.backgroundLabel = new System.Windows.Forms.Label();
            this.fixCorruptedCardsCheckbox = new System.Windows.Forms.CheckBox();
            this.remotePortLabel = new System.Windows.Forms.Label();
            this.remoteAddressLabel = new System.Windows.Forms.Label();
            this.remoteAddressBox = new System.Windows.Forms.TextBox();
            this.remotePortUpDown = new System.Windows.Forms.NumericUpDown();
            this.hardwareInterfacesCombo = new System.Windows.Forms.ComboBox();
            this.hardwareSpeedLabel = new System.Windows.Forms.Label();
            this.cardSlotLabel = new System.Windows.Forms.Label();
            this.cardSlotCombo = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.remotePortUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okButton.Location = new System.Drawing.Point(252, 228);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(76, 24);
            this.okButton.TabIndex = 99;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(332, 228);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(76, 24);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // applyButton
            // 
            this.applyButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.applyButton.Location = new System.Drawing.Point(412, 228);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(76, 24);
            this.applyButton.TabIndex = 1;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // fontCombo
            // 
            this.fontCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontCombo.FormattingEnabled = true;
            this.fontCombo.Location = new System.Drawing.Point(4, 20);
            this.fontCombo.Name = "fontCombo";
            this.fontCombo.Size = new System.Drawing.Size(236, 21);
            this.fontCombo.TabIndex = 3;
            // 
            // fontLabel
            // 
            this.fontLabel.AutoSize = true;
            this.fontLabel.Location = new System.Drawing.Point(4, 4);
            this.fontLabel.Name = "fontLabel";
            this.fontLabel.Size = new System.Drawing.Size(75, 13);
            this.fontLabel.TabIndex = 3;
            this.fontLabel.Text = "Save title font:";
            // 
            // gridCheckbox
            // 
            this.gridCheckbox.AutoSize = true;
            this.gridCheckbox.Location = new System.Drawing.Point(248, 12);
            this.gridCheckbox.Name = "gridCheckbox";
            this.gridCheckbox.Size = new System.Drawing.Size(125, 17);
            this.gridCheckbox.TabIndex = 9;
            this.gridCheckbox.Text = "Show grid on slot list.";
            this.gridCheckbox.UseVisualStyleBackColor = true;
            // 
            // iconSizeLabel
            // 
            this.iconSizeLabel.AutoSize = true;
            this.iconSizeLabel.Location = new System.Drawing.Point(124, 48);
            this.iconSizeLabel.Name = "iconSizeLabel";
            this.iconSizeLabel.Size = new System.Drawing.Size(52, 13);
            this.iconSizeLabel.TabIndex = 5;
            this.iconSizeLabel.Text = "Icon size:";
            // 
            // iconSizeCombo
            // 
            this.iconSizeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.iconSizeCombo.FormattingEnabled = true;
            this.iconSizeCombo.Items.AddRange(new object[] {
            "Medium",
            "Large"});
            this.iconSizeCombo.Location = new System.Drawing.Point(124, 64);
            this.iconSizeCombo.Name = "iconSizeCombo";
            this.iconSizeCombo.Size = new System.Drawing.Size(116, 21);
            this.iconSizeCombo.TabIndex = 5;
            // 
            // interpolationCombo
            // 
            this.interpolationCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.interpolationCombo.FormattingEnabled = true;
            this.interpolationCombo.Items.AddRange(new object[] {
            "Nearest Neighbor",
            "Bilinear"});
            this.interpolationCombo.Location = new System.Drawing.Point(4, 64);
            this.interpolationCombo.Name = "interpolationCombo";
            this.interpolationCombo.Size = new System.Drawing.Size(116, 21);
            this.interpolationCombo.TabIndex = 4;
            // 
            // interpolationLabel
            // 
            this.interpolationLabel.AutoSize = true;
            this.interpolationLabel.Location = new System.Drawing.Point(4, 48);
            this.interpolationLabel.Name = "interpolationLabel";
            this.interpolationLabel.Size = new System.Drawing.Size(53, 13);
            this.interpolationLabel.TabIndex = 0;
            this.interpolationLabel.Text = "Icon filter:";
            // 
            // backupWarningCheckBox
            // 
            this.backupWarningCheckBox.AutoSize = true;
            this.backupWarningCheckBox.Location = new System.Drawing.Point(248, 60);
            this.backupWarningCheckBox.Name = "backupWarningCheckBox";
            this.backupWarningCheckBox.Size = new System.Drawing.Size(212, 17);
            this.backupWarningCheckBox.TabIndex = 11;
            this.backupWarningCheckBox.Text = "Show warning messages (save editing).";
            this.backupWarningCheckBox.UseVisualStyleBackColor = true;
            // 
            // backupCheckbox
            // 
            this.backupCheckbox.AutoSize = true;
            this.backupCheckbox.Location = new System.Drawing.Point(248, 36);
            this.backupCheckbox.Name = "backupCheckbox";
            this.backupCheckbox.Size = new System.Drawing.Size(204, 17);
            this.backupCheckbox.TabIndex = 10;
            this.backupCheckbox.Text = "Backup Memory Cards upon opening.";
            this.backupCheckbox.UseVisualStyleBackColor = true;
            // 
            // dexDriveCombo
            // 
            this.dexDriveCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dexDriveCombo.FormattingEnabled = true;
            this.dexDriveCombo.Location = new System.Drawing.Point(4, 196);
            this.dexDriveCombo.Name = "dexDriveCombo";
            this.dexDriveCombo.Size = new System.Drawing.Size(68, 21);
            this.dexDriveCombo.TabIndex = 7;
            this.dexDriveCombo.SelectedIndexChanged += new System.EventHandler(this.dexDriveCombo_SelectedIndexChanged);
            // 
            // hardwarePortLabel
            // 
            this.hardwarePortLabel.AutoSize = true;
            this.hardwarePortLabel.Location = new System.Drawing.Point(4, 180);
            this.hardwarePortLabel.Name = "hardwarePortLabel";
            this.hardwarePortLabel.Size = new System.Drawing.Size(49, 13);
            this.hardwarePortLabel.TabIndex = 6;
            this.hardwarePortLabel.Text = "Comport:";
            // 
            // spacerLabel
            // 
            this.spacerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.spacerLabel.Location = new System.Drawing.Point(4, 223);
            this.spacerLabel.Name = "spacerLabel";
            this.spacerLabel.Size = new System.Drawing.Size(484, 2);
            this.spacerLabel.TabIndex = 8;
            // 
            // restorePositionCheckbox
            // 
            this.restorePositionCheckbox.AutoSize = true;
            this.restorePositionCheckbox.Location = new System.Drawing.Point(248, 84);
            this.restorePositionCheckbox.Name = "restorePositionCheckbox";
            this.restorePositionCheckbox.Size = new System.Drawing.Size(191, 17);
            this.restorePositionCheckbox.TabIndex = 13;
            this.restorePositionCheckbox.Text = "Restore window position on startup";
            this.restorePositionCheckbox.UseVisualStyleBackColor = true;
            // 
            // formatCombo
            // 
            this.formatCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.formatCombo.FormattingEnabled = true;
            this.formatCombo.Items.AddRange(new object[] {
            "Quick format",
            "Full format"});
            this.formatCombo.Location = new System.Drawing.Point(4, 152);
            this.formatCombo.Name = "formatCombo";
            this.formatCombo.Size = new System.Drawing.Size(236, 21);
            this.formatCombo.TabIndex = 8;
            // 
            // formatLabel
            // 
            this.formatLabel.AutoSize = true;
            this.formatLabel.Location = new System.Drawing.Point(4, 136);
            this.formatLabel.Name = "formatLabel";
            this.formatLabel.Size = new System.Drawing.Size(111, 13);
            this.formatLabel.TabIndex = 101;
            this.formatLabel.Text = "Hardware format type:";
            // 
            // backgroundCombo
            // 
            this.backgroundCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.backgroundCombo.FormattingEnabled = true;
            this.backgroundCombo.Items.AddRange(new object[] {
            "Transparent",
            "Black (Slim PS1 models)",
            "Gray (Older european PS1 models)",
            "Blue (Standard BIOS color)"});
            this.backgroundCombo.Location = new System.Drawing.Point(4, 108);
            this.backgroundCombo.Name = "backgroundCombo";
            this.backgroundCombo.Size = new System.Drawing.Size(236, 21);
            this.backgroundCombo.TabIndex = 6;
            // 
            // backgroundLabel
            // 
            this.backgroundLabel.AutoSize = true;
            this.backgroundLabel.Location = new System.Drawing.Point(4, 92);
            this.backgroundLabel.Name = "backgroundLabel";
            this.backgroundLabel.Size = new System.Drawing.Size(117, 13);
            this.backgroundLabel.TabIndex = 102;
            this.backgroundLabel.Text = "Icon background color:";
            // 
            // fixCorruptedCardsCheckbox
            // 
            this.fixCorruptedCardsCheckbox.AutoSize = true;
            this.fixCorruptedCardsCheckbox.Location = new System.Drawing.Point(248, 108);
            this.fixCorruptedCardsCheckbox.Name = "fixCorruptedCardsCheckbox";
            this.fixCorruptedCardsCheckbox.Size = new System.Drawing.Size(184, 17);
            this.fixCorruptedCardsCheckbox.TabIndex = 103;
            this.fixCorruptedCardsCheckbox.Text = "Try to fix corrupted Memory Cards";
            this.fixCorruptedCardsCheckbox.UseVisualStyleBackColor = true;
            // 
            // remotePortLabel
            // 
            this.remotePortLabel.AutoSize = true;
            this.remotePortLabel.Location = new System.Drawing.Point(365, 179);
            this.remotePortLabel.Name = "remotePortLabel";
            this.remotePortLabel.Size = new System.Drawing.Size(69, 13);
            this.remotePortLabel.TabIndex = 105;
            this.remotePortLabel.Text = "Remote Port:";
            // 
            // remoteAddressLabel
            // 
            this.remoteAddressLabel.AutoSize = true;
            this.remoteAddressLabel.Location = new System.Drawing.Point(245, 179);
            this.remoteAddressLabel.Name = "remoteAddressLabel";
            this.remoteAddressLabel.Size = new System.Drawing.Size(88, 13);
            this.remoteAddressLabel.TabIndex = 104;
            this.remoteAddressLabel.Text = "Remote Address:";
            // 
            // remoteAddressBox
            // 
            this.remoteAddressBox.Location = new System.Drawing.Point(248, 196);
            this.remoteAddressBox.Name = "remoteAddressBox";
            this.remoteAddressBox.Size = new System.Drawing.Size(113, 20);
            this.remoteAddressBox.TabIndex = 106;
            // 
            // remotePortUpDown
            // 
            this.remotePortUpDown.Location = new System.Drawing.Point(365, 196);
            this.remotePortUpDown.Name = "remotePortUpDown";
            this.remotePortUpDown.Size = new System.Drawing.Size(120, 20);
            this.remotePortUpDown.TabIndex = 107;
            // 
            // hardwareInterfacesCombo
            // 
            this.hardwareInterfacesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.hardwareInterfacesCombo.FormattingEnabled = true;
            this.hardwareInterfacesCombo.Location = new System.Drawing.Point(76, 196);
            this.hardwareInterfacesCombo.Name = "hardwareInterfacesCombo";
            this.hardwareInterfacesCombo.Size = new System.Drawing.Size(164, 21);
            this.hardwareInterfacesCombo.TabIndex = 109;
            // 
            // hardwareSpeedLabel
            // 
            this.hardwareSpeedLabel.AutoSize = true;
            this.hardwareSpeedLabel.Location = new System.Drawing.Point(76, 180);
            this.hardwareSpeedLabel.Name = "hardwareSpeedLabel";
            this.hardwareSpeedLabel.Size = new System.Drawing.Size(131, 13);
            this.hardwareSpeedLabel.TabIndex = 108;
            this.hardwareSpeedLabel.Text = "Active hardware interface:";
            // 
            // cardSlotLabel
            // 
            this.cardSlotLabel.AutoSize = true;
            this.cardSlotLabel.Location = new System.Drawing.Point(248, 136);
            this.cardSlotLabel.Name = "cardSlotLabel";
            this.cardSlotLabel.Size = new System.Drawing.Size(159, 13);
            this.cardSlotLabel.TabIndex = 111;
            this.cardSlotLabel.Text = "Unirom / PS1CardLink card slot:";
            // 
            // cardSlotCombo
            // 
            this.cardSlotCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cardSlotCombo.FormattingEnabled = true;
            this.cardSlotCombo.Items.AddRange(new object[] {
            "Slot 1",
            "Slot 2"});
            this.cardSlotCombo.Location = new System.Drawing.Point(248, 152);
            this.cardSlotCombo.Name = "cardSlotCombo";
            this.cardSlotCombo.Size = new System.Drawing.Size(236, 21);
            this.cardSlotCombo.TabIndex = 110;
            // 
            // preferencesWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 257);
            this.Controls.Add(this.cardSlotLabel);
            this.Controls.Add(this.cardSlotCombo);
            this.Controls.Add(this.hardwareInterfacesCombo);
            this.Controls.Add(this.hardwareSpeedLabel);
            this.Controls.Add(this.remotePortUpDown);
            this.Controls.Add(this.remoteAddressBox);
            this.Controls.Add(this.remotePortLabel);
            this.Controls.Add(this.remoteAddressLabel);
            this.Controls.Add(this.fixCorruptedCardsCheckbox);
            this.Controls.Add(this.backgroundCombo);
            this.Controls.Add(this.backgroundLabel);
            this.Controls.Add(this.formatLabel);
            this.Controls.Add(this.formatCombo);
            this.Controls.Add(this.restorePositionCheckbox);
            this.Controls.Add(this.spacerLabel);
            this.Controls.Add(this.gridCheckbox);
            this.Controls.Add(this.backupWarningCheckBox);
            this.Controls.Add(this.iconSizeLabel);
            this.Controls.Add(this.backupCheckbox);
            this.Controls.Add(this.iconSizeCombo);
            this.Controls.Add(this.fontLabel);
            this.Controls.Add(this.interpolationCombo);
            this.Controls.Add(this.dexDriveCombo);
            this.Controls.Add(this.interpolationLabel);
            this.Controls.Add(this.fontCombo);
            this.Controls.Add(this.hardwarePortLabel);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "preferencesWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preferences";
            ((System.ComponentModel.ISupportInitialize)(this.remotePortUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.CheckBox gridCheckbox;
        private System.Windows.Forms.ComboBox fontCombo;
        private System.Windows.Forms.Label fontLabel;
        private System.Windows.Forms.ComboBox interpolationCombo;
        private System.Windows.Forms.Label interpolationLabel;
        private System.Windows.Forms.CheckBox backupCheckbox;
        private System.Windows.Forms.CheckBox backupWarningCheckBox;
        private System.Windows.Forms.Label iconSizeLabel;
        private System.Windows.Forms.ComboBox iconSizeCombo;
        private System.Windows.Forms.ComboBox dexDriveCombo;
        private System.Windows.Forms.Label hardwarePortLabel;
        private System.Windows.Forms.Label spacerLabel;
        private System.Windows.Forms.CheckBox restorePositionCheckbox;
        private System.Windows.Forms.ComboBox formatCombo;
        private System.Windows.Forms.Label formatLabel;
        private System.Windows.Forms.ComboBox backgroundCombo;
        private System.Windows.Forms.Label backgroundLabel;
        private System.Windows.Forms.CheckBox fixCorruptedCardsCheckbox;
        private System.Windows.Forms.Label remotePortLabel;
        private System.Windows.Forms.Label remoteAddressLabel;
        private System.Windows.Forms.TextBox remoteAddressBox;
        private System.Windows.Forms.NumericUpDown remotePortUpDown;
        private System.Windows.Forms.ComboBox hardwareInterfacesCombo;
        private System.Windows.Forms.Label hardwareSpeedLabel;
        private System.Windows.Forms.Label cardSlotLabel;
        private System.Windows.Forms.ComboBox cardSlotCombo;
    }
}