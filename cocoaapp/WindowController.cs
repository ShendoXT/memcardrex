using System;
using Foundation;
using AppKit;
using Darwin;

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

            // Prefer Tabbed Windows
            Window.TabbingMode = NSWindowTabbingMode.Preferred;
            Window.TabbingIdentifier = "Main";

            // Set window to use Full Size Content View
            // Window.StyleMask = NSWindowStyle.FullSizeContentView;

            // Create a title bar accessory view controller and attach
            // the view created in Interface Builder
            var accessoryView = new NSTitlebarAccessoryViewController();
            accessoryView.View = AccessoryViewGoBar;

            // Set the location and attach the accessory view to the
            // titlebar to be displayed
            accessoryView.LayoutAttribute = NSLayoutAttribute.Bottom;
            Window.AddTitlebarAccessoryViewController(accessoryView);
        }

        public override void GetNewWindowForTab(NSObject sender)
        {
            // Ask app to open a new document window
            App.NewDocument(this);
        }
        #endregion
    }
}

