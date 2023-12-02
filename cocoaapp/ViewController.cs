using System;

using AppKit;
using Foundation;

namespace MemcardRex
{
    public partial class ViewController : NSViewController
    {
        //Active Memory Card of the current window
        private ps1card memCard = new ps1card();

        public bool isCardWindow = false;

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

            //Refresh app menu for the current view
            //SelectionDidChange();

            //this.ViewDidAppear
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
                    };

                    dialog.Presentor = this;
                    break;

                    case "EditCommentSegue":
                        var sheet = segue.DestinationController as CommentDialogController;

                        sheet.Comment = memCard.saveComments[selectedSlot];


                        sheet.DialogAccepted += (s, e) => {
                            memCard.saveComments[selectedSlot] = sheet.Comment;
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
            //Disable all edit items
            SetEditItemsState(false);

            //If nothing is selected abort
            if (CardTable.SelectedRow < 0) return;

            //Enable menu items based on the content
            switch (memCard.slotType[memCard.masterSlot[CardTable.SelectedRow]])
            {
                case (int)ps1card.SlotTypes.formatted:
                    importSaveItem.Enabled = true;
                    App.SetImportMenuItem(true);
                    break;

                case (int)ps1card.SlotTypes.initial:
                    SetEditItemsState(true);

                    restoreSaveItem.Enabled = false;
                    importSaveItem.Enabled = false;
                    App.SetImportMenuItem(false);
                    App.DisableRestoreItem();
                    break;

                case (int)ps1card.SlotTypes.deleted_initial:
                    SetEditItemsState(true);

                    deleteSaveItem.Enabled = false;
                    importSaveItem.Enabled = false;
                    App.SetImportMenuItem(false);
                    App.DisableDeleteItem();
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
                    case (int)ps1card.SlotTypes.deleted_middle_link:
                    case (int)ps1card.SlotTypes.deleted_end_link:
                        DataSource.Products.Add(new Product(memCard.saveName[i], memCard.saveProdCode[i], memCard.saveIdentifier[i], memCard.iconColorData[i, 0], memCard.saveRegion[i], true));
                        break;

                    default:
                        DataSource.Products.Add(new Product(memCard.saveName[i], memCard.saveProdCode[i], memCard.saveIdentifier[i], memCard.iconColorData[i, 0], memCard.saveRegion[i], false));
                        break;

                    case (byte)ps1card.SlotTypes.formatted:
                        DataSource.Products.Add(new Product("Free slot"));
                        break;

                    case (byte)ps1card.SlotTypes.corrupted:
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

            memCard.FormatSave(memCard.GetMasterLinkForSlot((int)CardTable.SelectedRow));
            FillMemcardTable();
        }

        //Save as file menu item
        [Export("saveDocumentAs:")]
        void SaveAsDialog(NSObject sender)
        {
            var dlg = NSSavePanel.SavePanel;
            dlg.AllowedFileTypes = dlg.AllowedFileTypes = memCard.SupportedExtensions;
            dlg.NameFieldStringValue = memCard.cardName;


            if (dlg.RunModal() == 1)
            {
                var url = dlg.Url;

                Console.WriteLine(url.Path.ToString());

                switch (url.PathExtension.ToLower()) {

                    default:
                        memCard.SaveMemoryCard(url.Path.ToString(), (int) ps1card.CardTypes.raw , false);
                        break;

                    case "gme":
                        memCard.SaveMemoryCard(url.Path.ToString(), (int)ps1card.CardTypes.gme, false);
                        break;

                    case "vgs":
                        memCard.SaveMemoryCard(url.Path.ToString(), (int)ps1card.CardTypes.vgs, false);
                        break;

                    case "mcx":
                        memCard.SaveMemoryCard(url.Path.ToString(), (int)ps1card.CardTypes.mcx, false);
                        break;

                    case "vmp":
                        memCard.SaveMemoryCard(url.Path.ToString(), (int)ps1card.CardTypes.vmp, false);
                        break;
                }

            }

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
