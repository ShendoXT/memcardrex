//PSX icon editor for MemcardRex
//Shendo 2009-2013

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace MemcardRex
{
    public partial class iconWindow : Form
    {
        //Icon data
        public byte[] iconData;
        Color[] iconPalette = new Color[16];
        Bitmap[] iconBitmap = new Bitmap[3];
        int[] selectedColor = new int[2];
        int selectedIcon = 0;

        //If dialog was closed with OK this will be true
        public bool okPressed = false;

        public iconWindow()
        {
            InitializeComponent();
        }

        //Initialize default values
        public void initializeDialog(string dialogTitle, int iconFrames, byte[] iconBytes)
        {
            this.Text = dialogTitle;
            iconData = iconBytes;

            switch (iconFrames)
            {
                default:        //Assume that there is only one icon frame
                    frameCombo.Items.Add("1st frame");
                    frameCombo.Enabled = false;
                    break;

                case 2:         //Two icons
                    frameCombo.Items.Add("1st frame");
                    frameCombo.Items.Add("2nd frame");
                    break;

                case 3:         //Three icons
                    frameCombo.Items.Add("1st frame");
                    frameCombo.Items.Add("2nd frame");
                    frameCombo.Items.Add("3rd frame");
                    break;
            }

            //Draw palette and icon
            setUpDisplay();

            //Select first frame
            frameCombo.SelectedIndex = 0;
        }

        //Set everything up for drawing
        private void setUpDisplay()
        {
            //Draw palette to color selector
            drawPalette();

            //Set selected colors to first and second colors in the palete
            setSelectedColor(0, 0);
            setSelectedColor(1, 1);

            //Draw icon on the icon render
            drawIcon();
        }

        //Load palette, copied from ps1class :p
        private void loadPalette()
        {
            int redChannel = 0;
            int greenChannel = 0;
            int blueChannel = 0;
            int colorCounter = 0;

            //Clear existing data
            iconPalette = new Color[16];

            //Reset color counter
            colorCounter = 0;

            //Fetch two bytes at a time
            for (int byteCount = 0; byteCount < 32; byteCount += 2)
            {
                redChannel = (iconData[byteCount] & 0x1F) << 3;
                greenChannel = ((iconData[byteCount + 1] & 0x3) << 6) | ((iconData[byteCount] & 0xE0) >> 2);
                blueChannel = ((iconData[byteCount + 1] & 0x7C) << 1);

                //Get the color value
                iconPalette[colorCounter] = Color.FromArgb(redChannel, greenChannel, blueChannel);
                colorCounter++;
            }
        }

        //Load icons, copied from the ps1class :p
        private void loadIcons()
        {
            int byteCount = 0;

            //Clear existing data
            iconBitmap = new Bitmap[3];

            //Each save has 3 icons (some are data but those will not be shown)
            for (int iconNumber = 0; iconNumber < 3; iconNumber++)
            {
                iconBitmap[iconNumber] = new Bitmap(16, 16);
                byteCount = 32 + (128 * iconNumber);

                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x += 2)
                    {
                        iconBitmap[iconNumber].SetPixel(x, y, iconPalette[iconData[byteCount] & 0xF]);
                        iconBitmap[iconNumber].SetPixel(x + 1, y, iconPalette[iconData[byteCount] >> 4]);
                        byteCount++;
                    }
                }
            }
        }

        //Draw selected icon to render
        private void drawIcon()
        {
            Bitmap drawBitmap = new Bitmap(177, 177);
            Graphics drawGraphics = Graphics.FromImage(drawBitmap);
            Pen blackPen = new Pen(Color.Black);

            //Load icon data
            loadIcons();

            drawGraphics.PixelOffsetMode = PixelOffsetMode.Half;
            drawGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            //Draw selected icon to drawBitmap
            drawGraphics.DrawImage(iconBitmap[selectedIcon], 0, 0, 177, 177);

            //Set offset mode to default so grid can be drawn
            drawGraphics.PixelOffsetMode = PixelOffsetMode.Default;

            //Draw grid
            for (int y = 0; y < 17; y++)
                drawGraphics.DrawLine(blackPen, 0, (y * 11), 177, (y * 11));

            for (int x = 0; x < 17; x++)
                drawGraphics.DrawLine(blackPen, (x * 11), 0, (x * 11), 177);

            drawGraphics.Dispose();
            iconRender.Image = drawBitmap;
        }

        //Draw palette image to render
        private void drawPalette()
        {
            Bitmap paletteBitmap = new Bitmap(8, 2);
            Bitmap drawBitmap = new Bitmap(121, 31);
            Graphics drawGraphics = Graphics.FromImage(drawBitmap);
            Pen blackPen = new Pen(Color.Black);
            int colorCounter = 0;

            //Load pallete data
            loadPalette();

            drawGraphics.PixelOffsetMode = PixelOffsetMode.Half;
            drawGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            //Plot pixels onto bitmap
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    paletteBitmap.SetPixel(x, y, iconPalette[colorCounter]);
                    colorCounter++;
                }
            }

            //Draw palette to drawBitmap
            drawGraphics.DrawImage(paletteBitmap, 0, 0, 120, 30);

            //Set offset mode to default so grid can be drawn
            drawGraphics.PixelOffsetMode = PixelOffsetMode.Default;

            //Draw grid
            for (int y = 0; y < 3; y++)
                drawGraphics.DrawLine(blackPen, 0, (y * 15), 121, (y * 15));

            for (int x = 0; x < 9; x++)
                drawGraphics.DrawLine(blackPen, (x * 15), 0, (x * 15), 31);

            drawGraphics.Dispose();
            paletteRender.Image = drawBitmap;
        }

        //Set selected color
        private void setSelectedColor(int selColor, int selectedColorIndex)
        {
            if (selectedColorIndex == 0)
            {
                selectedColor[0] = selColor;
                colorRender.BackColor = iconPalette[selectedColor[0]];
            }
            else
            {
                selectedColor[1] = selColor;
                colorRender2.BackColor = iconPalette[selectedColor[1]];
            }
        }

        //Place pixel on the selected icon
        private void putPixel(int X, int Y, int selectedColorIndex)
        {
            //Calculate destination byte to draw pixel to
            int destinationByte = (X + (Y * 16)) / 2;

            //Check what nibble to draw pixel to
            if ((X + (Y * 16)) % 2 == 0)
            {
                iconData[32 + destinationByte + (selectedIcon * 128)] &= 0xF0;
                iconData[32 + destinationByte + (selectedIcon * 128)] |= (byte)selectedColor[selectedColorIndex];
            }
            else
            {
                iconData[32 + destinationByte + (selectedIcon * 128)] &= 0x0F;
                iconData[32 + destinationByte + (selectedIcon * 128)] |= (byte)(selectedColor[selectedColorIndex] << 4);
            }

            //Redraw icon
            drawIcon();
        }

        //Import currently selected icon
        private void importIcon()
        {
            Bitmap OpenedBitmap = null;

            int redChannel = 0;
            int greenChannel = 0;
            int blueChannel = 0;

            Color tempColor = new Color();
            List<Color> foundColors = new List<Color>();

            byte tempIndex = 0;
            byte[,] returnData = new byte[16, 16];

            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Title = "Open icon";
            openDlg.Filter = "All supported|*.bmp;*.gif;*.jpeg;*.jpg;*.png|Bitmap (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpeg;*.jpg)|*.jpeg;*.jpg|PNG (*.png)|*.png";

            //Check if the user pressed OK
            if (openDlg.ShowDialog() != DialogResult.OK) return;


            try
            {
                OpenedBitmap = new Bitmap(openDlg.FileName);
            }
            catch (Exception e)
            {;
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                OpenedBitmap.Dispose();
                return;
            }

            //Check if the image is 16x16 pixels
            if (OpenedBitmap.Width != 16 || OpenedBitmap.Height != 16)
            {
                MessageBox.Show("Selected image is not a 16x16 pixel image.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                OpenedBitmap.Dispose();
                return;
            }

            //Create a palette from the given image
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    //Check if the given color exists and add it if it doesn't
                    if(!foundColors.Contains(OpenedBitmap.GetPixel(x,y)))foundColors.Add(OpenedBitmap.GetPixel(x,y));
                }
            }

            //Check if the palette has more than 16 colors
            if (foundColors.Count > 16)
            {
                MessageBox.Show("Selected image contains more than 16 colors.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                OpenedBitmap.Dispose();
                return;
            }

            //Check if some colors should be added to make a 16 color palette
            for (int i = foundColors.Count; i < 16; i++)
            {
                foundColors.Add(Color.Black);
            }

            //Copy palette from the opened Bitmap
            for (int i = 0; i < 16; i++)
            {
                //Get RGB channels from the Bitmap palette
                redChannel = (foundColors[i].R >> 3);
                greenChannel = (foundColors[i].G >> 3);
                blueChannel = (foundColors[i].B >> 3);

                //Set color to iconData (convert 24 bit color to 15 bit)
                iconData[i * 2] = (byte)(redChannel | ((greenChannel & 0x07) << 5));
                iconData[(i * 2) + 1] = (byte)((blueChannel << 2) | ((greenChannel & 0x18) >> 3));
            }

            //Copy image data from opened bitmap
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    //Reset index variable (not necessary, but to be safe anyway)
                    tempIndex = 0;

                    //Get the ARGB color of the current pixel
                    tempColor = OpenedBitmap.GetPixel(x, y);

                    //Cycle through palette to see the current index
                    //This way is a bit resource heavy but since image is always 16x16 it's not an issue
                    //There is no "good" alternative to do it with indexed bitmaps, only the unsafe one
                    for (byte pIndex = 0; pIndex < 16; pIndex++)
                    {
                        if (foundColors[pIndex] == tempColor)
                        {
                            tempIndex = pIndex;
                            break;
                        }
                    }

                    returnData[x, y] = tempIndex;
                }
            }

            setDataGrid(returnData);

            OpenedBitmap.Dispose();

            //Appy the imported icon
            setUpDisplay();
        }

        //Export currently selected icon
        private void exportIcon()
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Title = "Save icon";
            saveDlg.Filter = "Bitmap (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpeg;*.jpg)|*.jpeg;*.jpg|PNG (*.png)|*.png";

            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                ImageFormat imgFormat;

                switch (saveDlg.FilterIndex)
                {
                    default:        //bmp
                        imgFormat = ImageFormat.Bmp;
                        break;

                    case 2:         //gif
                        imgFormat = ImageFormat.Gif;
                        break;

                    case 3:         //jpg
                        imgFormat = ImageFormat.Jpeg;
                        break;

                    case 4:         //png
                        imgFormat = ImageFormat.Png;
                        break;
                }
                //Save the image in the selected format
                iconBitmap[selectedIcon].Save(saveDlg.FileName, imgFormat);
            }
        }

        //Flip the icon horizontally
        private void horizontalFlip()
        {
            byte[,] tempIconData = getDataGrid();
            byte[,] processedData = new byte[16, 16];

            //Process the data
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    processedData[x, y] = tempIconData[15 - x, y];
                }
            }

            //Update icon data
            setDataGrid(processedData);

            //Redraw icon
            drawIcon();
        }

        //Flip the icon vertically
        private void verticalFlip()
        {
            byte[,] tempIconData = getDataGrid();
            byte[,] processedData = new byte[16, 16];

            //Process the data
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    processedData[x, y] = tempIconData[x, 15 - y];
                }
            }

            //Update icon data
            setDataGrid(processedData);

            //Redraw icon
            drawIcon();
        }

        //Rotate the icon to the left
        private void leftRotate()
        {
            byte[,] tempIconData = getDataGrid();
            byte[,] processedData = new byte[16, 16];

            //Process the data
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    processedData[x, y] = tempIconData[y, x];
                }
            }

            //Update icon data
            setDataGrid(processedData);

            //Fix icon and update it
            verticalFlip();
        }

        //Rotate the icon to the right
        private void rightRotate()
        {
            byte[,] tempIconData = getDataGrid();
            byte[,] processedData = new byte[16, 16];

            //Process the data
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    processedData[x, y] = tempIconData[y, x];
                }
            }

            //Update icon data
            setDataGrid(processedData);

            //Fix icon and update it
            horizontalFlip();
        }

        //Get icon data as 16x16 byte grid
        private byte[,] getDataGrid()
        {
            byte[,] returnData = new byte[16, 16];
            int byteCount = 32 + (selectedIcon * 128);

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x+=2)
                {
                    returnData[x, y] = (byte)(iconData[byteCount] & 0x0F);
                    returnData[x + 1, y] = (byte)((iconData[byteCount] & 0xF0)>>4);
                    byteCount++;
                }
            }
            return returnData;
        }

        //Set icon data from 16x16 byte grid
        private void setDataGrid(byte[,] gridData)
        {
            int byteCount = 32 + (selectedIcon * 128);

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x += 2)
                {
                    iconData[byteCount] = (byte)(gridData[x, y] | (gridData[x + 1, y]<<4));
                    byteCount++;
                }
            }
        }

        //User has selected palette color
        private void paletteRender_MouseDown(object sender, MouseEventArgs e)
        {
            int Xpos = e.X / 15;
            int Ypos = e.Y / 15;

            if (Xpos > 7) Xpos = 7;
            if (Ypos > 1) Ypos = 1;

            if(e.Button == MouseButtons.Left)
                setSelectedColor(Xpos + (Ypos * 8), 0);     //Left color selector
            else
                setSelectedColor(Xpos + (Ypos * 8), 1);     //Right color selector
        }

        //Change selected color
        private void paletteRender_DoubleClick(object sender, EventArgs e)
        {
            int selectedColorIndex = 1;

            if (((MouseEventArgs)e).Button == MouseButtons.Left)
                selectedColorIndex = 0;

            ColorDialog colorDlg = new ColorDialog();
            colorDlg.Color = iconPalette[selectedColor[0]];
            colorDlg.FullOpen = true;

            //Apply selected palette color
            if(colorDlg.ShowDialog() == DialogResult.OK)
            {
                //Get each color channel
                int redChannel = (colorDlg.Color.R >> 3);
                int greenChannel = (colorDlg.Color.G >> 3);
                int blueChannel = (colorDlg.Color.B >> 3);

                //Set color to iconData (convert 24 bit color to 15 bit)
                iconData[selectedColor[selectedColorIndex] * 2] = (byte)(redChannel | ((greenChannel & 0x07) << 5));
                iconData[(selectedColor[selectedColorIndex] * 2) + 1] = (byte)((blueChannel << 2) | ((greenChannel & 0x18) >> 3));

                //Draw palette to color selector
                drawPalette();

                //Update selected colors
                setSelectedColor(selectedColor[0], 0);
                setSelectedColor(selectedColor[1], 1);

                //Draw icon on the icon render
                drawIcon();
            }
        }

        //Draw selected icon
        private void frameCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedIcon = frameCombo.SelectedIndex;
            drawIcon();
        }

        //User has selected a pixel to draw to
        private void iconRender_MouseDownMove(object sender, MouseEventArgs e)
        {
            int XposOriginal = e.X / 11;
            int YposOriginal = e.Y / 11;
            int Xpos = e.X / 11;
            int Ypos = e.Y / 11;

            if (Xpos > 15) Xpos = 15;
            if (Ypos > 15) Ypos = 15;
            if (Xpos < 0) Xpos = 0;
            if (Ypos < 0) Ypos = 0;

            Xlabel.Text = "X: " + Xpos.ToString();
            Ylabel.Text = "Y: " + Ypos.ToString();

            //Draw pixels if arrow is in range and left mouse button is pressed
            if (XposOriginal >= 0 && XposOriginal <= 15 && YposOriginal >= 0
                && YposOriginal <= 15)
            {
                //Color with first selected color
                if(e.Button == MouseButtons.Left)
                    putPixel(Xpos, Ypos,0);

                //Color with second selected colro
                if(e.Button == MouseButtons.Right)
                    putPixel(Xpos, Ypos, 1);
            }
        }

        //Mouse has left icon renderer, clear values
        private void iconRender_MouseLeave(object sender, EventArgs e)
        {
            Xlabel.Text = "X:";
            Ylabel.Text = "Y:";
        }

        //Cancel is pressed
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //OK is pressed
        private void okButton_Click(object sender, EventArgs e)
        {
            okPressed = true;
            this.Close();
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            //Open selected icon from image file
            importIcon();
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            //Save selected icon as image file
            exportIcon();
        }

        private void hFlipButton_Click(object sender, EventArgs e)
        {
            //H flip
            horizontalFlip();
        }

        private void vFlipButton_Click(object sender, EventArgs e)
        {
            //V flip
            verticalFlip();
        }

        private void leftButton_Click(object sender, EventArgs e)
        {
            //Rotate left
            leftRotate();
        }

        private void rightButton_Click(object sender, EventArgs e)
        {
            //Rotate right
            rightRotate();
        }

        private void iconWindow_Load(object sender, EventArgs e)
        {

        }
    }
}
