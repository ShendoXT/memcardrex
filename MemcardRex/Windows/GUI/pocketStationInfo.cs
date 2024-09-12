using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace MemcardRex.Windows.GUI
{
    [SupportedOSPlatform("windows")]
    public partial class pocketStationInfo : Form
    {
        private const string dialogName = "PocketStation info";
        private byte[] biosData;

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

        public pocketStationInfo()
        {
            InitializeComponent();
        }

        private UInt32 calcChecksum()
        {
            UInt32 checkum = 0;

            for(int i = 0; i < biosData.Length; i+=4)
                checkum += (UInt32)(biosData[i] | biosData[i + 1] << 8 | biosData[i + 2] << 16 | biosData[i + 3] << 24);

            return checkum;
        }

        private void displaySerial(UInt32 serial)
        {
            //Get ascii character
            serialTextbox.Text = ((char)(serial >> 24)).ToString();

            //Get rest of the serial in BCD form
            serialTextbox.Text += ((serial & 0xFFFFFF)).ToString("D8");
        }

        private void displayRemark(UInt32 checksum)
        {
            remarkTextbox.Text = "";

            foreach(KnownReleases kRel in releaseArray)
            {
                if(checksum == kRel._checksum)
                {
                    remarkTextbox.Text = kRel._comment;
                }
            }

            if (remarkTextbox.Text == "") remarkTextbox.Text = "Unknown / bad dump";
        }

        public void ShowSerial(UInt32 serial)
        {
            displaySerial(serial);

            this.Height = 104;
            this.Text = dialogName;
            this.ShowDialog();
        }

        //SHow everything about the dumped BIOS
        public void ShowBios(UInt32 serial, byte[] bData)
        {
            biosData = bData;
            UInt32 checksum;

            //Enable BIOS specific controls
            versionLabel.Visible = true;
            versionTextbox.Visible = true;

            dateLabel.Visible = true;
            dateTextbox.Visible = true;

            checksumLabel.Visible = true;
            checksumTextbox.Visible = true;

            remarkLabel.Visible = true;
            remarkTextbox.Visible = true;

            saveButton.Visible = true;

            //Display date
            dateTextbox.Text = bData[0x17].ToString("X") + bData[0x16].ToString("X")
                + "/" + bData[0x15].ToString("X") + "/" + bData[0x14].ToString("X");

            //Display version
            versionTextbox.Text = "";

            //Kernel version
            for(int i = 0; i < 4; i++)
                versionTextbox.Text += ((char)bData[0x1DFC + i]).ToString();

            versionTextbox.Text += ", ";

            //GUI version
            for (int i = 0; i < 4; i++)
                versionTextbox.Text += ((char)bData[0x3FFC + i]).ToString();

            //Display checksum
            checksum = calcChecksum();
            checksumTextbox.Text = checksum.ToString("X8");

            displayRemark(checksum);

            displaySerial(serial);
            this.Text = dialogName;
            this.ShowDialog();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Save BIOS to a file
        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDlg = new SaveFileDialog
            {
                Title = "Save PocketStation BIOS",
                Filter = "Binary image|bin.*",
                FileName = "BIOS.bin"
            };

            if(saveFileDlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                BinaryWriter binWriter = new BinaryWriter(File.Open(saveFileDlg.FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None));

                binWriter.Write(biosData);
                binWriter.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
