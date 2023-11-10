// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MemcardRex
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSClipView CardList { get; set; }

		[Outlet]
		AppKit.NSTableView CardTable { get; set; }

		[Outlet]
		AppKit.NSTextField ClickedLabel { get; set; }

		[Outlet]
		AppKit.NSTableColumn IdentifierColumn { get; set; }

		[Outlet]
		AppKit.NSTableColumn ProductColumn { get; set; }

		[Outlet]
		AppKit.NSTableColumn TitleColumn { get; set; }

		[Action ("ClickedButton:")]
		partial void ClickedButton (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (CardList != null) {
				CardList.Dispose ();
				CardList = null;
			}

			if (CardTable != null) {
				CardTable.Dispose ();
				CardTable = null;
			}

			if (ClickedLabel != null) {
				ClickedLabel.Dispose ();
				ClickedLabel = null;
			}

			if (IdentifierColumn != null) {
				IdentifierColumn.Dispose ();
				IdentifierColumn = null;
			}

			if (ProductColumn != null) {
				ProductColumn.Dispose ();
				ProductColumn = null;
			}

			if (TitleColumn != null) {
				TitleColumn.Dispose ();
				TitleColumn = null;
			}
		}
	}
}
