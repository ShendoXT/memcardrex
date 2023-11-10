using AppKit;
using Foundation;
using System;

namespace MemcardRex
{
	[Register ("AppDelegate")]
	public class AppDelegate : NSApplicationDelegate
	{
        //Number of open windows/documents/cards in the application
        private int windowCount = 1;

        public AppDelegate ()
		{

		}

		public override void DidFinishLaunching (NSNotification notification)
		{
            //Show experimental software alert
            var alert = new NSAlert()
            {
                AlertStyle = NSAlertStyle.Critical,
                InformativeText = "This build is higly experimental. Most of the funcionality is not yet implemented.\n\n"
                 + "The only guaranteed option available is opening and saving Memory Cards.",
                MessageText = "Warning",
            };

            alert.RunModal();

            //NewDocument(this);
        }

		public override void WillTerminate (NSNotification notification)
		{
			// Insert code here to tear down your application
		}

        #region Custom Actions
        [Export("newDocument:")]
        public void NewDocument(NSObject sender)
        {
            // Get new window
            var storyboard = NSStoryboard.FromName("Main", null);
            var controller = storyboard.InstantiateControllerWithIdentifier("MainWindow") as NSWindowController;

            // Display
            controller.ShowWindow(this);

            windowCount++;
        }

        //Open file menu item
        [Export("openDocument:")]
        void OpenDialog(NSObject sender)
        {
            ps1card memCard = new ps1card();

            var dlg = NSOpenPanel.OpenPanel;
            dlg.CanChooseFiles = true;
            dlg.CanChooseDirectories = false;
            dlg.AllowedFileTypes = memCard.SupportedExtensions;
            //dlg.ExtensionHidden = false;

            if (dlg.RunModal() == 1)
            {
                // Nab the first file
                var url = dlg.Urls[0];

                if (url != null)
                {
                    var path = url.Path;
                    string message = memCard.OpenMemoryCard(path, false);

                    if(message != null)
                    {
                        var alert = new NSAlert()
                        {
                            AlertStyle = NSAlertStyle.Critical,
                            InformativeText = message,
                            MessageText = "Unable to open Memory Card",
                        };

                        alert.RunModal();
                        return;
                    }

                    /*for(nuint i = 0; i < NSApplication.SharedApplication.DangerousWindows.Count; i++)
                    {
                        Console.WriteLine(i);
                        Console.WriteLine(NSApplication.SharedApplication.DangerousWindows[(int)i].Title);
                    }*/

                    //return;

                    /*var activeWindow = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;

                    //Check if only new unedited tab is active
                    if (windowCount == 1 && activeWindow.MemoryCardPath() == null && !activeWindow.View.Window.DocumentEdited)
                    {
                        //Set path of the MemoryCard
                        activeWindow.SetMemoryCard(path);
                        return;
                    }*/

                    //Open new empty document
                    NewDocument(this);

                    var window = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;

                    //Set path of the MemoryCard
                    window.SetMemoryCard(path);
                }
            }
        }
        #endregion
    }
}

