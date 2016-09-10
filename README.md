# GameLaunchProxy

This application may be used as an in-between for other applications. This tool was created to resolve issues with RPG Maker games and older games where in the needed fonts would not be installed in the system and thus not not be available. The user may not wish to pollute their system fonts with additional fonts specifically for the purpose of one game. With this tool, fonts may be loaded at startup and removed at shutdown of the proxied application. Additional features will be noted as they are implemented.

This tool can also be utilized for a 2 step proxied Steam launch of a chosen application/emulator/game.

Features:
* Load fonts into Windows temporarily and unload them after execution. Fonts will also be removed on reboot. Some games may not detect the fonts are installed but will properly utilize them anyway.
* Aggressive Focus will attempt to pull the launched program into the foreground once per second for the number of configured seconds.
* Launching emulators and games through Steam via proxy.

## Core Tab
The core configuration of the Proxy application is on this tab.  For the Steam functions to work, the Steam userdata shortcut.vdf path must be set.  To scrape the LaunchBox library the LaunchBox Library LaunchBox.xml path must be set.
The Front End Shortcut section allows for the creation of shortcut paths for use in LaunchBox or another front end.

## Steam Shortcut Names
To utilize the Steam Proxy feature a proxy shortcut must exist in Steam.  This can be done with 1 click to the "Add Default" button.
The LaunchBox library must be scraped to generate a platform and gamename list for the proper function of the advanced Steam Proxy functions, such as shortcut renaming. 
Platform names may be adjusted in the platform list. 
Game names are matched from the LaunchBox Library scrape to the stored name and platform information via the rom filename.  If the library item is an archive, the contained file will also be checked for the case where the front end extracted the rom file.

## Launch Options
These are the original functions of the GameLaunchProxy.  Currently available are temporary font loading and forced game focus.


~~~~
"X:\path\to\proxy\GameLaunchProxy.exe" [-steam] [-steambigpicture] [-name <string>] [-fallbackname <string>] -proxy "X:\path\to\emulator\emulator.exe" "FULL\PATH\TO\ROM\FILE"

-steam : optional, triggers the steam proxy logic
-steambigpicture : optional, triggers steam big picture via steam proxy logic
-name : Name for Steam shortcut.  The system will try to find a SteamProxy.exe shortcut with this name and, failing that, rename the default shortcut.
-fallbackname : Fallback name for the Steam shortcut.  The system will try this name if all attempts at using -name fail.
-proxy : REQUIRED, any instructions after this command will be run through the proxy

%platformname% and %gamename% may be used in -name and -fallbackname, they will be filled from the Launchbox Library Scrape data based on rom filename
~~~~