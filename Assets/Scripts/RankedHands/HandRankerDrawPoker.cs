using System;

public class HandRankerDrawPoker
{
	public HandRankerDrawPoker()
	{
        // empty
	}

    public static int GetHandRank( Hand pHand1, Hand pHand2 )
    {
        int tCompareResults;

        // Straight Flush
        tCompareResults = Hand.CompareRankedHands( new HandStraightFlush( pHand1.Cards ), new HandStraightFlush( pHand2.Cards ) );
        if ( tCompareResults > -2 )
        {
            return tCompareResults;
        };

        // Four Of A Kind
        tCompareResults = Hand.CompareRankedHands( new HandFourOfAKind( pHand1.Cards ), new HandFourOfAKind( pHand2.Cards ) );
        if ( tCompareResults > -2 )
        {
            return tCompareResults;
        };

        // Full House
        tCompareResults = Hand.CompareRankedHands( new HandFullHouse( pHand1.Cards ), new HandFullHouse( pHand2.Cards ) );
        if ( tCompareResults > -2 )
        {
            return tCompareResults;
        };

        // Flush
        tCompareResults = Hand.CompareRankedHands( new HandFlush( pHand1.Cards ), new HandFlush( pHand2.Cards ) );
        if ( tCompareResults > -2 )
        {
            return tCompareResults;
        };

        // Straight
        tCompareResults = Hand.CompareRankedHands( new HandStraight( pHand1.Cards ), new HandStraight( pHand2.Cards ) );
        if ( tCompareResults > -2 )
        {
            return tCompareResults;
        };

        // Three Of A Kind
        tCompareResults = Hand.CompareRankedHands( new HandThreeOfAKind( pHand1.Cards ), new HandThreeOfAKind( pHand2.Cards ) );
        if ( tCompareResults > -2 )
        {
            return tCompareResults;
        };

        // Two Pair
        tCompareResults = Hand.CompareRankedHands( new HandTwoPair( pHand1.Cards ), new HandTwoPair( pHand2.Cards ) );
        if ( tCompareResults > -2 )
        {
            return tCompareResults;
        };

        // One Pair
        tCompareResults = Hand.CompareRankedHands( new HandOnePair( pHand1.Cards ), new HandOnePair( pHand2.Cards ) );
        if ( tCompareResults > -2 )
        {
            return tCompareResults;
        };

        // High Card
        tCompareResults = Hand.CompareRankedHands( new HandHighCard( pHand1.Cards ), new HandHighCard( pHand2.Cards ) );
        
        // Manual manipulation for the case of No Winner (-2 converted to Tie (0) )
        if ( tCompareResults == -2 )
        {
            tCompareResults = 0;
        }

        return tCompareResults;
    }

}
