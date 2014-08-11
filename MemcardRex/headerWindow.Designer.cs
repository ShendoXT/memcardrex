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
            this.productLabel = new System.Windows.Forms.Label();
            this.prodCodeTextbox = new System.Windows.Forms.TextBox();
            this.identifierLabel = new System.Windows.Forms.Label();
            this.identifierTextbox = new System.Windows.Forms.TextBox();
            this.regionCombobox = new System.Windows.Forms.ComboBox();
            this.regionLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.spacerLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // productLabel
            // 
            this.productLabel.AutoSize = true;
            this.productLabel.Location = new System.Drawing.Point(4, 4);
            this.productLabel.Name = "productLabel";
            this.productLabel.Size = new System.Drawing.Size(71, 13);
            this.productLabel.TabIndex = 0;
            this.productLabel.Text = "Product code";
            // 
            // prodCodeTextbox
            // 
            this.prodCodeTextbox.Location = new System.Drawing.Point(4, 20);
            this.prodCodeTextbox.MaxLength = 10;
            this.prodCodeTextbox.Name = "prodCodeTextbox";
            this.prodCodeTextbox.Size = new System.Drawing.Size(104, 20);
            this.prodCodeTextbox.TabIndex = 0;
            // 
            // identifierLabel
            // 
            this.identifierLabel.AutoSize = true;
            this.identifierLabel.Location = new System.Drawing.Point(112, 4);
            this.identifierLabel.Name = "identifierLabel";
            this.identifierLabel.Size = new System.Drawing.Size(47, 13);
            this.identifierLabel.TabIndex = 2;
            this.identifierLabel.Text = "Identifier";
            // 
            // identifierTextbox
            // 
            this.identifierTextbox.Location = new System.Drawing.Point(112, 20);
            this.identifierTextbox.MaxLength = 8;
            this.identifierTextbox.Name = "identifierTextbox";
            this.identifierTextbox.Size = new System.Drawing.Size(104, 20);
            this.identifierTextbox.TabIndex = 1;
            // 
            // regionCombobox
            // 
            this.regionCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.regionCombobox.FormattingEnabled = true;
            this.regionCombobox.Items.AddRange(new object[] {
            "America",
            "Europe",
            "Japan"});
            this.regionCombobox.Location = new System.Drawing.Point(220, 20);
            this.regionCombobox.Name = "regionCombobox";
            this.regionCombobox.Size = new System.Drawing.Size(111, 21);
            this.regionCombobox.TabIndex = 2;
            // 
            // regionLabel
            // 
            this.regionLabel.AutoSize = true;
            this.regionLabel.Location = new System.Drawing.Point(220, 4);
            this.regionLabel.Name = "regionLabel";
            this.regionLabel.Size = new System.Drawing.Size(41, 13);
            this.regionLabel.TabIndex = 5;
            this.regionLabel.Text = "Region";
            // 
            // okButton
            // 
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okButton.Location = new System.Drawing.Point(176, 54);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(76, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(256, 54);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(76, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // spacerLabel
            // 
            this.spacerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.spacerLabel.Location = new System.Drawing.Point(4, 48);
            this.spacerLabel.Name = "spacerLabel";
            this.spacerLabel.Size = new System.Drawing.Size(328, 2);
            this.spacerLabel.TabIndex = 12;
            // 
            // headerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 81);
            this.Controls.Add(this.spacerLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.regionLabel);
            this.Controls.Add(this.regionCombobox);
            this.Controls.Add(this.identifierTextbox);
            this.Controls.Add(this.identifierLabel);
            this.Controls.Add(this.prodCodeTextbox);
            this.Controls.Add(this.productLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "headerWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "headerWindow";
            this.Load += new System.EventHandler(this.headerWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}