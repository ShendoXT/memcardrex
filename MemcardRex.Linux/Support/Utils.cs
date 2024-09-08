/* Copyright (C) 2024 Rob Hall
 * SPDX-License-Identifier: GPL-3.0-or-later */

﻿using System;
using System.Reflection;
using GdkPixbuf;
using GObject;
using System.Runtime.InteropServices;
using System.Collections;

using static Gtk.Functions;

namespace MemcardRex.Linux;

public class Utils
{
    public static Gdk.Texture? TextureResource(string resourceName)
    {
        try
        {
            var bytes = typeof(Utils).Assembly.ReadResourceAsByteArray(resourceName);
            var pixbuf = PixbufLoader.FromBytes(bytes);
            return Gdk.Texture.NewForPixbuf(pixbuf);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unable to load image resource '{resourceName}': {e.Message}");
            return null;
        }
    }

    public static void CheckLibraryVersions()
    {
        uint adwMajor = Adw.Functions.GetMajorVersion();
        uint adwMinor = Adw.Functions.GetMinorVersion();
        uint adwMicro = Adw.Functions.GetMicroVersion();

        if (adwMajor * 10000 + adwMinor * 100 + adwMicro < 10400) {
            var msg = string.Format("Cannot start application: libadwaita version 1.4.0 or later required, you have {0}.{1}.{2}", adwMajor, adwMinor, adwMicro);
            throw new PlatformNotSupportedException(msg);
        }

        uint gtkMajor = Gtk.Functions.GetMajorVersion();
        uint gtkMinor = Gtk.Functions.GetMinorVersion();
        uint gtkMicro = Gtk.Functions.GetMicroVersion();

        if (gtkMajor * 10000 + gtkMinor * 100 + gtkMicro < 41000) {
            var msg = string.Format("Cannot start application: GTK version 4.10.0 or later required, you have {0}.{1}.{2}", gtkMajor, gtkMinor, gtkMicro);
            throw new PlatformNotSupportedException(msg);
        }
    }
}