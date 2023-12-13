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
	[Register ("CommentDialogController")]
	partial class CommentDialogController
	{
		[Outlet]
		AppKit.NSTextFieldCell CommentTextField { get; set; }

		[Outlet]
		AppKit.NSTextField CommentTextInput { get; set; }

		[Action ("CancelDialog:")]
		partial void CancelDialog (Foundation.NSObject sender);

		[Action ("ConfirmDialog:")]
		partial void ConfirmDialog (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (CommentTextInput != null) {
				CommentTextInput.Dispose ();
				CommentTextInput = null;
			}

			if (CommentTextField != null) {
				CommentTextField.Dispose ();
				CommentTextField = null;
			}
		}
	}
}
