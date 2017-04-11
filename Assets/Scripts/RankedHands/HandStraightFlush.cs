using System.Collections.Generic;

public class HandStraightFlush : BaseRankedHand
{
    public HandStraightFlush( List<Card> pHand ) : base( pHand )
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
        if( !IsFlush() )
        {
            // each suit is not represented
            return -1;
        }
        
        if( IsStraight() )
        {
            // There's one or more of each suit and at least 1 pair
            // First in the list will be the high rank

            SortHand();
            SetAllUsedCardMaskTrue();

            return _cards[0].NumericRank;
        }

        if ( IsAcesLowStraight() )
        {
            // There's one or more of each suit and at least 1 pair
            // First in the list will be the high rank
            SortHandAceLow();
            SetAllUsedCardMaskTrue();

            return _cards[0].AceLowNumericRank;
        }

        return -1;
    }

    public override string SId
    {
        get { return "Straight Flush"; }
    }

    public override int PayoffMultiplier
    {
        get { return 50; }
    }

}
