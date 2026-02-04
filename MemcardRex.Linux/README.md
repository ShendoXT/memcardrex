# MemcardRex for Linux (alpha)
<img width="1848" height="1132" alt="mcrx linux 2 0a" src="https://github.com/user-attachments/assets/3e8e5a13-7f38-4241-a81c-af44190b0fad" />

This version of MemcardRex for Linux runs on the modern .NET runtime.<br>
WARNING: This is alpha and unfinished software, which has not been fully tested.<br>
Make sure you have backups before using it with any sensitive data.

MemcardRex uses [Gir.Core](https://gircore.github.io) to access GTK functions from C# code.<br>
The required packages will be downloaded through NuGet when building the project.

## Requirements


- .NET 8.0
- GLib development tools
- GTK 4.6 or later
- libadwaita 1.4 or later

## Building

Ubuntu 24.04:
```
sudo apt install dotnet-sdk-8.0 libglib2.0-dev-bin libgtk-4-1 libadwaita-1-0
```

Fedora 40:
```
sudo dnf install dotnet-sdk-8.0 dotnet-runtime-8.0 glib2-devel gtk4 libadwaita
```

To build and run the project, run:
```
dotnet build MemcardRex.Linux
dotnet run --project MemcardRex.Linux
```

To build a standalone executable (does not require .NET to be installed), run:
```
dotnet publish --self-contained MemcardRex.Linux
```

To use PS3 Memory Card Adaptor make sure to install libusb-1.0:
```
sudo apt install libusb-1.0-0-dev libusb-1.0-0
```

### What currently works:

* Open and Save to all supported Memory Card formats.
* Show save properties.
* Edit save header
* Edit save comment
* Delete, restore and format saves.
* Copy and paste to and from temp buffer
* Import and export saves in all supported formats.
* Compare save with temp buffer.
* Hardware interfaces.
* Preferences are working and are preserved between sessions.

### TODO:

* Undo/Redo with history list.
* Plugins.
* Icon editor.
* PocketStation serial read/BIOS dump