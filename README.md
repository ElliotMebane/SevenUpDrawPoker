# SevenUpDrawPoker
Draw Poker with limited Seven Up deck. Implemented in Unity with C#

Seven-Up Poker (also known as Manila Poker) is a Poker derivative played with a 32 card deck and community cards. Cards ranked lower than 7 are not included in the deck so higher-ranked hands occur more often. Flush is harder to achive so it gets a higher rank than Full House. 

I started with simple Draw Poker then restricted the deck and hand rankings to follow the Seven-Up structure. I retained the Ace-Low straight as a valid straight (A, 7, 8, 9, 10).

This is a 1-player game with an AI dealer opponent. There is one opportunity to bet, after the deal. 

Dealer AI: any cards that are part of the highest ranked _made_ hand are kept and the kickers are discarded in an attempt to draw to a better hand. 

Seven-Up Poker description: http://www.poker.com/game/holdem-poker-games/manila.htm

Playing Card art and models used with permission of 1Poly: https://www.assetstore.unity3d.com/en/#!/content/51076

Finite State Machine derived from work of Jackson Dunstan: http://jacksondunstan.com/articles/3137

Tween Engine by Prime31: https://github.com/prime31/ZestKit
