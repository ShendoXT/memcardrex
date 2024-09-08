/* Copyright (C) 2024 Rob Hall
 * SPDX-License-Identifier: GPL-3.0-or-later */

using Gdk;
using GdkPixbuf;
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
    private int SlotNumber { get; } = slotNumber;
    public ps1card.SlotTypes Type { get { return (SlotTypes) Card.slotType[SlotNumber]; } }
    public Gdk.Texture Icon { get { return _icon ??= GetIcon(); } }
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

    public Gdk.Texture GetIcon()
    {
        int frameNumber = 0;
        switch (Type)
        {
            case SlotTypes.formatted:
                var buf = GdkPixbuf.Pixbuf.New(Colorspace.Rgb, true, 8, 16, 16)!;
                buf.Fill(0x00000000);
                return Texture.NewForPixbuf(buf);
            case SlotTypes.initial:
            case SlotTypes.deleted_initial:
                break;
            case SlotTypes.middle_link:
            case SlotTypes.end_link:
            case SlotTypes.deleted_middle_link:
            case SlotTypes.deleted_end_link:
                return Utils.TextureResource("MemcardRex.Linux.Resources.extra.png")!;
            case SlotTypes.corrupted:
            default:
                return Utils.TextureResource("MemcardRex.Linux.Resources.unknown.png")!;
        }
        var bytesRgb = new byte[256 * 4];
        int offset = 128 * frameNumber + 32;
        byte[] iconBytes;
        try {
            iconBytes = Card.GetIconBytes(SlotNumber);
        }
        catch {
            Console.WriteLine("Could not get icon bytes");
            return Utils.TextureResource("MemcardRex.Linux.Resources.unknown.png")!;
        }
        for (int i = 0; i < 128; i += 1)
        {
            var color = Card.iconPalette[SlotNumber, iconBytes[i + offset] & 0xF];
            bytesRgb[8 * i]     = color.R;
            bytesRgb[8 * i + 1] = color.G;
            bytesRgb[8 * i + 2] = color.B;
            bytesRgb[8 * i + 3] = color.A;
            color = Card.iconPalette[SlotNumber, iconBytes[i + offset] >> 4];
            bytesRgb[8 * i + 4] = color.R;
            bytesRgb[8 * i + 5] = color.G;
            bytesRgb[8 * i + 6] = color.B;
            bytesRgb[8 * i + 7] = color.A;
        }
        var pixbuf = GdkPixbuf.Pixbuf.NewFromBytes(GLib.Bytes.New(bytesRgb), Colorspace.Rgb, true, 8, 16, 16, 16 * 4);
        return Texture.NewForPixbuf(pixbuf);
    }
}