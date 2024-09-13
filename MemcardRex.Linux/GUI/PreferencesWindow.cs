/* Copyright (C) 2024 Rob Hall
 * SPDX-License-Identifier: GPL-3.0-or-later */

using System;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using GdkPixbuf;
using Gio;
using GObject;
using Gtk;
using Adw;
using MemcardRex.Core;

namespace MemcardRex.Linux;

public class PreferencesWindow : Adw.Window
{
    [Connect] private readonly Gtk.DropDown serialPortCombo;

    private readonly StringList serialPortList;

    private readonly Gio.SimpleAction actionOk;
    private readonly Gio.SimpleAction actionCancel;

    private PreferencesWindow(Gtk.Builder builder, string name) : base(builder.GetPointer(name), false)
    {
        builder.Connect(this);
        serialPortCombo ??= new();
        serialPortList = Gtk.StringList.New(null);
        foreach (string port in SerialPort.GetPortNames())
        {
            serialPortList.Append(port);
        }
        serialPortCombo.SetModel(serialPortList);
        var app = (Application) Gio.Application.GetDefault()!;
        Load(app.Settings);

        var actions = Gio.SimpleActionGroup.New();

        actionOk = Gio.SimpleAction.New("ok", null);
        actionOk.OnActivate += OkAction;
        actions.AddAction(actionOk);
        actionCancel = Gio.SimpleAction.New("cancel", null);
        actionCancel.OnActivate += (_, _) => Close();
        actions.AddAction(actionCancel);

        this.InsertActionGroup("prefs-win", actions);
    }

    private void OkAction(Gio.SimpleAction sender, Gio.SimpleAction.ActivateSignalArgs args)
    {
        Save();
        Close();
    }

    private void Load(ProgramSettings settings)
    {
    }

    private void Save()
    {
    }

    public PreferencesWindow() : this(new Gtk.Builder("MemcardRex.Linux.GUI.PreferencesWindow.ui"), "preferences_window") {}
}