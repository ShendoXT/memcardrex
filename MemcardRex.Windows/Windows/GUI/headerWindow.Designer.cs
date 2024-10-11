namespace MemcardRex
{
    partial class headerWindow
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
            productLabel = new System.Windows.Forms.Label();
            prodCodeTextbox = new System.Windows.Forms.TextBox();
            identifierLabel = new System.Windows.Forms.Label();
            identifierTextbox = new System.Windows.Forms.TextBox();
            regionCombobox = new System.Windows.Forms.ComboBox();
            regionLabel = new System.Windows.Forms.Label();
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            spacerLabel = new System.Windows.Forms.Label();
            button1 = new System.Windows.Forms.Button();
            textBox1 = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // productLabel
            // 
            productLabel.AutoSize = true;
            productLabel.Location = new System.Drawing.Point(114, 6);
            productLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            productLabel.Name = "productLabel";
            productLabel.Size = new System.Drawing.Size(81, 15);
            productLabel.TabIndex = 0;
            productLabel.Text = "Product code:";
            // 
            // prodCodeTextbox
            // 
            prodCodeTextbox.Location = new System.Drawing.Point(114, 24);
            prodCodeTextbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            prodCodeTextbox.MaxLength = 10;
            prodCodeTextbox.Name = "prodCodeTextbox";
            prodCodeTextbox.Size = new System.Drawing.Size(121, 23);
            prodCodeTextbox.TabIndex = 1;
            // 
            // identifierLabel
            // 
            identifierLabel.AutoSize = true;
            identifierLabel.Location = new System.Drawing.Point(240, 6);
            identifierLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            identifierLabel.Name = "identifierLabel";
            identifierLabel.Size = new System.Drawing.Size(57, 15);
            identifierLabel.TabIndex = 2;
            identifierLabel.Text = "Identifier:";
            // 
            // identifierTextbox
            // 
            identifierTextbox.Location = new System.Drawing.Point(240, 24);
            identifierTextbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            identifierTextbox.MaxLength = 8;
            identifierTextbox.Name = "identifierTextbox";
            identifierTextbox.Size = new System.Drawing.Size(120, 23);
            identifierTextbox.TabIndex = 2;
            // 
            // regionCombobox
            // 
            regionCombobox.FormattingEnabled = true;
            regionCombobox.Items.AddRange(new object[] { "America", "Europe", "Japan" });
            regionCombobox.Location = new System.Drawing.Point(5, 24);
            regionCombobox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            regionCombobox.Name = "regionCombobox";
            regionCombobox.Size = new System.Drawing.Size(103, 23);
            regionCombobox.TabIndex = 0;
            // 
            // regionLabel
            // 
            regionLabel.AutoSize = true;
            regionLabel.Location = new System.Drawing.Point(5, 5);
            regionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            regionLabel.Name = "regionLabel";
            regionLabel.Size = new System.Drawing.Size(47, 15);
            regionLabel.TabIndex = 5;
            regionLabel.Text = "Region:";
            // 
            // okButton
            // 
            okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            okButton.Location = new System.Drawing.Point(206, 64);
            okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.TabIndex = 3;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            cancelButton.Location = new System.Drawing.Point(284, 64);
            cancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.TabIndex = 4;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // spacerLabel
            // 
            spacerLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            spacerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            spacerLabel.Location = new System.Drawing.Point(5, 55);
            spacerLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            spacerLabel.Name = "spacerLabel";
            spacerLabel.Size = new System.Drawing.Size(356, 2);
            spacerLabel.TabIndex = 12;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(5, 62);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(0, 0);
            button1.TabIndex = 13;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Location = new System.Drawing.Point(5, 63);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(0, 23);
            textBox1.TabIndex = 14;
            // 
            // headerWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(365, 93);
            Controls.Add(textBox1);
            Controls.Add(button1);
            Controls.Add(spacerLabel);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(regionLabel);
            Controls.Add(regionCombobox);
            Controls.Add(identifierTextbox);
            Controls.Add(identifierLabel);
            Controls.Add(prodCodeTextbox);
            Controls.Add(productLabel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "headerWindow";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "headerWindow";
            Load += headerWindow_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label productLabel;
        private System.Windows.Forms.TextBox prodCodeTextbox;
        private System.Windows.Forms.Label identifierLabel;
        private System.Windows.Forms.TextBox identifierTextbox;
        private System.Windows.Forms.ComboBox regionCombobox;
        private System.Windows.Forms.Label regionLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label spacerLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
    }
}