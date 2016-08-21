# GameLaunchProxy

This application may be used as an in-between for other applications. This tool was created to resolve issues with RPG Maker games and older games where in the needed fonts would not be installed in the system and thus not not be available. The user may not wish to pollute their system fonts with additional fonts specifically for the purpose of one game. With this tool, fonts may be loaded at startup and removed at shutdown of the proxied application. Additional features will be noted as they are implemented.

This tool can also be utilized for a 2 step proxied Steam launch of a chosen application/emulator/game.

Features:
* Load fonts into Windows temporarily and unload them after execution. Fonts will also be removed on reboot. Some games may not detect the fonts are installed but will properly utilize them anyway.
* Aggressive Focus will attempt to pull the launched program into the foreground once per second for the number of configured seconds.
* Launching emulators and games through Steam via proxy.

Planned future features:
* Support for passing through command line arguments to the proxied application.
* Support for forcing Steam dialogs to the foreground for games needing further information before launching.

Steam Proxy
-----------

To utilize the Steam Proxy feature, a shortcut must be added to steam as indicated by the program's GUI.

The Steam shortcut should be written as such:
~~~~
"X:\path\to\proxy\GameLaunchProxy.exe" -steamproxyactivate IDVALUE
~~~~

The command in your front end to launch this proxy should be as such:
~~~~
"X:\path\to\proxy\GameLaunchProxy.exe" -steamproxysetup IDVALUE -steamproxyname NAME -steamproxyhold "X:\path\to\emulator.exe" -a -b 22 -c "test" "X:\path\to\rom.file"
~~~~

~~~~
-steamproxyactivate IDVALUE
~~~~
This is the ID value of the proxy.  This is used by the launcher to lookup proxy information.
This is also used, if shortcut renaming is working and applied, to find the shortcut in steam.

~~~~
-steamproxysetup IDVALUE
~~~~
This is the ID value of the proxy.  It serves no purpose here except to allow the proxy to find the shortcut for renaming.
It also allows for multiple shortcuts to the same proxy with the same name to exist in Steam.
However, it is suggested you name your shortcuts appropriately for those incidents where the rename functionality fails.

~~~~
-steamproxyname NAME
~~~~
Optional
All instances of "%cleanromname%" in its value will be replaced with the filename of the rom without extension.
Note that if the rom is not the last argument you should avoid using "%cleanromname%".
Names with spaces should be quoted as with any CLI value.

~~~~
-steamproxyhold
~~~~
Optional
When supplied, the proxy will hold for 10 seconds and then look for the target application.  If found, it will hold until said program is closed.
Note that in some cases, such as with the RocketLauncher middleware, this option may cause the middleware to hang open. Be sure to test your setup.
