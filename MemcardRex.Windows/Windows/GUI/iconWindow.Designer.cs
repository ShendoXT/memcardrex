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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(iconWindow));
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            hFlipButton = new System.Windows.Forms.Button();
            vFlipButton = new System.Windows.Forms.Button();
            leftButton = new System.Windows.Forms.Button();
            rightButton = new System.Windows.Forms.Button();
            iconRender = new System.Windows.Forms.PictureBox();
            paletteRender = new System.Windows.Forms.PictureBox();
            colorRender = new System.Windows.Forms.PictureBox();
            Xlabel = new System.Windows.Forms.Label();
            Ylabel = new System.Windows.Forms.Label();
            colorRender2 = new System.Windows.Forms.PictureBox();
            spacerLabel = new System.Windows.Forms.Label();
            iconList = new System.Windows.Forms.ImageList(components);
            iconListView = new System.Windows.Forms.ListView();
            iconContextMenu = new System.Windows.Forms.ContextMenuStrip(components);
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            iconTimer = new System.Windows.Forms.Timer(components);
            penRadio = new System.Windows.Forms.RadioButton();
            bucketRadio = new System.Windows.Forms.RadioButton();
            gridSlider = new System.Windows.Forms.TrackBar();
            eraserRadio = new System.Windows.Forms.RadioButton();
            gridCheckbox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)iconRender).BeginInit();
            ((System.ComponentModel.ISupportInitialize)paletteRender).BeginInit();
            ((System.ComponentModel.ISupportInitialize)colorRender).BeginInit();
            ((System.ComponentModel.ISupportInitialize)colorRender2).BeginInit();
            iconContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridSlider).BeginInit();
            SuspendLayout();
            // 
            // okButton
            // 
            okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            okButton.Location = new System.Drawing.Point(232, 324);
            okButton.Margin = new System.Windows.Forms.Padding(4);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(95, 30);
            okButton.TabIndex = 8;
            okButton.Text = "OK";
            okButton.UseMnemonic = false;
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            cancelButton.Location = new System.Drawing.Point(332, 324);
            cancelButton.Margin = new System.Windows.Forms.Padding(4);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(95, 30);
            cancelButton.TabIndex = 0;
            cancelButton.Text = "Cancel";
            cancelButton.UseMnemonic = false;
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // hFlipButton
            // 
            hFlipButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            hFlipButton.Location = new System.Drawing.Point(392, 132);
            hFlipButton.Margin = new System.Windows.Forms.Padding(4);
            hFlipButton.Name = "hFlipButton";
            hFlipButton.Size = new System.Drawing.Size(40, 30);
            hFlipButton.TabIndex = 4;
            hFlipButton.Text = "↔️";
            hFlipButton.UseMnemonic = false;
            hFlipButton.UseVisualStyleBackColor = true;
            hFlipButton.Click += hFlipButton_Click;
            // 
            // vFlipButton
            // 
            vFlipButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            vFlipButton.Location = new System.Drawing.Point(392, 164);
            vFlipButton.Margin = new System.Windows.Forms.Padding(4);
            vFlipButton.Name = "vFlipButton";
            vFlipButton.Size = new System.Drawing.Size(40, 30);
            vFlipButton.TabIndex = 5;
            vFlipButton.Text = "↕️";
            vFlipButton.UseMnemonic = false;
            vFlipButton.UseVisualStyleBackColor = true;
            vFlipButton.Click += vFlipButton_Click;
            // 
            // leftButton
            // 
            leftButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            leftButton.Location = new System.Drawing.Point(392, 200);
            leftButton.Margin = new System.Windows.Forms.Padding(4);
            leftButton.Name = "leftButton";
            leftButton.Size = new System.Drawing.Size(40, 30);
            leftButton.TabIndex = 6;
            leftButton.Text = "↪️";
            leftButton.UseMnemonic = false;
            leftButton.UseVisualStyleBackColor = true;
            leftButton.Click += leftButton_Click;
            // 
            // rightButton
            // 
            rightButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            rightButton.Location = new System.Drawing.Point(392, 232);
            rightButton.Margin = new System.Windows.Forms.Padding(4);
            rightButton.Name = "rightButton";
            rightButton.Size = new System.Drawing.Size(40, 30);
            rightButton.TabIndex = 7;
            rightButton.Text = "↩️";
            rightButton.UseMnemonic = false;
            rightButton.UseVisualStyleBackColor = true;
            rightButton.Click += rightButton_Click;
            // 
            // iconRender
            // 
            iconRender.Cursor = System.Windows.Forms.Cursors.Cross;
            iconRender.Location = new System.Drawing.Point(132, 4);
            iconRender.Margin = new System.Windows.Forms.Padding(4);
            iconRender.Name = "iconRender";
            iconRender.Size = new System.Drawing.Size(260, 260);
            iconRender.TabIndex = 9;
            iconRender.TabStop = false;
            iconRender.MouseDown += iconRender_MouseDownMove;
            iconRender.MouseLeave += iconRender_MouseLeave;
            iconRender.MouseMove += iconRender_MouseDownMove;
            // 
            // paletteRender
            // 
            paletteRender.Location = new System.Drawing.Point(228, 268);
            paletteRender.Margin = new System.Windows.Forms.Padding(4);
            paletteRender.Name = "paletteRender";
            paletteRender.Size = new System.Drawing.Size(154, 41);
            paletteRender.TabIndex = 10;
            paletteRender.TabStop = false;
            paletteRender.DoubleClick += paletteRender_DoubleClick;
            paletteRender.MouseDown += paletteRender_MouseDown;
            // 
            // colorRender
            // 
            colorRender.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            colorRender.Location = new System.Drawing.Point(134, 268);
            colorRender.Margin = new System.Windows.Forms.Padding(4);
            colorRender.Name = "colorRender";
            colorRender.Size = new System.Drawing.Size(38, 41);
            colorRender.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            colorRender.TabIndex = 11;
            colorRender.TabStop = false;
            // 
            // Xlabel
            // 
            Xlabel.AutoSize = true;
            Xlabel.Location = new System.Drawing.Point(384, 268);
            Xlabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Xlabel.Name = "Xlabel";
            Xlabel.Size = new System.Drawing.Size(21, 20);
            Xlabel.TabIndex = 12;
            Xlabel.Text = "X:";
            // 
            // Ylabel
            // 
            Ylabel.AutoSize = true;
            Ylabel.Location = new System.Drawing.Point(384, 288);
            Ylabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Ylabel.Name = "Ylabel";
            Ylabel.Size = new System.Drawing.Size(20, 20);
            Ylabel.TabIndex = 13;
            Ylabel.Text = "Y:";
            // 
            // colorRender2
            // 
            colorRender2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            colorRender2.Location = new System.Drawing.Point(180, 268);
            colorRender2.Margin = new System.Windows.Forms.Padding(4);
            colorRender2.Name = "colorRender2";
            colorRender2.Size = new System.Drawing.Size(40, 41);
            colorRender2.TabIndex = 14;
            colorRender2.TabStop = false;
            // 
            // spacerLabel
            // 
            spacerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            spacerLabel.Location = new System.Drawing.Point(8, 316);
            spacerLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            spacerLabel.Name = "spacerLabel";
            spacerLabel.Size = new System.Drawing.Size(420, 2);
            spacerLabel.TabIndex = 15;
            // 
            // iconList
            // 
            iconList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            iconList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("iconList.ImageStream");
            iconList.TransparentColor = System.Drawing.Color.Transparent;
            iconList.Images.SetKeyName(0, "closeallcards.png");
            iconList.Images.SetKeyName(1, "closecard.png");
            iconList.Images.SetKeyName(2, "comparetemp.png");
            // 
            // iconListView
            // 
            iconListView.ContextMenuStrip = iconContextMenu;
            iconListView.LargeImageList = iconList;
            iconListView.Location = new System.Drawing.Point(4, 4);
            iconListView.Name = "iconListView";
            iconListView.Scrollable = false;
            iconListView.Size = new System.Drawing.Size(124, 304);
            iconListView.SmallImageList = iconList;
            iconListView.TabIndex = 16;
            iconListView.UseCompatibleStateImageBehavior = false;
            iconListView.View = System.Windows.Forms.View.Tile;
            iconListView.SelectedIndexChanged += iconListView_SelectedIndexChanged;
            // 
            // iconContextMenu
            // 
            iconContextMenu.ImageScalingSize = new System.Drawing.Size(26, 26);
            iconContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem3, toolStripMenuItem2 });
            iconContextMenu.Name = "iconContextMenu";
            iconContextMenu.Size = new System.Drawing.Size(184, 58);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(183, 24);
            toolStripMenuItem1.Text = "Replace frame...";
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new System.Drawing.Size(180, 6);
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(183, 24);
            toolStripMenuItem2.Text = "Export frame...";
            toolStripMenuItem2.Click += toolStripMenuItem2_Click;
            // 
            // iconTimer
            // 
            iconTimer.Interval = 180;
            iconTimer.Tick += iconTimer_Tick;
            // 
            // penRadio
            // 
            penRadio.Appearance = System.Windows.Forms.Appearance.Button;
            penRadio.Checked = true;
            penRadio.Location = new System.Drawing.Point(392, 4);
            penRadio.Name = "penRadio";
            penRadio.Size = new System.Drawing.Size(40, 30);
            penRadio.TabIndex = 21;
            penRadio.TabStop = true;
            penRadio.Text = "🖊";
            penRadio.UseVisualStyleBackColor = true;
            penRadio.CheckedChanged += penRadio_CheckedChanged;
            // 
            // bucketRadio
            // 
            bucketRadio.Appearance = System.Windows.Forms.Appearance.Button;
            bucketRadio.Location = new System.Drawing.Point(392, 36);
            bucketRadio.Name = "bucketRadio";
            bucketRadio.Size = new System.Drawing.Size(40, 30);
            bucketRadio.TabIndex = 22;
            bucketRadio.Text = "\U0001fad7";
            bucketRadio.UseVisualStyleBackColor = true;
            bucketRadio.CheckedChanged += bucketRadio_CheckedChanged;
            // 
            // gridSlider
            // 
            gridSlider.Location = new System.Drawing.Point(20, 324);
            gridSlider.Maximum = 255;
            gridSlider.Name = "gridSlider";
            gridSlider.Size = new System.Drawing.Size(208, 56);
            gridSlider.TabIndex = 23;
            gridSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            gridSlider.Value = 128;
            gridSlider.Scroll += gridSlider_Scroll;
            // 
            // eraserRadio
            // 
            eraserRadio.Appearance = System.Windows.Forms.Appearance.Button;
            eraserRadio.Location = new System.Drawing.Point(392, 68);
            eraserRadio.Name = "eraserRadio";
            eraserRadio.Size = new System.Drawing.Size(40, 30);
            eraserRadio.TabIndex = 24;
            eraserRadio.Text = "\U0001f9fc";
            eraserRadio.UseVisualStyleBackColor = true;
            eraserRadio.CheckedChanged += eraserRadio_CheckedChanged;
            // 
            // gridCheckbox
            // 
            gridCheckbox.AutoSize = true;
            gridCheckbox.Location = new System.Drawing.Point(8, 328);
            gridCheckbox.Name = "gridCheckbox";
            gridCheckbox.Size = new System.Drawing.Size(18, 17);
            gridCheckbox.TabIndex = 25;
            gridCheckbox.UseVisualStyleBackColor = true;
            gridCheckbox.CheckedChanged += gridCheckbox_CheckedChanged;
            // 
            // iconWindow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(433, 360);
            Controls.Add(gridCheckbox);
            Controls.Add(eraserRadio);
            Controls.Add(gridSlider);
            Controls.Add(bucketRadio);
            Controls.Add(penRadio);
            Controls.Add(iconListView);
            Controls.Add(spacerLabel);
            Controls.Add(colorRender2);
            Controls.Add(Ylabel);
            Controls.Add(Xlabel);
            Controls.Add(colorRender);
            Controls.Add(paletteRender);
            Controls.Add(iconRender);
            Controls.Add(rightButton);
            Controls.Add(leftButton);
            Controls.Add(vFlipButton);
            Controls.Add(hFlipButton);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "iconWindow";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "iconWindow";
            Load += iconWindow_Load;
            ((System.ComponentModel.ISupportInitialize)iconRender).EndInit();
            ((System.ComponentModel.ISupportInitialize)paletteRender).EndInit();
            ((System.ComponentModel.ISupportInitialize)colorRender).EndInit();
            ((System.ComponentModel.ISupportInitialize)colorRender2).EndInit();
            iconContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridSlider).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
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
        private System.Windows.Forms.ImageList iconList;
        private System.Windows.Forms.ListView iconListView;
        private System.Windows.Forms.Timer iconTimer;
        private System.Windows.Forms.ContextMenuStrip iconContextMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.RadioButton penRadio;
        private System.Windows.Forms.RadioButton bucketRadio;
        private System.Windows.Forms.TrackBar gridSlider;
        private System.Windows.Forms.RadioButton eraserRadio;
        private System.Windows.Forms.CheckBox gridCheckbox;
    }
}