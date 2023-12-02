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
	[Register ("InfoDialogController")]
	partial class InfoDialogController
	{
		[Outlet]
		AppKit.NSTextField iconFramesLabel { get; set; }

		[Outlet]
		AppKit.NSImageView iconImage { get; set; }

		[Outlet]
		AppKit.NSTextField identifierLabel { get; set; }

		[Outlet]
		AppKit.NSTextField productCodeLabel { get; set; }

		[Outlet]
		AppKit.NSTextField regionLabel { get; set; }

		[Outlet]
		AppKit.NSTextField sizeLabel { get; set; }

		[Outlet]
		AppKit.NSTextField slotLabel { get; set; }

		[Outlet]
		AppKit.NSTextField titleLabel { get; set; }

		[Action ("CloseDialog:")]
		partial void CloseDialog (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (titleLabel != null) {
				titleLabel.Dispose ();
				titleLabel = null;
			}

			if (productCodeLabel != null) {
				productCodeLabel.Dispose ();
				productCodeLabel = null;
			}

			if (identifierLabel != null) {
				identifierLabel.Dispose ();
				identifierLabel = null;
			}

			if (regionLabel != null) {
				regionLabel.Dispose ();
				regionLabel = null;
			}

			if (slotLabel != null) {
				slotLabel.Dispose ();
				slotLabel = null;
			}

			if (sizeLabel != null) {
				sizeLabel.Dispose ();
				sizeLabel = null;
			}

			if (iconFramesLabel != null) {
				iconFramesLabel.Dispose ();
				iconFramesLabel = null;
			}

			if (iconImage != null) {
				iconImage.Dispose ();
				iconImage = null;
			}
		}
	}
}
