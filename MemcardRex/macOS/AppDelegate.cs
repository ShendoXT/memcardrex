using AppKit;
using Foundation;
using System;
using System.Collections.Generic;
using System.IO;

namespace MemcardRex
{
    [Register("AppDelegate")]
    public partial class AppDelegate : NSApplicationDelegate
    {
        const string appName = "MemcardRex";
        const string appDate = "Unknown";
        const string appVersion = "2.0 alpha";

        //List of all active window controllers
        List<NSObject> winCtrlList = new List<NSObject>();

        public ProgramSettings appSettings = new ProgramSettings();

        string settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Library", "Application Support", appName);

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

        //Clone all hardware device menus from read to write and format
        private void BuildHardwareMenus()
        {
            NSMenuItem newReadItem;
            NSMenuItem newFormatItem;

            foreach (NSMenuItem menuItem in hardReadMenu.Items)
            {
                if (menuItem.Title == "")
                {
                    newReadItem = NSMenuItem.SeparatorItem;
                    newFormatItem = NSMenuItem.SeparatorItem;
                }
                else
                {
                    newReadItem = new NSMenuItem(menuItem.Title);
                    newFormatItem = new NSMenuItem(menuItem.Title);

                    newReadItem.Activated += HardwareItem_Activated;
                    newFormatItem.Activated += HardwareItem_Activated;
                    menuItem.Activated += HardwareItem_Activated;
                }

                hardWriteMenu.AddItem(newReadItem);
                hardFormatMenu.AddItem(newFormatItem);
            }
        }

        //Event for hardware menus
        private void HardwareItem_Activated(object sender, EventArgs e)
        {
            var menuItem = sender as NSMenuItem;

            int deviceId = -1;
            int mode = (int) HardwareInterface.Modes.serial;
            int commMode = (int) HardwareInterface.CommModes.read;

            if (menuItem.ParentItem.Title == "Write save data")
                commMode = (int)HardwareInterface.CommModes.write;

            else if(menuItem.ParentItem.Title == "Format card")
                commMode = (int)HardwareInterface.CommModes.format;

            //Set device id based on menu title
            switch (menuItem.Title)
            {
                case "DexDrive":
                    deviceId = (int)HardwareInterface.Types.dexdrive;
                    break;

                case "MemCARDuino":
                    deviceId = (int)HardwareInterface.Types.memcarduino;
                    break;

                case "PS1CardLink":
                    deviceId = (int)HardwareInterface.Types.ps1cardlink;
                    break;

                case "PS1CardLink over TCP":
                    deviceId = (int)HardwareInterface.Types.ps1cardlink;
                    mode = (int)HardwareInterface.Modes.tcp;
                    break;

                case "Unirom":
                    deviceId = (int)HardwareInterface.Types.unirom;
                    break;

                case "Unirom over TCP":
                    deviceId = (int)HardwareInterface.Types.unirom;
                    mode = (int)HardwareInterface.Modes.tcp;
                    break;

                case "PS3 Memory Card Adaptor":
                    deviceId = (int)HardwareInterface.Types.ps3mca;
                    break;
            }

            //If there are no windows available create a new document
            if (winCtrlList.Count < 1) NewDocument((NSObject)sender);

            var window = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;

            //Activate interface
            window.InitHardwareCommunication(deviceId, mode, commMode);
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            //Show experimental software alert
            var alert = new NSAlert()
            {
                AlertStyle = NSAlertStyle.Critical,
                InformativeText = "This build is highly experimental." +
                "\nAll funcionality may not be implemented yet or work as intended.",
                MessageText = "Warning",
            };
#if DEBUG
#else
            alert.RunModal();
#endif
            //Add initial window controller to the controller list
            winCtrlList.Add(NSApplication.SharedApplication.KeyWindow.WindowController);

            //Register Tab change event for menu manipulation
            NSNotificationCenter.DefaultCenter.AddObserver(NSWindow.DidBecomeMainNotification, TabChangeCallback);

            BuildHardwareMenus();

            appSettings.LoadSettings(settingsPath);
        }

        public override void WillTerminate(NSNotification notification)
        {
            appSettings.SaveSettings(settingsPath, appName, appVersion);
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
        public NSWindowController NewDocument(NSObject sender)
        {
            // Get new window
            var storyboard = NSStoryboard.FromName("Main", null);
            var controller = storyboard.InstantiateControllerWithIdentifier("MainWindow") as NSWindowController;

            //Add controller to list
            winCtrlList.Add(controller);

            // Display
            controller.ShowWindow(this);

            _cardCount++;

            return controller;
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
            window.ToggleDeleteSave(sender);
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

        [Export("exportSave:")]
        void ExportSave(NSObject sender)
        {
            var window = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;
            window.ExportSave(sender);
        }

        [Export("exportSaveRaw:")]
        void ExportSaveRaw(NSObject sender)
        {
            var window = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;
            window.ExportSaveRaw(sender);
        }

        [Export("importSave:")]
        void ImportSave(NSObject sender)
        {
            var window = NSApplication.SharedApplication.KeyWindow.ContentViewController as ViewController;
            window.ImportSave(sender);
        }

        //Open Memory Card dialog
        public void OpenDialog(NSObject sender)
        {
            ps1card memCard = new ps1card();

            NSOpenPanel dlg = new NSOpenPanel();
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
                    string message = memCard.OpenMemoryCard(path, appSettings.FixCorruptedCards == 1);

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
