/*
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

//Main window of the MemcardRex application
//Shendo 2009 - 2021

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MemcardRex
{
    public partial class mainWindow : Form
    {
        //Application related strings
        const string appName = "MemcardRex";
        const string appDate = "Unknown";

#if DEBUG
        const string appVersion = "1.9 (Debug)";
#else
        const string appVersion = "1.9";
#endif

        //Location of the application
        string appPath = Application.StartupPath;

        //API for the Aero glass effect
        glassSupport windowGlass = new glassSupport();

        //Margins for the Aero glass
        glassSupport.margins windowMargins = new glassSupport.margins();

        //Rectangle for the Aero glass
        Rectangle windowRectangle = new Rectangle();

        //Plugin system (public because plugin dialog has to access it)
        public rexPluginSystem pluginSystem = new rexPluginSystem();

        //Supported plugins for the currently selected save
        int[] supportedPlugins = null;

        //Currently clicked plugin (0 - clicked flag, 1 - plugin index)
        int[] clickedPlugin = new int[]{0,0};

        public double xScale = 1.0;
        public double yScale = 1.0;

        //Struct holding all program related settings (public because settings dialog has to access it)
        public struct programSettings
        {
            public int titleEncoding;              //Encoding of the save titles (0 - ASCII, 1 - UTF-16)
            public int showListGrid;               //List grid settings
            public int iconInterpolationMode;      //Icon iterpolation mode settings
            public int iconPropertiesSize;         //Icon size settings in save properties
            public int iconBackgroundColor;        //Various colors based on PS1 BIOS backgrounds
            public int backupMemcards;             //Backup Memory Card settings
            public int warningMessage;             //Warning message settings
            public int restoreWindowPosition;      //Restore window position
            public int fixCorruptedCards;          //Try to fix corrupted memory cards
            public int glassStatusBar;             //Vista glass status bar
            public int formatType;                 //Type of formatting for hardware interfaces
            public string listFont;                //List font
            public string communicationPort;       //Communication port for Hardware interfaces
        }

        //All program settings
        programSettings mainSettings = new programSettings();

        //List of opened Memory Cards
        List<ps1card> PScard = new List<ps1card>();

        //Listview of the opened Memory Cards
        List<ListView> cardList = new List<ListView>();

        //List of icons for the saves
        List<ImageList> iconList = new List<ImageList>();

        //Temp buffer used to store saves
        byte[] tempBuffer = null;
        string tempBufferName = null;

        public mainWindow()
        {
            InitializeComponent();
            Graphics g = CreateGraphics();
            xScale = g.DpiX / 96.0;
            yScale = g.DpiY / 96.0;
            g.Dispose();
        }

        //Apply glass effect on the client area
        private void applyGlass()
        {
            //Reset margins to zero
            windowMargins.top = 0;
            windowMargins.bottom = 0;
            windowMargins.left = 0;
            windowMargins.right = 0;

            //Check if the requirements for the Aero glass are met
            if (windowGlass.isGlassSupported() && mainSettings.glassStatusBar == 1)
            {
                //Hide status strip
                this.mainStatusStrip.Visible = false;

                windowMargins.bottom = 22;
                windowRectangle = new Rectangle(0, this.ClientSize.Height - windowMargins.bottom, this.ClientSize.Width, windowMargins.bottom + 5);
                glassSupport.DwmExtendFrameIntoClientArea(this.Handle, ref windowMargins);

                //Repaint the form
                this.Refresh();
            }
            else
            {
                //Check if effect of aero needs to be supressed
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    windowMargins.bottom = 0;
                    windowRectangle = new Rectangle(0, this.ClientSize.Height - windowMargins.bottom, this.ClientSize.Width, windowMargins.bottom + 5);
                    glassSupport.DwmExtendFrameIntoClientArea(this.Handle, ref windowMargins);

                    //Repaint the form
                    this.Refresh();
                }

                //Show status strip
                this.mainStatusStrip.Visible = true;
            }
        }

        //Apply program settings
        private void applySettings()
        {
            //Refresh all active lists
            for (int i = 0; i < cardList.Count; i++)
                refreshListView(i, cardList[i].SelectedIndices[0]);

            //Refresh status of Aero glass
            applyGlass();
        }

        //Apply program settings from given values
        public void applyProgramSettings(programSettings progSettings)
        {
            mainSettings = progSettings;

            //Apply given settings
            applySettings();
        }

        //Load program settings
        private void loadProgramSettings()
        {
            Point mainWindowLocation = new Point(0, 0);
            xmlSettingsEditor xmlAppSettings = new xmlSettingsEditor();

            //Check if the Settings.xml exists
            if (File.Exists(appPath + "/Settings.xml"))
            {
                //Open XML file for reading, file is auto-closed
                xmlAppSettings.openXmlReader(appPath + "/Settings.xml");

                //Load list font
                mainSettings.listFont = xmlAppSettings.readXmlEntry("ListFont");

                //Load DexDrive COM port
                mainSettings.communicationPort = xmlAppSettings.readXmlEntry("ComPort");

                //Load Title Encoding
                mainSettings.titleEncoding = xmlAppSettings.readXmlEntryInt("TitleEncoding", 0, 1);

                //Load List Grid settings
                mainSettings.showListGrid = xmlAppSettings.readXmlEntryInt("ShowGrid", 0, 1);

                //Load glass option switch
                mainSettings.glassStatusBar = xmlAppSettings.readXmlEntryInt("GlassStatusBar", 0, 1);

                //Load icon interpolation settings
                mainSettings.iconInterpolationMode = xmlAppSettings.readXmlEntryInt("IconInterpolationMode", 0, 1);

                //Load icon size settings
                mainSettings.iconPropertiesSize = xmlAppSettings.readXmlEntryInt("IconSize", 0, 1);

                //Load icon background color
                mainSettings.iconBackgroundColor = xmlAppSettings.readXmlEntryInt("IconBackgroundColor", 0, 4);

                //Load backup Memory Cards value
                mainSettings.backupMemcards = xmlAppSettings.readXmlEntryInt("BackupMemoryCards", 0, 1);

                //Load warning message switch
                mainSettings.warningMessage = xmlAppSettings.readXmlEntryInt("WarningMessage", 0, 1);

                //Load window position switch
                mainSettings.restoreWindowPosition = xmlAppSettings.readXmlEntryInt("RestoreWindowPosition", 0, 1);

                //Load format type
                mainSettings.formatType = xmlAppSettings.readXmlEntryInt("HardwareFormatType", 0, 1);

                //Load fix corrupted cards value
                mainSettings.fixCorruptedCards = xmlAppSettings.readXmlEntryInt("FixCorruptedCards", 0, 1);

                //Check if window position should be read
                if (mainSettings.restoreWindowPosition == 1)
                {
                    mainWindowLocation.X = xmlAppSettings.readXmlEntryInt("WindowX", -65535, 65535);
                    mainWindowLocation.Y = xmlAppSettings.readXmlEntryInt("WindowY", -65535, 65535);

                    //Apply read position
                    this.Location = mainWindowLocation;
                }

                //Apply loaded settings
                applySettings();
            }
        }

        //Save program settings
        private void saveProgramSettings()
        {
            xmlSettingsEditor xmlAppSettings = new xmlSettingsEditor();

            //Open XML file for writing
            xmlAppSettings.openXmlWriter(appPath + "/Settings.xml", appName + " " + appVersion + " settings data");

            //Set list font
            xmlAppSettings.writeXmlEntry("ListFont", mainSettings.listFont);

            //Set DexDrive port
            xmlAppSettings.writeXmlEntry("ComPort", mainSettings.communicationPort);

            //Set title encoding
            xmlAppSettings.writeXmlEntry("TitleEncoding", mainSettings.titleEncoding.ToString());

            //Set List Grid settings
            xmlAppSettings.writeXmlEntry("ShowGrid", mainSettings.showListGrid.ToString());

            //Set glass option switch
            xmlAppSettings.writeXmlEntry("GlassStatusBar", mainSettings.glassStatusBar.ToString());

            //Set icon interpolation settings
            xmlAppSettings.writeXmlEntry("IconInterpolationMode", mainSettings.iconInterpolationMode.ToString());

            //Set icon size options
            xmlAppSettings.writeXmlEntry("IconSize", mainSettings.iconPropertiesSize.ToString());

            //Set icon background color
            xmlAppSettings.writeXmlEntry("IconBackgroundColor", mainSettings.iconBackgroundColor.ToString());

            //Set backup Memory Cards value
            xmlAppSettings.writeXmlEntry("BackupMemoryCards", mainSettings.backupMemcards.ToString());

            //Set warning message switch
            xmlAppSettings.writeXmlEntry("WarningMessage", mainSettings.warningMessage.ToString());

            //Set window position switch
            xmlAppSettings.writeXmlEntry("RestoreWindowPosition", mainSettings.restoreWindowPosition.ToString());

            //Set format type
            xmlAppSettings.writeXmlEntry("HardwareFormatType", mainSettings.formatType.ToString());

            //Set fix corrupted cards value
            xmlAppSettings.writeXmlEntry("FixCorruptedCards", mainSettings.fixCorruptedCards.ToString());

            //Set window X coordinate
            xmlAppSettings.writeXmlEntry("WindowX", this.Location.X.ToString());

            //Set window Y coordinate
            xmlAppSettings.writeXmlEntry("WindowY", this.Location.Y.ToString());

            //Cleanly close opened XML file
            xmlAppSettings.closeXmlWriter();
        }

        //Quick and dirty settings bool converter
        private bool getSettingsBool(int intValue)
        {
            return (intValue == 1) ? true : false;
        }

        //Backup a Memory Card
        private void backupMemcard(string fileName)
        {
            //Check if backuping of memcard is allowed and the filename is valid
            if (mainSettings.backupMemcards == 1 && fileName != null)
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
                    catch(Exception)
                    {

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
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Title = "Open Memory Card";
            openFileDlg.Filter = "All supported|*.mcr;*.gme;*.bin;*.mcd;*.mem;*.vgs;*.mc;*.ddf;*.ps;*.psm;*.mci;*.VMP;*.VM1;*.srm|ePSXe/PSEmu Pro Memory Card (*.mcr)|*.mcr|DexDrive Memory Card (*.gme)|*.gme|pSX/AdriPSX Memory Card (*.bin)|*.bin|Bleem! Memory Card (*.mcd)|*.mcd|VGS Memory Card (*.mem, *.vgs)|*.mem; *.vgs|PSXGame Edit Memory Card (*.mc)|*.mc|DataDeck Memory Card (*.ddf)|*.ddf|WinPSM Memory Card (*.ps)|*.ps|Smart Link Memory Card (*.psm)|*.psm|MCExplorer (*.mci)|*.mci|PSP virtual Memory Card (*.VMP)|*.VMP|PS3 virtual Memory Card (*.VM1)|*.VM1|PCSX ReARMed/RetroArch|*.srm|All files (*.*)|*.*";
            openFileDlg.Multiselect = true;

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
                    //Card is already opened, display message and exit
                    MessageBox.Show("'" + Path.GetFileName(fileName) + "' is already opened.", appName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            //Create a new card
            PScard.Add(new ps1card());

            //Try to open card
            errorMsg = PScard[PScard.Count - 1].openMemoryCard(fileName, getSettingsBool(mainSettings.fixCorruptedCards));

            //If card is sucesfully opened proceed further, else destroy it
            if (errorMsg == null)
            {
                //Backup opened card
                backupMemcard(fileName);

                //Make a new tab for the opened card
                createTabPage();
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

            //Set default color
            tabPage.BackColor = SystemColors.Window;

            //Add a tab corresponding to opened card
            mainTabControl.TabPages.Add(tabPage);

            //Make a new ListView control
            makeListView();

            //Add ListView control to the tab page
            tabPage.Controls.Add(cardList[cardList.Count - 1]);

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
        }

        //Save a Memory Card with SaveFileDialog
        private void saveCardDialog(int listIndex)
        {
            //Check if there are any cards to save
            if (PScard.Count > 0)
            {
                byte memoryCardType = 0;
                SaveFileDialog saveFileDlg = new SaveFileDialog();
                saveFileDlg.Title = "Save Memory Card";
                saveFileDlg.Filter = "ePSXe/PSEmu Pro Memory Card (*.mcr)|*.mcr|PSP/Vita Memory Card (*.VMP)|*.VMP|DexDrive Memory Card (*.gme)|*.gme|pSX/AdriPSX Memory Card (*.bin)|*.bin|Bleem! Memory Card (*.mcd)|*.mcd|VGS Memory Card (*.mem, *.vgs)|*.mem; *.vgs|PSXGame Edit Memory Card (*.mc)|*.mc|DataDeck Memory Card (*.ddf)|*.ddf|WinPSM Memory Card (*.ps)|*.ps|Smart Link Memory Card (*.psm)|*.psm|MCExplorer (*.mci)|*.mci|PS3 virtual Memory Card (*.VM1)|*.VM1|PCSX ReARMed/RetroArch|*.srm";

                //If user selected a card save to it
                if (saveFileDlg.ShowDialog() == DialogResult.OK)
                {
                    //Get save type
                    switch(saveFileDlg.FilterIndex)
                    {
                        default:        //Raw Memory Card
                            memoryCardType = 1;
                            break;

                        case 2:         //VMP Memory Card
                            memoryCardType = 4;
                            break;

                        case 3:         //GME Memory Card
                            memoryCardType = 2;
                            break;

                        case 6:         //VGS Memory Card
                            memoryCardType = 3;
                            break;
                    }
                    saveMemoryCard(listIndex, saveFileDlg.FileName, memoryCardType);
                }
            }
        }

        //Save a Memory Card to a given filename
        private void saveMemoryCard(int listIndex, string fileName, byte memoryCardType)
        {
            if (PScard[listIndex].saveMemoryCard(fileName, memoryCardType, getSettingsBool(mainSettings.fixCorruptedCards)))
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
                //Check if file can be saved or save dialog must be shown (VMP is read only)
                if (PScard[listIndex].cardLocation == null || PScard[listIndex].cardType == 4)
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
                iconList.RemoveAt(listIndex);
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
            //Check if there are any cards to edit comments on
            if (PScard.Count > 0)
            {
                int listIndex = mainTabControl.SelectedIndex;

                //Check if a save is selected
                if (cardList[listIndex].SelectedIndices.Count == 0) return;

                int slotNumber = cardList[listIndex].SelectedIndices[0];
                string saveTitle = PScard[listIndex].saveName[slotNumber, mainSettings.titleEncoding];
                string saveComment = PScard[listIndex].saveComments[slotNumber];

                //Check if comments are allowed to be edited
                switch (PScard[listIndex].saveType[slotNumber])
                {
                    default:        //Not allowed
                        break;

                    case 1:
                    case 4:
                        commentsWindow commentsDlg = new commentsWindow();

                        //Load values to dialog
                        commentsDlg.initializeDialog(saveTitle, saveComment);
                        commentsDlg.ShowDialog(this);

                        //Update values if OK was pressed
                        if (commentsDlg.okPressed)
                        {
                            //Insert edited comments back in the card
                            PScard[listIndex].saveComments[slotNumber] = commentsDlg.saveComment;
                        }
                        commentsDlg.Dispose();
                        break;
                }
            }
        }

        //Create and show information dialog
        private void showInformation()
        {
            //Check if there are any cards
            if (PScard.Count > 0)
            {
                int listIndex = mainTabControl.SelectedIndex;

                //Check if a save is selected
                if (cardList[listIndex].SelectedIndices.Count == 0) return;

                int slotNumber = cardList[listIndex].SelectedIndices[0];
                ushort saveRegion = PScard[listIndex].saveRegion[slotNumber];
                int saveSize = PScard[listIndex].saveSize[slotNumber];
                int iconFrames = PScard[listIndex].iconFrames[slotNumber];
                string saveProdCode = PScard[listIndex].saveProdCode[slotNumber];
                string saveIdentifier = PScard[listIndex].saveIdentifier[slotNumber];
                string saveTitle = PScard[listIndex].saveName[slotNumber, mainSettings.titleEncoding];
                Bitmap[] saveIcons = new Bitmap[3];

                //Get all 3 bitmaps for selected save
                for (int i = 0; i < 3; i++)
                    saveIcons[i] = PScard[listIndex].iconData[slotNumber, i];

                //Check if slot is "legal"
                switch (PScard[listIndex].saveType[slotNumber])
                {
                    default:        //Not allowed
                        break;

                    case 1:
                    case 4:
                        informationWindow informationDlg = new informationWindow();

                        //Load values to dialog
                        informationDlg.initializeDialog(saveTitle, saveProdCode, saveIdentifier,
                            saveRegion, saveSize, iconFrames, mainSettings.iconInterpolationMode, mainSettings.iconPropertiesSize, saveIcons, PScard[listIndex].findSaveLinks(slotNumber), mainSettings.iconBackgroundColor);

                        informationDlg.ShowDialog(this);

                        informationDlg.Dispose();
                        break;
                }
            }
        }

        //Restore selected save
        private void restoreSave()
        {
            //Check if there are any cards available
            if (PScard.Count > 0)
            {
                int listIndex = mainTabControl.SelectedIndex;

                //Check if a save is selected
                if (cardList[listIndex].SelectedIndices.Count == 0) return;

                int slotNumber = cardList[listIndex].SelectedIndices[0];

                //Check the save type
                switch (PScard[listIndex].saveType[slotNumber])
                {
                    default:
                        break;

                    case 4:         //Deleted initial
                        PScard[listIndex].toggleDeleteSave(slotNumber);
                        refreshListView(listIndex, slotNumber);
                        break;

                    case 1:         //Initial save
                        MessageBox.Show("The selected save is not deleted.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning );
                        break;

                    case 2:
                    case 3:
                    case 5:
                    case 6:
                        MessageBox.Show("The selected slot is linked. Select the initial save slot to proceed.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
        }

        //Delete selected save
        private void deleteSave()
        {
            //Check if there are any cards available
            if (PScard.Count > 0)
            {
                int listIndex = mainTabControl.SelectedIndex;

                //Check if a save is selected
                if (cardList[listIndex].SelectedIndices.Count == 0) return;

                int slotNumber = cardList[listIndex].SelectedIndices[0];

                //Check the save type
                switch (PScard[listIndex].saveType[slotNumber])
                {
                    default:
                        break;

                    case 1:         //Initial save
                        PScard[listIndex].toggleDeleteSave(slotNumber);
                        refreshListView(listIndex, slotNumber);
                        break;

                    case 4:         //Deleted initial
                        MessageBox.Show("The selected save is already deleted.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case 2:
                    case 3:
                    case 5:
                    case 6:
                        MessageBox.Show("The selected slot is linked. Select the initial save slot to proceed.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
        }

        //Format selected save
        private void formatSave()
        {
            //Check if there are any cards available
            if (PScard.Count > 0)
            {
                int listIndex = mainTabControl.SelectedIndex;

                //Check if a save is selected
                if (cardList[listIndex].SelectedIndices.Count == 0) return;

                int slotNumber = cardList[listIndex].SelectedIndices[0];

                //Check the save type
                switch (PScard[listIndex].saveType[slotNumber])
                {
                    default:    //Slot is either initial, deleted initial or corrupted so it can be safetly formatted
                        DialogResult result = MessageBox.Show("Formatted slots cannot be restored.\nDo you want to proceed with this operation?", 
                                                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            PScard[listIndex].formatSave(slotNumber);
                            refreshListView(listIndex, slotNumber);
                        }
                        break;

                    case 2:
                    case 3:
                    case 5:
                    case 6:
                        MessageBox.Show("The selected slot is linked. Select the initial save slot to proceed.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
        }

        //Copy save selected save from Memory Card
        private void copySave()
        {
            //Check if there are any cards available
            if (PScard.Count > 0)
            {
                int listIndex = mainTabControl.SelectedIndex;

                //Check if a save is selected
                if (cardList[listIndex].SelectedIndices.Count == 0) return;

                int slotNumber = cardList[listIndex].SelectedIndices[0];
                string saveName = PScard[listIndex].saveName[slotNumber, 0];

                //Check the save type
                switch (PScard[listIndex].saveType[slotNumber])
                {
                    default:
                        break;

                    case 1:         //Initial save
                    case 4:         //Deleted initial
                        tempBuffer = PScard[listIndex].getSaveBytes(slotNumber);
                        tempBufferName = PScard[listIndex].saveName[slotNumber, 0];

                        //Show temp buffer toolbar info
                        tBufToolButton.Enabled = true;
                        tBufToolButton.Image = PScard[listIndex].iconData[slotNumber, 0];
                        tBufToolButton.Text = saveName;

                        //Refresh the current list
                        refreshListView(listIndex, slotNumber);

                        break;

                    case 2:
                    case 3:
                    case 5:
                    case 6:
                        MessageBox.Show("The selected slot is linked. Select the initial save slot to proceed.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
        }

        //Paste save to Memory Card
        private void pasteSave()
        {
            //Check if there are any cards available
            if (PScard.Count > 0)
            {
                int listIndex = mainTabControl.SelectedIndex;

                //Check if a save is selected
                if (cardList[listIndex].SelectedIndices.Count == 0) return;

                int slotNumber = cardList[listIndex].SelectedIndices[0];
                int requiredSlots = 0;

                //Check if temp buffer contains anything
                if (tempBuffer != null)
                {
                    //Check if the slot to paste the save on is free
                    if (PScard[listIndex].saveType[slotNumber] == 0)
                    {
                        if (PScard[listIndex].setSaveBytes(slotNumber, tempBuffer, out requiredSlots))
                        {
                            refreshListView(listIndex, slotNumber);
                        }
                        else
                        {
                            MessageBox.Show("To complete this operation " + requiredSlots.ToString() + " free slots are required.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("The selected slot is not empty.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Temp buffer is empty.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        //Export a save
        private void exportSaveDialog()
        {
            //Check if there are any cards available
            if (PScard.Count > 0)
            {
                int listIndex = mainTabControl.SelectedIndex;

                //Check if a save is selected
                if (cardList[listIndex].SelectedIndices.Count == 0) return;

                int slotNumber = cardList[listIndex].SelectedIndices[0];

                //Check the save type
                switch (PScard[listIndex].saveType[slotNumber])
                {
                    default:
                        break;

                    case 1:         //Initial save
                        byte singleSaveType = 0;

                        //Set output filename
                        byte[] identifierASCII = Encoding.ASCII.GetBytes(PScard[listIndex].saveIdentifier[slotNumber]);
                        string outputFilename = getRegionString(PScard[listIndex].saveRegion[slotNumber]) + PScard[listIndex].saveProdCode[slotNumber] +
                            BitConverter.ToString(identifierASCII).Replace("-","");
                        
                        //Filter illegal characters from the name
                        foreach (char illegalChar in Path.GetInvalidPathChars())
                        {
                            outputFilename = outputFilename.Replace(illegalChar.ToString(), "");
                        }

                        SaveFileDialog saveFileDlg = new SaveFileDialog();
                        saveFileDlg.Title = "Export save";
                        saveFileDlg.FileName = outputFilename;
                        saveFileDlg.Filter = "PSXGameEdit single save (*.mcs)|*.mcs|PS3 signed save (*.PSV)|*.PSV|XP, AR, GS, Caetla single save (*.psx)|*.psx|Memory Juggler (*.ps1)|*.ps1|Smart Link (*.mcb)|*.mcb|Datel (*.mcx;*.pda)|*.mcx;*.pda|RAW single save|B???????????*";

                        //If user selected a card save to it
                        if (saveFileDlg.ShowDialog() == DialogResult.OK)
                        {
                            //Get save type
                            switch (saveFileDlg.FilterIndex)
                            {
                                default:        //Action Replay
                                    singleSaveType = 1;
                                    break;

                                case 1:         //MCS single save
                                case 4:         //PS1 (Memory Juggler)
                                    singleSaveType = 2;
                                    break;

                                case 7:         //RAW single save
                                    singleSaveType = 3;

                                    //Omit the extension if the user left it
                                    saveFileDlg.FileName = saveFileDlg.FileName.Split('.')[0];
                                    break;
                                case 2:         //PS3 signed save
                                    singleSaveType = 4;
                                    break;
                            }
                            PScard[listIndex].saveSingleSave(saveFileDlg.FileName, slotNumber, singleSaveType);
                        }
                        break;
                    case 4:         //Deleted initial
                        MessageBox.Show("Deleted saves cannot be exported. Restore a save to proceed.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case 2:
                    case 3:
                    case 5:
                    case 6:
                        MessageBox.Show("The selected slot is linked. Select the initial save slot to proceed.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
        }

        //Import a save
        private void importSaveDialog()
        {
            //Check if there are any cards available
            if (PScard.Count > 0)
            {
                int listIndex = mainTabControl.SelectedIndex;

                //Check if a save is selected
                if (cardList[listIndex].SelectedIndices.Count == 0) return;

                int slotNumber = cardList[listIndex].SelectedIndices[0];
                int requiredSlots = 0;

                //Check if the slot to import the save on is free
                if (PScard[listIndex].saveType[slotNumber] == 0)
                {
                    OpenFileDialog openFileDlg = new OpenFileDialog();
                    openFileDlg.Title = "Import save";
                    openFileDlg.Filter = "All supported|*.mcs;*.psx;*.ps1;*.mcb;*.mcx;*.pda;B???????????*;*.psv|PSXGameEdit single save (*.mcs)|*.mcs|XP, AR, GS, Caetla single save (*.psx)|*.psx|Memory Juggler (*.ps1)|*.ps1|Smart Link (*.mcb)|*.mcb|Datel (*.mcx;*.pda)|*.mcx;*.pda|RAW single save|B???????????*|PS3 virtual save (*.psv)|*.psv";

                    //If user selected a card save to it
                    if (openFileDlg.ShowDialog() == DialogResult.OK)
                    {
                        if (PScard[listIndex].openSingleSave(openFileDlg.FileName, slotNumber, out requiredSlots))
                        {
                            refreshListView(listIndex, slotNumber);
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
        }

        //Get region string from region data
        private string getRegionString(ushort regionUshort)
        {
            byte[] tempRegion = new byte[3];

            //Convert region to byte array
            tempRegion[0] = (byte)(regionUshort & 0xFF);
            tempRegion[1] = (byte)(regionUshort >> 8);

            //Get UTF-16 string
            return Encoding.Default.GetString(tempRegion);
        }



        //Open preferences window
        private void editPreferences()
        {
            preferencesWindow prefDlg = new preferencesWindow();

            prefDlg.hostWindow = this;
            prefDlg.initializeDialog(mainSettings);

            prefDlg.ShowDialog(this);

            prefDlg.Dispose();
        }

        //Open edit icon dialog
        private void editIcon()
        {
            //Check if there are any cards available
            if (PScard.Count > 0)
            {
                int listIndex = mainTabControl.SelectedIndex;

                //Check if a save is selected
                if (cardList[listIndex].SelectedIndices.Count == 0) return;

                int slotNumber = cardList[listIndex].SelectedIndices[0];
                int iconFrames = PScard[listIndex].iconFrames[slotNumber];
                string saveTitle = PScard[listIndex].saveName[slotNumber, mainSettings.titleEncoding];
                byte[] iconBytes = PScard[listIndex].getIconBytes(slotNumber);

                //Check the save type
                switch (PScard[listIndex].saveType[slotNumber])
                {
                    default:
                        break;

                    case 1:         //Initial save
                    case 4:         //Deleted initial
                        iconWindow iconDlg = new iconWindow();
                        iconDlg.initializeDialog(saveTitle, iconFrames, iconBytes);
                        iconDlg.ShowDialog(this);

                        //Update data if OK has been pressed
                        if (iconDlg.okPressed)
                        {
                            PScard[listIndex].setIconBytes(slotNumber, iconDlg.iconData);
                            refreshListView(listIndex, slotNumber);
                        }

                        iconDlg.Dispose();
                        break;

                    case 2:
                    case 3:
                    case 5:
                    case 6:
                        MessageBox.Show("The selected slot is linked. Select the initial save slot to proceed.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
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
            new AboutWindow().initDialog(this, appName, appVersion, appDate, "Copyright © Shendo 2021", "Beta testers: Gamesoul Master, Xtreme2damax,\nCarmax91.\n\nThanks to: @ruantec, Cobalt, TheCloudOfSmoke,\nRedawgTS, Hard core Rikki, RainMotorsports,\nZieg, Bobbi, OuTman, Kevstah2004, Kubusleonidas, \nFrédéric Brière, Cor'e, Gemini, DeadlySystem.\n\n" +
                "Special thanks to the following people whose\nMemory Card utilities inspired me to write my own:\nSimon Mallion (PSXMemTool),\nLars Ole Dybdal (PSXGameEdit),\nAldo Vargas (Memory Card Manager),\nNeill Corlett (Dexter),\nPaul Phoneix (ConvertM).");
        }

        //Make a new ListView control
        private void makeListView()
        {
            //Add a new ImageList to hold the card icons
            iconList.Add(new ImageList());
            iconList[iconList.Count - 1].ImageSize = new Size((int)(xScale * 48), (int)(yScale * 16));
            iconList[iconList.Count - 1].ColorDepth = ColorDepth.Depth32Bit;
            iconList[iconList.Count - 1].TransparentColor = Color.Magenta;

            cardList.Add(new ListView());
            cardList[cardList.Count - 1].Location = new Point(0, 3);
            cardList[cardList.Count - 1].Dock = DockStyle.Fill;
            cardList[cardList.Count - 1].SmallImageList = iconList[iconList.Count - 1];
            cardList[cardList.Count - 1].ContextMenuStrip = mainContextMenu;
            cardList[cardList.Count - 1].FullRowSelect = true;
            cardList[cardList.Count - 1].MultiSelect = false;
            cardList[cardList.Count - 1].HeaderStyle = ColumnHeaderStyle.Nonclickable;
            cardList[cardList.Count - 1].HideSelection = false;
            cardList[cardList.Count - 1].Columns.Add("Icon, region and title");
            cardList[cardList.Count - 1].Columns.Add("Product code");
            cardList[cardList.Count - 1].Columns.Add("Identifier");
            cardList[cardList.Count - 1].Columns[0].Width = (int)(xScale * 322);
            cardList[cardList.Count - 1].Columns[1].Width = (int)(xScale * 87);
            cardList[cardList.Count - 1].Columns[2].Width = (int)(xScale * 87);
            cardList[cardList.Count - 1].View = View.Details;
            cardList[cardList.Count - 1].DoubleClick += new System.EventHandler(this.cardList_DoubleClick);
            cardList[cardList.Count - 1].SelectedIndexChanged += new System.EventHandler(this.cardList_IndexChanged);

            refreshListView(cardList.Count - 1,0);
        }
        
        //Refresh the ListView
        private void refreshListView(int listIndex, int slotNumber)
        {
            //Temporary FontFamily
            FontFamily tempFontFamily = null;

            //Place cardName on the tab
            mainTabControl.TabPages[listIndex].Text = PScard[listIndex].cardName;

            //Remove all icons from the list
            iconList[listIndex].Images.Clear();

            //Remove all items from the list
            cardList[listIndex].Items.Clear();

            //Add linked slot icons to iconList
            iconList[listIndex].Images.Add(Properties.Resources.linked);
            iconList[listIndex].Images.Add(Properties.Resources.linked_disabled);

            //Add 15 List items along with icons
            for (int i = 0; i < 15; i++)
            {
                //Add save icons to the list
                iconList[listIndex].Images.Add(prepareIcons(listIndex,i));

                switch (PScard[listIndex].saveType[i])
                {
                    default:        //Corrupted
                        cardList[listIndex].Items.Add("Corrupted slot");
                        break;

                    case 0:         //Formatted save
                        cardList[listIndex].Items.Add("Free slot");
                        break;

                    case 1:         //Initial save
                    case 4:         //Deleted initial save
                        cardList[listIndex].Items.Add(PScard[listIndex].saveName[i, mainSettings.titleEncoding]);
                        cardList[listIndex].Items[i].SubItems.Add(PScard[listIndex].saveProdCode[i]);
                        cardList[listIndex].Items[i].SubItems.Add(PScard[listIndex].saveIdentifier[i]);
                        cardList[listIndex].Items[i].ImageIndex = i + 2;      //Skip two linked slot icons
                        break;

                    case 2:         //Middle link
                        cardList[listIndex].Items.Add("Linked slot (middle link)");
                        cardList[listIndex].Items[i].ImageIndex = 0;
                        break;

                    case 5:         //Middle link deleted
                        cardList[listIndex].Items.Add("Linked slot (middle link)");
                        cardList[listIndex].Items[i].ImageIndex = 1;
                        break;

                    case 3:         //End link
                        cardList[listIndex].Items.Add("Linked slot (end link)");
                        cardList[listIndex].Items[i].ImageIndex = 0;
                        break;

                    case 6:         //End link deleted
                        cardList[listIndex].Items.Add("Linked slot (end link)");
                        cardList[listIndex].Items[i].ImageIndex = 1;
                        break;

                }
            }

            //Select the active item in the list
            cardList[listIndex].Items[slotNumber].Selected = true;

            //Set font for the list
            if (mainSettings.listFont != null)
            {
                //Create FontFamily from font name
                tempFontFamily = new FontFamily(mainSettings.listFont);

                //Check if regular style is supported
                if (tempFontFamily.IsStyleAvailable(FontStyle.Regular))
                {
                    //Use custom font
                    cardList[listIndex].Font = new Font(mainSettings.listFont, 8.25f);
                }
                else
                {
                    //Use default font
                    mainSettings.listFont = FontFamily.GenericSansSerif.Name;
                    cardList[listIndex].Font = new Font(mainSettings.listFont, 8.25f);
                }
            }

            //Set showListGrid option
            if (mainSettings.showListGrid == 0) cardList[listIndex].GridLines = false; else cardList[listIndex].GridLines = true;

            refreshPluginBindings();

            //Enable certain list items
            enableSelectiveEditItems();
        }

        //Prepare icons for drawing (add flags and make them transparent if save is deleted)
        private Bitmap prepareIcons(int listIndex, int slotNumber)
        {
            Bitmap iconBitmap = new Bitmap(48, 16);
            Graphics iconGraphics = Graphics.FromImage(iconBitmap);

            //Check what background color should be set
            switch (mainSettings.iconBackgroundColor)
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
            iconGraphics.DrawImage(PScard[listIndex].iconData[slotNumber, 0], 0, 0, 16, 16);

            //Draw flag depending of the region
            switch (PScard[listIndex].saveRegion[slotNumber])
            {
                default:        //Formatted save, Corrupted save, Unknown region
                    iconGraphics.DrawImage(Properties.Resources.naflag, 17, 0, 30, 16);
                    break;

                case 0x4142:    //American region
                    iconGraphics.DrawImage(Properties.Resources.amflag, 17, 0, 30, 16);
                    break;

                case 0x4542:    //European region
                    iconGraphics.DrawImage(Properties.Resources.euflag, 17, 0, 30, 16);
                    break;

                case 0x4942:    //Japanese region
                    iconGraphics.DrawImage(Properties.Resources.jpflag, 17, 0, 30, 16);
                    break;
            }

            //Check if save is deleted and color the icon
            if(PScard[listIndex].saveType[slotNumber] == 4)
                iconGraphics.FillRegion(new SolidBrush(Color.FromArgb(0xA0,0xFF,0xFF,0xFF)), new Region(new Rectangle(0, 0, 16, 16)));

            iconGraphics.Dispose();

            return iconBitmap;
        }

        //Refresh the toolstrip
        private void refreshStatusStrip()
        {
            //Show the location of the active card in the tool strip (if there are any cards)
            if (PScard.Count > 0)
                toolString.Text = PScard[mainTabControl.SelectedIndex].cardLocation;
            else
                toolString.Text = null;

            //If glass is enabled repaint the form
            if(windowGlass.isGlassSupported() && mainSettings.glassStatusBar == 1)this.Refresh();
        }

        //Save work and close the application
        private void exitApplication(FormClosingEventArgs e)
        {
            //Close every opened card
            if (closeAllCards() == 1)
                e.Cancel = true;

            //Save settings
            saveProgramSettings();
        }

        //Edit header of the selected save
        private void editSaveHeader()
        {
            //Check if there are any cards to edit
            if (PScard.Count > 0)
            {
                int listIndex = mainTabControl.SelectedIndex;

                //Check if a save is selected
                if (cardList[listIndex].SelectedIndices.Count == 0) return;

                int slotNumber = cardList[listIndex].SelectedIndices[0];
                ushort saveRegion = PScard[listIndex].saveRegion[slotNumber];
                string saveProdCode = PScard[listIndex].saveProdCode[slotNumber];
                string saveIdentifier = PScard[listIndex].saveIdentifier[slotNumber];
                string saveTitle = PScard[listIndex].saveName[slotNumber, mainSettings.titleEncoding];

                //Check if slot is allowed to be edited
                switch(PScard[listIndex].saveType[slotNumber])
                {
                    default:        //Not allowed
                        break;

                    case 1:
                    case 4:
                        headerWindow headerDlg = new headerWindow();

                        //Load values to dialog
                        headerDlg.initializeDialog(appName, saveTitle, saveProdCode, saveIdentifier, saveRegion);
                        headerDlg.ShowDialog(this);

                        //Update values if OK was pressed
                        if (headerDlg.okPressed)
                        {
                            //Insert data to save header of the selected card and slot
                            PScard[listIndex].setHeaderData(slotNumber, headerDlg.prodCode, headerDlg.saveIdentifier, headerDlg.saveRegion);
                            refreshListView(listIndex, slotNumber);
                        }
                        headerDlg.Dispose();
                    break;
                }
            }
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

        private void Form1_Load(object sender, EventArgs e)
        {
            //Show name of the application on the mainWindow
            this.Text = appName + " " + appVersion;

            //Set default settings
            mainSettings.titleEncoding = 0;
            mainSettings.listFont = FontFamily.GenericSansSerif.Name;
            mainSettings.showListGrid = 0;
            mainSettings.iconInterpolationMode = 0;
            mainSettings.iconPropertiesSize = 1;
            mainSettings.backupMemcards = 0;
            mainSettings.warningMessage = 1;
            mainSettings.restoreWindowPosition = 0;
            mainSettings.communicationPort = "COM1";
            mainSettings.formatType = 0;
            mainSettings.glassStatusBar = 1;

            //Load settings from Settings.ini
            loadProgramSettings();

            //Load available plugins
            pluginSystem.fetchPlugins(appPath + "/Plugins");

            //Create an empty card upon startup or load one given by the command line
            if(loadCommandLine() == false)openCard(null);

            //Apply Aero glass effect to the window
            applyGlass();
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

            //Check if there are any cards
            if (cardList.Count > 0)
            {
                int listIndex = mainTabControl.SelectedIndex;

                //Check if any item on the list is selected
                if (cardList[listIndex].SelectedItems.Count > 0)
                {
                    int slotNumber = cardList[listIndex].SelectedIndices[0];

                    //Check the save type
                    switch (PScard[listIndex].saveType[slotNumber])
                    {
                        default:
                            break;

                        case 1:         //Initial save
                        case 4:         //Deleted initial

                            //Get the supported plugins
                            supportedPlugins = pluginSystem.getSupportedPlugins(PScard[listIndex].saveProdCode[slotNumber]);

                            //Check if there are any plugins that support the product code
                            if (supportedPlugins != null)
                            {
                                //Enable plugin menu
                                editWithPluginToolStripMenuItem.Enabled = true;
                                editWithPluginToolStripMenuItem1.Enabled = true;

                                foreach (int currentAssembly in supportedPlugins)
                                {
                                    //Add item to the plugin menu
                                    editWithPluginToolStripMenuItem.DropDownItems.Add(pluginSystem.assembliesMetadata[currentAssembly].pluginName);
                                    editWithPluginToolStripMenuItem1.DropDownItems.Add(pluginSystem.assembliesMetadata[currentAssembly].pluginName);
                                }
                            }
                            break;
                    }
                }
            }
        }

        //Edit a selected save with a selected plugin
        private void editWithPlugin(int pluginIndex)
        {
            //Check if there are any cards to edit
            if (PScard.Count > 0)
            {
                //Show backup warning message
                if (mainSettings.warningMessage == 1)
                {
                    DialogResult result = MessageBox.Show("Save editing may potentially corrupt the save.\nDo you want to proceed with this operation?", 
                                            appName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result != DialogResult.Yes) return;
                }

                int listIndex = mainTabControl.SelectedIndex;
                int slotNumber = cardList[listIndex].SelectedIndices[0];
                int reqSlots = 0;
                byte[] editedSaveBytes = pluginSystem.editSaveData(supportedPlugins[pluginIndex], PScard[listIndex].getSaveBytes(slotNumber), PScard[listIndex].saveProdCode[slotNumber]);

                if (editedSaveBytes != null)
                {
                    //Delete save so the edited one can be placed in.
                    PScard[listIndex].formatSave(slotNumber);

                    PScard[listIndex].setSaveBytes(slotNumber, editedSaveBytes, out reqSlots);

                    //Refresh the list with new data
                    refreshListView(listIndex, slotNumber);

                    //Set the edited flag of the card
                    PScard[listIndex].changedFlag = true;
                }
            }
        }

        private void readMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Check if Readme.txt exists
            if (File.Exists(appPath + "/Readme.txt")) System.Diagnostics.Process.Start(appPath + "/Readme.txt");
            else MessageBox.Show("'ReadMe.txt' was not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        //Disable all items related to save editing
        private void disableEditItems()
        {
            //Edit menu
            editSaveHeaderToolStripMenuItem.Enabled = false;
            editSaveCommentToolStripMenuItem.Enabled = false;
            compareWithTempBufferToolStripMenuItem.Enabled = false;
            editIconToolStripMenuItem.Enabled = false;
            deleteSaveToolStripMenuItem.Enabled = false;
            restoreSaveToolStripMenuItem.Enabled = false;
            removeSaveformatSlotsToolStripMenuItem.Enabled = false;
            copySaveToTempraryBufferToolStripMenuItem.Enabled = false;
            pasteSaveFromTemporaryBufferToolStripMenuItem.Enabled = false;
            importSaveToolStripMenuItem.Enabled = false;
            exportSaveToolStripMenuItem.Enabled = false;

            //Edit toolbar
            editHeaderButton.Enabled = false;
            commentsButton.Enabled = false;
            editIconButton.Enabled = false;
            importButton.Enabled = false;
            exportButton.Enabled = false;

            //Edit popup
            editSaveHeaderToolStripMenuItem1.Enabled = false;
            editSaveCommentsToolStripMenuItem.Enabled = false;
            compareWithTempBufferToolStripMenuItem1.Enabled = false;
            editIconToolStripMenuItem1.Enabled = false;
            deleteSaveToolStripMenuItem1.Enabled = false;
            restoreSaveToolStripMenuItem1.Enabled = false;
            removeSaveformatSlotsToolStripMenuItem1.Enabled = false;
            copySaveToTempBufferToolStripMenuItem.Enabled = false;
            paseToolStripMenuItem.Enabled = false;
            importSaveToolStripMenuItem1.Enabled = false;
            exportSaveToolStripMenuItem1.Enabled = false;
            saveInformationToolStripMenuItem.Enabled = false;
        }

        //Enable all items related to save editing
        private void enableEditItems()
        {
            //Edit menu
            editSaveHeaderToolStripMenuItem.Enabled = true;
            editSaveCommentToolStripMenuItem.Enabled = true;
            editIconToolStripMenuItem.Enabled = true;
            deleteSaveToolStripMenuItem.Enabled = true;
            restoreSaveToolStripMenuItem.Enabled = true;
            removeSaveformatSlotsToolStripMenuItem.Enabled = true;
            copySaveToTempraryBufferToolStripMenuItem.Enabled = true;
            pasteSaveFromTemporaryBufferToolStripMenuItem.Enabled = true;
            importSaveToolStripMenuItem.Enabled = true;
            exportSaveToolStripMenuItem.Enabled = true;

            //Edit toolbar
            editHeaderButton.Enabled = true;
            commentsButton.Enabled = true;
            editIconButton.Enabled = true;
            importButton.Enabled = true;
            exportButton.Enabled = true;

            //Edit popup
            editSaveHeaderToolStripMenuItem1.Enabled = true;
            editSaveCommentsToolStripMenuItem.Enabled = true;
            editIconToolStripMenuItem1.Enabled = true;
            deleteSaveToolStripMenuItem1.Enabled = true;
            restoreSaveToolStripMenuItem1.Enabled = true;
            removeSaveformatSlotsToolStripMenuItem1.Enabled = true;
            copySaveToTempBufferToolStripMenuItem.Enabled = true;
            paseToolStripMenuItem.Enabled = true;
            importSaveToolStripMenuItem1.Enabled = true;
            exportSaveToolStripMenuItem1.Enabled = true;
            saveInformationToolStripMenuItem.Enabled = true;

            //Temp buffer related
            if (tempBuffer != null)
            {
                compareWithTempBufferToolStripMenuItem.Enabled = true;
                compareWithTempBufferToolStripMenuItem1.Enabled = true;
            }
            else
            {
                compareWithTempBufferToolStripMenuItem.Enabled = false;
                compareWithTempBufferToolStripMenuItem1.Enabled = false;
            }
        }

        //Enable only supported edit operations
        private void enableSelectiveEditItems()
        {
            //Check if there are any cards
            if (cardList.Count > 0)
            {
                int listIndex = mainTabControl.SelectedIndex;

                //Check if any item on the list is selected
                if (cardList[listIndex].SelectedItems.Count > 0)
                {
                    int slotNumber = cardList[listIndex].SelectedIndices[0];

                    //Check the save type
                    switch (PScard[listIndex].saveType[slotNumber])
                    {
                        default:
                            break;

                        case 0:         //Formatted
                            disableEditItems();
                            pasteSaveFromTemporaryBufferToolStripMenuItem.Enabled = true;
                            paseToolStripMenuItem.Enabled = true;
                            importSaveToolStripMenuItem.Enabled = true;
                            importSaveToolStripMenuItem1.Enabled = true;
                            importButton.Enabled = true;
                            break;

                        case 1:         //Initial
                            enableEditItems();
                            restoreSaveToolStripMenuItem.Enabled = false;
                            restoreSaveToolStripMenuItem1.Enabled = false;
                            pasteSaveFromTemporaryBufferToolStripMenuItem.Enabled = false;
                            paseToolStripMenuItem.Enabled = false;
                            importSaveToolStripMenuItem.Enabled = false;
                            importSaveToolStripMenuItem1.Enabled = false;
                            importButton.Enabled = false;
                            break;

                        case 2:         //Middle link
                        case 3:         //End link
                            disableEditItems();
                            break;

                        case 4:         //Deleted initial
                            enableEditItems();
                            deleteSaveToolStripMenuItem.Enabled = false;
                            deleteSaveToolStripMenuItem1.Enabled = false;
                            pasteSaveFromTemporaryBufferToolStripMenuItem.Enabled = false;
                            paseToolStripMenuItem.Enabled = false;
                            importSaveToolStripMenuItem.Enabled = false;
                            importSaveToolStripMenuItem1.Enabled = false;
                            importButton.Enabled = false;
                            exportSaveToolStripMenuItem.Enabled = false;
                            exportSaveToolStripMenuItem1.Enabled = false;
                            exportButton.Enabled = false;
                            break;

                        case 5:         //Deleted middle link
                        case 6:         //Deleted end link
                            disableEditItems();
                            break;

                        case 7:         //Corrupted
                            disableEditItems();
                            removeSaveformatSlotsToolStripMenuItem.Enabled = true;
                            removeSaveformatSlotsToolStripMenuItem1.Enabled = true;
                            break;
                    }
                }
                else
                {
                    //No save is selected, disable all items
                    disableEditItems();
                }
            }
            else
            {
                //There is no card, disable all items
                disableEditItems();
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

            //Check if there are any cards available
            if (PScard.Count > 0)
            {
                int listIndex = mainTabControl.SelectedIndex;

                //Check if a save is selected
                if (cardList[listIndex].SelectedIndices.Count == 0) return;

                int slotNumber = cardList[listIndex].SelectedIndices[0];

                //Check the save type
                switch (PScard[listIndex].saveType[slotNumber])
                {
                    default:
                        break;

                    case 1:         //Initial save
                    case 4:         //Deleted initial

                        //Get data to work with
                        fetchedData = PScard[listIndex].getSaveBytes(slotNumber);
                        fetchedDataTitle = PScard[listIndex].saveName[slotNumber, mainSettings.titleEncoding];

                        //Check if selected saves have the same size
                        if (fetchedData.Length != tempBuffer.Length)
                        {
                            MessageBox.Show("Save file size mismatch. Saves can't be compared.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        //Show compare window
                        new compareWindow().initializeDialog(this, appName, fetchedData, fetchedDataTitle, tempBuffer, tempBufferName + " (temp buffer)");

                        break;

                    case 2:
                    case 3:
                    case 5:
                    case 6:
                        MessageBox.Show("The selected slot is linked. Select the initial save slot to proceed.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
        }

        //Read a Memory Card from the physical device
        private void cardReaderRead(byte[] readData)
        {
            //Check if the Memory Card was sucessfully read
            if (readData != null)
            {
                //Create a new card
                PScard.Add(new ps1card());

                //Fill the card with the new data
                PScard[PScard.Count - 1].openMemoryCardStream(readData, getSettingsBool(mainSettings.fixCorruptedCards));

                //Temporary set a bogus file location (to fool filterNullCard function)
                PScard[PScard.Count - 1].cardLocation = "\0";

                //Create a tab page for the new card
                createTabPage();

                //Restore null location since DexDrive Memory Card is not a file present on the Hard Disk
                PScard[PScard.Count - 1].cardLocation = null;
            }
        }


        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Show the location of the active card in the tool strip
            refreshStatusStrip();

            //Load available plugins for the selected save
            refreshPluginBindings();

            //Enable certain list items
            enableSelectiveEditItems();
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
            //Application.Exit();
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

        private void cardList_IndexChanged(object sender, EventArgs e)
        {
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

        private void deleteSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Delete selected save
            deleteSave();
        }

        private void deleteSaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Delete selected save
            deleteSave();
        }

        private void restoreSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Restore deleted save
            restoreSave();
        }

        private void restoreSaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Restore deleted save
            restoreSave();
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

        private void removeSaveformatSlotsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Format selected save
            formatSave();
        }

        private void removeSaveformatSlotsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Format selected save
            formatSave();
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

        private void copySaveToTempraryBufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Store selected save to temp buffer
            copySave();
        }

        private void copySaveToTempBufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Store selected save to temp buffer
            copySave();
        }

        private void exportSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Export a save
            exportSaveDialog();
        }

        private void exportSaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Export a save
            exportSaveDialog();
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            //Export a save
            exportSaveDialog();
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

        private void mainWindow_Paint(object sender, PaintEventArgs e)
        {
            if (windowGlass.isGlassSupported() && mainSettings.glassStatusBar == 1)
            {
                Brush blackBrush = new SolidBrush(Color.Black);
                Graphics windowGraphics = e.Graphics;

                //Create a black rectangle for Aero glass
                windowGraphics.FillRectangle(blackBrush, windowRectangle);

                //Show path of the currently active card
                windowGlass.DrawTextOnGlass(this.Handle, toolString.Text, new Font("Segoe UI", 9f, FontStyle.Regular), windowRectangle, 10);
            }
        }

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            //DWM composition is changed and glass support should be revaluated
            if (message.Msg == glassSupport.WM_DWMCOMPOSITIONCHANGED)
            {
                applyGlass();
            }

            //Move the window if user clicked on the glass area
            if (message.Msg == glassSupport.WM_NCHITTEST && message.Result.ToInt32() == glassSupport.HTCLIENT)
            {
                //Check if DWM composition is enabled
                if (windowGlass.isGlassSupported() && mainSettings.glassStatusBar == 1)
                {
                    int mouseX = (message.LParam.ToInt32() & 0xFFFF);
                    int mouseY = (message.LParam.ToInt32() >> 16);
                    Point windowPoint = this.PointToClient(new Point(mouseX,mouseY));

                    //Check if the clicked area is on glass
                    if(windowRectangle.Contains(windowPoint)) message.Result = new IntPtr(glassSupport.HTCAPTION);
                }
            }
        }

        private void dexDriveMenuRead_Click(object sender, EventArgs e)
        {
            //Read a Memory Card from DexDrive
            byte[] tempByteArray = new cardReaderWindow().readMemoryCardDexDrive(this, appName, mainSettings.communicationPort);

            cardReaderRead(tempByteArray);
        }

        private void dexDriveMenuWrite_Click(object sender, EventArgs e)
        {
            //Write a Memory Card to DexDrive
            int listIndex = mainTabControl.SelectedIndex;

            //Check if there are any cards to write
            if (PScard.Count > 0)
            {
                //Open a DexDrive communication window
                new cardReaderWindow().writeMemoryCardDexDrive(this, appName, mainSettings.communicationPort, PScard[listIndex].saveMemoryCardStream(getSettingsBool(mainSettings.fixCorruptedCards)), 1024);
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

        private void memCARDuinoMenuRead_Click(object sender, EventArgs e)
        {
            //Read a Memory Card from MemCARDuino
            byte[] tempByteArray = new cardReaderWindow().readMemoryCardCARDuino(this, appName, mainSettings.communicationPort);

            cardReaderRead(tempByteArray);
        }

        private void memCARDuinoMenuWrite_Click(object sender, EventArgs e)
        {
            //Write a Memory Card to MemCARDuino
            int listIndex = mainTabControl.SelectedIndex;

            //Check if there are any cards to write
            if (PScard.Count > 0)
            {
                //Open a DexDrive communication window
                new cardReaderWindow().writeMemoryCardCARDuino(this, appName, mainSettings.communicationPort, PScard[listIndex].saveMemoryCardStream(getSettingsBool(mainSettings.fixCorruptedCards)), 1024);
            }
        }

        private void pS1CardLinkMenuRead_Click(object sender, EventArgs e)
        {
            //Read a Memory Card from PS1CardLink
            byte[] tempByteArray = new cardReaderWindow().readMemoryCardPS1CLnk(this, appName, mainSettings.communicationPort);

            cardReaderRead(tempByteArray);
        }

        //Write a Memory Card to PS1CardLink
        private void pS1CardLinkMenuWrite_Click(object sender, EventArgs e)
        {
            int listIndex = mainTabControl.SelectedIndex;

            //Check if there are any cards to write
            if (PScard.Count > 0)
            {
                //Open a DexDrive communication window
                new cardReaderWindow().writeMemoryCardPS1CLnk(this, appName, mainSettings.communicationPort, PScard[listIndex].saveMemoryCardStream(getSettingsBool(mainSettings.fixCorruptedCards)), 1024);
            }
        }

        private void pS3MemoryCardAdaptorMenuRead_Click(object sender, EventArgs e)
        {
            //Read a Memory Card from PS3 Memory Card Adaptor
            byte[] tempByteArray = new cardReaderWindow().readMemoryCardPS3MCA(this, appName, mainSettings.communicationPort);

            cardReaderRead(tempByteArray);
        }

        private void pS3MemoryCardAdaptorMenuWrite_Click(object sender, EventArgs e)
        {
            //Write a Memory Card to PS3 Memory Card Adaptor
            int listIndex = mainTabControl.SelectedIndex;

            //Check if there are any cards to write
            if (PScard.Count > 0)
            {
                //Open a PS3 Memory Card Adaptor communication window
                new cardReaderWindow().writeMemoryCardPS3MCA(this, appName, mainSettings.communicationPort, PScard[listIndex].saveMemoryCardStream(getSettingsBool(mainSettings.fixCorruptedCards)), 1024);
            }
        }

        private void dexDriveMenuFormat_Click(object sender, EventArgs e)
        {
            //Format a Memory Card on DexDrive
            formatHardwareCard(0);
        }

        private void memCARDuinoMenuFormat_Click(object sender, EventArgs e)
        {
            //Format a Memory Card on MemCARDuino
            formatHardwareCard(1);
        }

        private void pS1CardLinkMenuFormat_Click(object sender, EventArgs e)
        {
            //Format a Memory Card on PS1CardLink
            formatHardwareCard(2);
        }

        private void pS3MemoryCardAdaptorMenuFormat_Click(object sender, EventArgs e)
        {
            //Format a Memory Card on PS3 Memory Card Adaptor
            formatHardwareCard(3);
        }

        //Format a Memory Card on the hardware interface (0 - DexDrive, 1 - MemCARDuino, 2 - PS1CardLink)
        private void formatHardwareCard(int hardDevice)
        {
            //Show warning message
            DialogResult result = MessageBox.Show("Formatting will delete all saves on the Memory Card.\nDo you want to proceed with this operation?", 
                                    appName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) return;

            int frameNumber = 1024;
            ps1card blankCard = new ps1card();

            //Check if quick format option is turned on
            if (mainSettings.formatType == 0) frameNumber = 64;

            //Create a new card by giving a null path
            blankCard.openMemoryCard(null, true);

            //Check what device to use
            switch (hardDevice)
            {
                case 0:         //DexDrive
                    new cardReaderWindow().writeMemoryCardDexDrive(this, appName, mainSettings.communicationPort, blankCard.saveMemoryCardStream(true), frameNumber);
                    break;

                case 1:         //MemCARDuino
                    new cardReaderWindow().writeMemoryCardCARDuino(this, appName, mainSettings.communicationPort, blankCard.saveMemoryCardStream(true), frameNumber);
                    break;

                case 2:         //PS1CardLink
                    new cardReaderWindow().writeMemoryCardPS1CLnk(this, appName, mainSettings.communicationPort, blankCard.saveMemoryCardStream(true), frameNumber);
                    break;

                case 3:         //PS3 Memory Card Adaptor
                    new cardReaderWindow().writeMemoryCardPS3MCA(this, appName, mainSettings.communicationPort, blankCard.saveMemoryCardStream(true), frameNumber);
                    break;
            }
        }
    }
}
