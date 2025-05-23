using System.IO;

namespace MemcardRex.Core
{
    public class ProgramSettings
    {
        public int ShowListGrid = 0;                       //List grid settings
        public int IconBackgroundColor = 0;                //Various colors based on PS1 BIOS backgrounds
        public int BackupMemcards = 0;                     //Backup Memory Card settings
        public int RestoreWindowPosition = 0;              //Restore window position
        public int FixCorruptedCards = 0;                  //Try to fix corrupted memory cards
        public int FormatType = 0;                         //Type of formatting for hardware interfaces
        public string CommunicationPort = "COM1";          //Communication port for Hardware interfaces
        public int LastSaveFormat = 0;                     //Last used format to save memory card
        public int LastExportFormat = 0;                   //Last used format to export save
        public string RemoteCommAddress = "192.168.4.1";   //Address / hostname of the remote serial bridge host
        public int RemoteCommPort = 23;                    //Port to open a socket for the remote serial bridge
        public int CardSlot = 0;                           //Active card slot for reading data from PS1CardLink or Unirom
        public int ActiveInterface = 0;                    //Currently active hardware interface
        public int WarningMessages = 1;                    //Show warning messages for dangerous tasks
        public int WindowPositionX = 0;                    //Saved window X coordinate
        public int WindowPositionY = 0;                    //Saved window Y coordinate
        public int GridColorValue = 128;                   //Saved grid value for icon editor
        public int IconGridEnabled = 1;                    //Show grid in icon editor

        private const string settingsFilename = "Settings.xml";

        public ProgramSettings()
        {
        }

        /// <summary>
        /// Load settings from permanent storage
        /// </summary>
        /// <param name="directory">Directory to save settings to</param>
        public void LoadSettings(string directory)
        {
            if (!Directory.Exists(directory)) return;

            string filename = Path.Combine(directory, settingsFilename);

            if (!File.Exists(filename)) return;

            xmlSettingsEditor xmlAppSettings = new xmlSettingsEditor();

            //Open XML file for reading, file is auto-closed
            xmlAppSettings.openXmlReader(filename);

            ShowListGrid = xmlAppSettings.readXmlEntryInt("ShowGrid", 0, 1);

            CommunicationPort = xmlAppSettings.readXmlEntry("ComPort");

            RemoteCommAddress = xmlAppSettings.readXmlEntry("RemoteComAddress");
            RemoteCommPort = xmlAppSettings.readXmlEntryInt("RemoteComPort", 0, 65535);

            IconBackgroundColor = xmlAppSettings.readXmlEntryInt("IconBackgroundColor", 0, 4);

            BackupMemcards = xmlAppSettings.readXmlEntryInt("BackupMemoryCards", 0, 1);

            RestoreWindowPosition = xmlAppSettings.readXmlEntryInt("RestoreWindowPosition", 0, 1);

            WindowPositionX = xmlAppSettings.readXmlEntryInt("WindowPositionX", -65535, 65535);

            WindowPositionY = xmlAppSettings.readXmlEntryInt("WindowPositionY", -65535, 65535);

            FormatType = xmlAppSettings.readXmlEntryInt("HardwareFormatType", 0, 1);

            FixCorruptedCards = xmlAppSettings.readXmlEntryInt("FixCorruptedCards", 0, 1);

            LastSaveFormat = xmlAppSettings.readXmlEntryInt("LastSaveFormat", 0, 13);

            LastExportFormat = xmlAppSettings.readXmlEntryInt("LastExportFormat", 0, 7);

            CardSlot = xmlAppSettings.readXmlEntryInt("CardSlot", 0, 1);

            ActiveInterface = xmlAppSettings.readXmlEntryInt("ActiveInterface", 0, 10);

            WarningMessages = xmlAppSettings.readXmlEntryInt("WarningMessages", 0, 1);

            GridColorValue = xmlAppSettings.readXmlEntryInt("GridColorValue", 0, 255);

            IconGridEnabled = xmlAppSettings.readXmlEntryInt("IconGridEnabled", 0, 1);
        }

        /// <summary>
        /// Save settings to permanent storage
        /// </summary>
        /// <param name="filename">Full path to settings file</param>
        public void SaveSettings(string directory, string appName, string appVersion)
        {
            //Create directory if it doesn't exist
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            xmlSettingsEditor xmlAppSettings = new xmlSettingsEditor();

            xmlAppSettings.openXmlWriter(Path.Combine(directory, settingsFilename), appName + " " + appVersion + " settings data");

            xmlAppSettings.writeXmlEntry("ShowGrid", ShowListGrid.ToString());

            xmlAppSettings.writeXmlEntry("ComPort", CommunicationPort);

            xmlAppSettings.writeXmlEntry("RemoteComAddress", RemoteCommAddress);
            xmlAppSettings.writeXmlEntry("RemoteComPort", RemoteCommPort.ToString());

            xmlAppSettings.writeXmlEntry("IconBackgroundColor", IconBackgroundColor.ToString());

            xmlAppSettings.writeXmlEntry("BackupMemoryCards", BackupMemcards.ToString());

            xmlAppSettings.writeXmlEntry("RestoreWindowPosition", RestoreWindowPosition.ToString());

            xmlAppSettings.writeXmlEntry("WindowPositionX", WindowPositionX.ToString());

            xmlAppSettings.writeXmlEntry("WindowPositionY", WindowPositionY.ToString());

            xmlAppSettings.writeXmlEntry("HardwareFormatType", FormatType.ToString());

            xmlAppSettings.writeXmlEntry("FixCorruptedCards", FixCorruptedCards.ToString());

            xmlAppSettings.writeXmlEntry("LastSaveFormat", LastSaveFormat.ToString());

            xmlAppSettings.writeXmlEntry("LastExportFormat", LastExportFormat.ToString());

            xmlAppSettings.writeXmlEntry("CardSlot", CardSlot.ToString());

            xmlAppSettings.writeXmlEntry("ActiveInterface", ActiveInterface.ToString());

            xmlAppSettings.writeXmlEntry("WarningMessages", WarningMessages.ToString());

            xmlAppSettings.writeXmlEntry("GridColorValue", GridColorValue.ToString());

            xmlAppSettings.writeXmlEntry("IconGridEnabled", IconGridEnabled.ToString());

            xmlAppSettings.closeXmlWriter();
        }
    }
}
