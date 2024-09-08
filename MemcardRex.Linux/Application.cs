/* Copyright (C) 2024 Rob Hall
 * SPDX-License-Identifier: GPL-3.0-or-later */

using Adw;
using GdkPixbuf;
using Gio;
using GObject;
using Gtk;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using static Gtk.Functions;
using MemcardRex.Core;

namespace MemcardRex.Linux;

public class Application : Adw.Application
{
    private MainWindow? mainWindow;
    public ProgramSettings Settings { get; }

    public static new Application New(string? id, Gio.ApplicationFlags flags)
    {
        return new Application();
    }

    public Application(bool owned, params ConstructArgument[] constructArguments) : base(owned, constructArguments)
    {
        this.AddActions();
        this.Settings = new ProgramSettings();
        Settings.LoadSettings(ConfigDir());
        this.OnActivate += (_, _) => {
            mainWindow = new MainWindow();
            this.AddWindow(mainWindow);
            mainWindow.Show();
        };
        this.OnShutdown += (_, _) => {
            Settings.SaveSettings(ConfigDir(), "memcardrex for Linux", "2.0 (alpha)");
        };

        var data = typeof(Utils).Assembly.ReadResourceAsByteArray("MemcardRex.Linux.Resources.memcardrex.gresource");
        var bytes = GLib.Bytes.NewTake(data);
        SetResourceBasePath("/io/gitlab/robxnano/memcardrex");
        Gio.Functions.ResourcesRegister(Gio.Resource.NewFromData(bytes));
    }

    private void AddActions()
    {
        SimpleAction? action = Gio.SimpleAction.New("about", null);
        action.OnActivate += ShowAboutAction;
        this.AddAction(action);
        action = Gio.SimpleAction.New("quit", null);
        action.OnActivate += QuitAction;
        this.AddAction(action);
        action = Gio.SimpleAction.New("readme", null);
        action.OnActivate += ReadmeAction;
        this.AddAction(action);
        action = Gio.SimpleAction.New("preferences", null);
        action.OnActivate += PreferencesAction;
        this.AddAction(action);

        // Accels
        this.SetAccelsForAction("app.quit", ["<control>q"]);
        this.SetAccelsForAction("win.new-card", ["<control>n"]);
        this.SetAccelsForAction("win.open", ["<control>o"]);
        this.SetAccelsForAction("win.save", ["<control>s"]);
        this.SetAccelsForAction("win.close-tab", ["<control>w"]);
        this.SetAccelsForAction("win.save-as", ["<control><shift>s"]);
        this.SetAccelsForAction("app.preferences", ["<control>comma"]);
        this.SetAccelsForAction("app.readme", ["F1"]);
    }

    private void QuitAction(Gio.SimpleAction sender, Gio.SimpleAction.ActivateSignalArgs args)
    {
        mainWindow?.Close();
    }

    private void PreferencesAction(Gio.SimpleAction sender, Gio.SimpleAction.ActivateSignalArgs args)
    {
        var win = new PreferencesWindow();
        win.SetTransientFor(mainWindow);
        win.SetModal(true);
        win.Show();
    }

    private void ReadmeAction(Gio.SimpleAction sender, Gio.SimpleAction.ActivateSignalArgs args)
    {
        var readme = Gio.FileHelper.NewForPath("README.md");
        Gtk.Functions.ShowUri(mainWindow, readme.GetUri(), Gdk.Constants.CURRENT_TIME);
    }

    private void ShowAboutAction(Gio.SimpleAction sender, Gio.SimpleAction.ActivateSignalArgs args)
    {
        var about = new Adw.AboutWindow
        {
            ApplicationName = "MemcardRex",
            Version = "2.0 (alpha)",
            DeveloperName = "Shendo and robxnano",
            Comments = "MemcardRex is a PS1 Memory Card editor and writer which supports a variety of different formats.",
            Copyright = "© 2009-2024 Shendo (Core and Windows version)\n© 2024 robxnano (Linux version)",
            Developers = [ "Alvaro Tanarro", "bitrot-alpha", "KuromeSan", "lmiori92", "Nico de Poel", "robxnano", "Shendo",
                          "\nBeta testers:", "Gamesoul Master", "Xtreme2damax", "Carmax91" ],
            ApplicationIcon = "io.gitlab.robxnano.memcardrex",
            Website = "https://github.com/ShendoXT/memcardrex/",
            LicenseType = Gtk.License.Gpl30,
            DebugInfo = "Runtime:\t" + RuntimeInformation.FrameworkDescription
                            + "\nPlatform:\t" + RuntimeInformation.RuntimeIdentifier
                            + "\nGTK version:\t" + String.Format("{0}.{1}.{2}", GetMajorVersion(),
                                                                 GetMinorVersion(), GetMicroVersion())
                            + "\n\nEnvironment variables:" + EnvironmentVariables()
        };
        about.SetTransientFor(mainWindow);
        about.SetModal(true);
        about.Present();
    }

    static string EnvironmentVariables()
    {
        string env = "";
        foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
           env += "\n" + de.Key + "=" + de.Value;
        return env;
    }

    public static string ConfigDir()
    {
        var config_dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (config_dir == "" || config_dir == null)
            config_dir = ".config";
        return Path.Join(config_dir, "memcardrex");
    }

    public Application() : this(false, []) {}
}