using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MemcardRex
{
    public partial class compareWindow : Form
    {

        public compareWindow()
        {
            InitializeComponent();
        }

        //Show compare dialog
        public void initializeDialog(mainWindow hostWindow, string appName, byte[] save1Data, string save1Title, byte[] save2Data, string save2Title)
        {
            //Set window title
            this.Text = "Compare saves";

            compareListView.Columns[0].Width = (int)(compareListView.Columns[0].Width * hostWindow.xScale);
            compareListView.Columns[1].Width = (int)(compareListView.Columns[1].Width * hostWindow.xScale);
            compareListView.Columns[2].Width = (int)(compareListView.Columns[2].Width * hostWindow.xScale);

            //Set save titles
            save1Label.Text = "Save 1: " + save1Title;
            save2Label.Text = "Save 2: " + save2Title;
            
            //Compare saves
            for (int i = 0; i < save1Data.Length; i++)
            {
                //Check if the bytes are different
                if (save1Data[i] != save2Data[i])
                {
                    compareListView.Items.Add("0x" + i.ToString("X4") + " (" + i.ToString() + ")");
                    compareListView.Items[compareListView.Items.Count - 1].SubItems.Add("0x" + save1Data[i].ToString("X2") + " (" + save1Data[i].ToString() + ")");
                    compareListView.Items[compareListView.Items.Count - 1].SubItems.Add("0x" + save2Data[i].ToString("X2") + " (" + save2Data[i].ToString() + ")");
                }
            }

            //Check if the list contains any items
            if (compareListView.Items.Count < 1)
            {
                MessageBox.Show("Compared saves are identical.", appName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            this.ShowDialog(hostWindow);

        }

        private void compareWindow_Load(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
