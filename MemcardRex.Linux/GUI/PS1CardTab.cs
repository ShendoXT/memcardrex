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

public class ListCell : Gtk.Box
{
    public PS1Slot? Slot { get; set; }
    public Gtk.GestureClick Controller { get; }
    public string? Text;
    private Gtk.Label? label;
    private Gtk.Image? image;

    public void SetText(string text)
    {
        if (label == null) {
            label = Gtk.Label.New(text);
            label.SetXalign(0.0f);
            label.SetEllipsize(Pango.EllipsizeMode.End);
            this.Append(label);
        }
        else {
            label.SetText(text);
        }
    }

    public void SetImage(Gdk.Paintable? paintable)
    {
        if (image == null) {
            image = new();
            this.Append(image);
        }
        image.SetFromPaintable(paintable);
    }

    public ListCell(PS1Slot? slot) : base()
    {
        Controller = Gtk.GestureClick.New();
        Controller.SetButton(3);
        this.AddController(Controller);
    }
}

public class PS1CardTab : Gtk.Box
{
    [Connect] private readonly Gtk.ColumnView saveList;
    [Connect] private readonly Gtk.PopoverMenu contextMenu;
    [Connect] private readonly Gtk.PopoverMenu freeContextMenu;
    private readonly Gtk.SingleSelection model;
    private readonly Gtk.SignalListItemFactory iconFactory;
    private readonly Gtk.SignalListItemFactory titleFactory;
    private readonly Gtk.SignalListItemFactory productCodeFactory;
    private readonly Gtk.SignalListItemFactory identifierFactory;
    private readonly Gtk.ColumnViewColumn iconColumn;
    private readonly Gtk.ColumnViewColumn titleColumn;
    private readonly Gtk.ColumnViewColumn productCodeColumn;
    private readonly Gtk.ColumnViewColumn identifierColumn;
    private bool selectionChanging = false;

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
        contextMenu ??= new();
        freeContextMenu ??= new();
        var listStore = Gio.ListStore.New(PS1Slot.GetGType());
        model = Gtk.SingleSelection.New(listStore);
        model.CanUnselect = true;
        model.Autoselect = false;
        var signal = new Signal<SingleSelection>("selection-changed", "selectionChanged");
        signal.Connect(model, OnSelectionChanged);
        saveList.SetModel(model);

        saveList.OnActivate += (_, args) => Properties();

        iconFactory = Gtk.SignalListItemFactory.New();
        iconFactory.OnSetup += (_, args) => {
            var cell = new ListCell(null);
            var listItem = (Gtk.ListItem) args.Object;
            cell.Controller.OnPressed += ShowContextMenu;
            listItem.SetChild(cell);
        };
        iconFactory.OnBind += (_, args) => {
            var listItem = (Gtk.ListItem) args.Object;
            var data = (PS1Slot?) listItem.GetItem();
            var cell = (ListCell?) listItem.GetChild();
            if (data != null && cell != null) {
                cell.SetImage(data.Icon);
                cell.Slot = data;
            }
        };
        iconColumn = Gtk.ColumnViewColumn.New("", iconFactory);
        saveList.AppendColumn(iconColumn);

        titleFactory = Gtk.SignalListItemFactory.New();
        titleFactory.OnSetup += (_, args) => {
            var listItem = (Gtk.ListItem) args.Object;
            var cell = new ListCell(null);
            cell.Controller.OnPressed += ShowContextMenu;
            listItem.SetChild(cell);
        };
        titleFactory.OnBind += (_, args) => {
            var listItem = (Gtk.ListItem) args.Object;
            var data = (PS1Slot?) listItem.GetItem();
            var cell = (ListCell?) listItem.GetChild();
            if (data != null && cell != null) {
                cell.SetText(data.Title);
                cell.Slot = data;
            }
        };
        titleColumn = Gtk.ColumnViewColumn.New("Title", titleFactory);
        titleColumn.SetExpand(true);
        saveList.AppendColumn(titleColumn);

        productCodeFactory = Gtk.SignalListItemFactory.New();
        productCodeFactory.OnSetup += (_, args) => {
            var cell = new ListCell(null);
            var listItem = (Gtk.ListItem) args.Object;
            cell.Controller.OnPressed += ShowContextMenu;
            listItem.SetChild(cell);
        };
        productCodeFactory.OnBind += (_, args) => {
            var listItem = (Gtk.ListItem) args.Object;
            var data = (PS1Slot?) listItem.GetItem();
            var cell = (ListCell?) listItem.GetChild();
            if (data != null && cell != null) {
                cell.SetText(data.ProductCode);
                cell.Slot = data;
            }
        };
        productCodeColumn = Gtk.ColumnViewColumn.New("Product code", productCodeFactory);
        saveList.AppendColumn(productCodeColumn);

        identifierFactory = Gtk.SignalListItemFactory.New();
        identifierFactory.OnSetup += (_, args) => {
            var cell = new ListCell(null);
            var listItem = (Gtk.ListItem) args.Object;
            cell.Controller.OnPressed += ShowContextMenu;
            listItem.SetChild(cell);
        };
        identifierFactory.OnBind += (_, args) => {
            var listItem = (Gtk.ListItem) args.Object;
            var data = (PS1Slot?) listItem.GetItem();
            var cell = (ListCell?) listItem.GetChild();
            if (data != null && cell != null) {
                cell.SetText(data.Identifier);
                cell.Slot = data;
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

    public void ExportSave(Gtk.Window parent)
    {
        var dialog = new Adw.MessageDialog
        {
            Modal = true,
            Heading = "Export Save?",
            Body = "Do you want to export this save?",
            TransientFor = parent,
        };
        dialog.AddResponse("cancel", "Cancel");
        dialog.AddResponse("save", "Export");
        dialog.SetResponseAppearance("save", ResponseAppearance.Suggested);
        dialog.Show();
        Console.WriteLine("Exported save");
    }

    public void Properties()
    {
        var parent = (Gtk.Window?) this.GetAncestor(Gtk.Window.GetGType());
        var slot = SelectedSave();
        if (slot == null || slot.Type == SlotTypes.formatted) return;

        var dialog = new PropertiesDialog(slot);
        dialog.SetTransientFor(parent);
        dialog.SetModal(true);
        dialog.Show();
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

    private void ExportAction(Gio.SimpleAction sender, Gio.SimpleAction.ActivateSignalArgs args)
    {
        Console.WriteLine("Export");
    }

    private void ShowContextMenu(GestureClick obj, GestureClick.PressedSignalArgs args)
    {
        var cell = (ListCell?) obj.GetWidget();
        if (cell == null || cell.Slot == null) return;
        uint slotNumber = (uint) cell.Slot.SlotNumber;
        model.SelectItem(slotNumber, true);
        var menu = cell.Slot.Type switch {
            SlotTypes.formatted => freeContextMenu,
            _ => contextMenu,
        };
        var rect = new Gdk.Rectangle {
            X = (int)args.X,
            Y = (int)args.Y,
            Height = 1,
            Width = 1,
        };
        menu.SetPointingTo(rect);
        menu.Unparent();
        menu.SetParent(obj.GetWidget());
        menu.Popup();
    }

    public void OnSelectionChanged(Gtk.SingleSelection _model, EventArgs args)
    {
        if (selectionChanging) return;
        selectionChanging = true;
        selectionChanging = false;
    }

    private PS1Slot? SelectedSave()
    {
        return (PS1Slot?) model.GetSelectedItem();
    }

    public PS1CardTab(ps1card card) : this(card, new Gtk.Builder("MemcardRex.Linux.GUI.PS1CardTab.ui"), "card_tab") {}
}