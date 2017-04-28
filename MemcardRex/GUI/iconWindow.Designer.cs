namespace MemcardRex
{
    partial class iconWindow
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
            this.frameCombo = new System.Windows.Forms.ComboBox();
            this.exportButton = new System.Windows.Forms.Button();
            this.hFlipButton = new System.Windows.Forms.Button();
            this.vFlipButton = new System.Windows.Forms.Button();
            this.leftButton = new System.Windows.Forms.Button();
            this.rightButton = new System.Windows.Forms.Button();
            this.iconRender = new System.Windows.Forms.PictureBox();
            this.paletteRender = new System.Windows.Forms.PictureBox();
            this.colorRender = new System.Windows.Forms.PictureBox();
            this.Xlabel = new System.Windows.Forms.Label();
            this.Ylabel = new System.Windows.Forms.Label();
            this.colorRender2 = new System.Windows.Forms.PictureBox();
            this.spacerLabel = new System.Windows.Forms.Label();
            this.importButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.iconRender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteRender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorRender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorRender2)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okButton.Location = new System.Drawing.Point(108, 260);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(76, 24);
            this.okButton.TabIndex = 8;
            this.okButton.Text = "OK";
            this.okButton.UseMnemonic = false;
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(188, 260);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(76, 24);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseMnemonic = false;
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // frameCombo
            // 
            this.frameCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.frameCombo.FormattingEnabled = true;
            this.frameCombo.Location = new System.Drawing.Point(4, 4);
            this.frameCombo.Name = "frameCombo";
            this.frameCombo.Size = new System.Drawing.Size(259, 21);
            this.frameCombo.TabIndex = 1;
            this.frameCombo.SelectedIndexChanged += new System.EventHandler(this.frameCombo_SelectedIndexChanged);
            // 
            // exportButton
            // 
            this.exportButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.exportButton.Location = new System.Drawing.Point(188, 56);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(76, 24);
            this.exportButton.TabIndex = 3;
            this.exportButton.Text = "Export icon...";
            this.exportButton.UseMnemonic = false;
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // hFlipButton
            // 
            this.hFlipButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.hFlipButton.Location = new System.Drawing.Point(188, 84);
            this.hFlipButton.Name = "hFlipButton";
            this.hFlipButton.Size = new System.Drawing.Size(76, 24);
            this.hFlipButton.TabIndex = 4;
            this.hFlipButton.Text = "H flip";
            this.hFlipButton.UseMnemonic = false;
            this.hFlipButton.UseVisualStyleBackColor = true;
            this.hFlipButton.Click += new System.EventHandler(this.hFlipButton_Click);
            // 
            // vFlipButton
            // 
            this.vFlipButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.vFlipButton.Location = new System.Drawing.Point(188, 112);
            this.vFlipButton.Name = "vFlipButton";
            this.vFlipButton.Size = new System.Drawing.Size(76, 24);
            this.vFlipButton.TabIndex = 5;
            this.vFlipButton.Text = "V flip";
            this.vFlipButton.UseMnemonic = false;
            this.vFlipButton.UseVisualStyleBackColor = true;
            this.vFlipButton.Click += new System.EventHandler(this.vFlipButton_Click);
            // 
            // leftButton
            // 
            this.leftButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.leftButton.Location = new System.Drawing.Point(188, 140);
            this.leftButton.Name = "leftButton";
            this.leftButton.Size = new System.Drawing.Size(76, 24);
            this.leftButton.TabIndex = 6;
            this.leftButton.Text = "Rotate left";
            this.leftButton.UseMnemonic = false;
            this.leftButton.UseVisualStyleBackColor = true;
            this.leftButton.Click += new System.EventHandler(this.leftButton_Click);
            // 
            // rightButton
            // 
            this.rightButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.rightButton.Location = new System.Drawing.Point(188, 168);
            this.rightButton.Name = "rightButton";
            this.rightButton.Size = new System.Drawing.Size(76, 24);
            this.rightButton.TabIndex = 7;
            this.rightButton.Text = "Rotate right";
            this.rightButton.UseMnemonic = false;
            this.rightButton.UseVisualStyleBackColor = true;
            this.rightButton.Click += new System.EventHandler(this.rightButton_Click);
            // 
            // iconRender
            // 
            this.iconRender.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.iconRender.Location = new System.Drawing.Point(4, 28);
            this.iconRender.Name = "iconRender";
            this.iconRender.Size = new System.Drawing.Size(181, 181);
            this.iconRender.TabIndex = 9;
            this.iconRender.TabStop = false;
            this.iconRender.MouseDown += new System.Windows.Forms.MouseEventHandler(this.iconRender_MouseDownMove);
            this.iconRender.MouseLeave += new System.EventHandler(this.iconRender_MouseLeave);
            this.iconRender.MouseMove += new System.Windows.Forms.MouseEventHandler(this.iconRender_MouseDownMove);
            // 
            // paletteRender
            // 
            this.paletteRender.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.paletteRender.Location = new System.Drawing.Point(60, 213);
            this.paletteRender.Name = "paletteRender";
            this.paletteRender.Size = new System.Drawing.Size(125, 35);
            this.paletteRender.TabIndex = 10;
            this.paletteRender.TabStop = false;
            this.paletteRender.DoubleClick += new System.EventHandler(this.paletteRender_DoubleClick);
            this.paletteRender.MouseDown += new System.Windows.Forms.MouseEventHandler(this.paletteRender_MouseDown);
            // 
            // colorRender
            // 
            this.colorRender.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.colorRender.Location = new System.Drawing.Point(4, 213);
            this.colorRender.Name = "colorRender";
            this.colorRender.Size = new System.Drawing.Size(26, 35);
            this.colorRender.TabIndex = 11;
            this.colorRender.TabStop = false;
            // 
            // Xlabel
            // 
            this.Xlabel.AutoSize = true;
            this.Xlabel.Location = new System.Drawing.Point(188, 232);
            this.Xlabel.Name = "Xlabel";
            this.Xlabel.Size = new System.Drawing.Size(17, 13);
            this.Xlabel.TabIndex = 12;
            this.Xlabel.Text = "X:";
            // 
            // Ylabel
            // 
            this.Ylabel.AutoSize = true;
            this.Ylabel.Location = new System.Drawing.Point(220, 232);
            this.Ylabel.Name = "Ylabel";
            this.Ylabel.Size = new System.Drawing.Size(17, 13);
            this.Ylabel.TabIndex = 13;
            this.Ylabel.Text = "Y:";
            // 
            // colorRender2
            // 
            this.colorRender2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.colorRender2.Location = new System.Drawing.Point(32, 213);
            this.colorRender2.Name = "colorRender2";
            this.colorRender2.Size = new System.Drawing.Size(26, 35);
            this.colorRender2.TabIndex = 14;
            this.colorRender2.TabStop = false;
            // 
            // spacerLabel
            // 
            this.spacerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.spacerLabel.Location = new System.Drawing.Point(4, 254);
            this.spacerLabel.Name = "spacerLabel";
            this.spacerLabel.Size = new System.Drawing.Size(260, 2);
            this.spacerLabel.TabIndex = 15;
            // 
            // importButton
            // 
            this.importButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.importButton.Location = new System.Drawing.Point(188, 28);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(76, 24);
            this.importButton.TabIndex = 2;
            this.importButton.Text = "Import icon...";
            this.importButton.UseMnemonic = false;
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // iconWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(267, 288);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.spacerLabel);
            this.Controls.Add(this.colorRender2);
            this.Controls.Add(this.Ylabel);
            this.Controls.Add(this.Xlabel);
            this.Controls.Add(this.colorRender);
            this.Controls.Add(this.paletteRender);
            this.Controls.Add(this.iconRender);
            this.Controls.Add(this.rightButton);
            this.Controls.Add(this.leftButton);
            this.Controls.Add(this.vFlipButton);
            this.Controls.Add(this.hFlipButton);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.frameCombo);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "iconWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "iconWindow";
            this.Load += new System.EventHandler(this.iconWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.iconRender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteRender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorRender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorRender2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox frameCombo;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button hFlipButton;
        private System.Windows.Forms.Button vFlipButton;
        private System.Windows.Forms.Button leftButton;
        private System.Windows.Forms.Button rightButton;
        private System.Windows.Forms.PictureBox iconRender;
        private System.Windows.Forms.PictureBox paletteRender;
        private System.Windows.Forms.PictureBox colorRender;
        private System.Windows.Forms.Label Xlabel;
        private System.Windows.Forms.Label Ylabel;
        private System.Windows.Forms.PictureBox colorRender2;
        private System.Windows.Forms.Label spacerLabel;
        private System.Windows.Forms.Button importButton;

    }
}