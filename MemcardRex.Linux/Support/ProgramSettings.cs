/* Copyright (C) 2009 Shendo
 * SPDX-License-Identifer: GPL-3.0-or-later */

using System;
using System.IO;
using MemcardRex.Core;

namespace MemcardRex.Linux
{
    public class ProgramSettings
    {
        public int ShowListGrid = 0;                        //List grid settings
        public int IconInterpolationMode = 0;               //Icon iterpolation mode settings
        public int IconPropertiesSize = 0;                  //Icon size settings in save properties
        public string ListFont = "Sans 11";                 //Font for the save list
        public int IconBackgroundColor = 0;                //Various colors based on PS1 BIOS backgrounds
        public int BackupMemcards = 0;                     //Backup Memory Card settings
        public int RestoreWindowPosition = 0;              //Restore window position
        public int FixCorruptedCards = 0;                  //Try to fix corrupted memory cards
        public int FormatType = 0;                         //Type of formatting for hardware interfaces
        public string CommunicationPort = "COM1";          //Communication port for Hardware interfaces
        public int CommunicationSpeed = 0;                 //Speed setting for serial port
        public int LastSaveFormat = 0;                     //Last used format to save memory card
        public int LastExportFormat = 0;                   //Last used format to export save
        public string RemoteCommAddress = "192.168.4.1";   // Address / hostname of the remote serial bridge host
        public int RemoteCommPort = 23;                    // Port to open a socket for the remote serial bridge
        public int CardSlot = 0;                           //Active card slot for reading data from PS1CardLink or Unirom

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
            IconInterpolationMode = xmlAppSettings.readXmlEntryInt("IconInterpolationMode", 0, 1);
            IconPropertiesSize = xmlAppSettings.readXmlEntryInt("IconSize", 0, 1);
            ListFont = xmlAppSettings.readXmlEntry("ListFont");

            CommunicationPort = xmlAppSettings.readXmlEntry("ComPort");
            CommunicationSpeed = xmlAppSettings.readXmlEntryInt("ComSpeed", 0, 1);

            RemoteCommAddress = xmlAppSettings.readXmlEntry("RemoteComAddress");
            RemoteCommPort = xmlAppSettings.readXmlEntryInt("RemoteComPort", 0, 65535);

            IconBackgroundColor = xmlAppSettings.readXmlEntryInt("IconBackgroundColor", 0, 4);

            BackupMemcards = xmlAppSettings.readXmlEntryInt("BackupMemoryCards", 0, 1);

            RestoreWindowPosition = xmlAppSettings.readXmlEntryInt("RestoreWindowPosition", 0, 1);

            FormatType = xmlAppSettings.readXmlEntryInt("HardwareFormatType", 0, 1);

            FixCorruptedCards = xmlAppSettings.readXmlEntryInt("FixCorruptedCards", 0, 1);

            LastSaveFormat = xmlAppSettings.readXmlEntryInt("LastSaveFormat", 0, 13);

            LastExportFormat = xmlAppSettings.readXmlEntryInt("LastExportFormat", 0, 7);

            CardSlot = xmlAppSettings.readXmlEntryInt("CardSlot", 0, 1);
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
            xmlAppSettings.writeXmlEntry("IconInterpolationMode", IconInterpolationMode.ToString());
            xmlAppSettings.writeXmlEntry("IconSize", IconPropertiesSize.ToString());
            xmlAppSettings.writeXmlEntry("ListFont", ListFont);

            xmlAppSettings.writeXmlEntry("ComPort", CommunicationPort);
            xmlAppSettings.writeXmlEntry("ComSpeed", CommunicationSpeed.ToString());

            xmlAppSettings.writeXmlEntry("RemoteComAddress", RemoteCommAddress);
            xmlAppSettings.writeXmlEntry("RemoteComPort", RemoteCommPort.ToString());

            xmlAppSettings.writeXmlEntry("IconBackgroundColor", IconBackgroundColor.ToString());

            xmlAppSettings.writeXmlEntry("BackupMemoryCards", BackupMemcards.ToString());

            xmlAppSettings.writeXmlEntry("RestoreWindowPosition", RestoreWindowPosition.ToString());

            xmlAppSettings.writeXmlEntry("HardwareFormatType", FormatType.ToString());

            xmlAppSettings.writeXmlEntry("FixCorruptedCards", FixCorruptedCards.ToString());

            xmlAppSettings.writeXmlEntry("LastSaveFormat", LastSaveFormat.ToString());

            xmlAppSettings.writeXmlEntry("LastExportFormat", LastExportFormat.ToString());

            xmlAppSettings.writeXmlEntry("CardSlot", CardSlot.ToString());

            xmlAppSettings.closeXmlWriter();
        }
    }
}

