using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace MemcardRex
{
    public partial class informationWindow : Form
    {
        //Save icons
        Bitmap[] iconData;
        int iconIndex = 0;
        int maxCount = 1;
        int iconInterpolationMode = 0;
        int iconSize = 0;
        int iconBackColor = 0;

        public informationWindow()
        {
            InitializeComponent();
        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Initialize default values
        public void initializeDialog(string saveTitle, string saveProdCode, string saveIdentifier, string saveRegion, string saveSize, int iconFrames, int interpolationMode, int iconPropertiesSize,  Bitmap[] saveIcons, string slotNumber, int backColor)
        {
            string ocupiedSlots = null;

            iconInterpolationMode = interpolationMode;
            iconSize = iconPropertiesSize;
            saveTitleLabel.Text = saveTitle;
            productCodeLabel.Text = saveProdCode;
            identifierLabel.Text = saveIdentifier;
            sizeLabel.Text = saveSize;
            iconFramesLabel.Text = iconFrames.ToString();
            maxCount = iconFrames;
            iconData = saveIcons;
            iconBackColor = backColor;

            //Show region string
            switch (saveRegion)
            {
                default:        //Formatted save, Corrupted save, Unknown region
                    regionLabel.Text = "Custom";
                    break;

                case "BA":    //American region
                    regionLabel.Text = "America";
                    break;

                case "BE":    //European region
                    regionLabel.Text = "Europe";
                    break;

                case "BI":    //Japanese region
                    regionLabel.Text = "Japan";
                    break;
            }

            //Show slot count
            slotLabel.Text = slotNumber;

            //Draw first icon so there is no delay
            drawIcons(iconIndex);

            //Enable Paint timer in case of multiple frames
            if (iconFrames > 1) iconPaintTimer.Enabled = true;
        }

        //Draw scaled icons
        private void drawIcons(int selectedIndex)
        {
            Bitmap tempBitmap = new Bitmap(48, 48);
            Graphics iconGraphics = Graphics.FromImage(tempBitmap);

            //Set icon interpolation mode
            if(iconInterpolationMode == 0)iconGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            else iconGraphics.InterpolationMode = InterpolationMode.Bilinear;

            iconGraphics.PixelOffsetMode = PixelOffsetMode.Half;

            //Check what background color should be set
            switch (iconBackColor)
            {
                case 1:     //Black
                    iconGraphics.FillRegion(new SolidBrush(Color.Black), new Region(new Rectangle(0, 0, 48, 48)));
                    break;

                case 2:     //Gray
                    iconGraphics.FillRegion(new SolidBrush(Color.FromArgb(0xFF, 0x30, 0x30, 0x30)), new Region(new Rectangle(0, 0, 48, 48)));
                    break;

                case 3:     //Blue
                    iconGraphics.FillRegion(new SolidBrush(Color.FromArgb(0xFF, 0x44, 0x44, 0x98)), new Region(new Rectangle(0, 0, 48, 48)));
                    break;
            }

            iconGraphics.DrawImage(iconData[selectedIndex], 0, 0, 32 + (iconSize * 16), 32 + (iconSize * 16));

            iconRender.Image = tempBitmap;
            iconGraphics.Dispose();
        }

        private void informationWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Disable Paint timer
            iconPaintTimer.Enabled = false;
        }

        private void iconPaintTimer_Tick(object sender, EventArgs e)
        {
            if (iconIndex < (maxCount-1)) iconIndex++; else iconIndex = 0;
            drawIcons(iconIndex);
        }

        private void informationWindow_Load(object sender, EventArgs e)
        {

        }
    }
}
