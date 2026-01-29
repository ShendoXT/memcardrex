using Gtk;
using System;
using System.Reflection;

namespace MemcardRex.Linux;

public class AboutDialog : Window
{
    private Button _okButton;

    private string GetBuildDate()
    {
        var attribute = typeof(Program).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == "BuildDate");

        return attribute?.Value ?? "Unknown";
    }

    private string GetGitHash()
    {
        var attribute = System.Reflection.Assembly.GetExecutingAssembly()
            .GetCustomAttributes<System.Reflection.AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == "GitHash");

        return attribute?.Value ?? "N/A";
    }

    public AboutDialog()
    {
        var builder = new Builder("MemcardRex.Linux.GUI.AboutDialog.ui");
        
        var content = builder.GetObject("root_box") as Box;
        if (content != null)
        {
            this.SetChild(content);
        }

        if (builder.GetObject("lblCompileDate") is Gtk.Label label)
            label.SetLabel($"Compile date: {GetBuildDate()}");

        if (builder.GetObject("lblAppVersion") is Gtk.Label verlabel)
            verlabel.SetLabel($"Version: 2.0a ({GetGitHash()})");

        this.SetTitle("About");
        this.SetResizable(false);
        this.SetDefaultSize(400, -1);

        _okButton = (Button)builder.GetObject("_okButton")!;
        _okButton!.OnClicked += (s, e) => this.Close();
    }

}