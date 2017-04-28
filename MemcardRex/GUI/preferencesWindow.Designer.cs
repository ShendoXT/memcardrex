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
            this.encodingCombo = new System.Windows.Forms.ComboBox();
            this.titleEncoding = new System.Windows.Forms.Label();
            this.fontCombo = new System.Windows.Forms.ComboBox();
            this.fontLabel = new System.Windows.Forms.Label();
            this.gridCheckbox = new System.Windows.Forms.CheckBox();
            this.iconSizeLabel = new System.Windows.Forms.Label();
            this.iconSizeCombo = new System.Windows.Forms.ComboBox();
            this.interpolationCombo = new System.Windows.Forms.ComboBox();
            this.interpolationLabel = new System.Windows.Forms.Label();
            this.backupWarningCheckBox = new System.Windows.Forms.CheckBox();
            this.glassCheckbox = new System.Windows.Forms.CheckBox();
            this.backupCheckbox = new System.Windows.Forms.CheckBox();
            this.dexDriveCombo = new System.Windows.Forms.ComboBox();
            this.hardwarePortLabel = new System.Windows.Forms.Label();
            this.spacerLabel = new System.Windows.Forms.Label();
            this.restorePositionCheckbox = new System.Windows.Forms.CheckBox();
            this.formatCombo = new System.Windows.Forms.ComboBox();
            this.formatLabel = new System.Windows.Forms.Label();
            this.backgroundCombo = new System.Windows.Forms.ComboBox();
            this.backgroundLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okButton.Location = new System.Drawing.Point(224, 184);
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
            this.cancelButton.Location = new System.Drawing.Point(304, 184);
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
            this.applyButton.Location = new System.Drawing.Point(384, 184);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(76, 24);
            this.applyButton.TabIndex = 1;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // encodingCombo
            // 
            this.encodingCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.encodingCombo.FormattingEnabled = true;
            this.encodingCombo.Items.AddRange(new object[] {
            "ASCII",
            "UTF-16"});
            this.encodingCombo.Location = new System.Drawing.Point(4, 20);
            this.encodingCombo.Name = "encodingCombo";
            this.encodingCombo.Size = new System.Drawing.Size(116, 21);
            this.encodingCombo.TabIndex = 2;
            // 
            // titleEncoding
            // 
            this.titleEncoding.AutoSize = true;
            this.titleEncoding.Location = new System.Drawing.Point(4, 4);
            this.titleEncoding.Name = "titleEncoding";
            this.titleEncoding.Size = new System.Drawing.Size(101, 13);
            this.titleEncoding.TabIndex = 5;
            this.titleEncoding.Text = "Save title encoding:";
            // 
            // fontCombo
            // 
            this.fontCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontCombo.FormattingEnabled = true;
            this.fontCombo.Location = new System.Drawing.Point(124, 20);
            this.fontCombo.Name = "fontCombo";
            this.fontCombo.Size = new System.Drawing.Size(116, 21);
            this.fontCombo.TabIndex = 3;
            // 
            // fontLabel
            // 
            this.fontLabel.AutoSize = true;
            this.fontLabel.Location = new System.Drawing.Point(124, 4);
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
            "32x32",
            "48x48"});
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
            // glassCheckbox
            // 
            this.glassCheckbox.AutoSize = true;
            this.glassCheckbox.Location = new System.Drawing.Point(248, 84);
            this.glassCheckbox.Name = "glassCheckbox";
            this.glassCheckbox.Size = new System.Drawing.Size(101, 17);
            this.glassCheckbox.TabIndex = 12;
            this.glassCheckbox.Text = "Glass status bar";
            this.glassCheckbox.UseVisualStyleBackColor = true;
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
            this.dexDriveCombo.Location = new System.Drawing.Point(4, 152);
            this.dexDriveCombo.Name = "dexDriveCombo";
            this.dexDriveCombo.Size = new System.Drawing.Size(116, 21);
            this.dexDriveCombo.TabIndex = 7;
            this.dexDriveCombo.SelectedIndexChanged += new System.EventHandler(this.dexDriveCombo_SelectedIndexChanged);
            // 
            // hardwarePortLabel
            // 
            this.hardwarePortLabel.AutoSize = true;
            this.hardwarePortLabel.Location = new System.Drawing.Point(4, 136);
            this.hardwarePortLabel.Name = "hardwarePortLabel";
            this.hardwarePortLabel.Size = new System.Drawing.Size(103, 13);
            this.hardwarePortLabel.TabIndex = 6;
            this.hardwarePortLabel.Text = "Communication port:";
            // 
            // spacerLabel
            // 
            this.spacerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.spacerLabel.Location = new System.Drawing.Point(4, 179);
            this.spacerLabel.Name = "spacerLabel";
            this.spacerLabel.Size = new System.Drawing.Size(456, 2);
            this.spacerLabel.TabIndex = 8;
            // 
            // restorePositionCheckbox
            // 
            this.restorePositionCheckbox.AutoSize = true;
            this.restorePositionCheckbox.Location = new System.Drawing.Point(248, 108);
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
            this.formatCombo.Location = new System.Drawing.Point(124, 152);
            this.formatCombo.Name = "formatCombo";
            this.formatCombo.Size = new System.Drawing.Size(116, 21);
            this.formatCombo.TabIndex = 8;
            // 
            // formatLabel
            // 
            this.formatLabel.AutoSize = true;
            this.formatLabel.Location = new System.Drawing.Point(124, 136);
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
            // preferencesWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 212);
            this.Controls.Add(this.backgroundCombo);
            this.Controls.Add(this.backgroundLabel);
            this.Controls.Add(this.formatLabel);
            this.Controls.Add(this.formatCombo);
            this.Controls.Add(this.restorePositionCheckbox);
            this.Controls.Add(this.spacerLabel);
            this.Controls.Add(this.gridCheckbox);
            this.Controls.Add(this.glassCheckbox);
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
            this.Controls.Add(this.encodingCombo);
            this.Controls.Add(this.titleEncoding);
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
        private System.Windows.Forms.Label titleEncoding;
        private System.Windows.Forms.ComboBox encodingCombo;
        private System.Windows.Forms.ComboBox interpolationCombo;
        private System.Windows.Forms.Label interpolationLabel;
        private System.Windows.Forms.CheckBox backupCheckbox;
        private System.Windows.Forms.CheckBox glassCheckbox;
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
    }
}