//Plugin information dialog
//Shendo 2010 - 2011

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MemcardRex
{
    public partial class pluginsWindow : Form
    {
        //Form that hosted this dialog
        mainWindow hostFrm = null;

        //Descriptive data for each loaded plugin
        List<pluginMetadata> loadedMetadata = new List<pluginMetadata>();

        //Currently selected plugin
        int pluginIndex = 0;

        public pluginsWindow()
        {
            InitializeComponent();
        }

        //Load default values
        public void initializeDialog(mainWindow hostForm, List<pluginMetadata> plgMetadata)
        {
            hostFrm = hostForm;
            loadedMetadata = plgMetadata;

            //Apply the native theme to the listview
            //glassSupport.SetWindowTheme(pluginListView.Handle, "Explorer", null);

            //Check if list contains any members
            if (loadedMetadata.Count > 0)
            {
                //Populate list with plugins
                for (int i = 0; i < loadedMetadata.Count; i++)
                {
                    pluginListView.Items.Add(loadedMetadata[i].pluginName);
                    pluginListView.Items[i].SubItems.Add(loadedMetadata[i].pluginAuthor);
                    pluginListView.Items[i].SubItems.Add(loadedMetadata[i].pluginSupportedGames);

                    //Select first item in the list
                    pluginListView.Items[0].Selected = true;
                }
            }
        }

        //Close this dialog
        private void OKbutton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Show about info
        private void aboutButton_Click(object sender, EventArgs e)
        {
            //Check if there are any loaded plugins
            if (loadedMetadata.Count > 0)
            {
                //Show plugin about dialog
                hostFrm.pluginSystem.showAboutDialog(pluginIndex);
            }
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            //Check if there are any loaded plugins
            if (loadedMetadata.Count > 0)
            {
                //Show plugin config dialog
                hostFrm.pluginSystem.showConfigDialog(pluginIndex);
            }
        }

        private void pluginListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Check if there are any items selected
            if (pluginListView.SelectedIndices.Count == 0) return;

            //Update currenty selected plugin
            pluginIndex = pluginListView.SelectedIndices[0];
        }
    }
}
