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

            //Start with a clean MemoryCard
            if(memCard.cardLocation == null) SetMemoryCard(null);
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
                if (memCard.slotType[i] == 1)
                {
                    DataSource.Products.Add(new Product(memCard.saveName[i], memCard.saveProdCode[i], memCard.saveIdentifier[i]));
                }
                else
                {
                    DataSource.Products.Add(new Product("Free slot", "", ""));
                }
            }

            // Populate the Product Table
            CardTable.DataSource = DataSource;
            CardTable.Delegate = new ProductTableDelegate(DataSource);
        }

        //Save file menu item
        [Export("saveDocument:")]
        void SaveDialog(NSObject sender)
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

        //Open file menu item
        /*[Export("openDocument:")]
        void OpenDialog(NSObject sender)
        {
            var dlg = NSOpenPanel.OpenPanel;
            dlg.CanChooseFiles = true;
            dlg.CanChooseDirectories = false;
            dlg.AllowedFileTypes = mcSupportedExtensions;
            //dlg.ExtensionHidden = false;

            if (dlg.RunModal() == 1)
            {
                Console.WriteLine(dlg.Urls.Length);

                // Nab the first file
                var url = dlg.Urls[0];

                if (url != null)
                {
                    var path = url.Path;
                    //Console.WriteLine(path);

                    memCard.OpenMemoryCard(path, false);

                    FillMemcardTable();
                }
            }
        }*/

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
