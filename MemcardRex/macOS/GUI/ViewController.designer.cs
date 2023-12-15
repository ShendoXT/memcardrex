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
		AppKit.NSMenuItem compareSaveItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem cpyTempBufferItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem deleteSaveItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem editIconItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem editSaveCommentItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem editSaveHeaderItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem exportSaveItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem exportSaveRawItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem getInfoItem { get; set; }

		[Outlet]
		AppKit.NSTableColumn IdentifierColumn { get; set; }

		[Outlet]
		AppKit.NSMenuItem importSaveItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem pasteTempBufferItem { get; set; }

		[Outlet]
		AppKit.NSTableColumn ProductColumn { get; set; }

		[Outlet]
		AppKit.NSMenuItem removeSaveItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem restoreSaveItem { get; set; }

		[Outlet]
		AppKit.NSTableColumn TitleColumn { get; set; }

		[Action ("ClickedButton:")]
		partial void ClickedButton (Foundation.NSObject sender);

		[Action ("compareTempBuffer:")]
		partial void compareTempBuffer (Foundation.NSObject sender);

		[Action ("copyToTempBuffer:")]
		partial void copyToTempBuffer (Foundation.NSObject sender);

		[Action ("deleteSave:")]
		partial void deleteSave (Foundation.NSObject sender);

		[Action ("editIcon:")]
		partial void editIcon (Foundation.NSObject sender);

		[Action ("editSaveComment:")]
		partial void editSaveComment (Foundation.NSObject sender);

		[Action ("exporrtSaveRaw:")]
		partial void exporrtSaveRaw (Foundation.NSObject sender);

		[Action ("exportSave:")]
		partial void exportSave (Foundation.NSObject sender);

		[Action ("getInfo:")]
		partial void getInfo (Foundation.NSObject sender);

		[Action ("importSave:")]
		partial void importSave (Foundation.NSObject sender);

		[Action ("pasteFromTempBuffer:")]
		partial void pasteFromTempBuffer (Foundation.NSObject sender);

		[Action ("removeSave:")]
		partial void removeSave (Foundation.NSObject sender);

		[Action ("restoreSave:")]
		partial void restoreSave (Foundation.NSObject sender);
		
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

			if (compareSaveItem != null) {
				compareSaveItem.Dispose ();
				compareSaveItem = null;
			}

			if (cpyTempBufferItem != null) {
				cpyTempBufferItem.Dispose ();
				cpyTempBufferItem = null;
			}

			if (deleteSaveItem != null) {
				deleteSaveItem.Dispose ();
				deleteSaveItem = null;
			}

			if (editIconItem != null) {
				editIconItem.Dispose ();
				editIconItem = null;
			}

			if (editSaveCommentItem != null) {
				editSaveCommentItem.Dispose ();
				editSaveCommentItem = null;
			}

			if (editSaveHeaderItem != null) {
				editSaveHeaderItem.Dispose ();
				editSaveHeaderItem = null;
			}

			if (exportSaveItem != null) {
				exportSaveItem.Dispose ();
				exportSaveItem = null;
			}

			if (exportSaveRawItem != null) {
				exportSaveRawItem.Dispose ();
				exportSaveRawItem = null;
			}

			if (getInfoItem != null) {
				getInfoItem.Dispose ();
				getInfoItem = null;
			}

			if (IdentifierColumn != null) {
				IdentifierColumn.Dispose ();
				IdentifierColumn = null;
			}

			if (importSaveItem != null) {
				importSaveItem.Dispose ();
				importSaveItem = null;
			}

			if (pasteTempBufferItem != null) {
				pasteTempBufferItem.Dispose ();
				pasteTempBufferItem = null;
			}

			if (ProductColumn != null) {
				ProductColumn.Dispose ();
				ProductColumn = null;
			}

			if (removeSaveItem != null) {
				removeSaveItem.Dispose ();
				removeSaveItem = null;
			}

			if (restoreSaveItem != null) {
				restoreSaveItem.Dispose ();
				restoreSaveItem = null;
			}

			if (TitleColumn != null) {
				TitleColumn.Dispose ();
				TitleColumn = null;
			}
		}
	}
}
