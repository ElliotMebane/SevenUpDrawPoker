using System.Collections.Generic;

public class HandTwoPair : BaseRankedHand
{
    public HandTwoPair( List<Card> pHand ) : base( pHand )
    {
        // empty
    }

    override public bool IsMade()
    {
        if ( GetMadeNumericRank() >= 0 && GetMade2NumericRank() >= 0 )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    override public int GetMadeNumericRank()
    {
        DuplicateRankCounter tDuplicateRankCounter = GetDuplicateCards( 0, 2 );

        if ( tDuplicateRankCounter.duplicateCardNumericRank >= 0 )
        {
            _usedCardMask[ tDuplicateRankCounter.duplicateCardLastIndex - 1 ] = true;
            _usedCardMask[ tDuplicateRankCounter.duplicateCardLastIndex ] = true;
        }

        return tDuplicateRankCounter.duplicateCardNumericRank;
    }

    override public int GetMade2NumericRank()
    {
        // Find the first pair and its index then add 2 to pick the index to start searching for the second pair
        int tStartIndex = GetDuplicateCards(0, 2).duplicateCardLastIndex + 1;
        DuplicateRankCounter tDuplicateRankCounter = GetDuplicateCards( tStartIndex, 2 );

        if ( tDuplicateRankCounter.duplicateCardNumericRank >= 0 )
        {
            _usedCardMask[ tDuplicateRankCounter.duplicateCardLastIndex - 1 ] = true;
            _usedCardMask[ tDuplicateRankCounter.duplicateCardLastIndex ] = true;
        }

        return tDuplicateRankCounter.duplicateCardNumericRank;
    }

    public override string SId
    {
        get { return "Two Pair"; }
    }

    public override int PayoffMultiplier
    {
        get { return 2; }
    }

}
