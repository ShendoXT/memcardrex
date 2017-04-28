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
            this.infoLabel = new System.Windows.Forms.Label();
            this.OKbutton = new System.Windows.Forms.Button();
            this.appNameLabel = new System.Windows.Forms.Label();
            this.appVersionLabel = new System.Windows.Forms.Label();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.compileDateLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.ForeColor = System.Drawing.Color.Black;
            this.infoLabel.Location = new System.Drawing.Point(8, 60);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(115, 13);
            this.infoLabel.TabIndex = 1;
            this.infoLabel.Text = "infoLabelPlaceholder";
            // 
            // OKbutton
            // 
            this.OKbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKbutton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.OKbutton.Location = new System.Drawing.Point(229, 120);
            this.OKbutton.Name = "OKbutton";
            this.OKbutton.Size = new System.Drawing.Size(76, 24);
            this.OKbutton.TabIndex = 0;
            this.OKbutton.Text = "OK";
            this.OKbutton.UseVisualStyleBackColor = true;
            this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
            // 
            // appNameLabel
            // 
            this.appNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.appNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.appNameLabel.ForeColor = System.Drawing.Color.White;
            this.appNameLabel.Location = new System.Drawing.Point(6, 8);
            this.appNameLabel.Margin = new System.Windows.Forms.Padding(0);
            this.appNameLabel.Name = "appNameLabel";
            this.appNameLabel.Size = new System.Drawing.Size(298, 20);
            this.appNameLabel.TabIndex = 3;
            // 
            // appVersionLabel
            // 
            this.appVersionLabel.BackColor = System.Drawing.Color.Transparent;
            this.appVersionLabel.ForeColor = System.Drawing.Color.White;
            this.appVersionLabel.Location = new System.Drawing.Point(8, 32);
            this.appVersionLabel.Name = "appVersionLabel";
            this.appVersionLabel.Size = new System.Drawing.Size(115, 16);
            this.appVersionLabel.TabIndex = 4;
            // 
            // copyrightLabel
            // 
            this.copyrightLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.copyrightLabel.Location = new System.Drawing.Point(8, 124);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(212, 16);
            this.copyrightLabel.TabIndex = 1;
            // 
            // compileDateLabel
            // 
            this.compileDateLabel.BackColor = System.Drawing.Color.Transparent;
            this.compileDateLabel.ForeColor = System.Drawing.Color.White;
            this.compileDateLabel.Location = new System.Drawing.Point(129, 32);
            this.compileDateLabel.Name = "compileDateLabel";
            this.compileDateLabel.Size = new System.Drawing.Size(175, 16);
            this.compileDateLabel.TabIndex = 6;
            this.compileDateLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // AboutWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(308, 147);
            this.Controls.Add(this.OKbutton);
            this.Controls.Add(this.copyrightLabel);
            this.Controls.Add(this.compileDateLabel);
            this.Controls.Add(this.appVersionLabel);
            this.Controls.Add(this.appNameLabel);
            this.Controls.Add(this.infoLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "aboutWindow";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.AboutWindow_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Button OKbutton;
        private System.Windows.Forms.Label appNameLabel;
        private System.Windows.Forms.Label appVersionLabel;
        private System.Windows.Forms.Label copyrightLabel;
        private System.Windows.Forms.Label compileDateLabel;
    }
}