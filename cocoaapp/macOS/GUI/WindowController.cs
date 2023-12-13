using System;
using Foundation;
using AppKit;

namespace MemcardRex
{
    public partial class WindowController : NSWindowController
    {
        #region Application Access
        /// <summary>
        /// A helper shortcut to the app delegate.
        /// </summary>
        /// <value>The app.</value>
        public static AppDelegate App
        {
            get { return (AppDelegate)NSApplication.SharedApplication.Delegate; }
        }
        #endregion

        #region Constructor
        public WindowController(IntPtr handle) : base(handle)
        {

        }
        #endregion

        #region Override Methods
        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

            Window.TabbingIdentifier = "Main";

            //Disable Temp buffer toolbar button if temp buffer is empty
            if(App.TempBuffer == null) tmpBufferToolbar.Enabled = false;
        }

        public void SetPasteFromBufferToolbar(NSImage icon, bool itemState)
        {
            if (icon != null) tmpBufferToolbar.Image = icon; 
            tmpBufferToolbar.Enabled = itemState;
        }

        public override void GetNewWindowForTab(NSObject sender)
        {
            // Ask app to open a new document window
            App.NewDocument(this);
        }

        [Export("tmpBufferToolbarAction:")]
        void TmpBufferToolbarAction(NSObject sender)
        {
            var window = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;
            window.PasteFromTempBuffer(sender);
        }

        [Export("editHeaderToolbar:")]
        void EditHeaderToolbar(NSObject sender)
        {
            var window = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;
            window.EditHeader(sender);
        }

        [Export("editCommentToolbar:")]
        void EditCommentToolbar(NSObject sender)
        {
            var window = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;
            window.EditSaveComment(sender);
        }

        [Export("openDocToolbar:")]
        void OpenDocToolbar(NSObject sender)
        {
            App.OpenDialog(sender);
        }

        [Export("saveDocToolbar:")]
        void SaveDocToolbar(NSObject sender)
        {
            var window = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;
            window.SaveDialog(sender);
        }

        #endregion
    }
}

