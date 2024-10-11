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
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            gridCheckbox = new System.Windows.Forms.CheckBox();
            backupWarningCheckBox = new System.Windows.Forms.CheckBox();
            backupCheckbox = new System.Windows.Forms.CheckBox();
            dexDriveCombo = new System.Windows.Forms.ComboBox();
            hardwarePortLabel = new System.Windows.Forms.Label();
            restorePositionCheckbox = new System.Windows.Forms.CheckBox();
            formatCombo = new System.Windows.Forms.ComboBox();
            formatLabel = new System.Windows.Forms.Label();
            backgroundCombo = new System.Windows.Forms.ComboBox();
            backgroundLabel = new System.Windows.Forms.Label();
            fixCorruptedCardsCheckbox = new System.Windows.Forms.CheckBox();
            remotePortLabel = new System.Windows.Forms.Label();
            remoteAddressLabel = new System.Windows.Forms.Label();
            remoteAddressBox = new System.Windows.Forms.TextBox();
            remotePortUpDown = new System.Windows.Forms.NumericUpDown();
            hardwareInterfacesCombo = new System.Windows.Forms.ComboBox();
            hardwareSpeedLabel = new System.Windows.Forms.Label();
            cardSlotLabel = new System.Windows.Forms.Label();
            cardSlotCombo = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)remotePortUpDown).BeginInit();
            SuspendLayout();
            // 
            // okButton
            // 
            okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            okButton.Location = new System.Drawing.Point(368, 168);
            okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.TabIndex = 99;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            cancelButton.Location = new System.Drawing.Point(448, 168);
            cancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.TabIndex = 0;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // gridCheckbox
            // 
            gridCheckbox.AutoSize = true;
            gridCheckbox.Location = new System.Drawing.Point(288, 8);
            gridCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            gridCheckbox.Name = "gridCheckbox";
            gridCheckbox.Size = new System.Drawing.Size(139, 19);
            gridCheckbox.TabIndex = 9;
            gridCheckbox.Text = "Show grid on slot list.";
            gridCheckbox.UseVisualStyleBackColor = true;
            // 
            // backupWarningCheckBox
            // 
            backupWarningCheckBox.AutoSize = true;
            backupWarningCheckBox.Location = new System.Drawing.Point(288, 56);
            backupWarningCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            backupWarningCheckBox.Name = "backupWarningCheckBox";
            backupWarningCheckBox.Size = new System.Drawing.Size(155, 19);
            backupWarningCheckBox.TabIndex = 11;
            backupWarningCheckBox.Text = "Show warning messages";
            backupWarningCheckBox.UseVisualStyleBackColor = true;
            // 
            // backupCheckbox
            // 
            backupCheckbox.AutoSize = true;
            backupCheckbox.Location = new System.Drawing.Point(288, 32);
            backupCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            backupCheckbox.Name = "backupCheckbox";
            backupCheckbox.Size = new System.Drawing.Size(227, 19);
            backupCheckbox.TabIndex = 10;
            backupCheckbox.Text = "Backup Memory Cards upon opening.";
            backupCheckbox.UseVisualStyleBackColor = true;
            // 
            // dexDriveCombo
            // 
            dexDriveCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            dexDriveCombo.FormattingEnabled = true;
            dexDriveCombo.Location = new System.Drawing.Point(4, 24);
            dexDriveCombo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dexDriveCombo.Name = "dexDriveCombo";
            dexDriveCombo.Size = new System.Drawing.Size(79, 23);
            dexDriveCombo.TabIndex = 7;
            dexDriveCombo.SelectedIndexChanged += dexDriveCombo_SelectedIndexChanged;
            // 
            // hardwarePortLabel
            // 
            hardwarePortLabel.AutoSize = true;
            hardwarePortLabel.Location = new System.Drawing.Point(4, 6);
            hardwarePortLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            hardwarePortLabel.Name = "hardwarePortLabel";
            hardwarePortLabel.Size = new System.Drawing.Size(58, 15);
            hardwarePortLabel.TabIndex = 6;
            hardwarePortLabel.Text = "Comport:";
            // 
            // restorePositionCheckbox
            // 
            restorePositionCheckbox.AutoSize = true;
            restorePositionCheckbox.Location = new System.Drawing.Point(288, 80);
            restorePositionCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            restorePositionCheckbox.Name = "restorePositionCheckbox";
            restorePositionCheckbox.Size = new System.Drawing.Size(213, 19);
            restorePositionCheckbox.TabIndex = 13;
            restorePositionCheckbox.Text = "Restore window position on startup";
            restorePositionCheckbox.UseVisualStyleBackColor = true;
            // 
            // formatCombo
            // 
            formatCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            formatCombo.FormattingEnabled = true;
            formatCombo.Items.AddRange(new object[] { "Quick format", "Full format" });
            formatCombo.Location = new System.Drawing.Point(4, 168);
            formatCombo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            formatCombo.Name = "formatCombo";
            formatCombo.Size = new System.Drawing.Size(132, 23);
            formatCombo.TabIndex = 8;
            // 
            // formatLabel
            // 
            formatLabel.AutoSize = true;
            formatLabel.Location = new System.Drawing.Point(4, 150);
            formatLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            formatLabel.Name = "formatLabel";
            formatLabel.Size = new System.Drawing.Size(126, 15);
            formatLabel.TabIndex = 101;
            formatLabel.Text = "Hardware format type:";
            // 
            // backgroundCombo
            // 
            backgroundCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            backgroundCombo.FormattingEnabled = true;
            backgroundCombo.Items.AddRange(new object[] { "Transparent", "Black (Slim PS1 models)", "Gray (Older european PS1 models)", "Blue (Standard BIOS color)" });
            backgroundCombo.Location = new System.Drawing.Point(4, 120);
            backgroundCombo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            backgroundCombo.Name = "backgroundCombo";
            backgroundCombo.Size = new System.Drawing.Size(275, 23);
            backgroundCombo.TabIndex = 6;
            // 
            // backgroundLabel
            // 
            backgroundLabel.AutoSize = true;
            backgroundLabel.Location = new System.Drawing.Point(4, 101);
            backgroundLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            backgroundLabel.Name = "backgroundLabel";
            backgroundLabel.Size = new System.Drawing.Size(130, 15);
            backgroundLabel.TabIndex = 102;
            backgroundLabel.Text = "Icon background color:";
            // 
            // fixCorruptedCardsCheckbox
            // 
            fixCorruptedCardsCheckbox.AutoSize = true;
            fixCorruptedCardsCheckbox.Location = new System.Drawing.Point(288, 104);
            fixCorruptedCardsCheckbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            fixCorruptedCardsCheckbox.Name = "fixCorruptedCardsCheckbox";
            fixCorruptedCardsCheckbox.Size = new System.Drawing.Size(207, 19);
            fixCorruptedCardsCheckbox.TabIndex = 103;
            fixCorruptedCardsCheckbox.Text = "Try to fix corrupted Memory Cards";
            fixCorruptedCardsCheckbox.UseVisualStyleBackColor = true;
            // 
            // remotePortLabel
            // 
            remotePortLabel.AutoSize = true;
            remotePortLabel.Location = new System.Drawing.Point(140, 52);
            remotePortLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            remotePortLabel.Name = "remotePortLabel";
            remotePortLabel.Size = new System.Drawing.Size(76, 15);
            remotePortLabel.TabIndex = 105;
            remotePortLabel.Text = "Remote Port:";
            // 
            // remoteAddressLabel
            // 
            remoteAddressLabel.AutoSize = true;
            remoteAddressLabel.Location = new System.Drawing.Point(5, 53);
            remoteAddressLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            remoteAddressLabel.Name = "remoteAddressLabel";
            remoteAddressLabel.Size = new System.Drawing.Size(96, 15);
            remoteAddressLabel.TabIndex = 104;
            remoteAddressLabel.Text = "Remote Address:";
            // 
            // remoteAddressBox
            // 
            remoteAddressBox.Location = new System.Drawing.Point(4, 72);
            remoteAddressBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            remoteAddressBox.Name = "remoteAddressBox";
            remoteAddressBox.Size = new System.Drawing.Size(131, 23);
            remoteAddressBox.TabIndex = 106;
            // 
            // remotePortUpDown
            // 
            remotePortUpDown.Location = new System.Drawing.Point(140, 72);
            remotePortUpDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            remotePortUpDown.Name = "remotePortUpDown";
            remotePortUpDown.Size = new System.Drawing.Size(140, 23);
            remotePortUpDown.TabIndex = 107;
            // 
            // hardwareInterfacesCombo
            // 
            hardwareInterfacesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            hardwareInterfacesCombo.FormattingEnabled = true;
            hardwareInterfacesCombo.Location = new System.Drawing.Point(88, 24);
            hardwareInterfacesCombo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            hardwareInterfacesCombo.Name = "hardwareInterfacesCombo";
            hardwareInterfacesCombo.Size = new System.Drawing.Size(191, 23);
            hardwareInterfacesCombo.TabIndex = 109;
            // 
            // hardwareSpeedLabel
            // 
            hardwareSpeedLabel.AutoSize = true;
            hardwareSpeedLabel.Location = new System.Drawing.Point(88, 6);
            hardwareSpeedLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            hardwareSpeedLabel.Name = "hardwareSpeedLabel";
            hardwareSpeedLabel.Size = new System.Drawing.Size(144, 15);
            hardwareSpeedLabel.TabIndex = 108;
            hardwareSpeedLabel.Text = "Active hardware interface:";
            // 
            // cardSlotLabel
            // 
            cardSlotLabel.AutoSize = true;
            cardSlotLabel.Location = new System.Drawing.Point(140, 152);
            cardSlotLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            cardSlotLabel.Name = "cardSlotLabel";
            cardSlotLabel.Size = new System.Drawing.Size(109, 15);
            cardSlotLabel.TabIndex = 111;
            cardSlotLabel.Text = "Hardware card slot:";
            // 
            // cardSlotCombo
            // 
            cardSlotCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cardSlotCombo.FormattingEnabled = true;
            cardSlotCombo.Items.AddRange(new object[] { "Slot 1", "Slot 2" });
            cardSlotCombo.Location = new System.Drawing.Point(140, 168);
            cardSlotCombo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cardSlotCombo.Name = "cardSlotCombo";
            cardSlotCombo.Size = new System.Drawing.Size(140, 23);
            cardSlotCombo.TabIndex = 110;
            // 
            // preferencesWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(527, 196);
            Controls.Add(cardSlotLabel);
            Controls.Add(cardSlotCombo);
            Controls.Add(hardwareInterfacesCombo);
            Controls.Add(hardwareSpeedLabel);
            Controls.Add(remotePortUpDown);
            Controls.Add(remoteAddressBox);
            Controls.Add(remotePortLabel);
            Controls.Add(remoteAddressLabel);
            Controls.Add(fixCorruptedCardsCheckbox);
            Controls.Add(backgroundCombo);
            Controls.Add(backgroundLabel);
            Controls.Add(formatLabel);
            Controls.Add(formatCombo);
            Controls.Add(restorePositionCheckbox);
            Controls.Add(gridCheckbox);
            Controls.Add(backupWarningCheckBox);
            Controls.Add(backupCheckbox);
            Controls.Add(dexDriveCombo);
            Controls.Add(hardwarePortLabel);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "preferencesWindow";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Preferences";
            ((System.ComponentModel.ISupportInitialize)remotePortUpDown).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox gridCheckbox;
        private System.Windows.Forms.CheckBox backupCheckbox;
        private System.Windows.Forms.CheckBox backupWarningCheckBox;
        private System.Windows.Forms.ComboBox dexDriveCombo;
        private System.Windows.Forms.Label hardwarePortLabel;
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