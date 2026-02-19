using Gtk;
using System;
using MemcardRex.Core;
using System.Drawing;
using Gdk;
using GdkPixbuf;
using Gio;
using GLib;

namespace MemcardRex.Linux;

public class SaveInfoDialog
{
    private readonly Dialog dialog;
    private readonly Image saveIcon1;
    private readonly Image saveIcon2;
    private readonly Image saveIcon3;
    private readonly Label titleLabel;
    private readonly Label productCodeLabel;
    private readonly Label identifierLabel;
    private readonly Label regionLabel;
    private readonly Label fileTypeLabel;
    private readonly Label slotLabel;
    private readonly Label sizeLabel;
    private readonly Label iconFramesLabel;
    private readonly Button closeButton;

    //Save icons
    Texture[] iconData = new Texture[3];
    Texture[]? mcIconData;
    Texture[]? apIconData;
    int iconIndex = 0;
    int mcIconIndex = 0;
    int apIconIndex = 0;
    int maxCount = 1;
    //int iconBackColor = 0;
    int apIconDelay = 0;
    int apDelayCount = 0;

    private uint _timerId;

    public SaveInfoDialog(Window parent)
    {
        var builder = new Builder("MemcardRex.Linux.GUI.SaveInfoDialog.ui");
        
        dialog = (Dialog)builder.GetObject("dialog")!;
        saveIcon1 = (Image)builder.GetObject("saveIcon1")!;
        saveIcon2 = (Image)builder.GetObject("saveIcon2")!;
        saveIcon3 = (Image)builder.GetObject("saveIcon3")!;
        titleLabel = (Label)builder.GetObject("titleLabel")!;
        productCodeLabel = (Label)builder.GetObject("productCodeLabel")!;
        identifierLabel = (Label)builder.GetObject("identifierLabel")!;
        regionLabel = (Label)builder.GetObject("regionLabel")!;
        fileTypeLabel = (Label)builder.GetObject("fileTypeLabel")!;
        slotLabel = (Label)builder.GetObject("slotLabel")!;
        sizeLabel = (Label)builder.GetObject("sizeLabel")!;
        iconFramesLabel = (Label)builder.GetObject("iconFramesLabel")!;
        closeButton = (Button)builder.GetObject("closeButton")!;
        
        dialog.SetTransientFor(parent);
        
        closeButton.OnClicked += (sender, args) => dialog.Close();

        // 1. Poveži se na Realize signal (izvršava se kad se kreiraju nativni resursi)
        dialog.OnRealize += (sender, args) => StartAnimTimer();
        
        // 2. Obavezno ugasi tajmer kad se dijalog zatvori da ne curi memorija
        dialog.OnUnrealize += (sender, args) => StopAnimTimer();
    }

    public void SetSaveInfo(string title, string productCode, string identifier, 
                           string region, ps1card.DataTypes fileType, int[] slots, 
                           int size, int iconFrames, Color[,][] saveIcons, byte[] mcIcons, byte[] apIcons,
            int iconDelay)
    {
        BmpBuilder bmpImage = new BmpBuilder();
        string framesString = "";

        titleLabel.SetLabel(title);
        productCodeLabel.SetLabel(productCode);
        identifierLabel.SetLabel(identifier);
        regionLabel.SetLabel(region);
        sizeLabel.SetLabel(size.ToString() + " KB");
        framesString = iconFrames.ToString();
        maxCount = iconFrames;

        //Save file data type
        if (fileType == ps1card.DataTypes.save) fileTypeLabel.SetLabel("Save data");
        else if (fileType == ps1card.DataTypes.software) fileTypeLabel.SetLabel("Software (PocketStation)");

        string ocupiedSlots = "";

        //Get ocupied slots
        for (int i = 0; i < slots.Length; i++)
            ocupiedSlots += (slots[i] + 1).ToString() + ", ";

        //Show ocupied slots
        slotLabel.SetLabel(ocupiedSlots.Remove(ocupiedSlots.Length-2));

        //Create icons
        for (int i = 0; i < 3; i++)
            iconData[i] = TextureBuilder.BmpToTexture(bmpImage.BuildBmp(saveIcons[slots[0], i]))!;

        apIconDelay = iconDelay / 10;
        apDelayCount = apIconDelay;

        //Create mcIcons (if available)
        if(mcIcons != null)
        {
            mcIconData = new Texture[mcIcons.Length / 0x80];
            byte[] mcIconArray = new byte[0x80];

            //Add info to icon frames
            framesString += " (" + mcIconData.Length + ")";

            for(int i = 0; i < mcIconData.Length; i++)
            {
                System.Array.Copy(mcIcons, i * 0x80, mcIconArray, 0, 0x80);
                mcIconData[i] = TextureBuilder.BmpToTexture(bmpImage.BuildBmp(mcIconArray))!;
            }
        }

        //Create apIcons (if available)
        if(apIcons != null)
        {
            apIconData = new Texture[apIcons.Length / 0x80];
            byte[] apIconArray = new byte[0x80];

            //Add info to icon frames
            framesString += " (" + apIconData.Length + ")";

            for (int i = 0; i < apIconData.Length; i++)
            {
                System.Array.Copy(apIcons, i * 0x80, apIconArray, 0, 0x80);
                apIconData[i] = TextureBuilder.BmpToTexture(bmpImage.BuildBmp(apIconArray))!;
            }

            //PocketStation starts with a 2nd icon in the APIcon list if there is more than 1 icon
            //Why? I have no idea but we will emulate this in this information dialog
            if (apIconData.Length > 1) apIconIndex = 1;
        }

        iconFramesLabel.SetLabel(framesString);

        drawIcons();
    }

    private void drawIcons(){
        //Show save icon
        saveIcon1.SetFromPaintable(iconData[iconIndex]);

        if (mcIconData != null)
        {
            saveIcon2.SetFromPaintable(mcIconData[mcIconIndex]);
            if (mcIconIndex < (mcIconData.Length - 1)) mcIconIndex++; else mcIconIndex = 0;
        }

        if (apIconData != null)
        {
            if(mcIconData != null) saveIcon3.SetFromPaintable(apIconData[apIconIndex]);
            else saveIcon2.SetFromPaintable(apIconData[apIconIndex]);

            if (apDelayCount > 0) apDelayCount--;
            else
            {
                if (apIconIndex < (apIconData.Length - 1)) apIconIndex++; else apIconIndex = 0;
                apDelayCount = apIconDelay;
            }
        }

        if (iconIndex < (maxCount - 1)) iconIndex++; else iconIndex = 0;
    }

    private void StartAnimTimer()
    {
        _timerId = GLib.Functions.TimeoutAdd(0, 180, () => 
        {
            drawIcons();
            return true; 
        });
    }

    private void StopAnimTimer()
    {
        if (_timerId > 0)
        {
            GLib.Functions.SourceRemove(_timerId);
            _timerId = 0;
        }
    }

    public void Present()
    {
        dialog.Present();
    }
}