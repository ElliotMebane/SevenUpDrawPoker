using System;
using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    private List<Card> _cards;
    private List<Card> _chosenCards;
    private List<Card> _discardCards;

    public BaseRankedHand highestMadeRankedHand;

    public Hand()
	{
        _cards = new List<Card>();
        _chosenCards = new List<Card>();
        _discardCards = new List<Card>();
    }

    public void AddCard( Card pCard )
    {
        _cards.Add( pCard );
    }

    public Card AddCardToChosenCards ( Card pCard )
    {
        _chosenCards.Add ( pCard );

        return pCard;
    }

    public void ChooseCard ( Card pCard )
    {
        if (_cards.Remove( pCard ))
        {
            _chosenCards.Add( pCard );
        }
    }

    public void SetDiscardCards ()
    {
        // move remaining cards from original hand to the discard hand for animation 
        int iLen = _cards.Count;
        for ( int i = 0; i < iLen; i++ )
        {
            _discardCards.Add( _cards[i] );
        }

        _cards.Clear();
    }

    // move the chosen cards into the primary cards list for final hand ranking
    public void SetFinalHand()
    {
        _cards.Clear();

        foreach ( Card tCard in _chosenCards)
        {
             _cards.Add( tCard );
        }

        _chosenCards.Clear();
    }

    /// <summary>
    /// Static Utility method
    /// </summary>
    /// <param name="pHand1"></param>
    /// <param name="pHand2"></param>
    /// <param name="pGameType"></param>
    /// <returns>
    /// -1, pHand1 wins
    /// 0, tie
    /// 1, pHand2 wins
    /// </returns>
    public static int CompareHands( Hand pHand1, Hand pHand2, GameType pGameType )
    {
        if( pGameType == GameType.draw )
        {
            return HandRankerDrawPoker.GetHandRank( pHand1, pHand2 );
        }
        else if ( pGameType == GameType.sevenUp )
        {
            return HandRankerSevenUpDrawPoker.GetHandRank( pHand1, pHand2 );
        }

        return 0;
    }

    /// <summary>
    /// Static Utility method
    /// </summary>
    /// <param name="pRankedHand1"></param>
    /// <param name="pRankedHand2"></param>
    /// <returns>     
    /// return -2 if no winner
    /// return -1 if 1 wins
    /// return 0 if tie
    /// return 1 if 2 wins
    /// </returns>
    public static int CompareRankedHands( BaseRankedHand pRankedHand1, BaseRankedHand pRankedHand2 )
    {
        bool tIsHand1Made = pRankedHand1.IsMade();
        bool tIsHand2Made = pRankedHand2.IsMade();    

        if ( tIsHand1Made && tIsHand2Made )
        {
            // Determine if one it's a tie or one of them is the winner
            return pRankedHand1.CompareSameRankHand( pRankedHand2 );
        } 
        else if ( tIsHand1Made )
        {
            return -1;
        }
        else if ( tIsHand2Made)
        {
            return 1;
        };

        // no winner
        return -2;
    }

    public BaseRankedHand GetHighestMadeRankedHand()
    {
        highestMadeRankedHand = new HandStraightFlush( _cards );
        if (highestMadeRankedHand.IsMade() )
        {
            return highestMadeRankedHand;
        }

        highestMadeRankedHand = new HandFourOfAKind( _cards );
        if ( highestMadeRankedHand.IsMade() )
        {
            return highestMadeRankedHand;
        }

        // SEVEN-UP
        // Modified for Seven-Up. Flush beats Full House. 
        highestMadeRankedHand = new HandFlush( _cards );
        if ( highestMadeRankedHand.IsMade() )
        {
            return highestMadeRankedHand;
        }

        highestMadeRankedHand = new HandFullHouse( _cards );
        if ( highestMadeRankedHand.IsMade() )
        {
            return highestMadeRankedHand;
        }
       
        highestMadeRankedHand = new HandStraight( _cards );
        if ( highestMadeRankedHand.IsMade() )
        {
            return highestMadeRankedHand;
        }
        
        highestMadeRankedHand = new HandThreeOfAKind( _cards );
        if ( highestMadeRankedHand.IsMade() )
        {
            return highestMadeRankedHand;
        }
       
        highestMadeRankedHand = new HandTwoPair( _cards );
        if ( highestMadeRankedHand.IsMade() )
        {
            return highestMadeRankedHand;
        }

        highestMadeRankedHand = new HandOnePair( _cards );
        if ( highestMadeRankedHand.IsMade() )
        {
            return highestMadeRankedHand;
        }
       
        highestMadeRankedHand = new HandHighCard( _cards );

        return highestMadeRankedHand;
    }

    public Card SearchForCardByTransform ( Transform tTransform )
    {
        foreach ( Card tCard in _cards )
        {
            if ( tCard.InstantiatedGO.transform == tTransform )
            {
                return tCard;
            }
        }

        return null;
    }

    public List<Card> Cards
    {
        get { return _cards; }
    }

    public List<Card> ChosenCards
    {
        get
        {
            return _chosenCards;
        }

        set
        {
            _chosenCards = value;
        }
    }

    public List<Card> DiscardCards
    {
        get
        {
            return _discardCards;
        }

        set
        {
            _discardCards = value;
        }
    }

    public override string ToString()
    {
        string tReturn = "";
        foreach ( Card tCard in _cards )
        {
            tReturn += tCard.ToString() + ", ";
        }
        tReturn += GetHighestMadeRankedHand().SId;

        return tReturn;
    }
}
