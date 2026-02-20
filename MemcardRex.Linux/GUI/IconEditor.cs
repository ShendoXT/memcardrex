using System;
using System.Drawing;
using Gtk;
using Cairo;

namespace MemcardRex.Linux;

public class IconEditor
{
    private readonly Window _window;
    private readonly DrawingArea _canvas;
    private readonly Button _btnOk;
    private readonly Button _btnCancel;
    private readonly ListBox _sidebarListBox;
    private readonly DrawingArea _paletteCanvas;
    private readonly DrawingArea _activeColorsCanvas;

    private enum ToolTypes : int
    {
        Pen,
        Bucket,
        Eraser
    };

    //Icon data
    public byte[]? iconData;
    Color[] iconPalette = new Color[16];

    private Dictionary<int, Gtk.Image> _frameImages = new();

    //Bitmap chequeredBitmap = null;
    //Bitmap chequeredPalette = null;
    int[] selectedColor = new int[2];
    int transparentEntry = -1;
    int selectedIcon = 0;
    int previewIndex = 0;
    int frameCount = 0;

    public bool gridEnabled = true;
    public int gridColorValue = 128;

    //Drawing tools
    //ToolTypes selectedTool = ToolTypes.Pen;

    private uint _timerId;

    public IconEditor(Window parent)
    {
        var builder = new Builder("MemcardRex.Linux.GUI.IconEditor.ui");

        _window = (Window)builder.GetObject("EditorWindow")!;
        _canvas = (DrawingArea)builder.GetObject("Canvas")!;
        _btnOk = (Button)builder.GetObject("BtnOk")!;
        _btnCancel = (Button)builder.GetObject("BtnCancel")!;
        _paletteCanvas = (DrawingArea)builder.GetObject("PaletteCanvas")!;
        _paletteCanvas.SetDrawFunc(OnDrawPalette);
        _sidebarListBox = (ListBox)builder.GetObject("SidebarListBox")!;
        _activeColorsCanvas = (DrawingArea)builder.GetObject("ActiveColorsCanvas")!;
        _activeColorsCanvas.SetDrawFunc(OnDrawActiveColors);

        _window.SetTransientFor(parent);

        _canvas.SetDrawFunc(OnDraw);

        _canvas.SetCursorFromName("crosshair");

        var clickGesture = GestureClick.New();
        clickGesture.SetButton(0);

        _btnCancel.OnClicked += (s, e) => _window.Destroy();
        _btnOk.OnClicked += (s, e) => _window.Destroy();

        clickGesture.OnPressed += (sender, args) => 
        {
            double clickX = args.X;
            double clickY = args.Y;

            int width = _paletteCanvas.GetAllocatedWidth();
            int height = _paletteCanvas.GetAllocatedHeight();

            double cellWidth = (double)width / 8;
            double cellHeight = (double)height / 2;

            int col = (int)(clickX / cellWidth);
            int row = (int)(clickY / cellHeight);

            col = Math.Clamp(col, 0, 7);
            row = Math.Clamp(row, 0, 1);

            int result = (row * 8) + col;
            uint button = clickGesture.GetCurrentButton();

            if(button == 1) setSelectedColor(result, 0);
            else if (button == 3) setSelectedColor(result, 1);
        };
        _paletteCanvas.AddController(clickGesture);

        _sidebarListBox.OnSelectedRowsChanged += (sender, args) => 
        {
            var selectedRow = _sidebarListBox.GetSelectedRow();
            if (selectedRow is not null)
            {
                int index = selectedRow.GetIndex();
                
                selectedIcon = index;
                _canvas.QueueDraw();
            }
        };

        _window.OnUnrealize += (sender, args) => StopAnimTimer();
    }

    //Initialize default values
    public void initDialog(string dialogTitle, int iconFrames, byte[] iconBytes)
    {
        _window.SetTitle(dialogTitle);

        iconData = iconBytes;
        frameCount = iconFrames;

        loadPalette();
        setUpDisplay();

        //Populate icon list with items
        if (iconFrames > 0) _sidebarListBox.Append(CreateFrameRow(0, "1st frame", CreateIconTexture(0)));
        if (iconFrames > 1) _sidebarListBox.Append(CreateFrameRow(1, "2nd frame", CreateIconTexture(1)));
        if (iconFrames > 2) _sidebarListBox.Append(CreateFrameRow(2, "3rd frame", CreateIconTexture(2)));

        //Add preview pane if the icons are animated
        if (iconFrames > 1){
            _sidebarListBox.Append(CreateFrameRow(3, "Preview", CreateIconTexture(previewIndex)));
            StartAnimTimer();
        } 

        /*gridSlider.Value = gridColorValue;
        gridSlider.Enabled = gridEnabled;
        gridCheckbox.Checked = gridEnabled;*/
    }

    public void RefreshSidebarIcon(int index)
    {
        if (_frameImages.TryGetValue(index, out var imgWidget))
        {
           if (previewIndex < frameCount - 1) previewIndex++; else previewIndex = 0;

            imgWidget.SetFromPaintable(CreateIconTexture(previewIndex));

            //Also animate main render if preview is selected
            if (selectedIcon > frameCount - 1) _canvas.QueueDraw();
        }
    }

    private void StartAnimTimer()
    {
        _timerId = GLib.Functions.TimeoutAdd(0, 180, () => 
        {
            //Preview index is 3
            RefreshSidebarIcon(3);
           
            return true; 
        });
    }

    private void StopAnimTimer()
    {
        if (_timerId > 0)
        {
            GLib.Functions.SourceRemove(_timerId);
            _timerId = 0;
        }
    }

    private ListBoxRow CreateFrameRow(int index, string title, Gdk.Texture texture)
    {
        var row = ListBoxRow.New();
        var box = Box.New(Orientation.Horizontal, 10);
        box.SetMarginStart(10);
        box.SetMarginEnd(10);
        box.SetMarginTop(5);
        box.SetMarginBottom(5);

        var image = Image.NewFromPaintable(texture);
        image.SetPixelSize(24);
        
        _frameImages[index] = image;

        box.Append(image);
        box.Append(Label.New(title));
        row.SetChild(box);
        
        return row;
    }

    private void setSelectedColor(int selColor, int selectedColorIndex){
        selectedColor[selectedColorIndex] = selColor;

        _activeColorsCanvas.QueueDraw();
    }

    //Set everything up for drawing
    private void setUpDisplay()
    {
        //Set selected colors to first and second colors in the palete
        setSelectedColor(0, 0);
        setSelectedColor(1, 1);
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
            redChannel = (iconData![byteCount] & 0x1F) << 3;
            greenChannel = ((iconData![byteCount + 1] & 0x3) << 6) | ((iconData![byteCount] & 0xE0) >> 2);
            blueChannel = ((iconData![byteCount + 1] & 0x7C) << 1);
            blackFlag = (iconData![byteCount + 1] & 0x80);

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

    //Display boxes for actively selected colors
    private void OnDrawActiveColors(DrawingArea area, Cairo.Context cr, int width, int height)
    {
        double size = 30;

        cr!.SetSourceRgb((double)iconPalette[selectedColor[1]].R / 255.0,
        (double)iconPalette[selectedColor[1]].G / 255.0,
        (double)iconPalette[selectedColor[1]].B / 255.0);

        cr.Rectangle(width - size - 2, height - size - 2, size, size);
        cr.Fill();
        cr.SetSourceRgb(0, 0, 0);
        cr.LineWidth = 1;
        cr.Rectangle(width - size - 2, height - size - 2, size, size);
        cr.Stroke();

        cr.SetSourceRgb((double)iconPalette[selectedColor[0]].R / 255.0,
        (double)iconPalette[selectedColor[0]].G / 255.0,
        (double)iconPalette[selectedColor[0]].B / 255.0);

        cr.Rectangle(2, 2, size, size);
        cr.Fill();
        cr.SetSourceRgb(1, 1, 1);
        cr.LineWidth = 1.5;
        cr.Rectangle(2, 2, size, size);
        cr.Stroke();
    }

    //Display for palette
    private void OnDrawPalette(DrawingArea area, Cairo.Context cr, int width, int height)
    {
        double cellW = (double)width / 8;
        double cellH = (double)height / 2;

        int index = 0;

        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                cr.SetSourceRgb((double)iconPalette[index].R / 255.0,
                (double)iconPalette[index].G / 255.0,
                (double)iconPalette[index].B / 255.0);

                cr.Rectangle(x * cellW, y * cellH, cellW, cellH);
                cr.Fill();
                
                cr.SetSourceRgb(0.1, 0.1, 0.1);
                cr.Rectangle(x * cellW, y * cellH, cellW, cellH);
                cr.Stroke();

                index++;
            }
        }
    }

    //List item with icon and title
    public void AddFrameToList(Gdk.Texture texture, string frameName)
    {
        var rowBox = Box.New(Orientation.Horizontal, 8);
        rowBox.SetMarginStart(4);
        rowBox.SetMarginEnd(4);
        rowBox.SetMarginTop(4);
        rowBox.SetMarginBottom(4);

        var img = Image.NewFromPaintable(texture);
        img.SetSizeRequest(32, 32);

        var lbl = Label.New(frameName);

        rowBox.Append(img);
        rowBox.Append(lbl);

        var row = ListBoxRow.New();
        row.SetChild(rowBox);

        _sidebarListBox.Append(row);
    }

    //Create icons as a texture from live data
    public Gdk.Texture CreateIconTexture(int frameIndex)
    {
        var bytesRgb = new byte[1024];
        int offset = 32 + (128 * frameIndex);

        for (int i = 0; i < 128; i++)
        {
            //Left pixel
            var color1 = iconPalette![iconData![i + offset] & 0xF];
            bytesRgb[8 * i]     = color1.R;
            bytesRgb[8 * i + 1] = color1.G;
            bytesRgb[8 * i + 2] = color1.B;
            bytesRgb[8 * i + 3] = color1.A;

            //Right pixel
            var color2 = iconPalette![iconData![i + offset] >> 4];
            bytesRgb[8 * i + 4] = color2.R;
            bytesRgb[8 * i + 5] = color2.G;
            bytesRgb[8 * i + 6] = color2.B;
            bytesRgb[8 * i + 7] = color2.A;
        }

        var pixbuf = GdkPixbuf.Pixbuf.NewFromBytes(
            GLib.Bytes.New(bytesRgb), 
            GdkPixbuf.Colorspace.Rgb, 
            true, 
            8, 
            16, 
            16, 
            16 * 4
        );

        return Gdk.Texture.NewForPixbuf(pixbuf);
    }

    private void OnDraw(DrawingArea area, Context cr, int width, int height)
    {
        cr.SetSourceRgb(0, 0, 0);
        cr.Paint();

        double cellW = (double)width / 16;
        double cellH = (double)height / 16;

        int selIcon = selectedIcon;
        if (selectedIcon > frameCount - 1) selIcon = previewIndex;
        int index = 32 + (128 * selIcon);

        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 16; x+=2)
            {
                cr.SetSourceRgb((double)iconPalette[iconData![index] & 0xF].R / 255.0,
                (double)iconPalette[iconData![index] & 0xF].G / 255.0,
                (double)iconPalette[iconData![index] & 0xF].B / 255.0);

                cr.Rectangle(x * cellW, y * cellH, cellW, cellH);
                cr.Fill();

                cr.SetSourceRgb((double)iconPalette[iconData![index] >> 4].R / 255.0,
                (double)iconPalette[iconData![index] >> 4].G / 255.0,
                (double)iconPalette[iconData![index] >> 4].B / 255.0);

                cr.Rectangle((x+1) * cellW, y * cellH, cellW, cellH);
                cr.Fill();

                index++;
            }
        }

        //Grid
        double cellSize = (double)width / 16;
        cr.SetSourceRgb(0.1, 0.45, 0.75); // Boja linija
        cr.LineWidth = 0.5;

        for (int i = 0; i <= 16; i++)
        {
            cr.MoveTo(i * cellSize, 0);
            cr.LineTo(i * cellSize, height);
            cr.MoveTo(0, i * cellSize);
            cr.LineTo(width, i * cellSize);
        }
        cr.Stroke();
    }

    public void Show() => _window.Show();
}
