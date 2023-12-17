using System;

using Foundation;
using AppKit;
using System.IO.Ports;


namespace MemcardRex
{
	public partial class PreferencesViewController : NSViewController
	{
        public static AppDelegate App
        {
            get { return (AppDelegate)NSApplication.SharedApplication.Delegate; }
        }

        public PreferencesViewController (IntPtr handle) : base (handle)
		{
		}

        public override void ViewWillAppear()
        {
            base.ViewWillAppear();

            this.View.Window.Title = "Preferences";

            //Disable resizing and closing of modal dialog
            this.View.Window.StyleMask &= ~NSWindowStyle.Resizable;
            this.View.Window.StyleMask &= ~NSWindowStyle.Closable;

            //Load all valid COM ports found on the system
            foreach (string port in SerialPort.GetPortNames())
            {
                if (port.Contains("/dev/tty.")) portPopup.AddItem(port);
            }

            //Read program settings
            iconPopup.SelectItem(iconPopup.ItemAtIndex(App.appSettings.IconBackgroundColor));
            formatPopup.SelectItem(formatPopup.ItemAtIndex(App.appSettings.FormatType));
            speedPopup.SelectItem(speedPopup.ItemAtIndex(App.appSettings.CommunicationSpeed));
            cardPopup.SelectItem(cardPopup.ItemAtIndex(App.appSettings.CardSlot));

            addressInput.StringValue = App.appSettings.RemoteCommAddress;
            portInput.IntValue = App.appSettings.RemoteCommPort;

            backupCheckbox.IntValue = App.appSettings.BackupMemcards;
            restoreCheckbox.IntValue = App.appSettings.RestoreWindowPosition;
            fixCheckbox.IntValue = App.appSettings.FixCorruptedCards;

            NSMenuItem comItem = portPopup.ItemWithTitle(App.appSettings.CommunicationPort);

            if(portPopup.ItemCount > 0)
            {
                if (comItem != null) portPopup.SelectItem(comItem);
            }
            else
            {
                portPopup.Enabled = false;
            }
        }

        [Export("confirmDialog:")]
        void ConfirmDialog(NSObject sender)
        {
            //Save program settings
            App.appSettings.IconBackgroundColor = (int) iconPopup.IndexOfSelectedItem;
            App.appSettings.FormatType = (int) formatPopup.IndexOfSelectedItem;
            App.appSettings.CommunicationSpeed = (int)speedPopup.IndexOfSelectedItem;
            App.appSettings.CardSlot = (int)cardPopup.IndexOfSelectedItem;

            App.appSettings.RemoteCommAddress = addressInput.StringValue;
            App.appSettings.RemoteCommPort = portInput.IntValue;

            App.appSettings.BackupMemcards = backupCheckbox.IntValue;
            App.appSettings.RestoreWindowPosition = restoreCheckbox.IntValue;
            App.appSettings.FixCorruptedCards = fixCheckbox.IntValue;

            //Save com port if there are any
            if (portPopup.ItemCount > 0) App.appSettings.CommunicationPort = portPopup.TitleOfSelectedItem;

            this.View.Window.Close();
        }

        [Export("cancelDialog:")]
        void CancelDialog(NSObject sender)
        {
            this.View.Window.Close();
        }
    }
}
