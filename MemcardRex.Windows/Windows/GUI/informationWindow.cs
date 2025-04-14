using System;
using System.Diagnostics;
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
        Bitmap[] mcIconData;
        Bitmap[] apIconData;
        int iconIndex = 0;
        int mcIconIndex = 0;
        int apIconIndex = 0;
        int maxCount = 1;
        int iconBackColor = 0;
        int apIconDelay = 0;
        int apDelayCount = 0;

        public informationWindow()
        {
            InitializeComponent();
        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Initialize default values
        public void initializeDialog(string saveTitle, string saveProdCode, string saveIdentifier, string saveRegion,
            ps1card.DataTypes saveType, int saveSize, int iconFrames, Color[,][] saveIcons, byte[] mcIcons, byte[] apIcons,
            int iconDelay, int[] slotNumbers, int backColor)
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

            //Needed for proper scaling on different DPI settings
            iconRender.Width = 48;
            iconRender.Height = 48;

            Graphics graphics = CreateGraphics();

            //Calculate position of the middle render for smaller DPI
            if(graphics.DpiY <= 96.0)
            pocketIconRender.Top = pocketIconRender2.Top - pocketIconRender.Height - (int)(4 * graphics.DpiY / 96.0);

            //Create icons
            for (int i = 0; i < 3; i++)
            {
                iconData[i] = new Bitmap(new MemoryStream(bmpImage.BuildBmp(saveIcons[slotNumbers[0], i])));
            }

            apIconDelay = iconDelay / 10;
            apDelayCount = apIconDelay;

            //Create mcIcons (if available)
            if(mcIcons != null)
            {
                mcIconData = new Bitmap[mcIcons.Length / 0x80];
                byte[] mcIconArray = new byte[0x80];

                //Add info to icon frames
                iconFramesLabel.Text += " (" + mcIconData.Length + ")";

                for(int i = 0; i < mcIconData.Length; i++)
                {
                    Array.Copy(mcIcons, i * 0x80, mcIconArray, 0, 0x80);
                    mcIconData[i] = new Bitmap(new MemoryStream(bmpImage.BuildBmp(mcIconArray)));
                }
            }

            //Create apIcons (if available)
            if(apIcons != null)
            {
                apIconData = new Bitmap[apIcons.Length / 0x80];
                byte[] apIconArray = new byte[0x80];

                //Add info to icon frames
                iconFramesLabel.Text += " (" + apIconData.Length + ")";

                for (int i = 0; i < apIconData.Length; i++)
                {
                    Array.Copy(apIcons, i * 0x80, apIconArray, 0, 0x80);
                    apIconData[i] = new Bitmap(new MemoryStream(bmpImage.BuildBmp(apIconArray)));
                }

                //PocketStation starts with a 2nd icon in the APIcon list if there is more than 1 icon
                //Why? I have no idea but we will emulate this in this information dialog
                if (apIconData.Length > 1) apIconIndex = 1;
            }

            //Get ocupied slots
            for (int i = 0; i < slotNumbers.Length; i++)
            {
                ocupiedSlots += (slotNumbers[i] + 1).ToString() + ", ";
            }

            //Show ocupied slots
            slotLabel.Text = ocupiedSlots.Remove(ocupiedSlots.Length-2);

            //Draw first icon so there is no delay
            drawIcons();

            //Enable Paint timer
            iconPaintTimer.Enabled = true;
        }

        //Draw scaled icons
        private void drawIcons()
        {
            Bitmap tempBitmap = new Bitmap(48, 48);
            Bitmap mcTempBitmap = new Bitmap(64, 64);
            Bitmap apTempBitmap = new Bitmap(64, 64);
            Graphics iconGraphics = Graphics.FromImage(tempBitmap);
            Graphics mcIconGraphics = Graphics.FromImage(mcTempBitmap);
            Graphics apIconGraphics = Graphics.FromImage(apTempBitmap);

            //Set icon interpolation mode
            iconGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            mcIconGraphics.InterpolationMode = pocketIconRender.Width < 64 ? InterpolationMode.Bilinear : InterpolationMode.NearestNeighbor;
            apIconGraphics.InterpolationMode = pocketIconRender.Width < 64 ? InterpolationMode.Bilinear : InterpolationMode.NearestNeighbor;

            iconGraphics.PixelOffsetMode = PixelOffsetMode.Half;
            mcIconGraphics.PixelOffsetMode = PixelOffsetMode.Half;
            apIconGraphics.PixelOffsetMode = PixelOffsetMode.Half;

            Rectangle iconRectangle = new Rectangle(0, 0, iconRender.Width, iconRender.Height);

            //Check what background color should be set
            switch (iconBackColor)
            {
                case 1:     //Black
                    iconGraphics.FillRegion(new SolidBrush(Color.Black), new Region(iconRectangle));
                    break;

                case 2:     //Gray
                    iconGraphics.FillRegion(new SolidBrush(Color.FromArgb(0xFF, 0x30, 0x30, 0x30)), new Region(iconRectangle));
                    break;

                case 3:     //Blue
                    iconGraphics.FillRegion(new SolidBrush(Color.FromArgb(0xFF, 0x44, 0x44, 0x98)), new Region(iconRectangle));
                    break;
            }

            iconGraphics.DrawImage(iconData[iconIndex], 0, 0, iconRender.Width, iconRender.Height);

            iconRender.Image = tempBitmap;
            iconGraphics.Dispose();

            if (mcIconData != null)
            {
                mcIconGraphics.DrawImage(mcIconData[mcIconIndex], 0, 0, pocketIconRender.Width, pocketIconRender.Height);
                pocketIconRender.Image = mcTempBitmap;
                mcIconGraphics.Dispose();

                if (mcIconIndex < (mcIconData.Length - 1)) mcIconIndex++; else mcIconIndex = 0;
            }

            if (apIconData != null)
            {
                apIconGraphics.DrawImage(apIconData[apIconIndex], 0, 0, pocketIconRender.Width, pocketIconRender.Height);

                if(mcIconData != null) pocketIconRender2.Image = apTempBitmap;
                else pocketIconRender.Image = apTempBitmap;
                apIconGraphics.Dispose();

                if (apDelayCount > 0) apDelayCount--;
                else
                {
                    if (apIconIndex < (apIconData.Length - 1)) apIconIndex++; else apIconIndex = 0;
                    apDelayCount = apIconDelay;
                }

            }

            if (iconIndex < (maxCount - 1)) iconIndex++; else iconIndex = 0;
        }

        private void informationWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Disable Paint timer
            iconPaintTimer.Enabled = false;
        }

        private void iconPaintTimer_Tick(object sender, EventArgs e)
        {
            drawIcons();
        }
    }
}
