using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gtk;

namespace MemcardRex.Linux
{
    public class PocketStationInfo : Gtk.Window
    {
        private Gtk.Box? _biosGroup;
        private Gtk.Entry? _entrySerial;
        private Gtk.Entry? _entryVersion;
        private Gtk.Label? _entryVersionLabel;
        private Gtk.Entry? _entryDate;
        private Gtk.Label? _entryDateLabel;
        private Gtk.Entry? _entryChecksum;
        private Gtk.Label? _entryChecksumLabel;
        private Gtk.Entry? _entryRemark;
        private Gtk.Label? _entryRemarkLabel;
        private Gtk.Button? _btnOk;
        private Gtk.Button? _btnSave;

        private byte[]? biosData;

        struct KnownReleases
        {
            public string _comment;
            public UInt32 _checksum;

            public KnownReleases(string comment, UInt32 checksum)
            {
                _comment = comment;
                _checksum = checksum;
            }
        }

        //Crete array of known releases
        static readonly IList<KnownReleases> releaseArray = new ReadOnlyCollection<KnownReleases>
            (new[] {
             new KnownReleases ("1st release", 0x27E94C07),
             new KnownReleases ("2nd release", 0xB16CE96C),
             new KnownReleases ("DTL-H4000", 0x1BABAF29)
            });

        public PocketStationInfo(Gtk.Window parent, bool showFullInfo)
        {
            this.Title = "PocketStation info";
            this.TransientFor = parent;
            this.Modal = true;
            this.Resizable = false;

            var builder = new Builder("MemcardRex.Linux.GUI.PocketStationInfo.ui");

            if (builder.GetObject("main_content") is Gtk.Box mainBox)
            {
                this.SetChild(mainBox);
                mainBox.Show();
            }
            else
            {
                return;
            }

            _biosGroup = (Gtk.Box)builder.GetObject("biosGroup")!;
            _entrySerial = (Gtk.Entry)builder.GetObject("entrySerial")!;
            _entryVersion = (Gtk.Entry)builder.GetObject("entryVersion")!;
            _entryVersionLabel = (Gtk.Label)builder.GetObject("entryVersionLabel")!;
            _entryDate = (Gtk.Entry)builder.GetObject("entryDate")!;
            _entryDateLabel = (Gtk.Label)builder.GetObject("entryDateLabel")!;
            _entryChecksum = (Gtk.Entry)builder.GetObject("entryChecksum")!;
            _entryChecksumLabel = (Gtk.Label)builder.GetObject("entryChecksumLabel")!;
            _entryRemark = (Gtk.Entry)builder.GetObject("entryRemark")!;
            _entryRemarkLabel = (Gtk.Label)builder.GetObject("entryRemarkLabel")!;
            _btnOk = (Gtk.Button)builder.GetObject("btnOk")!;
            _btnSave = (Gtk.Button)builder.GetObject("btnSave")!;

            if(!showFullInfo){
                _entryVersion.Hide();
                _entryVersionLabel.Hide();
                _entryDate.Hide();
                _entryDateLabel.Hide();
                _entryChecksum.Hide();
                _entryChecksumLabel.Hide();
                _entryRemark.Hide();
                _entryRemarkLabel.Hide();
                _btnSave.SetVisible(false);
            }

            this.QueueResize();

            _btnOk.OnClicked += (s, e) => this.Close();

            _btnSave.OnClicked += (s, e) => {
                var fileChooser = Gtk.FileChooserNative.New("Save PocketStation BIOS", parent, Gtk.FileChooserAction.Save, "Save", "Cancel");
                fileChooser.SetModal(true);
                fileChooser.SetCurrentName("BIOS.bin");
                var filter = Gtk.FileFilter.New();
                filter.Name = "Binary image (*.bin)";
                filter.AddPattern("*.bin");
                fileChooser.AddFilter(filter);

                fileChooser.Show();
                fileChooser.OnResponse += (sender, args) => {
                    if (args.ResponseId != (int)Gtk.ResponseType.Accept)
                    {
                        fileChooser.Destroy();
                        return;
                    }

                    try
                    {
                        var file = fileChooser.GetFile();
                        string path = file!.GetPath()!;
                        fileChooser.Destroy();

                        BinaryWriter binWriter = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None));

                        binWriter.Write(biosData!);
                        binWriter.Close();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                };
            };
        }

        public void ShowSerial(UInt32 serial)
        {
            //Get ascii character
            _entrySerial!.SetText(((char)(serial >> 24)).ToString() + ((serial & 0xFFFFFF)).ToString("D8"));
        }

        private UInt32 calcChecksum()
        {
            UInt32 checkum = 0;

            for(int i = 0; i < biosData!.Length; i+=4)
                checkum += (UInt32)(biosData![i] | biosData![i + 1] << 8 | biosData![i + 2] << 16 | biosData![i + 3] << 24);

            return checkum;
        }

        //Show everything about the dumped BIOS
        public void ShowBios(UInt32 serial, byte[] bData)
        {
            biosData = bData;
            UInt32 checksum;
            string versionText = "";

            //Display date
            _entryDate!.SetText(bData[0x17].ToString("X") + bData[0x16].ToString("X")
                + "/" + bData[0x15].ToString("X") + "/" + bData[0x14].ToString("X"));

            //Kernel version
            for(int i = 0; i < 4; i++)
                versionText += ((char)bData[0x1DFC + i]).ToString();

            versionText += ", ";

            //GUI version
            for (int i = 0; i < 4; i++)
                versionText += ((char)bData[0x3FFC + i]).ToString();

            _entryVersion!.SetText(versionText);

            checksum = calcChecksum();
            _entryChecksum!.SetText(checksum.ToString("X8"));

            displayRemark(checksum);

            ShowSerial(serial);
        }

        private void displayRemark(UInt32 checksum)
        {
            string remarkText = "";

            foreach(KnownReleases kRel in releaseArray)
            {
                if(checksum == kRel._checksum)
                {
                    remarkText = kRel._comment;
                }
            }

            if (remarkText == "") _entryRemark!.SetText("Unknown / bad dump");
            else _entryRemark!.SetText(remarkText);
        }
    }
}
