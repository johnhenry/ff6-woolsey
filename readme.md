# FINAL FANTASY VI T-EDITION & FINAL FANTASY VI T-EDITION EX

ENGLISH TRANSLATION PATCHES

Note: this is a rom, but rather tools to patch a rom.

See: http://tomato.fobby.net/ff6-t-edition/

## How To Use

1. Put a Japanese FF6 T-Edition ROM (fully patched) into the top folder
   The file MUST be named "ff6t.smc"!
2. Put a Japanese FF6 T-Edition EX ROM (fully patched) into the top folder
   The file MUST be named "ff6t-ex.smc"!
3. Run the "a.bat" file. Note that if you simply double click the a.bat
   file, it'll still run but any warning messages, status messages, etc.
   in the build process will instantly disappear when the window
   instantly disappears. I recommend running a.bat from a command
   prompt.
4. If everything is set up properly, the tools will spit out two new files:
   ff6t-english.smc
   ff6t-english-ex.smc

---

Directory Descriptions

    _asm		Contains all the new SNES coding that makes the translation work in-game
    _graphics	Contains the image data for the game's English fonts and more
    _menus		Contains the translated menu text strings, the format is generally:

    				(SNES ADDRESS OF ORIGINAL STRING STRUCT)|[BB][AA]|String

    				Where BB and AA combine into the 16-bit value AABB
    				You can think of these AA and BB values as a weird form of
    				text display coordinates. Generally, you want to always
    				add or subtract by multiples of 2. If you slightly change
    				some of these values and then check the menu text in-game,
    				you'll start to get a feel for how the coordinate system works.

    _text		All the translated text is stored in here
    			There are two subfolders, one for the main FF6 T-Edition patch,
    			and one for the EX patch. Since the EX patch uses a lot of the same
    			names, etc. as the FF6 T-Edition patch, the EX folder has fewer
    			files than the T-Edition folder.

    			If this text inserter program tries to load a text file while
    			building the EX ROM, and if that text file is missing, then the
    			inserter will look for the same file in the T-Edition folder
    			instead.
