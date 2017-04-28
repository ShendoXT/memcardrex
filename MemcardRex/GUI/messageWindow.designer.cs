namespace MemcardRex
{
    partial class messageWindow
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
            this.MessageLabel = new System.Windows.Forms.Label();
            this.Button1 = new System.Windows.Forms.Button();
            this.Button2 = new System.Windows.Forms.Button();
            this.BackgroundColorLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // MessageLabel
            // 
            this.MessageLabel.AutoSize = true;
            this.MessageLabel.BackColor = System.Drawing.SystemColors.Window;
            this.MessageLabel.Location = new System.Drawing.Point(16, 22);
            this.MessageLabel.Name = "MessageLabel";
            this.MessageLabel.Size = new System.Drawing.Size(76, 13);
            this.MessageLabel.TabIndex = 0;
            this.MessageLabel.Text = "MessageLabel";
            // 
            // Button1
            // 
            this.Button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Button1.Location = new System.Drawing.Point(196, 48);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(84, 23);
            this.Button1.TabIndex = 1;
            this.Button1.Text = "Button1";
            this.Button1.UseVisualStyleBackColor = true;
            this.Button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // Button2
            // 
            this.Button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Button2.Location = new System.Drawing.Point(108, 48);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(84, 23);
            this.Button2.TabIndex = 2;
            this.Button2.Text = "Button2";
            this.Button2.UseVisualStyleBackColor = true;
            this.Button2.Visible = false;
            this.Button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // BackgroundColorLabel
            // 
            this.BackgroundColorLabel.BackColor = System.Drawing.SystemColors.Window;
            this.BackgroundColorLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.BackgroundColorLabel.Location = new System.Drawing.Point(0, 0);
            this.BackgroundColorLabel.Name = "BackgroundColorLabel";
            this.BackgroundColorLabel.Size = new System.Drawing.Size(284, 44);
            this.BackgroundColorLabel.TabIndex = 3;
            // 
            // messageWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 76);
            this.Controls.Add(this.Button2);
            this.Controls.Add(this.Button1);
            this.Controls.Add(this.MessageLabel);
            this.Controls.Add(this.BackgroundColorLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "messageWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "messageWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label MessageLabel;
        private System.Windows.Forms.Button Button1;
        private System.Windows.Forms.Button Button2;
        private System.Windows.Forms.Label BackgroundColorLabel;
    }
}