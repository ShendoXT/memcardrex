/* Copyright (C) 2024 Rob Hall
 * SPDX-License-Identifier: GPL-3.0-or-later */

using Gdk;
using GdkPixbuf;
using GLib;
using GObject;
using MemcardRex.Core;
using static MemcardRex.Core.ps1card;

namespace MemcardRex.Linux;

/*
 * A wrapper class for the individual save slots, to allow
 * easier binding to the list view.
 */
public class PS1Slot(ps1card card, int slotNumber) : GObject.Object(true, [])
{
    private ps1card Card { get; } = card;
    public int SlotNumber { get; } = slotNumber;
    public int Size { get { return Card.saveSize[SlotNumber]; } }
    public string Region { get { return Card.saveRegion[SlotNumber]; } }
    public ps1card.SlotTypes Type { get { return (SlotTypes) Card.slotType[SlotNumber]; } }
    public Gdk.Texture Icon { get { return _icon ??= GetIcon(true); } }
    private Gdk.Texture? _icon = null;
    public string Title {
        get {
            return Type switch
            {
                SlotTypes.formatted => "Free slot",
                SlotTypes.initial or SlotTypes.deleted_initial => Card.saveName[SlotNumber],
                SlotTypes.middle_link or SlotTypes.end_link or SlotTypes.deleted_middle_link or SlotTypes.deleted_end_link => "Linked slot",
                _ => "Corrupted data",
            };
        }
    }
    public string Identifier {
        get {
            return Type switch
            {
                SlotTypes.initial or SlotTypes.deleted_initial => Card.saveIdentifier[SlotNumber],
                _ => "",
            };
        }
    }
    public string ProductCode {
        get {
            return Type switch
            {
                SlotTypes.initial or SlotTypes.deleted_initial => Card.saveProdCode[SlotNumber],
                _ => "",
            };
        }
    }

    //Get slot icon as a texture with or without a region flag
    public Gdk.Texture GetIcon(bool WithFlag)
    {
        int iconWidth = WithFlag ? 48 : 16;

        //Create pixbuf for the output composite
        var buf = GdkPixbuf.Pixbuf.New(Colorspace.Rgb, true, 8, iconWidth, 16);

        //If slot doesn't contain a valid icon return transparent image
        if(Type == SlotTypes.formatted || Type == SlotTypes.corrupted){
            buf!.Fill(0x0); 
            return Texture.NewForPixbuf(buf);
        }

        //On Windows and macOS we can use BMP builder
        //but gtk pixbuff from bytes requires headerless data, so we need to construct it here
        var bytesRgb = new byte[1024];
        int offset = 32;
        int masterSlot = Card.GetMasterLinkForSlot(SlotNumber);
        byte[] iconBytes;
        try {
            iconBytes = Card.GetIconBytes(masterSlot);
        }
        catch {
            Console.WriteLine("Could not get icon bytes");
            return Utils.TextureResource("MemcardRex.Linux.Resources.unknown.png")!;
        }
        for (int i = 0; i < 128; i += 1)
        {
            var color = Card.iconPalette[masterSlot, iconBytes[i + offset] & 0xF];
            bytesRgb[8 * i]     = color.R;
            bytesRgb[8 * i + 1] = color.G;
            bytesRgb[8 * i + 2] = color.B;
            bytesRgb[8 * i + 3] = color.A;
            color = Card.iconPalette[masterSlot, iconBytes[i + offset] >> 4];
            bytesRgb[8 * i + 4] = color.R;
            bytesRgb[8 * i + 5] = color.G;
            bytesRgb[8 * i + 6] = color.B;
            bytesRgb[8 * i + 7] = color.A;
        }
        var pixbuf = GdkPixbuf.Pixbuf.NewFromBytes(GLib.Bytes.New(bytesRgb), Colorspace.Rgb, true, 8, 16, 16, 16 * 4);

        //Copy icon to composite pixbuff
        pixbuf.CopyArea(0, 0, 16, 16, buf!, 0, 0);

        //Set region flag resource name
        if(WithFlag){
            string flagName = "";

            switch(Card.saveRegion[masterSlot]){
                case "America":
                    flagName = "amflag";
                    break;

                case "Europe":
                    flagName = "euflag";
                    break;

                case "Japan":
                    flagName = "jpflag";
                    break;
            }

            if(flagName != ""){
                var flagBytes = typeof(Utils).Assembly.ReadResourceAsByteArray("MemcardRex.Linux.Resources." + flagName + ".bmp");
                var flagPixbuf = PixbufLoader.FromBytes(flagBytes);

                flagPixbuf.CopyArea(0, 0, 30, 16, buf!, 17, 0);
            }
        }

        //Return composite texture
        return Texture.NewForPixbuf(buf!);
    }
}