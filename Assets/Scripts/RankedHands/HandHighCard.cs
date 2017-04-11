using System.Collections.Generic;

public class HandHighCard : BaseRankedHand
{
    public HandHighCard( List<Card> pHand ) : base( pHand )
	{
        // empty
	}

    override public bool IsMade()
    {
        return true;
    }

    override public int GetMadeNumericRank()
    {
        _usedCardMask[0] = true;
        // First card in sorted order is always the one we want
        return _cards[0].NumericRank;
    }

    public override string SId
    {
        get { return "High Card"; }
    }

    public override int PayoffMultiplier
    {
        get { return 0; }
    }

}
