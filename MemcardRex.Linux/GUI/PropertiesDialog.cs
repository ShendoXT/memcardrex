/* Copyright (C) 2024 Rob Hall
 * SPDX-License-Identifier: GPL-3.0-or-later */

using Gio;
using GObject;
using Gtk;
using Adw;
using MemcardRex.Core;
using static MemcardRex.Core.ps1card;

namespace MemcardRex.Linux;

public class PropertiesDialog : Adw.Window
{
    private readonly PS1Slot save;
    [Connect] private readonly Gtk.EditableLabel productCodeLabel;
    [Connect] private readonly Gtk.EditableLabel identifierLabel;
    [Connect] private readonly Adw.ComboRow regionCombo;
    [Connect] private readonly Gtk.Label slotLabel;
    [Connect] private readonly Gtk.Label sizeLabel;

    private PropertiesDialog(Gtk.Builder builder, string name, PS1Slot save) : base(builder.GetPointer(name), false)
    {
        builder.Connect(this);
        productCodeLabel ??= new();
        identifierLabel ??= new();
        regionCombo ??= new();
        slotLabel ??= new();
        sizeLabel ??= new();
        this.save = save;
        this.SetTitle(save.Title);
        uint region = save.Region switch {
            "America" => 0,
            "Europe" => 1,
            "Japan" => 2,
            _ => 0
        };
        regionCombo.SetSelected(region);
        productCodeLabel.SetText(save.ProductCode);
        identifierLabel.SetText(save.Identifier);
        sizeLabel.SetLabel(save.Size.ToString() + " KB");
        slotLabel.SetText(save.SlotNumber.ToString());
    }

    public PropertiesDialog(PS1Slot save) : this(new Gtk.Builder("MemcardRex.Linux.GUI.PropertiesDialog.ui"), "properties_dialog", save) {}
}