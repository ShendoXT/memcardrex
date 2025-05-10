# MemcardRex
### Advanced PlayStation 1 Memory Card editor
![memcardrex](https://github.com/user-attachments/assets/82553694-5cd2-49e8-b900-524dc32ccade)

<br>**Features:**
* Tabbed interface - multiple Memory Cards can be opened at the same time.
* Ability to copy, delete, restore, export, import and edit saves.
* Plugin support for 3rd party save editors.
* Hardware interfaces for communication with real Memory Cards.
* PocketStation support (read serial, dump BIOS, push PC time)

<br>**Requirements:**
* .NET 8.

<br>**Supported Memory Card formats:**
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
* PCSX ReARMed/RetroArch(*.srm)
* PSP virtual Memory Card(*.VMP)
* PS3 virtual Memory Card(*.VM1)
* PS Vita "MCX" PocketStation Memory Card(*.BIN)
* POPStarter Virtual Memory Card(*.VMC)

<br>**Supported single save formats:**
* PSXGame Edit single save(*.mcs)
* XP, AR, GS, Caetla single save(*.psx)
* Memory Juggler(*.ps1)
* Smart Link(*.mcb)
* Datel(*.mcx,*.pda)
* RAW single saves
* PS3 virtual saves (*.psv)

### Hardware interfaces
MemcardRex supports communication with the real Memory Cards via external devices.
<br>Make sure to select a proper COM port in Options->Preferences.

<details>
<summary>1. DexDrive</summary>
Original way of transferring data from MemoryCard to PC and vice versa albeit a little quirky.
<br>If you encounter problems, unplug power from DexDrive, unplug it from COM port and connect it all again.

It is recommended that a power cord is connected to DexDrive, otherwise some cards won't be detected.
<br>Works with native COM port or USB based adapters.
</details>
</summary>

<details>
<summary>2. MemCARDuino</summary>
MemCARDuino is an open source Memory Card communication software for various Arduino boards.
https://github.com/ShendoXT/memcarduino
</details>
</summary>

<details>
<summary>3. PS1CardLink</summary>
PS1CardLink is a software for the actual PlayStation and PSOne consoles.
<br>It requires an official or home made TTL serial cable for communication with PC.

With it your console becomes a Memory Card reader similar to the DexDrive and MemCARDuino.

MemcardRex can also talk to the serial port remotely by using a Serial Port Bridge like [esp-link](https://github.com/jeelabs/esp-link).
<br>It conveniently fits into a PSOne which has otherwise no external hardware ports.
<br>https://github.com/ShendoXT/ps1cardlink
</details>
</summary>

<details>
<summary>4. Unirom</summary>
Unirom is a shell for the PlayStation and PSOne consoles.
<br>It requires an official or home made TTL serial cable for communication with PC.
<br>https://unirom.github.io.
</details>
</summary>

<details>
<summary>5. PS3 Memory Card Adaptor</summary>
The PS3 Memory Card Adaptor is an official Sony USB adapter that allows reading and writing PS1 Memory Cards on a PlayStation 3.
<br>To use it on a Windows PC, a custom USB driver needs to be installed.
 
This USB driver can be easily created and installed using [Zadig](https://zadig.akeo.ie) by following these steps:
* Plug the PS3 Memory Card Adaptor into a free USB port and start Zadig.
* Zadig should display the PS3 MCA as an "Unknown Device". Verify that the USB ID matches: 054C 02EA
* Click the Edit checkbox and name the device "PS3 Memory Card Adaptor"
* Ensure that "WinUSB" is selected from the list of Driver options and click the Install Driver button.
    - If you need LibUSB driver support place "libusb-1.0.dll" inside MemcardRex's directory (2.0 RC1 and up only).
* After about 30 seconds Zadig should show a message that the driver was installed successfully.

With the USB driver installed and the PS3 Memory Card Adaptor plugged in, you should now be able to read, write and format PS1 Memory Cards.
</details>
</summary>

### Credits
**Authors:**
<br>Alvaro Tanarro, bitrot-alpha, lmiori92, Nico de Poel, KuromeSan, Robxnano, Shendo.

**Beta testers:**
<br>Gamesoul Master, Xtreme2damax, Carmax91 and NKO.

**Thanks to:**
<br>@ruantec, Cobalt, TheCloudOfSmoke, RedawgTS, Hard core Rikki, RainMotorsports, Zieg, Bobbi, OuTman, Kevstah2004,  Kubusleonidas, Frédéric Brière, Mark James, Cor'e, DeadlySystem, Padraig Flood and Martin Korth (nocash).
