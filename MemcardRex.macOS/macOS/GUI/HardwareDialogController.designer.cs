// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MemcardRex.macOS
{
	[Register ("HardwareDialogController")]
	partial class HardwareDialogController
	{
		[Outlet]
		AppKit.NSButton abortButton { get; set; }

		[Outlet]
		AppKit.NSTextField deviceLabel { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator progressBar { get; set; }

		[Action ("abortDialog:")]
		partial void abortDialog (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (deviceLabel != null) {
				deviceLabel.Dispose ();
				deviceLabel = null;
			}

			if (progressBar != null) {
				progressBar.Dispose ();
				progressBar = null;
			}

			if (abortButton != null) {
				abortButton.Dispose ();
				abortButton = null;
			}
		}
	}
}
