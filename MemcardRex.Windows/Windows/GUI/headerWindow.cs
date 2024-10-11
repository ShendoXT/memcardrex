using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace MemcardRex
{
    [SupportedOSPlatform("windows")]
    public partial class headerWindow : Form
    {
        //If OK is pressed this value will be true
        public bool okPressed = false;

        //Name of the host application
        string appName = null;

        //Save header data
        public string prodCode = null;
        public string saveIdentifier = null;
        public string saveRegion = null;

        public headerWindow()
        {
            InitializeComponent();
        }

        private void headerWindow_Load(object sender, EventArgs e)
        {

        }

        //Initialize dialog by loading provided values
        public void initializeDialog(string applicationName, string dialogTitle, string prodCode, string identifier, string region)
        {
            appName = applicationName;
            this.Text = dialogTitle;
            prodCodeTextbox.Text = prodCode;
            identifierTextbox.Text = identifier;
            regionCombobox.Text = region;

            //A fix for selected all behaviour
            prodCodeTextbox.Select(prodCodeTextbox.Text.Length, 0);
            identifierTextbox.Select(identifierTextbox.Text.Length, 0);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            //Check if values are valid to be submitted
            if (prodCodeTextbox.Text.Length < 10 && identifierTextbox.Text.Length != 0)
            {
                //String is not valid
                MessageBox.Show("Product code must be exactly 10 characters long.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                //String is valid
                prodCode = prodCodeTextbox.Text;
                saveIdentifier = identifierTextbox.Text;
                saveRegion = regionCombobox.Text;

                //OK is pressed
                okPressed = true;
                this.Close();
            }
        }
    }
}
