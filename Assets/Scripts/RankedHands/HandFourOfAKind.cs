using System.Collections.Generic;

public class HandFourOfAKind : BaseRankedHand
{
    public HandFourOfAKind( List<Card> pHand ) : base( pHand )
    {
        // empty
    }

    override public bool IsMade()
    {
        if ( GetMadeNumericRank() >= 0 )
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
        DuplicateRankCounter tDuplicateRankCounter = GetDuplicateCards( 0, 4 );

        if ( tDuplicateRankCounter.duplicateCardNumericRank >= 0 )
        {
            _usedCardMask[ tDuplicateRankCounter.duplicateCardLastIndex - 3 ] = true;
            _usedCardMask[ tDuplicateRankCounter.duplicateCardLastIndex - 2 ] = true;
            _usedCardMask[ tDuplicateRankCounter.duplicateCardLastIndex - 1 ] = true;
            _usedCardMask[ tDuplicateRankCounter.duplicateCardLastIndex ] = true;
        }

        return tDuplicateRankCounter.duplicateCardNumericRank;
    }

    public override string SId
    {
        get { return "Four of a Kind"; }
    }

    public override int PayoffMultiplier
    {
        get { return 25; }
    }

}
