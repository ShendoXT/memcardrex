using System;
using System.Security.Policy;
using System.Threading.Tasks;
using AppKit;
using CoreGraphics;
using CoreImage;
using Foundation;

namespace MemcardRex
{
    public partial class ViewController : NSViewController
    {
        //Active Memory Card of the current window
        private ps1card memCard = new ps1card();
        private bool firstRun = true;

        public static AppDelegate App
        {
            get { return (AppDelegate)NSApplication.SharedApplication.Delegate; }
        }

        public ViewController (IntPtr handle) : base (handle)
		{

		}

        //Set a new Memory Card for the existing window
        public void SetMemoryCard(string path)
        {
            memCard.OpenMemoryCard(path, false);

            //Set window title (tab) name to Memory Card name
            this.View.Window.Title = memCard.cardName;

            FillMemcardTable();
        }

        //Get memory card path
        public string MemoryCardPath()
        {
            return memCard.cardLocation;
        }

        public override void ViewWillAppear()
        {
            base.ViewWillAppear();

            //Create a blank Memory Card only the first time view is created
            if (firstRun)
            {
                SetMemoryCard(null);
                firstRun = false;
                CardTable.SelectRow(0, false);
            }
        }

        public override void ViewWillDisappear()
        {
            base.ViewWillDisappear();

            App.PopControllerFromList(View.Window.WindowController);
            if (App.CardCount > 0) App.CardCount--;
        }

        //Segue for various dialogs
        public override void PrepareForSegue(NSStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
            nint selectedSlot = memCard.masterSlot[CardTable.SelectedRow];

            // Take action based on the segue name
            switch (segue.Identifier)
            {
                case "EditHeaderSegue":
                    var dialog = segue.DestinationController as HeaderDialogController;
                    dialog.DialogTitle = memCard.saveName[selectedSlot];

                    dialog.ProductCode = memCard.saveProdCode[selectedSlot];
                    dialog.IdentifierString = memCard.saveIdentifier[selectedSlot];
                    dialog.Region = memCard.saveRegion[selectedSlot];

                    dialog.DialogAccepted += (s, e) => {

                        //Insert data to save header of the selected save slot
                        memCard.SetHeaderData((int) selectedSlot, dialog.ProductCode, dialog.IdentifierString, dialog.Region);

                        //Refresh table
                        FillMemcardTable();

                        View.Window.DocumentEdited = true;
                    };

                    dialog.Presentor = this;
                    break;

                case "EditCommentSegue":
                    var sheet = segue.DestinationController as CommentDialogController;

                    sheet.Comment = memCard.saveComments[selectedSlot];


                    sheet.DialogAccepted += (s, e) => {
                        memCard.saveComments[selectedSlot] = sheet.Comment;
                        View.Window.DocumentEdited = true;
                    };
                    sheet.Presentor = this;
                    break;

                case "GetInfoSegue":
                    var infoSheet = segue.DestinationController as InfoDialogController;

                    infoSheet.DialogTitle = "Save information";

                    infoSheet.SaveTitle = memCard.saveName[selectedSlot];
                    infoSheet.ProductCode = memCard.saveProdCode[selectedSlot];
                    infoSheet.IdentifierString = memCard.saveIdentifier[selectedSlot];
                    infoSheet.Region = memCard.saveRegion[selectedSlot];
                    infoSheet.Size = memCard.saveSize[selectedSlot];
                    infoSheet.Frames = memCard.iconFrames[selectedSlot];
                    infoSheet.Slots = memCard.FindSaveLinks((int)selectedSlot);
                    infoSheet.Icons = memCard.iconColorData;

                    infoSheet.Presentor = this;
                    break;
            }
        }

        public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

            //Add event listener for double click on the card list
            CardTable.DoubleClick += CardTable_DoubleClick;
        }

        //Enable or disable menu items based on the currently selected save
        private void SetEditItemsState(bool itemStates)
        {
            //Context menu
            editSaveHeaderItem.Enabled = itemStates;
            editSaveCommentItem.Enabled = itemStates;
            compareSaveItem.Enabled = itemStates;
            editIconItem.Enabled = itemStates;
            deleteSaveItem.Enabled = itemStates;
            restoreSaveItem.Enabled = itemStates;
            removeSaveItem.Enabled = itemStates;
            cpyTempBufferItem.Enabled = itemStates;
            pasteTempBufferItem.Enabled = itemStates;
            importSaveItem.Enabled = itemStates;
            exportSaveItem.Enabled = itemStates;
            exportSaveRawItem.Enabled = itemStates;
            getInfoItem.Enabled = itemStates;

            //App menu
            App.SetEditItemsState(itemStates);
        }

        //User changed selected slot
        public void SelectionDidChange()
        {
            var window = View.Window.WindowController as WindowController;

            //Disable all edit items
            SetEditItemsState(false);

            //Disable undo/redo
            App.SetUndoItem(false);
            App.SetRedoItem(false);

            //If nothing is selected abort
            if (CardTable.SelectedRow < 0) return;

            //Enable undo/redo based on buffers
            if (memCard.UndoCount > 0) App.SetUndoItem(true);
            if (memCard.RedoCount > 0) App.SetRedoItem(true);

            //Enable menu items based on the content
            switch (memCard.slotType[memCard.masterSlot[CardTable.SelectedRow]])
            {
                case (int)ps1card.SlotTypes.formatted:
                    importSaveItem.Enabled = true;
                    if (App.TempBuffer != null)
                    {
                        pasteTempBufferItem.Enabled = true;
                        App.SetPasteFromBuffer(true);
                        //window.SetPasteFromBufferToolbar(true);
                    }
                    App.SetImportMenuItem(true);
                    break;

                case (int)ps1card.SlotTypes.initial:
                    SetEditItemsState(true);

                    restoreSaveItem.Enabled = false;
                    importSaveItem.Enabled = false;
                    pasteTempBufferItem.Enabled = false;
                    App.SetImportMenuItem(false);
                    App.DisableRestoreItem();
                    App.SetPasteFromBuffer(false);
                    break;

                case (int)ps1card.SlotTypes.deleted_initial:
                    SetEditItemsState(true);

                    deleteSaveItem.Enabled = false;
                    importSaveItem.Enabled = false;
                    pasteTempBufferItem.Enabled = false;
                    App.SetImportMenuItem(false);
                    App.DisableDeleteItem();
                    App.SetPasteFromBuffer(false);
                    break;

                case (int)ps1card.SlotTypes.corrupted:
                    //Enable only formating of the slot
                    removeSaveItem.Enabled = true;
                    App.EnableFormatMenuItem();
                    break;
            }

            //Select all slots on the save
            foreach (int saveSlot in memCard.FindSaveLinks(memCard.masterSlot[CardTable.SelectedRow]))
            {
                CardTable.SelectRow(saveSlot, true);
            }
        }

        //Populate table with data from the MemoryCard
        private void FillMemcardTable()
        {
            nint selectedSlot = CardTable.SelectedRow;

            var DataSource = new ProductTableDataSource();

            for (int i = 0; i < ps1card.SlotCount; i++)
            {
                switch (memCard.slotType[i])
                {
                    case (int) ps1card.SlotTypes.deleted_initial:
                    case (int) ps1card.SlotTypes.deleted_middle_link:
                    case (int) ps1card.SlotTypes.deleted_end_link:
                        DataSource.Products.Add(new Product(memCard.saveName[i], memCard.saveProdCode[i], memCard.saveIdentifier[i], memCard.iconColorData[i, 0], memCard.saveRegion[i], true));
                        break;

                    default:
                        DataSource.Products.Add(new Product(memCard.saveName[i], memCard.saveProdCode[i], memCard.saveIdentifier[i], memCard.iconColorData[i, 0], memCard.saveRegion[i], false));
                        break;

                    case (byte) ps1card.SlotTypes.formatted:
                        DataSource.Products.Add(new Product("Free slot"));
                        break;

                    case (byte) ps1card.SlotTypes.corrupted:
                        DataSource.Products.Add(new Product("Corrupted slot"));
                        break;
                }
            }

            // Populate the Product Table
            CardTable.DataSource = DataSource;
            CardTable.Delegate = new ProductTableDelegate(DataSource, SelectionDidChange);

            CardTable.RowHeight = 18;

            CardTable.ToolTip = "";
            CardTable.RemoveAllToolTips();

            //Restore selected slot
            CardTable.SelectRow(selectedSlot, false);
        }

        //List view double click event handler
        private void CardTable_DoubleClick(object sender, EventArgs e)
        {
            GetSaveInfo((NSObject)sender);
        }

        //Check if a valid save slot is selected
        private bool CheckSelectionValidity()
        {
            //If no slot is selected abort
            if (CardTable.SelectedRow < 0) return false;

            int selectedSlot = memCard.masterSlot[CardTable.SelectedRow];

            //Only edit valid initial slots
            return (memCard.slotType[selectedSlot] == (int)ps1card.SlotTypes.initial ||
                memCard.slotType[selectedSlot] == (int)ps1card.SlotTypes.deleted_initial);
        }

        //Run specified dialog using checks
        private void RunDialog(string dialogName, NSObject sender)
        {
            //Call edit header dialog
            if(CheckSelectionValidity()) PerformSegue(dialogName, sender);
        }

        public void Undo()
        {
            if (memCard.Undo())
            {
                View.Window.DocumentEdited = true;
                FillMemcardTable();
                SelectionDidChange();
            }
        }

        public void Redo()
        {
            if (memCard.Redo())
            {
                View.Window.DocumentEdited = true;
                FillMemcardTable();
                SelectionDidChange();
            }
        }

        //Copy save to temp buffer
        [Action("copyToTempBuffer:")]
        public void CopyToTempBuffer(NSObject sender)
        {
            int selectedSlot = memCard.masterSlot[CardTable.SelectedRow];
            var window = View.Window.WindowController as WindowController;

            //Build icon for toolbar
            BmpBuilder builder = new BmpBuilder();
            NSData iconImage = NSData.FromArray(builder.BuildBmp(memCard.iconColorData[selectedSlot, 0]));
            NSImage icon = new NSImage(iconImage);
            icon.Flipped = true;

            App.TempBuffer = memCard.GetSaveBytes(selectedSlot);
            App.BufferImage = icon;

            try
            {
                //Enable toolbar item
                window.SetPasteFromBufferToolbar(icon, true);
            }catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Paste save from temp buffer
        [Action("pasteFromTempBuffer:")]
        public void PasteFromTempBuffer(NSObject sender)
        {
            if (CardTable.SelectedRow < 0) return;

            int selectedSlot = memCard.masterSlot[CardTable.SelectedRow];
            int requiredSlots = 0;

            if (App.TempBuffer == null || selectedSlot < 0) return;
            if (memCard.slotType[selectedSlot] != (int)ps1card.SlotTypes.formatted) return;

            if(memCard.SetSaveBytes(selectedSlot, App.TempBuffer, out requiredSlots))
            {
                View.Window.DocumentEdited = true;
                FillMemcardTable();
            }
            else
            {
                Console.WriteLine("Failed");
            }
        }

        //Import save
        [Action("importSave:")]
        public void ExportSave(NSObject sender)
        {
            this.View.Window.WindowController = null;
            Console.WriteLine("Important");
        }

        //Show save info
        [Action("getInfo:")]
        public void GetSaveInfo(NSObject sender)
        {
            RunDialog("GetInfoSegue", sender);
        }

        //Edit save comment
        [Action("editSaveComment:")]
        public void EditSaveComment(NSObject sender)
        {
            //Call edit header dialog
            RunDialog("EditCommentSegue", sender);
        }

        //Edit save headwer
        [Action("editHeader:")]
        public void EditHeader(NSObject sender)
        {
            RunDialog("EditHeaderSegue", sender);
        }

        //Save file menu item
        [Export("saveDocument:")]
        public void SaveDialog(NSObject sender)
        {
            SaveAsDialog(sender);
        }

        //Block operations
        [Export("deleteSave:")]
        public void DeleteSave(NSObject sender)
        {
            //If no slot is selected abort
            if (CardTable.SelectedRow < 0) return;

            memCard.ToggleDeleteSave(memCard.GetMasterLinkForSlot((int)CardTable.SelectedRow));
            FillMemcardTable();
            View.Window.DocumentEdited = true;
        }

        [Export("restoreSave:")]
        public void RestoreSave(NSObject sender)
        {
            //We are just toggling delete/undelete state
            DeleteSave(sender);
        }

        [Export("removeSave:")]
        public void RemoveSave(NSObject sender)
        {
            //If no slot is selected abort
            if (CardTable.SelectedRow < 0) return;

            int selectedSlot = memCard.GetMasterLinkForSlot((int)CardTable.SelectedRow);

            memCard.FormatSave(selectedSlot);
            FillMemcardTable();
            View.Window.DocumentEdited = true;

            //Leave slot selected (initial slot in case of multi slot saves)
            CardTable.SelectRow(selectedSlot, false);
        }

        //Save as file menu item
        [Export("saveDocumentAs:")]
        void SaveAsDialog(NSObject sender)
        {
            var dlg = new NSSavePanel();
            dlg.AllowedFileTypes = memCard.SupportedExtensions;
            dlg.Title = "Save Memory Card";
            dlg.NameFieldStringValue = memCard.cardName;
            dlg.ExtensionHidden = false;

            //Available file types
            var popupButton = new NSPopUpButton(new CGRect(60, 0, 246, 22), false);
            popupButton.AddItems(new string[]{ "Standard Memory Card (*.mcr, *.mcd, ...)", "PSP/Vita Memory Card (*.vmp)",
            "PS Vita \"MCX\" Memory Card (*.bin)", "DexDrive Memory Card (*.gme)",
            "VGS Memory Card (*.vgs, *.mem)"});

            var popupLabel = new NSTextField(new CGRect(0, -2, 60, 22));
            popupLabel.StringValue = "File type:";
            popupLabel.Editable = false;
            popupLabel.Bordered = false;
            popupLabel.DrawsBackground = false;
            popupLabel.Font = NSFont.SystemFontOfSize(11);
            popupLabel.TextColor = NSColor.SecondaryLabel;

            //Accessory view for file type chooser
            var accessoryView = new NSView(new CGRect(0, 0, 315, 24));
            accessoryView.AddSubview(popupLabel);
            accessoryView.AddSubview(popupButton);

            //On file type change event
            popupButton.Activated += (object snder, EventArgs e) =>
            {
                switch (popupButton.IndexOfSelectedItem)
                {
                    default:        //raw
                        dlg.AllowedFileTypes = new string[] { "mcr", "bin", "ddf", "mc", "mcd", "mci", "ps", "psm", "srm", "vm1", "vmc" };
                        break;

                    case 1:         //vmp
                        dlg.AllowedFileTypes = new string[] { "vmp" };
                        break;

                    case 2:         //mcx
                        dlg.AllowedFileTypes = new string[] { "bin" };
                        break;

                    case 3:         //gme
                        dlg.AllowedFileTypes = new string[] { "gme" };
                        break;

                    case 4:         //vgs
                        dlg.AllowedFileTypes = new string[] { "vgs", "mem" };
                        break;
                }
            };

            dlg.AccessoryView = accessoryView;

            if (dlg.RunModal() == 1)
            {
                var url = dlg.Url;
                int cardType = (int)ps1card.CardTypes.raw;

                switch (url.PathExtension.ToLower())
                {
                    case "gme":
                        cardType = (int)ps1card.CardTypes.gme;
                        break;

                    case "vgs":
                        cardType = (int)ps1card.CardTypes.vgs;
                        break;

                    case "bin":
                        if(popupButton.IndexOfSelectedItem == 2) cardType = (int)ps1card.CardTypes.mcx;
                        break;

                    case "vmp":
                        cardType = (int)ps1card.CardTypes.vmp;
                        break;
                }

                if(memCard.SaveMemoryCard(url.Path.ToString(), cardType, false))
                {
                    View.Window.Title = memCard.cardName;
                    View.Window.DocumentEdited = false;
                }
            }
            dlg.Dispose();
        }

        public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
	}
}
