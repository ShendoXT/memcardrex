using System;
using System.Collections.Generic;
using System.Text;
using Adw;
using Gtk;

namespace rexPluginSystem
{
    public interface rexPluginInterfaceV2
    {
        string getPluginName();
        string getPluginAuthor();
        string getPluginSupportedGames();
        string[] getSupportedProductCodes();
        byte[] editSaveData(byte[] gameSaveData, string saveProductCode);
        void showAboutDialog();
        void showConfigDialog();
        void setWindowParent(IntPtr parentHandle);
    }

    public class rexPlugin : rexPluginInterfaceV2
    {
        private const string pluginName = "CrashReSign";
        private const string pluginVersion = "0.3";
        private const string pluginAuthor = "Shendo";
        private const string pluginSupportedGames = "Crash Anywhere (PocketStation)";

        //Exit status
        bool exitResult = false;
        bool isDone = false;

        //Window that's hosting this plugin
        private static Gtk.Window? parentWindow;

        private byte[]? saveData;

        //Return Plugin's name (name + plugin version is recommended)
        public string getPluginName()
        {
            return pluginName + " " + pluginVersion;
        }

        //Return Author's name.
        public string getPluginAuthor()
        {
            return pluginAuthor;
        }

        //Return a list of games supported by the plugin
        public string getPluginSupportedGames()
        {
            return pluginSupportedGames;
        }

        //Return a string array of product codes compatible with this plugin.
        //In order to make a product-code-independent-plugin one member should be "*.*".
        public string[] getSupportedProductCodes()
        {
            return new string[] { "SCPSP10073" };
        }

        //Set hosting window by the handle reference
        public void setWindowParent(IntPtr parentHandle)
        {
            if (parentHandle == IntPtr.Zero) return;

            //In order to keep plugin API cross platform
            //we need to get window object back from handle
            //and it's a bit of an involved process...
            try
            {
                var gobjectAssembly = typeof(GObject.Object).Assembly;
                var wrapperType = gobjectAssembly.GetType("GObject.Internal.ObjectWrapper");
                
                var wrapMethod = wrapperType?.GetMethod("WrapHandle", 
                    System.Reflection.BindingFlags.Static | 
                    System.Reflection.BindingFlags.Public | 
                    System.Reflection.BindingFlags.NonPublic);

                if (wrapMethod != null)
                {
                    var genericMethod = wrapMethod.MakeGenericMethod(typeof(Gtk.Window));
                    parentWindow = (Gtk.Window)genericMethod.Invoke(null, new object[] { parentHandle, false })!;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Reflection error: {ex.Message}");
            }
        }

        //A data to process. Edited save data should be returned.
        //Array size depends on the number of slots that specific save takes (Save header (128 bytes) + Number of slots * 8192 bytes).
        public byte[] editSaveData(byte[] gameSaveData, string saveProductCode)
        {
            saveData = gameSaveData;
            PocketStationInfoWindow();

            //Blocking untill dialog is closed
            while (!isDone)
            {
                GLib.Functions.MainContextDefault().Iteration(true);
            }

            //Return data based on user selection
            if(exitResult) return saveData!;
            else return null!;
        }

        //Custom about dialog
        public void showAboutDialog()
        {
            Gtk.Module.Initialize();

            var window = Gtk.Window.New();
            window.SetTitle("About");
            window.SetResizable(false);
            window.SetModal(true);
            window.SetSizeRequest(360, -1);

            if (parentWindow != null)
            {
                window.SetTransientFor(parentWindow);
                window.SetDestroyWithParent(true);
            }

            var mainVbox = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
            window.SetChild(mainVbox);

            var headerBox = Gtk.Box.New(Gtk.Orientation.Vertical, 2);
            headerBox.SetMarginStart(10);
            headerBox.SetMarginEnd(10);
            headerBox.SetMarginTop(2);
            headerBox.SetMarginBottom(10);

            var titleLabel = Gtk.Label.New(null);
            titleLabel.SetHalign(Gtk.Align.Start);
            titleLabel.SetMarkup("<span size='14000' weight='bold'>CrashReSign</span>");

            var infoLine = Gtk.Box.New(Gtk.Orientation.Horizontal, 0);
            var versionLabel = Gtk.Label.New("Version: 0.3");
            var subTitleLabel = Gtk.Label.New("Serial ID signer for Crash Anywhere");
            subTitleLabel.SetHexpand(true);
            subTitleLabel.SetHalign(Gtk.Align.End);

            infoLine.Append(versionLabel);
            infoLine.Append(subTitleLabel);

            headerBox.Append(titleLabel);
            headerBox.Append(infoLine);
            mainVbox.Append(headerBox);

            mainVbox.Append(Gtk.Separator.New(Gtk.Orientation.Horizontal));

            var bodyBox = Gtk.Box.New(Gtk.Orientation.Vertical, 10);
            bodyBox.SetMarginStart(10);
            bodyBox.SetMarginEnd(10);
            bodyBox.SetMarginTop(10);
            bodyBox.SetVexpand(true);

            var contentLabel = Gtk.Label.New(
                "Supported product code:\n" +
                "SCPSP10073 - NTSC J.\n\n" +
                "Use this plugin to sign your Crash Anywhere\n" +
                "file with your PocketStation serial number.\n\n" +
                "Running the same Crash Anywhere app on\n" +
                "multiple devices does not work properly\n" +
                "without matching serial number."
            );
            contentLabel.SetHalign(Gtk.Align.Start);
            contentLabel.SetJustify(Gtk.Justification.Left);
            bodyBox.Append(contentLabel);

            var thanksLabel = Gtk.Label.New("Thanks to:\nNK055");
            thanksLabel.SetHalign(Gtk.Align.Start);
            bodyBox.Append(thanksLabel);

            mainVbox.Append(bodyBox);

            var footerRow = Gtk.Box.New(Gtk.Orientation.Horizontal, 0);
            footerRow.SetMarginStart(10);
            footerRow.SetMarginEnd(10);
            footerRow.SetMarginBottom(10);
            footerRow.SetMarginTop(0);

            var authorLabel = Gtk.Label.New(null);
            authorLabel.SetHalign(Gtk.Align.Start);
            authorLabel.SetValign(Gtk.Align.End);
            authorLabel.SetMarkup("Author: Shendo");
            authorLabel.SetHexpand(true);

            var okButton = Gtk.Button.NewWithLabel("Close");
            okButton.SetSizeRequest(80, -1);
            okButton.AddCssClass("suggested-action");
            okButton.OnClicked += (s, e) => window.Destroy();

            footerRow.Append(authorLabel);
            footerRow.Append(okButton);

            mainVbox.Append(footerRow);

            window.Present();
        }

        public void PocketStationInfoWindow()
        {
            var window = Gtk.Window.New();
            window.SetTitle(getPluginName());
            window.SetResizable(false);
            window.SetModal(true);

            if (parentWindow != null)
                window.SetTransientFor(parentWindow);

            var mainBox = new Gtk.Box();
            mainBox.SetOrientation(Gtk.Orientation.Vertical);
            mainBox.SetSpacing(6);
            mainBox.SetMarginTop(10);
            mainBox.SetMarginBottom(10);
            mainBox.SetMarginStart(10);
            mainBox.SetMarginEnd(10);

            var rowBox = new Gtk.Box();
            rowBox.SetOrientation(Gtk.Orientation.Horizontal);
            rowBox.SetSpacing(12);

            var label = Gtk.Label.New("PocketStation serial:");
            label.SetHalign(Gtk.Align.Start);
            
            var entry = new Gtk.Entry();
            entry.MaxLength = 9;

            //Store first letter 
            string pocketSerial = (Convert.ToChar(saveData![0xA03])).ToString();
            int serialCode = (saveData[0xA02] << 16 | saveData[0xA01] << 8 | saveData[0xA00]);
            pocketSerial += serialCode.ToString("D8");

            entry.SetText(pocketSerial);
            entry.SetHexpand(true);

            rowBox.Append(label);
            rowBox.Append(entry);

            var buttonBox = new Gtk.Box();
            buttonBox.SetOrientation(Gtk.Orientation.Horizontal);
            buttonBox.SetSpacing(10);
            buttonBox.SetHalign(Gtk.Align.End);
            buttonBox.SetMarginTop(10);

            var okButton = Gtk.Button.NewWithLabel("OK");
            okButton.AddCssClass("suggested-action");
            
            var closeButton = Gtk.Button.NewWithLabel("Close");

            okButton.OnClicked += (s, e) => {

                if(entry.GetText().Length < 9) return;

                saveData[0xA03] = Convert.ToByte(entry.GetText()[0]);
                UInt32 outputValue = 0;

                try{
                    outputValue = uint.Parse(entry.GetText().Substring(1));
                } catch {
                    return;
                }

                saveData[0xA02] = (byte) (outputValue >> 16);
                saveData[0xA01] = (byte) (outputValue >> 8);
                saveData[0xA00] = (byte) (outputValue);

                exitResult = true;
                window.Destroy();
            };

            closeButton.OnClicked += (s, e) => {
                window.Destroy();
            };

            window.OnUnrealize += (s, e) => {
                isDone = true;
            };

            okButton.SetSizeRequest(80, -1);
            closeButton.SetSizeRequest(80, -1);

            buttonBox.Append(closeButton);
            buttonBox.Append(okButton);

            mainBox.Append(rowBox);
            mainBox.Append(buttonBox);

            window.SetChild(mainBox);
            window.Present();
        }

        //No configurable options available
        public void showConfigDialog()
        {
            var dialog = new Adw.MessageDialog
            {
                Modal = true,
                Heading = getPluginName(),
                Body = "This plugin has no options you can configure.",
                TransientFor = parentWindow
            };
            dialog.AddResponse("close", "Close");
            dialog.Show();
            dialog.OnResponse += (_, _) => {
                dialog.Destroy();
            };
        }
    }
}