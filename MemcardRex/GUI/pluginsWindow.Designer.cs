namespace MemcardRex
{
    partial class pluginsWindow
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
            this.OKbutton = new System.Windows.Forms.Button();
            this.aboutButton = new System.Windows.Forms.Button();
            this.configButton = new System.Windows.Forms.Button();
            this.pluginListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // OKbutton
            // 
            this.OKbutton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.OKbutton.Location = new System.Drawing.Point(376, 256);
            this.OKbutton.Name = "OKbutton";
            this.OKbutton.Size = new System.Drawing.Size(76, 24);
            this.OKbutton.TabIndex = 0;
            this.OKbutton.Text = "OK";
            this.OKbutton.UseMnemonic = false;
            this.OKbutton.UseVisualStyleBackColor = true;
            this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
            // 
            // aboutButton
            // 
            this.aboutButton.Location = new System.Drawing.Point(84, 256);
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size(76, 24);
            this.aboutButton.TabIndex = 3;
            this.aboutButton.Text = "About";
            this.aboutButton.UseVisualStyleBackColor = true;
            this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // configButton
            // 
            this.configButton.Location = new System.Drawing.Point(4, 256);
            this.configButton.Name = "configButton";
            this.configButton.Size = new System.Drawing.Size(76, 24);
            this.configButton.TabIndex = 2;
            this.configButton.Text = "Config";
            this.configButton.UseVisualStyleBackColor = true;
            this.configButton.Click += new System.EventHandler(this.configButton_Click);
            // 
            // pluginListView
            // 
            this.pluginListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.pluginListView.FullRowSelect = true;
            this.pluginListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.pluginListView.HideSelection = false;
            this.pluginListView.Location = new System.Drawing.Point(4, 4);
            this.pluginListView.MultiSelect = false;
            this.pluginListView.Name = "pluginListView";
            this.pluginListView.Size = new System.Drawing.Size(448, 248);
            this.pluginListView.TabIndex = 1;
            this.pluginListView.UseCompatibleStateImageBehavior = false;
            this.pluginListView.View = System.Windows.Forms.View.Details;
            this.pluginListView.SelectedIndexChanged += new System.EventHandler(this.pluginListView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Plugin name";
            this.columnHeader1.Width = 131;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Author";
            this.columnHeader2.Width = 92;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Supported game(s)";
            this.columnHeader3.Width = 219;
            // 
            // pluginsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 283);
            this.Controls.Add(this.pluginListView);
            this.Controls.Add(this.configButton);
            this.Controls.Add(this.aboutButton);
            this.Controls.Add(this.OKbutton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "pluginsWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Plugins";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button OKbutton;
        private System.Windows.Forms.Button aboutButton;
        private System.Windows.Forms.Button configButton;
        private System.Windows.Forms.ListView pluginListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}