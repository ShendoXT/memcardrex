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
		
		void ReleaseDesignerOutlets ()
		{
			if (AccessoryViewGoBar != null) {
				AccessoryViewGoBar.Dispose ();
				AccessoryViewGoBar = null;
			}
		}
	}
}
