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
	[Register ("WindowController")]
	partial class WindowController
	{
		[Outlet]
		AppKit.NSView AccessoryViewGoBar { get; set; }

		[Action ("editCommentToolbar:")]
		partial void editCommentToolbar (Foundation.NSObject sender);

		[Action ("editHeaderToolbar:")]
		partial void editHeaderToolbar (Foundation.NSObject sender);

		[Action ("editIconToolbar:")]
		partial void editIconToolbar (Foundation.NSObject sender);

		[Action ("exportSaveToolbar:")]
		partial void exportSaveToolbar (Foundation.NSObject sender);

		[Action ("importSaveToolbar:")]
		partial void importSaveToolbar (Foundation.NSObject sender);

		[Action ("openDocToolbar:")]
		partial void openDocToolbar (Foundation.NSObject sender);

		[Action ("saveDocToolbar:")]
		partial void saveDocToolbar (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AccessoryViewGoBar != null) {
				AccessoryViewGoBar.Dispose ();
				AccessoryViewGoBar = null;
			}
		}
	}
}
