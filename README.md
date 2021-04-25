# MemcardRex
### Advanced PlayStation 1 Memory Card editor
![memcardrex](https://cloud.githubusercontent.com/assets/8411572/25514938/21160ed8-2be1-11e7-9848-e086a5ac5859.png)

<b>Requirements:</b>
* .NET Framework 3.5.
* Windows® Vista™ or 7 for the glass status bar.

<b>Supported Memory Card formats:</b>
* ePSXe/PSEmu Pro Memory Card(*.mcr)
* DexDrive Memory Card(*.gme)
* pSX/AdriPSX Memory Card(*.bin)
* Bleem! Memory Card(*.mcd)
* VGS Memory Card(*.mem, *.vgs)
* PSXGame Edit Memory Card(*.mc)
* DataDeck Memory Card(*.ddf)
* WinPSM Memory Card(*.ps)
* Smart Link Memory Card(*.psm)
* MCExplorer(*.mci)
* PSP virtual Memory Card(*.VMP) (opening only)
* PS3 virtual Memory Card(*.VM1)

<b>Supported single save formats:</b>
* PSXGame Edit single save(*.mcs)
* XP, AR, GS, Caetla single save(*.psx)
* Memory Juggler(*.ps1)
* Smart Link(*.mcb)
* Datel(*.mcx;*.pda)
* RAW single saves
* PS3 virtual saves (*.psv) (importing only)

### Hardware interfaces
MemcardRex supports communication with the real Memory Cards via external devices:

<b>1. DexDrive</b>
<br>As you may or may not know DexDrive is a very quirky device and sometimes it just refuses to work.
<br>Even the first party software (DexPlorer) has problems with it (failed detection of a device).
<br>If you encounter problems, unplug power from DexDrive, unplug it from COM port and connect it all again.

It is recommended that a power cord is connected to DexDrive, otherwise some cards won't be detected.
<br>Communication was tested on Windows 7 x64 on a real COM port and with a Prolific and FTDI based USB adapters.

To select a COM port DexDrive is connected to go to "Options"->"Preferences".

<b>2. MemCARDuino</b>
<br>[MemCARDuino](https://github.com/ShendoXT/memcarduino) is an open source Memory Card communication software for various Arduino boards.

<b>3. PS1CardLink</b>
<br>[PS1CardLink](https://github.com/ShendoXT/ps1cardlink) is a software for the actual PlayStation and PSOne consoles.
<br>It requires an official or home made TTL serial cable for communication with PC.

With it your console becomes a Memory Card reader similar to the DexDrive and MemCARDuino.

<b>4. PS3 Memory Card Adapter</b>
PS3 Memory Card Support requires [libusb-1.0.dll](https://nzgamer41.win/assets/libusb-1.0.dll) in the same folder as MemcardRex.exe, as well as a libusb-win32 driver installed. The easiest way to install the driver is to download [Zadig](https://zadig.akeo.ie/), go to Options and tick "List All Devices" then find the device with a USB ID of 054C 02EA, then set the driver above the "Replace Driver" button to "libusb-win32 (v1.2.6.0)", then click Replace Driver.

### Credits
<b>Beta testers:</b>
<br>Gamesoul Master, Xtreme2damax and Carmax91.

<b>Thanks to:</b>
<br>@ruantec, Cobalt, TheCloudOfSmoke, RedawgTS, Hard core Rikki, RainMotorsports, Zieg, Bobbi, OuTman, Kevstah2004,  Kubusleonidas, Frédéric Brière, Mark James, Cor'e and DeadlySystem.
