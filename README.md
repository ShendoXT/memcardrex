# MemcardRex
### Advanced PlayStation 1 Memory Card editor
![memcardrex](https://cloud.githubusercontent.com/assets/8411572/25514938/21160ed8-2be1-11e7-9848-e086a5ac5859.png)

<b>Requirements:</b>
* .NET Framework 4.8 (Thanks Microsoft 😐).

<b>Extras:</b>
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
* PCSX ReARMed/RetroArch(*.srm)
* PSP virtual Memory Card(*.VMP)
* PS3 virtual Memory Card(*.VM1)
* PS Vita "MCX" PocketStation Memory Card(*.BIN)
* POPStarter Virtual Memory Card(*.VMC)

<b>Supported single save formats:</b>
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

<b>1. DexDrive</b>
<br>Original way of transferring data from MemoryCard to PC and vice versa albeit a little quirky.
<br>If you encounter problems, unplug power from DexDrive, unplug it from COM port and connect it all again.

It is recommended that a power cord is connected to DexDrive, otherwise some cards won't be detected.
<br>Works with native COM port or USB based adapters.

<b>2. MemCARDuino</b>
<br>[MemCARDuino](https://github.com/ShendoXT/memcarduino) is an open source Memory Card communication software for various Arduino boards.

<b>3. PS1CardLink</b>
<br>[PS1CardLink](https://github.com/ShendoXT/ps1cardlink) is a software for the actual PlayStation and PSOne consoles.
<br>It requires an official or home made TTL serial cable for communication with PC.

With it your console becomes a Memory Card reader similar to the DexDrive and MemCARDuino.

MemcardRex can also talk to the serial port remotely by using a Serial Port Bridge like [esp-link](https://github.com/jeelabs/esp-link).
<br>It conveniently fits into a PSOne which has otherwise no external hardware ports.

<b>4. Unirom</b>
<br>[Unirom](https://unirom.github.io) is a shell for the PlayStation and PSOne consoles.
<br>It requires an official or home made TTL serial cable for communication with PC.

<b>5. PS3 Memory Card Adaptor</b>
<br>The PS3 Memory Card Adaptor is an official Sony USB adapter that allows reading and writing PS1 Memory Cards on a PlayStation 3.
<br>To use it on a Windows PC, a custom USB driver needs to be installed.
 
This USB driver can be easily created and installed using [Zadig](https://zadig.akeo.ie) by following these steps:
* Plug the PS3 Memory Card Adaptor into a free USB port and start Zadig.
* Zadig should display the PS3 MCA as an "Unknown Device". Verify that the USB ID matches: 054C 02EA
* Click the Edit checkbox and name the device "PS3 Memory Card Adaptor"
* Ensure that "WinUSB" is selected from the list of Driver options and click the Install Driver button.
* After about 30 seconds Zadig should show a message that the driver was installed successfully.

With the USB driver installed and the PS3 Memory Card Adaptor plugged in, you should now be able to read, write and format PS1 Memory Cards.

### Credits
<b>Authors:</b>
<br>Alvaro Tanarro, bitrot-alpha, lmiori92, Nico de Poel, KuromeSan, Robxnano, Shendo.

<b>Beta testers:</b>
<br>Gamesoul Master, Xtreme2damax and Carmax91.

<b>Thanks to:</b>
<br>@ruantec, Cobalt, TheCloudOfSmoke, RedawgTS, Hard core Rikki, RainMotorsports, Zieg, Bobbi, OuTman, Kevstah2004,  Kubusleonidas, Frédéric Brière, Mark James, Cor'e and DeadlySystem.
