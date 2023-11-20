using System;
using AppKit;
using CoreGraphics;
using Foundation;
using System.Collections;
using System.Collections.Generic;
using ImageKit;
using System.Drawing;

namespace MemcardRex
{
    public class Product
    {
        #region Computed Properties
        public string Title { get; set; } = "";
        public string ProductCode { get; set; } = "";
        public string Identifier { get; set; } = "";
        public Color[] IconData { get; set; } = new Color[256];
        public ushort SaveRegion { get; set; } = 0;
        public bool HideIcons { get; set; } = false;
        #endregion

        #region Constructors
        public Product()
        {
        }

        public Product(string title, string productCode, string identifier, Color[] iconData, ushort saveRegion)
        {
            this.Title = title;
            this.ProductCode = productCode;
            this.Identifier = identifier;
            this.IconData = iconData;
            this.SaveRegion = saveRegion;
            this.HideIcons = false;
        }

        public Product(string title)
        {
            this.Title = title;
            this.ProductCode = "";
            this.Identifier = "";
            this.HideIcons = true;
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
        public ProductTableDelegate(ProductTableDataSource datasource)
        {
            this.DataSource = datasource;
        }
        #endregion

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

                    //Flipped because bmp stores data flipped but the raw icon data is not
                    image.Flipped = true;

                    //Select proper flag for region
                    switch (DataSource.Products[(int)row].SaveRegion)
                    {
                        default:        //Formatted save, Corrupted save, Unknown region
                            flagImage = new NSImage("naflag.bmp");
                            break;

                        case 0x4142:    //American region
                            flagImage = new NSImage("amflag.bmp");
                            break;

                        case 0x4542:    //European region
                            flagImage = new NSImage("euflag.bmp");
                            break;

                        case 0x4942:    //Japanese region
                            flagImage = new NSImage("jpflag.bmp");
                            break;
                    }

                    //Compose icon and flag
                    float width = (float)(16);
                    float height = (float)(16);
                    var targetRect = new CoreGraphics.CGRect(0, 0, width, height);
                    var newImage = new NSImage(new CoreGraphics.CGSize(48, height));
                    newImage.LockFocus();
                    image.Draw(targetRect, CoreGraphics.CGRect.Empty, NSCompositingOperation.SourceOver, 1.0f);

                    targetRect = new CoreGraphics.CGRect(17, 0, 30, height);
                    flagImage.Draw(targetRect, CoreGraphics.CGRect.Empty, NSCompositingOperation.SourceOver, 1.0f);
                    newImage.UnlockFocus();

                    if (!DataSource.Products[(int)row].HideIcons)view.ImageView.Image = newImage;
                    view.TextField.StringValue = DataSource.Products[(int)row].Title;
                    break;

                case "Product code":
                    view.TextField.StringValue = DataSource.Products[(int)row].ProductCode;
                    break;

                case "Identifier":
                    view.TextField.StringValue = DataSource.Products[(int)row].Identifier;
                    break;
            }

            return view;
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

