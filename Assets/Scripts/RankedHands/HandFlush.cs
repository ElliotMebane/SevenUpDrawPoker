using System.Collections.Generic;

public class HandFlush : BaseRankedHand
{
    public HandFlush( List<Card> pHand ) : base( pHand )
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
        if ( !IsFlush() )
        {
            return -1;
        }

        ResetUsedCardMask();
        _usedCardMask[ 0 ] = true;

        return _cards[0].NumericRank;
    }

    public override string SId
    {
        get { return "Flush"; }
    }

    // SEVEN-UP
    // Modified for Seven-Up rules. Flush beats Full House.
    public override int PayoffMultiplier
    {
        get { return 9; }
    }

}
