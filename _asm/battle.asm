//====================================================================
// Load battle dialogue strings from our new location
//====================================================================

org $c197c8; lda #$4d



//====================================================================
// Display battle dialogue with our proper, original font
//====================================================================

org $c161c4; lda $c494c0,x
org $c1613d; lda $c494c0,x
org $c16143; lda $c494c0,x

// Make the battle dialogue & battle text load the correct VWF entries
// For reference, this same welding routine starts at c16126 in FF3us
org $c160ff; jml load_battle_dialogue_text_character_width_PartA
org $c1621c; jsl load_battle_dialogue_text_character_width_PartB; nop; nop; nop



// make battle text numeric digits print properly
org $c15f6b; lda #$54
org $c15f76; cmp #$54
org $c166ef; cmp #$54



//====================================================================
// Make battle text work in English
//====================================================================

org $599500; db "****************BATTLE TEXT BEG.****************"
org $59A500; db "****************BATTLE TEXT END ****************"

// load from new bank since we moved it
org $c198ac; lda #$59



//====================================================================
// Load enemy text (not names!) strings from our new location
//====================================================================

org $c197e6; lda #$55


org $59BD30; db "****************EN. NAMES SHORT ****************"
org $59d160; db "****************EN.NAMES SHT END****************"
org $c166ef; nop; nop; nop; nop
org $c169d5; lda $59bd60,x

org $c169ae; lda #$0A   // expand enemy names from 8 to 10 chars, will need to delete the enemy count display though
// we're relocating short enemy names + expanding from 8 to 10 chars, so need new code
org $c169ce
jsl load_enemy_names_for_battle_window

// this appears to be the menu layout template for enemy names displayed in battle
//!!! figure out layout code
// alter window template layout to remove enemy count from enemy window in battle
org $c2e177; db $0b,$00,$ff,$ff,$01,$0b,$01,$ff,$ff,$01,$0b,$02,$ff,$ff,$01,$0b,$03,$ff,$ff,$00


// make our new short names appear in Rage selection menu
org $c16639; lda #$0a
org $c16644; lda $59BD60,x

org $c2e016; db $04	// move all rage names left 2 tiles
org $c1821e; db $10 // move left column cursor left 2 tiles
org $c16634; lda #$0a // tell game to clear more tiles when erasing names


//==============================================
// let battle top bar use longer item names?


org $59A530; db "****************LONG ITEMS BEGIN****************"
org $59BB40; db "****************LONG ITEMS END  ****************"

org $c16008; lda #$14








org $54d700; db "****************ENEMY MOVES BEGN****************"
org $54FFD0; db "****************ENEMY MOVES END ****************"

org $c196ee; lda #$54
org $c196fd; adc #$d730
org $c196f9; jsl get_enemy_move_string_location
org $c19710; cpy #$0014

org $c1983b; lda #$6c    // center battle top msg box slightly text better
// note that anything longer than 0x6b in length will get wonky centering
//not sure how to fix!!!


//-------------------------------------------------------------------------
// let expanded magic spell names be used in the top battle msg box
//-------------------------------------------------------------------------

org $557860; db "****************LONG MAGIC NAMES***************"
org $557a40; db "****************LONG MNAMES END****************"
org $c15fd0; lda #$08
org $c15fe7; lda $557890,x
org $c15fe4; nop; nop; nop;




//-------------------------------------------------------------------------
// let expanded special attack name/char-specific attack names
// be used in the top battle msg box
//-------------------------------------------------------------------------

org $557c50; db "****************LONG ATTK NAMES ****************"
org $558c50; db "****************LONG ATTKS END  ****************"
org $c26473; lda #$14
org $c2647f; lda $557C80,x
org $c2648c; cpy #$0014



org $558d40; db "****************S DANCE NAME BEG****************"
org $558Dc0; db "****************S DANCE NAME END****************"
org $c1660c; lda #$0c
org $c16611; lda #$0c
org $c1661c; lda $558cb0,x

// move dance names left 2 tiles in battle
org $c2e009; db $04  
org $c2e00f; db $02

// not sure if long dance names will ever be needed, but just in case
org $558df0; db "****************L DANCE NAME BEG****************"
org $558ec0; db "****************L DANCE NAME END****************"










org $558ef0; db "****************ESPR ATTK NAMES ****************"
org $559140; db "****************ESPR ATTKS END  ****************"
org $c2baef; lda #$14
org $c2bafb; lda $558F20,x
org $c2bb08; cpy #$0014




org $559170; db "****************BATT ACT. NAMES ****************"
org $559280; db "****************BATT ACTIONS END****************"
org $c16a11; lda #$07
org $c16a22; lda $5591A0,x

org $c15f00; lda #$07
org $c15f12; lda $5591A0,x

// make the battle action menu one tile wider
// moving it left to match FF3us is easy enough but then the enemy name
// erasing code needs to be modified too, which is a pain, so this is good enough
org $c2dd48; db $0A
// c2e20a is start of battle menu layout data

// expand and reorganize the "shortcut" battle menu layout
org $c2dd74; db $12	// widens shortcut battle action window to fit larger action text
//org $c2e1d1; db $16	// removes one space before the righthand battle action
// !!!!! The above fix caused a weird problem in which the second party character's
// final letter in their name won't get erased when the shortcut battle window is
// open. I forgot how these FF6 battle window layout data structures work, so I'm
// just undoing my fix for now. The cursor position for the righthand battle option
// will be off, but I'm hoping someone can figure all this out later. 

// adjust cursor positions for standard battle menu layout
org $c179d7; db $17,$a2,$17,$ae,$17,$ba,$17,$c6
org $c17e6a; lda #$07
org $c17dfc; lda #$37

// adjust cursor positions for battle magic menu
org $c18219; db $00,$37,$6f
org $c18222; db $a6,$b2,$be,$ca
org $c1828f; lda #$37
org $c18294; lda #$aa

// adjust cursor positions for battle item menu
org $c1821c; db $0f,$7f
org $c18dde; lda #$b6

// adjust cursor positions for battle rage menu
org $c1821e; db $0f,$7f

// adjust cursor positions for battle blue magic menu
org $c18220; db $07,$57





// swordtechs in battle need to be redone from scratch
// c2babf is where name loading begins but it uses a different system than the others
org $c2bac1
sta $22
lda #$0C
sta $24
jsl $c118ad
ldx $26
tdc
tay
-
lda $55a990,x
cmp #$ff
beq +
sta $57d5,y
inx
iny
cpy #$000c
bne -

+
tdc
sta $57d5,y
rtl


// make swdtech counter 1-8 thing work with our new font
org $c2E0A2; db $55
org $c2E0A5; db $56
org $c2E0A8; db $57
org $c2E0AB; db $58
org $c2E0AE; db $59
org $c2E0B1; db $5A
org $c2E0B4; db $5B
org $c2E0b7; db $5C







org $55d620; db "****************ENEMY NAMES LONG****************"
org $55Fe50; db "****************EN.NAME LONG END****************"

// Adding name to Scan results
// note that this won't work on enemy 255 (which is Io) but most people probably won't notice
// actually I might be wrong, I think enemy 256 (Tonberries) might be the messed up one
org $c250c5; fill $22,$ea
org $c250c5
jsl prep_name_in_scan_stuff
lda $b6
cmp #$08	// if we're scanning a party member, skip the name entry
bcc +

jsl add_name_to_scan_messages_A
jsr $63f9

+
jsl add_name_to_scan_messages_B

org $c1601f; jml make_item_code_display_enemy_name_in_scan
org $4e054e; adc #$1400	// add proper support for extended >256 items








org $55CE10; db "****************BLUE MAG MED BEG****************"
org $55Cfc0; db "****************BLUE MAG MED END****************"

// move magitek attack names in magitek menu
org $c2dffc; db $04
org $c2e002; db $02

// load expanded blue magic names
org $c164fc; lda #$0c
org $c16507; lda $55ce58,x
org $c164f5; lda #$0c     // erase names properly

org $4564fc; lda #$0c
org $456507; lda $55ce58,x
org $4564f5; lda #$0c

// make blue magic names load properly in the "BLUEMAG learned!" message
org $c15fa9; lda #$14
org $c15fbd; lda $557C80,x


// make it say "MP Cost" in magic sub-menu
// the actual list of characters is stored in ff6t-misc-strings.txt
org $c149e8; db $16,$17,$ff,$18,$19,$1a,$1b	// tilemap for our custom "MP Cost"
org $c149f6; db $02,$02,$00,$02,$02,$02,$02


// make it say "Right Hand" and "Left Hand" in battle item menu
// string is stored in ff6t-misc-strings.txt & might overwrite data
// that looks blank but really isn't?? not 100% sure
org $c2e058; dw $ce00





// nerf joker doom exploits? just gonna leave this out since I dunno
// how/if Tsushiy fully dealt with the issue
//org $c236a9; nop; nop; nop; nop; nop


//c2dfd8 contains layout data for one row of magic menu in battle
//this is copied to 7e5755 to 7e5769 and filled in with the appropriate magic numbers
//kind of a pain to convert all of this into a 2-column menu, bleh



// make esper names in summon slot display full name
org $c16689; fill $18,$ea
org $c16689
jsl display_full_esper_name_in_battle_summon_menu
-
lda $59d980,x
jsr $63d3
inx
dec $40
bne -
rts

// reposition the esper MP cost string to compensate for longer esper names
org $c2e043; db $02