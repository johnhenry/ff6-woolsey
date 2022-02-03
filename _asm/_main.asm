arch snes.cpu
ExHiROM

//====================================================================
// MAP OF NEW DATA IN EXPANDED ROM AREA
//
// Note: Addresses are plain hex addreses, not SNES addresses
//       Addresses over 0x300000 actually map 1:1 between both formats
//
// Note: This map doesn't include ASM modifications that overwrite
//       existing code in-place, only new additions
//
// Note: This map applies to the main T-Edition translation patch,
//       the EX patch is mostly the same but be aware that some data
//       might move/have moved from these original locations
//
//====================================================================
//
// 58e000 - 58ee1f	(CODE: Brand-New ASM code)
//
// 5c9ce0 - 5cc5cf	(TEXT: Script Bank 1 pointer table)
// 2b2b60 - 2ce1df	(TEXT: Script Bank 1B)
// 520000 - 53feff	(TEXT: Script Bank 1A)
//
// 51E600 - 51ffff	(TEXT: Script Bank 2 pointer table)
// 57a710 - 58deff	(TEXT: Script Bank 2A)
// 593440 - 5994ff	(TEXT: Script Bank 2B)
//
// 268400 - 2684ff	(TEXT: Place Names pointer table)
// 5FF2A0 - 5fbfff	(TEXT: Place Names)
//
// 10FC00 - 10fdff	(TEXT: Battle Dialogue pointer table)
// 4D4480 - 4d7fff	(TEXT: Battle Dialogue)
//
// 0F3940 - 0F3B3f	(TEXT: Battle Text pointer table)
// 599530 - 59A4ff	(TEXT: Battle Text)
//
// 0FE000 - 0FE1ff	(TEXT: Enemy Text pointer table)
// 552B60 - 556fff	(TEXT: Enemy Text)
//
// 59BD60 - 59d15f	(TEXT: Enemy Names [short])
// 55d650 - 55fe4f	(TEXT: Enemy Names [long])
//
// 58F000 - 58f10d	(TEXT: Magic Names [short])
// 557aa0 - 557c1f	(TEXT: Magic Names [medium])
// 557890 - 557a3f	(TEXT: Magic Names [long])
// 55c730 - 55c91f	(TEXT: Magic Names [for esper details page])
//
// 58EE20 - 58eeff	(TEXT: Esper Names)
// 55b490 - 55b56f	(TEXT: Esper Names [short])
// 59d980 - 59db2f	(TEXT: Esper Names [long])
//
// 58EF00 - 58efff	(TEXT: Blue Magic Names [short])
// 55ce40 - 55cfbf	(TEXT: Blue Magic Names [long])
//
// 55a990 - 55aa5f	(TEXT: Bushido Names)
//
// 558d70 - 558dbf	(TEXT: Dance Names [short])
// 558cb0 - 558d0f	(TEXT: Dance Names [medium])
// 558e20 - 558ebf	(TEXT: Dance Names [long])
//
// 5591A0 - 55927f	(TEXT: Battle Menu Action/Command Names)
//
// 0478c0 - 047a3f	(TEXT: Character Names)
//
// 0efba0 - 0efc8f	(TEXT: Rare Item Names)
//
// 02ade1 - 02aebf	(TEXT: Battle Status Effect Names)
//
// 54d730 - 54ffcf	(TEXT: Enemy Move Names)
//
// 557C80 - 558c4f	(TEXT: Special Move Names)
//
// 558F20 - 55913f	(TEXT: Esper Move/Attack Names)
//
// 0efb60 - 0efb9b	(TEXT: Rare Item Descriptions pointer table)
// 5592e0 - 559bff	(TEXT: Rare Item Descriptions)
//
// 041000 - 04122f	(TEXT: Item Descriptions pointer table)
// 5CC600 - 5CfFCF	(TEXT: Item Descriptions)
//
// 0ffe40 - 0ffe75	(TEXT: Esper Descriptions pointer table)
// 559C60 - 55a1ff	(TEXT: Esper Descriptions)
//
// 26f690 - 26f6fc	(TEXT: Magic Descriptions pointer table)
// 18c9a0 - 18cfff	(TEXT: Magic Descriptions)
//
// 0fffae - 0fffbd	(TEXT: Bushido Descriptions pointer table)
// 55aac0 - 55af5f	(TEXT: Bushido Descriptions)
//
// 400e00 - 400fff	(TEXT: Rage Descriptions pointer table)
// 401000 - 402fdf	(TEXT: Rage Descriptions)
//
// 0fff9e - 0fffad	(TEXT: Blitz Descriptions pointer table)
// 55d020 - 55d1cf	(TEXT: Blitz Descriptions)
//
// 0fff3e - 0fff6d	(TEXT: Blue Magic Descriptions pointer table)
// 55d230 - 55d5ef	(TEXT: Blue Magic Descriptions)
//
// 185000 - 1859d7	(TEXT: Item Names [short])
// 59A560 - 59bb3f	(TEXT: Item Names [long])
// 59db90 - 59e9cf	(TEXT: Item Names [medium])
// 59BBa0 - 59bcff	(TEXT: Item Name Article Assignment Table)
//
// 55afc0 - 55b42f	(TEXT: Job Names [short])
// 59d1c0 - 59d91f	(TEXT: Job Names [long])
//
// 590000 - 5907ff	(MENU: Main Menu string pointer table)
// 590860 - 592fff	(MENU: Main Menu string + position structs)
//
// 0e1000 - 0e12ff	(MENU: Music Player Song Title string pointer table)
// 04A830 - 04Bbbf	(MENU: Music Player Song Title strings)
//
// 55b5d0 - 55c6cf	(MENU: Main Menu Misc Strings)
//
// 048fc0 - 0490bf	(GRAPHICS: Main Font width table)
// 0494c0 - 04A27f	(GRAPHICS: Main Font glyphs)
//
// 0481c0 - 0486ff	(GRAPHICS: Small Font glyphs)
//
// 04bc20 - 04be1f	(GRAPHICS: Final Credits Font glyphs)
//
// 296300 - 2968ff	(GRAPHICS: "THE END" graphic)
//====================================================================



//====================================================================
// INSERT NEW FONTS
//====================================================================

// this contains the font glyph width table, taken mostly from FF3us
org $c48fc0; incbin _graphics/graphics-english-font-width-table.bin

// this contains the actual font glyph data, taken mostly from FF3us
org $c494c0; incbin _graphics/graphics-english-font.bin

// modify the smaller menu font to work with English FF3us glyphs
org $c481c0; incbin _graphics/graphics-english-small-font.bin
org $c487b0; fill $10,$00  // clear char 0x7F to make it a space char

// change size of arrow character used in main script choice selection
org $c08767
db $00,$10,$18,$1c,$1e,$1c,$18,$10,$00,$00,$00,$00

// insert ending credits small font because our font changes above
// mess with the existing ending credits font system
org $c4bbf0; db "****************S.CREDIT FONT ST****************"
org $C4be20; db "****************S.CREDT FONT END****************"
org $c4bc20; incbin _graphics/graphics-credits-small-font.bin
org $c3e3e8; ldy #$bc20	// make game load our new small credits font
org $c3e3ed; lda #$c4



//====================================================================
// INSERT TRANSLATED "THE END" GRAPHIC
//====================================================================
org $e96300; incbin _graphics/graphics-the-end.bin
org $e5f6a1; lda #$00
org $e5f6be; lda #$00
org $e5f6db; lda #$00



//====================================================================
// INCLUDE EXTERNAL ASM MODIFICATION FILES
//====================================================================

incsrc _asm/new-code.asm
incsrc _asm/main-script.asm
incsrc _asm/battle.asm
incsrc _asm/menus.asm
incsrc _asm/bug-fixes.asm



//====================================================================
// OPTIONAL HACKS FOR EASIER DEBUGGING
//====================================================================

// up up down down fart A B

// Lots of exp points after each battle (cheat codes work better though)
//org $c25da9; lda #$e000

// 255 magic points after each battle
//org $c25d84; lda #$ff; nop; nop
