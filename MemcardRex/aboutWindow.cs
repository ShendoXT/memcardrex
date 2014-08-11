//Generic about window
//Shendo 2009-2010

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MemcardRex
{
    public partial class AboutWindow : Form
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the application about dialog
        /// </summary>
        /// <param name="owner">Handle of the window which hosts this dialog</param>
        /// <param name="applicationName">Name of the host application</param>
        /// <param name="applicationVersion">Version of the host application</param>
        /// <param name="compileDate">Date of the compilation</param>
        /// <param name="copyrightInfo">The copyright info shown in the bottom area of the dialog</param>
        /// <param name="additionalInfo">Used for additional information such as credits</param>
        public void initDialog(IWin32Window owner, string applicationName, string applicationVersion, string compileDate, string copyrightInfo, string additionalInfo)
        {
            //Set Window title
            this.Text = "About";

            //Display program name
            appNameLabel.Text = applicationName;

            //Display program version
            appVersionLabel.Text = "Version: " + applicationVersion;

            //Display program compile date
            compileDateLabel.Text = "Compile date: " + compileDate;

            //Display copyright information
            copyrightLabel.Text = copyrightInfo;

            //Display other info
            infoLabel.Text = additionalInfo;

            //Resize dialog according to the quantity of text
            this.Size = new Size(this.Size.Width, 132 + infoLabel.Height);

            //Display a dialog
            this.ShowDialog(owner);
        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            //Close the form
            this.Close();
        }

        private void AboutWindow_Paint(object sender, PaintEventArgs e)
        {
            //Draw gray rectangle
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(80,80,80)), 0, 0, this.Width, 52);
        }
    }
}
