/* Copyright (C) 2024 Rob Hall
 * SPDX-License-Identifier: GPL-3.0-or-later */

using Adw;
using GdkPixbuf;
using Gio;
using GLib;
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
    private ProgramSettings Settings;

    //Hardware interfaces
    private HardwareSetup hwSetup = new HardwareSetup();

    //Currently active interface
    public HardInterfaces activeInterface
    {
        get
        {
            return hwSetup.registeredInterfaces[Settings.ActiveInterface];
        }
    }

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

            //Set up hardware interfaces
            hwSetup.AttachInterface(new DexDrive());
            hwSetup.AttachInterface(new MemCARDuino());
            hwSetup.AttachInterface(new PS1CardLink());
            hwSetup.AttachInterface(new Unirom());
            hwSetup.AttachInterface(new PS3MemCardAdaptor());

            this.AddWindow(mainWindow);
            mainWindow.Show();
        };
        this.OnShutdown += (_, _) => {
            Settings.SaveSettings(ConfigDir(), "memcardrex for Linux", "2.0 (alpha)");
        };
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
        this.SetAccelsForAction("win.copy-save", ["<Control>c"]);
        this.SetAccelsForAction("win.paste-save", ["<Control>v"]);
    }

    //Notify all required parties of the settings change
    public void SettingsChanged(){
        mainWindow!.SettingsChanged();
    }

    private void QuitAction(Gio.SimpleAction sender, Gio.SimpleAction.ActivateSignalArgs args)
    {
        mainWindow?.Close();
    }

    private void PreferencesAction(Gio.SimpleAction sender, Gio.SimpleAction.ActivateSignalArgs args)
    {
        var dialog = new SettingsDialog(mainWindow!, ref Settings, hwSetup.GetAllInterfaceNames());
        dialog.Present();
    }

    private void ReadmeAction(Gio.SimpleAction sender, Gio.SimpleAction.ActivateSignalArgs args)
    {
        var readme = Gio.FileHelper.NewForPath("README.md");
        Gtk.Functions.ShowUri(mainWindow, readme.GetUri(), Gdk.Constants.CURRENT_TIME);
    }

    private void ShowAboutAction(Gio.SimpleAction sender, Gio.SimpleAction.ActivateSignalArgs args)
    {
        var aboutDialog = new AboutDialog();
        aboutDialog.SetTransientFor(mainWindow); 
        aboutDialog.SetModal(true);
        aboutDialog.Show();
    }

    static string EnvironmentVariables()
    {
        string env = "";
        foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
           env += "\n" + de.Key + "=" + de.Value;
        return env;
    }

    //Location of the config directory for the current user
    public static string ConfigDir()
    {
        var config_dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (config_dir == "" || config_dir == null)
            config_dir = ".config";
        return Path.Join(config_dir, "memcardrex");
    }

    public Application() : this(false, []) {}
}