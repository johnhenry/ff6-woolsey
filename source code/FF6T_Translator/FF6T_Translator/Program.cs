using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace FF6T_Translator
{
    // FF6 T-Edition Translated Text Insertion Tool
    // by Tomato

    // This program takes all the various text files and such related to the FFVI T-Edition
    // translation and inserts them/applies them to the Japanese ROM in order to create
    // an English version of the ROM. I didn't have the time or font space to implement
    // multiple language character sets like I usually prefer to, unfortunately. This means
    // that translating the game into other languages will require a bit of extra work to
    // create usable font stuff, add new code to this file as needed, etc.
    
    // If you're looking for all the assembly-level tweaks to the ROM's programming, see
    // the "asm" subfolder. This C# program doesn't handle any ASM stuff, it just takes
    // all the data, text, etc. and inserts it into the ROM. See the a.bat file in a text
    // editor to see how the assembly is put together during the build process.

    // Per usual, the code here is messy and not ideal for a public release. Even so, I
    // hope others can use this program, modify it, etc. to achieve their own goals.
    // I don't know anything about open source software licensing but you're free to use
    // any of this programming code however you want. Just, if you use it to become a
    // billionaire, please consider buying Square Enix. Even if the company is haunted.

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            BuildFF6TEnglishRom();
            BuildEXEnglishRom();
        }


        //==========================================================================================

        static void BuildFF6TEnglishRom()
        {
            string romName = "ff6t.smc";
            string newRomName = "ff6t-english.smc";
            TableReader eTable = new TableReader("_text/ff6t-script.tbl");

            //-------------------------------------------------

            if (!File.Exists(romName))
            {
                Console.WriteLine("ERROR! UNABLE TO OPEN BASE ROM FILE: " + romName);
                Environment.Exit(1);
            }

            // we don't want to edit the original files directly, so let's make a copy first
            File.Delete(newRomName);
            File.Copy(romName, newRomName);
            FileStream romFile = new FileStream(newRomName, FileMode.Open);
            romFile.Close();

            /* print out full item names prefixed by their assigned articles for manual review
             * string[] articleBytes = System.IO.File.ReadAllLines("_text/ff6t/ff6t-item-article-data.txt");
            string[] itemNames = System.IO.File.ReadAllLines("_text/ff6t/ff6t-item-names-long.txt");

            for (int i = 0; i < articleBytes.Length; i++)
            {
                string fart = "";
                if (articleBytes[i] == "1")
                {
                    fart = "A ";
                }
                if (articleBytes[i] == "2")
                {
                    fart = "An ";
                }

                Console.WriteLine(fart + itemNames[i]);
            }*/



            Console.WriteLine("-------Building FF6T Translation-------");

            //-------------------------------------------------

            // If you make any super significant text changes, like if you create an all-new hack or translate the text into another language,
            // then paste a big sample of your text into a new file called "dtesample.txt", then uncomment the following line. It will print
            // out a bunch of common substrings. Take this output and put it in the "_text/script-compression-dictionary.txt" file in place
            // of whatever is currently there. This will allow you to compress your text quite a bit. Without compression, the game's main
            // script files won't fit into the ROM we're building.

            //FindMostCommonSubstrings("dtesampletext.txt");
            WriteScriptCompressionLookupData(newRomName, "_text/script-compression-dictionary.txt", eTable, 0x5ffc00, 0x5ffd00);


            string[] scriptLinesBank1 = LoadScript("_text/ff6t/ff6t-translation-bank1.txt");
            scriptLinesBank1 = PrepScript(scriptLinesBank1);
            // !!!! note! need to keep script uncompressed for problem checkers to work properly!!
            WriteScript(newRomName, scriptLinesBank1, eTable, 0x5c9ce0, 0x520000, 0x53feff, 0x2b2b60, 0x2ce1df, "Script Bank 1", true, true);

            string[] scriptLinesBank2 = LoadScript("_text/ff6t/ff6t-translation-bank2.txt");
            scriptLinesBank2 = PrepScript(scriptLinesBank2);
            WriteScript(newRomName, scriptLinesBank2, eTable, 0x51E600, 0x57a710, 0x58deff, 0x593440, 0x599400, "Script Bank 2", true, true);

            WritePlaceNames(newRomName, "_text/ff6t/ff6t-place-names.txt", "", eTable, 0x268400, 0x5FF2A0);
            CheckBattleDialogueForWidthProblems(newRomName, 0x268400, 0x5FF2A0, 127, 224, "Place Names", eTable);

            //-------------------------------------------------

            WriteBattleDialogue(newRomName, "_text/ff6t/ff6t-battle-dialogue.txt", eTable, 0x10FC00, 0x4D4480, 0x4D7eff);
            CheckBattleDialogueForWidthProblems(newRomName, 0x10FC00, 0x4D0000, 256, 207, "Battle Dialogue", eTable);

            WriteGenericBattleText(newRomName, "_text/ff6t/ff6t-battle-text.txt", "", 0, eTable, 0xF3940, 0x599530, 0x59A4ff, "Battle Text"); // Battle Text
            CheckBattleTopTextForWidthProblems(newRomName, 0xF3940, 0x599530, 256, 209, "Battle Text", eTable);

            WriteGenericBattleText(newRomName, "_text/ff6t/ff6t-enemy-text.txt", "", 1, eTable, 0xFE000, 0x552B60, 0x557000, "Enemy Text");  // Enemy text
            CheckBattleDialogueForWidthProblems(newRomName, 0xFE000, 0x550000, 256, 209, "Enemy Text", eTable);


            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-enemy-names.txt", "", 0x59BD60, 10);           // short enemy names
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-enemy-names-long.txt", "", 0x55d650, 20);      // long enemy names (for Libra/Scan message)

            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-magic-names.txt", "", 0x58F000, 5);            // magic spell names
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-magic-names-medium.txt", "", 0x557aa0, 7);     // expanded magic spell names (medium)
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-magic-names-long.txt", "", 0x557890, 8);       // expanded magic spell names (long, for top battle window)
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-magic-names-for-esper-page.txt", "", 0x55c730, 9);       // expanded magic spell names (long, for top battle window)
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-esper-names.txt", "", 0x58EE20, 8);            // esper names for main menu
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-esper-names-short.txt", "", 0x55b490, 8);      // esper names for menu sub windows (short)
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-esper-names-long.txt", "", 0x59d980, 16);      // esper names for status page (long)
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-blue-magic-names.txt", "", 0x58EF00, 8);       // blue magic, etc. spell names (short)
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-blue-magic-names-medium.txt", "", 0x55ce40, 12);      // blue magic, etc. spell names (medium)
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-swordtech-names.txt", "", 0x55a990, 12);       // cyan's sword technique names, asm for hacking this is more complex than others
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-dance-names.txt", "", 0x558d70, 10);           // mog's dance names, short versions for battle
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-dance-names-medium.txt", "", 0x558cb0, 12);    // mog's dance names, medium version for main menu area
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-dance-names-long.txt", "", 0x558e20, 20);      // mog's dance names, long versions for battle
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-battle-menu-actions.txt", "", 0x5591A0, 7);    // action names like "Fight" "Item" "Magic", etc.
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-character-names.txt", "", 0x478c0, 6);         // main characters' names + moogle, etc. names
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-rare-item-names.txt", "", 0xefba0, 8);         // rare item names, extremely shortened though
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-battle-status-effects.txt", "", 0x2ade1, 7);   // status effect names when using magic/items on a character in battle

            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-enemy-moves.txt", "", 0x54d730, 20, true);     // enemy moves
            CheckBattleTextForWidthProblems(newRomName, 0x54d730, 20, 510, 0x6B, "Enemy Move Names", eTable, 20);

            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-attack-names.txt", "", 0x0557C80, 20);         // special attack names, char-specific attacks, etc.
            CheckBattleTextForWidthProblems(newRomName, 0x557c80, 20, 202, 0x6B, "Attack Names", eTable, 20);

            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-esper-attack-names.txt", "", 0x558F20, 20);    // espers' attack names
            CheckBattleTextForWidthProblems(newRomName, 0x558F20, 20, 27, 0x6B, "Esper Attack Names", eTable, 20);

            WriteGenericBattleText(newRomName, "_text/ff6t/ff6t-rare-item-descriptions.txt", "", 1, eTable, 0xefb60, 0x5592e0, 0x559bff, "Rare Item Descs");
            CheckBattleDialogueForWidthProblems(newRomName, 0xefb60, 0x5592e0, 30, 204, "Rare Item Descs", eTable);

            WriteGenericBattleText(newRomName, "_text/ff6t/ff6t-item-descriptions.txt", "", 1, eTable, 0x41000, 0x5CC600, 0x5CfFCF, "Item Descs");
            WriteGenericBattleText(newRomName, "_text/ff6t/ff6t-esper-attack-descriptions.txt", "", 0, eTable, 0xffe40, 0x559C60, 0x55a1ff, "Esper Attk Descs");
            WriteGenericBattleText(newRomName, "_text/ff6t/ff6t-magic-descriptions.txt", "", 0, eTable, 0x26f690, 0x18c9a0, 0x18cfff, "Magic Descs");
            WriteGenericBattleText(newRomName, "_text/ff6t/ff6t-swordtech-descriptions.txt", "", 0, eTable, 0xfffae, 0x55aac0, 0x55af00, "SwdTech Descs");
            WriteGenericBattleText(newRomName, "_text/ff6t/ff6t-rage-descriptions.txt", "", 0, eTable, 0x400e00, 0x401000, 0x402fdf, "Rage Descs");
            WriteGenericBattleText(newRomName, "_text/ff6t/ff6t-blitz-descriptions.txt", "", 0, eTable, 0xfff9e, 0x55d020, 0x55d1cf, "Blitz Descs");
            WriteGenericBattleText(newRomName, "_text/ff6t/ff6t-blue-magic-descriptions.txt", "", 0, eTable, 0xfff3e, 0x55d230, 0x55d5ef, "Blue Magic Descs");

            ReorganizeRageLists(newRomName, "_text/ff6t/ff6t-enemy-names.txt", 0x26600);
            

            //-------------------------------------------------

            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-item-names.txt", "", 0x185000, 9);       // short item names
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-item-names-long.txt", "", 0x59A560, 20); // long item names
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-item-names-medium.txt", "", 0x59db90, 13); // medium item names
            //CheckBattleTextForWidthProblems(newRomName, 0x59A560, 20, 279, 0x6B, "Long Item Names", eTable, 20);

            WriteItemArticleData(newRomName, "_text/ff6t/ff6t-item-article-data.txt", "", 0x59BBa0);

            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-job-names.txt", "", 0x55afc0, 12);       // expanded job names
            WriteGenericItemText(newRomName, "_text/ff6t/ff6t-job-names-long.txt", "", 0x59d1c0, 20);  // expanded job names (longer for status screen)

            //-------------------------------------------------

            // main menu stuff
            ClearData(newRomName, 0x590000, 0x3000);

            int customMenuTextLoc = 0x590860;
            int totalCustomMenuPointerCount = 0;

            WriteMenuData(newRomName, "_menus/ff6t-main-menu.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-file-management.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-item-menu.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-skill-menu.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-equip-menu.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-status-menu.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-config-menu.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-party-setup-menus.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-shop-menus.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-coliseum-menus.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);

            ClearData(newRomName, 0x4A830, 5008, 0);
            WriteGenericBattleText(newRomName, "_menus/ff6t-song-names.txt", "", 0, eTable, 0xe1000, 0x4A830, 0x4Bbbf, "Song Names");

            WriteSimpleStrings(newRomName, "_menus/ff6t-misc-strings.txt", "", eTable);
        }


        //==========================================================================================

        static void BuildEXEnglishRom()
        {
            string romName = "ff6t-ex.smc";
            string newRomName = "ff6t-ex-english.smc";
            TableReader eTable = new TableReader("_text/ff6t-script.tbl");

            //-------------------------------------------------

            if (!File.Exists(romName))
            {
                Console.WriteLine("ERROR! UNABLE TO OPEN BASE ROM FILE: " + romName);
                Environment.Exit(1);
            }

            // we don't want to edit the original files directly, so let's make a copy first
            File.Delete(newRomName);
            File.Copy(romName, newRomName);
            FileStream romFile = new FileStream(newRomName, FileMode.Open);
            romFile.Close();

            Console.WriteLine("\n\n-------Building FF6T-EX Translation-------");

            //-------------------------------------------------

            //FindMostCommonSubstrings("dtesampletext.txt");
            WriteScriptCompressionLookupData(newRomName, "_text/script-compression-dictionary.txt", eTable, 0x5ffc00, 0x5ffd00);


            string[] scriptLinesBank1 = LoadScript("_text/ex/ex-script-bank1.txt");
            scriptLinesBank1 = PrepScript(scriptLinesBank1);
            WriteScript(newRomName, scriptLinesBank1, eTable, 0x5c9ce0, 0x520000, 0x53feff, 0x2b2b60, 0x2ce1df, "Script Bank 1", true, false);

            string[] scriptLinesBank2 = LoadScript("_text/ex/ex-script-bank2.txt");
            scriptLinesBank2 = PrepScript(scriptLinesBank2);
            WriteScript(newRomName, scriptLinesBank2, eTable, 0x51E600, 0x560000, 0x58deff, 0x5FD180, 0x5FF1a0, "Script Bank 2", true, false);

            WritePlaceNames(newRomName, "_text/ex/ex-place-names.txt", "", eTable, 0x268400, 0x5FF2A0);
            // used to have a check here using CheckScriptForWidthProblems, accidentally removed it           

            //-------------------------------------------------

            WriteBattleDialogue(newRomName, "_text/ex/ex-battle-dialogue.txt", eTable, 0x10FC00, 0x4D4480, 0x4D7eff);
            CheckBattleDialogueForWidthProblems(newRomName, 0x10FC00, 0x4D0000, 256, 209, "Battle Dialogue", eTable);

            WriteGenericBattleText(newRomName, "_text/ex/ex-battle-text.txt", "", 0, eTable, 0xF3940, 0x599530, 0x59A4ff, "Battle Text"); // Battle Text
            CheckBattleTopTextForWidthProblems(newRomName, 0xF3940, 0x599530, 256, 209, "Battle Text", eTable);
            
            WriteGenericBattleText(newRomName, "_text/ex/ex-enemy-text.txt", "", 1, eTable, 0xFE000, 0x552B60, 0x557000, "Enemy Text");  // Enemy text
            CheckBattleDialogueForWidthProblems(newRomName, 0xFE000, 0x550000, 256, 209, "Enemy Text", eTable);


            WriteGenericItemText(newRomName, "_text/ex/ex-enemy-names.txt", "_text/ff6t/ff6t-enemy-names.txt", 0x59BD60, 10);           // short enemy names
            WriteGenericItemText(newRomName, "_text/ex/ex-enemy-names-long.txt", "_text/ff6t/ff6t-enemy-names-long.txt", 0x55d650, 20);      // long enemy names (for Libra/Scan message)

            WriteGenericItemText(newRomName, "_text/ex/ex-magic-names.txt", "_text/ff6t/ff6t-magic-names.txt", 0x58F000, 5);            // magic spell names
            WriteGenericItemText(newRomName, "_text/ex/ex-magic-names-medium.txt", "_text/ff6t/ff6t-magic-names-medium.txt", 0x557aa0, 7);     // expanded magic spell names (medium)
            WriteGenericItemText(newRomName, "_text/ex/ex-magic-names-long.txt", "_text/ff6t/ff6t-magic-names-long.txt", 0x557890, 8);       // expanded magic spell names (long, for top battle window)
            WriteGenericItemText(newRomName, "_text/ex/ex-magic-names-for-esper-page.txt", "_text/ff6t/ff6t-magic-names-for-esper-page.txt", 0x55c730, 9);       // expanded magic spell names (long, for top battle window)
            WriteGenericItemText(newRomName, "_text/ex/ex-esper-names.txt", "_text/ff6t/ff6t-esper-names.txt", 0x58EE20, 8);            // esper names for main menu
            WriteGenericItemText(newRomName, "_text/ex/ex-esper-names-short.txt", "_text/ff6t/ff6t-esper-names-short.txt", 0x55b490, 8);      // esper names for menu sub windows (short)
            WriteGenericItemText(newRomName, "_text/ex/ex-esper-names-long.txt", "_text/ff6t/ff6t-esper-names-long.txt", 0x59d980, 16);      // esper names for status page (long)
            WriteGenericItemText(newRomName, "_text/ex/ex-blue-magic-names.txt", "_text/ff6t/ff6t-blue-magic-names.txt", 0x58EF00, 8);       // blue magic, etc. spell names (short)
            WriteGenericItemText(newRomName, "_text/ex/ex-blue-magic-names-medium.txt", "_text/ff6t/ff6t-blue-magic-names-medium.txt", 0x55ce40, 12);      // blue magic, etc. spell names (medium)
            WriteGenericItemText(newRomName, "_text/ex/ex-swordtech-names.txt", "_text/ff6t/ff6t-swordtech-names.txt", 0x55a990, 12);       // cyan's sword technique names, asm for hacking this is more complex than others
            WriteGenericItemText(newRomName, "_text/ex/ex-dance-names.txt", "_text/ff6t/ff6t-dance-names.txt", 0x558d70, 10);           // mog's dance names, short versions for battle
            WriteGenericItemText(newRomName, "_text/ex/ex-dance-names-medium.txt", "_text/ff6t/ff6t-dance-names-medium.txt", 0x558cb0, 12);    // mog's dance names, medium version for main menu area
            WriteGenericItemText(newRomName, "_text/ex/ex-dance-names-long.txt", "_text/ff6t/ff6t-dance-names-long.txt", 0x558e20, 20);      // mog's dance names, long versions for battle
            WriteGenericItemText(newRomName, "_text/ex/ex-battle-menu-actions.txt", "_text/ff6t/ff6t-battle-menu-actions.txt", 0x5591A0, 7);    // action names like "Fight" "Item" "Magic", etc.
            WriteGenericItemText(newRomName, "_text/ex/ex-character-names.txt", "_text/ff6t/ff6t-character-names.txt", 0x478c0, 6);         // main characters' names + moogle, etc. names
            WriteGenericItemText(newRomName, "_text/ex/ex-rare-item-names.txt", "_text/ff6t/ff6t-rare-item-names.txt", 0xefba0, 8);         // rare item names, extremely shortened though
            WriteGenericItemText(newRomName, "_text/ex/ex-battle-status-effects.txt", "_text/ff6t/ff6t-battle-status-effects.txt", 0x2ade1, 7);   // status effect names when using magic/items on a character in battle

            WriteGenericItemText(newRomName, "_text/ex/ex-enemy-moves.txt", "_text/ff6t/ff6t-enemy-moves.txt", 0x54d730, 20, true);     // enemy moves
            CheckBattleTextForWidthProblems(newRomName, 0x54d730, 20, 510, 0x6B, "Attack Names", eTable, 20);

            WriteGenericItemText(newRomName, "_text/ex/ex-attack-names.txt", "_text/ff6t/ff6t-attack-names.txt", 0x0557C80, 20);         // special attack names, char-specific attacks, etc.
            CheckBattleTextForWidthProblems(newRomName, 0x557c80, 20, 202, 0x6B, "Attack Names", eTable, 20);

            WriteGenericItemText(newRomName, "_text/ex/ex-esper-attack-names.txt", "_text/ff6t/ff6t-esper-attack-names.txt", 0x558F20, 20);    // espers' attack names
            CheckBattleTextForWidthProblems(newRomName, 0x558F20, 20, 27, 0x6B, "Esper Attack Names", eTable, 20);

            WriteGenericBattleText(newRomName, "_text/ex/ex-rare-item-descriptions.txt", "_text/ff6t/ff6t-rare-item-descriptions.txt", 1, eTable, 0xefb60, 0x5592e0, 0x559bff, "Rare Item Descs");
            CheckBattleDialogueForWidthProblems(newRomName, 0xefb60, 0x5592e0, 30, 0xCC, "Rare Item Descs", eTable);
            // rare item descs max width 204 pixels

            WriteGenericBattleText(newRomName, "_text/ex/ex-item-descriptions.txt", "_text/ff6t/ff6t-item-descriptions.txt", 1, eTable, 0x41000, 0x5CC600, 0x5CfFCF, "Item Descs");
            WriteGenericBattleText(newRomName, "_text/ex/ex-esper-attack-descriptions.txt", "_text/ff6t/ff6t-esper-attack-descriptions.txt", 0, eTable, 0xffe40, 0x559C60, 0x55a1ff, "Esper Attk Descs");
            WriteGenericBattleText(newRomName, "_text/ex/ex-magic-descriptions.txt", "_text/ff6t/ff6t-magic-descriptions.txt", 0, eTable, 0x26f690, 0x18c9a0, 0x18cfff, "Magic Descs");
            WriteGenericBattleText(newRomName, "_text/ex/ex-swordtech-descriptions.txt", "_text/ff6t/ff6t-swordtech-descriptions.txt", 0, eTable, 0xfffae, 0x55aac0, 0x55af00, "SwdTech Descs");
            WriteGenericBattleText(newRomName, "_text/ex/ex-rage-descriptions.txt", "_text/ff6t/ff6t-rage-descriptions.txt", 0, eTable, 0x400e00, 0x401000, 0x402fdf, "Rage Descs");
            WriteGenericBattleText(newRomName, "_text/ex/ex-blitz-descriptions.txt", "_text/ff6t/ff6t-blitz-descriptions.txt", 0, eTable, 0xfff9e, 0x55d020, 0x55d1cf, "Blitz Descs");
            WriteGenericBattleText(newRomName, "_text/ex/ex-blue-magic-descriptions.txt", "_text/ff6t/ff6t-blue-magic-descriptions.txt", 0, eTable, 0xfff3e, 0x55d230, 0x55d5ef, "Blue Magic Descs");

            ReorganizeRageLists(newRomName, "_text/ff6t/ff6t-enemy-names.txt", 0x26600);

            //-------------------------------------------------

            WriteGenericItemText(newRomName, "_text/ex/ex-item-names.txt", "_text/ff6t/ff6t-item-names.txt", 0x185000, 9);       // short item names
            WriteGenericItemText(newRomName, "_text/ex/ex-item-names-long.txt", "_text/ff6t/ff6t-item-names-long.txt", 0x59A560, 20); // long item names
            WriteGenericItemText(newRomName, "_text/ex/ex-item-names-medium.txt", "_text/ff6t/ff6t-item-names-medium.txt", 0x59db90, 13); // long item names
            //CheckBattleTextForWidthProblems(newRomName, 0x59A560, 20, 279, 0x6B, "Long Item Names", eTable, 20);

            WriteItemArticleData(newRomName, "_text/ex/ex-item-article-data.txt", "_text/ff6t/ff6t-item-article-data.txt", 0x59BB70);

            WriteGenericItemText(newRomName, "_text/ex/ex-job-names.txt", "_text/ff6t/ff6t-job-names.txt", 0x55afc0, 12);       // expanded job names
            WriteGenericItemText(newRomName, "_text/ex/ex-job-names-long.txt", "_text/ff6t/ff6t-job-names-long.txt", 0x59d1c0, 20);  // expanded job names (longer for status screen)

            //-------------------------------------------------

            // main menu stuff
            ClearData(newRomName, 0x590000, 0x3000);

            int customMenuTextLoc = 0x590860;
            int totalCustomMenuPointerCount = 0;

            WriteMenuData(newRomName, "_menus/ff6t-main-menu.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-file-management.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-item-menu.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-skill-menu.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-equip-menu.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-status-menu.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-config-menu.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-party-setup-menus.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-shop-menus.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);
            WriteMenuData(newRomName, "_menus/ff6t-coliseum-menus.txt", "", eTable, ref totalCustomMenuPointerCount, ref customMenuTextLoc);

            ClearData(newRomName, 0x4A830, 5008, 0);
            WriteGenericBattleText(newRomName, "_menus/ff6t-song-names.txt", "", 0, eTable, 0xe1000, 0x4A830, 0x4Bbbf, "Song Names");

            WriteSimpleStrings(newRomName, "_menus/ff6t-misc-strings.txt", "", eTable);
        }

        //==========================================================================================

        static string[] LoadTextAsset_AllLines(string primaryFilename, string secondaryFilename = "")
        {
            string[] str = {};

            if (File.Exists(primaryFilename))
            {
                str = System.IO.File.ReadAllLines(primaryFilename);
            }
            else if (secondaryFilename != "" && File.Exists(secondaryFilename))
            {
                str = System.IO.File.ReadAllLines(secondaryFilename);
            }
            else
            {
                string errorStr = "ERROR: Unable to load files " + primaryFilename.ToString().ToUpper();
                if (secondaryFilename != "")
                {
                    errorStr += " / " + secondaryFilename.ToString().ToUpper();
                }

                Console.WriteLine(errorStr);
            }

            return str;
        }

        //==========================================================================================

        static string LoadTextAsset_AllText(string primaryFilename, string secondaryFilename = "")
        {
            string str = "";

            if (File.Exists(primaryFilename))
            {
                str = File.ReadAllText(primaryFilename);
            }
            else if (secondaryFilename != "" && File.Exists(secondaryFilename))
            {
                str = File.ReadAllText(secondaryFilename);
            }
            else
            {
                string errorStr = "ERROR: Unable to load files " + primaryFilename.ToString().ToUpper();
                if (secondaryFilename != "")
                {
                    errorStr += " / " + secondaryFilename.ToString().ToUpper();
                }

                Console.WriteLine(errorStr);
            }

            return str;
        }

        //==========================================================================================

        static void WriteMenuData(string newRomName, string filename1, string filename2, TableReader table, ref int totalPointerCount, ref int lastCustomTextAddress)
        {
            string[] menuEntries = LoadTextAsset_AllLines(filename1, filename2);  //System.IO.File.ReadAllLines(filename);
            FileStream output = new FileStream(newRomName, FileMode.Open);

            string[] splitter = { "|" };

            int customPointerBlockStart = 0x590000;
            //int customTextLoc = 0x590860;

            for (int i = 0; i < menuEntries.Length; i++)
            {
                // parse our menu text file format of "ADDR|DATA"
                
                string[] menuEntryData = menuEntries[i].Split(splitter, StringSplitOptions.None);
                int origPointerLocation = Int32.Parse(menuEntryData[0], System.Globalization.NumberStyles.HexNumber);

                // assign the current menu entry a unique value based on the current count,
                // then write that value into first 2 bytes of the text struct entry
                // any offsets < 0x400 will be caught by our custom ASM and treated
                // as custom menu text to be read from our new expanded location
                
                output.Seek(origPointerLocation, SeekOrigin.Begin);
                //int origOffset = (int)output.ReadByte() | (int)(output.ReadByte() << 8);
                //int origTextStructLoc = ((bank >> 16) << 16) + origOffset;

                //output.Seek(origTextStructLoc, SeekOrigin.Begin);
                output.WriteByte((byte)(totalPointerCount & 0xFF));
                output.WriteByte((byte)((totalPointerCount >> 8) & 0xFF));

                // now we write in our custom text offset block where our new text will be
                int newOffset = lastCustomTextAddress & 0xFFFF;
                output.Seek(customPointerBlockStart + totalPointerCount * 2, SeekOrigin.Begin);
                output.WriteByte((byte)(newOffset& 0xFF));
                output.WriteByte((byte)((newOffset >> 8) & 0xFF));

                // now we write the actual data to the position we want
                menuEntryData[1] = menuEntryData[1] + "[00]";
                table.WriteLine(menuEntryData[1], output, lastCustomTextAddress);

                // these are meant to carry over between calls to this function
                lastCustomTextAddress = (int)output.Position;
                
                // might need to end on an even number to make things display properly??
                if (lastCustomTextAddress % 2 == 1)
                {
                    output.WriteByte(0x00);
                    lastCustomTextAddress++;
                }

                totalPointerCount++;

                // now let's clear out the previously existing menu text to help us find any loose text in a future manual search
                int pos = 0;
                int ch = -1;
                byte fillerByte = 0xd3;
                while (ch != 0x00)
                {
                    output.Seek(origPointerLocation + 2 + pos, SeekOrigin.Begin);
                    ch = output.ReadByte();

                    if (ch != 0x00)
                    {
                        output.Seek(origPointerLocation + 2 + pos, SeekOrigin.Begin);
                        output.WriteByte(fillerByte);
                    }

                    pos++;
                }
                
            }

            output.Close();
        }

        //==========================================================================================

        static void WriteSimpleStrings(string newRomName, string filename1, string filename2, TableReader table)
        {
            string[] menuEntries = LoadTextAsset_AllLines(filename1, filename2); //System.IO.File.ReadAllLines(filename);
            FileStream output = new FileStream(newRomName, FileMode.Open);

            string[] splitter = { "|" };

            for (int i = 0; i < menuEntries.Length; i++)
            {
                // parse our menu text file format of "ADDR|DATA"
                string[] menuEntryData = menuEntries[i].Split(splitter, StringSplitOptions.None);
                int writeLocation = Int32.Parse(menuEntryData[0], System.Globalization.NumberStyles.HexNumber);

                // write the actual data to the position we want
                table.WriteLine(menuEntryData[1], output, writeLocation);
            }

            output.Close();
        }
    

        //==========================================================================================

        static void ClearData(string newRomName, int dataStart, int amount, int clearValue = 0x37)
        {
            FileStream output = new FileStream(newRomName, FileMode.Open);

            output.Seek(dataStart, SeekOrigin.Begin);
            for (int i = 0; i < amount; i++)
            {
                output.WriteByte((byte)clearValue);
            }

            output.Close();
        }

        //==========================================================================================

        static string[] LoadScript(string filename)
        {
            string textBlob = LoadTextAsset_AllText(filename, ""); //System.IO.File.ReadAllText(filename);
            string[] splitter = { "\n--------------------\n" };
            return textBlob.Split(splitter, StringSplitOptions.None);
        }

        //==========================================================================================


        static string PrepScriptLine(string line)
        {
            string retVal = line;

            string[] replacements = {
                "...|…",
                "\r\n[NEWPAGE]\r\n|[13]",
                "\n[NEWPAGE]\n|[13]",
                "\r\n|[01]",
                "\n|[01]",
                "[LONGPAUSE]|[10]",
                "[SHORTPAUSE]|[12]",
                "[CENTER]|[SPACE_00]",
                "[PAUSE_|[11][",
                "[SPACE_|[14][",
                "[CHOICE]|[15]",
                "[WAIT_|[16][",
                "[GP]|[19]",
                "[ITEM]|[1A]",

                "[TERRA]|[02]",
                "[LOCKE]|[03]",
                "[CYAN]|[04]",
                "[SHADOW]|[05]",
                "[EDGAR]|[06]",
                "[SABIN]|[07]",
                "[CELES]|[08]",
                "[STRAGO]|[09]",
                "[RELM]|[0A]",
                "[SETZER]|[0B]",
                "[MOG]|[0C]",
                "[GAU]|[0D]",
                "[GOGO]|[0E]",
                "[UMARO]|[0F]",

                "[TERRA_ALT]|[F0]",
                "[LOCKE_ALT]|[F1]",
                "[CYAN_ALT]|[F2]",
                "[SHADOW_ALT]|[F3]",
                "[EDGAR_ALT]|[F4]",
                "[SABIN_ALT]|[F5]",
                "[CELES_ALT]|[F6]",
                "[STRAGO_ALT]|[F7]",
                "[RELM_ALT]|[F8]",
                "[SETZER_ALT]|[F9]",
                "[MOG_ALT]|[FA]",
                "[GAU_ALT]|[FB]",
                "[GOGO_ALT]|[FC]",
                "[UMARO_ALT]|[FD]",
                "[WEDGE]|[FE]",
                "\n[13]|[13]",
            };

            for (int j = 0; j < replacements.Length; j++)
            {
                string[] splitter = { "|" };
                string[] temp = replacements[j].Split(splitter, StringSplitOptions.None);

                if (temp.Length > 1)
                {
                    retVal = retVal.Replace(temp[0], temp[1]);
                }
            }
            

            return retVal;
        }


        static string[] PrepScript(string[] scriptLines)
        {
            for (int i = 0; i < scriptLines.Length; i++)
            {
                scriptLines[i] = PrepScriptLine(scriptLines[i]);
                scriptLines[i] += "[00]";
            }

            // make quote marks work properly automatically
            for (int i = 0; i < scriptLines.Length; i++)
            {
                int totalQuoteMarksFound = 0;
                for (int j = 0; j < scriptLines[i].Length; j++)
                {
                    if (scriptLines[i][j] == '\"')
                    {
                        totalQuoteMarksFound++;
                        if (totalQuoteMarksFound % 2 == 0)
                        {
                            string temp = scriptLines[i];
                            string tempStr1 = scriptLines[i].Substring(0, j);
                            string tempStr2 = scriptLines[i].Substring(j + 1);

                            scriptLines[i] = tempStr1 + "”" + tempStr2;
                        }
                    }
                }

                if (totalQuoteMarksFound % 2 == 1)
                {
                    Console.WriteLine("MISSING QUOTE MARK: " + scriptLines[i]);
                }
            }

            return scriptLines;
        }

        //==========================================================================================

        static string[] PrepBattleText(string[] scriptLines)
        {
            string[] replacements = {
                "...|…",
                "\r\n[NEWPAGE]\r\n|[01]",
                "\n[NEWPAGE]\n|[01]",
                "\r\n|[01]",
                "\n|[01]",
                "[COLOR]|[04]",
                "[PAUSE]|[05]",
                "[WAIT]|[07]",
                
                "[ACTOR]|[12][00]",
                "[ITEM]|[12][01]",
                "[SKILL]|[12][02]",
                "[COMMAND]|[12][03]",

                "[TERRA]|[02][00]",
                "[LOCKE]|[02][01]",
                "[CYAN]|[02][02]",
                "[SHADOW]|[02][03]",
                "[EDGAR]|[02][04]",
                "[SABIN]|[02][05]",
                "[CELES]|[02][06]",
                "[STRAGO]|[02][07]",
                "[RELM]|[02][08]",
                "[SETZER]|[02][09]",
                "[MOG]|[02][0A]",
                "[GAU]|[02][0B]",
                "[GOGO]|[02][0C]",
                "[UMARO]|[02][0D]",
                "[GUEST1]|[02][0E]",
                "[GUEST2]|[02][0F]",

            };

            for (int i = 0; i < scriptLines.Length; i++)
            {
                for (int j = 0; j < replacements.Length; j++)
                {
                    string[] splitter = { "|" };
                    string[] temp = replacements[j].Split(splitter, StringSplitOptions.None);

                    if (temp.Length > 1)
                    {
                        scriptLines[i] = scriptLines[i].Replace(temp[0], temp[1]);
                    }

                    // let's end every line with an EOL control code
                }

                scriptLines[i] += "[00]";
            }

            return scriptLines;
        }

        //==========================================================================================

        // call this to find out what the most common substrings are to manually build your own compression-dictionary.txt file
        static void FindMostCommonSubstrings(string dteSampleFileName)
        {
            var text = File.ReadAllText(dteSampleFileName);
 
            var match = Regex.Match(text, "\\w+");
            Dictionary<string, int> freq = new Dictionary<string, int>();
            while (match.Success) {
                string word = match.Value;
                if (freq.ContainsKey(word)) {
                    freq[word]++;
                } else {
                    freq.Add(word, 1);
                }
 
                match = match.NextMatch();
            }

            foreach (var elem in freq.OrderByDescending(a => a.Value).Take(112).OrderBy(a => a.Key.Length).Reverse()) {
                if (elem.Key.Length == 1)
                {
                    Console.WriteLine(elem.Key + " ");
                }
                else
                {
                    Console.WriteLine(elem.Key);
                }
            }
        }

        //==========================================================================================

        static void WriteScriptCompressionLookupData(string newRomName, string filename, TableReader table, int pointerDataStart, int textDataStart)
        {
            string[] dictionary = System.IO.File.ReadAllLines(filename);
            FileStream output = new FileStream(newRomName, FileMode.Open);

            int textDataLoc = textDataStart;
            int totalTextDataSize = 0;

            for (int i = 0; i < dictionary.Length; i++)
            {
                // write the line as byte data into the ROM now
                dictionary[i] = dictionary[i] + "[00]";
                int convertedLength = table.WriteLine(dictionary[i], output, textDataLoc);
                totalTextDataSize += convertedLength;

                // write the pointer to the current line
                // note that if we want to use an area not in ExHiROM space then we gotta do the ordinary address conversion

                int pointerLoc = pointerDataStart + i * 2;
                output.Seek(pointerLoc, SeekOrigin.Begin);
                output.WriteByte((byte)(textDataLoc & 0xFF));
                output.WriteByte((byte)((textDataLoc >> 8) & 0xFF));

                textDataLoc += convertedLength;
            }

            output.Close();
        }

        //==========================================================================================

        static string[] CompressScript(string[] lines, string compressionListFileName, int dteStartNumber)
        {
            string[] dictionary = System.IO.File.ReadAllLines(compressionListFileName);

            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < dictionary.Length; j++)
                {
                    int dteKey = dteStartNumber + j;
                    string dteBracketCode = "[" + dteKey.ToString("X2") + "]";
                    lines[i] = lines[i].Replace(dictionary[j], dteBracketCode);
                }
            }

            return lines;
        }

        //=================================================================================================

        static void WritePlaceNames(string newRomName, string filename1, string filename2, TableReader table, int pointerStart, int textDataStart)
        {
            string[] lines = LoadTextAsset_AllLines(filename1, filename2); //System.IO.File.ReadAllLines(filename);
            FileStream output = new FileStream(newRomName, FileMode.Open);

            int textDataLoc = textDataStart;
            int pointerLoc = pointerStart;
            int totalDataSize = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i] + "[00]";
                int convertedLength = table.WriteLine(lines[i], output, textDataLoc);
                totalDataSize += convertedLength;

                pointerLoc = pointerStart + i * 2;
                output.Seek(pointerLoc, SeekOrigin.Begin);
                output.WriteByte((byte)(textDataLoc & 0xFF));
                output.WriteByte((byte)((textDataLoc >> 8) & 0xFF));

                textDataLoc += convertedLength;
            }

            output.Close();

            // Now let's check to make sure no names are too long
            int totalProblems = 0;

            FileStream newRomFile = new FileStream(newRomName, FileMode.Open);

            for (int i = 0; i < lines.Length; i++)
            {
                pointerLoc = pointerStart + i * 2;
                newRomFile.Seek(pointerLoc, SeekOrigin.Begin);
                int offset = (byte)(newRomFile.ReadByte() & 0xFF) | (byte)((newRomFile.ReadByte() << 8) & 0xFF);
                textDataLoc = textDataStart + offset;
                newRomFile.Seek(textDataLoc, SeekOrigin.Begin);

                int isTooLong = ScriptTextHasWidthProblems(newRomFile, textDataLoc, 0xDC, table);
                if (isTooLong != 0)
                {
                    totalProblems++;

                    if (totalProblems < 10)
                    {
                        Console.WriteLine("PLACE NAME TOO LONG: " + textDataLoc.ToString("X6") + " " + table.GetLine(newRomFile, isTooLong));
                    }
                }
            }

            newRomFile.Close();
        }

        //=================================================================================================

        static void WriteBattleDialogue(string newRomName, string filename, TableReader table, int pointerStart, int textDataStart, int textDataEnd)
        {
            string textBlob = LoadTextAsset_AllText(filename, ""); //System.IO.File.ReadAllText(filename);
            string[] splitter = { "\n--------------------\n" };
            string[] lines = textBlob.Split(splitter, StringSplitOptions.None);

            FileStream output = new FileStream(newRomName, FileMode.Open);

            int textDataLoc = textDataStart;
            int pointerLoc = pointerStart;
            int totalDataSize = 0;
            int allotedSpace = textDataEnd - textDataStart; // original space, need to manually change this

            lines = PrepBattleText(lines);

            for (int i = 0; i < lines.Length; i++)
            {
                int convertedLength = table.WriteLine(lines[i], output, textDataLoc);
                totalDataSize += convertedLength;

                byte lowByte = (byte)(textDataLoc & 0xFF);
                byte midByte = (byte)((textDataLoc >> 8) & 0xFF);

                //int loc = (textDataStart & 0xFF0000) | lowByte | (midByte << 8);

                pointerLoc = pointerStart + i * 2;
                output.Seek(pointerLoc, SeekOrigin.Begin);
                output.WriteByte(lowByte);
                output.WriteByte(midByte);

                textDataLoc += convertedLength;
            }

            int freeSpace = allotedSpace - totalDataSize;
            string tempType = "Battle Dialogue" + ":";
            Console.WriteLine(tempType.PadRight(20) + totalDataSize.ToString().PadLeft(6) + " bytes / " + allotedSpace.ToString().PadLeft(6) + " bytes   (" + freeSpace.ToString().PadLeft(6) + " bytes free)");
            if (freeSpace < 0)
            {
                Console.WriteLine("Battle Dialogue is " + Math.Abs(freeSpace).ToString() + " bytes too long!!!");
            }

            output.Close();
        }

        //=================================================================================================

        static void WriteGenericBattleText(string newRomName, string filename1, string filename2, int readType, TableReader table, int pointerStart, int textDataStart, int textDataEnd, string textType)
        {
            string[] lines;

            if (readType == 0)
            {
                lines = LoadTextAsset_AllLines(filename1, filename2); //System.IO.File.ReadAllLines(filename);
            }
            else
            {
                string textBlob = LoadTextAsset_AllText(filename1, filename2);
                string[] splitter = { "\n--------------------\n" };
                lines = textBlob.Split(splitter, StringSplitOptions.None);
            }
            
            

            FileStream output = new FileStream(newRomName, FileMode.Open);

            int textDataLoc = textDataStart;
            int pointerLoc = pointerStart;
            int totalDataSize = 0;
            int allotedSpace = textDataEnd - textDataStart; // original space, need to manually change this

            lines = PrepBattleText(lines);

            for (int i = 0; i < lines.Length; i++)
            {
                int convertedLength = table.WriteLine(lines[i], output, textDataLoc);
                totalDataSize += convertedLength;

                byte lowByte = (byte)(textDataLoc & 0xFF);
                byte midByte = (byte)((textDataLoc >> 8) & 0xFF);

                pointerLoc = pointerStart + i * 2;
                output.Seek(pointerLoc, SeekOrigin.Begin);
                output.WriteByte(lowByte);
                output.WriteByte(midByte);

                textDataLoc += convertedLength;
            }

            int freeSpace = allotedSpace - totalDataSize;
            string tempType = textType + ":";
            Console.WriteLine(tempType.PadRight(20) +"" + totalDataSize.ToString().PadLeft(6) + " bytes / " + allotedSpace.ToString().PadLeft(6) + " bytes   (" + freeSpace.ToString().PadLeft(6) + " bytes free)");
            if (freeSpace < 0)
            {
                Console.WriteLine(textType + " is " + Math.Abs(freeSpace).ToString() + " bytes too long!!!");
            }

            output.Close();
        }

        //=================================================================================================

        static void WriteGenericItemText(string newRomName, string textFileName1, string textFilename2, int textDataStart, int maxLength, bool convertSpaces = true)
        {
            string[] lines = LoadTextAsset_AllLines(textFileName1, textFilename2); //System.IO.File.ReadAllLines(textFileName);
            TableReader table = new TableReader("_text/ff6t-item.tbl");

            FileStream output = new FileStream(newRomName, FileMode.Open);

            for (int i = 0; i < lines.Length; i++)
            {
                if (convertSpaces)
                {
                    lines[i] = lines[i].Replace(" ", "[7F]");
                }

                int loc = textDataStart + maxLength * i;
                table.WriteLine(lines[i], output, loc, maxLength, 0xFF);
            }

            output.Close();
        }

        static void WriteItemArticleData(string newRomName, string filename1, string filename2, int textDataStart)
        {
            string[] lines = LoadTextAsset_AllLines(filename1, filename2);  //System.IO.File.ReadAllLines(filename);

            FileStream output = new FileStream(newRomName, FileMode.Open);
            output.Seek(textDataStart, SeekOrigin.Begin);

            for (int i = 0; i < lines.Length; i++)
            {
                byte articleValue = (byte)Int32.Parse(lines[i]);

                output.WriteByte(articleValue);
            }

            output.Close();
        }

    

        //=================================================================================================
    
        static void WriteScript(string newRomName, string[] lines, TableReader table, int pointerStart, int textDataStart1, int textDataEnd1, int textDataStart2, int textDataEnd2, string scriptType, bool compressText, bool checkTextForIssues)
        {
            int textDataLoc = textDataStart1;
            int pointerLoc = pointerStart;
            int bankCount = 1;
            int totalScriptDataSize = 0;
            int allotedSpace = (textDataEnd1 - textDataStart1) + (textDataEnd2 - textDataStart2);

            int totalWidthProblems = 0;
            int totalUnsupportedCharProblems = 0;
            int totalNewpageProblems = 0;

            //----------------------------------------------------------------------------------
            // Here, we create a temporary file that we write each uncompressed line of text to,
            // then load that text back and look for issues if requested
            //----------------------------------------------------------------------------------


            FileStream testOutput = new FileStream("_script-check.tmp", FileMode.Create);
            for (int i = 0; i < lines.Length; i++)
            {
                // write the line as byte data into the temporary test ROM now
                // we'll just write it at the start of the file each time since this is a temporary file
                
                if (checkTextForIssues)
                {
                    table.WriteLine(lines[i], testOutput, 0);

                    int scriptTextProblemLoc = ScriptTextHasWidthProblems(testOutput, 0, 0xDC, table);
                    if (scriptTextProblemLoc != 0)
                    {
                        totalWidthProblems++;

                        if (totalWidthProblems < 10)
                        {
                            Console.WriteLine("WIDTH ISSUE @ Line " + i.ToString() + "\n" + lines[i].Substring(scriptTextProblemLoc) + "\n------\n");
                        }
                    }

                    if (ScriptTextHasUnsupportedCharacters(testOutput, 0, table) != -1)
                    {
                        totalUnsupportedCharProblems++;
                        if (totalUnsupportedCharProblems < 10)
                        {
                            Console.WriteLine("UNSUPPORTED CHAR @ Line " + i.ToString() + "\n" + lines[i] + "\n------\n");
                        }
                    }

                    if (ScriptTextHasTooManyLinesPerTextWindow(testOutput, 0, table) != -1)
                    {
                        totalNewpageProblems++;
                        if (totalNewpageProblems < 10)
                        {
                            Console.WriteLine("NEWPAGE ISSUE @ Line " + i.ToString() + "\n" + lines[i] + "\n------\n");
                        }
                    }
                }

                if (lines[i].Contains("[14][00]"))
                {
                    table.WriteLine(lines[i], testOutput, 0);
                    lines[i] = ProcessCenterCodes(testOutput, lines[i], table, 0xDC);
                    lines[i] = PrepScriptLine(lines[i]) + "[00]";
                }
            }
            testOutput.Close();

    
            //----------------------------------------------------------------------------------
            // Compress the text in preparation of ROM insertion if requested
            //----------------------------------------------------------------------------------
            if (compressText)
            {
                lines = CompressScript(lines, "_text/script-compression-dictionary.txt", 0x80);
            }


            //----------------------------------------------------------------------------------
            // Write each line to the actual ROM now
            //----------------------------------------------------------------------------------
            FileStream output = new FileStream(newRomName, FileMode.Open);

            for (int i = 0; i < lines.Length; i++)
            {
                // write the line as byte data into the ROM now
                int convertedLength = table.WriteLine(lines[i], output, textDataLoc);
                totalScriptDataSize += convertedLength;
                
                // write the pointer to the current line
                // note that if we want to use an area not in ExHiROM space then we gotta do the ordinary address conversion
                // for now we'll ignore that, might need to look into it later
                // also for now we're not caring about smart placement of the text, it'll just bowl over everything in its way
                pointerLoc = pointerStart + i * 3;

                byte lowDigit = (byte)(textDataLoc & 0xFF);
                byte midDigit = (byte)((textDataLoc >> 8) & 0xFF);
                byte highDigit = (byte)((textDataLoc >> 16) & 0xFF);
                if (highDigit < 0x30)
                {
                    highDigit += 0xC0;
                }

                output.Seek(pointerLoc, SeekOrigin.Begin);
                output.WriteByte(lowDigit);
                output.WriteByte(midDigit);
                output.WriteByte(highDigit);
                
                textDataLoc += convertedLength;

                // if we've run out of space in the primary text data bank, switch to the secondary
                if (bankCount == 1 && textDataLoc >= textDataEnd1)
                {
                    textDataLoc = textDataStart2;
                    bankCount++;
                }
            }

            int freeSpace = allotedSpace - totalScriptDataSize;
            string tempType = scriptType + ":";
            Console.WriteLine(tempType.PadRight(20) + "" + totalScriptDataSize.ToString().PadLeft(6) + " bytes / " + allotedSpace.ToString().PadLeft(6) + " bytes   (" + freeSpace.ToString().PadLeft(6) + " bytes free)");


            if (totalWidthProblems > 0)
            {
                Console.WriteLine("\n\nTOTAL WIDTH ISSUES IN " + scriptType + ": " + totalWidthProblems.ToString());
            }

            if (totalUnsupportedCharProblems > 0)
            {
                Console.WriteLine("\n\nTOTAL UNSUPPORTED CHAR ISSUES IN " + scriptType + ": " + totalUnsupportedCharProblems.ToString());
            }

            if (totalNewpageProblems > 0)
            {
                Console.WriteLine("\n\nTOTAL NEWPAGE ISSUES IN " + scriptType + ": " + totalNewpageProblems.ToString());
            }

            if (freeSpace < 0)
            {
                Console.WriteLine(scriptType + " is " + Math.Abs(freeSpace).ToString() + " bytes too long!!!");
            }

            //Console.ReadKey();

            output.Close();
        }

        //=================================================================================================

        static string ProcessCenterCodes(FileStream newRomFile, string line, TableReader table, int maxLineWidth)
        {
            string retVal = line;

            newRomFile.Seek(0, SeekOrigin.Begin);
            int ch = newRomFile.ReadByte();
            while (ch != 0x00) // stop on END
            {
                if (ch == 0x11)
                {
                    // [PAUSE_XX]
                    newRomFile.ReadByte();
                }
                else if (ch == 0x14)
                {
                    // [SPACE_XX]
                    int spaceCount = newRomFile.ReadByte();
                    if (spaceCount == 0x00)
                    {
                        int currLoc = (int)newRomFile.Position;

                        int lineWidth = GetWidthOfSingleSubLine(newRomFile, currLoc, table);
                        int widthDiff = (maxLineWidth - lineWidth) / 2;
                        int paddingAmount = widthDiff - 1;

                        if (paddingAmount <= 0)
                        {
                            paddingAmount = 1;
                        }

                        newRomFile.Seek(currLoc - 1, SeekOrigin.Begin);
                        newRomFile.WriteByte((byte)paddingAmount);
                    }
                }
                else if (ch == 0x16)
                {
                    // [WAIT_XX], not sure if this takes up any text space or not
                    newRomFile.ReadByte();
                }


                ch = newRomFile.ReadByte();
            }

            retVal = table.GetLine(newRomFile, 0);

            return retVal;
        }



        //=================================================================================================

        static int GetWidthOfSingleSubLine(FileStream newRomFile, int subLineStart, TableReader table)
        {
            int totalWidth = 0;

            // first, load the width data from our custom internal width table
            FileStream widthFile = new FileStream("_graphics/graphics-english-font-width-table.bin", FileMode.Open);
            byte[] widths = new byte[256];
            for (int i = 0; i < widths.Length; i++)
            {
                widths[i] = (byte)widthFile.ReadByte();
            }
            widthFile.Close();

            for (int i = 0x02; i < 0x10; i++)
            {
                widths[i] = 6 * 0x0C;
            }

            for (int i = 0xF0; i < 0xFE; i++)
            {
                widths[i] = 6 * 0x0C;
            }
            

            // now we parse the designated line of text data and see if anything goes above maxLength
            newRomFile.Seek(subLineStart, SeekOrigin.Begin);
            int ch = newRomFile.ReadByte();
            while (ch != 0x00 && ch != 0x01 && ch != 0x13) // stop on END, NEWLINE, or NEWPAGE
            {
                if (ch == 0x10)
                {
                    // [LONGPAUSE]
                }
                else if (ch == 0x11)
                {
                    // [PAUSE_XX]
                    newRomFile.ReadByte();
                }
                else if (ch == 0x12)
                {
                    // [SHORTPAUSE]
                }
                else if (ch == 0x14)
                {
                    // [SPACE_XX]
                    int spaceCount = newRomFile.ReadByte();
                    totalWidth += widths[0x75] * spaceCount;
                }
                else if (ch == 0x15)
                {
                    // [CHOICE], not sure if this takes up any text space or not
                }
                else if (ch == 0x16)
                {
                    // [WAIT_XX], not sure if this takes up any text space or not
                    newRomFile.ReadByte();
                }
                else if (ch == 0x19)
                {
                    // [GP], not sure if this takes up any text space or not
                }
                else if (ch == 0x1A)
                {
                    // [ITEM], not sure if this takes up any text space or not
                }
                else
                {
                    int charWidth = widths[ch];
                    totalWidth += charWidth;
                }

                ch = newRomFile.ReadByte();
            }

            //newRomFile.Close();

            return totalWidth;
        }

        //=================================================================================================

        static void CheckScriptForProblems(string newRomName, string[] lines, int pointerTableStart, int tooLongWidth, string textType, TableReader table)
        {
            int totalWidthProblems = 0;
            int totalUnsupportedCharProblems = 0;
            TableReader tempTable = table;

            FileStream newRomFile = new FileStream(newRomName, FileMode.Open);

            for (int i = 0; i < lines.Length; i++)
            {
                newRomFile.Seek(pointerTableStart + i * 3, SeekOrigin.Begin);
                int textLoc = newRomFile.ReadByte() | (newRomFile.ReadByte() << 8) | (newRomFile.ReadByte() << 16);
                if (textLoc > 0x600000)
                {
                    textLoc -= 0xC00000;
                }

                int isTooLong = ScriptTextHasWidthProblems(newRomFile, textLoc, tooLongWidth, table);
                if (isTooLong != 0)
                {
                    totalWidthProblems++;

                    if (totalWidthProblems < 10)
                    {
                        Console.WriteLine("WIDTH ISSUE @ Line " + i.ToString() + "\n" + table.GetLine(newRomFile, isTooLong) + "\n------\n");
                    }
                }

                int hasUnsupportedCharacters = ScriptTextHasUnsupportedCharacters(newRomFile, textLoc, table);
                if (hasUnsupportedCharacters != -1)
                {
                    totalUnsupportedCharProblems++;
                    if (totalUnsupportedCharProblems < 10)
                    {
                        Console.WriteLine("UNSUPPORTED CHAR @ Line " + i.ToString() + "\n" + table.GetLine(newRomFile, textLoc) + "\n------\n");
                    }
                }
            }

            if (totalWidthProblems > 0)
            {
                Console.WriteLine("\n\nTOTAL WIDTH ISSUES IN " + textType + ": " + totalWidthProblems.ToString());
            }

            if (totalUnsupportedCharProblems > 0)
            {
                Console.WriteLine("\n\nTOTAL UNSUPPORTED CHAR ISSUES IN " + textType + ": " + totalUnsupportedCharProblems.ToString());
            }





            newRomFile.Close();
        }

        //=================================================================================================

        static int ScriptTextHasWidthProblems(FileStream newRomFile, int textDataStart, int tooLongWidth, TableReader table)
        {
            // first, load the width data from our custom internal width table
            FileStream widthFile = new FileStream("_graphics/graphics-english-font-width-table.bin", FileMode.Open);
            byte[] widths = new byte[256];
            for (int i = 0; i < widths.Length; i++)
            {
                widths[i] = (byte)widthFile.ReadByte();
            }
            widthFile.Close();

            for (int i = 0x02; i < 0x10; i++)
            {
                widths[i] = 6 * 0x0C;
            }

            for (int i = 0xF0; i < 0xFE; i++)
            {
                widths[i] = 6 * 0x0C;
            }
            

            // -----------------------------

            // now we parse the designated line of text data and see if anything goes above maxLength
            newRomFile.Seek(textDataStart, SeekOrigin.Begin);
            int ch = newRomFile.ReadByte();
            int totalWidth = 0;
            int problemSpot = 0;
            while (ch != 0x00)
            {
                if (ch == 0x01 || ch == 0x13)
                {
                    // 0x01 is [NEWLINE], 0x13 is [NEWPAGE]
                    totalWidth = 0;
                }
                else if (ch == 0x10)
                {
                    // [LONGPAUSE]
                }
                else if (ch == 0x11)
                {
                    // [PAUSE_XX]
                    newRomFile.ReadByte();
                }
                else if (ch == 0x12)
                {
                    // [SHORTPAUSE]
                }
                else if (ch == 0x14)
                {
                    // [SPACE_XX]
                    int spaceCount = newRomFile.ReadByte();
                    totalWidth += widths[0x75] * spaceCount;
                }
                else if (ch == 0x15)
                {
                    // [CHOICE], not sure if this takes up any text space or not
                }
                else if (ch == 0x16)
                {
                    // [WAIT_XX], not sure if this takes up any text space or not
                    newRomFile.ReadByte();
                }
                else if (ch == 0x19)
                {
                    // [GP], not sure if this takes up any text space or not
                }
                else if (ch == 0x1A)
                {
                    // [ITEM], not sure if this takes up any text space or not
                }
                else
                {
                    int charWidth = widths[ch];
                    totalWidth += charWidth;
                }

                // now we do our check to see if we've exceeded the max width at any point during this line of text
                if (totalWidth >= tooLongWidth)
                {
                    problemSpot = (int)newRomFile.Position;
                    break;
                }

                ch = newRomFile.ReadByte();
            }

            return problemSpot; // returns 0 if no issues
        }

        //=================================================================================================

        static int ScriptTextHasUnsupportedCharacters(FileStream newRomFile, int textDataStart, TableReader table)
        {
            // now we parse the designated line of text data and see if anything goes above maxLength
            string line = table.GetLine(newRomFile, textDataStart);
            int problemSpot = -1;

            for (int i = 0; i < line.Length; i++)
            {
                if (line.Contains('#'))
                {
                    problemSpot = i;
                }
            }

            return problemSpot; // returns -1 if no issues
        }

        //=================================================================================================

        static int ScriptTextHasTooManyLinesPerTextWindow(FileStream newRomFile, int textDataStart, TableReader table, int maxLinesPerWindow = 4)
        {
            // we want to find any blocks of text that try to display over 4 lines per text window
            // now we parse the designated line of text data and see if anything goes above maxLength
            string line = table.GetLine(newRomFile, textDataStart);
            line = line.Replace("\n[NEWPAGE]", "$");
            line = line.Replace("[NEWPAGE]", "$");

            char[] lineChars = line.ToCharArray();

            int problemSpot = -1;
            int currNewlineTotal = 0;

            for (int i = 0; i < lineChars.Length; i++)
            {
                char ch = lineChars[i];

                if (ch == '\n')
                {
                    currNewlineTotal++;

                    if (currNewlineTotal > maxLinesPerWindow)
                    {
                        problemSpot = i;
                    }
                }
                else if (ch == '$')
                {
                    currNewlineTotal = 0;
                }
            }

            return problemSpot; // returns -1 if no issues
        }

        //=================================================================================================

        static void CheckBattleDialogueForWidthProblems(string newRomName, int pointerTableStart, int textDataStart, int totalLines, int tooLongWidth, string textType, TableReader table)
        {
            int totalProblems = 0;


            FileStream newRomFile = new FileStream(newRomName, FileMode.Open);

            for (int i = 0; i < totalLines; i++)
            {
                newRomFile.Seek(pointerTableStart + i * 2, SeekOrigin.Begin);
                int offset = newRomFile.ReadByte() | (newRomFile.ReadByte() << 8);
                int textLoc = (textDataStart & 0xFF0000) + offset;

                int isTooLong = BattleDialogueTextHasWidthProblems(newRomFile, textLoc, tooLongWidth, table);
                if (isTooLong != 0)
                {
                    totalProblems++;
                }
            }

            if (totalProblems > 0)
            {
                Console.WriteLine("\n\nTOTAL WIDTH ISSUES IN " + textType + ": " + totalProblems.ToString());
            }

            newRomFile.Close();
        }

        //=================================================================================================

        static void CheckBattleTextForWidthProblems(string newRomName, int textDataStart, int singleLength, int totalLines, int tooLongWidth, string textType, TableReader table, int maxCharLen = -1)
        {
            int totalProblems = 0;


            FileStream newRomFile = new FileStream(newRomName, FileMode.Open);

            for (int i = 0; i < totalLines; i++)
            {
                int textLoc = textDataStart + i * singleLength;
                int isTooLong = BattleDialogueTextHasWidthProblems(newRomFile, textLoc, tooLongWidth, table, maxCharLen);
                if (isTooLong != 0)
                {
                    totalProblems++;
                }
            }

            if (totalProblems > 0)
            {
                Console.WriteLine("\n\nTOTAL WIDTH ISSUES IN " + textType + ": " + totalProblems.ToString());
            }

            newRomFile.Close();
        }

        //=================================================================================================

        static void CheckBattleTopTextForWidthProblems(string newRomName, int pointerTableStart, int textDataStart, int totalLines, int tooLongWidth, string textType, TableReader table, int maxCharLen = -1)
        {
            int totalProblems = 0;

            FileStream newRomFile = new FileStream(newRomName, FileMode.Open);

            for (int i = 0; i < totalLines; i++)
            {
                newRomFile.Seek(pointerTableStart + i * 2, SeekOrigin.Begin);
                int offset = newRomFile.ReadByte() | (newRomFile.ReadByte() << 8);
                int textLoc = (textDataStart & 0xFF0000) + offset;

                int isTooLong = BattleTopTextHasWidthProblems(newRomFile, textLoc, tooLongWidth, i, table, maxCharLen);
                if (isTooLong != 0)
                {
                    totalProblems++;
                    Console.WriteLine("TOP BATTLE TEXT LINE #" + i.ToString() + "\n" + table.GetLine(newRomFile, textLoc));
                }
            }

            if (totalProblems > 0)
            {
                Console.WriteLine("\n\nTOTAL WIDTH ISSUES IN " + textType + ": " + totalProblems.ToString());
            }

            newRomFile.Close();
        }

        //=================================================================================================

        static int BattleDialogueTextHasWidthProblems(FileStream newRomFile, int textDataStart, int tooLongWidth, TableReader table, int maxCharLen = -1)
        {
            // first, load the width data from our custom internal width table
            FileStream widthFile = new FileStream("_graphics/graphics-english-font-width-table.bin", FileMode.Open);
            byte[] widths = new byte[256];
            for (int i = 0; i < widths.Length; i++)
            {
                widths[i] = (byte)widthFile.ReadByte();
            }
            widthFile.Close();

            for (int i = 0x02; i < 0x10; i++)
            {
                widths[i] = 6 * 0x0C;
            }

            for (int i = 0xF0; i < 0xFE; i++)
            {
                widths[i] = 6 * 0x0C;
            }


            // -----------------------------

            // now we parse the designated line of text data and see if anything goes above maxLength
            newRomFile.Seek(textDataStart, SeekOrigin.Begin);
            int ch = newRomFile.ReadByte();
            int totalWidth = 0;
            int problemSpot = 0;
            int charCount = 0;

            while (ch != 0x00 && ch != 0xFF)
            {
                charCount++;

                if (maxCharLen > 0 && charCount > maxCharLen)
                {
                    break;
                }

                if (ch == 0x01)
                {
                    // 0x01 is [NEWLINE]
                    totalWidth = 0;
                }
                else if (ch == 0x02)
                {
                    // [02 XX] is a character's name
                    int param = newRomFile.ReadByte();
                    totalWidth += 6 * 0x0C;
                    charCount++;
                }
                else if (ch == 0x04)
                {
                    // [COLOR]
                }
                else if (ch == 0x05)
                {
                    // [PAUSE]
                }
                else if (ch == 0x06)
                {
                    // [LONGPAUSE]
                }
                else if (ch == 0x07)
                {
                    // [WAIT]
                }
                else if (ch == 0x10 || ch == 0x11 || ch == 0x13 || ch == 0x14)
                {
                    // various variables, just gonna assume it's long
                    totalWidth += 9 * 0x0C;
                }
                else if (ch == 0x12)
                {
                    // other various variable strings
                    int type = newRomFile.ReadByte();
                    charCount++;

                    if (type == 0)
                    {
                        totalWidth += 6 * 0x0C;
                    }
                    else if (type == 1)
                    {
                        totalWidth += 9 * 0x0C;
                    }
                    else if (type == 2)
                    {
                        totalWidth += 9 * 0x0C;
                    }
                    else if (type == 3)
                    {
                        totalWidth += 9 * 0x0C;
                    }
                    else
                    {
                        totalWidth += 1000;
                    }
                }
                else
                {
                    int charWidth = widths[ch];
                    totalWidth += charWidth;
                }

                // now we do our check to see if we've exceeded the max width at any point during this line of text
                if (totalWidth >= tooLongWidth)
                {
                    problemSpot = (int)newRomFile.Position;
                    //break;
                }

                ch = newRomFile.ReadByte();
            }

            if (problemSpot != 0)
            {

                Console.WriteLine("WIDTH ISSUE @ Line \n" + table.GetLine(newRomFile, textDataStart));
                Console.WriteLine("  Current Width: " + totalWidth.ToString() + "/ Max Width:" + tooLongWidth.ToString());
                Console.WriteLine("------\n");
            }

            return problemSpot; // returns 0 if no issues
        }

        //=================================================================================================

        static int BattleTopTextHasWidthProblems(FileStream newRomFile, int textDataStart, int tooLongWidth, int lineNumber, TableReader table, int maxCharLen = -1)
        {
            // first, load the width data from our custom internal width table
            FileStream widthFile = new FileStream("_graphics/graphics-english-font-width-table.bin", FileMode.Open);
            byte[] widths = new byte[256];
            for (int i = 0; i < widths.Length; i++)
            {
                widths[i] = (byte)widthFile.ReadByte();
            }
            widthFile.Close();

            for (int i = 0x02; i < 0x10; i++)
            {
                widths[i] = 6 * 0x0C;
            }

            for (int i = 0xF0; i < 0xFE; i++)
            {
                widths[i] = 6 * 0x0C;
            }


            // -----------------------------

            // now we parse the designated line of text data and see if anything goes above maxLength
            newRomFile.Seek(textDataStart, SeekOrigin.Begin);
            int ch = newRomFile.ReadByte();
            int totalWidth = 0;
            int problemSpot = 0;
            int charCount = 0;

            while (ch != 0x00 && ch != 0xFF)
            {
                charCount++;

                if (maxCharLen > 0 && charCount > maxCharLen)
                {
                    break;
                }

                if (ch == 0x01)
                {
                    totalWidth = 0;
                }
                else if (ch == 0x02)
                {
                    // [02 XX] is a character's name
                    int param = newRomFile.ReadByte();
                    totalWidth += 6 * 0x0C;
                    charCount++;
                }
                else if (ch == 0x07)
                {
                    // [WAIT]
                }
                else if (ch == 0x10 || ch == 0x11 || ch == 0x13 || ch == 0x14)
                {
                    if (lineNumber == 33)
                    {
                        totalWidth += 1 * 0x0C;
                    }
                    else if (lineNumber == 48 || lineNumber == 49 || lineNumber == 53)
                    {
                        totalWidth += 5 * 0x0C;
                    }
                    else
                    {
                        totalWidth += 9 * 0x0C;
                    }
                }
                else if (ch == 0x12)
                {
                    // other various variable strings
                    int type = newRomFile.ReadByte();
                    charCount++;

                    if (type == 0)
                    {
                        totalWidth += 6 * 0x0C;
                    }
                    else if (type == 1)
                    {
                        totalWidth += 9 * 0x0C;
                    }
                    else if (type == 2)
                    {
                        totalWidth += 5 * 0x0C;
                    }
                    else if (type == 3)
                    {
                        totalWidth += 9 * 0x0C;
                    }
                    else
                    {
                        totalWidth += 1000;
                    }
                }
                else
                {
                    int charWidth = widths[ch];
                    totalWidth += charWidth;
                }

                // now we do our check to see if we've exceeded the max width at any point during this line of text
                if (totalWidth >= tooLongWidth)
                {
                    problemSpot = (int)newRomFile.Position;
                }

                ch = newRomFile.ReadByte();
            }

            if (problemSpot != 0)
            {

                Console.WriteLine("WIDTH ISSUE @ Line \n" + table.GetLine(newRomFile, textDataStart));
                Console.WriteLine("  Current Width: " + totalWidth.ToString() + "/ Max Width:" + tooLongWidth.ToString());
                Console.WriteLine("------\n");
            }

            return problemSpot; // returns 0 if no issues
        }

        //=================================================================================================

        static int[] AlphabetizeRageList(string rageFileName)
        {
            string[] enemyNames = LoadTextAsset_AllLines(rageFileName);
            string[] rages = new string[256];
            int[] orderedRageIDs = new int[256];

            for (int i = 0; i < 256; i++)
            {
                rages[i] = enemyNames[i].ToLower() + "|" + i.ToString();
            }

            Array.Sort(rages);

            int rageCounter = 0;
            for (int i = 0; i < rages.Length; i++)
            {
                string[] pieces = rages[i].Split(new Char[] { '|' });
                int currRageID = Int32.Parse(pieces[1]);
                
                // we need to skip the "Tonberries" entry because it's supposed to be 0xFF and is intended to be the last entry in the menus
                if (currRageID != 0xFF)
                {
                    orderedRageIDs[rageCounter++] = currRageID;
                }
            }

            orderedRageIDs[0xFF] = 0xFF; // accounting for the final "Tonberries" entry, the Rage order table MUST end with 0xFF

            return orderedRageIDs;
        }

        //=================================================================================================

        static void ReorganizeRageLists(string newRomName, string enemyFileName, int rageOrderTableLoc)
        {
            int[] rageIDsAlphabetized = AlphabetizeRageList(enemyFileName);

            FileStream output = new FileStream(newRomName, FileMode.Open);
            output.Seek(rageOrderTableLoc, SeekOrigin.Begin);
            for (int i = 0; i < rageIDsAlphabetized.Length; i++)
            {
                output.WriteByte((byte)rageIDsAlphabetized[i]);
            }

            output.Close();
        }

    }

    //========================================================================================/
    //========================================================================================/

    public class TableReader
    {
        Hashtable htoeEntries;
        Hashtable etohEntries;
        string tableFilename;

        public TableReader()
        {
        }

        public TableReader(string filename)
        {
            LoadTextTable(filename);
        }

        //=================================================================================================

        public void LoadTextTable(string filename)
        {
            tableFilename = filename;
            string[] tableLines = System.IO.File.ReadAllLines(tableFilename);
            string[] pieces;

            htoeEntries = new Hashtable();
            etohEntries = new Hashtable();
            foreach (string str in tableLines)
            {
                pieces = str.Split(new Char[] { '=' });

                htoeEntries[pieces[0]] = pieces[1];
                etohEntries[pieces[1]] = pieces[0];
            }
        }

        //=================================================================================================

        public string HexToEntry(int hex)
        {
            string temp = String.Format("{0:X}", hex);
            if (hex < 0x10)
            {
                temp = hex.ToString("X2");
            }

            // this is a custom thing that is only meant for Funky Fantasy IV!
            // i don't remember if i already edited this for ffvi t edition or not oops
            if (hex == 0x7A)
            {
                return "'";
            }

            if (htoeEntries[temp] != null)
                return (string)htoeEntries[temp];
            else
                return null;
        }

        //=================================================================================================

        public int EntryToHex(string entry)
        {
            string temp = (string)etohEntries[entry];
            if (etohEntries[entry] != null)
            {
                return Int32.Parse((string)etohEntries[entry], System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                return 0x69; // return a garbage sign if the input doesn't exist in the table
            }
        }

        //=================================================================================================

        public int WriteLine(string str, FileStream romFile, long loc, int maxLength = -1, byte fillerByte = 0xFF)
        {
            byte[] bytes = StringToHex(str, true);

            if (bytes == null)
            {
                bytes = new byte[] { fillerByte };
            }

            if (maxLength != -1 && bytes.Length > 1 && bytes[bytes.Length - 1] == 0x00)
            {
                bytes[bytes.Length - 1] = fillerByte;
            }

            int bytesToWrite = maxLength;
            if (bytesToWrite == -1 || maxLength > bytes.Count())
            {
                bytesToWrite = bytes.Count();
            }

            int i = 0;
            romFile.Seek(loc, SeekOrigin.Begin);
            for (i = 0; i < bytesToWrite; i++)
            {
                romFile.WriteByte(bytes[i]);
            }

            int bytesLeftToWrite = 0;
            if (maxLength != -1)
            {
                bytesLeftToWrite = maxLength - i;
                for (int j = 0; j < bytesLeftToWrite; j++)
                {
                    romFile.WriteByte(fillerByte);
                }
            }

            return (bytesToWrite + bytesLeftToWrite);
        }

        //=================================================================================================

        public byte[] StringToHex(string str, bool parseBrackets = false)
        {
            ArrayList buffer = new ArrayList();
            byte[] retVal = null;
            int j = 0;
            int i = 0;
            int bracketedByte = -1;

            while (i < str.Length)
            {
                bracketedByte = -1;

                if (parseBrackets)
                {
                    if (str[i] == '[' && (i + 1 < str.Length))
                    {
                        int offset = str.IndexOf("]", i);

                        if (offset != -1)
                        {
                            string subStr = str.Substring(i + 1, offset - i - 1).Trim();

                            try
                            {
                                bracketedByte = Convert.ToInt32(subStr, 16);
                            }
                            catch (Exception e)
                            {
                                bracketedByte = 0x69; // this is a garbage indicating character, possibly remove this check later!
                            }

                            buffer.Add((byte)bracketedByte);
                            i = offset;
                        }
                    }
                }

                if (bracketedByte == -1)
                {
                    j = EntryToHex(str[i].ToString());
                    if (j != -1)
                        buffer.Add((byte)j);
                }

                i++;
            }

            if (buffer.Count > 0)
            {
                //if ((byte)buffer[buffer.Count - 1] == 0x00) // we were getting double EOL 0x00s before, so let's check for a 0x00 first
                //{
                    byte temp = 0;
                    retVal = new byte[buffer.Count];
                    for (i = 0; i < buffer.Count; i++)
                    {
                        temp = (byte)buffer[i];
                        retVal[i] = temp;
                    }
                /*}
                else // if there isn't an end 0x00 then let's add one
                {
                    byte temp = 0;
                    retVal = new byte[buffer.Count + 1];
                    for (i = 0; i < buffer.Count; i++)
                    {
                        temp = (byte)buffer[i];
                        retVal[i] = temp;
                    }

                    retVal[i] = 0x00;
                }*/
            }

            return retVal;
        }

        public virtual string GetLine(FileStream f, int loc, int maxLength = 5000, bool isBattle = false, bool alternateCodes = false, bool useCompression = false)
        {
            string str = "";
            int ch = 0;
            int count = 0;
            int charCount = 0;
            int lastCh = 0;

            bool showAllCodes = true;
            bool convertToHTML = false;

            string[] dictionary = System.IO.File.ReadAllLines("_text/script-compression-dictionary.txt");

            f.Seek(loc, SeekOrigin.Begin);
            ch = f.ReadByte();
            count++;
            while (ch != 0x00 && count <= maxLength)
            {
                if (ch == 0x00) // end of line
                {
                    break;
                }
                else if (ch == 0x01) // newline
                {
                    if (showAllCodes)
                    {
                        str += (convertToHTML) ? "<br />" : "\n";
                    }
                    else
                    {
                        str += (convertToHTML) ? "<br />" : "\n";
                    }
                }
                else if (ch == 0x10)
                {
                    if (showAllCodes)
                    {
                        str += "[LONGPAUSE]";
                    }
                }
                else if (ch == 0x12) // pause codes, 0x10 pauses for 60 frames, 0x12 pauses for 1, 0x11 uses a parameter
                {
                    if (showAllCodes)
                    {
                        str += "[SHORTPAUSE]";
                    }
                }
                else if (ch == 0x11)
                {
                    int duration = f.ReadByte(); // *15;
                    count++;
                    str += "[PAUSE_" + duration.ToString("X2") + "]";
                }
                else if (ch == 0x13) // new page
                {
                    if (showAllCodes)
                    {
                        str += (convertToHTML) ? "<hr class=\"newpage\" />" : "\n[NEWPAGE]\n";
                    }
                    else
                    {
                        str += (convertToHTML) ? "<hr class=\"newpage\" />" : "\n\n";
                    }
                }
                else if (ch == 0x14) // blank space padding; parameter byte is the # of spaces to write
                {
                    int param1 = f.ReadByte();
                    count++;

                    str += (convertToHTML) ? "&nbsp;" : "[SPACE_" + param1.ToString("X2") + "]";

                    /*for (int i = 0; i < param1; i++)
                    {
                        if (showAllCodes)
                        {
                            str += (convertToHTML) ? "&nbsp;" : " ";
                        }
                    }*/
                }
                else if (ch == 0x15) // choice given
                {
                    if (showAllCodes)
                    {
                        str += (convertToHTML) ? "<div class=\"choice\"></div>" : "[CHOICE]";
                    }
                }
                else if (ch == 0x16) // wait for key input for XX * 15 frames
                {
                    int duration = f.ReadByte();// *15;
                    count++;

                    if (showAllCodes)
                    {
                        str += "[WAIT_" + duration.ToString("X2") + "]";
                    }
                }
                else if (ch == 0x19) // GP
                {
                    str += (convertToHTML) ? "<div class=\"money\">GP</div>" : "[GP]";
                }
                else if (ch == 0x1A) // Item
                {
                    str += (convertToHTML) ? "<div class=\"item\">Item</div>" : "[ITEM]";
                }
                /*else if (ch == 0x1B) // Skill
                {
                    str += (convertToHTML) ? "<div class=\"skill\">Skill</div>" : "[SKILL]";
                }*/
                /*else if (ch == 0x17 || ch == 0x18 || ch == 0x1B || ch == 0x1C || ch == 0x1D || ch == 0x1E || ch == 0x1F) // kanji code, following byte specifies a certain kanji
                {
                    int param = f.ReadByte();
                    count++;
                    str += HexToEntry((ch << 8) + param);
                }*/
                else if (useCompression && ch >= 0x80 && ch <= 0x80 + dictionary.Length)
                {
                    str += dictionary[ch - 0x80];
                }
                else
                {
                    string s = HexToEntry((int)ch);

                    if (s != null)
                    {
                        str += s;
                    }
                    else
                    {
                        str += "[" + ch.ToString("X2") + "]";
                    }

                    charCount++;
                }

                lastCh = ch;

                ch = f.ReadByte();
                count++;

            }

            if (str == "")
            {
                str = "BLANK LINE: 0x" + loc.ToString("X6");
            }

            //str += "\n--------------------";
            //str += "\n[END]";


            return str.Trim();
        }

    }
}
