arch snes.cpu

ExHiROM

//----------------------------------------------------------------
// Properly initialize $1444 to allow special battle item drop names
// to always display properly after a >256 chest is opened
//----------------------------------------------------------------

org $c2d3d2
jsl properly_clear_data_for_battle_item_drops
nop



//----------------------------------------------------------------
// Fix countdown timer display when the main menu is open
// Original FF6T didn't give enough time for division math to end
//----------------------------------------------------------------

org $c33355
jmp $335e



//----------------------------------------------------------------
// Fix step count > 200,000 achievement bug
// Note that it actually unlocks at around 196,608 steps
//
// This flag checker is also used to check the player's total Gil,
// even though there's no FF6T achievement for it
//----------------------------------------------------------------

org $4008a3
jsl fix_step_counter_achievement_check; nop; nop



//----------------------------------------------------------------
// Fix rage achievement unlock bug - getting the Tonberries rage
// used to override 15 other rages, allowing the player to get
// the achievement early
//
// Note that the flag checker connected to this is also used
// to check to see if Sabin has all his blitzes, even though FF6T
// doesn't have an achievement for having them all
//----------------------------------------------------------------


org $400933
jsl fix_rage_achievement_check