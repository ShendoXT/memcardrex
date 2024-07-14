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
            this.hardwareSpeedCombo = new System.Windows.Forms.ComboBox();
            this.hardwareSpeedLabel = new System.Windows.Forms.Label();
            this.cardSlotLabel = new System.Windows.Forms.Label();
            this.cardSlotCombo = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.remotePortUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okButton.Location = new System.Drawing.Point(504, 438);
            this.okButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(152, 46);
            this.okButton.TabIndex = 99;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(664, 438);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(152, 46);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // applyButton
            // 
            this.applyButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.applyButton.Location = new System.Drawing.Point(824, 438);
            this.applyButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(152, 46);
            this.applyButton.TabIndex = 1;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // fontCombo
            // 
            this.fontCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontCombo.FormattingEnabled = true;
            this.fontCombo.Location = new System.Drawing.Point(8, 38);
            this.fontCombo.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.fontCombo.Name = "fontCombo";
            this.fontCombo.Size = new System.Drawing.Size(468, 33);
            this.fontCombo.TabIndex = 3;
            // 
            // fontLabel
            // 
            this.fontLabel.AutoSize = true;
            this.fontLabel.Location = new System.Drawing.Point(8, 8);
            this.fontLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.fontLabel.Name = "fontLabel";
            this.fontLabel.Size = new System.Drawing.Size(149, 25);
            this.fontLabel.TabIndex = 3;
            this.fontLabel.Text = "Save title font:";
            // 
            // gridCheckbox
            // 
            this.gridCheckbox.AutoSize = true;
            this.gridCheckbox.Location = new System.Drawing.Point(496, 23);
            this.gridCheckbox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gridCheckbox.Name = "gridCheckbox";
            this.gridCheckbox.Size = new System.Drawing.Size(248, 29);
            this.gridCheckbox.TabIndex = 9;
            this.gridCheckbox.Text = "Show grid on slot list.";
            this.gridCheckbox.UseVisualStyleBackColor = true;
            // 
            // iconSizeLabel
            // 
            this.iconSizeLabel.AutoSize = true;
            this.iconSizeLabel.Location = new System.Drawing.Point(248, 92);
            this.iconSizeLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.iconSizeLabel.Name = "iconSizeLabel";
            this.iconSizeLabel.Size = new System.Drawing.Size(103, 25);
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
            this.iconSizeCombo.Location = new System.Drawing.Point(248, 123);
            this.iconSizeCombo.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.iconSizeCombo.Name = "iconSizeCombo";
            this.iconSizeCombo.Size = new System.Drawing.Size(228, 33);
            this.iconSizeCombo.TabIndex = 5;
            // 
            // interpolationCombo
            // 
            this.interpolationCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.interpolationCombo.FormattingEnabled = true;
            this.interpolationCombo.Items.AddRange(new object[] {
            "Nearest Neighbor",
            "Bilinear"});
            this.interpolationCombo.Location = new System.Drawing.Point(8, 123);
            this.interpolationCombo.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.interpolationCombo.Name = "interpolationCombo";
            this.interpolationCombo.Size = new System.Drawing.Size(228, 33);
            this.interpolationCombo.TabIndex = 4;
            // 
            // interpolationLabel
            // 
            this.interpolationLabel.AutoSize = true;
            this.interpolationLabel.Location = new System.Drawing.Point(8, 92);
            this.interpolationLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.interpolationLabel.Name = "interpolationLabel";
            this.interpolationLabel.Size = new System.Drawing.Size(105, 25);
            this.interpolationLabel.TabIndex = 0;
            this.interpolationLabel.Text = "Icon filter:";
            // 
            // backupWarningCheckBox
            // 
            this.backupWarningCheckBox.AutoSize = true;
            this.backupWarningCheckBox.Location = new System.Drawing.Point(496, 115);
            this.backupWarningCheckBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.backupWarningCheckBox.Name = "backupWarningCheckBox";
            this.backupWarningCheckBox.Size = new System.Drawing.Size(424, 29);
            this.backupWarningCheckBox.TabIndex = 11;
            this.backupWarningCheckBox.Text = "Show warning messages (save editing).";
            this.backupWarningCheckBox.UseVisualStyleBackColor = true;
            // 
            // backupCheckbox
            // 
            this.backupCheckbox.AutoSize = true;
            this.backupCheckbox.Location = new System.Drawing.Point(496, 69);
            this.backupCheckbox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.backupCheckbox.Name = "backupCheckbox";
            this.backupCheckbox.Size = new System.Drawing.Size(405, 29);
            this.backupCheckbox.TabIndex = 10;
            this.backupCheckbox.Text = "Backup Memory Cards upon opening.";
            this.backupCheckbox.UseVisualStyleBackColor = true;
            // 
            // dexDriveCombo
            // 
            this.dexDriveCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dexDriveCombo.FormattingEnabled = true;
            this.dexDriveCombo.Location = new System.Drawing.Point(8, 377);
            this.dexDriveCombo.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dexDriveCombo.Name = "dexDriveCombo";
            this.dexDriveCombo.Size = new System.Drawing.Size(228, 33);
            this.dexDriveCombo.TabIndex = 7;
            this.dexDriveCombo.SelectedIndexChanged += new System.EventHandler(this.dexDriveCombo_SelectedIndexChanged);
            // 
            // hardwarePortLabel
            // 
            this.hardwarePortLabel.AutoSize = true;
            this.hardwarePortLabel.Location = new System.Drawing.Point(8, 346);
            this.hardwarePortLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.hardwarePortLabel.Name = "hardwarePortLabel";
            this.hardwarePortLabel.Size = new System.Drawing.Size(209, 25);
            this.hardwarePortLabel.TabIndex = 6;
            this.hardwarePortLabel.Text = "Communication port:";
            // 
            // spacerLabel
            // 
            this.spacerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.spacerLabel.Location = new System.Drawing.Point(8, 429);
            this.spacerLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.spacerLabel.Name = "spacerLabel";
            this.spacerLabel.Size = new System.Drawing.Size(968, 4);
            this.spacerLabel.TabIndex = 8;
            // 
            // restorePositionCheckbox
            // 
            this.restorePositionCheckbox.AutoSize = true;
            this.restorePositionCheckbox.Location = new System.Drawing.Point(496, 162);
            this.restorePositionCheckbox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.restorePositionCheckbox.Name = "restorePositionCheckbox";
            this.restorePositionCheckbox.Size = new System.Drawing.Size(379, 29);
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
            this.formatCombo.Location = new System.Drawing.Point(8, 292);
            this.formatCombo.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.formatCombo.Name = "formatCombo";
            this.formatCombo.Size = new System.Drawing.Size(468, 33);
            this.formatCombo.TabIndex = 8;
            // 
            // formatLabel
            // 
            this.formatLabel.AutoSize = true;
            this.formatLabel.Location = new System.Drawing.Point(8, 262);
            this.formatLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.formatLabel.Name = "formatLabel";
            this.formatLabel.Size = new System.Drawing.Size(223, 25);
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
            this.backgroundCombo.Location = new System.Drawing.Point(8, 208);
            this.backgroundCombo.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.backgroundCombo.Name = "backgroundCombo";
            this.backgroundCombo.Size = new System.Drawing.Size(468, 33);
            this.backgroundCombo.TabIndex = 6;
            // 
            // backgroundLabel
            // 
            this.backgroundLabel.AutoSize = true;
            this.backgroundLabel.Location = new System.Drawing.Point(8, 177);
            this.backgroundLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.backgroundLabel.Name = "backgroundLabel";
            this.backgroundLabel.Size = new System.Drawing.Size(230, 25);
            this.backgroundLabel.TabIndex = 102;
            this.backgroundLabel.Text = "Icon background color:";
            // 
            // fixCorruptedCardsCheckbox
            // 
            this.fixCorruptedCardsCheckbox.AutoSize = true;
            this.fixCorruptedCardsCheckbox.Location = new System.Drawing.Point(496, 208);
            this.fixCorruptedCardsCheckbox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.fixCorruptedCardsCheckbox.Name = "fixCorruptedCardsCheckbox";
            this.fixCorruptedCardsCheckbox.Size = new System.Drawing.Size(370, 29);
            this.fixCorruptedCardsCheckbox.TabIndex = 103;
            this.fixCorruptedCardsCheckbox.Text = "Try to fix corrupted Memory Cards";
            this.fixCorruptedCardsCheckbox.UseVisualStyleBackColor = true;
            // 
            // remotePortLabel
            // 
            this.remotePortLabel.AutoSize = true;
            this.remotePortLabel.Location = new System.Drawing.Point(730, 344);
            this.remotePortLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.remotePortLabel.Name = "remotePortLabel";
            this.remotePortLabel.Size = new System.Drawing.Size(137, 25);
            this.remotePortLabel.TabIndex = 105;
            this.remotePortLabel.Text = "Remote Port:";
            // 
            // remoteAddressLabel
            // 
            this.remoteAddressLabel.AutoSize = true;
            this.remoteAddressLabel.Location = new System.Drawing.Point(490, 344);
            this.remoteAddressLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.remoteAddressLabel.Name = "remoteAddressLabel";
            this.remoteAddressLabel.Size = new System.Drawing.Size(177, 25);
            this.remoteAddressLabel.TabIndex = 104;
            this.remoteAddressLabel.Text = "Remote Address:";
            // 
            // remoteAddressBox
            // 
            this.remoteAddressBox.Location = new System.Drawing.Point(496, 377);
            this.remoteAddressBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.remoteAddressBox.Name = "remoteAddressBox";
            this.remoteAddressBox.Size = new System.Drawing.Size(222, 31);
            this.remoteAddressBox.TabIndex = 106;
            // 
            // remotePortUpDown
            // 
            this.remotePortUpDown.Location = new System.Drawing.Point(730, 377);
            this.remotePortUpDown.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.remotePortUpDown.Name = "remotePortUpDown";
            this.remotePortUpDown.Size = new System.Drawing.Size(240, 31);
            this.remotePortUpDown.TabIndex = 107;
            // 
            // hardwareSpeedCombo
            // 
            this.hardwareSpeedCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.hardwareSpeedCombo.FormattingEnabled = true;
            this.hardwareSpeedCombo.Items.AddRange(new object[] {
            "115200 bps",
            "38400 bps (legacy)"});
            this.hardwareSpeedCombo.Location = new System.Drawing.Point(248, 377);
            this.hardwareSpeedCombo.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.hardwareSpeedCombo.Name = "hardwareSpeedCombo";
            this.hardwareSpeedCombo.Size = new System.Drawing.Size(228, 33);
            this.hardwareSpeedCombo.TabIndex = 109;
            // 
            // hardwareSpeedLabel
            // 
            this.hardwareSpeedLabel.AutoSize = true;
            this.hardwareSpeedLabel.Location = new System.Drawing.Point(248, 346);
            this.hardwareSpeedLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.hardwareSpeedLabel.Name = "hardwareSpeedLabel";
            this.hardwareSpeedLabel.Size = new System.Drawing.Size(231, 25);
            this.hardwareSpeedLabel.TabIndex = 108;
            this.hardwareSpeedLabel.Text = "Communication speed:";
            // 
            // cardSlotLabel
            // 
            this.cardSlotLabel.AutoSize = true;
            this.cardSlotLabel.Location = new System.Drawing.Point(496, 262);
            this.cardSlotLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.cardSlotLabel.Name = "cardSlotLabel";
            this.cardSlotLabel.Size = new System.Drawing.Size(318, 25);
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
            this.cardSlotCombo.Location = new System.Drawing.Point(496, 292);
            this.cardSlotCombo.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cardSlotCombo.Name = "cardSlotCombo";
            this.cardSlotCombo.Size = new System.Drawing.Size(468, 33);
            this.cardSlotCombo.TabIndex = 110;
            // 
            // preferencesWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 494);
            this.Controls.Add(this.cardSlotLabel);
            this.Controls.Add(this.cardSlotCombo);
            this.Controls.Add(this.hardwareSpeedCombo);
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
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
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
        private System.Windows.Forms.ComboBox hardwareSpeedCombo;
        private System.Windows.Forms.Label hardwareSpeedLabel;
        private System.Windows.Forms.Label cardSlotLabel;
        private System.Windows.Forms.ComboBox cardSlotCombo;
    }
}