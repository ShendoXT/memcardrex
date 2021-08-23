namespace MemcardRex
{
    partial class AboutWindow
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.appNameLabel = new System.Windows.Forms.Label();
            this.appVersionLabel = new System.Windows.Forms.Label();
            this.compileDateLabel = new System.Windows.Forms.Label();
            this.infoLabel = new System.Windows.Forms.Label();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.OKbutton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DimGray;
            this.panel1.Controls.Add(this.compileDateLabel);
            this.panel1.Controls.Add(this.appVersionLabel);
            this.panel1.Controls.Add(this.appNameLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(280, 89);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.copyrightLabel);
            this.panel2.Controls.Add(this.infoLabel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 89);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(280, 271);
            this.panel2.TabIndex = 1;
            // 
            // appNameLabel
            // 
            this.appNameLabel.AutoSize = true;
            this.appNameLabel.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.appNameLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.appNameLabel.Location = new System.Drawing.Point(0, 0);
            this.appNameLabel.Name = "appNameLabel";
            this.appNameLabel.Size = new System.Drawing.Size(222, 45);
            this.appNameLabel.TabIndex = 0;
            this.appNameLabel.Text = "MemCardRex";
            // 
            // appVersionLabel
            // 
            this.appVersionLabel.AutoSize = true;
            this.appVersionLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.appVersionLabel.Location = new System.Drawing.Point(8, 45);
            this.appVersionLabel.Name = "appVersionLabel";
            this.appVersionLabel.Size = new System.Drawing.Size(48, 13);
            this.appVersionLabel.TabIndex = 1;
            this.appVersionLabel.Text = "Version:";
            // 
            // compileDateLabel
            // 
            this.compileDateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.compileDateLabel.AutoSize = true;
            this.compileDateLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.compileDateLabel.Location = new System.Drawing.Point(8, 65);
            this.compileDateLabel.Name = "compileDateLabel";
            this.compileDateLabel.Size = new System.Drawing.Size(78, 13);
            this.compileDateLabel.TabIndex = 2;
            this.compileDateLabel.Text = "Compile date:";
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Location = new System.Drawing.Point(8, 26);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(61, 13);
            this.infoLabel.TabIndex = 0;
            this.infoLabel.Text = "Thanks to:";
            // 
            // copyrightLabel
            // 
            this.copyrightLabel.AutoSize = true;
            this.copyrightLabel.Location = new System.Drawing.Point(8, 7);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(58, 13);
            this.copyrightLabel.TabIndex = 1;
            this.copyrightLabel.Text = "Copyright";
            // 
            // OKbutton
            // 
            this.OKbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKbutton.Location = new System.Drawing.Point(190, 321);
            this.OKbutton.Name = "OKbutton";
            this.OKbutton.Size = new System.Drawing.Size(75, 23);
            this.OKbutton.TabIndex = 2;
            this.OKbutton.Text = "Close";
            this.OKbutton.UseVisualStyleBackColor = true;
            this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
            // 
            // AboutWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(280, 360);
            this.Controls.Add(this.OKbutton);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "aboutWindow";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.AboutWindow_Paint);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label compileDateLabel;
        private System.Windows.Forms.Label appVersionLabel;
        private System.Windows.Forms.Label appNameLabel;
        private System.Windows.Forms.Button OKbutton;
        private System.Windows.Forms.Label copyrightLabel;
        private System.Windows.Forms.Label infoLabel;
    }
}