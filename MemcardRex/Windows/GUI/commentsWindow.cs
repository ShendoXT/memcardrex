using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MemcardRex
{
    public partial class commentsWindow : Form
    {
        //If OK is pressed this will be true
        public bool okPressed = false;
        public string saveComment = null;

        public commentsWindow()
        {
            InitializeComponent();
        }

        private void commentsWindow_Load(object sender, EventArgs e)
        {

        }

        //Load initial values
        public void initializeDialog(string dialogTitle, string saveComment)
        {
            //Set window title to save name
            this.Text = dialogTitle;
            commentsTextBox.Text = saveComment;

            //A fix for selected all behaviour
            commentsTextBox.Select(commentsTextBox.Text.Length, 0);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            //Return value given by the dialog
            saveComment = commentsTextBox.Text;

            okPressed = true;
            this.Close();
        }
    }
}
