namespace MemcardRex
{
    partial class compareWindow
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
            this.compareListView = new System.Windows.Forms.ListView();
            this.offsetColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.save1Column = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.save2Column = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.save1Label = new System.Windows.Forms.Label();
            this.save2Label = new System.Windows.Forms.Label();
            this.spacerLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // compareListView
            // 
            this.compareListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.offsetColumn,
            this.save1Column,
            this.save2Column});
            this.compareListView.FullRowSelect = true;
            this.compareListView.Location = new System.Drawing.Point(4, 44);
            this.compareListView.MultiSelect = false;
            this.compareListView.Name = "compareListView";
            this.compareListView.Size = new System.Drawing.Size(368, 216);
            this.compareListView.TabIndex = 0;
            this.compareListView.UseCompatibleStateImageBehavior = false;
            this.compareListView.View = System.Windows.Forms.View.Details;
            // 
            // offsetColumn
            // 
            this.offsetColumn.Text = "Offset (hex, int)";
            this.offsetColumn.Width = 115;
            // 
            // save1Column
            // 
            this.save1Column.Text = "Save1 (hex, int)";
            this.save1Column.Width = 115;
            // 
            // save2Column
            // 
            this.save2Column.Text = "Save2 (hex, int)";
            this.save2Column.Width = 115;
            // 
            // save1Label
            // 
            this.save1Label.AutoSize = true;
            this.save1Label.Location = new System.Drawing.Point(4, 4);
            this.save1Label.Name = "save1Label";
            this.save1Label.Size = new System.Drawing.Size(62, 13);
            this.save1Label.TabIndex = 1;
            this.save1Label.Text = "save1Label";
            // 
            // save2Label
            // 
            this.save2Label.AutoSize = true;
            this.save2Label.Location = new System.Drawing.Point(4, 24);
            this.save2Label.Name = "save2Label";
            this.save2Label.Size = new System.Drawing.Size(64, 13);
            this.save2Label.TabIndex = 2;
            this.save2Label.Text = "save2Labell";
            // 
            // spacerLabel
            // 
            this.spacerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.spacerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.spacerLabel.Location = new System.Drawing.Point(4, 265);
            this.spacerLabel.Name = "spacerLabel";
            this.spacerLabel.Size = new System.Drawing.Size(368, 2);
            this.spacerLabel.TabIndex = 11;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(296, 269);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(76, 24);
            this.okButton.TabIndex = 10;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // compareWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(376, 297);
            this.Controls.Add(this.spacerLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.save2Label);
            this.Controls.Add(this.save1Label);
            this.Controls.Add(this.compareListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "compareWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "compareWindow";
            this.Load += new System.EventHandler(this.compareWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView compareListView;
        private System.Windows.Forms.ColumnHeader save1Column;
        private System.Windows.Forms.ColumnHeader save2Column;
        private System.Windows.Forms.ColumnHeader offsetColumn;
        private System.Windows.Forms.Label save1Label;
        private System.Windows.Forms.Label save2Label;
        private System.Windows.Forms.Label spacerLabel;
        private System.Windows.Forms.Button okButton;
    }
}