/* Copyright (C) 2024 Rob Hall
 * SPDX-License-Identifier: GPL-3.0-or-later */

using Adw;
using Gtk;
using GObject;
using MemcardRex.Core;
using System.ComponentModel;
using static MemcardRex.Linux.MainWindow;
using static MemcardRex.Core.ps1card;

namespace MemcardRex.Linux;

public class PS1CardTab : Gtk.Box
{
    [Connect] private readonly Gtk.ColumnView saveList;
    private readonly Gtk.SignalListItemFactory iconFactory;
    private readonly Gtk.SignalListItemFactory titleFactory;
    private readonly Gtk.SignalListItemFactory productCodeFactory;
    private readonly Gtk.SignalListItemFactory identifierFactory;
    private readonly Gtk.ColumnViewColumn iconColumn;
    private readonly Gtk.ColumnViewColumn titleColumn;
    private readonly Gtk.ColumnViewColumn productCodeColumn;
    private readonly Gtk.ColumnViewColumn identifierColumn;

    public bool HasUnsavedChanges { get; set; }
    public string? Title {
        get { return memcard.cardName; }
    }
    private Adw.TabPage? Page;

    private readonly ps1card memcard;

    private PS1CardTab(ps1card card, Gtk.Builder builder, string name) : base(builder.GetPointer(name), false)
    {
        builder.Connect(this);
        memcard = card;

        saveList ??= new();
        var listStore = Gio.ListStore.New(PS1Slot.GetGType());
        var model = Gtk.MultiSelection.New(listStore);
        saveList.SetModel(model);

        saveList.OnActivate += (_, args) => {
            var slot = (PS1Slot?) model.GetObject(args.Position);
            Console.WriteLine("Selected item: {0}", slot?.Title);
        };

        iconFactory = Gtk.SignalListItemFactory.New();
        iconFactory.OnSetup += (_, args) => {
            var image = new Gtk.Image();
            var listItem = (Gtk.ListItem) args.Object;
            listItem.SetChild(image);
        };
        iconFactory.OnBind += (_, args) => {
            var listItem = (Gtk.ListItem) args.Object;
            var data = (PS1Slot?) listItem.GetItem();
            var image = (Gtk.Image?) listItem.GetChild();
            if (data != null && image != null) {
                    image.SetFromPaintable(data.Icon);
            }
        };
        iconColumn = Gtk.ColumnViewColumn.New("", iconFactory);
        saveList.AppendColumn(iconColumn);

        titleFactory = Gtk.SignalListItemFactory.New();
        titleFactory.OnSetup += (_, args) => {
            var label = Gtk.Label.New("");
            label.SetXalign(0.0f);
            label.SetEllipsize(Pango.EllipsizeMode.End);
            var listItem = (Gtk.ListItem) args.Object;
            listItem.SetChild(label);
        };
        titleFactory.OnBind += (_, args) => {
            var listItem = (Gtk.ListItem) args.Object;
            var data = (PS1Slot?) listItem.GetItem();
            var label = (Gtk.Label?) listItem.GetChild();
            if (data != null && label != null) {
                label.SetLabel(data.Title);
            }
        };
        titleColumn = Gtk.ColumnViewColumn.New("Title", titleFactory);
        titleColumn.SetExpand(true);
        saveList.AppendColumn(titleColumn);

        productCodeFactory = Gtk.SignalListItemFactory.New();
        productCodeFactory.OnSetup += (_, args) => {
            var label = Gtk.Label.New("");
            label.SetXalign(0.0f);
            var listItem = (Gtk.ListItem) args.Object;
            listItem.SetChild(label);
        };
        productCodeFactory.OnBind += (_, args) => {
            var listItem = (Gtk.ListItem) args.Object;
            var data = (PS1Slot?) listItem.GetItem();
            var label = (Gtk.Label?) listItem.GetChild();
            if (data != null && label != null) {
                label.SetLabel(data.ProductCode);
            }
        };
        productCodeColumn = Gtk.ColumnViewColumn.New("Product code", productCodeFactory);
        saveList.AppendColumn(productCodeColumn);

        identifierFactory = Gtk.SignalListItemFactory.New();
        identifierFactory.OnSetup += (_, args) => {
            var label = Gtk.Label.New("");
            label.SetXalign(0.0f);
            label.SetEllipsize(Pango.EllipsizeMode.End);
            var listItem = (Gtk.ListItem) args.Object;
            listItem.SetChild(label);
        };
        identifierFactory.OnBind += (_, args) => {
            var listItem = (Gtk.ListItem) args.Object;
            var data = (PS1Slot?) listItem.GetItem();
            var label = (Gtk.Label?) listItem.GetChild();
            if (data != null && label != null) {
                label.SetLabel(data.Identifier);
            }
        };
        identifierColumn = Gtk.ColumnViewColumn.New("Identifier", identifierFactory);
        saveList.AppendColumn(identifierColumn);

        for (int i = 0; i < 15; i++)
        {
            var data = new PS1Slot(card, i);
            listStore.Append(data);
        }
    }

    public void SetPage(Adw.TabPage page)
    {
        Page = page;
        Page.SetTitle(memcard.cardName);
        Page.SetTooltip(memcard.cardLocation);
        if (HasUnsavedChanges)
            Page.SetIcon(Gio.ThemedIcon.New("document-modified-symbolic"));
    }

    public void Save(Gtk.Window window)
    {
        if (Page == null) return;

        if (memcard.cardLocation == null) {
            SaveAs(window);
            return;
        }

        if (memcard.SaveMemoryCard(memcard.cardLocation, memcard.cardType, false)) {
            Page.SetIcon(null);
            HasUnsavedChanges = false;
        }
        return;
    }

    public void SaveAs(Gtk.Window window)
    {
        var fileChooser = Gtk.FileChooserNative.New("Save Memory Card", window, Gtk.FileChooserAction.Save, "Save", "Cancel");
        fileChooser.SetModal(true);

        ps1card.CardTypes[] types = [CardTypes.raw, CardTypes.gme, CardTypes.vgs, CardTypes.vmp, CardTypes.mcx];
        var currentFilter = MainWindow.FilterForType(this.memcard.cardType);
        fileChooser.AddFilter(currentFilter);
        foreach (CardTypes type in types)
        {
            if (type != this.memcard.cardType)
                fileChooser.AddFilter(MainWindow.FilterForType(type));
        }
        fileChooser.SetFilter(currentFilter);
        fileChooser.Show();
        fileChooser.OnResponse += (sender, args) => {
            var file = fileChooser.GetFile();
            fileChooser.Destroy();
            if (args.ResponseId != (int) Gtk.ResponseType.Accept)
                return;
            try {
                bool result = memcard.SaveMemoryCard(file!.GetPath()!, memcard.cardType, false);
                if (result) {
                    HasUnsavedChanges = false;
                }
                else {
                    Console.WriteLine("Failed to save memory card: {0}", result);
                    return;
                }
            }
            catch { return; }
        };
        return;
    }

    public PS1CardTab(ps1card card) : this(card, new Gtk.Builder("MemcardRex.Linux.GUI.PS1CardTab.ui"), "card_tab") {}
}