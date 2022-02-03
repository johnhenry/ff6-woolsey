org $55B5A0; db "****************MISC MENU STRNGS****************"
org $55C6D0; db "****************M MENU STRNG END****************"




//----------------------------------------------------------------------------------------
// Main menu
//----------------------------------------------------------------------------------------

org $58FFD0; db "****************MAIN MENU PNTRS ****************"
org $590800; db "****************MNMENU PTRS END ****************"
org $590830; db "****************MMENU TXT STRCTS****************"
org $593030; db "****************MNMENU STRCT END****************"


// for simplicity's sake we'll need to do this a different way
// we'll put an indicator for a custom text in the first 2 bytes of the
// text struct, telling it to look in the new location!!!
// c30306 is where the first 2 bytes are read in, so put the new code here!

org $c30306; jsl redirect_to_custom_menu_text; nop; nop
org $440306; jsl redirect_to_custom_menu_text; nop; nop
org $c30333; jsl redirect_to_custom_menu_text; nop; nop
org $440333; jsl redirect_to_custom_menu_text; nop; nop


// take out dakuten crap that erases stuff we don't want
// this might cause trouble in other spots though!!! ***
org $c30388; nop; nop;
org $c3038d; nop; nop;


// move main menu pointer icon left 1 tile
org $c33025
db $af,$13,$af,$22,$af,$31,$af,$40,$af,$4f,$af,$5e,$af,$6d



// move job class text down and to the left for each character
org $c333a1; ldy #$391d
org $c333ed; ldy #$3a9d
org $c33439; ldy #$3c1d
org $c33485; ldy #$3d9d

// FF6T adds new job names, we're gonna expand them from 7 to 12 characters
org $55AF90; db "****************JOB NAMES BEGIN ****************"
org $55B430; db "****************JOB NAMES END   ****************"

org $c33583
jml load_short_or_long_job_name; nop; nop


org $c33589; lda #$0c
org $c33595; ldy #$000c
org $c33598; lda $55afc0,x

org $59d190; db "****************LONG JOB NAMES  ****************"
org $59d920; db "****************LNG JOBNAMES END****************"

// prevent job name from appearing in magic submenu, as it overwrites other text
// we still need to display status icons though, so we require some new code
org $c33544; jml hide_job_name_in_magic_menus

// prevent job name from appearing in equip/relic menus, status icons not needed
org $c39bde; jmp $87c5


//----------------------------------------------------------------------------------------
// Main Menu > Items page
//----------------------------------------------------------------------------------------

// change cursor positions for top action bar
org $c3854c
db $3f,$16,$67,$16,$af,$16

// resize top subwindows to allow for "Items"
org $c385c4; db $05
org $c385c6; db $99,$58,$15

// resize item name box when using an item, it slightly covers the portraits but oh well
org $c391cb; db $09

// move item name to fit in new item use box
org $c391dd; lda #$78cd

org $5cc5d0; db "****************ITEM DESCS BEGIN****************"
org $5cffd0; db "****************ITEM DESCS END  ****************"
org $c38a97; lda #$5c

// tidy up cursor sprite coordinates
org $c384b7
db $07,$5A,$77,$5A
db $07,$66,$77,$66
db $07,$72,$77,$72
db $07,$7E,$77,$7E
db $07,$8A,$77,$8A
db $07,$96,$77,$96
db $07,$A2,$77,$A2
db $07,$AE,$77,$AE
db $07,$BA,$77,$BA
db $07,$C6,$77,$C6




//----------------------------------------------------------------------------------------
// Main Menu > Items > Detailed item info page
//----------------------------------------------------------------------------------------

// c394bd is start of "wo soubi dekiru kyarakutaa" string, others in the area
// also need to fix the magic learning stuff in the left window

// display " can be used by:" string, string length is manually defined here too for some reason
org $c38d47; lda $55b630,x
org $c38d52; cpy #$0011

// c39058 seems to be start of status icon printing
// d8e990 is start of standard kanji -> icon conversion
org $d8e990
db $7d	// water icon
db $7a	// earth icon, kinda sucks, should make a better one someday
db $76	// light icon, kinda sucks
db $79	// wind icon
db $7e	// darkness icon, had to make own icon for this, looks meh
db $78	// lightning icon
db $7b	// ice icon, is this correct? doesn't look good though
db $7c	// fire icon
db $00  // end of data notifier

org $c3b324; cpx #$0009
org $c3b1ec; jml $4e0800	// undo two-byte kanji codes
org $c39058; ldx #$7b61		// move icons right
org $c39064; ldx #$7be1		// move icons right
org $c39070; ldx #$7c61		// move icons right
org $c3907c; ldx #$7ce1		// move icons right


// make item detail stat numbers display properly, uses a weird pre-made string table
// seems tsushiy added this in and wasn't in the original game?
org $400300
db $FF,$FF,$54,$00,$D3,$FF,$59,$00,$D3,$55,$54,$00,$D3,$55,$59,$00
db $D3,$56,$54,$00,$D3,$56,$59,$00,$D3,$57,$54,$00,$C5,$55,$54,$00
db $C5,$56,$54,$00,$C5,$57,$54,$00,$C5,$59,$54,$00,$FF,$54,$D3,$55
db $D3,$56,$D3,$57,$D3,$58,$D3,$59,$D3,$5A,$D3,$5B,$FF,$54,$C5,$55
db $C5,$56,$C5,$57,$C5,$58,$C5,$59,$C5,$5A,$C5,$5B



//----------------------------------------------------------------------------------------
// Main Menu > Items > Rare Items page
//----------------------------------------------------------------------------------------

org $5592B0; db "****************RARE ITEM DESCS ****************"
org $559C00; db "****************R ITEM DESCS END****************"
org $c38abf; ldx #$0000
org $c38ac2; jsl load_rare_item_descriptions; nop; nop; nop; nop

org $c3b2b7; jsl load_rare_item_vwf_widths; nop; nop; nop

// adjust cursor positions
org $c384f3
db $07,$5A,$4F,$5A,$97,$5A
db $07,$66,$4F,$66,$97,$66
db $07,$72,$4F,$72,$97,$72
db $07,$7E,$4F,$7E,$97,$7E
db $07,$8A,$4F,$8A,$97,$8A
db $07,$96,$4F,$96,$97,$96
db $07,$A2,$4F,$A2,$97,$A2
db $07,$AE,$4F,$AE,$97,$AE
db $07,$BA,$4F,$BA,$97,$BA
db $07,$C6,$4F,$C6,$97,$C6



//----------------------------------------------------------------------------------------
// Main Menu > Skill > Magic page
//----------------------------------------------------------------------------------------

org $557a70; db "****************MED. MAGIC NAMES****************"
org $557c20; db "****************MED. MNAMES END ****************"

org $c35729; ldx #$9e92   // this allows the full string to be printed BUT! this change might bork other stuff!!!

// display short magic names instead of medium names if the spell isn't 100% mastered
// this is because a XX% normally appears right next to the spell name in the magic menu
org $c35692; fill $0E,$EA
org $c35692
normal_magic_name_load:
jsr $57bb
jsr $5771  // returns A as 0xFF if the spell is mastered
esper_magic_name_load:
jsl load_magic_name_for_main_menus_A



org $c3573d
jsl load_magic_name_for_main_menus_B

org $c36142; jsr esper_magic_name_load

org $55A230; db "****************MAGIC DESCS STRT****************"
org $55a930; db "****************MAGIC DESCS END ****************"
org $c36240; ldx #$0000


// adjust cursor positions
org $c35271
db $07,$66,$4F,$66,$97,$66
db $07,$72,$4F,$72,$97,$72
db $07,$7E,$4F,$7E,$97,$7E
db $07,$8A,$4F,$8A,$97,$8A
db $07,$96,$4F,$96,$97,$96
db $07,$A2,$4F,$A2,$97,$A2
db $07,$AE,$4F,$AE,$97,$AE
db $07,$BA,$4F,$BA,$97,$BA
db $07,$C6,$4F,$C6,$97,$C6



//----------------------------------------------------------------------------------------
// Main Menu > Skill > Magic > Use page
//----------------------------------------------------------------------------------------
org $c35ebe; lda #$78cd   // repositions medium spell name



//----------------------------------------------------------------------------------------
// Main Menu > Skill > Rage page
//----------------------------------------------------------------------------------------

// loads our expanded enemy names
org $c35a64; fill $0e,$ea
org $c35a64
plx
phx
jsl load_normal_or_long_enemy_name

// make it erase the longer enemy names properly, BUT!!! this might affect other stuff?
org $c35a95; ldy #$000a

// make game load rage descriptions properly
org $c38ff8; ldx #$0000

// adjust cursor positions
org $c35319
db $17,$66,$87,$66
db $17,$72,$87,$72
db $17,$7e,$87,$7e
db $17,$8a,$87,$8a
db $17,$96,$87,$96
db $17,$a2,$87,$a2
db $17,$ae,$87,$ae
db $17,$ba,$87,$ba
db $17,$c6,$87,$c6

// Display the total number of Rages obtained on the Rage submenu
// For reference, the Rage event bytes are at 7e1d2c to 7e1d4b
org $c35a1c; fill $28,$ea
org $c35a1c
jsl check_rage_bits_and_count_total
pha
sep #$20
lda $64
jsr $0563
ldx #$81bd
jsl display_rage_total
rep #$10
pla
rts




//----------------------------------------------------------------------------------------
// Main Menu > Skill > Dance page
//----------------------------------------------------------------------------------------

org $558C80; db "****************M DANCE NAME BEG****************"
org $558D10; db "****************M DANCE NAME END****************"

// load shortened dance names that we slightly expanded
org $c35e20; ldy #$000c
org $c35e25; ldy #$8cb0
org $c35e2a; lda #$55

// move dance names left a bit on main menu's dance page + adjust cursor positions
org $c35e0c; ldx #$0003
org $c35e17; ldx #$0011

// note that this data area seems to contain the window's function layout
// the previous two bytes seem to indicate 2 columns and 4 rows!
org $c352bd
db $08,$74,$78,$74
db $08,$8c,$78,$8c
db $08,$a4,$78,$a4
db $08,$bc,$78,$bc

// adjust cursor positions to look better, affects other 2x4 screens too
org $c352bd
db $07,$72,$77,$72,$07,$8a,$77,$8a,$07,$a2,$77,$a2,$07,$ba,$77,$ba



//----------------------------------------------------------------------------------------
// Main Menu > Skills page
//----------------------------------------------------------------------------------------

// widen the Espers/Magic/Blitz/etc. sub-menu windows
// this will end up covering the first letter of the current equipped esper though
org $c35460; db $07
org $c35464; db $07

// expanding the skill sub-menu windows means we gotta shorten the esper name by 1
// so we'll load from a second set of slighty shorter esper names
org $55b460; db "****************SHRT ESPER NAMES****************"
org $55b570; db "****************S ESPR NAMES END****************"
org $c335bc; ldy #$0007
org $c335bf; lda $55b490,x
org $c335d0; ldy #$0007

// move shorter esper name right one tile
org $c355db; ldy #$41dd

// move cursor right 1 tile
org $c32f90; lda #$003f

// move status icons right and up a tiny bit to account for the expanded menu windows
org $c355bf; ldx #$4550

// expand small box in skill submenus that say "MP...", "Bushido", "Dance", etc.
org $c3547a; db $71,$61,$09,$01



//----------------------------------------------------------------------------------------
// Main Menu > Skill > Espers page
//----------------------------------------------------------------------------------------

org $559C30; db "****************ESPER ATTK DESCS****************"
org $55a200; db "****************ESPATK DESCS END****************"
org $c36266; ldx #$0000
org $c3626f; lda #$55

org $55C700; db "****************LARG MAG W ICONS****************"
org $55C920; db "****************LGMG W ICONS END****************"

// display "RATE" and "MASTERY" *after* the long esper name is printed
org $c35ff8; fill $32,$ea
org $c35ff8
lda $99
jsr $5bcd
ldy #$444f
jsr $35e1
tdc
lda $99
jsl load_long_esper_name_for_esper_details_page
-
lda $59d980,x
sta $2180
inx
dey
bne -
stz $2180
jsr $87b9
tdc
lda #$20
sta $29
ldy #$634c	// display "RATE  MASTERY"
jsr $0326


// erase/prepare existing spell names before being displayed
org $c3617e; ldy #$0010

// move spell names left
org $c3605a; ldx #$0004

// allow spell names to appear properly by moving the : right
// also take into account the item details sub-menu that shows
//   when an equipped item can teach a spell
org $c3614e; fill $16,$ea
org $c3614e
ldx #$9e94
stx $2181
jsl format_magic_name_and_rate_for_item_details_menu // print other stuff including the "X" character

// display "100%" properly, using our new font
org $c36117; lda #$55
org $c3611c; lda #$54

// "XXX already has that esper equipped" msg
org $c35bf9; lda #$40cd		// move text left
org $c35c1b; lda $55b5d0,x	// load from our new custom string location
org $c35f4b; ldy #$0040		// show the message for a little longer

// adjust cursor positions
org $c35fe6
db $07,$72
db $10,$7e,$10,$8a,$10,$96,$10,$a2,$10,$ae,$10,$ba


//----------------------------------------------------------------------------------------
// Main Menu > Skill > SwdTech page
//----------------------------------------------------------------------------------------

org $55Aa90; db "****************SWDTCH DESCS BEG****************"
org $55af60; db "****************SWDTCH DESCS END****************"
org $c35d62; ldx #$0000
org $c35d6b; lda #$55

org $55A960; db "****************SWDTCH NAMES BEG****************"
org $55aa60; db "****************SWDTCH NAMES END****************"

// delete sword tech names properly when leaving menu
org $c32a11; jsr $0f68

// jpn sword techs are displayed in a different way than usual so we gotta break the old
// code and treat the eng names like regular text, which requires lots of code replacing
org $c3ae09
jsr $71a9
jsr $8b78
jsl prepare_swordtech_name_display
-
phy
jsr display_swordtech_string
jsl display_swordtech_string_spacesaver
ply
dey
bne -
rts

display_swordtech_string:
jsl display_swordtech_string_spacesaverA
jsr $5e2f
jsl display_swordtech_string_spacesaverB
jsr $5e2f
inc $e5
rts


//----------------------------------------------------------------------------------------
// Main Menu > Skill > Blitz page
//----------------------------------------------------------------------------------------

org $55CFF0; db "****************BLITZ DESCS BEG ****************"
org $55D1d0; db "****************BLITZ DESCS END ****************"

org $c35d77; ldx #$0000
org $c35d80; lda #$55



//----------------------------------------------------------------------------------------
// Main Menu > Skill > Blue Magic page
//----------------------------------------------------------------------------------------

org $55D200; db "****************BLUMAG DESCS BEG****************"
org $55d5f0; db "****************BLUMAG DESCS END****************"

org $c35d4d; ldx #$0000
org $c35d56; lda #$55

// adjust cursor positions
org $c352e1
db $07,$66,$77,$66
db $07,$72,$77,$72
db $07,$7e,$77,$7e
db $07,$8a,$77,$8a
db $07,$96,$77,$96
db $07,$a2,$77,$a2
db $07,$ae,$77,$ae
db $07,$ba,$77,$ba
db $07,$c6,$77,$c6




//----------------------------------------------------------------------------------------
// Main Menu > Status page
//----------------------------------------------------------------------------------------

org $c36551; ldy #$0007
org $c36554; lda $5591A0,x

// let the game load our expanded action names
org $c36546; fill $B,$ea
org $c36546; jsl get_action_text_data_location

// prettify the action names in Gogo's action customization list
org $c36514; ldy #$8089	// move text left one tile

org $c36a60	// move cursor positions
db $ee,$0e,$ee,$1e,$ee,$2a,$ee,$36,$ee,$42,$ee,$4e,$ee,$5a,$ee,$66
db $ee,$72,$ee,$7e,$ee,$8a,$ee,$96,$ee,$a2,$ee,$ae,$ee,$ba,$ee,$c6

org $c337db
db $8f,$5a,$8f,$66,$8f,$72,$8f,$7e

// widen window that says "STATUS"
org $c365d1; db $06

// move and resize window that contains battle action names since we expanded the strings elsewhere
org $c365d3; dw $5aed
org $c365d5; db $08

// move character's name to new spot on screen
org $c3669f; ldy #$394f

// c366a0

// move status effect icons
org $c368a7; ldx #$1490

org $c3669f
ldy #$391d	// prints job name
jsr $3580
ldy #$395d	// prints esper name
jsr $35ae
ldy #$38dd	// prints char's name
jsr $356a

// move a bit of blank text out of the way and where the status icon sprites go
org $c368a4; ldy #$38eB
//org $c36957; lda #$20

// we want to print full esper names on the status screen
org $59D950; db "****************LONG ESPER NAMES****************"
org $59DB30; db "****************L ESPR NAMES END****************"
org $c335b8; jml load_long_or_short_esper_names
org $c335d0; jml clear_long_or_short_esper_name; nop


//----------------------------------------------------------------------------------------
// File Load screens
//----------------------------------------------------------------------------------------

// moves Yes/No cursor one tile left
org $c31651
db $b6,$46,$b6,$56

// increase height of "Load this file?/save over file?" box
org $c33269; db $0b


//----------------------------------------------------------------------------------------
// Main Menu > Equip page
//----------------------------------------------------------------------------------------

// move character's name down and on top of portrait
org $c39bd5; ldy #$7b77

// change top window cursor positions
org $c395d9
db $07,$12,$3F,$12,$6F,$12,$AF,$12

// adjust RHand/LHand/etc. cursor positions
org $c395f5
db $37,$2a,$37,$36,$37,$42,$37,$4e

// adjust equipment selecting cursor positions
org $c39620
db $00,$66
db $00,$72
db $00,$7E
db $00,$8A
db $00,$96
db $00,$A2
db $00,$AE
db $00,$BA
db $00,$C6

//----------------------------------------------------------------------------------------
// Main Menu > Relics page
//----------------------------------------------------------------------------------------

// widen the mini window that holds "EQUIP" and "REMOVE" text
org $c39c82
dw $60b7
db $06

// move top window cursor positions
org $c39646
db $0F,$12,$47,$12

// arrange Relic slot cursor positions
org $c3965e
db $37,$42,$37,$4e


//----------------------------------------------------------------------------------------
// Main Menu > Config page
//----------------------------------------------------------------------------------------

// c3396f is where the entire config display routine begins!!!

// widen box that says "CONFIG"
org $c33aba
dw $58b7
db $06

// make number slider number display correct number
org $c33cb1; adc #$55
org $c33cf2; adc #$55
org $c34143; adc #$55
org $c341f1; adc #$55

// the magic type list in the config menu originally only allowed for 4 characters, not enough for our needs
org $c34175; fill $34,$ea
org $c34175; jsl print_magic_types_in_config_menu

// Config > Battle Menu type > Arrange screen
// prevent character jobs from being displayed
org $c345b2; nop; nop; nop;
org $c345eb; nop; nop; nop;
org $c34624; nop; nop; nop;
org $c3465d; nop; nop; nop;

// widen the "BUTTON CONFIG" mini-window
org $c34a80
dw $58a9
db $0d

// arrange cursor positions
org $c3393b
db $5f,$2b,$5f,$3b,$5f,$4b,$5f,$5b,$5f,$6b,$5f,$7b,$5f,$8b,$5f,$9b,$5f,$ab,$5f,$bb

org $c33963
db $5f,$2b,$5f,$6b,$5f,$7b,$5f,$9b,$5f,$ab,$5f,$bb



//----------------------------------------------------------------------------------------
// Main Menu > Config > Sound test menu
//----------------------------------------------------------------------------------------

org $c4a800; db "****************SONG NAMES BEGIN****************"
org $c4bbc0; db "****************SONG NAMES END  ****************"
org $c3c033; lda $c40000,x

org $c3c093
db $07,$32,$07,$3e,$07,$4a,$07,$56,$07,$62,$07,$6e,$07,$7a
db $07,$86,$07,$92,$07,$9e,$07,$aa,$07,$b6,$07,$c2



//----------------------------------------------------------------------------------------
// Party set-up screens
//----------------------------------------------------------------------------------------

// moves the digit left in "Form X parties"
org $c37d00; ldx #$3927

// allow it to print "parties" instead of "party" if more than 1 is needed
org $c37cf8; jsl print_party_or_parties; nop

// remove the number from the "not enough parties" string
org $c37a78; nop; nop; nop

// let the error messages last a little longer
org $c37a83; lda #$40

// move job, name, and esper text up to allow for long job name
org $c38114; ldy #$399b
org $c38121; ldy #$3a1b
org $c38127; ldy #$3a9b

// actually, the job name text looks ugly so let's drop it
org $c38114; fill $0d,$ea
org $c38114
lda #$20
sta $29


//----------------------------------------------------------------------------------------
// Final battle party arrangement menu
//----------------------------------------------------------------------------------------

org $c3b5d8
db $0f,$1e,$1f,$2a,$1f,$36,$1f,$42
db $1f,$4e,$1f,$5a,$1f,$66,$1f,$72,$1f,$7e,$1f,$8a,$1f,$96,$1f,$a2
db $1f,$ae,$0f,$ba



//----------------------------------------------------------------------------------------
// Shop menus
//----------------------------------------------------------------------------------------

// resize buy/sell/exit window and Gil windows
org $c3cb7c; db $11,$02,$b1,$59,$09,$02

// adjust cursor positions
org $c3c80b
db $07,$32,$07,$3e,$07,$4a,$07,$56,$07,$62,$07,$6e,$07,$7a,$07,$86

org $c3c7aa; ldx #$0007
org $c3c7af; ldx #$0032

org $c3c5ac; ldx #$0007
org $c3c5b1; ldx #$0032

// move total gil number
org $c3c8dc; ldx #$7a33

// move cursor positions
org $c3c7f1
db $07,$32,$2f,$32,$5F,$32

// let various error messages last a little longer
org $c3c7d8; lda #$40

// allow medium item names to be displayed on "Buy this item?" screen
org $c3cfd8; jml load_item_names_for_shops; nop


//----------------------------------------------------------------------------------------
// Coliseum menus
//----------------------------------------------------------------------------------------

// resize some windows
org $c3b6fb
db $08,$02,$9f,$58,$12,$02

// we'll load custom medium-sized item names for the coliseum menu
org $59db60; db "****************MED ITEM NAMES  ****************"
org $59e9d0; db "****************MED ITEMS END   ****************"

// load medium-sized item strings properly
org $4e0999; lda #$0d
org $4e09a4; ldy #$000d
org $4e09a7; lda $59db90,x
org $c3bbec; sta $7e9e98

// move both item names to the left to fill their boxes
org $c3bbbd; ldx #$78eb
org $4e0a4d; ldx #$78cd



//----------------------------------------------------------------------------------------
// Naming screens
//----------------------------------------------------------------------------------------

// c36e90 area is where windows are defined

org $c36bad; bra $c36bb9 // disable access to the bottom screen

org $c36fdc; lda #$00ff // remove flashing arrow from screen, simplest and safest fix

// character layout begins around d8e8c0
// first hiragana a is at d8e8c8
// need to disable dakuten support too
// the dakuten stuff makes many characters wrong

// remove dakuten support
org $c36ef9
cmp #$20
bcc $c36f23

// adjust cursor positions to look better
org $c36d34
db $37,$36,$47,$36,$57,$36,$67,$36,$77,$36,$8F,$36,$9F,$36,$AF,$36,$BF,$36,$CF,$36
db $37,$46,$47,$46,$57,$46,$67,$46,$77,$46,$8F,$46,$9F,$46,$AF,$46,$BF,$46,$CF,$46
db $37,$56,$47,$56,$57,$56,$67,$56,$77,$56,$8F,$56,$9F,$56,$AF,$56,$BF,$56,$CF,$56
db $37,$66,$47,$66,$57,$66,$67,$66,$77,$66,$8F,$66,$9F,$66,$AF,$66,$BF,$66,$CF,$66
db $37,$76,$47,$76,$57,$76,$67,$76,$77,$76,$8F,$76,$9F,$76,$AF,$76,$BF,$76,$CF,$76
db $37,$86,$47,$86,$57,$86,$67,$86,$77,$86,$8F,$86,$9F,$86,$AF,$86,$BF,$86,$CF,$86
db $37,$96,$47,$96,$57,$96,$67,$96,$77,$96,$8F,$96,$9F,$96,$AF,$96,$BF,$96,$CF,$96
db $37,$A6,$47,$A6,$57,$A6,$67,$A6,$77,$A6,$8F,$A6,$9F,$A6,$AF,$A6,$BF,$A6,$CF,$A6
db $37,$B6,$47,$B6,$57,$B6,$67,$B6,$77,$B6,$8F,$B6,$9F,$B6,$AF,$B6,$BF,$B6,$CF,$B6
db $37,$C6,$47,$C6,$57,$C6,$67,$C6,$77,$C6,$8F,$C6,$9F,$C6,$AF,$C6,$BF,$C6,$CF,$C6






