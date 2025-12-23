# MemcardRex for Linux (alpha)

This version of MemcardRex for Linux runs on the modern .NET runtime.
WARNING: This is alpha and unfinished software, which has not been fully tested. Make sure you have backups before using it with any sensitive data.

MemcardRex uses [Gir.Core](https://gircore.github.io) to access GTK functions from C# code. The required packages will be downloaded through NuGet when building the project.

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

## Roadmap

### Version 1.0

[x] Open and save memory cards
[ ] Import and export saves to slots
[ ] Edit save headers

### Version 2.0

[ ] Save icon editor
[ ] Hardware support (not tested)

### Not currently planned

[ ] Plugin support

