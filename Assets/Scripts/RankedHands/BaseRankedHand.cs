using System;
using System.Collections.Generic;

public class BaseRankedHand
{
    protected List<Card> _cards;
    // Masking identifies which cards in _usedCardMask have been used for Ranked Value comparisons (including made, secondary made, and kickers)
    // _usedCardMask will be reset upon sorting
    protected bool[] _usedCardMask; 

    public BaseRankedHand( List<Card> pHand )
    {
        _cards = new List<Card>();
  
        foreach( Card tCard in pHand )
        {
            _cards.Add( tCard );
        }

        SortHand();
    }

    protected virtual void SortHand()
    {
        _cards.Sort( SortAlgorithm );

        // Reset _usedCardMask whenever we sort
        ResetUsedCardMask();
    }

    protected virtual int SortAlgorithm( Card pCard0, Card pCard1 )
    {
        int tCard0NumericRank = pCard0.NumericRank;
        int tCard1NumericRank = pCard1.NumericRank;

        if ( tCard0NumericRank < tCard1NumericRank )
        {
            return 1;
        }
        else if ( tCard0NumericRank > tCard1NumericRank )
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    protected virtual void SortHandAceLow()
    {
        _cards.Sort( SortAceLowAlgorithm );

        // Reset _usedCardMask whenever we sort
        ResetUsedCardMask();
    }

    protected virtual int SortAceLowAlgorithm( Card pCard0, Card pCard1 )
    {
        int tCard0NumericRank = pCard0.AceLowNumericRank;
        int tCard1NumericRank = pCard1.AceLowNumericRank;

        if ( tCard0NumericRank < tCard1NumericRank )
        {
            return 1;
        }
        else if ( tCard0NumericRank > tCard1NumericRank )
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    public virtual bool IsMade()
    {
        return false;
    }
    
    public List<Card> GetCards()
    {
        return _cards;
    }

    public virtual int GetMadeNumericRank()
    {
        return -1;
    }

    public virtual int GetMade2NumericRank()
    {
        return -1;
    }
    
    protected bool IsFlush()
    {
        SortHand();

        string tSuit = _cards[0].Suit;
        for ( int i = 1; i < 5; i++ )
        {
            if ( _cards[ i ].Suit != tSuit )
            {
                return false;
            }
        }

        return true;
    }

    protected bool IsStraight()
    {
        SortHand();

        int tNumericRankLead;
        int tNumericRankFollow;
        for ( int i = 0; i < 4; i++ )
        {
            tNumericRankLead = _cards[i].NumericRank;
            tNumericRankFollow = _cards[i + 1].NumericRank;
            if( tNumericRankLead - tNumericRankFollow != 1 )
            {
                return false;
            }
        }

        return true;
    }

    protected bool IsAcesLowStraight()
    {
        SortHandAceLow();

        int tNumericRankLead;
        int tNumericRankFollow;
        for (int i = 0; i < 4; i++)
        {
            tNumericRankLead = _cards[i].AceLowNumericRank;
            tNumericRankFollow = _cards[i + 1].AceLowNumericRank;
            if ( tNumericRankLead - tNumericRankFollow != 1 )
            {
                return false;
            }
        }

        return true;
    }

    // pExcludeRank is used for preventing the full house check from using the set of 3.
    protected DuplicateRankCounter GetDuplicateCards( int pStartIndex, int pLength, int pExcludeRank = -1 )
    {
        int[] tRankCounter = new int[15] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        DuplicateRankCounter tDuplicateRankCounter = new DuplicateRankCounter();
        for ( int i = pStartIndex; i < 5; i++ )
        {
            int tNumericRank = _cards[i].NumericRank;
            tRankCounter[tNumericRank]++;
             
            if ( tRankCounter[tNumericRank] == pLength && tNumericRank != pExcludeRank )
            {
                tDuplicateRankCounter.duplicateCardNumericRank = tNumericRank;
                tDuplicateRankCounter.duplicateCardCount = tRankCounter[tNumericRank];
                tDuplicateRankCounter.duplicateCardLastIndex = i;

                return tDuplicateRankCounter;
            };
        }
        return tDuplicateRankCounter;
    }

    protected void ResetUsedCardMask()
    {
        _usedCardMask = new bool[5] { false, false, false, false, false };
    }

    protected void SetAllUsedCardMaskTrue()
    {
        _usedCardMask = new bool[5] { true, true, true, true, true };
    }

    protected int GetNextKickerValue( bool pUpdateUsedCardMask = true )
    {
        for( int i = 0; i < 5; i++ )
        {
            if( !_usedCardMask[i] )
            {
                if( pUpdateUsedCardMask )
                {
                    _usedCardMask[i] = true;

                    return _cards[i].NumericRank;
                }
            }
        }

        return -1;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="pOpponentHand"></param>
    /// <returns> 
    /// -1 if this Hand wins
    /// 1 if pOpponentHand wins
    /// </returns>
    public int CompareSameRankHand( BaseRankedHand pOpponentHand )
    {
        int tHand1;
        int tHand2;

        // Primary Made
        tHand1 = GetMadeNumericRank();
        tHand2 = pOpponentHand.GetMadeNumericRank();
        if ( tHand1 > tHand2 )
        {
            return -1;
        } 
        else if (tHand1 < tHand2)
        {
            return 1;
        }

        // Secondary Made
        tHand1 = GetMade2NumericRank();
        tHand2 = pOpponentHand.GetMade2NumericRank();
        if ( tHand1 > tHand2 )
        {
            return -1;
        }
        else if ( tHand1 < tHand2 )
        {
            return 1;
        }

        // Kickers
        for (int i = 0; i < 4; i++)
        {
            tHand1 = GetNextKickerValue();
            tHand2 = pOpponentHand.GetNextKickerValue();
            if ( tHand1 > tHand2 )
            {
                return -1;
            }
            else if ( tHand1 < tHand2 )
            {
                return 1;
            }
        }
        
        // Couldn't find a winner! Return tie. :(
        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>
    /// A List of kickers without disturbing _usedCardMask
    /// </returns>
    public virtual List<Card> GetKickerList()
    {
        List<Card> tKickers = new List<Card>();

        GetMadeNumericRank();
        GetMade2NumericRank();

        // Kickers
    
        for (int i = 0; i < 5; i++)
        {
            if ( !_usedCardMask[i] )
            {
                tKickers.Add( _cards[i] );
            }
        }
        
        return tKickers;
    }

    public List<Card> GetMadeList ()
    {
        List<Card> tMadeList = new List<Card>();

        GetMadeNumericRank();
        GetMade2NumericRank();

        for (int i = 0; i < 5; i++)
        {
            if ( _usedCardMask[i] )
            {
                tMadeList.Add( _cards[i] );
            }
        }

        return tMadeList;
    }

    public string GetMadeRankValues()
    {
        return String.Format( "{0}, {1}, {2}, {3}, {4}, {5}", 
            GetMadeNumericRank(), 
            GetMade2NumericRank(),
            GetNextKickerValue(),
            GetNextKickerValue(),
            GetNextKickerValue(),
            GetNextKickerValue() );      
    }

    public override string ToString()
    {
        string tReturn = "";
        foreach ( Card tCard in _cards )
        {
            tReturn += tCard.ToString() + ", ";
        }

        return tReturn;
    }

    public virtual string SId
    {
        get{ return "[BaseRankedHand] sId"; }
    }

    public virtual int PayoffMultiplier
    {
        get { return 0; }
    }

}

public class DuplicateRankCounter
{
    public int duplicateCardNumericRank = -1;
    public int duplicateCardCount = -1;
    public int duplicateCardLastIndex = -1;
}
