using AppKit;
using Foundation;
using GameController;
using System;
using System.Collections.Generic;

namespace MemcardRex
{
    [Register("AppDelegate")]
    public partial class AppDelegate : NSApplicationDelegate
    {
        //List of all active window controllers
        List<NSObject> winCtrlList = new List<NSObject>();

        #region Private Variables
        private byte[] _tempBuffer;
        private NSImage _bufferImage;
        private int _cardCount = 1;
        #endregion

        #region Computed Properties
        public byte[] TempBuffer
        {
            get { return _tempBuffer; }
            set { _tempBuffer = value; }
        }

        public NSImage BufferImage
        {
            get { return _bufferImage; }
            set { _bufferImage = value; }
        }

        public int CardCount
        {
            get { return _cardCount; }
            set { _cardCount = value; }
        }
        #endregion

        public AppDelegate()
        {

        }

        //Return controller which holds memory card of the specified path
        public NSObject GetControllerForPath(string path)
        {
            for (int i = 0; i < winCtrlList.Count; i++)
            {
                if (((winCtrlList[i] as WindowController).ContentViewController as ViewController).MemoryCardPath() == path) return winCtrlList[i];
            }
            return null;
        }

        //Pop a specified window controller from the list
        public void PopControllerFromList(NSObject windowCtrl)
        {
            winCtrlList.Remove(windowCtrl);
        }

        //Triggered on each tab change
        void TabChangeCallback(NSNotification notification)
        {
            try {
                var contentView = NSApplication.SharedApplication.KeyWindow.ContentViewController;
                if (contentView.GetType() != typeof(ViewController)) return;

                var window = NSApplication.SharedApplication.KeyWindow.WindowController as WindowController;

                //Update menus and stuff
                ((ViewController)contentView).SelectionDidChange();

                //Update temp buffer icon and state
                window.SetPasteFromBufferToolbar(BufferImage, TempBuffer != null);
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e.ToString());
#endif
            }
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            //Show experimental software alert
            var alert = new NSAlert()
            {
                AlertStyle = NSAlertStyle.Critical,
                InformativeText = "This build is higly experimental." +
                "\nAll funcionality may not be implemented yet or work as intended.",
                MessageText = "Warning",
            };

            //alert.RunModal();

            //Add initial window controller to the controller list
            winCtrlList.Add(NSApplication.SharedApplication.KeyWindow.WindowController);

            //Register Tab change event for menu manipulation
            NSNotificationCenter.DefaultCenter.AddObserver(NSWindow.DidBecomeMainNotification, TabChangeCallback);
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }

        //Specific menu item enables/disableas
        public void EnableFormatMenuItem()
        {
            removeSaveMItem.Enabled = true;
        }

        public void SetImportMenuItem(bool itemState)
        {
            importSaveMItem.Enabled = itemState;
        }

        public void DisableRestoreItem()
        {
            restoreSaveMItem.Enabled = false;
        }

        public void DisableDeleteItem()
        {
            deleteSaveMItem.Enabled = false;
        }

        public void SetPasteFromBuffer(bool itemState)
        {
            pasteSaveTempBufferMItem.Enabled = itemState;
        }

        public void SetUndoItem(bool itemState)
        {
            undoMItem.Enabled = itemState;
        }

        public void SetRedoItem(bool itemState)
        {
            redoMItem.Enabled = itemState;
        }

        //Enable or disable menu items based on the currently selected save
        public void SetEditItemsState(bool itemStates)
        {
            editSaveHeaderMItem.Enabled = itemStates;
            editSaveCommentMItem.Enabled = itemStates;
            compareBufferMItem.Enabled = itemStates;
            editIconMItem.Enabled = itemStates;
            deleteSaveMItem.Enabled = itemStates;
            restoreSaveMItem.Enabled = itemStates;
            removeSaveMItem.Enabled = itemStates;
            cpySaveTempBufferMItem.Enabled = itemStates;
            pasteSaveTempBufferMItem.Enabled = itemStates;
            importSaveMItem.Enabled = itemStates;
            exportSaveMItem.Enabled = itemStates;
            exportSaveRawMItem.Enabled = itemStates;
        }

        #region Custom Actions
        [Export("newDocument:")]
        public void NewDocument(NSObject sender)
        {
            // Get new window
            var storyboard = NSStoryboard.FromName("Main", null);
            var controller = storyboard.InstantiateControllerWithIdentifier("MainWindow") as NSWindowController;

            //Add controller to list
            winCtrlList.Add(controller);

            // Display
            controller.ShowWindow(this);

            _cardCount++;
        }

        [Export("editSaveComment:")]
        void EditSaveComment(NSObject sender)
        {
            var window = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;
            window.EditSaveComment(sender);
        }

        [Export("copyToTempBuffer:")]
        void CopyToTempBuffer(NSObject sender)
        {
            var window = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;
            window.CopyToTempBuffer(sender);
        }

        [Export("deleteSave:")]
        void DeleteSave(NSObject sender)
        {
            var window = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;
            window.DeleteSave(sender);
        }

        [Export("restoreSave:")]
        void RestoreSave(NSObject sender)
        {
            var window = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;
            window.RestoreSave(sender);
        }

        [Export("removeSave:")]
        void RemoveSave(NSObject sender)
        {
            var window = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;
            window.RemoveSave(sender);
        }

        [Export("pasteFromTempBuffer:")]
        void PasteFromTempBuffer(NSObject sender)
        {
            var window = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;
            window.PasteFromTempBuffer(sender);
        }

        [Export("undoOperation:")]
        void UndoOperation(NSObject sender)
        {
            var window = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;
            window.Undo();
        }

        [Export("redoOperation:")]
        void RedoOperation(NSObject sender)
        {
            var window = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;
            window.Redo();
        }

        //Open Memory Card dialog
        public void OpenDialog(NSObject sender)
        {
            ps1card memCard = new ps1card();

            var dlg = NSOpenPanel.OpenPanel;
            dlg.CanChooseFiles = true;
            dlg.CanChooseDirectories = false;
            dlg.AllowedFileTypes = memCard.SupportedExtensions;

            if (dlg.RunModal() == 1)
            {
                // Nab the first file
                var url = dlg.Urls[0];

                if (url != null)
                {
                    var path = url.Path;

                    NSObject existingCtrl = GetControllerForPath(path);

                    //Check if the card is already opened
                    if(existingCtrl != null)
                    {
                        (existingCtrl as WindowController).ShowWindow(this);
                        dlg.Dispose();
                        return;
                    }

                    //Open memory card, get message on error
                    string message = memCard.OpenMemoryCard(path, false);

                    if (message != null)
                    {
                        var alert = new NSAlert()
                        {
                            AlertStyle = NSAlertStyle.Critical,
                            InformativeText = message,
                            MessageText = "Unable to open Memory Card"
                        };

                        alert.RunModal();
                        dlg.Dispose();
                        return;
                    }

                    if(NSApplication.SharedApplication.KeyWindow == null)
                    {
                        NewDocument(sender);
                    }
                    else
                    {
                        var viewCtrl = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;

                        //Check if a new window (tab) needs to be opened
                        //Only do this check when there is 1 active window
                        if (CardCount == 1)
                        {
                            //Check if only new unedited tab is active
                            if (!(viewCtrl.MemoryCardPath() == null && !viewCtrl.View.Window.DocumentEdited))
                                NewDocument(sender);
                        }
                        else
                        {
                            NewDocument(sender);
                        }
                    }

                    var activeWindow = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;
                    activeWindow.SetMemoryCard(path);
                }
            }

            dlg.Dispose();
        }

        //Open file menu item
        [Export("openDocument:")]
        public void OpenDlg(NSObject sender)
        {
            OpenDialog(sender);
        }
        #endregion
    }
}
