/* Copyright (C) 2024 Rob Hall
 * SPDX-License-Identifier: GPL-3.0-or-later */

﻿using System;
using System.Reflection;
using GdkPixbuf;
using Gio;
using GObject;
using Gtk;
using Adw;
using MemcardRex.Core;
using static MemcardRex.Core.ps1card;

namespace MemcardRex.Linux;

public class MainWindow : Gtk.ApplicationWindow
{
    [Connect] private readonly Adw.TabView tabView;
    [Connect] private readonly Gtk.Stack stack;
    [Connect] private readonly Adw.WindowTitle windowTitle;

    private readonly Gio.SimpleAction actionNew;
    private readonly Gio.SimpleAction actionOpen;
    private readonly Gio.SimpleAction actionSave;
    private readonly Gio.SimpleAction actionSaveAs;
    private readonly Gio.SimpleAction actionCloseTab;
    private readonly Gio.SimpleAction actionCloseAll;

    private MainWindow(Gtk.Builder builder, string name) : base(builder.GetPointer(name), false)
    {
        builder.Connect(this);
        tabView ??= new();
        stack ??= new();
        windowTitle ??= new();

        var style_manager = Adw.StyleManager.GetDefault();
        style_manager.SetColorScheme(ColorScheme.PreferDark);

        tabView.OnClosePage += OnTabCloseRequest;
        tabView.OnNotify += (_, args) => {
            if (args.Pspec.GetName() == "selected-page") {
                var page = tabView.GetSelectedPage();
                if (page != null) {
                    var child = (PS1CardTab?)page.GetChild();
                    if (child != null && child.Title != null)
                        windowTitle.SetSubtitle(child.Title);
                }
            }
            else if (args.Pspec.GetName() == "n-pages") {
                if (tabView.NPages >= 1) {
                    var page = tabView.GetSelectedPage();
                    if (page != null) {
                        var child = (PS1CardTab?)page.GetChild();
                        if (child != null && child.Title != null)
                            windowTitle.SetSubtitle(child.Title);
                    }
                    stack.SetVisibleChildName("tabs");
                    SetCardActionsEnabled(true);
                }
                else {
                    windowTitle.SetSubtitle("");
                    stack.SetVisibleChildName("welcome");
                    SetCardActionsEnabled(false);
                }
            }
        };

        this.OnCloseRequest += OnWindowCloseRequest;

        actionNew = Gio.SimpleAction.New("new-card", null);
        actionNew.OnActivate += NewCardAction;
        this.AddAction(actionNew);
        actionOpen = Gio.SimpleAction.New("open", null);
        actionOpen.OnActivate += OpenCardAction;
        this.AddAction(actionOpen);
        actionSave = Gio.SimpleAction.New("save", null);
        actionSave.OnActivate += SaveAction;
        this.AddAction(actionSave);
        actionSaveAs = Gio.SimpleAction.New("save-as", null);
        actionSaveAs.OnActivate += SaveAsAction;
        this.AddAction(actionSaveAs);
        actionCloseTab = Gio.SimpleAction.New("close-tab", null);
        actionCloseTab.OnActivate += CloseTabAction;
        this.AddAction(actionCloseTab);
        actionCloseAll = Gio.SimpleAction.New("close-all", null);
        actionCloseAll.OnActivate += CloseAllAction;
        this.AddAction(actionCloseAll);

        SetCardActionsEnabled(false);
    }

    private void SetCardActionsEnabled(bool enabled)
    {
        actionSave.SetEnabled(enabled);
        actionSaveAs.SetEnabled(enabled);
        actionCloseTab.SetEnabled(enabled);
        actionCloseAll.SetEnabled(enabled);
    }

    private void NewCardAction(Gio.SimpleAction sender, Gio.SimpleAction.ActivateSignalArgs args)
    {
        var card = new ps1card();
        string? result = card.OpenMemoryCard(null, false);
        if (result != null) {
            Console.WriteLine("Failed to create new memory card: {0}", result);
            return;
        }
        var tab = new PS1CardTab(card);
        var child = tabView.Append(tab);
        tab.SetPage(child);
        SetCardActionsEnabled(true);
    }

    internal static Gtk.FileFilter AllFilesFilter()
    {
        var filter = FileFilter.New();
        filter.Name = "All Files";
        filter.AddPattern("*");
        return filter;
    }

    internal static Gtk.FileFilter FormatFilter(string name, string[] patterns)
    {
        var filter = Gtk.FileFilter.New();
        filter.Name = name;
        foreach (string pattern in patterns)
        {
            filter.AddPattern(pattern);
        }
        return filter;
    }

    internal static Gtk.FileFilter SingleSavesFilter()
    {
        var filter = Gtk.FileFilter.New();
        string[] patterns = ["*.mcs", "*.ps1", "*.PSV", "*.mcb", "*.mcx", "*.pda", "*.psx", "B???????????*"];
        filter.Name = "All Supported Files";
        foreach (string pattern in patterns)
        {
            filter.AddPattern(pattern);
        }
        return filter;
    }

    internal static Gtk.FileFilter FilterForType(CardTypes type)
    {
        return type switch
        {
            CardTypes.gme => FormatFilter("DexDrive Memory Card", ["*.gme"]),
            CardTypes.vgs => FormatFilter("VGS Memory Card", ["*.mem", "*.vgs"]),
            CardTypes.vmp => FormatFilter("PSP/Vita Memory Card", ["*.VMP"]),
            CardTypes.mcx => FormatFilter("PS Vita 'MCX' PocketStation Memory Card", ["*.BIN"]),
            _ => FormatFilter("Standard Memory Card", ["*.mcr", "*.bin", "*.ddf", "*.mc", "*.mcd", "*.mci", "*.ps", "*.psm", "*.srm", "*.vm1", "*.vmc"]),
        };
    }

    internal static Gtk.FileFilter MemoryCardsFilter()
    {
        var filter = Gtk.FileFilter.New();
        string[] patterns = [".BIN", "*.mcr", "*.VMP", "*.ddf", "*.mc", "*.mcd", "*.mci", "*.ps", "*.psm", "*.srm", "*.vm1", "*.vmc"];
        filter.Name = "All Supported Files";
        foreach (string pattern in patterns)
        {
            filter.AddPattern(pattern);
        }
        return filter;
    }

    internal static ps1card.CardTypes TypeForName(string name)
    {
        return ps1card.CardTypes.raw;
    }

    private void OpenCardAction(Gio.SimpleAction sender, Gio.SimpleAction.ActivateSignalArgs args)
    {
        var fileChooser = Gtk.FileChooserNative.New("Open Memory Card", this, Gtk.FileChooserAction.Open, "Open", "Cancel");
        fileChooser.SetModal(true);
        fileChooser.AddFilter(AllFilesFilter());
        fileChooser.AddFilter(MemoryCardsFilter());
        fileChooser.Show();
        fileChooser.OnResponse += (sender, args) => {
            var file = fileChooser.GetFile();
            fileChooser.Destroy();
            if (args.ResponseId != (int) Gtk.ResponseType.Accept)
                return;
            try {
                var card = new ps1card();
                string? result = card.OpenMemoryCard(file!.GetPath()!, false);
                if (result != null) {
                    Console.WriteLine("Failed to open memory card: {0}", result);
                    return;
                }
                var tab = new PS1CardTab(card);
                var child = tabView.Append(tab);
                tab.SetPage(child);
                SetCardActionsEnabled(true);
            }
            catch { return; }
        };
    }

    private void SaveAction(Gio.SimpleAction sender, Gio.SimpleAction.ActivateSignalArgs args)
    {
        var page = tabView.GetSelectedPage();
        var tab = (PS1CardTab?) page?.GetChild();
        tab?.Save(this);
    }

    private void SaveAsAction(Gio.SimpleAction sender, Gio.SimpleAction.ActivateSignalArgs args)
    {
        var page = tabView.GetSelectedPage();
        var tab = (PS1CardTab?) page?.GetChild();
        tab?.SaveAs(this);
    }

    private void CloseTabAction(Gio.SimpleAction sender, Gio.SimpleAction.ActivateSignalArgs args)
    {
        var page = tabView.GetSelectedPage();
        if (page != null)
            tabView.ClosePage(page);
    }

    private void CloseAllAction(Gio.SimpleAction sender, Gio.SimpleAction.ActivateSignalArgs args)
    {
    }

    public bool HasUnsavedChanges()
    {
        bool unsaved = false;
        for (int i = 0; i < tabView.NPages; i++)
        {
            var page = (PS1CardTab) tabView.GetNthPage(i).GetChild();
            if (page.HasUnsavedChanges == true)
            {
                unsaved = true;
            }
        }
        return unsaved;
    }

    private bool OnTabCloseRequest (Adw.TabView sender, Adw.TabView.ClosePageSignalArgs args)
    {
        var page = args.Page;
        var child = (PS1CardTab) page.GetChild();
        if (child.HasUnsavedChanges) {
            var dialog = new Adw.MessageDialog
            {
                Modal = true,
                Heading = "Save Changes?",
                Body = string.Format("“{0}” has been modified. Unsaved data will be permanently lost.", child.Title),
                TransientFor = this
            };
            dialog.AddResponse("cancel", "Cancel");
            dialog.AddResponse("discard", "Discard");
            dialog.AddResponse("save", "Save");
            dialog.SetResponseAppearance("discard", ResponseAppearance.Destructive);
            dialog.SetResponseAppearance("save", ResponseAppearance.Suggested);
            dialog.Show();
            dialog.OnResponse += (_, args2) => {
                dialog.Destroy();
                if (args2.Response == "save") {
                    child.Save(this);
                }
                sender.ClosePageFinish(page, args2.Response != "cancel");
            };
        }
        else {
            sender.ClosePageFinish(page, true);
        }
        return true;
    }

    private bool OnWindowCloseRequest (Gtk.Window sender, EventArgs args)
    {
        if (HasUnsavedChanges()) {
            var dialog = new Adw.MessageDialog
            {
                Modal = true,
                Heading = "Discard Changes and Quit?",
                Body = "If you quit now, unsaved data will be lost.",
                TransientFor = this
            };
            dialog.AddResponse("cancel", "Cancel");
            dialog.AddResponse("quit", "Quit");
            dialog.SetResponseAppearance("quit", Adw.ResponseAppearance.Destructive);
            dialog.Show();
            dialog.OnResponse += (_, dialogArgs) => {
                dialog.Destroy();
                if (dialogArgs.Response == "quit")
                    this.Destroy();
            };
            return true;
        }
        return false;
    }

    public MainWindow() : this(new Gtk.Builder("MemcardRex.Linux.GUI.MainWindow.ui"), "main_window") {}
}