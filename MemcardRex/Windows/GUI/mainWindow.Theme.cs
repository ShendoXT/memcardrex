using MemcardRex.Properties;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MemcardRex.mainWindow;
using System.Runtime.InteropServices;
using System.Globalization;
using System.ComponentModel;

namespace MemcardRex
{
    public partial class CardTabControl : TabControl
    {
        private struct TabItemInfo
        {
            public Color BackColor;
            public Rectangle Bounds;
            public Font Font;
            public Color ForeColor;
            public int Index;
            public DrawItemState State;

            public TabItemInfo(DrawItemEventArgs e)
            {
                this.BackColor = e.BackColor;
                this.ForeColor = e.ForeColor;
                this.Bounds = e.Bounds;
                this.Font = e.Font;
                this.Index = e.Index;
                this.State = e.State;
            }
        }

        private Dictionary<int, TabItemInfo> _tabItemStateMap = new Dictionary<int, TabItemInfo>();
        public CardTabControl() : base()
        {
            base.DrawMode = TabDrawMode.OwnerDrawFixed;
        }

        //Needed to clear the map of the existing tabs
        public void RemoveTabPrepare()
        {
            _tabItemStateMap.Clear();
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen p = new Pen(Color.Blue);
            Font font = new Font("Arial", 8.25f);
            SolidBrush brush = new SolidBrush(Color.Red);

            //g.DrawRectangle(p, this.GetTabRect(e.Index));
            //g.DrawString(this.TabPages[e.Index].Text, font, brush, (RectangleF)this.GetTabRect(e.Index));

            //Console.WriteLine(this.TabPages[e.Index].Text);

            //base.OnDrawItem(e);

            //return;

            base.OnDrawItem(e);
            if (!_tabItemStateMap.ContainsKey(e.Index))
            {
                _tabItemStateMap.Add(e.Index, new TabItemInfo(e));
            }
            else
            {
                _tabItemStateMap[e.Index] = new TabItemInfo(e);
            }
        }

        private const int WM_PAINT = 0x000F;
        private const int WM_ERASEBKGND = 0x0014;

        // Cache context to avoid repeatedly re-creating the object.
        // WM_PAINT is called frequently so it's better to declare it as a member.
        private BufferedGraphicsContext _bufferContext = BufferedGraphicsManager.Current;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_PAINT:
                    {
                        // Let system do its thing first.
                        base.WndProc(ref m);

                        // Custom paint Tab items.
                        HandlePaint(ref m);

                        break;
                    }
                case WM_ERASEBKGND:
                    {
                        if (DesignMode)
                        {
                            // Ignore to prevent flickering in DesignMode.
                        }
                        else
                        {
                            base.WndProc(ref m);
                        }
                        break;
                    }
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private Color _backColor = new MyColorTable().ToolStripBorder;
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public new Color BackColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                _backColor = value;
            }
        }


        private void HandlePaint(ref Message m)
        {
            using (var g = Graphics.FromHwnd(m.HWnd))
            {
                MyColorTable colorTable = new MyColorTable();

                SolidBrush backBrush = new SolidBrush(BackColor);
                SolidBrush borderBrush = new SolidBrush(colorTable.MenuBorder);

                Rectangle r = ClientRectangle;
                using (var buffer = _bufferContext.Allocate(g, r))
                {
                    buffer.Graphics.FillRectangle(backBrush, r);

                    //Draw outlines
                    buffer.Graphics.DrawLine(new Pen(borderBrush), new Point(r.Left, r.Top + 23), new Point(r.Left, r.Bottom));
                    buffer.Graphics.DrawLine(new Pen(borderBrush), new Point(r.Left, r.Bottom - 1), new Point(r.Right, r.Bottom - 1));
                    buffer.Graphics.DrawLine(new Pen(borderBrush), new Point(r.Right - 1, r.Bottom - 1), new Point(r.Right - 1, r.Top + 23));
                    buffer.Graphics.DrawLine(new Pen(borderBrush), new Point(r.Left, r.Top + 23), new Point(r.Right - 1, r.Top + 23));

                    // Paint items
                    foreach (int index in _tabItemStateMap.Keys)
                    {
                        DrawTabItemInternal(buffer.Graphics, _tabItemStateMap[index]);
                    }

                    buffer.Render();
                }
                backBrush.Dispose();
            }
        }


        private void DrawTabItemInternal(Graphics gr, TabItemInfo tabInfo)
        {
            if (_tabItemStateMap.Count < 1) return;

            int fullHeight = _tabItemStateMap[this.SelectedIndex].Bounds.Height;
            //int fullWidth = _tabItemStateMap[this.SelectedIndex].Bounds.Width;
            tabInfo.Bounds.Height = fullHeight;
            //tabInfo.Bounds.Width = fullWidth;

            MyColorTable colorTable = new MyColorTable();

            SolidBrush backBrush = new SolidBrush(BackColor);

            SolidBrush activeBrush = new SolidBrush(colorTable.MenuItemBorder);
            SolidBrush borderBrush = new SolidBrush(colorTable.MenuBorder);

            StringFormat sf = new StringFormat();

            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            sf.Trimming = StringTrimming.EllipsisCharacter;

            // Paint selected. 
            if ((tabInfo.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                gr.FillRectangle(backBrush, tabInfo.Bounds);
                gr.DrawLine(new Pen(borderBrush), new Point(tabInfo.Bounds.Left, tabInfo.Bounds.Top), new Point(tabInfo.Bounds.Right - 1, tabInfo.Bounds.Top));
                gr.DrawLine(new Pen(activeBrush), new Point(tabInfo.Bounds.Left, tabInfo.Bounds.Top + 1), new Point(tabInfo.Bounds.Right - 1, tabInfo.Bounds.Top + 1));
                gr.DrawLine(new Pen(activeBrush), new Point(tabInfo.Bounds.Left, tabInfo.Bounds.Top + 2), new Point(tabInfo.Bounds.Right - 1, tabInfo.Bounds.Top + 2));
                gr.DrawLine(new Pen(activeBrush), new Point(tabInfo.Bounds.Left, tabInfo.Bounds.Top + 3), new Point(tabInfo.Bounds.Right - 1, tabInfo.Bounds.Top + 3));
                gr.DrawLine(new Pen(borderBrush), new Point(tabInfo.Bounds.Left, tabInfo.Bounds.Top), new Point(tabInfo.Bounds.Left, tabInfo.Bounds.Bottom));
                gr.DrawLine(new Pen(borderBrush), new Point(tabInfo.Bounds.Right - 1, tabInfo.Bounds.Top), new Point(tabInfo.Bounds.Right - 1, tabInfo.Bounds.Bottom));
            }
            // Paint unselected.
            else
            {
                gr.FillRectangle(backBrush, tabInfo.Bounds);
                gr.DrawLine(new Pen(borderBrush), new Point(tabInfo.Bounds.Left-2, tabInfo.Bounds.Top), new Point(tabInfo.Bounds.Right + 1, tabInfo.Bounds.Top));
                gr.DrawLine(new Pen(borderBrush), new Point(tabInfo.Bounds.Left-2, tabInfo.Bounds.Top), new Point(tabInfo.Bounds.Left-2, tabInfo.Bounds.Bottom-3));
                gr.DrawLine(new Pen(borderBrush), new Point(tabInfo.Bounds.Right + 1, tabInfo.Bounds.Top), new Point(tabInfo.Bounds.Right + 1, tabInfo.Bounds.Bottom));
                gr.DrawLine(new Pen(borderBrush), new Point(tabInfo.Bounds.Left-2, tabInfo.Bounds.Bottom - 3), new Point(tabInfo.Bounds.Right +1, tabInfo.Bounds.Bottom - 3 ));
            }

            gr.DrawString(this.TabPages[tabInfo.Index].Text, new Font(tabInfo.Font.FontFamily, 8.25f), SystemBrushes.HighlightText, tabInfo.Bounds, sf);

            backBrush.Dispose();
        }
    }

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
                Rectangle textRectangle = new Rectangle(e.Bounds.Left + 48, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height);

                sf.LineAlignment = StringAlignment.Center;

                e.Graphics.FillRectangle(brush, e.Bounds);
                
                if (this.SmallImageList != null)
                {
                    e.Graphics.DrawImage(this.SmallImageList.Images[e.ItemIndex], e.Bounds.Left, e.Bounds.Top, 48, 16);

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

        protected override void OnHandleCreated(EventArgs e)
        {
            //return;
            if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
                DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
        }
        public enum Theme
        {
            Light,
            Dark
        };

        public struct ThemeColors
        {
            public Color backColor;
            public Color foreColor;
            public Color menuBorder;
            public Color menuHighlight;
        }

        ThemeColors darkColors = new ThemeColors
        {
            backColor = Color.FromArgb(32, 32, 32),
            foreColor = Color.FromArgb(0xFF, 0xFF, 0xFF),
            menuBorder = Color.FromKnownColor(KnownColor.MenuHighlight),
            menuHighlight = Color.FromArgb(128, Color.FromKnownColor(KnownColor.MenuHighlight))
        };

        public ThemeColors ActiveColors
        {
            get { return darkColors; }
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

        private void ApplyTheme(Theme theme)
        {
            if(theme == Theme.Light)
            {

            }else if(theme == Theme.Dark)
            {
                //ChangeTheme(this.Controls);

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
