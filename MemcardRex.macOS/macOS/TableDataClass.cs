using System;
using AppKit;
using CoreGraphics;
using Foundation;
using System.Collections.Generic;
using System.Drawing;
using MemcardRex.Core;

namespace MemcardRex
{
    // Declarationm of delegate for selection change event
    public delegate void SelectionChangeCallback();

    public class Product
    {
        #region Computed Properties
        public string Title { get; set; } = "";
        public string ProductCode { get; set; } = "";
        public string Identifier { get; set; } = "";
        public Color[] IconData { get; set; } = new Color[256];
        public string SaveRegion { get; set; } = "";
        public bool HideIcons { get; set; } = false;
        public bool FadedIcons { get; set; } = false;
        #endregion

        #region Constructors
        public Product()
        {
        }

        public Product(string title, string productCode, string identifier, Color[] iconData, string saveRegion, bool fadedIcons)
        {
            this.Title = title;
            this.ProductCode = productCode;
            this.Identifier = identifier;
            this.IconData = iconData;
            this.SaveRegion = saveRegion;
            this.HideIcons = false;
            this.FadedIcons = fadedIcons;
        }

        public Product(string title)
        {
            this.Title = title;
            this.ProductCode = "";
            this.Identifier = "";
            this.HideIcons = true;
            this.FadedIcons = false;
        }
        #endregion
    }

    public class ProductTableDataSource : NSTableViewDataSource
    {
        #region Public Variables
        public List<Product> Products = new List<Product>();
        #endregion

        #region Constructors
        public ProductTableDataSource()
        {
        }
        #endregion

        #region Override Methods
        public override nint GetRowCount(NSTableView tableView)
        {
            return Products.Count;
        }

        //public override 

        /*public override NSDragOperation DraggingEntered(NSDraggingInfo sender)
        {
            // When we start dragging, inform the system that we will be handling this as
            // a copy/paste
            Console.WriteLine("draggin");
            return NSDragOperation.Copy;
        }*/

        /*public override NSDragOperation ValidateDrop(NSTableView tableView, NSDraggingInfo info, nint row, NSTableViewDropOperation dropOperation)
        {
            return NSDragOperation.All;
        }*/
        #endregion
    }

    public class ProductTableDelegate : NSTableViewDelegate
    {
        #region Constants 
        private const string CellIdentifier = "ProdCell";
        #endregion

        #region Private Variables
        private ProductTableDataSource DataSource;
        #endregion

        #region Constructors
        public ProductTableDelegate(ProductTableDataSource datasource, SelectionChangeCallback call)
        {
            this.DataSource = datasource;
            handle = call;
        }
        #endregion

        public SelectionChangeCallback handle { get; set; }

        #region Override Methods
        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            NSTableCellView view = (NSTableCellView)tableView.MakeView(CellIdentifier, this);
            if (view == null)
            {
                view = new NSTableCellView();
                if (tableColumn.Title == "Icon, region and title")
                {
                    view.ImageView = new NSImageView(new CGRect(0, 0, 48, 16));
                    view.AddSubview(view.ImageView);
                    view.TextField = new NSTextField(new CGRect(52, -1, 600, 18));
                }
                else
                {
                    view.TextField = new NSTextField(new CGRect(0, -1, 400, 18));
                }
                view.TextField.AutoresizingMask = NSViewResizingMask.WidthSizable;
                view.AddSubview(view.TextField);
                view.Identifier = CellIdentifier;
                view.TextField.BackgroundColor = NSColor.Clear;
                view.TextField.Bordered = false;
                view.TextField.Selectable = false;
                //view.TextField.Editable = true;
            }

            // Setup view based on the column selected
            switch (tableColumn.Title)
            {
                case "Icon, region and title":

                    BmpBuilder bmpImage = new BmpBuilder();

                    NSData imageData = NSData.FromArray(bmpImage.BuildBmp(DataSource.Products[(int)row].IconData));
                    NSImage image = new NSImage(imageData);
                    NSImage flagImage;

                    string resourceBaseDir = System.AppDomain.CurrentDomain.BaseDirectory + "/../Resources/";

                    //Select proper flag for region
                    switch (DataSource.Products[(int)row].SaveRegion)
                    {
                        default:        //Formatted save, Corrupted save, Unknown region
                            //Add region string to product code
                            DataSource.Products[(int)row].ProductCode = DataSource.Products[(int)row].SaveRegion + DataSource.Products[(int)row].ProductCode;

                            //flagImage = new NSImage("naflag.bmp");
                            flagImage = new NSImage(new CGSize ( 32, 16 ));
                            break;

                        case "America":    //American region
                            flagImage = new NSImage(resourceBaseDir + "amflag.bmp");
                            break;

                        case "Europe":    //European region
                            flagImage = new NSImage(resourceBaseDir + "euflag.bmp");
                            break;

                        case "Japan":    //Japanese region
                            flagImage = new NSImage(resourceBaseDir + "jpflag.bmp");
                            break;
                    }

                    //Compose icon and flag
                    float width = (float)(16);
                    float height = (float)(16);
                    var targetRect = new CoreGraphics.CGRect(0, 0, width, height);
                    var newImage = new NSImage(new CoreGraphics.CGSize(48, height));

                    float blendingVal = 1.0f;
                    if(DataSource.Products[(int)row].FadedIcons) blendingVal = 0.5f;

                    newImage.LockFocus();
                    image.Draw(targetRect, CoreGraphics.CGRect.Empty, NSCompositingOperation.Overlay, blendingVal);

                    targetRect = new CoreGraphics.CGRect(17, 0, 30, height);
                    flagImage.Draw(targetRect, CoreGraphics.CGRect.Empty, NSCompositingOperation.Overlay, blendingVal);
                    newImage.UnlockFocus();

                    if (!DataSource.Products[(int)row].HideIcons) view.ImageView.Image = newImage;
                    else view.ImageView.Image = new NSImage(new CGSize(48, 16));

                    view.TextField.StringValue = DataSource.Products[(int)row].Title;

                    /*if(DataSource.Products[(int)row].FadedIcons) view.TextField.TextColor = NSColor.SecondaryLabel;
                    else view.TextField.TextColor = NSColor.Text;*/
                    break;

                case "Product code":
                    view.TextField.StringValue = DataSource.Products[(int)row].ProductCode;
                    /*if (DataSource.Products[(int)row].FadedIcons) view.TextField.TextColor = NSColor.SecondaryLabel;
                    else view.TextField.TextColor = NSColor.Text;*/
                    break;

                case "Identifier":
                    view.TextField.StringValue = DataSource.Products[(int)row].Identifier;
                    /*if (DataSource.Products[(int)row].FadedIcons) view.TextField.TextColor = NSColor.SecondaryLabel;
                    else view.TextField.TextColor = NSColor.Text;*/
                    break;
            }

            return view;
        }

        //public override 

        public override void SelectionDidChange(NSNotification notification)
        {
            if (handle != null) handle();
        }
        #endregion
    }

    /*public class TableDataClass
	{
		public TableDataClass()
		{
		}

	}*/
}

