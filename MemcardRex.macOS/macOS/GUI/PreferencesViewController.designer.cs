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
	[Register ("PreferencesViewController")]
	partial class PreferencesViewController
	{
		[Outlet]
		AppKit.NSTextField addressInput { get; set; }

		[Outlet]
		AppKit.NSButton backupCheckbox { get; set; }

		[Outlet]
		AppKit.NSPopUpButton cardPopup { get; set; }

		[Outlet]
		AppKit.NSButton fixCheckbox { get; set; }

		[Outlet]
		AppKit.NSPopUpButton formatPopup { get; set; }

		[Outlet]
		AppKit.NSPopUpButton iconPopup { get; set; }

		[Outlet]
		AppKit.NSTextField portInput { get; set; }

		[Outlet]
		AppKit.NSPopUpButton portPopup { get; set; }

		[Outlet]
		AppKit.NSButton restoreCheckbox { get; set; }

		[Outlet]
		AppKit.NSPopUpButton speedPopup { get; set; }

		[Action ("cancelDialog:")]
		partial void cancelDialog (Foundation.NSObject sender);

		[Action ("confirmDialog:")]
		partial void confirmDialog (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (portPopup != null) {
				portPopup.Dispose ();
				portPopup = null;
			}

			if (speedPopup != null) {
				speedPopup.Dispose ();
				speedPopup = null;
			}

			if (addressInput != null) {
				addressInput.Dispose ();
				addressInput = null;
			}

			if (portInput != null) {
				portInput.Dispose ();
				portInput = null;
			}

			if (formatPopup != null) {
				formatPopup.Dispose ();
				formatPopup = null;
			}

			if (cardPopup != null) {
				cardPopup.Dispose ();
				cardPopup = null;
			}

			if (iconPopup != null) {
				iconPopup.Dispose ();
				iconPopup = null;
			}

			if (backupCheckbox != null) {
				backupCheckbox.Dispose ();
				backupCheckbox = null;
			}

			if (restoreCheckbox != null) {
				restoreCheckbox.Dispose ();
				restoreCheckbox = null;
			}

			if (fixCheckbox != null) {
				fixCheckbox.Dispose ();
				fixCheckbox = null;
			}
		}
	}
}
