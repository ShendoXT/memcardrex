using System;
using AppKit;
using CoreGraphics;
using Foundation;
using System.Collections;
using System.Collections.Generic;

namespace MemcardRex
{
    public class Product
    {
        #region Computed Properties
        public string Title { get; set; } = "";
        public string ProductCode { get; set; } = "";
        public string Identifier { get; set; } = "";
        #endregion

        #region Constructors
        public Product()
        {
        }

        public Product(string title, string productCode, string identifier)
        {
            this.Title = title;
            this.ProductCode = productCode;
            this.Identifier = identifier;
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
            // This pattern allows you reuse existing views when they are no-longer in use.
            // If the returned view is null, you instance up a new view
            // If a non-null view is returned, you modify it enough to reflect the new data
            NSTextField view = (NSTextField)tableView.MakeView(CellIdentifier, this);
            if (view == null)
            {
                view = new NSTextField();
                view.Identifier = CellIdentifier;
                view.BackgroundColor = NSColor.Clear;
                view.Bordered = false;
                view.Selectable = false;
                view.Editable = false;
            }

            // Setup view based on the column selected
            switch (tableColumn.Title)
            {
                case "Title":
                    view.StringValue = DataSource.Products[(int)row].Title;
                    break;
                case "Product code":
                    view.StringValue = DataSource.Products[(int)row].ProductCode;
                    break;
                case "Identifier":
                    view.StringValue = DataSource.Products[(int)row].Identifier;
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

