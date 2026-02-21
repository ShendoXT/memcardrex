using System;
using System.Drawing;
using Gtk;
using Cairo;
using GdkPixbuf;

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
    private readonly Adjustment _gridAdjustment;
    private readonly CheckButton _checkActive;
    private readonly Scale _speedSlider;

    //Edit buttons
    private readonly Button _btnFlipH;
    private readonly Button _btnFlipV;
    private readonly Button _btnRotateL;
    private readonly Button _btnRotateR;

    //Tool buttons
    private readonly Button _btnPencil;
    private readonly Button _btnFill;
    private readonly Button _btnEraser;

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

    int[] selectedColor = new int[2];
    int transparentEntry = -1;
    int selectedIcon = 0;
    int previewIndex = 0;
    int frameCount = 0;

    public bool gridEnabled = true;
    public int gridColorValue = 128;

    //Drawing tools
    ToolTypes selectedTool = ToolTypes.Pen;

    private Gtk.ColorDialog _colorDialog = Gtk.ColorDialog.New();
    private uint _timerId;
    private enum TransformType { HorizontalFlip, VerticalFlip, RotateLeft, RotateRight }

    public event EventHandler? OnDialogClosed;
    public bool OkResponse = false;

    public IconEditor(Window parent)
    {
        var builder = new Builder("MemcardRex.Linux.GUI.IconEditor.ui");

        _window = (Window)builder.GetObject("EditorWindow")!;
        _canvas = (DrawingArea)builder.GetObject("Canvas")!;
        _btnOk = (Button)builder.GetObject("BtnOk")!;
        _btnCancel = (Button)builder.GetObject("BtnCancel")!;

        _btnFlipH = (Button)builder.GetObject("BtnFlipH")!;
        _btnFlipV = (Button)builder.GetObject("BtnFlipV")!;
        _btnRotateL = (Button)builder.GetObject("BtnRotateL")!;
        _btnRotateR = (Button)builder.GetObject("BtnRotateR")!;

        _btnPencil = (Button)builder.GetObject("BtnPencil")!;
        _btnFill = (Button)builder.GetObject("BtnFill")!;
        _btnEraser = (Button)builder.GetObject("BtnEraser")!;

        _paletteCanvas = (DrawingArea)builder.GetObject("PaletteCanvas")!;
        _paletteCanvas.SetDrawFunc(OnDrawPalette);
        _sidebarListBox = (ListBox)builder.GetObject("SidebarListBox")!;
        _activeColorsCanvas = (DrawingArea)builder.GetObject("ActiveColorsCanvas")!;
        _activeColorsCanvas.SetDrawFunc(OnDrawActiveColors);

        _gridAdjustment = (Adjustment)builder.GetObject("GridAdjustment")!;
        _checkActive = (CheckButton)builder.GetObject("CheckActive")!;
        _speedSlider = (Scale)builder.GetObject("SpeedSlider")!;

        _window.SetTransientFor(parent);
        _canvas.SetDrawFunc(OnDraw);
        _canvas.SetCursorFromName("crosshair");

        var menuModel = (Gio.Menu)builder.GetObject("icon_context_menu")!;
        var popover = Gtk.PopoverMenu.NewFromModel(menuModel);
        popover.SetParent(_sidebarListBox);
        popover.HasArrow = false;

        var actionGroup = Gio.SimpleActionGroup.New();
        var exportAction = Gio.SimpleAction.New("export", null);

        exportAction.OnActivate += (sender, args) => {
           ExportSelectedIcon();
        };

        actionGroup.AddAction(exportAction);
        _sidebarListBox.InsertActionGroup("list", actionGroup);

        var listClickGesture = Gtk.GestureClick.New();
        listClickGesture.SetButton(3);

        listClickGesture.OnPressed += (sender, args) => {
            //Do not show on preview
            if (selectedIcon > frameCount - 1) return;
            var rect = new Gdk.Rectangle {
                X = (int)args.X,
                Y = (int)args.Y,
                Width = 0,
                Height = 0
            };
            
            popover.SetPointingTo(rect);
            popover.Popup();
        };

        _sidebarListBox.AddController(listClickGesture);

        var dragGesture = GestureDrag.New();
        dragGesture.SetButton(0);

        var clickGesture = GestureClick.New();
        clickGesture.SetButton(0);

        _btnCancel.OnClicked += (s, e) => CloseDialog(false);
        _btnOk.OnClicked += (s, e) => CloseDialog(true);

        _btnFlipH.OnClicked += (s, e) => transformIcon(TransformType.HorizontalFlip);
        _btnFlipV.OnClicked += (s, e) => transformIcon(TransformType.VerticalFlip);
        _btnRotateL.OnClicked += (s, e) => transformIcon(TransformType.RotateLeft);
        _btnRotateR.OnClicked += (s, e) => transformIcon(TransformType.RotateRight);

        _btnPencil.OnClicked += (s, e) => SetSelectedTool(ToolTypes.Pen);
        _btnFill.OnClicked += (s, e) => SetSelectedTool(ToolTypes.Bucket);
        _btnEraser.OnClicked += (s, e) => SetSelectedTool(ToolTypes.Eraser);

        //Canvas mouse click
        dragGesture.OnDragBegin += (sender, args) => 
        {
            HandleInput(sender, args.StartX, args.StartY);
        };

        //Canvas mouse drag
        dragGesture.OnDragUpdate += (sender, args) => 
        {
            sender.GetStartPoint(out double startX, out double startY);
            
            double currentX = startX + args.OffsetX;
            double currentY = startY + args.OffsetY;

            HandleInput(sender, currentX, currentY);
        };

        _canvas.AddController(dragGesture);

        clickGesture.OnPressed += (sender, args) => PaletteClicked(sender, args);
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

        //Grid slider
        _gridAdjustment.OnValueChanged += (sender, args) => 
        {
            double value = sender.Value;
            gridColorValue = (int) value;
            _canvas.QueueDraw();
        };

        //Grid checkbox
        _checkActive.OnToggled += (sender, args) => 
        {
            gridEnabled = sender.Active;
            _speedSlider.Sensitive = sender.Active;
            _canvas.QueueDraw();
        };

        _window.OnUnrealize += (sender, args) => StopAnimTimer();
    }

    private void CloseDialog(bool dlgMsge){
        OkResponse = dlgMsge;
        OnDialogClosed?.Invoke(this, EventArgs.Empty);
        _window.Destroy();
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

        //Set Pen as a default tool
        SetSelectedTool(ToolTypes.Pen);

        _gridAdjustment.Value = gridColorValue;
        _checkActive.Active = gridEnabled;
        _speedSlider.Sensitive = gridEnabled;
    }

    private void ExportSelectedIcon(){
        var saver = Gtk.FileChooserNative.New(
            "Save icon",
            _window,
            Gtk.FileChooserAction.Save,
            "Save",
            "Cancel"
        );

        saver.SetCurrentName("icon.png");

        saver.OnResponse += (sender, args) =>
        {
            if (args.ResponseId == (int)Gtk.ResponseType.Accept)
            {
                var file = saver.GetFile();
                string path = file!.GetPath() ?? "icon.png";

                if (!path.EndsWith(".png")) path += ".png";

                CreateIconTexture(selectedIcon).SaveToPng(path);
            }
            saver.Destroy();
        };

        saver.Show();
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
            iconData![targetIndex] = (byte)((iconData[targetIndex] & 0xF0) | colorIndex);
        }
        else
        {
            iconData![targetIndex] = (byte)((iconData[targetIndex] & 0x0F) | (colorIndex << 4));
        }

        _canvas.QueueDraw();
        RefreshIconPrev(selectedIcon);
    }

    //Get color index of a single pixel
    private int getPixel(int X, int Y)
    {
        int offset = 32 + (selectedIcon * 128);
        int pixelIndex = X + (Y * 16);
        int destinationByte = pixelIndex / 2;
        int targetIndex = offset + destinationByte;

        byte data = iconData![targetIndex];

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

    //Handle mouse input on the canvas
    private void HandleInput(Gtk.GestureDrag sender, double rawX, double rawY)
    {
        uint button = sender.GetCurrentButton();
        if (button != 1 && button != 3) return;

        //Disable editing if preview is selected
        if (selectedIcon > frameCount - 1) return;

        var canvas = (Gtk.DrawingArea)sender.GetWidget()!;
        
        int x = (int)(rawX / (canvas.GetAllocatedWidth() / 16.0));
        int y = (int)(rawY / (canvas.GetAllocatedHeight() / 16.0));

        int paletteIndex = (button == 1) ? 0 : 1;

        if (x >= 0 && x < 16 && y >= 0 && y < 16){
           switch (selectedTool)
            {
                case ToolTypes.Pen:
                    putPixel(x, y, selectedColor[paletteIndex]);
                    break;

                case ToolTypes.Bucket:
                    FloodFill(x, y, selectedColor[paletteIndex]);
                    break;

                case ToolTypes.Eraser:
                    //Erase only if transparent entry is available
                    if (transparentEntry >= 0) putPixel(x, y, transparentEntry);
                    break;
            }
        }
    }

    public static Gdk.RGBA ToGdkRgba(System.Drawing.Color color)
    {
        var rgba = new Gdk.RGBA();
        rgba.Red = (float)color.R / 255f;
        rgba.Green = (float)color.G / 255f;
        rgba.Blue = (float)color.B / 255f;
        rgba.Alpha = (float)color.A / 255f;
        return rgba;
    }

    public static System.Drawing.Color ToDrawingColor(Gdk.RGBA rgba)
    {
        return System.Drawing.Color.FromArgb(
            (int)(rgba.Alpha * 255),
            (int)(rgba.Red * 255),
            (int)(rgba.Green * 255),
            (int)(rgba.Blue * 255)
        );
    }

    private void OpenColorPicker(int paletteIndex)
    {
        var dialog = Gtk.ColorChooserDialog.New("Color", _window);
        
        dialog.Rgba = ToGdkRgba(iconPalette[paletteIndex]);

        dialog.OnResponse += (sender, args) => 
        {
            if (args.ResponseId == (int)Gtk.ResponseType.Ok)
            {                
                System.Drawing.Color dialogColor = ToDrawingColor(dialog.Rgba);

                //Get each color channel
                bool alphaFlag = (dialogColor.A == 0);
                int redChannel   = alphaFlag ? 0 : (dialogColor.R >> 3);
                int greenChannel = alphaFlag ? 0 : (dialogColor.G >> 3);
                int blueChannel  = alphaFlag ? 0 : (dialogColor.B >> 3);

                //Set color to iconData (convert 24 bit color to 15 bit)
                iconData![selectedColor[0] * 2] = (byte)(redChannel | ((greenChannel & 0x07) << 5));
                iconData![(selectedColor[0] * 2) + 1] = (byte)((blueChannel << 2) | ((greenChannel & 0x18) >> 3));

                //Remove transparency if there is no alpha
                if(!alphaFlag) iconData[(selectedColor[0] * 2) + 1] |= 0x80;

                //If this current entry was transparent disable it if it's not now
                if (!alphaFlag && transparentEntry == selectedColor[0]) transparentEntry = -1;

                loadPalette();
                _canvas.QueueDraw();
                _paletteCanvas.QueueDraw();

                //Refresh preview icons
                for (int i=0; i < frameCount; i++) RefreshIconPrev(i);
            }
            dialog.Destroy();
        };

        dialog.Show();
    }

    //User clicked on palette
    private void PaletteClicked(GestureClick clickGesture, GestureClick.PressedSignalArgs args)
    {
        double cellWidth = (double)_paletteCanvas.GetAllocatedWidth() / 8;
        double cellHeight = (double)_paletteCanvas.GetAllocatedHeight() / 2;

        int col = Math.Clamp((int)(args.X / cellWidth), 0, 7);
        int row = Math.Clamp((int)(args.Y / cellHeight), 0, 1);
        int result = (row * 8) + col;

        uint button = clickGesture.GetCurrentButton();

        if (args.NPress == 2 && button == 1)
        {
            OpenColorPicker(result);
            return;
        }

        if (button == 1) setSelectedColor(result, 0);
        else if (button == 3) setSelectedColor(result, 1);
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
                returnData[x, y] = (byte)(iconData![byteCount] & 0x0F);
                returnData[x + 1, y] = (byte)((iconData![byteCount] & 0xF0) >> 4);
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
                iconData![byteCount] = (byte)(gridData[x, y] | (gridData[x + 1, y] << 4));
                byteCount++;
            }
        }
    }

    //Set active tool
    private void SetSelectedTool(ToolTypes tool){
        selectedTool = tool;
        _btnPencil.GetStyleContext().RemoveClass("suggested-action");
        _btnFill.GetStyleContext().RemoveClass("suggested-action");
        _btnEraser.GetStyleContext().RemoveClass("suggested-action");

        switch(tool){
            case ToolTypes.Pen:
                _btnPencil.GetStyleContext().AddClass("suggested-action");
                break;

            case ToolTypes.Bucket:
                _btnFill.GetStyleContext().AddClass("suggested-action");
                break;

            case ToolTypes.Eraser:
                _btnEraser.GetStyleContext().AddClass("suggested-action");
                ActivateEraser();
                break;
        }
    }

    private void ActivateEraser(){
        if (transparentEntry < 0)
        {
            var dialog = new Adw.MessageDialog
            {
                Modal = true,
                Heading = "Transparent entry required",
                Body = "Eraser tool requires transparent entry in the palette.\nDo you want to change currently selected color to transparent?",
                TransientFor = _window
            };
            dialog.AddResponse("yes", "Yes");
            dialog.AddResponse("no", "No");
            dialog.SetResponseAppearance("yes", Adw.ResponseAppearance.Suggested);
            dialog.Show();
            dialog.OnResponse += (_, dialogArgs) => {
                if (dialogArgs.Response == "yes")
                {
                    iconData![selectedColor[0] * 2] = 0;
                    iconData[(selectedColor[0] * 2) + 1] = 0;

                    loadPalette();
                    _paletteCanvas.QueueDraw();
                    setSelectedColor(selectedColor[0], 0);
                    setSelectedColor(selectedColor[1], 1);
                    _canvas.QueueDraw();
                    for (int i=0; i < frameCount; i++) RefreshIconPrev(i);
                }
                dialog.Destroy();
            };
        }
    }

    //Flip or rotate the active icon
    private void transformIcon(TransformType type)
    {
        //Disable editing if preview is selected
        if (selectedIcon > frameCount - 1) return;

        byte[,] data = getDataGrid();
        byte[,] result = new byte[16, 16];

        switch (type)
        {
            case TransformType.HorizontalFlip:
                for (int y = 0; y < 16; y++)
                    for (int x = 0; x < 8; x++) {
                        byte temp = data[x, y];
                        data[x, y] = data[15 - x, y];
                        data[15 - x, y] = temp;
                    }
                result = data;
                break;

            case TransformType.VerticalFlip:
                for (int x = 0; x < 16; x++)
                    for (int y = 0; y < 8; y++) {
                        byte temp = data[x, y];
                        data[x, y] = data[x, 15 - y];
                        data[x, 15 - y] = temp;
                    }
                result = data;
                break;

            case TransformType.RotateRight:
                for (int y = 0; y < 16; y++)
                    for (int x = 0; x < 16; x++)
                        result[15 - y, x] = data[x, y];
                break;

            case TransformType.RotateLeft:
                for (int y = 0; y < 16; y++)
                    for (int x = 0; x < 16; x++)
                        result[y, 15 - x] = data[x, y];
                break;
        }

        setDataGrid(result);
        _canvas.QueueDraw();
        RefreshIconPrev(selectedIcon);
    }

    //Refresh for regular icon preview
    private void RefreshIconPrev(int index){
        if (_frameImages.TryGetValue(index, out var imgWidget))
        {
            imgWidget.SetFromPaintable(CreateIconTexture(index));
        }
    }

    //Refresh for animated preview icon
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

        // Second color (bottom-right)
        DrawColorWithChecker(cr, width - size - 2, height - size - 2, size, iconPalette[selectedColor[1]]);
        cr.SetSourceRgb(0, 0, 0);
        cr.LineWidth = 1;
        cr.Rectangle(width - size - 2, height - size - 2, size, size);
        cr.Stroke();

        // First color (top-left)
        DrawColorWithChecker(cr, 2, 2, size, iconPalette[selectedColor[0]]);
        cr.SetSourceRgb(1, 1, 1);
        cr.LineWidth = 1.5;
        cr.Rectangle(2, 2, size, size);
        cr.Stroke();
    }

    private void DrawColorWithChecker(Cairo.Context cr, double x, double y, double size, System.Drawing.Color color)
    {
        double sub = size / 2;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                if ((i + j) % 2 == 0) cr.SetSourceRgb(0.9, 0.9, 0.9);
                else cr.SetSourceRgb(0.7, 0.7, 0.7);
                cr.Rectangle(x + (i * sub), y + (j * sub), sub, sub);
                cr.Fill();
            }
        }

        if (color.A > 0)
        {
            cr.SetSourceRgb(color.R / 255.0, color.G / 255.0, color.B / 255.0);
            cr.Rectangle(x, y, size, size);
            cr.Fill();
        }
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
                double posX = x * cellW;
                double posY = y * cellH;

                //Checkerboard
                if (iconPalette[index].A == 0)
                {
                    double subW = cellW / 2;
                    double subH = cellH / 2;

                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            if ((i + j) % 2 == 0) cr.SetSourceRgb(0.9, 0.9, 0.9);
                            else cr.SetSourceRgb(0.7, 0.7, 0.7);

                            cr.Rectangle(posX + (i * subW), posY + (j * subH), subW, subH);
                            cr.Fill();
                        }
                    }
                }
                else
                {
                    cr.SetSourceRgb(
                        iconPalette[index].R / 255.0,
                        iconPalette[index].G / 255.0,
                        iconPalette[index].B / 255.0
                    );
                    cr.Rectangle(posX, posY, cellW, cellH);
                    cr.Fill();
                }

                cr.SetSourceRgb(0.1, 0.1, 0.1);
                cr.LineWidth = 0.7;
                cr.Rectangle(posX, posY, cellW, cellH);
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

    //Main canvas drawing
    private void OnDraw(DrawingArea area, Context cr, int width, int height)
    {
        cr.SetSourceRgb(0, 0, 0);
        cr.Paint();

        double cellW = (double)width / 16;
        double cellH = (double)height / 16;
        double subW = cellW / 2;
        double subH = cellH / 2;

        int selIcon = selectedIcon >= frameCount ? previewIndex : selectedIcon;
        int index = 32 + (128 * selIcon);

        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 16; x += 2)
            {
                int[] pixelColors = { iconData![index] & 0xF, iconData![index] >> 4 };

                for (int i = 0; i < 2; i++)
                {
                    double posX = (x + i) * cellW;
                    double posY = y * cellH;
                    var color = iconPalette[pixelColors[i]];

                    //Checkerboard
                    for (int subY = 0; subY < 2; subY++)
                    {
                        for (int subX = 0; subX < 2; subX++)
                        {
                            if ((subX + subY) % 2 == 0) cr.SetSourceRgb(0.9, 0.9, 0.9);
                            else cr.SetSourceRgb(0.7, 0.7, 0.7);

                            cr.Rectangle(posX + (subX * subW), posY + (subY * subH), subW, subH);
                            cr.Fill();
                        }
                    }

                    if (color.A > 0)
                    {
                        cr.SetSourceRgb(color.R / 255.0, color.G / 255.0, color.B / 255.0);
                        cr.Rectangle(posX, posY, cellW, cellH);
                        cr.Fill();
                    }
                }
                index++;
            }
        }

        //Do now draw grid if it's disabled
        if(!gridEnabled) return;

        //Grid
        cr.SetSourceRgb(gridColorValue / 255.0, gridColorValue / 255.0, gridColorValue / 255.0);
        cr.LineWidth = 0.7;
        for (int i = 0; i <= 16; i++)
        {
            cr.MoveTo(i * cellW, 0);
            cr.LineTo(i * cellW, height);
            cr.MoveTo(0, i * cellH);
            cr.LineTo(width, i * cellH);
        }
        cr.Stroke();
    }

    public void Show() => _window.Show();
}
