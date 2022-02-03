org $58e000

//----------------------------------------------------------------
// Do calculations so location name strings get properly centered
//----------------------------------------------------------------

place_text_custom_center_routine:
lda $f0  // we need a temp vwf width total, so we'll borrow this RAM
pha
stz $f0

place_text_center_looper_start:
lda [$c9],y
beq place_text_center_looper_end
cmp #$20
bcs +
iny
+
tax
lda $c48fc0,x
clc
adc $f0
sta $f0
iny
bra place_text_center_looper_start

place_text_center_looper_end:
lda #$e0
sec
sbc $f0
lsr
sta $bf

pla   // restore the piece of RAM we used
sta $f0
lda $bf
jml $c08036



//----------------------------------------------------------------
// Make the battle dialogue display routine load the proper
// widths for each character rather than assume they're all 0x0D
// pixels wide
//----------------------------------------------------------------

load_battle_dialogue_text_character_width_PartA:
// A has the raw character being read in
// we want to ignore characters that are 0c through 0f, inclusive

sta $ecb0   // NOTE!!! THIS MIGHT NOT BE A SAFE LOCATION, IT'S JUST THE EQUIVALENT LOCATION THAT FF3US USES FOR THIS PURPOSE
sec         // Also, this Part A ensures that part B will load the proper stuff when displaying a variable
sbc #$20
phy
jml $c16103

//-------

load_battle_dialogue_text_character_width_PartB:
phx
lda $ecb0  // problem here: if the code is a variable it just loads the variable code's width and not the right widths
tax
lda $c48fc0,x
plx
clc
adc $7a
sta $7a
rtl


//----------------------------------------------------------------
// Add "n " or just " " to main script [ITEM] control codes
//----------------------------------------------------------------

load_main_script_item_name:
ldy $00
lda #$7e
pha
plb

lda $1444	// if this item ID > 256 let's do the extra needed logic, else do normal
bit #$01
beq +


jsl	load_main_script_extended_item_name_and_article
stz $1444
rtl

+

jsl	load_main_script_standard_item_name_and_article
stz $1444
rtl



//----------------------------------------------------------------

load_main_script_standard_item_name_and_article:
phx

ldx $d0			// check to see current script line number, if umaro or gau, don't add article!
cpx #$29c       // check if this is gau, if so don't use any articles!
beq main_script_standard_item_name_copier
cpx #$29e       // check if this is umaro
beq main_script_standard_item_name_copier

// Let's prefix with the letter "A " if article ID == 1
lda $0583       // load the item number
tax
lda $59BBa0,x   // check our custom article data block
cmp #$01
bne +
lda #$20        // adds "A"
sta $9183,y
iny
lda #$7F        // adds space
sta $9183,y
iny

// let's lowercase the "a" if we're doing a generic "Obtained [ITEM]!"
ldx $d0
cpx #2949
bne main_script_standard_item_name_copier
lda #$3A
sta $9181,y
jmp main_script_standard_item_name_copier


+
// Let's prefix with the string "An " if article ID == 2
lda $0583       // load the item number
tax
lda $59BBa0,x   // check our custom article data block
cmp #$02
bne +
lda #$20        // adds "A" after article if necessary
sta $9183,y
iny
lda #$47        // adds "n"
sta $9183,y
iny
lda #$7F        // adds space
sta $9183,y
iny

// let's lowercase the "a" if we're doing a generic "Obtained [ITEM]!"
ldx $d0
cpx #2949
bne main_script_standard_item_name_copier
lda #$3A
sta $9180,y



+
main_script_standard_item_name_copier:
plx
-
lda $59a561,x
sta $9183,y
cmp #$ff
beq +
inx
iny
cpy #$0013
bne -

+
rtl





//----------------------------------------------------------------
load_main_script_extended_item_name_and_article:
phx

ldx $d0			// check to see current script line number, if umaro or gau, don't add article!
cpx #$29c       // check if this is gau, if so don't use any articles!
beq main_script_extended_item_name_copier
cpx #$29e       // check if this is umaro
beq main_script_extended_item_name_copier

rep #$20
lda #$0000
sep #$20
lda $0583       // load the item number + 0x100
rep #$20
clc
adc #$100
tax
sep #$20

// Let's prefix with the letter "A " if article ID == 1
lda $59BBa0,x   // check our custom article data block
cmp #$01
bne +
lda #$20        // adds "A" at start of item string
sta $9183,y
iny
lda #$7F        // adds space
sta $9183,y
iny

// let's lowercase the "a" if we're doing a generic "Obtained [ITEM]!"
ldx $d0
cpx #2949
bne main_script_extended_item_name_copier
lda #$3A
sta $9181,y
jmp main_script_extended_item_name_copier


+
// Let's prefix with the letter "An " if article ID == 2
lda $59BBa0,x   // check our custom article data block
cmp #$02
bne +
lda #$20        // adds "A" at start of item string
sta $9183,y
iny
lda #$47        // adds "n"
sta $9183,y
iny
lda #$7F        // adds space
sta $9183,y
iny

// let's lowercase the "a" if we're doing a generic "Obtained [ITEM]!"
ldx $d0
cpx #2949
bne main_script_extended_item_name_copier
lda #$3A
sta $9180,y



+
main_script_extended_item_name_copier:
plx
rep #$20
txa
clc
adc #$1400
tax
sep #$20

-
lda $59a561,x
sta $9183,y
cmp #$ff
beq +
inx
iny
cpy #$0013
bne -

+
rtl




//----------------------------------------------------------------
load_enemy_names_for_battle_window:
ldx $10   // saving stuff so we can use RAM

sta $10
asl
asl
asl
adc $10
adc $10

stx $10

tax

sep #$20
lda #$0a

rtl


//----------------------------------------------------------------
get_enemy_move_string_location:

lda ($76),y
sta $22
asl
asl
asl
asl
clc
adc $22
clc
adc $22
clc
adc $22
clc
adc $22

clc
rtl

//----------------------------------------------------------------

// NEED TO IMPROVE THIS!!
// IF ON ESPER DETAILS PAGE
//		LOAD ESPER PAGE LENGTH NAMES
// ELSE IF ON MAGIC PAGE
//		IF NOT MASTERED
//			LOAD SHORT NAMES
//		ELSE
//			LOAD MEDIUM NAMES
// $26 HAS MENU STATE
//		0a or 1a when loading magic menu, 3c when loading back from using magic
//		3a when using magic
//		1e when loading esper details menu


load_magic_name_for_main_menus_A:
pha
lda $26
cmp #$1e	// if esper details screen
beq load_magic_name_full
cmp #$19	// if item details screen
beq load_magic_name_full

			// if we're here, we're printing small or medium names
pla			// A now contains the mastery percentage again
cmp #$ff	// mastery will be 0xFF if fully mastered
beq load_magic_name_medium

ldy #$0005  // if spell isn't mastered, we use smaller name
sty $eb
ldy #$F000
sty $ef
lda #$58
sta $f1
rtl





load_magic_name_full:
pla
ldy #$0009
sty $eb
ldy #$c730
sty $ef
lda #$55
sta $f1
rtl



load_magic_name_medium:
ldy #$0007
sty $eb
ldy #$7aa0
sty $ef
lda #$55
sta $f1
rtl






//----------------------------------------------------------------
load_magic_name_for_main_menus_B:
lda #$2c
sta $29
ldx #$9e90
stx $2181
rtl





//----------------------------------------------------------------
load_rare_item_descriptions:
stz $1440
stx $eb
lda #$55
sta $ed
lda #$ce
sta $e9
rtl


//----------------------------------------------------------------
load_rare_item_vwf_widths:
rep #$20
and #$00FF
clc
adc #$0020
tax

sep #$20
lda $c48fc0,x 
clc
adc $8d
sta $8d

rtl

//----------------------------------------------------------------
prepare_swordtech_name_display:
lda #$20
sta $29		// sets color to white
inc $e6
inc $e6
stz $e5
ldy #$0004  // loop for 4 rows
rtl


load_next_swordtech_name:
ldy #$000c	// max length of each string
sty $eb
ldy #$a990	// lower part of text address base
sty $ef
lda #$55	// high part of text address base
sta $f1
rtl


display_swordtech_string_spacesaver:
lda $e6
inc
inc
inc
inc
and #$1f
sta $e6
rtl


display_swordtech_string_spacesaverA:
jsl load_next_swordtech_name
ldx #$0003
rtl


display_swordtech_string_spacesaverB:
inc $e5
jsl load_next_swordtech_name
ldx #$0011
rtl





//----------------------------------------------------------------
// Allows enemy names to appear during Scan/Libra messages
// Lots of ugly work here, it basically latches onto the [ITEM]
// code and displays the current enemy's name instead if 
// a specific line number from ff6t-battle-text.txt is being
// displayed. Because it's using the item code,
// Enemy 255 (which is "Tonberries") won't get its name displayed.
//----------------------------------------------------------------
prep_name_in_scan_stuff:
ldx $b6		// original setup code
lda #$ff
sta $2d72
lda #$02
sta $2d6e
stz $2f36
stz $2f37
stz $2f3a
rtl


add_name_to_scan_messages_A:
rep #$20
lda $1ff9,x
sta $2f35
sep #$20
lda #$4d
sta $2d6f
lda #$04
rtl


add_name_to_scan_messages_B:	// original Level message display
stz $2f36
lda $3b18,x
sta $2f35
lda #$34
sta $2d6f
lda #$04
rtl




add_name_to_scan_messages_C:
lda $2d6f	// if battle text line ID == 0x4d print enemy name instead of item name
cmp #$4d
beq allow_for_more_than_256_enemies

lda $2f35	// if we're doing a standard ITEM code, do original stuff
cmp #$ff
bne +
lda #$09
jml $c15f8c
+
xba
lda #$14
sta $10
sta $613d
jml $c1600f


allow_for_more_than_256_enemies:
rep #$20
lda $2f35
xba
lda #$14
sta $10
sta $613d
jml $c1600f




make_item_code_display_enemy_name_in_scan:
lda $2d6f	// current battle text line number, if 0x4D we want to print the enemy name and not the item name
cmp #$4d
beq load_enemy_scan_name
-
lda $59a560,x
// if A is between 0C and 0F inclusive, let's ignore it!
cmp #$0C
bcc +
cmp #$10
bcs +

// if we're here, then we're loading a character between 0xC and 0xF
// we want to skip these during the item name copy routine
// so let's move forward one character
inx
jmp -

+
jml $c16023

load_enemy_scan_name:
phx
ldx $2f35
cpx #$0100	// if we're looking at an enemy ID > 255 then read from the next block of names
bcs +

plx
lda $55d650,x
jml $c16023

+
plx
lda $55EA50,x
jml $c16023



//----------------------------------------------------------------
// Because we've expanded action text from 5 or 6 to 7 chars,
// we need to adjust the multiplication done to reach the
// desired action name
//----------------------------------------------------------------

get_action_text_data_location:
sta $e0
asl
asl
clc
adc $e0
clc
adc $e0
clc
adc $e0
tax
rtl







redirect_to_custom_menu_text:
cmp #$0400
bcs +

phx
asl
tax
lda $590000,x
inc
inc
sta $e7

lda $590000,x
tax
lda $590000,x
sta $eb

sep #$20
lda #$59
sta $e9

plx
rtl

+
sta $eb
inc $e7
inc $e7
rtl









hide_job_name_in_magic_menus:
lda #$20	// set color to white
sta $29

phx
ldx $e7
cpx #$4550	// very iffy way of checking if we're in the magic menus
bne +		// this is the starting sprite location for status ailment icons on this screen

plx
jml $c387c5	// skip job name printing

+
plx			// continue code as normal if not in magic menus
jml $c33548







print_magic_types_in_config_menu:
lda $c341b3,x
asl
asl
tax		// x now has the index * 0x10
phy
ldy #$0000

-
lda $55b600,x
beq +
phx
tyx
sta $7e9e8b,x
plx
inx
iny
bra -

+
lda #$00
sta $7e9e8b,x

ply
dey
tya
asl
tax
rep #$20
lda $c341ad,x
sta $7e9e89

sep #$20
ldy #$9e89
sty $e7
lda #$7e
sta $e9
rtl



format_magic_name_and_rate_for_item_details_menu:
rep #$20
lda $7e9e89
cmp #$82ef
bne +		// if we're not in the item details page do normal stuff

sep #$20
lda #$cf // print ":"
sta $2180
lda #$ff
rtl			// don't print the extra spaces between the magic name and rate

+
sep #$20
lda #$ff
sta $2180
lda #$cf // print ":"
sta $2180
lda #$ff
sta $2180	// do print the extra spaces between the magic name and rate
sta $2180
rtl



display_esper_screen_rate_label:
lda $4b
tax
phx
ldy #$634e
plx
lda $7e9d89,x
rtl





print_party_or_parties:
sta $29
ldy #$8229	// start with "form X party" string as default

lda $0201
and #$07
cmp #$01
beq +

ldy #$8230	// "form X parties" string

+
rtl




load_short_or_long_job_name:
lda $0001,y	// original code
sta $4202

plx			// put the return PC address in X so we can see where this is being called from
phx

cpx #$66a4	// this is 0x66a4 if we're loading the status screen
bne +

lda #$14	// print longer name for status screen
sta $4203
nop
nop
nop
nop
ldx $4216
ldy #$0014
-
lda $59d1c0,x
sta $2180
inx
dey
bne -
jml $c335a3


+
jml $c33589 // do default job length size on non-status screens






load_long_or_short_esper_names:
plx			// put the return PC address in X so we can see where this is being called from
phx



cpx #$66aa  // if this is 0x66aa then we're loading the status screen
bne +

asl			// load long esper name and do custom loading
asl
asl
asl
tax
ldy #$0010
-
lda $59d980,x
sta $2180
inx
dey
bne -
jml $c335ca


+
asl		// load short esper name and do regular loading
asl
asl
tax
jml $c335bc






clear_long_or_short_esper_name:
ldy #$0007		// default amount to erase of esper's name

plx
phx
cpx #$66aa		// if we're on the loading screen, erase more text
bne +
ldy #$0010

+
lda #$ff
jml $c335d5





load_long_esper_name_for_esper_details_page:
rep #$20
asl
asl
asl
asl
tax
sep #$20
ldy #$0010
rtl








load_normal_or_long_enemy_name:
cpx #$bc07
bne +

ldy #$0014	// load our full length enemy names
sty $eb
ldy #$d650
sty $ef
lda #$55
sta $f1

rtl

+
ldy #$000a	// load our standard 10-letter enemy names
sty $eb
ldy #$bd60
sty $ef
lda #$59
sta $f1
rtl





load_item_names_for_shops:
plx
phx
cpx #$ca25		// are we calling from the "buy this item?" page?
beq +

cpx #$ca3e		// are we calling from the "sell this item?" page?
beq +

lda #$09		// original code for short items
sta $211c
sta $211c
jml $c3cfe0

+
lda #$0d		// load medium item name
sta $211c
sta $211c
ldx $2134
ldy #$000d
-
lda $59db90,x
sta $2180
inx
dey
bne -
jml $c3cff1



// bug fix for original ff6t
properly_clear_data_for_battle_item_drops:
stz $1444
lda #$5c
sta $1500
rtl





display_cast_name_properly:
sec
sbc #$20
//ldy #$0000
stz $eb
stz $ec
//sta $eb
rtl



center_cast_name_properly:
ldx $00
ldy #$0000
sty $3d		  // initialize string's width
rep #$20	  // initialize A
lda #$0000
sep #$20

-
lda $7e9e89,x
sta $7e9e93,x
cmp #$ff
beq +

inx
phx
tax
sep #$20
lda $c48fc0,x // load character's width
rep #$20
plx
clc
adc $3d
sta $3d
sep #$20

cpx #$0006
bne -


+
lsr $3d		//    6*C/2 - totalWidth/2

rep #$20
lda #$0080
sec
sbc $3d
eor #$ffff
inc
//lda #$0000
sta $3d
sep #$20

rtl






fix_step_counter_achievement_check:
tay
ldx $1800,y

cmp #$68
bne +

txa
cmp $ec
rtl

+
cpx $ec
rtl






// I wanted to use a blank character with 1-pixel widths to fine-tune the positions of text
// but this turned out to be super slow as it would spend a ton of time rendering tiny
// blanks. So here we're gonna use 8-width blanks when possible, and then fill the rest
// with 1-width blanks
improve_space_control_code:
phy
lda $be
ldx #$0000
tax
sec
stx $4204
lda #$08
sta $4206
nop
nop
nop
nop
nop
nop
nop
ldx $4214	// load number of 8-width spaces to fill
cpx #$0000  // if 0 8-width spaces, then skip this part
beq +

txa
sta $1e   // do as many 8-width spaces as available
stz $1f
ldx $00
lda #$70  // insert 8-width spaces

-
sta $7e9183,x
inx
cpx $1e
bne -


+
ldy $4216	// y has remaining number of 1-width spaces
cpy #$0000  // if no remaining 1-spaces needed, skip it
beq +

lda #$75	// insert 1-width spaces
-
sta $7e9183,x
inx
dey
cpy #$0000
bne -

+
tdc
sta $7e9183,x
stz $cf

ply
rtl











fix_rage_achievement_check:
rep #$20
pha
sep #$20
tay
ldx $1d00,y


// see if we're lookinga the final rage word
cpy #$004a
bne +

// ignore the high Tonberry bit in the final rage word
rep #$20
txa
and #$7FFF
tax
sep #$20


+
rep #$20
pla
sep #$20
rtl










check_rage_bits_and_count_total:
stz $64 // init our Rage count

ldx #$9d89
stx $2181
sep #$10
ldx $00
-
lda $c26600,x
tay
phx
clc
jsl $c26700
bit $1d2c,x
beq +
inc $64
tya
bra crbact_store
+
lda #$ff
crbact_store:
sta $2180
plx
inx
bne -
rep #$10
rtl



display_rage_total:
ldy #$0003
sty $e0
stx $eb
lda #$7e
sta $ed
ldy #$0000
tyx
-
lda $f7,x
sta [$eb],y
iny
lda #$20 // white font
sta [$eb],y
iny
inx
cpx $e0
bne -
rtl




display_full_esper_name_in_battle_summon_menu:
rep #$20
pha
sep #$20
pha
lda #$0c
sta $40

pla
rep #$20
and #$00ff
asl
asl
asl
asl
tax
pla
sep #$20
rtl