using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace MemcardRex
{
    public partial class preferencesWindow : Form
    {
        //Window that hosted this dialog
        public mainWindow hostWindow;

        //Saved DexDrive COM port (in case it's a removable USB adapter)
        string SavedComPort = null;

        public preferencesWindow()
        {
            InitializeComponent();
        }

        //Load default values
        public void initializeDialog(mainWindow.programSettings progSettings)
        {
            interpolationCombo.SelectedIndex = progSettings.iconInterpolationMode;
            iconSizeCombo.SelectedIndex = progSettings.iconPropertiesSize;
            backgroundCombo.SelectedIndex = progSettings.iconBackgroundColor;
            formatCombo.SelectedIndex = progSettings.formatType;
            SavedComPort = progSettings.communicationPort;
            if (progSettings.showListGrid == 1) gridCheckbox.Checked = true; else gridCheckbox.Checked = false;
            if (progSettings.backupMemcards == 1) backupCheckbox.Checked = true; else backupCheckbox.Checked = false;
            if (progSettings.warningMessage == 1) backupWarningCheckBox.Checked = true; else backupWarningCheckBox.Checked = false;
            if (progSettings.restoreWindowPosition == 1) restorePositionCheckbox.Checked = true; else restorePositionCheckbox.Checked = false;
            if (progSettings.fixCorruptedCards == 1) fixCorruptedCardsCheckbox.Checked = true; else fixCorruptedCardsCheckbox.Checked = false;
            remoteAddressBox.Text = progSettings.remoteCommunicationAddress;
            remotePortUpDown.Value = progSettings.remoteCommunicationPort;
            hardwareSpeedCombo.SelectedIndex = progSettings.communicationSpeed;


            //Load all COM ports found on the system
            foreach (string port in SerialPort.GetPortNames())
            {
                dexDriveCombo.Items.Add(port);
            }

            //If there are no ports disable combobox
            if(dexDriveCombo.Items.Count < 1) dexDriveCombo.Enabled = false;

            //Select a com port (if it exists)
            dexDriveCombo.SelectedItem = progSettings.communicationPort;

            //Load all fonts installed on system
            foreach (FontFamily font in FontFamily.Families)
            {
                //Add the font on the list
                fontCombo.Items.Add(font.Name);
            }

            //Find font used in the save list
            fontCombo.SelectedItem = progSettings.listFont;
        }

        //Apply configured settings
        private void applySettings()
        {
            mainWindow.programSettings progSettings = new mainWindow.programSettings();

            progSettings.iconInterpolationMode = interpolationCombo.SelectedIndex;
            progSettings.iconPropertiesSize = iconSizeCombo.SelectedIndex;
            progSettings.iconBackgroundColor = backgroundCombo.SelectedIndex;
            progSettings.formatType = formatCombo.SelectedIndex;
            progSettings.communicationPort = SavedComPort;
            progSettings.remoteCommunicationAddress = remoteAddressBox.Text;
            progSettings.remoteCommunicationPort = Convert.ToUInt16(remotePortUpDown.Value);
            progSettings.communicationSpeed = hardwareSpeedCombo.SelectedIndex;

            if (gridCheckbox.Checked == true) progSettings.showListGrid = 1; else progSettings.showListGrid = 0;
            if (backupCheckbox.Checked == true) progSettings.backupMemcards = 1; else progSettings.backupMemcards = 0;
            if (backupWarningCheckBox.Checked == true) progSettings.warningMessage = 1; else progSettings.warningMessage = 0;
            if (restorePositionCheckbox.Checked == true) progSettings.restoreWindowPosition = 1; else progSettings.restoreWindowPosition = 0;
            if (fixCorruptedCardsCheckbox.Checked == true) progSettings.fixCorruptedCards = 1; else progSettings.fixCorruptedCards = 0;
            if (fontCombo.SelectedIndex != -1) progSettings.listFont = fontCombo.SelectedItem.ToString();

            hostWindow.applyProgramSettings(progSettings);
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

        private void applyButton_Click(object sender, EventArgs e)
        {
            applySettings();
        }

        private void dexDriveCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Save the COM port if the user selected a new one
            SavedComPort = dexDriveCombo.Text;
        }
    }
}
