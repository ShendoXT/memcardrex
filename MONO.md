# How to build and/or run MemcardRex on Linux

The project is working well with Mono.

If you just want to run MemcardRex, follow Step 1 of [Install Build tools and Dependencies](#install-build-tools-and-dependencies) below, and then download MemcardRex from [releases](https://github.com/ShendoXT/memcardrex/releases). Then, follow [Run MemcardRex](#run-memcardrex) and you're running! 
> Note: This last step has to be done every time you want to run MemcardRex. If you want to make this easier, make a `.sh` or `.desktop` file.

### Clone the project
In a directory of your choice, download MemcardRex:

``git clone https://github.com/ShendoXT/memcardrex.git``

### Install Build tools and Dependencies

 1. Install mono from either the [official site](http://www.mono-project.com/download/), or your distribution's package manager. The package included on some Linux distributions may not work properly.
 2. Install nuget from your package manager. If you installed Mono following the website instructions, your package manager will now install the official version of nuget.
 3. Update nuget: ``sudo nuget update -self``
 4. Edit nuget's configuration file in `~/.config/NuGet/NuGet.Config`

Either use the configuration below, or make your own with [Microsoft's documentation](https://docs.microsoft.com/en-us/nuget/consume-packages/configuring-nuget-behavior).
```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    
    <config>

    </config>
    <packageSources>
            <add key="public feed" value="https://api.nuget.org/v3/index.json"/>
    </packageSources>
    <packageRestore>
       <add key="enabled" value="True" />
       <add key="automatic" value="True" />
   </packageRestore>
   <bindingRedirects>
       <add key="skip" value="False" />
   </bindingRedirects>
   <packageManagement>
       <add key="format" value="0" />
      <add key="disabled" value="True" />
  </packageManagement>

</configuration>
```

 5. `cd` to wherever you downloaded MemcardRex to.
 6. Run `nuget restore -NoCache` to download project dependencies.

### Build MemcardRex

 In the folder where the git project is located, run `msbuild` in your terminal.
 If that didn't work (probably because your distro's version of mono is misconfigured) try: ``xbuild /property:TargetFrameworkVersion=v4.0``
 
 If the build is successful, the executable will be in `memcardrex/MemcardRex/bin/Debug/`
 
### Run MemcardRex

Navigate to where the executable is located (see last step) in terminal and run `mono MemcardRex.exe`

If you are trying to use a DexDrive connected by serial port or USB serial, you may have to run mono as root: `sudo mono MemcardRex.exe`
> If you don't want to run MemcardRex with `sudo`, try adding your user to the `dialout` user group with `sudo usermod -a -G dialout $USER` as detailed in this StackExchange post: [Unix StackExchange](https://unix.stackexchange.com/questions/14354/read-write-to-a-serial-port-without-root)

