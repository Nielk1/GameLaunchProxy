# GameLaunchProxy

This application may be used as an in-between for other applications. This tool was created to resolve issues with RPG Maker games and older games where in the needed fonts would not be installed in the system and thus not not be available. The user may not wish to pollute their system fonts with additional fonts specifically for the purpose of one game. With this tool, fonts may be loaded at startup and removed at shutdown of the proxied application.  Additional features will be noted as they are implemented.

Features:
* Load fonts into Windows temporarily and unload them after execution.  Fonts will also be removed on reboot.  Some games may not detect the fonts are installed but will properly utilize them anyway.
* Aggressive Focus will attempt to pull the launched program into the forground once per second for the number of configured seconds.

Planned future features:
* Support for passing through command line arguments to the proxied application.
* Support for forcing Steam dialogs to the foreground for games needing further information before launching.
