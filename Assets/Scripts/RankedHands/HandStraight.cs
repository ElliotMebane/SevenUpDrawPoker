using System.Collections.Generic;

public class HandStraight : BaseRankedHand 
{
   public HandStraight( List<Card> pHand ) : base( pHand )
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
        if( IsStraight() )
        {
            SortHand();
            SetAllUsedCardMaskTrue();

            return _cards[0].NumericRank;
        }
        else if( IsAcesLowStraight() )
        {
            SortHandAceLow();
            SetAllUsedCardMaskTrue();

            return _cards[0].AceLowNumericRank;
        }

        return -1;
    }

    public override string SId
    {
        get { return "Straight"; }
    }

    public override int PayoffMultiplier
    {
        get { return 4; }
    }

}
