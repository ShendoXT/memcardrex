using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MemcardRex
{
    public partial class headerWindow : Form
    {
        //If OK is pressed this value will be true
        public bool okPressed = false;

        //Name of the host application
        string appName = null;

        //Save header data
        public string prodCode = null;
        public string saveIdentifier = null;
        public ushort saveRegion = 0;

        //Custom save region (If the save uses nonstandard region)
        private ushort customSaveRegion = 0;

        public headerWindow()
        {
            InitializeComponent();
        }

        private void headerWindow_Load(object sender, EventArgs e)
        {

        }

        //Initialize dialog by loading provided values
        public void initializeDialog(string applicationName, string dialogTitle, string prodCode, string identifier, ushort region)
        {
            appName = applicationName;
            this.Text = dialogTitle;
            prodCodeTextbox.Text = prodCode;
            identifierTextbox.Text = identifier;

            //Check what region is selected
            switch (region)
            {
                default:        //Region custom, show hex
                    customSaveRegion = region;
                    regionCombobox.Items.Add("0x" + region.ToString("X4"));
                    regionCombobox.SelectedIndex = 3;
                    break;

                case 0x4142:    //America
                    regionCombobox.SelectedIndex = 0;
                    break;

                case 0x4542:    //Europe
                    regionCombobox.SelectedIndex = 1;
                    break;

                case 0x4942:    //Japan
                    regionCombobox.SelectedIndex = 2;
                    break;
            }

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
                new messageWindow().ShowMessage(this, appName, "Product code must be exactly 10 characters long.", "OK", null, true);
            }
            else
            {
                //String is valid
                prodCode = prodCodeTextbox.Text;
                saveIdentifier = identifierTextbox.Text;

                //Set the save region
                switch (regionCombobox.SelectedIndex)
                {
                    default:        //Custom region
                        saveRegion = customSaveRegion;
                        break;

                    case 0:    //America
                        saveRegion = 0x4142;
                        break;

                    case 1:    //Europe
                        saveRegion = 0x4542;
                        break;

                    case 2:    //Japan
                        saveRegion = 0x4942;
                        break;
                }

                //OK is pressed
                okPressed = true;
                this.Close();
            }
        }
    }
}
