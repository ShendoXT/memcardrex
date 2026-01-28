using System;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using Gtk;
using MemcardRex.Core;

namespace MemcardRex.Linux
{
    public class SettingsDialog
    {
        private readonly Dialog _dialog;
        
        private readonly ComboBoxText drpComPort;
        private readonly ComboBoxText drpHardware;
        private readonly ComboBoxText drpIconBg;
        private readonly ComboBoxText drpFormat;
        private readonly ComboBoxText drpSlot;

        private readonly Entry txtAddress;
        private readonly SpinButton numPort;

        private readonly CheckButton chkBackup;
        private readonly CheckButton chkWarnings;
        private readonly CheckButton chkRestorePos;
        private readonly CheckButton chkFixCorrupted;

        private readonly Button btnOk;
        private readonly Button btnCancel;

        //Saved Hardware COM port (in case it's a removable USB adapter)
        string? SavedComPort = "";

        public SettingsDialog(Window parent, ref ProgramSettings Settings, string[] hwNames)
        {
            var builder = new Builder("MemcardRex.Linux.GUI.SettingsDialog.ui");

            _dialog = (Dialog)builder.GetObject("SettingsDialog")!;
            _dialog.SetTransientFor(parent);

            drpComPort = (ComboBoxText)builder.GetObject("drpComPort")!;
            drpHardware = (ComboBoxText)builder.GetObject("drpHardware")!;
            drpIconBg = (ComboBoxText)builder.GetObject("drpIconBg")!;
            drpFormat = (ComboBoxText)builder.GetObject("drpFormat")!;
            drpSlot = (ComboBoxText)builder.GetObject("drpSlot")!;

            txtAddress = (Entry)builder.GetObject("txtAddress")!;
            numPort = (SpinButton)builder.GetObject("numPort")!;

            chkBackup = (CheckButton)builder.GetObject("chkBackup")!;
            chkWarnings = (CheckButton)builder.GetObject("chkWarnings")!;
            chkRestorePos = (CheckButton)builder.GetObject("chkRestorePos")!;
            chkFixCorrupted = (CheckButton)builder.GetObject("chkFixCorrupted")!;

            btnOk = (Button)builder.GetObject("btnOk")!;
            btnCancel = (Button)builder.GetObject("btnCancel")!;

            SavedComPort = Settings.CommunicationPort;

            //Icon Background
            drpIconBg.AppendText("Transparent");
            drpIconBg.AppendText("Black (Slim PS1 models)");
            drpIconBg.AppendText("Gray (Older european PS1 models)");
            drpIconBg.AppendText("Blue (Standard BIOS color)");
            drpIconBg.SetActive(Settings.IconBackgroundColor);

            //Format Type
            drpFormat.AppendText("Quick format");
            drpFormat.AppendText("Full format");
            drpFormat.SetActive(Settings.FormatType);

            //Hardware Slot
            drpSlot.AppendText("Slot 1");
            drpSlot.AppendText("Slot 2");
            drpSlot.SetActive(Settings.CardSlot);

            //Remote port and address
            txtAddress.SetText(Settings.RemoteCommAddress);
            numPort.SetValue(Settings.RemoteCommPort);

            //Checkboxes
            chkBackup.Active = (Settings.BackupMemcards == 1);
            chkWarnings.Active = (Settings.WarningMessages == 1);
            chkRestorePos.Active = (Settings.RestoreWindowPosition == 1);
            chkFixCorrupted.Active = (Settings.FixCorruptedCards == 1);

            //Load all COM ports found on the system
            foreach (string port in SerialPort.GetPortNames())
                drpComPort.Append(port, port);

            //Select port from the list
            drpComPort.ActiveId = Settings.CommunicationPort;

            drpComPort.OnChanged += (sender, e) => 
            {
               SavedComPort = drpComPort.GetActiveText();
            };

            //Load all hardware interfaces available
            foreach (string name in hwNames)
                drpHardware.AppendText(name);

            drpHardware.SetActive(Settings.ActiveInterface);

            var localSettings = Settings;

            _dialog.OnResponse += (sender, args) =>
            {
                //Save settings on OK
                if (args.ResponseId == (int)ResponseType.Ok)
                {
                    localSettings.IconBackgroundColor = drpIconBg.GetActive();
                    localSettings.FormatType = drpFormat.GetActive();
                    localSettings.CardSlot = drpSlot.GetActive();
                    localSettings.RemoteCommAddress = txtAddress.GetText();
                    localSettings.RemoteCommPort = (int) numPort.GetValue();

                    localSettings.BackupMemcards = chkBackup.Active ? 1 : 0;
                    localSettings.WarningMessages = chkWarnings.Active ? 1 : 0;
                    localSettings.RestoreWindowPosition = chkRestorePos.Active ? 1 : 0;
                    localSettings.FixCorruptedCards = chkFixCorrupted.Active ? 1 : 0;

                    localSettings.CommunicationPort = SavedComPort;

                    localSettings.ActiveInterface = drpHardware.GetActive();

                    //Notify application of the settings change
                    var app = (Application)Gtk.Application.GetDefault()!;
                    app.SettingsChanged();

                }
                _dialog.Destroy();
            };

        }

        public void Present() => _dialog.Present();
    }
}