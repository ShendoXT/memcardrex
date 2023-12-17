using System;
namespace MemcardRex
{
	public class ProgramSettings
	{
        public int IconBackgroundColor = 0;                //Various colors based on PS1 BIOS backgrounds
        public int BackupMemcards = 0;                     //Backup Memory Card settings
        public int RestoreWindowPosition = 0;              //Restore window position
        public int FixCorruptedCards = 0;                  //Try to fix corrupted memory cards
        public int FormatType = 0;                         //Type of formatting for hardware interfaces
        public string CommunicationPort = "";              //Communication port for Hardware interfaces
        public int CommunicationSpeed = 0;                 //Speed setting for serial port
        public int LastSaveFormat = 0;                     //Last used format to save memory card
        public int LastExportFormat = 0;                   //Last used format to export save
        public string RemoteCommAddress = "192.168.4.1";   // Address / hostname of the remote serial bridge host
        public int RemoteCommPort = 23;                    // Port to open a socket for the remote serial bridge
        public int CardSlot = 0;                           //Active card slot for reading data from PS1CardLink or Unirom

        public ProgramSettings()
		{
		}
	}
}

