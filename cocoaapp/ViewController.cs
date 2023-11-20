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

        public ViewController (IntPtr handle) : base (handle)
		{

		}

        //Set a new Memory Card for the existing window
        public void SetMemoryCard(string path)
        {
            memCard.OpenMemoryCard(path, false);

            //this.View.Window.ContentViewController.f
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
            }
        }

        public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
        }

        //Populate table with data from the MemoryCard
        private void FillMemcardTable()
        {
            var DataSource = new ProductTableDataSource();

            for (int i = 0; i < ps1card.SlotCount; i++)
            {
                switch (memCard.slotType[i])
                {
                    default:
                        DataSource.Products.Add(new Product(memCard.saveName[i], memCard.saveProdCode[i], memCard.saveIdentifier[i], memCard.iconColorData[i, 0], memCard.saveRegion[i]));
                        break;

                    /*case (byte)ps1card.SlotTypes.initial:
                    case (byte)ps1card.SlotTypes.deleted_initial:

                        break;

                    case (byte)ps1card.SlotTypes.middle_link:
                    case (byte)ps1card.SlotTypes.deleted_middle_link:
                        DataSource.Products.Add(new Product("Linked slot (middle link)"));
                        break;

                    case (byte)ps1card.SlotTypes.end_link:
                    case (byte)ps1card.SlotTypes.deleted_end_link:
                        DataSource.Products.Add(new Product("Linked slot (end link)"));
                        break;*/

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
            CardTable.Delegate = new ProductTableDelegate(DataSource);

            CardTable.RowHeight = 18;

            CardTable.ToolTip = "";
            CardTable.RemoveAllToolTips();

            //CardTable.AllowsMultipleSelection = true;
        }

        //Save file menu item
        [Export("saveDocument:")]
        public void SaveDialog(NSObject sender)
        {
            SaveAsDialog(sender);
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
