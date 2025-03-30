using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.Versioning;
using System.Windows.Forms;
using MemcardRex.Core;

namespace MemcardRex
{
    [SupportedOSPlatform("windows")]
    public partial class informationWindow : Form
    {
        //Save icons
        Bitmap[] iconData = new Bitmap[3];
        int iconIndex = 0;
        int maxCount = 1;
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
        public void initializeDialog(string saveTitle, string saveProdCode, string saveIdentifier, string saveRegion, ps1card.DataTypes saveType, int saveSize, int iconFrames, Color[,][] saveIcons, int[] slotNumbers, int backColor, double xScale, double yScale)
        {
            string ocupiedSlots = null;

            BmpBuilder bmpImage = new BmpBuilder();

            saveTitleLabel.Text = saveTitle;
            productCodeLabel.Text = saveProdCode;
            identifierLabel.Text = saveIdentifier;
            sizeLabel.Text = saveSize.ToString() + " KB";
            iconFramesLabel.Text = iconFrames.ToString();
            maxCount = iconFrames;
            iconBackColor = backColor;
            regionLabel.Text = saveRegion;

            //Save file data type
            if (saveType == ps1card.DataTypes.save) typeLabel.Text = "Save data";
            else if (saveType == ps1card.DataTypes.software) typeLabel.Text = "Software (PocketStation)";

            //Create icons
            for (int i = 0; i < 3; i++)
            {
                iconData[i] = new Bitmap(new MemoryStream(bmpImage.BuildBmp(saveIcons[slotNumbers[0], i])));
                iconData[i].RotateFlip(RotateFlipType.RotateNoneFlipY);
            }

            //Get ocupied slots
            for (int i = 0; i < slotNumbers.Length; i++)
            {
                ocupiedSlots += (slotNumbers[i] + 1).ToString() + ", ";
            }

            //Show ocupied slots
            slotLabel.Text = ocupiedSlots.Remove(ocupiedSlots.Length-2);

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
            iconGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;

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

            iconGraphics.DrawImage(iconData[selectedIndex], 0, 0, 48, 48);

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
    }
}
