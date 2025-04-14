using MemcardRex.Properties;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.Versioning;
using Microsoft.Win32;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

namespace MemcardRex
{
    [SupportedOSPlatform("windows")]

    public class CardTabControl : TabControl
    {
        MyColorTable colorTable = new MyColorTable();

        public CardTabControl()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            DrawMode = TabDrawMode.OwnerDrawFixed;
            DoubleBuffered = true;
            BackColor = mainWindow.ActiveColors.backColor;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Prevent default background paint to avoid flicker
            e.Graphics.Clear(mainWindow.ActiveColors.backColor);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(mainWindow.ActiveColors.controlColor);

            SolidBrush borderBrush = new SolidBrush(colorTable.MenuBorder);
            SolidBrush activeBrush = new SolidBrush(colorTable.MenuItemBorder);
            SolidBrush backBrush = new SolidBrush(mainWindow.ActiveColors.controlColor);

            // Draw tab pages background (selected)
            if (SelectedTab != null)
            {
                Rectangle pageRect = SelectedTab.Bounds;
                Rectangle r = ClientRectangle;
                pageRect.Inflate(1, 1);
                e.Graphics.FillRectangle(new SolidBrush(mainWindow.ActiveColors.controlColor), pageRect);

                //Draw outline around the control
                e.Graphics.DrawLine(new Pen(borderBrush), new Point(r.Left, r.Top + 23), new Point(r.Left, r.Bottom));
                e.Graphics.DrawLine(new Pen(borderBrush), new Point(r.Left, r.Bottom - 1), new Point(r.Right, r.Bottom - 1));
                e.Graphics.DrawLine(new Pen(borderBrush), new Point(r.Right - 1, r.Bottom - 1), new Point(r.Right - 1, r.Top + 23));
                e.Graphics.DrawLine(new Pen(borderBrush), new Point(r.Left, r.Top + 23), new Point(r.Right - 1, r.Top + 23));
            }

            // Draw tabs
            for (int i = 0; i < TabCount; i++)
            {
                Rectangle tabRect = GetTabRect(i);
                bool isSelected = (i == SelectedIndex);
                Color fill = isSelected ? Color.FromArgb(60, 60, 60) : Color.FromArgb(40, 40, 40);
                Color textColor = isSelected ? Color.White : Color.Gainsboro;

                e.Graphics.FillRectangle(backBrush, tabRect);

                if (isSelected)
                {
                    tabRect.Inflate(2, 2);

                    e.Graphics.DrawLine(new Pen(borderBrush), new Point(tabRect.Left, tabRect.Top), new Point(tabRect.Right - 1, tabRect.Top));
                    e.Graphics.DrawLine(new Pen(activeBrush), new Point(tabRect.Left, tabRect.Top + 1), new Point(tabRect.Right - 1, tabRect.Top + 1));
                    e.Graphics.DrawLine(new Pen(activeBrush), new Point(tabRect.Left, tabRect.Top + 2), new Point(tabRect.Right - 1, tabRect.Top + 2));
                    e.Graphics.DrawLine(new Pen(activeBrush), new Point(tabRect.Left, tabRect.Top + 3), new Point(tabRect.Right - 1, tabRect.Top + 3));
                    e.Graphics.DrawLine(new Pen(borderBrush), new Point(tabRect.Left, tabRect.Top), new Point(tabRect.Left, tabRect.Bottom));
                    e.Graphics.DrawLine(new Pen(borderBrush), new Point(tabRect.Right - 1, tabRect.Top), new Point(tabRect.Right - 1, tabRect.Bottom));
                }
                else
                {
                    e.Graphics.DrawLine(new Pen(borderBrush), new Point(tabRect.Left, tabRect.Top), new Point(tabRect.Right - 1, tabRect.Top));
                    e.Graphics.DrawLine(new Pen(borderBrush), new Point(tabRect.Left, tabRect.Top), new Point(tabRect.Left, tabRect.Bottom));
                    e.Graphics.DrawLine(new Pen(borderBrush), new Point(tabRect.Right - 1, tabRect.Top), new Point(tabRect.Right - 1, tabRect.Bottom));
                }

                TextRenderer.DrawText(
                e.Graphics,
                TabPages[i].Text,
                new Font("Arial", 8.25f),
                tabRect,
                mainWindow.ActiveColors.foreColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );
            }
        }
    }

    [SupportedOSPlatform("windows")]
    public partial class CardListView : ListView
    {
        public CardListView() : base()
        {
            //Needed to fix the flickering
            DoubleBuffered = true;

            this.OwnerDraw = true;
            this.DrawItem += CardListView_DrawItem;
            this.DrawSubItem += CardListView_DrawSubItem;
            this.DrawColumnHeader += CardListView_DrawColumnHeader;

            this.MouseUp += CardListView_MouseUp;
            this.MouseMove += CardListView_MouseMove;

            this.Invalidated += CardListView_Invalidated;
        }

        private void CardListView_Invalidated(object sender, InvalidateEventArgs e)
        {
            foreach (ListViewItem item in this.Items)
            {
                if (item == null) return;
                item.Tag = null;
            }
        }

        private void CardListView_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewItem item = this.GetItemAt(e.X, e.Y);
            if (item != null && item.Tag == null)
            {
                this.Invalidate(item.Bounds);
                item.Tag = "tagged";
            }
        }

        private void CardListView_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewItem clickedItem = this.GetItemAt(5, e.Y);
            if (clickedItem != null)
            {
                clickedItem.Selected = true;
                clickedItem.Focused = true;
            }
        }

        private void CardListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using (StringFormat sf = new StringFormat())
            {
                // Store the column text alignment, letting it default
                // to Left if it has not been set to Center or Right.
                switch (e.Header.TextAlign)
                {
                    case HorizontalAlignment.Center:
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case HorizontalAlignment.Right:
                        sf.Alignment = StringAlignment.Far;
                        break;
                }

                sf.LineAlignment = StringAlignment.Center;

                // Draw the standard header background.
                //e.DrawBackground();

                // Draw the header text.
                e.Graphics.DrawString(" " +  e.Header.Text, this.Font, new SolidBrush(this.ForeColor), e.Bounds, sf);
            }
            return;
        }

        private void CardListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            using (StringFormat sf = new StringFormat())
            {
                switch (e.Header.TextAlign)
                {
                    case HorizontalAlignment.Center:
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case HorizontalAlignment.Right:
                        sf.Alignment = StringAlignment.Far;
                        break;
                }

                sf.LineAlignment = StringAlignment.Center;
 
                using (SolidBrush brush = new SolidBrush(e.Item.ForeColor))
                {
                    if(e.ColumnIndex !=0) e.Graphics.DrawString(" " + e.SubItem.Text, this.Font, brush, e.Bounds, sf);
                }
            }
        }

        private void CardListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            // Draw the background for an item.
            using (SolidBrush brush = new SolidBrush(e.Item.BackColor))
            {
                StringFormat sf = new StringFormat();
                Rectangle textRectangle = new Rectangle(e.Bounds.Left + this.SmallImageList.ImageSize.Width, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height);

                sf.LineAlignment = StringAlignment.Center;

                e.Graphics.FillRectangle(brush, e.Bounds);
                
                if (this.SmallImageList != null)
                {
                    if(this.SmallImageList.Images.Count > 0)
                    e.Graphics.DrawImage(this.SmallImageList.Images[e.ItemIndex], e.Bounds.Left, e.Bounds.Top, this.SmallImageList.Images[e.ItemIndex].Width, this.SmallImageList.Images[e.ItemIndex].Height);

                    if(e.Item.BackColor != this.BackColor)
                    {
                        Color blendColor = Color.FromArgb(0x55, e.Item.BackColor);
                        e.Graphics.FillRectangle(new SolidBrush(blendColor), e.Bounds.Left, e.Bounds.Top, 48, 16);
                    }

                    e.Graphics.DrawString(e.Item.Text, this.Font, new SolidBrush(e.Item.ForeColor), textRectangle, sf);
                }
                else
                {
                    e.Graphics.DrawString(e.Item.Text, this.Font, new SolidBrush(e.Item.ForeColor), e.Bounds, sf);
                }
            }

            // Draw the item text for views other than the Details view.
            if (this.View != View.Details)
            {
                //e.DrawText();
            }
            }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0007) //WM_SETFOCUS
            {
                //return;
            }
            base.WndProc(ref m);
        }
    }
    [SupportedOSPlatform("windows")]
    public class MyRender : ToolStripProfessionalRenderer
    {
        public MyRender() : base(new MyColorTable()) { }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            var tsMenuItem = e.Item as ToolStripMenuItem;
            if (tsMenuItem != null)
                e.ArrowColor = Color.White;
            base.OnRenderArrow(e);
        }
    }

    public class MyColorTable : ProfessionalColorTable
    {
        private Color menuBorder = Color.FromKnownColor(KnownColor.MenuHighlight);
        private Color menuBackground = Color.FromArgb(28, 28, 28);
        private Color menuHighlight = Color.FromArgb(128, Color.FromKnownColor(KnownColor.MenuHighlight));
        private Color imageMenu = Color.FromArgb(45, 45, 45);
        private Color borderColor = Color.FromArgb(104, 104, 104);
        private Color backColor = Color.FromArgb(32, 32, 32);

        public override Color MenuItemPressedGradientBegin => imageMenu;
        public override Color MenuItemPressedGradientEnd => imageMenu;
        public override Color MenuItemBorder => menuBorder;
        public override Color MenuItemSelected => menuHighlight;
        public override Color MenuItemSelectedGradientBegin => menuHighlight;
        public override Color MenuItemSelectedGradientEnd => menuHighlight;
        public override Color ToolStripDropDownBackground => menuBackground;
        public override Color ImageMarginGradientBegin => imageMenu;
        public override Color ImageMarginGradientMiddle => imageMenu;
        public override Color ImageMarginGradientEnd => imageMenu;
        public override Color SeparatorLight => borderColor;
        public override Color SeparatorDark => borderColor;
        public override Color MenuBorder => borderColor;
        public override Color GripDark => borderColor;
        public override Color GripLight => backColor;
        public override Color ToolStripGradientMiddle => backColor;
        public override Color ToolStripBorder => backColor;
        public override Color OverflowButtonGradientEnd => backColor;
        public override Color OverflowButtonGradientMiddle => backColor;
        public override Color OverflowButtonGradientBegin => backColor;
        public override Color ToolStripGradientBegin => backColor;
        public override Color ToolStripGradientEnd => backColor;
    }

    partial class mainWindow
    {
        [DllImport("DwmApi")] //System.Runtime.InteropServices
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        private static bool darkThemeState = false;
        private static bool registryChecked = false;

        protected override void OnHandleCreated(EventArgs e)
        {
            if (!IsDarkModeEnabled()) return;
            if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
                DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
        }

        public struct ThemeColors
        {
            public Color backColor;
            public Color foreColor;
            public Color menuBorder;
            public Color menuHighlight;
            public Color controlColor;
        }

        private static ThemeColors darkColors = new ThemeColors
        {
            backColor = Color.FromArgb(32, 32, 32),
            foreColor = Color.FromArgb(0xFF, 0xFF, 0xFF),
            menuBorder = Color.FromKnownColor(KnownColor.MenuHighlight),
            menuHighlight = Color.FromArgb(128, Color.FromKnownColor(KnownColor.MenuHighlight)),
            controlColor = Color.FromArgb(32, 32, 32)
        };

        private static ThemeColors lightColors = new ThemeColors
        {
            backColor = Color.FromKnownColor(KnownColor.Window),
            foreColor = Color.FromKnownColor(KnownColor.WindowText),
            menuBorder = Color.FromKnownColor(KnownColor.MenuHighlight),
            menuHighlight = Color.FromArgb(128, Color.FromKnownColor(KnownColor.MenuHighlight)),
            controlColor = Color.FromKnownColor(KnownColor.Control)
        };

        public static ThemeColors ActiveColors
        {
            get { if (darkThemeState) return darkColors; else return lightColors; }
        }

        public void ChangeTheme(Control.ControlCollection container)
        {
            foreach (Control component in container)
            {
                component.BackColor = darkColors.backColor;
                component.ForeColor = darkColors.foreColor;

                if (component is Panel)
                {
                    ChangeTheme(component.Controls);
                    component.BackColor = darkColors.backColor;
                    component.ForeColor = darkColors.foreColor;
                }
                else if (component is Button)
                {
                    component.BackColor = darkColors.backColor;
                    component.ForeColor = darkColors.foreColor;
                }
                else if (component is TextBox)
                {
                    component.BackColor = darkColors.backColor;
                    component.ForeColor = darkColors.foreColor;
                }
            }
        }

        static bool IsDarkModeEnabled()
        {
            if (registryChecked) return darkThemeState;

            // The registry key for dark mode setting
            string key = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            string valueName = "AppsUseLightTheme"; // 0 = dark, 1 = light

            // Check if the value exists and retrieve it
            object value = Registry.GetValue(key, valueName, null);

            //We checked the registry once
            registryChecked = true;

            // Default to light mode if the value is not set or invalid
            darkThemeState = false;

            if (value != null && value is int)
            {
                // If the value is 0, dark mode is enabled, otherwise light mode is enabled
                darkThemeState = (int)value == 0;
            }

            return darkThemeState;
        }

        private void ApplyTheme()
        {
            //Check if dark theme needs to be set
            if (!IsDarkModeEnabled()) return;

            this.BackColor = darkColors.backColor;

            mainMenu.Renderer  = new MyRender();
            mainMenu.ForeColor = darkColors.foreColor;
            mainMenu.BackColor = Color.FromArgb(32, 32, 32);

            //Color for each of the menu items
            foreach (ToolStripMenuItem mainItem in mainMenu.Items)
            {
                foreach (ToolStripItem toolItem in mainItem.DropDownItems)
                {
                    toolItem.ForeColor = darkColors.foreColor;
                }
            }

            //Color PocketStation menus
            foreach (ToolStripItem toolItem in pocketStationToolStripMenuItem.DropDownItems)
            {
                toolItem.ForeColor = darkColors.foreColor;
            }

            mainToolbar.Renderer = new MyRender();
            mainToolbar.ForeColor = darkColors.foreColor;
            mainToolbar.BackColor = Color.FromArgb(32, 32, 32);

            mainContextMenu.Renderer = new MyRender();
            mainContextMenu.ForeColor = darkColors.foreColor;
            mainContextMenu.BackColor = Color.FromArgb(32, 32, 32);

            mainStatusStrip.BackColor = darkColors.backColor;
            mainStatusStrip.ForeColor = darkColors.foreColor;
        }

        //Create toolbar and menu icons with proper DPI scale and accent color
        private void BuildToolbarIcons()
        {
            double scaleWidth = 20 * xScale;
            double scaleHeight = 20 * yScale;
            double menuScaleWidth = 20 * xScale;
            double menuScaleHeight = 20 * yScale;

            mainToolbar.ImageScalingSize = new Size((int)scaleWidth, (int)scaleHeight);

            mainMenu.ImageScalingSize = new Size((int)scaleWidth, (int)scaleHeight);
            mainContextMenu.ImageScalingSize = new Size((int)scaleWidth, (int)scaleHeight);

            newButton.Image = BuildToolbarIcon(Resources.newcard, (int)scaleWidth, (int)scaleHeight);
            newToolStripMenuItem.Image = BuildToolbarIcon(Resources.newcard, (int)menuScaleWidth, (int)menuScaleHeight);

            openButton.Image = BuildToolbarIcon(Resources.opencard, (int)scaleWidth, (int)scaleHeight);
            openToolStripMenuItem.Image = BuildToolbarIcon(Resources.opencard, (int)menuScaleWidth, (int)menuScaleHeight);

            saveButton.Image = BuildToolbarIcon(Resources.savecard, (int)scaleWidth, (int)scaleHeight);
            saveToolStripMenuItem.Image = BuildToolbarIcon(Resources.savecard, (int)menuScaleWidth, (int)menuScaleHeight);

            editHeaderButton.Image = BuildToolbarIcon(Resources.headeredit, (int)scaleWidth, (int)scaleHeight);
            editSaveHeaderToolStripMenuItem.Image = BuildToolbarIcon(Resources.headeredit, (int)menuScaleWidth, (int)menuScaleHeight);
            editSaveHeaderToolStripMenuItem1.Image = BuildToolbarIcon(Resources.headeredit, (int)menuScaleWidth, (int)menuScaleHeight);

            commentsButton.Image = BuildToolbarIcon(Resources.comments, (int)scaleWidth, (int)scaleHeight);
            editSaveCommentToolStripMenuItem.Image = BuildToolbarIcon(Resources.comments, (int)menuScaleWidth, (int)menuScaleHeight);
            editSaveCommentsToolStripMenuItem.Image = BuildToolbarIcon(Resources.comments, (int)menuScaleWidth, (int)menuScaleHeight);

            editIconButton.Image = BuildToolbarIcon(Resources.iconedit, (int)scaleWidth, (int)scaleHeight);
            editIconToolStripMenuItem.Image = BuildToolbarIcon(Resources.iconedit, (int)menuScaleWidth, (int)menuScaleHeight);
            editIconToolStripMenuItem1.Image = BuildToolbarIcon(Resources.iconedit, (int)menuScaleWidth, (int)menuScaleHeight);

            importButton.Image = BuildToolbarIcon(Resources.importsave, (int)scaleWidth, (int)scaleHeight);
            importSaveToolStripMenuItem.Image = BuildToolbarIcon(Resources.importsave, (int)menuScaleWidth, (int)menuScaleHeight);
            importSaveToolStripMenuItem1.Image = BuildToolbarIcon(Resources.importsave, (int)menuScaleWidth, (int)menuScaleHeight);

            exportButton.Image = BuildToolbarIcon(Resources.exportsave, (int)scaleWidth, (int)scaleHeight);
            exportSaveToolStripMenuItem.Image = BuildToolbarIcon(Resources.exportsave, (int)menuScaleWidth, (int)menuScaleHeight);
            exportRAWSaveToolStripMenuItem.Image = BuildToolbarIcon(Resources.exportsave, (int)menuScaleWidth, (int)menuScaleHeight);
            exportSaveToolStripMenuItem1.Image = BuildToolbarIcon(Resources.exportsave, (int)menuScaleWidth, (int)menuScaleHeight);
            exportRAWSaveToolStripMenuItem1.Image = BuildToolbarIcon(Resources.exportsave, (int)menuScaleWidth, (int)menuScaleHeight);

            closeToolStripMenuItem.Image = BuildToolbarIcon(Resources.closecard, (int)menuScaleWidth, (int)menuScaleHeight);
            exitToolStripMenuItem.Image = BuildToolbarIcon(Resources.quiticon, (int)menuScaleWidth, (int)menuScaleHeight);

            editWithPluginToolStripMenuItem.Image = BuildToolbarIcon(Resources.plugin, (int)menuScaleWidth, (int)menuScaleHeight);
            editWithPluginToolStripMenuItem1.Image = BuildToolbarIcon(Resources.plugin, (int)menuScaleWidth, (int)menuScaleHeight);
            managePluginsToolStripMenuItem.Image = BuildToolbarIcon(Resources.plugin, (int)menuScaleWidth, (int)menuScaleHeight);

            compareWithTempBufferToolStripMenuItem.Image = BuildToolbarIcon(Resources.comparetemp, (int)menuScaleWidth, (int)menuScaleHeight);
            compareWithTempBufferToolStripMenuItem1.Image = BuildToolbarIcon(Resources.comparetemp, (int)menuScaleWidth, (int)menuScaleHeight);

            preferencesToolStripMenuItem.Image = BuildToolbarIcon(Resources.options, (int)menuScaleWidth, (int)menuScaleHeight);

            readMeToolStripMenuItem.Image = BuildToolbarIcon(Resources.readicon, (int)menuScaleWidth, (int)menuScaleHeight);
            aboutToolStripMenuItem.Image = BuildToolbarIcon(Resources.infoicon, (int)menuScaleWidth, (int)menuScaleHeight);

            saveInformationToolStripMenuItem.Image = BuildToolbarIcon(Resources.infoicon, (int)menuScaleWidth, (int)menuScaleHeight);
        }

        private Bitmap ColorizeIcon(Bitmap icon, Color color)
        {
            Bitmap newIcon = new Bitmap(icon.Width, icon.Height);

            for(int y =0; y < icon.Height; y++)
            {
                for (int x = 0; x < icon.Width; x++)
                {
                    if(icon.GetPixel(x,y) != Color.FromArgb(0,0,0,0)) newIcon.SetPixel(x, y, color);
                }
            }

            return newIcon;
        }

        private Bitmap BuildToolbarIcon(Bitmap resourceIcon, int outWidth, int outHeight)
        {
            double imageWidth = 16 * xScale;
            double imageHeight = 16 * yScale;

            Bitmap toolbarIcon = new Bitmap(outWidth, outHeight);
            Graphics iconGraphics = Graphics.FromImage(toolbarIcon);

            iconGraphics.PixelOffsetMode = PixelOffsetMode.Half;
            iconGraphics.InterpolationMode = InterpolationMode.Bilinear;

            iconGraphics.FillRegion(new SolidBrush(Color.FromArgb(180, Color.FromKnownColor(KnownColor.MenuHighlight))), new Region(new Rectangle(0, 0, outHeight, outWidth)));

            //Draw icon on top
            iconGraphics.DrawImage(ColorizeIcon(resourceIcon, Color.White), (outWidth - (int)imageWidth) / 2,
                (outHeight - (int)imageHeight) / 2, (int)imageWidth, (int)imageHeight);

            iconGraphics.Dispose();

            return toolbarIcon;
        }
    }
}
