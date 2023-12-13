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
	partial class AppDelegate
	{
		[Outlet]
		AppKit.NSMenuItem compareBufferMItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem cpySaveTempBufferMItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem deleteSaveMItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem editIconMItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem editSaveCommentMItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem editSaveHeaderMItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem exportSaveMItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem exportSaveRawMItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem importSaveMItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem pasteSaveTempBufferMItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem redoMItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem removeSaveMItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem restoreSaveMItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem undoMItem { get; set; }

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

		[Action ("exportSave:")]
		partial void exportSave (Foundation.NSObject sender);

		[Action ("exportSaveRaw:")]
		partial void exportSaveRaw (Foundation.NSObject sender);

		[Action ("importSave:")]
		partial void importSave (Foundation.NSObject sender);

		[Action ("pasteFromTempBuffer:")]
		partial void pasteFromTempBuffer (Foundation.NSObject sender);

		[Action ("redoOperation:")]
		partial void redoOperation (Foundation.NSObject sender);

		[Action ("removeSave:")]
		partial void removeSave (Foundation.NSObject sender);

		[Action ("restoreSave:")]
		partial void restoreSave (Foundation.NSObject sender);

		[Action ("undoOperation:")]
		partial void undoOperation (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (undoMItem != null) {
				undoMItem.Dispose ();
				undoMItem = null;
			}

			if (redoMItem != null) {
				redoMItem.Dispose ();
				redoMItem = null;
			}

			if (compareBufferMItem != null) {
				compareBufferMItem.Dispose ();
				compareBufferMItem = null;
			}

			if (cpySaveTempBufferMItem != null) {
				cpySaveTempBufferMItem.Dispose ();
				cpySaveTempBufferMItem = null;
			}

			if (deleteSaveMItem != null) {
				deleteSaveMItem.Dispose ();
				deleteSaveMItem = null;
			}

			if (editIconMItem != null) {
				editIconMItem.Dispose ();
				editIconMItem = null;
			}

			if (editSaveCommentMItem != null) {
				editSaveCommentMItem.Dispose ();
				editSaveCommentMItem = null;
			}

			if (editSaveHeaderMItem != null) {
				editSaveHeaderMItem.Dispose ();
				editSaveHeaderMItem = null;
			}

			if (exportSaveMItem != null) {
				exportSaveMItem.Dispose ();
				exportSaveMItem = null;
			}

			if (exportSaveRawMItem != null) {
				exportSaveRawMItem.Dispose ();
				exportSaveRawMItem = null;
			}

			if (importSaveMItem != null) {
				importSaveMItem.Dispose ();
				importSaveMItem = null;
			}

			if (pasteSaveTempBufferMItem != null) {
				pasteSaveTempBufferMItem.Dispose ();
				pasteSaveTempBufferMItem = null;
			}

			if (removeSaveMItem != null) {
				removeSaveMItem.Dispose ();
				removeSaveMItem = null;
			}

			if (restoreSaveMItem != null) {
				restoreSaveMItem.Dispose ();
				restoreSaveMItem = null;
			}
		}
	}
}
