using Gtk;
using System;

namespace MemcardRex.Linux;

public class HeaderDialog
{
    private readonly Dialog dialog;
    private readonly Label titleLabel;
    private readonly ComboBox regionCombo;
    private readonly Entry productCodeEntry;
    private readonly Entry identifierEntry;
    private readonly Button okButton;
    private readonly Button cancelButton;
    
    private bool okClicked = false;

    public HeaderDialog(Window parent)
    {
        var builder = new Builder("MemcardRex.Linux.GUI.HeaderDialog.ui");
        
        dialog = (Dialog)builder.GetObject("dialog")!;
        titleLabel = (Label)builder.GetObject("titleLabel")!;
        regionCombo = (ComboBox)builder.GetObject("regionCombo")!;
        productCodeEntry = (Entry)builder.GetObject("productCodeEntry")!;
        identifierEntry = (Entry)builder.GetObject("identifierEntry")!;
        okButton = (Button)builder.GetObject("okButton")!;
        cancelButton = (Button)builder.GetObject("cancelButton")!;
        
        dialog.SetTransientFor(parent);

        okButton.OnClicked += (sender, args) => 
        {
            //Check if values are valid to be submitted
            /*if ((productCodeEntry?.Buffer?.Text?.Length ?? 0) < 10 && (identifierEntry?.Buffer?.Text?.Length ?? 0) != 0)
            {
                //String is not valid
                //MessageBox.Show("Product code must be exactly 10 characters long.", appName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }*/

            okClicked = true;
            dialog.Close();
        };
        
        cancelButton.OnClicked += (sender, args) => 
        {
            okClicked = false;
            dialog.Close();
        };
    }

    public void SetSaveInfo(string title, string region, string productCode, string identifier){
        titleLabel.SetLabel(title);
        productCodeEntry.SetText(productCode);
        identifierEntry.SetText(identifier);

        var entry = regionCombo.Child as Gtk.Entry;
        if (entry != null)
            entry.SetText(region);
    }

    public string GetRegion()
    {
        if (regionCombo.Child is Gtk.Editable entry)
        {
            return entry.GetText();
        }else{
            return "";
        }
    }

    public string GetProductCode()
    {
        return productCodeEntry.GetText();
    }

    public string GetIdentifier()
    {
        return identifierEntry.GetText();
    }

    public bool Run()
    {
        okClicked = false;
        dialog.Present();
        
        // Wait for dialog to close
        var loop = GLib.MainLoop.New(null, false);
        dialog.OnCloseRequest += (sender, args) =>
        {
            loop.Quit();
            return false;
        };
        loop.Run();
        
        return okClicked;
    }

    public void Present()
    {
        dialog.Present();
    }
}