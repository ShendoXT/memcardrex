//Main window of the MemcardRex application
//Shendo 2009 - 2024

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MemcardRex.Windows.GUI;
using System.Diagnostics;
using MemcardRex.Core;
using System.Runtime.Versioning;

namespace MemcardRex
{
    [SupportedOSPlatform("windows")]
    public partial class mainWindow : Form
    {
        //Application related strings
        const string appName = "MemcardRex";
        const string appDate = "Unknown";

#if DEBUG
        const string appVersion = "2.0 alpha (Debug)";
#else
        const string appVersion = "2.0 alpha";
#endif
        //All available application settings
        public ProgramSettings appSettings = new ProgramSettings();

        //Location of the application
        string appPath = Application.StartupPath;

        //Plugin system (public because plugin dialog has to access it)
        public rexPluginSystem pluginSystem = new rexPluginSystem();

        //Supported plugins for the currently selected save
        int[] supportedPlugins = null;

        //Currently clicked plugin (0 - clicked flag, 1 - plugin index)
        int[] clickedPlugin = new int[]{0,0};

        public double xScale = 1.0;
        public double yScale = 1.0;

        //List of opened Memory Cards
        List<ps1card> PScard = new List<ps1card>();

        //Listview of the opened Memory Cards
        List<CardListView> cardList = new List<CardListView>();

        //History listview
        List<ListView> historyList = new List<ListView>();

        //List of icons for the saves
        List<ImageList> iconList = new List<ImageList>();

        //List of icons for the history list
        List<ImageList> historyIconList = new List<ImageList>();

        public struct HardInterfaces
        {
            public HardwareInterface hardwareInterface;
            public HardwareInterface.Modes mode;
            public string displayName;
        }

        //Registered hardware interfaces
        List<HardInterfaces> registeredInterfaces = new List<HardInterfaces>();

        //Currently active interface
        private HardInterfaces activeInterface
        {
            get
            {
                return registeredInterfaces[appSettings.ActiveInterface];
            }
        }

        //Currently active Memory Card
        private ps1card memCard
        {
            get { if (PScard.Count == 0) return new ps1card();
                return PScard[mainTabControl.SelectedIndex];
            }
        }

        //Temp buffer used to store saves
        byte[] tempBuffer = null;
        string tempBufferName = null;

        //Supported Memory Card extensions
        private const string mcSupportedExtensions = "All supported|*.bin;*.ddf;*.gme;*.mc;*.mcd;*.mci;*.mcr;*.mem;*.ps;*.psm;*.srm;*.vgs;*.vm1;*.vmp;*.vmc";
        private const string mcExtensions = "Standard Memory Card|*.mcr;*.bin;*.ddf;*.mc;*.mcd;*.mci;*.ps;*.psm;*.srm;*.vm1;*.vmc|PSP/Vita Memory Card|*.VMP|PS Vita \"MCX\" PocketStation Memory Card|*.BIN|DexDrive Memory Card|*.gme|VGS Memory Card|*.mem;*.vgs";

        //Supported single save extensions
        private const string ssSupportedExtensions = "All supported|*.mcs;*.ps1;*.PSV;*.mcb;*.mcx;*.pda;*.psx;B???????????*";
        private const string ssExtensions = "PSXGameEdit/Memory Juggler|*.mcs;*.ps1|PS3 single save|*.PSV|Smart Link/XP, AR, GS, Caetla/Datel|*.mcb;*.mcx;*.pda;*.psx";

        public mainWindow()
        {
            InitializeComponent();
            using (Graphics graphics = CreateGraphics())
            {
                xScale = graphics.DpiX / 96.0;
                yScale = graphics.DpiY / 96.0;
            }

            BuildToolbarIcons();

            //Create hardware menus
            BuildHardwareMenus();

            //Beginning of Dark Theme implementation
            ApplyTheme(Theme.Dark);
        }

        //Register hardware interfaces
        private void RegisterInterface(HardwareInterface hardInterface, HardwareInterface.Modes mode)
        {
            HardInterfaces regInterface = new HardInterfaces();
            regInterface.hardwareInterface = hardInterface;
            regInterface.mode = mode;
            regInterface.displayName = hardInterface.Name();

            //Append via TCP if interface mode is tcp
            if(mode == HardwareInterface.Modes.tcp) regInterface.displayName += " via TCP";

            registeredInterfaces.Add(regInterface);
        }
        
        private void AttachInterface(HardwareInterface hardInterface)
        {
            //Serial always available
            RegisterInterface(hardInterface, HardwareInterface.Modes.serial);

            //Check if interface supports TCP mode
            if((hardInterface.Features() & HardwareInterface.SupportedFeatures.TcpMode) > 0)
                RegisterInterface(hardInterface, HardwareInterface.Modes.tcp);
        }

        //Add all available hardware interfaces
        private void BuildHardwareMenus()
        {
            AttachInterface(new DexDrive());
            AttachInterface(new MemCARDuino());
            AttachInterface(new PS1CardLink());
            AttachInterface(new Unirom());
            AttachInterface(new PS3MemCardAdaptor());

            EnableDisableHardwareMenus();
        }

        private void EnableDisableHardwareMenus()
        {
            //Set currently active interface to hardware menu
            hardwareToolStripMenuItem.Text = activeInterface.hardwareInterface.Name();

            //Add TCP if TCP interface
            if (activeInterface.mode == HardwareInterface.Modes.tcp) hardwareToolStripMenuItem.Text += " (TCP)";

            //Enable or disable realtime and PocketStation menus
            realtimeConnectionToolStripMenuItem.Enabled = ((activeInterface.hardwareInterface.Features() & HardwareInterface.SupportedFeatures.RealtimeMode) > 0);
            pocketStationToolStripMenuItem.Enabled = ((activeInterface.hardwareInterface.Features() & HardwareInterface.SupportedFeatures.PocketStation) > 0);
        }

        private void HardwareItem_Activated(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem == null) return;

            //Read data
            if(menuItem == readFromToolStripMenuItem)
                activeInterface.hardwareInterface.CommMode = HardwareInterface.CommModes.read;

            //Write data
            if (menuItem == writeToToolStripMenuItem)
                activeInterface.hardwareInterface.CommMode = HardwareInterface.CommModes.write;

            //Format
            if (menuItem == formatMemoryCardToolStripMenuItem)
                activeInterface.hardwareInterface.CommMode = HardwareInterface.CommModes.format;

            //Realtime link
            if (menuItem == realtimeConnectionToolStripMenuItem)
                activeInterface.hardwareInterface.CommMode = HardwareInterface.CommModes.realtime;

            //Read PocketStation serial
            if (menuItem == readSerialToolStripMenuItem)
                activeInterface.hardwareInterface.CommMode = HardwareInterface.CommModes.psinfo;

            //Dump PocketSTation BIOS
            if (menuItem == dumpBIOSToolStripMenuItem)
                activeInterface.hardwareInterface.CommMode = HardwareInterface.CommModes.psbios;

            //Set PocketStation time
            if (menuItem == setDateTimeToolStripMenuItem)
                activeInterface.hardwareInterface.CommMode = HardwareInterface.CommModes.pstime;

            //Set serial or TCP mode
            activeInterface.hardwareInterface.Mode = activeInterface.mode;

            //Activate interface
            InitHardwareCommunication(activeInterface.hardwareInterface);
        }

        //Communication with real device
        public void InitHardwareCommunication(HardwareInterface hardInterface)
        {
            //Abort if the interface is not valid
            if (hardInterface == null) return;

            //Set card slot
            hardInterface.CardSlot = appSettings.CardSlot;

            cardReaderWindow cardReader = new cardReaderWindow(hardInterface);

            //Update setings
            cardReader.ComPort = appSettings.CommunicationPort;
            cardReader.RemoteCommAddress = appSettings.RemoteCommAddress;
            cardReader.RemoteCommPort = appSettings.RemoteCommPort;

            //If the data is to be written fetch the current raw Memory Card data
            if (hardInterface.CommMode == HardwareInterface.CommModes.write)
                cardReader.MemoryCard = memCard.SaveMemoryCardStream(appSettings.FixCorruptedCards == 1);

            //Create a new blank formatted Memory Card and write it to device
            else if (hardInterface.CommMode == HardwareInterface.CommModes.format)
            {
                ps1card blankCard = new ps1card();
                blankCard.OpenMemoryCard(null, true);
                cardReader.MemoryCard = blankCard.SaveMemoryCardStream(true);
            }

            cardReader.QuickFormat = appSettings.FormatType == 0;

            //Read complete event
            cardReader.ReadingComplete += (s, e) =>
            {
                if (hardInterface.CommMode == HardwareInterface.CommModes.read)
                    cardReaderRead(cardReader.MemoryCard, hardInterface.Name());
            };

            cardReader.ShowDialog();

            //Check if any errors occured and display them
            if(cardReader.ErrorMessage != null)
            {
                MessageBox.Show(cardReader.ErrorMessage, "Unable to start " + hardInterface.Name(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //If this was a serial read display result
            if(hardInterface.CommMode == HardwareInterface.CommModes.psinfo)
            {
                new pocketStationInfo().ShowSerial(cardReader.PocketSerial);
            }

            //If this was a BIOS read display it in dialog
            if(hardInterface.CommMode == HardwareInterface.CommModes.psbios && cardReader.OperationCompleted)
            {
                new pocketStationInfo().ShowBios(cardReader.PocketSerial, cardReader.PocketBIOS);
            }

            //If this was time command display confirmation dialog
            if (hardInterface.CommMode == HardwareInterface.CommModes.pstime)
            {
                MessageBox.Show("Time set successfully", "PocketStation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //Backup a Memory Card
        private void backupMemcard(string fileName)
        {
            //Check if backuping of memcard is allowed and the filename is valid
            if (appSettings.BackupMemcards == 1 && fileName != null)
            {
                FileInfo fInfo = new FileInfo(fileName);

                //Backup only if file is less then 512KB
                if (fInfo.Length < 524288)
                {
                    //Copy the file
                    try
                    {
                        //Check if the backup directory exists and create it if it's missing
                        if (!Directory.Exists(appPath + "/Backup")) Directory.CreateDirectory(appPath + "/Backup");

                        //Copy the file (make a backup of it)
                        File.Copy(fileName, appPath + "/Backup/" + fInfo.Name);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        //Remove the first "Untitled" card if the user opened a valid card
        private void filterNullCard()
        {
            //Check if there are any cards opened
            if (PScard.Count > 0)
            {
                if (PScard.Count == 2 && PScard[0].cardLocation == null && PScard[0].changedFlag == false)
                {
                    closeCard(0);
                }
            }
        }

        //Open a Memory Card with OpenFileDialog
        private void openCardDialog()
        {
            OpenFileDialog openFileDlg = new OpenFileDialog
            {
                Title = "Open Memory Card",
                Filter = mcSupportedExtensions + "|" + mcExtensions + " | All files|*.*",
                Multiselect = true
            };

            //If user selected a card open it
            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in openFileDlg.FileNames)
                {
                    openCard(fileName);
                }
            }
        }

        //Open a Memory Card from the given filename
        private void openCard(string fileName)
        {
            //Container for the error message
            string errorMsg = null;

            //Check if the card already exists
            foreach (ps1card checkCard in PScard)
            {
                if (checkCard.cardLocation == fileName && fileName != null)
                {
                    //Card is already opened, bring it to front
                    mainTabControl.SelectedIndex = PScard.IndexOf(checkCard);
                    return;
                }
            }

            //Create a new card
            PScard.Add(new ps1card());

            //Try to open card
            errorMsg = PScard[PScard.Count - 1].OpenMemoryCard(fileName, appSettings.FixCorruptedCards == 1);

            //If card is sucesfully opened proceed further, else destroy it
            if (errorMsg == null)
            {
                //Backup opened card
                backupMemcard(fileName);

                //Make a new tab for the opened card
                createTabPage();

                //Show event in the history tab
                if(fileName != null) pushHistory("Card opened", historyList.Count - 1, new Bitmap(16, 16));
                else pushHistory("Card created", historyList.Count - 1, new Bitmap(16, 16));
            }
            else
            {
                //Remove the last card created
                PScard.RemoveAt(PScard.Count-1);

                //Display error message
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Create a new tab page for the Memory Card
        private void createTabPage()
        {
            //Make new tab page
            TabPage tabPage = new TabPage();

            //Add a tab corresponding to opened card
            mainTabControl.TabPages.Add(tabPage);

            //Make a new ListView control
            makeListView();

            //Add ListView control to the tab page
            tabPage.Controls.Add(cardList[cardList.Count - 1]);

            //Add history listview to tab page
            tabPage.Controls.Add(historyList[historyList.Count - 1]);

            //Delete the initial "Untitled" card
            if (PScard[PScard.Count - 1].cardLocation != null) filterNullCard();

            //Switch the active tab to the currently opened card
            mainTabControl.SelectedIndex = PScard.Count - 1;

            //Show the location of the card in the tool strip
            refreshStatusStrip();

            //Enable "Close", "Close All", "Save" and "Save as" menu items
            closeToolStripMenuItem.Enabled = true;
            closeAllToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
            saveButton.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;

            //Select first save in the list
            cardList[cardList.Count - 1].Items[0].Selected = true;
        }

        //Save a Memory Card with SaveFileDialog
        private void saveCardDialog(int listIndex)
        {
            //Check if there are any cards to save
            if (PScard.Count > 0)
            {
                ps1card.CardTypes memoryCardType;
                SaveFileDialog saveFileDlg = new SaveFileDialog
                {
                    Title = "Save Memory Card",
                    Filter = mcExtensions,
                    FilterIndex = appSettings.LastSaveFormat
                };

                //If user selected a card save to it
                if (saveFileDlg.ShowDialog() == DialogResult.OK)
                {
                    if (saveFileDlg.FilterIndex != appSettings.LastExportFormat)
                    {
                        appSettings.LastSaveFormat = saveFileDlg.FilterIndex;
                        appSettings.SaveSettings(appPath, appName, appVersion);
                    }
                    //Get save type
                    switch (saveFileDlg.FilterIndex)
                    {
                        default:        //Raw Memory Card
                            memoryCardType = ps1card.CardTypes.raw;
                            break;

                        case 2:         //VMP Memory Card
                            memoryCardType = ps1card.CardTypes.vmp;
                            break;

                        case 3:         //MCX Memory Card
                            memoryCardType = ps1card.CardTypes.mcx;
                            break;

                        case 4:         //GME Memory Card
                            memoryCardType = ps1card.CardTypes.gme;
                            break;

                        case 5:         //VGS Memory Card
                            memoryCardType = ps1card.CardTypes.vgs;
                            break;

                    }
                    saveMemoryCard(listIndex, saveFileDlg.FileName, memoryCardType);
                }
            }
        }

        //Save a Memory Card to a given filename
        private void saveMemoryCard(int listIndex, string fileName, ps1card.CardTypes memoryCardType)
        {
            if (PScard[listIndex].SaveMemoryCard(fileName, memoryCardType, appSettings.FixCorruptedCards == 1))
            {
                refreshListView(listIndex, cardList[listIndex].SelectedIndices[0]);
                refreshStatusStrip();
            }
            else
                MessageBox.Show("Memory Card could not be saved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //Save a selected Memory Card
        private void saveCardFunction(int listIndex)
        {
            //Check if there are any cards to save
            if (PScard.Count > 0)
            {
                //Check if file can be saved or save dialog must be shown
                if (PScard[listIndex].cardLocation == null)
                    saveCardDialog(listIndex);
                else
                    saveMemoryCard(listIndex, PScard[listIndex].cardLocation, PScard[listIndex].cardType);
            }
        }

        //Cleanly close the selected card
        private int closeCard(int listIndex, bool switchToFirst)
        {
            //Check if there are any cards to delete
            if (PScard.Count > 0)
            {
                //Check if the file has been changed
                if (PScard[listIndex].changedFlag)
                {
                    //Ask for saving before closing
                    DialogResult result = MessageBox.Show("Do you want to save changes to '" + PScard[listIndex].cardName + "'?", appName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                        saveCardFunction(listIndex);
                    else if (result == DialogResult.Cancel)
                        return 1;
                }

                PScard.RemoveAt(listIndex);
                cardList.RemoveAt(listIndex);
                historyList.RemoveAt(listIndex);
                iconList.RemoveAt(listIndex);
                historyIconList.RemoveAt(listIndex);
                mainTabControl.RemoveTabPrepare();
                mainTabControl.TabPages.RemoveAt(listIndex);

                //Select first tab
                if (PScard.Count > 0 && switchToFirst)
                    mainTabControl.SelectedIndex = 0;

                //Refresh plugin list
                refreshPluginBindings();

                //Enable certain list items
                enableSelectiveEditItems();
            }

            //If this was the last card disable "Close", "Close All", "Save" and "Save as" menu items
            if (PScard.Count <= 0)
            {
                closeToolStripMenuItem.Enabled = false;
                closeAllToolStripMenuItem.Enabled = false;
                saveToolStripMenuItem.Enabled = false;
                saveButton.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;
            }
            return 0;
        }

        //Overload for closeCard function
        private int closeCard(int listIndex)
        {
            return closeCard(listIndex, true);
        }

        //Close all opened cards
        private int closeAllCards()
        {
            //Run through the loop as long as there are cards opened
            while (PScard.Count > 0)
            {
                mainTabControl.SelectedIndex = 0;
                if (closeCard(0) == 1)
                    return 1;
            }
            return 0;
        }

        //Edit save comments
        private void editSaveComments()
        {
            if (!validityCheck(out int listIndex, out int slotNumber)) return;

            int slotNum = memCard.GetMasterLinkForSlot(slotNumber);

            commentsWindow commentsDlg = new commentsWindow();

            //Load values to dialog
            commentsDlg.initializeDialog(memCard.saveName[slotNum], memCard.saveComments[slotNum]);
            commentsDlg.ShowDialog(this);

            //Update values if OK was pressed
            if (commentsDlg.okPressed)
            {
                //Insert edited comments back in the card
                memCard.SetComment(slotNum, commentsDlg.saveComment);
                pushHistory("Comment edited", mainTabControl.SelectedIndex, prepareIcons(listIndex, slotNum, false));

                refreshListView(listIndex, slotNumber);
            }
            commentsDlg.Dispose();
        }

        //Create and show information dialog
        private void showInformation()
        {
            if (!validityCheck(out int listIndex, out int slotNumber)) return;

            informationWindow informationDlg = new informationWindow();
            int masterSlot = memCard.GetMasterLinkForSlot(slotNumber);

            //Only show info for valid saves
            if (!(memCard.slotType[masterSlot] == ps1card.SlotTypes.initial || 
                memCard.slotType[masterSlot] == ps1card.SlotTypes.deleted_initial)) return;

            //Load values to dialog
            informationDlg.initializeDialog(memCard.saveName[masterSlot], memCard.saveProdCode[masterSlot], memCard.saveIdentifier[masterSlot],
                memCard.saveRegion[masterSlot], memCard.saveSize[masterSlot], memCard.iconFrames[masterSlot],
                memCard.iconColorData, memCard.FindSaveLinks(masterSlot), appSettings.IconBackgroundColor, xScale, yScale);

            informationDlg.ShowDialog(this);

            informationDlg.Dispose();
        }

        //Check if save is properly selected
        private bool validityCheck(out int listIndex, out int slotNumber)
        {
            listIndex = -1;
            slotNumber = -1;

            //If there are no cards return
            if (PScard.Count < 1) return false;

            listIndex = mainTabControl.SelectedIndex;

            //Return if no save is selected
            if (cardList[listIndex].SelectedIndices.Count < 1) return false;

            slotNumber = cardList[listIndex].SelectedIndices[0];

            return true;
        }

        //Delete/Restore selected save
        private void deleteRestoreSave(object sender, EventArgs e)
        {
            if (!validityCheck(out int listIndex, out int slotNumber)) return;

            int masterSlot = memCard.GetMasterLinkForSlot(slotNumber);

            memCard.ToggleDeleteSave(masterSlot);

            refreshListView(listIndex, slotNumber);

            if (memCard.slotType[masterSlot] == ps1card.SlotTypes.deleted_initial)
                pushHistory("Save deleted", mainTabControl.SelectedIndex, prepareIcons(listIndex, masterSlot, false));
            else
                pushHistory("Save restored", mainTabControl.SelectedIndex, prepareIcons(listIndex, masterSlot, false));
        }

        //Format selected save
        private void formatSave(object sender, EventArgs e)
        {
            if (!validityCheck(out int listIndex, out int slotNumber)) return;

            int masterSlot = memCard.GetMasterLinkForSlot(slotNumber);

            //Fetch save icon before deletion
            Bitmap saveIcon = prepareIcons(listIndex, masterSlot, false);

            memCard.FormatSave(masterSlot);

            refreshListView(listIndex, slotNumber);

            pushHistory("Save removed", mainTabControl.SelectedIndex, saveIcon);
        }

        //Copy save selected save from Memory Card
        private void copySave(object sender, EventArgs e)
        {
            if (!validityCheck(out int listIndex, out int slotNumber)) return;

            int initialSlot = memCard.GetMasterLinkForSlot(slotNumber);

            tempBuffer = PScard[listIndex].GetSaveBytes(initialSlot);
            tempBufferName = PScard[listIndex].saveName[initialSlot];
            BmpBuilder bmpImage = new BmpBuilder();
            Bitmap saveIcon = new Bitmap(new MemoryStream(bmpImage.BuildBmp(PScard[listIndex].iconColorData[initialSlot, 0])));

            saveIcon.RotateFlip(RotateFlipType.RotateNoneFlipY);

            //Show temp buffer toolbar info
            tBufToolButton.Enabled = true;
            tBufToolButton.Image = saveIcon;
            tBufToolButton.Text = tempBufferName;

            //Refresh the current list
            refreshListView(listIndex, slotNumber);
        }

        //Paste save to Memory Card
        private void pasteSave()
        {
            if (!validityCheck(out int listIndex, out int slotNumber)) return;

            if (tempBuffer == null) return;

            int requiredSlots = 0;
            if (PScard[listIndex].SetSaveBytes(slotNumber, tempBuffer, out requiredSlots))
            {
                refreshListView(listIndex, slotNumber);
                pushHistory("Save pasted", mainTabControl.SelectedIndex, prepareIcons(listIndex, slotNumber, false));
            }
            else
            {
                MessageBox.Show("To complete this operation " + requiredSlots.ToString() + " free slots are required.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Export a save
        private void exportSaveDialog(bool isRaw)
        {
            if (!validityCheck(out int listIndex, out int slotNumber)) return;

            int masterSlot = memCard.GetMasterLinkForSlot(slotNumber);

            byte singleSaveType;
            string outputFilename;

            if (isRaw)
            {
                //RAW file name on the system
                outputFilename = memCard.saveRegionRaw[masterSlot] + PScard[listIndex].saveProdCode[masterSlot] + PScard[listIndex].saveIdentifier[masterSlot];
            }
            else
            {
                //Set output filename to be compatible with PS3
                byte[] identifierASCII = Encoding.ASCII.GetBytes(PScard[listIndex].saveIdentifier[masterSlot]);
                outputFilename = memCard.saveRegionRaw[masterSlot] + PScard[listIndex].saveProdCode[masterSlot] +
                    BitConverter.ToString(identifierASCII).Replace("-", "");
            }

            //This will help us preserve full file title if illegal characters were found in save file name
            int illegalCharCount = 0;
            string completeFileName = outputFilename;

            //Filter illegal characters from the name
            foreach (char illegalChar in "\\/\":*?<>|".ToCharArray())
            {
                if (outputFilename.Contains(illegalChar.ToString())) illegalCharCount++;
                outputFilename = outputFilename.Replace(illegalChar.ToString(), "");
            }

            SaveFileDialog saveFileDlg = new SaveFileDialog
            {
                Title = "Export save",
                FileName = outputFilename,
                Filter = ssExtensions,
                FilterIndex = appSettings.LastExportFormat
            };

            if (isRaw) saveFileDlg.Filter = "RAW single save|B???????????*";

            //If user selected a card save to it
            if (saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                if (!isRaw && saveFileDlg.FilterIndex != appSettings.LastExportFormat)
                {
                    appSettings.LastExportFormat = saveFileDlg.FilterIndex;
                    //saveProgramSettings();
                }

                //Get save type
                switch (saveFileDlg.FilterIndex)
                {
                    default:         //MCS single save
                        singleSaveType = (int) ps1card.SingleSaveTypes.mcs;
                        break;

                    case 2:         //PS3 signed save
                        singleSaveType = (int) ps1card.SingleSaveTypes.psv;
                        break;

                    case 3:        //Action Replay
                        singleSaveType = (int) ps1card.SingleSaveTypes.psx;
                        break;
                }

                //RAW save type
                if (isRaw)
                {
                    singleSaveType = (int) ps1card.SingleSaveTypes.raw;

                    //Create text file with full file name if illegal characters were found
                    if (illegalCharCount > 0)
                    {
                        StreamWriter sw = File.CreateText(saveFileDlg.FileName + "_info.txt");
                        sw.WriteLine(completeFileName);
                        sw.WriteLine("");
                        sw.WriteLine("Region: \"" + memCard.saveRegion[masterSlot] + "\"");
                        sw.WriteLine("Product code: \"" + PScard[listIndex].saveProdCode[masterSlot] + "\"");
                        sw.WriteLine("Identifier: \"" + PScard[listIndex].saveIdentifier[masterSlot] + "\"");
                        sw.WriteLine("");
                        sw.WriteLine("This text file was created because the exported RAW save file name contains forbidden characters.");
                        sw.WriteLine("You can use this info when importing for example with uLaunchELF to make your save valid.");
                        sw.Write("Rename \"" + outputFilename + "\" to \"" + completeFileName + "\" after importing the save.");
                        sw.Close();
                    }
                }

                PScard[listIndex].SaveSingleSave(saveFileDlg.FileName, masterSlot, singleSaveType);
            }
        }

        //Import a save
        private void importSaveDialog()
        {
            if (!validityCheck(out int listIndex, out int slotNumber)) return;

            //Check if the slot to import the save on is free
            if (PScard[listIndex].slotType[slotNumber] == (byte) ps1card.SlotTypes.formatted)
            {
                OpenFileDialog openFileDlg = new OpenFileDialog
                {
                    Title = "Import save",
                    Filter = ssSupportedExtensions + "|" + ssExtensions + "|RAW single save|B???????????*"
                };

                //If user selected a save load it
                if (openFileDlg.ShowDialog() == DialogResult.OK)
                {
                    if (PScard[listIndex].OpenSingleSave(openFileDlg.FileName, slotNumber, out int requiredSlots))
                    {
                        refreshListView(listIndex, slotNumber);
                        pushHistory("Save imported", mainTabControl.SelectedIndex, prepareIcons(listIndex, slotNumber, false));
                    }
                    else if (requiredSlots > 0)
                    {
                        MessageBox.Show("To complete this operation " + requiredSlots.ToString() + " free slots are required.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show("The file could not be opened.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("The selected slot is not empty.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Open preferences window
        private void editPreferences()
        {
            new preferencesWindow().initializeDialog(this, registeredInterfaces);
            EnableDisableHardwareMenus();

            //Refresh current list after preferences change
            if(!validityCheck(out int listIndex, out int slotNumber)) return;
            refreshListView(listIndex, slotNumber);
        }

        //Open edit icon dialog
        private void editIcon()
        {
            if (!validityCheck(out int listIndex, out int slotNumber)) return;

            int masterSlot = memCard.GetMasterLinkForSlot(slotNumber);
            iconWindow iconDlg = new iconWindow();

            iconDlg.initializeDialog(memCard.saveName[masterSlot], memCard.iconFrames[masterSlot], memCard.GetIconBytes(masterSlot));
            iconDlg.ShowDialog(this);

            //Update data if OK has been pressed
            if (iconDlg.okPressed)
            {
                PScard[listIndex].SetIconBytes(masterSlot, iconDlg.iconData);
                refreshListView(listIndex, slotNumber);
                pushHistory("Icon edited", mainTabControl.SelectedIndex, prepareIcons(listIndex, masterSlot, false));
            }

            iconDlg.Dispose();
        }

        //Create and show plugins dialog
        private void showPluginsWindow()
        {
            pluginsWindow pluginsDlg = new pluginsWindow();

            pluginsDlg.initializeDialog(this, pluginSystem.assembliesMetadata);
            pluginsDlg.ShowDialog(this);
            pluginsDlg.Dispose();
        }

        //Create and show about dialog
        private void showAbout()
        {
            new AboutWindow().initDialog(this, appName, appVersion, appDate, "Copyright © Shendo 2024", 
                "Authors: Alvaro Tanarro, bitrot-alpha, lmiori92, \nNico de Poel, KuromeSan, Robxnano, Shendo\n\n" +
                "Beta testers: Gamesoul Master, Xtreme2damax,\nCarmax91, NKO. \n\n" +
                "Thanks to: @ruantec, Cobalt, TheCloudOfSmoke,\nRedawgTS, Hard core Rikki, RainMotorsports,\nZieg, Bobbi, OuTman, Kevstah2004, Kubusleonidas, \nFrédéric Brière, Cor'e, Gemini, DeadlySystem, \nPadraig Flood.\n\n" +
                "Special thanks to the following people whose\nMemory Card utilities inspired me to write my own:\nSimon Mallion (PSXMemTool),\nLars Ole Dybdal (PSXGameEdit),\nAldo Vargas (Memory Card Manager),\nNeill Corlett (Dexter),\nPaul Phoneix (ConvertM).");
        }

        //Bring a new item to the history list
        private void pushHistory(string description, int listIndex, Bitmap saveIcon)
        {

            //Clean everything from current index to end
            if (historyList[listIndex].SelectedIndices.Count > 0)
            {
                while (historyList[listIndex].SelectedIndices[0] < historyList[listIndex].Items.Count - 1)
                {
                    historyList[listIndex].Items.RemoveAt(historyList[listIndex].Items.Count - 1);
                    historyIconList[listIndex].Images.RemoveAt(historyIconList[listIndex].Images.Count - 1);
                }
            }

            //Add save icon to history list
            historyIconList[listIndex].Images.Add(saveIcon);

            //Add item to list
            historyList[listIndex].Items.Add(description);

            //Select the added item
            historyList[listIndex].Items[historyList[listIndex].Items.Count - 1].Selected = true;

            //Make sure that it's visible
            historyList[listIndex].Items[historyList[listIndex].Items.Count - 1].EnsureVisible();
        }

        //Make a new ListView control
        private void makeListView()
        {
            //Add a new ImageList to hold the card icons
            iconList.Add(new ImageList());
            iconList[iconList.Count - 1].ImageSize = new Size((int)(xScale * 48), (int)(yScale * 16));
            iconList[iconList.Count - 1].ColorDepth = ColorDepth.Depth32Bit;

            //Also for history list
            historyIconList.Add(new ImageList());
            historyIconList[historyIconList.Count - 1].ImageSize = new Size((int)(xScale * 16), (int)(yScale * 16));
            historyIconList[historyIconList.Count - 1].ColorDepth = ColorDepth.Depth32Bit;

            //Add history list
            historyList.Add(new CardListView());
            historyList[historyList.Count - 1].Font = new Font(FontFamily.GenericSansSerif.Name, 8.25f);
            historyList[historyList.Count - 1].Location = new Point(512, 0);
            historyList[historyList.Count - 1].Size = new Size(172, 300);
            historyList[historyList.Count - 1].BorderStyle = BorderStyle.None;
            historyList[historyList.Count - 1].BackColor = ActiveColors.backColor;
            historyList[historyList.Count - 1].ForeColor = ActiveColors.foreColor;
            historyList[historyList.Count - 1].FullRowSelect = true;
            historyList[historyList.Count - 1].MultiSelect = false;
            historyList[historyList.Count - 1].HideSelection = false;
            historyList[historyList.Count - 1].Columns.Add("History");
            historyList[historyList.Count - 1].Columns[0].Width = (int)(xScale * 160);
            historyList[historyList.Count - 1].View = View.Details;
            historyList[historyList.Count - 1].SelectedIndexChanged += new System.EventHandler(this.historyList_IndexChanged);
            historyList[historyList.Count - 1].SmallImageList = historyIconList[historyIconList.Count - 1];

            cardList.Add(new CardListView());
            cardList[cardList.Count - 1].Font = new Font(FontFamily.GenericSansSerif.Name, 8.25f);
            cardList[cardList.Count - 1].BorderStyle = BorderStyle.None;
            cardList[cardList.Count - 1].Size = new Size(512, 300);
            cardList[cardList.Count - 1].BackColor = ActiveColors.backColor;
            cardList[cardList.Count - 1].ForeColor = ActiveColors.foreColor;
            cardList[cardList.Count - 1].Dock = DockStyle.Bottom | DockStyle.Top | DockStyle.Left;
            cardList[cardList.Count - 1].SmallImageList = iconList[iconList.Count - 1];
            cardList[cardList.Count - 1].ContextMenuStrip = mainContextMenu;
            cardList[cardList.Count - 1].FullRowSelect = true;
            cardList[cardList.Count - 1].MultiSelect = false;
            cardList[cardList.Count - 1].HeaderStyle = ColumnHeaderStyle.Nonclickable;
            cardList[cardList.Count - 1].HideSelection = false;
            cardList[cardList.Count - 1].Columns.Add("Icon, region and title");
            cardList[cardList.Count - 1].Columns.Add("Product code");
            cardList[cardList.Count - 1].Columns.Add("Identifier");
            cardList[cardList.Count - 1].Columns[0].Width = (int)(xScale * 312);
            cardList[cardList.Count - 1].Columns[1].Width = (int)(xScale * 98);
            cardList[cardList.Count - 1].Columns[2].Width = (int)(xScale * 102);
            cardList[cardList.Count - 1].View = View.Details;
            //cardList[cardList.Count - 1].Click += new System.EventHandler(this.cardList_Click);
            cardList[cardList.Count - 1].DoubleClick += new System.EventHandler(this.cardList_DoubleClick);
            cardList[cardList.Count - 1].SelectedIndexChanged += new System.EventHandler(this.cardList_IndexChanged);

            refreshListView(cardList.Count - 1, -1);
        }
        
        //Refresh the ListView
        private void refreshListView(int listIndex, int slotNumber)
        {
            //Place cardName on the tab
            if(mainTabControl.TabPages[listIndex].Text != PScard[listIndex].cardName)
                mainTabControl.TabPages[listIndex].Text = PScard[listIndex].cardName;

            //Remove all icons from the list
            iconList[listIndex].Images.Clear();

            //Remove all items from the list
            cardList[listIndex].Items.Clear();

            for (int i = 0; i < ps1card.SlotCount; i++)
            {
                switch (PScard[listIndex].slotType[i])
                {
                    default:
                        iconList[listIndex].Images.Add(prepareIcons(listIndex, i, true));
                        cardList[listIndex].Items.Add(PScard[listIndex].saveName[i]);

                        //Check if save is using non standard region and append it to product code if not
                        //this fixes info for FreePSXBoot, soundscope, codelist and bunch of other nonstandard save names
                        if (PScard[listIndex].saveRegion[i] == "America" ||
                            PScard[listIndex].saveRegion[i] == "Europe" ||
                            PScard[listIndex].saveRegion[i] == "Japan")
                        {
                            cardList[listIndex].Items[i].SubItems.Add(PScard[listIndex].saveProdCode[i]);
                        }
                        else
                        {
                            cardList[listIndex].Items[i].SubItems.Add(PScard[listIndex].saveRegion[i] + PScard[listIndex].saveProdCode[i]);
                        }

                        cardList[listIndex].Items[i].SubItems.Add(PScard[listIndex].saveIdentifier[i]);
                        cardList[listIndex].Items[i].ImageIndex = i;
                        break;

                    case ps1card.SlotTypes.formatted:
                        cardList[listIndex].Items.Add("Free slot");
                        iconList[listIndex].Images.Add(new Bitmap(48, 16));
                        break;

                    case ps1card.SlotTypes.corrupted:
                        cardList[listIndex].Items.Add("Corrupted slot");
                        iconList[listIndex].Images.Add(new Bitmap(48, 16));
                        break;
                }
            }

            //Select the active item in the list
            if(slotNumber >= 0) cardList[listIndex].Items[slotNumber].Selected = true;

            //Set showListGrid option
            cardList[listIndex].GridLines = appSettings.ShowListGrid == 1;

            refreshPluginBindings();

            //Enable certain list items
            enableSelectiveEditItems();

            //Enable or disable undo/redo menu items
            enableDisableUndoRedo();
        }

        //Prepare icons for drawing (add flags and make them transparent if save is deleted)
        private Bitmap prepareIcons(int listIndex, int slotNumber, bool withFlag)
        {
            Bitmap iconBitmap;

            if (withFlag) iconBitmap = new Bitmap(48, 16);
            else iconBitmap = new Bitmap(16, 16);

            Graphics iconGraphics = Graphics.FromImage(iconBitmap);
            BmpBuilder bmpImage = new BmpBuilder();
            Bitmap saveIcon = new Bitmap(new MemoryStream(bmpImage.BuildBmp(PScard[listIndex].iconColorData[slotNumber, 0])));

            //Flipped because bmp stores data flipped but the raw icon data is not
            saveIcon.RotateFlip(RotateFlipType.RotateNoneFlipY);

            //Check what background color should be set
            switch (appSettings.IconBackgroundColor)
            {
                case 1:     //Black
                    iconGraphics.FillRegion(new SolidBrush(Color.Black), new Region(new Rectangle(0, 0, 16, 16)));
                    break;

                case 2:     //Gray
                    iconGraphics.FillRegion(new SolidBrush(Color.FromArgb(0xFF, 0x30, 0x30, 0x30)), new Region(new Rectangle(0, 0, 16, 16)));
                    break;

                case 3:     //Blue
                    iconGraphics.FillRegion(new SolidBrush(Color.FromArgb(0xFF, 0x44, 0x44, 0x98)), new Region(new Rectangle(0, 0, 16, 16)));
                    break;
            }

            //Draw icon
            iconGraphics.DrawImage(saveIcon, 0, 0, 16, 16);

            switch (PScard[listIndex].slotType[slotNumber])
            {
                case ps1card.SlotTypes.deleted_initial:
                case ps1card.SlotTypes.deleted_middle_link:
                case ps1card.SlotTypes.deleted_end_link:
                    Color blendColor = cardList[listIndex].BackColor;
                    iconGraphics.FillRegion(new SolidBrush(Color.FromArgb(0xA0, blendColor.R, blendColor.G, blendColor.B)), 
                        new Region(new Rectangle(0, 0, 16, 16)));
                    break;
            }

            //Skip drawing flag
            if (!withFlag)
            {
                iconGraphics.Dispose();
                return iconBitmap;
            }

            //Draw flag depending of the region
            switch (PScard[listIndex].saveRegion[slotNumber])
            {
                case "America":    //American region
                    iconGraphics.DrawImage(Properties.Resources.amflag, 17, 0, 30, 16);
                    break;

                case "Europe":    //European region
                    iconGraphics.DrawImage(Properties.Resources.euflag, 17, 0, 30, 16);
                    break;

                case "Japan":    //Japanese region
                    iconGraphics.DrawImage(Properties.Resources.jpflag, 17, 0, 30, 16);
                    break;
            }

            //Draw comment icon if save contains a comment
            if(PScard[listIndex].saveComments[slotNumber].Length > 0)
            {
                if(PScard[listIndex].saveComments[slotNumber][0] != '\0')
                iconGraphics.DrawImage(Properties.Resources.comments, 38, 6, 8, 8);
            }

            iconGraphics.Dispose();

            return iconBitmap;
        }

        //Refresh the toolstrip
        private void refreshStatusStrip()
        {
            //Show the location of the active card in the tool strip (if there are any cards)
            if (PScard.Count > 0)
                toolString.Text = PScard[mainTabControl.SelectedIndex].cardLocation + " ";
            else
                toolString.Text = " ";
        }

        //Save work and close the application
        private void exitApplication(FormClosingEventArgs e)
        {
            //Close every opened card
            if (closeAllCards() == 1)
                e.Cancel = true;

            //Save settings
            appSettings.SaveSettings(appPath, appName, appVersion);
        }

        //Edit header of the selected save
        private void editSaveHeader()
        {
            if (!validityCheck(out int listIndex, out int slotNumber)) return;

            headerWindow headerDlg = new headerWindow();
            int masterSlot = memCard.GetMasterLinkForSlot(slotNumber);

            //Load values to dialog
            headerDlg.initializeDialog(appName, memCard.saveName[masterSlot], memCard.saveProdCode[masterSlot],
                memCard.saveIdentifier[masterSlot], memCard.saveRegion[masterSlot]);
            headerDlg.ShowDialog(this);

            //Update values if OK was pressed
            if (headerDlg.okPressed)
            {
                //Insert data to save header of the selected card and slot
                memCard.SetHeaderData(masterSlot, headerDlg.prodCode, headerDlg.saveIdentifier, headerDlg.saveRegion);
                refreshListView(listIndex, slotNumber);
                pushHistory("Header edited", mainTabControl.SelectedIndex, prepareIcons(listIndex, masterSlot, false));
            }
            headerDlg.Dispose();
        }

        //Open a card if it's given by command line
        private bool loadCommandLine()
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                openCard(Environment.GetCommandLineArgs()[1]);
                return true;
            }
            else return false;
        }

        private void mainWindowLoad(object sender, EventArgs e)
        {
            //Show name of the application on the mainWindow
            this.Text = appName + " " + appVersion;

            //Load settings from settings file
            appSettings.LoadSettings(appPath);

            //Set active interface after loaded settings
            EnableDisableHardwareMenus();

            //Load available plugins
            pluginSystem.fetchPlugins(appPath + "/Plugins");

            //Create an empty card upon startup or load one given by the command line
            if(loadCommandLine() == false)openCard(null);
        }

        private void managePluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Show the plugins dialog
            showPluginsWindow();
        }

        //Refresh plugin menu
        private void refreshPluginBindings()
        {
            //Clear the menus
            editWithPluginToolStripMenuItem.DropDownItems.Clear();
            editWithPluginToolStripMenuItem.Enabled = false;

            editWithPluginToolStripMenuItem1.DropDownItems.Clear();
            editWithPluginToolStripMenuItem1.Enabled = false;

            if (!validityCheck(out int listIndex, out int slotNumber)) return;

            //Get the supported plugins
            supportedPlugins = pluginSystem.getSupportedPlugins(memCard.saveProdCode[memCard.GetMasterLinkForSlot(slotNumber)]);

            //Check if there are any plugins that support the product code
            if (supportedPlugins != null)
            {
                //Enable plugin menu
                editWithPluginToolStripMenuItem.Enabled = true;
                editWithPluginToolStripMenuItem1.Enabled = true;

                int count = 0;

                foreach (int currentAssembly in supportedPlugins)
                {
                    //Add item to the plugin menu
                    editWithPluginToolStripMenuItem.DropDownItems.Add(pluginSystem.assembliesMetadata[currentAssembly].pluginName);
                    editWithPluginToolStripMenuItem1.DropDownItems.Add(pluginSystem.assembliesMetadata[currentAssembly].pluginName);

                    count++;

                    //Color it with the parent decorations
                    editWithPluginToolStripMenuItem.DropDownItems[count - 1].ForeColor = editWithPluginToolStripMenuItem.ForeColor;
                    editWithPluginToolStripMenuItem1.DropDownItems[count - 1].ForeColor = editWithPluginToolStripMenuItem1.ForeColor;
                }
            }
        }

        //Edit a selected save with a selected plugin
        private void editWithPlugin(int pluginIndex)
        {
            //Check if there are any cards to edit
            if (PScard.Count > 0)
            {
                int listIndex = mainTabControl.SelectedIndex;
                int slotNumber = memCard.GetMasterLinkForSlot(cardList[listIndex].SelectedIndices[0]);
                byte[] editedSaveBytes = pluginSystem.editSaveData(supportedPlugins[pluginIndex], PScard[listIndex].GetSaveBytes(slotNumber), PScard[listIndex].saveProdCode[slotNumber]);

                if (editedSaveBytes != null)
                {
                    PScard[listIndex].ReplaceSaveBytes(slotNumber, editedSaveBytes);

                    //Refresh the list with new data
                    refreshListView(listIndex, slotNumber);

                    //Set the edited flag of the card
                    PScard[listIndex].changedFlag = true;

                    pushHistory("Edited by plugin", mainTabControl.SelectedIndex, prepareIcons(listIndex, slotNumber, false));
                }
            }
        }

        private void readMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Check if Readme.txt exists
            if (File.Exists(appPath + "/Readme.txt")) System.Diagnostics.Process.Start(appPath + "/Readme.txt");
            else MessageBox.Show("'ReadMe.txt' was not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //Enable or disable undo and redo items
        private void enableDisableUndoRedo()
        {
            undoToolStripMenuItem.Enabled = (memCard.UndoCount > 0);
            redoToolStripMenuItem.Enabled = (memCard.RedoCount > 0);
        }

        //Enable only supported edit operations
        private void enableSelectiveEditItems()
        {
            //Start with everything disabled
            SetEditItemsState(false);

            //Do not enable items if no cards are open
            if (mainTabControl.TabCount < 1) return;

            //Current card list we are working on
            CardListView currentCardList = cardList[mainTabControl.SelectedIndex];

            //Do not enable items if nothing is selected
            if (currentCardList.SelectedIndices.Count < 1) return;

            //Enable menu items based on the content
            switch (memCard.slotType[memCard.masterSlot[currentCardList.SelectedIndices[0]]])
            {
                case ps1card.SlotTypes.formatted:
                    importSaveToolStripMenuItem.Enabled = true;
                    importSaveToolStripMenuItem1.Enabled = true;
                    importButton.Enabled = true;

                    if (tempBuffer != null)
                    {
                        pasteSaveFromTemporaryBufferToolStripMenuItem.Enabled = true;
                        paseToolStripMenuItem.Enabled = true;
                    }
                    break;

                case ps1card.SlotTypes.initial:
                    SetEditItemsState(true);

                    restoreSaveToolStripMenuItem.Enabled = false;
                    restoreSaveToolStripMenuItem1.Enabled = false;
                    importSaveToolStripMenuItem.Enabled = false;
                    importSaveToolStripMenuItem1.Enabled = false;
                    importButton.Enabled = false;
                    pasteSaveFromTemporaryBufferToolStripMenuItem.Enabled = false;
                    paseToolStripMenuItem.Enabled = false;
                    break;

                case ps1card.SlotTypes.deleted_initial:
                    SetEditItemsState(true);

                    deleteSaveToolStripMenuItem.Enabled = false;
                    deleteSaveToolStripMenuItem1.Enabled = false;
                    importSaveToolStripMenuItem.Enabled = false;
                    importSaveToolStripMenuItem1.Enabled = false;
                    importButton.Enabled = false;
                    pasteSaveFromTemporaryBufferToolStripMenuItem.Enabled = false;
                    paseToolStripMenuItem.Enabled = false;
                    break;

                case ps1card.SlotTypes.corrupted:
                    //Enable only formating of the slot
                    removeSaveformatSlotsToolStripMenuItem.Enabled = true;
                    removeSaveformatSlotsToolStripMenuItem1.Enabled = true;
                    break;
            }

        }

        //Compare currently selected save with the temp buffer
        private void compareSaveWithTemp()
        {
            //Save data to work with
            byte[] fetchedData = null;
            string fetchedDataTitle = null;

            //Check if temp buffer contains anything
            if (tempBuffer == null)
            {
               MessageBox.Show("Temp buffer is empty. Save can't be compared.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!validityCheck(out int listIndex, out int slotNumber)) return;

            //Get data to work with
            fetchedData = memCard.GetSaveBytes(memCard.GetMasterLinkForSlot(slotNumber));
            fetchedDataTitle = memCard.saveName[memCard.GetMasterLinkForSlot(slotNumber)];

            //Check if selected saves have the same size
            if (fetchedData.Length != tempBuffer.Length)
            {
                MessageBox.Show("Save file size mismatch. Saves can't be compared.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Show compare window
            new compareWindow().initializeDialog(this, appName, fetchedData, fetchedDataTitle, tempBuffer, tempBufferName + " (temp buffer)");
        }

        //Read a Memory Card from the physical device
        private void cardReaderRead(byte[] readData, string deviceName)
        {
            //Create a new card
            PScard.Add(new ps1card());

            //Fill the card with the new data
            PScard[PScard.Count - 1].OpenMemoryCardStream(readData, appSettings.FixCorruptedCards == 1);

            //Temporary set a bogus file location (to fool filterNullCard function)
            PScard[PScard.Count - 1].cardLocation = "\0";

            //Create a tab page for the new card
            createTabPage();

            //Restore null location since DexDrive Memory Card is not a file present on the Hard Disk
            PScard[PScard.Count - 1].cardLocation = null;

            //Set the info to history list
            pushHistory("Card read (" + deviceName + ")", historyList.Count - 1, new Bitmap(16, 16));
        }

        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Show the location of the active card in the tool strip
            refreshStatusStrip();

            //Load available plugins for the selected save
            refreshPluginBindings();

            //Enable certain list items
            enableSelectiveEditItems();

            //Enable undo/redo menu items
            enableDisableUndoRedo();

            //Refresh active listview
            if (!validityCheck(out int listIndex, out int slotNumber)) return;
            refreshListView(listIndex, slotNumber);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Show about dialog
            showAbout();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Show browse dialog and open a selected Memory Card
            openCardDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Close the application
            this.Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Create a new card by giving a null path
            openCard(null);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Close the selected card
            closeCard(mainTabControl.SelectedIndex);
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Close all opened cards
            closeAllCards();
        }

        private void editSaveHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Edit header of the selected save
            editSaveHeader();
        }

        private void editSaveHeaderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Edit header of the selected save
            editSaveHeader();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Save a Memory Card as...
            saveCardDialog(mainTabControl.SelectedIndex);
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            //Create a new card by giving a null path
            openCard(null);
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            //Show browse dialog and open a selected Memory Card
            openCardDialog();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            //Save a Memory Card
            saveCardFunction(mainTabControl.SelectedIndex);
        }

        private void editSaveCommentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Edit save comment of the selected slot
            editSaveComments();
        }

        private void editSaveCommentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Edit save comment of the selected slot
            editSaveComments();
        }

        private void commentsButton_Click(object sender, EventArgs e)
        {
            //Edit save comment of the selected slot
            editSaveComments();
        }

        private void cardList_DoubleClick(object sender, EventArgs e)
        {
            //Show information about the selected save
            showInformation();
        }

        //Apply back and fore colors to all items and subitems in the list
        private void colorListItem(ListView lv, int index, Color back, Color fore)
        {
            lv.Items[index].BackColor = back;
            lv.Items[index].ForeColor = fore;

            //And subitems
            if (lv.Items[index].SubItems.Count > 1)
            {
                lv.Items[index].SubItems[1].BackColor = back;
                lv.Items[index].SubItems[1].ForeColor = fore;

                lv.Items[index].SubItems[2].BackColor = back;
                lv.Items[index].SubItems[2].ForeColor = fore;
            }
        }

        //Enable or disable menu items based on the currently selected save
        private void SetEditItemsState(bool itemStates)
        {
            //Edit menu
            editSaveHeaderToolStripMenuItem.Enabled = itemStates;
            editSaveCommentToolStripMenuItem.Enabled = itemStates;
            compareWithTempBufferToolStripMenuItem.Enabled = itemStates;
            editIconToolStripMenuItem.Enabled = itemStates;
            deleteSaveToolStripMenuItem.Enabled = itemStates;
            restoreSaveToolStripMenuItem.Enabled = itemStates;
            removeSaveformatSlotsToolStripMenuItem.Enabled = itemStates;
            copySaveToTempraryBufferToolStripMenuItem.Enabled = itemStates;
            pasteSaveFromTemporaryBufferToolStripMenuItem.Enabled = itemStates;
            importSaveToolStripMenuItem.Enabled = itemStates;
            exportSaveToolStripMenuItem.Enabled = itemStates;
            exportRAWSaveToolStripMenuItem.Enabled = itemStates;

            //Edit toolbar
            editHeaderButton.Enabled = itemStates;
            commentsButton.Enabled = itemStates;
            editIconButton.Enabled = itemStates;
            importButton.Enabled = itemStates;
            exportButton.Enabled = itemStates;

            //Edit popup
            editSaveHeaderToolStripMenuItem1.Enabled = itemStates;
            editSaveCommentsToolStripMenuItem.Enabled = itemStates;
            compareWithTempBufferToolStripMenuItem1.Enabled = itemStates;
            editIconToolStripMenuItem1.Enabled = itemStates;
            deleteSaveToolStripMenuItem1.Enabled = itemStates;
            restoreSaveToolStripMenuItem1.Enabled = itemStates;
            removeSaveformatSlotsToolStripMenuItem1.Enabled = itemStates;
            copySaveToTempBufferToolStripMenuItem.Enabled = itemStates;
            paseToolStripMenuItem.Enabled = itemStates;
            importSaveToolStripMenuItem1.Enabled = itemStates;
            exportSaveToolStripMenuItem1.Enabled = itemStates;
            exportRAWSaveToolStripMenuItem1.Enabled = itemStates;
            saveInformationToolStripMenuItem.Enabled = itemStates;
        }

        private void historyList_IndexChanged(object sender, EventArgs e)
        {
            var selectedList = sender as ListView;

            //If nothing is selected abort
            if (selectedList.SelectedIndices.Count < 1) return;

            int selectedIndex = selectedList.SelectedIndices[0];

            //Set item colors
            for (int i = 0; i < selectedList.Items.Count; i++)
            {
                if (selectedList.Items[i].Selected)
                colorListItem(selectedList, i, Color.FromArgb(220, Color.FromKnownColor(KnownColor.MenuHighlight)), SystemColors.HighlightText);
                else
                colorListItem(selectedList, i, selectedList.BackColor, selectedList.ForeColor);
            }

            //Jump to the selected point in time
            if(memCard.UndoCount > selectedIndex)
            {
                while(memCard.UndoCount > selectedIndex) memCard.Undo();
            }
            else if (memCard.UndoCount < selectedIndex)
            {
                while (memCard.UndoCount < selectedIndex) memCard.Redo();
            }

            //Refresh list so the updates can be visible
            validityCheck(out int listIndex, out int slotNumber);
            refreshListView(listIndex, slotNumber);
        }

        private void cardList_IndexChanged(object sender, EventArgs e)
        {
            var selectedList = sender as ListView;

            SetEditItemsState(false);

            //If nothing is selected abort
            if (selectedList.SelectedIndices.Count < 1) return;

            //Reset colors to default
            for (int i = 0; i < ps1card.SlotCount; i++)
            {
                colorListItem(selectedList, i , selectedList.BackColor, selectedList.ForeColor);
            }

            //Select all linked save slots
            foreach(int slotIndex in memCard.FindSaveLinks(memCard.GetMasterLinkForSlot(selectedList.SelectedIndices[0])))
            {
                colorListItem(selectedList, slotIndex, Color.FromArgb(220, Color.FromKnownColor(KnownColor.MenuHighlight)), SystemColors.HighlightText);
            }

            //Load appropriate plugins for the selected save
            refreshPluginBindings();

            //Enable certain list items
            enableSelectiveEditItems();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Save Memory Card
            saveCardFunction(mainTabControl.SelectedIndex);
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open preferences dialog
            editPreferences();
        }

        private void mainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Cleanly close the application
            exitApplication(e);
        }

        private void editHeaderButton_Click(object sender, EventArgs e)
        {
            //Edit header of the selected save
            editSaveHeader();
        }

        private void editIconButton_Click(object sender, EventArgs e)
        {
            //Edit save icon
            editIcon();
        }

        private void editIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Edit icon of the selected save
            editIcon();
        }

        private void editIconToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Edit icon of the selected save
            editIcon();
        }

        private void exportSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Export a save
            exportSaveDialog(false);
        }

        private void exportSaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Export a save
            exportSaveDialog(false);
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            //Export a save
            exportSaveDialog(false);
        }

        private void importSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Import a save
            importSaveDialog();
        }

        private void importSaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Import a save
            importSaveDialog();
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            //Import a save
            importSaveDialog();
        }

        private void pasteSaveFromTemporaryBufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Paste a save from temp buffer to selected slot
            pasteSave();
        }

        private void paseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Paste a save from temp buffer to selected slot
            pasteSave();
        }

        private void tBufToolButton_Click(object sender, EventArgs e)
        {
            //Paste a save from temp buffer to selected slot
            pasteSave();
        }

        private void saveInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Show information about a selected save
            showInformation();
        }

        private void mainTabControl_DragEnter(object sender, DragEventArgs e)
        {
            //Check if dragged data are files
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private void editWithPluginToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //Set the click item
            clickedPlugin[1] = e.ClickedItem.Owner.Items.IndexOf(e.ClickedItem);

            //Set the clicked flag
            clickedPlugin[0] = 1;
        }

        private void editWithPluginToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
            //Load clicked plugin if the menu was clicked
            if (clickedPlugin[0] == 1) editWithPlugin(clickedPlugin[1]);

            //Set the clicked flag on false
            clickedPlugin[0] = 0;
        }

        private void mainTabControl_MouseDown(object sender, MouseEventArgs e)
        {
            //Check if the middle mouse button is pressed
            if (e.Button == MouseButtons.Middle)
            {
                Rectangle tabRectangle;

                //Cycle through all available tabs
                for (int i = 0; i < mainTabControl.TabCount; i++)
                {
                    tabRectangle = mainTabControl.GetTabRect(i);

                    //Close the middle clicked tab
                    if (tabRectangle.Contains(e.X, e.Y))closeCard(i,false);
                }
            }
        }

        private void mainTabControl_DragDrop(object sender, DragEventArgs e)
        {
            string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

            //Cycle through every dropped file
            foreach (string fileName in droppedFiles)
            {
                openCard(fileName);
            }
        }

        private void compareWithTempBufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Compare selected save to a temp buffer save
            compareSaveWithTemp();
        }

        private void compareWithTempBufferToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Compare selected save to a temp buffer save
            compareSaveWithTemp();
        }

        private void exportRAWSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Export RAW save
            exportSaveDialog(true);
        }

        private void exportRAWSaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Export RAW save
            exportSaveDialog(true);
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Go up a history list
            if(!validityCheck(out int listIndex, out int slotNumber)) return;

            if (historyList[listIndex].SelectedIndices.Count < 1) return;
            int selectedIndex = historyList[listIndex].SelectedIndices[0];

            if (selectedIndex > 0)
            {
                historyList[listIndex].Items[selectedIndex - 1].Selected = true;
                historyList[listIndex].Items[selectedIndex - 1].EnsureVisible();
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Go down a history list
            if (!validityCheck(out int listIndex, out int slotNumber)) return;

            if (historyList[listIndex].SelectedIndices.Count < 1) return;
            int selectedIndex = historyList[listIndex].SelectedIndices[0];

            if (selectedIndex < historyList[listIndex].Items.Count - 1)
            {
                historyList[listIndex].Items[selectedIndex + 1].Selected = true;
                historyList[listIndex].Items[selectedIndex + 1].EnsureVisible();
            }
        }
    }
}
