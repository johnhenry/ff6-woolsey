// speed up [space_xx] by having it only do 1 but have it be xx length


//====================================================================
// UNDO FF6T'S FONT HACK SINCE WE DON'T NEED 8000 KANJI, ETC.
//====================================================================

// make the game load VWF widths from the original area
org $C08407; lda $c48fc0,x
org $c0842c; adc $c48fc0,x
org $c08448; adc $c48fc0,x
org $408407; lda $c48fc0,x
org $40842c; adc $c48fc0,x
org $408448; adc $c48fc0,x

// make the game load font glyphs from the original area
org $c08a5c; lda $c49200,x

org $c08a82; lda $c49200,x
org $c08a9d; lda $c49202,x
org $c08ab8; lda $c49204,x
org $c08ad3; lda $c49206,x
org $c08aee; lda $c49208,x
org $c08b09; lda $c4920A,x
org $c08b24; lda $c4920C,x
org $c08b3f; lda $c4920E,x
org $c08b5a; lda $c49210,x
org $c08b75; lda $c49212,x
org $c08b90; lda $c49214,x

org $c08998; lda $c49200,x
org $c089a9; lda $c49202,x
org $c089ba; lda $c49204,x
org $c089cb; lda $c49206,x
org $c089dc; lda $c49208,x
org $c089ed; lda $c4920A,x
org $c089fe; lda $c4920C,x
org $c08a0f; lda $c4920e,x
org $c08a20; lda $c49210,x
org $c08a31; lda $c49212,x
org $c08a42; lda $c49214,x


org $c3b253; lda $c494c0,x


//====================================================================
// Make on-screen countdown timer display properly with our new font
//====================================================================

// copy digits 0 through 7 to tile memory
org $c06aee; lda $c48500,x; eor $c48501,x
org $c06af9; lda $c48501,x
org $c06b00; lda $c48502,x; eor $c48503,x
org $c06b0b; lda $c48503,x
org $c06b12; lda $c48504,x; eor $c48505,x
org $c06b1d; lda $c48505,x
org $c06b24; lda $c48506,x; eor $c48507,x
org $c06b2f; lda $c48507,x
org $c06b36; lda $c48508,x; eor $c48509,x
org $c06b41; lda $c48509,x
org $c06b48; lda $c4850a,x; eor $c4850b,x
org $c06b53; lda $c4850b,x
org $c06b5a; lda $c4850c,x; eor $c4850d,x
org $c06b65; lda $c4850d,x
org $c06b6c; lda $c4850e,x; eor $c4850f,x
org $c06b77; lda $c4850f,x

// copy digits 8 and 9 to tile memory
org $c06ba8; lda $c48500,x; eor $c48501,x
org $c06bb3; lda $c48501,x
org $c06bba; lda $c48502,x; eor $c48503,x
org $c06bc5; lda $c48503,x
org $c06bcc; lda $c48504,x; eor $c48505,x
org $c06bd7; lda $c48505,x
org $c06bde; lda $c48506,x; eor $c48507,x
org $c06be9; lda $c48507,x
org $c06bf0; lda $c48508,x; eor $c48509,x
org $c06bfb; lda $c48509,x
org $c06c02; lda $c4850a,x; eor $c4850b,x
org $c06c0d; lda $c4850b,x
org $c06c14; lda $c4850c,x; eor $c4850d,x
org $c06c1f; lda $c4850d,x
org $c06c26; lda $c4850e,x; eor $c4850f,x
org $c06c31; lda $c4850f,x



//====================================================================
// fix cast of character name display at end of game
org $c3afaf; jsl display_cast_name_properly; nop

// center cast name properly instead of assuming 8 width for each char
org $c3ef03; fill $23,$ea // NOP most of existing centering code
org $c3ef03; jsl center_cast_name_properly

// display extra-wide names properly
org $c3af77; ldy #$0592
org $c3af7c; ldy #$0580

org $c3af87; ldy #$05d2
org $c3af8c; ldy #$05c0


//====================================================================
// Change [SPACE_XX] control code to display char 0x75 which we'll
// set to have 1 pixel width and be blank, allowing us to finely
// format stuff as needed
//====================================================================

org $c082ba; lda #$75


//====================================================================
// Change 2-byte script text pointer load routine to 3-byte pointer
// Original/Bank 1 script only
//====================================================================

org $c07fbf
rep #$20
lda $d0
asl
adc $d0
tax
lda $5c9ce0,x
sta $c9
sep #$20
lda $5c9ce2,x
sta $cb

// original code
rep #$20
tdc
sep #$20
lda #$01
sta $0568
rts



//====================================================================
// Change 2-byte script text pointer load routine to 3-byte pointer
// FF6T Custom Bank 2 script only
//====================================================================

org $c0ee33
rep #$20
lda $d0
asl
adc $d0
tax
lda $51E600,x
sta $c9
sep #$20
lda $51E602,x
sta $cb
rep #$20
tdc
sep #$20
lda #$01
sta $0568
rts




//====================================================================
// Expand DTE text compression range
// We're going to make characters 0x80 through 0xEF act as dictionary
// entries instead of just 0xD8 through 0xEF
//====================================================================

org $c081c8; cmp #$80
org $c083ce; sbc #$80
org $c083d4; lda $5ffc00,x
org $c083dd; lda #$5f



//====================================================================
// Display character names properly in main script
//====================================================================

org $c0821c
nop; nop
nop; nop; nop; nop

org $c08225
sta $7e9183,x

org $c0822f
nop
cpx #$0006



//====================================================================
// Load location name strings from our new location
//====================================================================

org $c07ffd; lda #$5f
org $c0800c; nop; nop; nop


//====================================================================
// Load expanded item names for main script [ITEM] control code
//====================================================================

// load our new custom article data for [ITEM] control codes
org $59BB70; db "****************ITEM ARTICLES   ****************"
org $59BD00; db "****************ITEM ARTS END   ****************"

org $c0834a; lda #$14
org $c08355; jsl load_main_script_item_name; fill $14,$EA



//====================================================================
// Center location name strings, original Japanese ROM just assumes
// everything is 12 pixels wide when doing centering
//====================================================================

org $c08017
place_name_centering:
jml place_text_custom_center_routine






//====================================================================
// Make small font, non-dialogue text display the new font properly
//====================================================================

// prevent the game from adding dakutens, etc. if char < 0x53
org $c3035b; nop; nop; nop; nop;

// prevent battle item menu from adding dakuten stuff
org $c163d3; nop; nop; nop; nop;

// make battle item menu show proper item count
// not sure if this affects anything els
org $c164ae; adc #$54
org $c164c1; adc #$54

// effects ???
org $c08329; adc #$54

// effects ???
org $c16470; adc #$54
org $c16485; adc #$54
org $c1648c; adc #$54

// effects ???
org $c16794; adc #$54
org $c167a7; adc #$54

// effects ???
org $c169fd; adc #$54


// effects in-battle timer
org $c2b908; adc #$5e
org $c2b915; adc #$54
org $c2b927; adc #$5e
org $c2b92e; adc #$54

// effects ???
org $c305a3; adc #$54

// effects ???
org $c305db; adc #$54
org $c305ed; cmp #$54  // not sure about this!

// effects ???
org $c3063f; adc #$54
org $c30653; cmp #$54  // not sure about this!

// effects ???
org $c37d92; adc #$54

// effects ???
org $c3c1da; adc #$54

// effects ???
org $408329; adc #$54

// effects ???
org $4405a3; adc #$54

// effects ???
org $4405db; adc #$54
org $4405ed; cmp #$54  // not sure about this

// effects ???
org $44063f; adc #$54
org $440653; cmp #$54 // not sure about this

// effects ???
org $447d92; adc #$54

// effects ???
org $44c1da; adc #$54

// effects ???
org $456470; adc #$54
org $456485; adc #$54
org $45648c; adc #$54

// effects ???
org $4564ae; adc #$54
org $4564c1; adc #$54

// effects ???
org $456794; adc #$54
org $4564a7; adc #$54

// effects ???
org $4569fd; adc #$54

// effects in-battle timer
org $47b908; adc #$5e
org $47b915; adc #$54
org $47b927; adc #$5e
org $47b92e; adc #$54


// effects ???
org $44056e; cmp #$54

org $c3056e; cmp #$54


// make battle HP digits display right?
org $c16953; lda #$54
org $c1696b; lda #$54
org $456953; lda #$54
org $45696b; lda #$54

// make MP Cost digits display properly
org $c10558; adc #$13
org $c10560; adc #$09
org $c10568; adc #$09

// 18 69 53 is adc #$53, need to find all of these!


// modify the [SPACE_XX] code to be a little more helpful & efficient for us
org $c082b2; fill $1a,$ea
org $c082b2; jsl improve_space_control_code