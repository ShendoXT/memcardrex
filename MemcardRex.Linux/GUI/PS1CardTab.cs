/* Copyright (C) 2024 Rob Hall
 * SPDX-License-Identifier: GPL-3.0-or-later */

using System;
using System.Text;
using Adw;
using Gtk;
using Gdk;
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
    private Gtk.Picture? image;

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
        if (image == null)
        {
            image = Gtk.Picture.New();
            image.SetCanShrink(false);
            this.Append(image);
        }

        image.SetPaintable(paintable);
    }

    public ListCell(PS1Slot? slot) : base()
    {
        Controller = Gtk.GestureClick.New();
        Controller.SetButton(3);
        this.AddController(Controller);
    }
}


public class HistoryRow : GObject.Object
{
    public Gdk.Texture? Icon { get; }
    public string Text { get; }

    public HistoryRow(Gdk.Texture? icon, string text)
        : base(true, [])
    {
        Icon = icon;
        Text = text;
    }
}

public class PS1CardTab : Gtk.Box
{
    [Connect] private readonly Gtk.ColumnView saveList;
    [Connect] private readonly Gtk.ColumnView historyList;
    [Connect] private readonly Gtk.PopoverMenu contextMenu;
    [Connect] private readonly Gtk.PopoverMenu freeContextMenu;
    private readonly Gtk.MultiSelection model;
    private Gtk.SingleSelection historyModel;
    private bool internalSelectionChange = false;
    private readonly Gtk.SignalListItemFactory iconFactory;
    private readonly Gtk.SignalListItemFactory titleFactory;
    private readonly Gtk.SignalListItemFactory productCodeFactory;
    private readonly Gtk.SignalListItemFactory identifierFactory;
    private readonly Gtk.ColumnViewColumn iconColumn;
    private readonly Gtk.ColumnViewColumn titleColumn;
    private readonly Gtk.ColumnViewColumn productCodeColumn;
    private readonly Gtk.ColumnViewColumn identifierColumn;

    private Gio.ListStore listStore;

    public bool HasUnsavedChanges {
        get { return memcard.changedFlag; }
    }
    public string? Title {
        get { return memcard.cardName; }
    }
    public string Location{
        get { return memcard.cardLocation; }
    }
    private Adw.TabPage? Page;

    private readonly ps1card memcard;

    Application mainApp = (Application)Gtk.Application.GetDefault()!;

    private PS1CardTab(ps1card card, Gtk.Builder builder, string name) : base(builder.GetPointer(name), false)
    {
        builder.Connect(this);
        memcard = card;

        saveList ??= new();
        historyList ??= new();
        contextMenu ??= new();
        freeContextMenu ??= new();
        listStore = Gio.ListStore.New(PS1Slot.GetGType());
        var historyStore = Gio.ListStore.New(HistoryRow.GetGType());
        model = Gtk.MultiSelection.New(listStore);
        historyModel = Gtk.SingleSelection.New(historyStore);
        historyModel.Autoselect = false;
        historyModel.CanUnselect = true;
        var signal = new Signal<MultiSelection>("selection-changed", "selectionChanged");
        signal.Connect(model, OnSaveSelectionChanged);

        saveList.SetModel(model);
        historyList.SetModel(historyModel);

        var clickGesture = Gtk.GestureClick.New();
        clickGesture.SetButton(1); // Left click
        clickGesture.SetPropagationPhase(Gtk.PropagationPhase.Capture);

        //Activated or OnPressed is leaving ghost selections all over the list
        //but OnReleased works fine, so OnRelease it is
        clickGesture.OnReleased += (sender, args) => {
            //Double click
            if (args.NPress == 2) 
            {
                Properties();
                clickGesture.SetState(Gtk.EventSequenceState.Claimed);
            }
        };

        saveList.AddController(clickGesture);

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
        iconColumn = Gtk.ColumnViewColumn.New("Icon", iconFactory);
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
        titleColumn.SetFixedWidth(350);
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
        productCodeColumn.SetFixedWidth(130);
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
        identifierColumn.SetFixedWidth(130);
        saveList.AppendColumn(identifierColumn);

        for (int i = 0; i < 15; i++)
        {
            var data = new PS1Slot(card, i);
            listStore.Append(data);
        }

        var historyFactory = Gtk.SignalListItemFactory.New();

        historyFactory.OnSetup += (_, args) =>
        {
            var listItem = (Gtk.ListItem)args.Object;
            var cell = new ListCell(null);
            listItem.SetChild(cell);
        };

        historyFactory.OnBind += (_, args) =>
        {
            var listItem = (Gtk.ListItem)args.Object;
            var row = (HistoryRow?)listItem.GetItem();
            var cell = (ListCell?)listItem.GetChild();

            if (row != null && cell != null)
            {
                cell.SetImage(row.Icon);
                cell.SetText(row.Text);
            }
        };

        var historyColumn = Gtk.ColumnViewColumn.New("History", historyFactory);
        historyColumn.SetFixedWidth(160);
        historyList.AppendColumn(historyColumn);

        historyStore.Append(
            new HistoryRow(null, "Card created")
        );

        //Select first items on the list
        model.SelectItem(0, false);
        historyModel.SelectItem(0, false);
    }

    //Delete/Restore selected save
    public void DeleteRestoreSave()
    {
        if(!ValidityCheck(out var parent, out int masterSlot)) return;

        memcard.ToggleDeleteSave(masterSlot);

        RefreshSaveList();

        /*if (memCard.slotType[masterSlot] == ps1card.SlotTypes.deleted_initial)
            pushHistory("Save deleted", mainTabControl.SelectedIndex, prepareIcons(listIndex, masterSlot, false));
        else
            pushHistory("Save restored", mainTabControl.SelectedIndex, prepareIcons(listIndex, masterSlot, false));*/
    }

    //Format selected save
    public void FormatSave()
    {
        if(!ValidityCheck(out var parent, out int masterSlot)) return;
        //Fetch save icon before deletion
        //Bitmap saveIcon = prepareIcons(listIndex, masterSlot, false);

        memcard.FormatSave(masterSlot);

        RefreshSaveList();

        //pushHistory("Save removed", mainTabControl.SelectedIndex, saveIcon);
    }

    private void UpdatePageInfo(){
        Page!.SetTitle(HasUnsavedChanges ? "● " + memcard.cardName:memcard.cardName);
        Page.SetTooltip(memcard.cardLocation);
    }

    public void SetPage(Adw.TabPage page)
    {
        Page = page;
        UpdatePageInfo();
    }

    public void Save(Gtk.Window window)
    {
        if (Page == null) return;

        if (memcard.cardLocation == null) {
            SaveAs(window);
            return;
        }

        if (memcard.SaveMemoryCard(memcard.cardLocation, memcard.cardType, false)) {
            RefreshSaveList();
        }
        return;
    }

    public void CompareSave(Gtk.Window parent){

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

    //Import save data from external supported single save
    public void ImportSave(Gtk.Window parent){
        //ValidityCheck(out var parent, out int masterSlot);
        var fileChooser = Gtk.FileChooserNative.New("Import save", parent, Gtk.FileChooserAction.Open, "Open", "Cancel");
        fileChooser.SetModal(true);
        fileChooser.AddFilter(SingleSavesFilter());
        //fileChooser.AddFilter(AllFilesFilter());
        fileChooser.Show();
        fileChooser.OnResponse += (sender, args) => {
            var file = fileChooser.GetFile();
            fileChooser.Destroy();
            if (args.ResponseId != (int) Gtk.ResponseType.Accept)
                return;
            try {
                    if (memcard.OpenSingleSave(file!.GetPath()!, (int) SelectedSave()!, out int requiredSlots))
                    {
                        RefreshSaveList();
                        //pushHistory("Save imported", mainTabControl.SelectedIndex, prepareIcons(listIndex, slotNumber, false));
                    }
                    else if (requiredSlots > 0)
                    {
                        NoSpaceMessage(requiredSlots, parent);
                    }
                    else
                    {
                        Utils.ErrorMessage(parent, "Import error", "The file could not be opened.");
                    }
            }
            catch { return; }
        };
    }

    //Export single save
    public void ExportSave(Gtk.Window parent, bool isRaw){
        if(!ValidityCheck(out var xparent, out int masterSlot)) return;

        byte singleSaveType;
        string outputFilename;

        if (isRaw)
        {
            //RAW file name on the system
            outputFilename = memcard.saveRegionRaw[masterSlot] + memcard.saveProdCode[masterSlot] + memcard.saveIdentifier[masterSlot];
        }
        else
        {
            //Set output filename to be compatible with PS3
            byte[] identifierASCII = Encoding.ASCII.GetBytes(memcard.saveIdentifier[masterSlot]);
            outputFilename = memcard.saveRegionRaw[masterSlot] + memcard.saveProdCode[masterSlot] +
                BitConverter.ToString(identifierASCII).Replace("-", "");
        }

        //This will help us preserve full file title if illegal characters were found in save file name
        int illegalCharCount = 0;
        string completeFileName = outputFilename;

        //Filter illegal characters from the name
        foreach (char illegalChar in "\\/\":*?<>|".ToCharArray())
        {
            if (outputFilename.Contains(illegalChar.ToString())) illegalCharCount++;
            outputFilename = outputFilename.Replace(illegalChar.ToString(), "");
        }
    }

    //Get a complete snapshot of the opened Memory Card
    public byte[] GetRawCard(){
        return memcard.SaveMemoryCardStream(mainApp.Settings.FixCorruptedCards == 1);
    }

    //Check if a selected save is valid for the editing operations
    private bool ValidityCheck(out Gtk.Window parent, out int masterSlot){
        parent = (Gtk.Window?) this.GetAncestor(Gtk.Window.GetGType())!;
        masterSlot = 0;

        var slot = SelectedSave();
        if (slot == null) return false;

        masterSlot = (int) slot;

        //Only initial saves
        if(memcard.slotType[masterSlot] == SlotTypes.initial ||
        memcard.slotType[masterSlot] == SlotTypes.deleted_initial) return true;

        return false;
    }

    //Refresh list view after save editing
    private void RefreshSaveList(){
        //Refresh icons
        for (uint i = 0; i < listStore.GetNItems(); i++)
        {
            var handle = listStore.GetItem(i);
            var slot = GObject.Internal.ObjectWrapper.WrapHandle<PS1Slot>(handle, false);
            slot.ForceRefresh();
        }

        //Refresh list view
        saveList.SetModel(null);
        saveList.SetModel(model);

        UpdatePageInfo();
        MainWindow.Instance.SetSlotActionsEnabled(memcard.slotType[(int)SelectedSave()!]);
        MainWindow.Instance.UpdateTitleLocation(HasUnsavedChanges ? "● " + memcard.cardName:memcard.cardName, memcard.cardLocation);
    }

    //Edit save header of the currently selected save
    public void EditHeader(){
        if(!ValidityCheck(out var parent, out int masterSlot)) return;

        var dialog = new HeaderDialog(parent!);

        dialog.SetSaveInfo(title: memcard.saveName[masterSlot],
            region: memcard.saveRegion[masterSlot],
            productCode: memcard.saveProdCode[masterSlot],
            identifier: memcard.saveIdentifier[masterSlot]
        );

        //User pressed OK
        if (dialog.Run())
        {
            memcard.SetHeaderData(masterSlot, dialog.GetProductCode(), dialog.GetIdentifier(), dialog.GetRegion());

            //Refresh list
            RefreshSaveList();
        }
    }

    //Edit save comments of the currently selected save
    public void EditComments(){
        if(!ValidityCheck(out var parent, out int masterSlot)) return;

        var dialog = new CommentsDialog(parent!);

        dialog.SetComments(memcard.saveName[masterSlot], memcard.saveComments[masterSlot]);

        if(dialog.Run()){
            memcard.SetComment(masterSlot, dialog.GetComments());
        }
    }

    //Copy save to global temp buffer
    public bool CopySave(ref byte[] tempBuffer, ref string tempBufferName, out Gdk.Texture? icon){
        icon = null;
        if(!ValidityCheck(out var parent, out int masterSlot)) return false;

        tempBuffer = memcard.GetSaveBytes(masterSlot);
        tempBufferName = memcard.saveName[masterSlot];

        //Fetch icon for the save
        BmpBuilder bmpImage = new BmpBuilder();
        icon = TextureBuilder.BmpToTexture(bmpImage.BuildBmp(memcard.iconColorData[masterSlot, 0]))!;

        return true;
    }

    //Show error message about the lack of space on the Memory Card
    public void NoSpaceMessage(int requiredSlots, Gtk.Window parent){
        var dialog = new Adw.MessageDialog
        {
            Modal = true,
            Heading = "Insufficient space",
            Body = "To complete this operation " + requiredSlots.ToString() + " free slots are required.",
            TransientFor = parent
        };
        dialog.AddResponse("cancel", "Close");
        dialog.Show();
        dialog.OnResponse += (_, dialogArgs) => {
            dialog.Destroy();
        };
    }

    //Paste save from global temp buffer
    public void PasteSave(byte[]? tempBuffer){
        if (tempBuffer == null) return;

        //Grab parent and master slot, not a real validity check
        ValidityCheck(out var parent, out int masterSlot);

        int requiredSlots = 0;
        if (memcard.SetSaveBytes(masterSlot, tempBuffer, out requiredSlots))
        {
            RefreshSaveList();
            //pushHistory("Save pasted", mainTabControl.SelectedIndex, prepareIcons(listIndex, slotNumber, false));
        }
        else
        {
            NoSpaceMessage(requiredSlots, parent);
        }
    }

    //Show properties of a selected save
    public void Properties()
    {
        var parent = (Gtk.Window?) this.GetAncestor(Gtk.Window.GetGType());
        var slot = SelectedSave();
        if (slot == null) return;

        int masterSlot = (int) slot;

        //Only show properties for the initial saves
        if((memcard.slotType[masterSlot] != SlotTypes.initial &&
        memcard.slotType[masterSlot] != SlotTypes.deleted_initial)) return;

        //PocketStation icons
        int iconDelay = 0;
        byte[] mcIconData = memcard.GetPocketStationIcon(masterSlot, ps1card.IconTypes.MCIcon, out iconDelay);
        byte[] apIconData = memcard.GetPocketStationIcon(masterSlot, ps1card.IconTypes.APIcon, out iconDelay);

        var dialog = new SaveInfoDialog(parent!);
        dialog.SetSaveInfo(
            title: memcard.saveName[masterSlot],
            productCode: memcard.saveProdCode[masterSlot],
            identifier: memcard.saveIdentifier[masterSlot],
            region: memcard.saveRegion[masterSlot],
            fileType: memcard.saveDataType[masterSlot],
            slots: memcard.FindSaveLinks(masterSlot),
            size: memcard.saveSize[masterSlot],
            iconFrames: memcard.iconFrames[masterSlot],
            saveIcons: memcard.iconColorData,
            mcIcons: mcIconData,
            apIcons: apIconData,
            iconDelay: iconDelay
        );

        dialog.Present();
    }

    //Select name and format of the Memory Card to save
    public void SaveAs(Gtk.Window window)
    {
        var fileChooser = Gtk.FileChooserNative.New("Save Memory Card", window, Gtk.FileChooserAction.Save, "Save", "Cancel");
        fileChooser.SetModal(true);

        var filterToType = new Dictionary<Gtk.FileFilter, CardTypes>();

        CardTypes[] types = [CardTypes.raw, CardTypes.gme, CardTypes.vgs, CardTypes.vmp, CardTypes.mcx];
        
        foreach (CardTypes type in types)
        {
            var filter = MainWindow.FilterForType(type);
            fileChooser.AddFilter(filter);
            filterToType[filter] = type;

            if (type == this.memcard.cardType)
                fileChooser.SetFilter(filter);
        }

        fileChooser.Show();
        fileChooser.OnResponse += (sender, args) => {
            if (args.ResponseId != (int)Gtk.ResponseType.Accept)
            {
                fileChooser.Destroy();
                return;
            }

            var selectedFilter = fileChooser.GetFilter();
            var targetType = filterToType[selectedFilter!]; 

            var file = fileChooser.GetFile();
            string path = file!.GetPath()!;
            fileChooser.Destroy();

            string extension = targetType switch {
                    CardTypes.gme => ".gme",
                    CardTypes.vgs => ".vgs",
                    CardTypes.vmp => ".vmp",
                    CardTypes.mcx => ".bin",
                    _ => ".mcr" // default for raw
                };

                if (!path.EndsWith(extension, StringComparison.OrdinalIgnoreCase)) {
                    path += extension;
                }

            try {
                bool result = memcard.SaveMemoryCard(path, targetType, false);
                if (result) {
                    this.memcard.cardType = targetType;
                    UpdatePageInfo();
                    RefreshSaveList();
                }
            }
            catch { return; }
        };
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

    //User selected a save slot
    public void OnSaveSelectionChanged(Gtk.MultiSelection sel, EventArgs args)
    {
        //For multislot don't trigger on each link
        if (internalSelectionChange)
            return;

        var bitset = sel.GetSelection();

        ulong index = bitset.GetMinimum();

        if (index == Gtk.Constants.INVALID_LIST_POSITION)
            return;

        internalSelectionChange = true;
        int masterSlot = memcard.GetMasterLinkForSlot((int)index);

        sel.UnselectAll();

        foreach (int slot in memcard.FindSaveLinks(masterSlot))
            sel.SelectItem((uint)slot, false);

        internalSelectionChange = false;

        //Enable or disable edit menus
        MainWindow.Instance.SetSlotActionsEnabled(memcard.slotType[masterSlot]);
    }

    //Return selected master slot for the selected save in the list
    private int? SelectedSave()
    {
        var bitset = model.GetSelection();
        
        if (bitset.GetSize() > 0)
            return memcard.GetMasterLinkForSlot((int)bitset.GetNth(0));
        else return null;
    }

    public PS1CardTab(ps1card card) : this(card, new Gtk.Builder("MemcardRex.Linux.GUI.PS1CardTab.ui"), "card_tab") {}
}