//PSX icon editor for MemcardRex
//Shendo 2009-2024

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using System.Diagnostics;

namespace MemcardRex
{
    [SupportedOSPlatform("windows")]
    public partial class iconWindow : Form
    {
        private enum ToolTypes : int
        {
            Pen,
            Bucket,
            Eraser
        };

        //Icon data
        public byte[] iconData;
        Color[] iconPalette = new Color[16];
        Bitmap[] iconBitmap = new Bitmap[3];
        Bitmap chequeredBitmap = null;
        Bitmap chequeredPalette = null;
        int[] selectedColor = new int[2];
        int transparentEntry = -1;
        int selectedIcon = 0;
        int previewIndex = 0;
        int frameCount = 0;

        double xScale = 1.0;
        double yScale = 1.0;
        int pixelWidth = 11;
        int pixelHeight = 11;
        int paletteWidth = 15;
        int paletteHeight = 15;

        public bool gridEnabled = true;
        public int gridColorValue = 128;

        //Drawing tools
        ToolTypes selectedTool = ToolTypes.Pen;

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
            using (Graphics graphics = CreateGraphics())
            {
                xScale = graphics.DpiX / 96.0;
                yScale = graphics.DpiY / 96.0;
            }

            frameCount = iconFrames;

            //Populate icon list with items
            if (iconFrames > 0) iconListView.Items.Add("1st frame");
            if (iconFrames > 1) iconListView.Items.Add("2nd frame");
            if (iconFrames > 2) iconListView.Items.Add("3rd frame");

            //Assign icon indexes
            for (int i = 0; i < iconListView.Items.Count; i++)
            {
                iconListView.Items[i].ImageIndex = i;
            }

            //Add preview pane if the icons are animated
            if (iconFrames > 1)
            {
                iconListView.Items.Add("Preview");

                //Preview icon is always index 3
                iconListView.Items[iconListView.Items.Count - 1].ImageIndex = 3;
            }

            gridSlider.Value = gridColorValue;
            gridSlider.Enabled = gridEnabled;
            gridCheckbox.Checked = gridEnabled;

            //Draw palette and icon
            setUpDisplay();

            //Select first frame
            iconListView.Items[0].Selected = true;

            //Enable timer if there are more than one frame
            if (iconFrames > 1) iconTimer.Enabled = true;
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
            int alphaChannel = 0;
            int redChannel = 0;
            int greenChannel = 0;
            int blueChannel = 0;
            int colorCounter = 0;
            int blackFlag = 0;

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
                blackFlag = (iconData[byteCount + 1] & 0x80);

                //If the color value is 0 along with no flag it is treated as transparent
                if ((redChannel | greenChannel | blueChannel | blackFlag) == 0) alphaChannel = 0;
                else alphaChannel = 255;

                //Save transparent entry for eraser
                if (alphaChannel == 0) transparentEntry = colorCounter;

                //Get the color value
                iconPalette[colorCounter] = Color.FromArgb(alphaChannel, redChannel, greenChannel, blueChannel);
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

            //Place icons in the image list
            iconList.Images.Clear();

            iconList.Images.Add(iconBitmap[0]);
            iconList.Images.Add(iconBitmap[1]);
            iconList.Images.Add(iconBitmap[2]);

            //Add current preview index icon
            iconList.Images.Add(iconBitmap[previewIndex]);
        }

        //Draw selected icon to render
        private void drawIcon()
        {
            int iconFrame = 0;

            if (selectedIcon > frameCount - 1)
            {
                iconFrame = previewIndex;
            }
            else
            {
                iconFrame = selectedIcon;
            }

            pixelWidth = (int)(xScale * 13);
            pixelHeight = (int)(yScale * 13);
            Bitmap drawBitmap = new Bitmap(pixelWidth * 16 + 1, pixelHeight * 16 + 1);

            Graphics drawGraphics = Graphics.FromImage(drawBitmap);
            Pen blackPen = new Pen(Color.FromArgb(gridColorValue, gridColorValue, gridColorValue));

            //Create chequered bitmap only on the first run
            if (chequeredBitmap == null)
            {
                chequeredBitmap = new Bitmap(32, 32);

                bool isEvenRow;
                for (int y = 0; y < 32; y++)
                {
                    isEvenRow = (y % 2 == 0);
                    for (int x = 0; x < 32; x += 2)
                    {
                        chequeredBitmap.SetPixel(x, y, isEvenRow ? Color.White : Color.Gray);
                        chequeredBitmap.SetPixel(x + 1, y, isEvenRow ? Color.Gray : Color.White);
                    }
                }
            }
            //Load icon data
            loadIcons();

            drawGraphics.PixelOffsetMode = PixelOffsetMode.Half;
            drawGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            //Draw background bitmap
            drawGraphics.DrawImage(chequeredBitmap, 0, 0, pixelWidth * 16, pixelHeight * 16);

            //Draw selected icon to drawBitmap
            drawGraphics.DrawImage(iconBitmap[iconFrame], 0, 0, pixelWidth * 16, pixelHeight * 16);

            //Set offset mode to default so grid can be drawn
            drawGraphics.PixelOffsetMode = PixelOffsetMode.Default;

            //Draw grid
            if (gridEnabled)
            {
                for (int y = 0; y < 17; y++)
                    drawGraphics.DrawLine(blackPen, 0, (y * pixelHeight), pixelWidth * 16, (y * pixelHeight));

                for (int x = 0; x < 17; x++)
                    drawGraphics.DrawLine(blackPen, (x * pixelWidth), 0, (x * pixelWidth), pixelHeight * 16);
            }

            drawGraphics.Dispose();
            iconRender.Image = drawBitmap;
        }

        //Draw palette image to render
        private void drawPalette()
        {
            paletteWidth = (int)(xScale * 15);
            paletteHeight = (int)(yScale * 15);
            Bitmap paletteBitmap = new Bitmap(8, 2);
            Bitmap drawBitmap = new Bitmap(8 * paletteWidth + 1, 2 * paletteHeight + 1);
            Graphics drawGraphics = Graphics.FromImage(drawBitmap);
            Pen blackPen = new Pen(Color.Black);
            int colorCounter = 0;
            colorRender.Height = paletteHeight * 2 + 1;
            colorRender2.Height = paletteHeight * 2 + 1;

            //Create chequered bitmap only on the first run
            if (chequeredPalette == null)
            {
                chequeredPalette = new Bitmap(16, 4);

                bool isEvenRow;
                for (int y = 0; y < 4; y++)
                {
                    isEvenRow = (y % 2 == 0);
                    for (int x = 0; x < 16; x += 2)
                    {
                        chequeredPalette.SetPixel(x, y, isEvenRow ? Color.White : Color.Gray);
                        chequeredPalette.SetPixel(x + 1, y, isEvenRow ? Color.Gray : Color.White);
                    }
                }
            }

            //Load palette data
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

            //Draw chequered background
            drawGraphics.DrawImage(chequeredPalette, 0, 0, 8 * paletteWidth, 2 * paletteHeight);

            //Draw palette to drawBitmap
            drawGraphics.DrawImage(paletteBitmap, 0, 0, 8 * paletteWidth, 2 * paletteHeight);

            //Set offset mode to default so grid can be drawn
            drawGraphics.PixelOffsetMode = PixelOffsetMode.Default;

            //Draw grid
            for (int y = 0; y < 3; y++)
                drawGraphics.DrawLine(blackPen, 0, (y * paletteHeight), 8 * paletteWidth, (y * paletteHeight));

            for (int x = 0; x < 9; x++)
                drawGraphics.DrawLine(blackPen, (x * paletteWidth), 0, (x * paletteWidth), 2 * paletteHeight);

            drawGraphics.Dispose();
            paletteRender.Image = drawBitmap;
        }

        //Set selected color
        private void setSelectedColor(int selColor, int selectedColorIndex)
        {
            Bitmap bgBitmap = new Bitmap(2, 2);
            Bitmap renderBitmap = new Bitmap(colorRender.Width, colorRender.Height);
            Graphics bgGraphics = Graphics.FromImage(renderBitmap);

            Color blockOne = Color.White;
            Color blockTwo = Color.Gray;

            selectedColor[selectedColorIndex] = selColor;

            //Change to real colors if it is not a transparent color
            if (iconPalette[selectedColor[selectedColorIndex]].A != 0)
            {
                blockOne = iconPalette[selectedColor[selectedColorIndex]];
                blockTwo = iconPalette[selectedColor[selectedColorIndex]];
            }

            bgBitmap.SetPixel(0, 0, blockOne);
            bgBitmap.SetPixel(1, 0, blockTwo);
            bgBitmap.SetPixel(0, 1, blockTwo);
            bgBitmap.SetPixel(1, 1, blockOne);

            bgGraphics.PixelOffsetMode = PixelOffsetMode.Half;
            bgGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            bgGraphics.DrawImage(bgBitmap, 0, 0, renderBitmap.Width, renderBitmap.Height);
            bgGraphics.Dispose();

            if (selectedColorIndex == 0)
                colorRender.Image = renderBitmap;
            else
                colorRender2.Image = renderBitmap;
        }

        //Place pixel on the selected icon
        private void putPixel(int X, int Y, int colorIndex)
        {
            int offset = 32 + (selectedIcon * 128);
            int pixelIndex = X + (Y * 16);
            int destinationByte = pixelIndex / 2;
            int targetIndex = offset + destinationByte;

            // Apply color to either the lower or upper nibble
            if (pixelIndex % 2 == 0)
            {
                iconData[targetIndex] = (byte)((iconData[targetIndex] & 0xF0) | colorIndex);
            }
            else
            {
                iconData[targetIndex] = (byte)((iconData[targetIndex] & 0x0F) | (colorIndex << 4));
            }

            drawIcon();
        }


        //Get color index of a single pixel
        private int getPixel(int X, int Y)
        {
            int offset = 32 + (selectedIcon * 128);
            int pixelIndex = X + (Y * 16);
            int destinationByte = pixelIndex / 2;
            int targetIndex = offset + destinationByte;

            byte data = iconData[targetIndex];

            // Return the color index from either the lower or upper nibble
            if (pixelIndex % 2 == 0)
            {
                return data & 0x0F; // Lower nibble
            }
            else
            {
                return (data >> 4) & 0x0F; // Upper nibble
            }
        }

        //Bucket flood fill tool
        public void FloodFill(int startX, int startY, int selectedColorIndex)
        {
            const int width = 16;
            const int height = 16;

            int targetColor = getPixel(startX, startY);

            // Avoid filling if the color is the same
            if (targetColor == selectedColorIndex)
                return;

            Queue<(int, int)> pixels = new Queue<(int, int)>();
            pixels.Enqueue((startX, startY));

            while (pixels.Count > 0)
            {
                var (x, y) = pixels.Dequeue();

                // Skip if out of bounds
                if (x < 0 || x >= width || y < 0 || y >= height)
                    continue;

                // Skip if not the target color
                if (getPixel(x, y) != targetColor)
                    continue;

                // Set the pixel to the new color
                putPixel(x, y, selectedColorIndex);

                // Add neighboring pixels
                pixels.Enqueue((x + 1, y));
                pixels.Enqueue((x - 1, y));
                pixels.Enqueue((x, y + 1));
                pixels.Enqueue((x, y - 1));
            }
        }

        //Import currently selected icon
        private void importIcon()
        {
            //Do not import to previews
            if (selectedIcon >= frameCount) return;

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
            {
                ;
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
                    if (!foundColors.Contains(OpenedBitmap.GetPixel(x, y))) foundColors.Add(OpenedBitmap.GetPixel(x, y));
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

                //Make sure to get true black and not transparent
                if (iconData[i * 2] == 0 && iconData[(i * 2) + 1] == 0)
                    iconData[(i * 2) + 1] |= 0x80;
            }

            //No transparent entry available
            transparentEntry = -1;

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
            //Do not export previews
            if (selectedIcon >= frameCount) return;

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
                for (int x = 0; x < 16; x += 2)
                {
                    returnData[x, y] = (byte)(iconData[byteCount] & 0x0F);
                    returnData[x + 1, y] = (byte)((iconData[byteCount] & 0xF0) >> 4);
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
                    iconData[byteCount] = (byte)(gridData[x, y] | (gridData[x + 1, y] << 4));
                    byteCount++;
                }
            }
        }

        //User has selected palette color
        private void paletteRender_MouseDown(object sender, MouseEventArgs e)
        {
            int Xpos = e.X / paletteWidth;
            int Ypos = e.Y / paletteHeight;

            if (Xpos > 7) Xpos = 7;
            if (Ypos > 1) Ypos = 1;

            if (e.Button == MouseButtons.Left)
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
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                //Get each color channel
                int redChannel = (colorDlg.Color.R >> 3);
                int greenChannel = (colorDlg.Color.G >> 3);
                int blueChannel = (colorDlg.Color.B >> 3);

                //Set color to iconData (convert 24 bit color to 15 bit)
                iconData[selectedColor[selectedColorIndex] * 2] = (byte)(redChannel | ((greenChannel & 0x07) << 5));
                iconData[(selectedColor[selectedColorIndex] * 2) + 1] = (byte)((blueChannel << 2) | ((greenChannel & 0x18) >> 3));

                //Make sure black does not become transparent
                if (iconData[selectedColor[selectedColorIndex] * 2] == 0 && iconData[(selectedColor[selectedColorIndex] * 2) + 1] == 0)
                    iconData[(selectedColor[selectedColorIndex] * 2) + 1] |= 0x80;

                //If this current entry was transparent reset index as it's no longer not
                if (transparentEntry == selectedColor[selectedColorIndex]) transparentEntry = -1;

                //Draw palette to color selector
                drawPalette();

                //Update selected colors
                setSelectedColor(selectedColor[0], 0);
                setSelectedColor(selectedColor[1], 1);

                //Draw icon on the icon render
                drawIcon();
            }
        }

        //User has selected a pixel to draw to
        private void iconRender_MouseDownMove(object sender, MouseEventArgs e)
        {
            //Only draw on frames, not preview
            if (selectedIcon >= frameCount) return;

            int XposOriginal = e.X / pixelWidth;
            int YposOriginal = e.Y / pixelHeight;
            int Xpos = e.X / pixelWidth;
            int Ypos = e.Y / pixelHeight;

            int paletteIndex = e.Button == MouseButtons.Right ? 1 : 0;

            if (Xpos > 15) Xpos = 15;
            if (Ypos > 15) Ypos = 15;
            if (Xpos < 0) Xpos = 0;
            if (Ypos < 0) Ypos = 0;

            Xlabel.Text = "X: " + Xpos.ToString();
            Ylabel.Text = "Y: " + Ypos.ToString();

            //Draw pixels if arrow is in range and mousebutton is pressed
            if (XposOriginal >= 0 && XposOriginal <= 15 && YposOriginal >= 0
                && YposOriginal <= 15 && (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left))
            {
                switch (selectedTool)
                {
                    case ToolTypes.Pen:
                        putPixel(Xpos, Ypos, selectedColor[paletteIndex]);
                        break;

                    case ToolTypes.Bucket:
                        FloodFill(Xpos, Ypos, selectedColor[paletteIndex]);
                        break;

                    case ToolTypes.Eraser:
                        //Erase only if transparent entry is available
                        if (transparentEntry >= 0) putPixel(Xpos, Ypos, transparentEntry);
                        break;
                }
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

        private void hFlipButton_Click(object sender, EventArgs e)
        {
            //Only draw on frames, not preview
            if (selectedIcon >= frameCount) return;

            //H flip
            horizontalFlip();
        }

        private void vFlipButton_Click(object sender, EventArgs e)
        {
            //Only draw on frames, not preview
            if (selectedIcon >= frameCount) return;

            //V flip
            verticalFlip();
        }

        private void leftButton_Click(object sender, EventArgs e)
        {
            //Only draw on frames, not preview
            if (selectedIcon >= frameCount) return;

            //Rotate left
            leftRotate();
        }

        private void rightButton_Click(object sender, EventArgs e)
        {
            //Only draw on frames, not preview
            if (selectedIcon >= frameCount) return;

            //Rotate right
            rightRotate();
        }

        private void iconWindow_Load(object sender, EventArgs e)
        {

        }

        private void iconTimer_Tick(object sender, EventArgs e)
        {
            if (previewIndex < frameCount - 1) previewIndex++; else previewIndex = 0;

            iconList.Images.RemoveAt(3);
            iconList.Images.Add(iconBitmap[previewIndex]);
            iconListView.Refresh();

            //Also animate main render if preview is selected
            if (selectedIcon > frameCount - 1) drawIcon();
        }

        private void iconListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (iconListView.SelectedItems.Count < 1) return;

            selectedIcon = iconListView.SelectedItems[0].Index;
            drawIcon();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Open selected icon from image file
            importIcon();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //Save selected icon as image file
            exportIcon();
        }

        private void penRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (!penRadio.Checked) return;
            selectedTool = ToolTypes.Pen;
        }

        private void bucketRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (!bucketRadio.Checked) return;
            selectedTool = ToolTypes.Bucket;
        }

        private void gridSlider_Scroll(object sender, EventArgs e)
        {
            gridColorValue = gridSlider.Value;
            drawIcon();
        }

        private void eraserRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (!eraserRadio.Checked) return;
            selectedTool = ToolTypes.Eraser;

            //Notify user of a required transparent entry
            if(transparentEntry < 0)
            {
                if(MessageBox.Show("Eraser tool requires transparent entry in the palette." +
                    "\nDo you want to change currently selected color to transparent?", "Transparent entry required",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    //Set transparent color to iconData
                    iconData[selectedColor[0] * 2] = 0;
                    iconData[(selectedColor[0] * 2) + 1] = 0;

                    //Draw palette to color selector
                    drawPalette();

                    //Update selected colors
                    setSelectedColor(selectedColor[0], 0);
                    setSelectedColor(selectedColor[1], 1);

                    //Draw icon on the icon render
                    drawIcon();
                }
            }
        }

        private void gridCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            gridEnabled = gridCheckbox.Checked;
            gridSlider.Enabled = gridEnabled;
            drawIcon();
        }
    }
}
