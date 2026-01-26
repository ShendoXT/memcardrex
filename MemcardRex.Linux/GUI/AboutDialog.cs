using Gtk;
using System;

namespace MemcardRex.Linux;

public class AboutDialog : Window
{
    private Button _okButton;

    public AboutDialog()
    {
        var builder = new Builder("MemcardRex.Linux.GUI.AboutDialog.ui");
        
        var content = builder.GetObject("root_box") as Box;
        if (content != null)
        {
            this.SetChild(content);
        }

        this.SetTitle("About");
        this.SetResizable(false);
        this.SetDefaultSize(400, -1);

        _okButton = (Button)builder.GetObject("_okButton")!;
        _okButton!.OnClicked += (s, e) => this.Close();
    }

}