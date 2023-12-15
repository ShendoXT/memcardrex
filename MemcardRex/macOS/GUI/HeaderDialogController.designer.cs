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
	[Register ("HeaderDialogController")]
	partial class HeaderDialogController
	{
		[Outlet]
		AppKit.NSButton CancelButton { get; set; }

		[Outlet]
		AppKit.NSTextField IdentifierInput { get; set; }

		[Outlet]
		AppKit.NSButton OkButton { get; set; }

		[Outlet]
		AppKit.NSTextField ProductCodeInput { get; set; }

		[Outlet]
		AppKit.NSComboBox RegionCombo { get; set; }

		[Action ("CancelDialog:")]
		partial void CancelDialog (Foundation.NSObject sender);

		[Action ("ConfirmDialog:")]
		partial void ConfirmDialog (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}

			if (IdentifierInput != null) {
				IdentifierInput.Dispose ();
				IdentifierInput = null;
			}

			if (OkButton != null) {
				OkButton.Dispose ();
				OkButton = null;
			}

			if (ProductCodeInput != null) {
				ProductCodeInput.Dispose ();
				ProductCodeInput = null;
			}

			if (RegionCombo != null) {
				RegionCombo.Dispose ();
				RegionCombo = null;
			}
		}
	}
}
