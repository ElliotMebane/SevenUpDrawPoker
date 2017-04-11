using System.Collections.Generic;

public class HandFullHouse : BaseRankedHand
{
   public HandFullHouse(List<Card> pHand) : base(pHand)
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
        DuplicateRankCounter tDuplicateRankCounter = GetDuplicateCards( 0, 3 );

        // a 5-card hand. will set all card masks true in GetMade2NumericRank
        return tDuplicateRankCounter.duplicateCardNumericRank;
    }

    override public int GetMade2NumericRank()
    {
        // find the first set of 3 then exclude that rank from the following search for a pair.
        DuplicateRankCounter tThreeOfAKind = GetDuplicateCards( 0, 3 );

        // a 5-card hand
        SetAllUsedCardMaskTrue();

        return GetDuplicateCards( 0, 2, tThreeOfAKind.duplicateCardNumericRank ).duplicateCardNumericRank;
    }

    public override string SId
    {
        get { return "Full House"; }
    }

    // SEVEN-UP
    // Modified for Seven-Up rules. Flush beats Full House.
    public override int PayoffMultiplier
    {
        get { return 5; }
    }

}
