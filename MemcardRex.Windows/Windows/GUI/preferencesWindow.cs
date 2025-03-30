using System;
using System.Windows.Forms;
using System.IO.Ports;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace MemcardRex
{
    [SupportedOSPlatform("windows")]
    public partial class preferencesWindow : Form
    {
        //Window that hosted this dialog
        mainWindow hostWindow;

        //Saved Hardware COM port (in case it's a removable USB adapter)
        string SavedComPort = null;

        public preferencesWindow()
        {
            InitializeComponent();
        }

        //Load default values
        public void initializeDialog(mainWindow window, List<mainWindow.HardInterfaces> registeredInterfaces)
        {
            hostWindow = window;

            backgroundCombo.SelectedIndex = hostWindow.appSettings.IconBackgroundColor;
            formatCombo.SelectedIndex = hostWindow.appSettings.FormatType;
            SavedComPort = hostWindow.appSettings.CommunicationPort;
            if (hostWindow.appSettings.ShowListGrid == 1) gridCheckbox.Checked = true; else gridCheckbox.Checked = false;
            if (hostWindow.appSettings.BackupMemcards == 1) backupCheckbox.Checked = true; else backupCheckbox.Checked = false;
            if (hostWindow.appSettings.RestoreWindowPosition == 1) restorePositionCheckbox.Checked = true; else restorePositionCheckbox.Checked = false;
            if (hostWindow.appSettings.FixCorruptedCards == 1) fixCorruptedCardsCheckbox.Checked = true; else fixCorruptedCardsCheckbox.Checked = false;
            if (hostWindow.appSettings.WarningMessages == 1) warningCheckBox.Checked = true; else warningCheckBox.Checked = false;
            remoteAddressBox.Text = hostWindow.appSettings.RemoteCommAddress;
            remotePortUpDown.Value = hostWindow.appSettings.RemoteCommPort;
            cardSlotCombo.SelectedIndex = hostWindow.appSettings.CardSlot;

            //Load all COM ports found on the system
            foreach (string port in SerialPort.GetPortNames())
            {
                dexDriveCombo.Items.Add(port);
            }

            //Load all available hardware interfaces
            foreach(mainWindow.HardInterfaces iface in registeredInterfaces)
            {
                hardwareInterfacesCombo.Items.Add(iface.displayName);
            }

            hardwareInterfacesCombo.SelectedIndex = hostWindow.appSettings.ActiveInterface;

            //If there are no ports disable combobox
            if(dexDriveCombo.Items.Count < 1) dexDriveCombo.Enabled = false;

            //Select a com port (if it exists)
            dexDriveCombo.SelectedItem = hostWindow.appSettings.CommunicationPort;

            this.ShowDialog(hostWindow);
        }

        //Apply configured settings
        private void applySettings()
        {
            hostWindow.appSettings.IconBackgroundColor = backgroundCombo.SelectedIndex;
            hostWindow.appSettings.FormatType = formatCombo.SelectedIndex;
            hostWindow.appSettings.CommunicationPort = SavedComPort;
            hostWindow.appSettings.RemoteCommAddress = remoteAddressBox.Text;
            hostWindow.appSettings.RemoteCommPort = Convert.ToUInt16(remotePortUpDown.Value);
            hostWindow.appSettings.ActiveInterface = hardwareInterfacesCombo.SelectedIndex;
            hostWindow.appSettings.CardSlot = cardSlotCombo.SelectedIndex;

            if (gridCheckbox.Checked) hostWindow.appSettings.ShowListGrid = 1; else hostWindow.appSettings.ShowListGrid = 0;
            if (backupCheckbox.Checked) hostWindow.appSettings.BackupMemcards = 1; else hostWindow.appSettings.BackupMemcards = 0;
            if (warningCheckBox.Checked) hostWindow.appSettings.WarningMessages = 1; else hostWindow.appSettings.WarningMessages = 0;
            if (restorePositionCheckbox.Checked) hostWindow.appSettings.RestoreWindowPosition = 1; else hostWindow.appSettings.RestoreWindowPosition = 0;
            if (fixCorruptedCardsCheckbox.Checked) hostWindow.appSettings.FixCorruptedCards = 1; else hostWindow.appSettings.FixCorruptedCards = 0;

            //hostWindow.appSettings.SaveSettings(hostWindow.appPath, hostWindow.appName, hostWindow.appVersion);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            applySettings();
            this.Close();
        }

        private void dexDriveCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Save the COM port if the user selected a new one
            SavedComPort = dexDriveCombo.Text;
        }
    }
}
