vocabulary tictactoe
tictactoe definitions

create board   9 chars allot

char X constant x
char O constant o
defer playing
variable state

: clear   board 9 bl fill ;

: tile   chars board + ;
: tile@   tile c@ ;
: tile!   tile c! ;

create buffer 8 chars allot

: "retry"   ." try again " ;

create input   8 chars allot
variable #input
variable >input

: accept   input 8 accept #input ! 0 >input ! ;
: nextc   >input @ dup 1+ >input ! chars input + c@ ;
: backc   >input @ if -1 >input +! then ;
: end?   >input @ #input @ = ;
: deblank
 begin end? 0= while
  nextc bl <> if backc exit then
 repeat ;
: >blank
 begin end? 0= while
  nextc bl .s cr = if backc exit then
 repeat ;
: word
 deblank >input @ >r >blank >input @
 r@ -
 r> chars input + swap ;

: prompt   playing emit ." 's turn. enter the column and row numbers " ;

: number   0 0 word >number nip nip 0= ;
: 2numbers   accept number number rot and ;

: input-xy ( -- x y )
 begin 2numbers 0= while 2drop "retry" repeat
 1- swap 1- swap cr ;

: validate ( x y -- tile true | false )
 2dup 2 u> swap 2 u> or if 2drop false exit then
 3 * +
 dup 8 u> if drop false exit then
 dup tile@ bl = dup if exit then
 nip ;

: place   playing swap tile! ;

: take-turn
 prompt
 begin input-xy validate 0= while "retry" repeat
 place ;

: turn   playing x = if ['] o else ['] x then is playing ;

: uppercase
 dup [char] Z > if
  [ char A char a - ] literal + then ;

: character
 begin accept word 1 <> while drop "retry" repeat
 c@ uppercase ;

: choose-turn
 ." playing as x or o? "
 begin
  character cr dup x = if drop ['] x is playing exit
  else o = if ['] o is playing exit
  else "retry" then then
 again ;

: range   over + swap ;

: full?
 board 9 range do i c@ bl = if
  unloop false exit then
 loop true ;

: win   1 state ! true rdrop rdrop ;

: ?tile   dup c@ playing <> if drop rdrop false then ;

: row?   ?tile char+ ?tile char+ ?tile drop true ;
: ?rows
 board 9 chars range do
  i row? if unloop win then
 [ 3 chars ] literal +loop ;

: +col   [ 3 chars ] literal + ;
: col?   ?tile +col ?tile +col ?tile drop true ;
: ?cols
 board 3 chars range do
  i col? if unloop win then
 [ 1 chars ] literal +loop ;

: +diag1   [ 4 chars ] literal + ;
: diag1?   board ?tile +diag1 ?tile +diag1 ?tile drop true ;

: +diag2   [ 2 chars ] literal + ;
: diag2?   board +diag2 ?tile +diag2 ?tile +diag2 ?tile drop true ;

: ?diags
 diag1? if win then
 diag2? if win then ;

: over? ?rows ?cols ?diags full? ;

: pipe   ."  | " ;
: sym   dup c@ emit char+ ;
: row   space sym pipe sym pipe sym cr ;
: bar   ." ---+---+---" cr ;
: draw   page board row bar row bar row drop ;

: play
 begin
  draw
  take-turn
 over? 0= while
  turn
 repeat ;

: "win"   playing emit ."  won." cr ;
: "tie"   ." it's a tie." cr ;

: game
 0 state !
 clear
 choose-turn
 play
 draw
 state @ if "win" else "tie" then
 ." type GAME to play again, or BYE to quit." cr ;
