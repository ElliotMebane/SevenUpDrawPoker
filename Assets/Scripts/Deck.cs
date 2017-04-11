using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<Card> _cards;
    private List<Card> _allCards;
    private List<Card> _allSevenUpCards;
    private string _GOPathBase = "Free_Playing_Cards/PlayingCards_";

    // prepend invalid entries as needed so 0-based indexing for rank will match 1-13 card face values, for convenience and legibility
    // This is a traditional 52-card deck
    public static string[] drawRanks = new string[] { null, null, "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
    public static string[] drawRanksAceLow = new string[] { null, "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    // 7-up, Manila Deck: http://www.pokerpistols.com/wop_manillapoker.php
    public static string[] sevenUpRanks = new string[] { null, null, "7", "8", "9", "10", "J", "Q", "K", "A" };
    public static string[] sevenUpRanksAceLow = new string[] { null, "A", "7", "8", "9", "10", "J", "Q", "K" };

    public static string[] ranks = drawRanks;
    public static string[] ranksAceLow = drawRanksAceLow;

    public static string[] suitsFullName = new string[] { "Heart", "Spades", "Club", "Diamond" };
    public static string[] suits = new string[] { "H", "S", "C", "D" };
    
    public Deck( Vector3 pScale )
    {
        _cards = new List<Card>();
        _allCards = new List<Card>();
        _allSevenUpCards = new List<Card>();
        Vector3 tScale = pScale;

        int iLen = suits.Length;
        for ( int i = 0; i < iLen; i++ )
        {
            int jLen;

            jLen = drawRanks.Length;
            // start with 2 to avoid the null placeholder starting entries
            for ( int j = 2; j < jLen; j++ )
            {
                string tResourceString = _GOPathBase + drawRanks[ j ] + suitsFullName[ i ];
                GameObject tCardGO = Resources.Load<GameObject>( tResourceString ) as GameObject;

                Card tCard = new Card( drawRanks[ j ], suits[ i ], suitsFullName[ i ], tCardGO, pScale );
                _cards.Add( tCard );
                _allCards.Add( tCard );
            }
        }

        // SevenUp cards are a subset so find them in the primary _cards List. 
        iLen = suits.Length;
        for ( int i = 0; i < iLen; i++ )
        {
            int jLen;

            jLen = sevenUpRanks.Length;
            // start with 2 to avoid the null placeholder starting entries
            for ( int j = 2; j < jLen; j++ )
            {
                Card tCard = _cards[ GetCardIndex( sevenUpRanks[ j ], suits[ i ] ) ];

                _allSevenUpCards.Add( tCard );
            }
        }
    }

    public Deck Refresh( GameType pGameType )
    {
        _cards.Clear();

        if ( pGameType == GameType.draw )
        {
            foreach ( Card tCard in _allCards )
            {
                _cards.Add( tCard );
            }
        }
        else if ( pGameType == GameType.sevenUp )
        {
            foreach ( Card tCard in _allSevenUpCards )
            {
                _cards.Add( tCard );
            }
        }

        return this;
    }

    public Deck Shuffle()
    {
        Card tHolder;
        //int tInsertion;
        int kLen = _cards.Count;
        for ( int k = 0; k < kLen; k++ )
        {
            int tExtractionIndex = Random.Range( k, kLen );
            tHolder = _cards[k];
            _cards[k] = _cards[tExtractionIndex];
            _cards[tExtractionIndex] = tHolder;
        }

        return this;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="pHand">
    /// SEVEN-UP
    /// String representing hand should be comma-delimited, rank then suit.
    /// No spaces (including trailing spaces), only choose cards from 7-A.
    /// No Data Validation is performed
    /// ex: AH,KH,QH,JH,10H</param></param>
    /// <returns></returns>
    private Hand GetHandFromHandString( string pHand )
    {
        Hand tHand = new Hand();
        string[] tCardStrings = pHand.ToUpper().Split( ',' );
        foreach( string tCardString in tCardStrings )
        {
            string tRank;
            string tSuit;
            if ( tCardString.Length == 3 )
            {
                // special case for 10. String will be 3 characters long.
                tRank = tCardString.Substring( 0, 2 );
                tSuit = tCardString.Substring( 2, 1 );
            }
            else
            {
                tRank = tCardString.Substring( 0, 1 );
                tSuit = tCardString.Substring( 1, 1 );
            }

            tHand.AddCard( DealCard( tRank, tSuit ) );
        }

        return tHand;
    }

    public Hand DealHand( DebugHands pDebugHand = 0, string pHand = null )
    {
        Hand tHand = new Hand();

        switch( pDebugHand )
        {
            case DebugHands.NormalHandDeal:
                for ( int i = 0; i < 5; i++ )
                {
                    tHand.AddCard( DealCard() );
                }
                break;
            case DebugHands.CommaDelimitedHand:
                tHand = GetHandFromHandString( pHand );
                break;
            default:
                break;
        }
    
        return tHand;
    }

    public Card DealCard( string pRank = null, string pSuit = null )
    {
        if ( _cards.Count == 1 )
        {
            // can't deal the last card in the deck. No cheaters!
            return null;
        }

        int tIndex;
        if ( pRank != null && pSuit != null )
        {
            tIndex = GetCardIndex( pRank, pSuit );
        }
        else
        {
            tIndex = Random.Range( 0, _cards.Count );
        }
        Card tCard = _cards[tIndex];
        _cards.RemoveAt( tIndex );

        return tCard;
    }

    public int GetCardIndex( string pRank, string pSuit )
    {
        int iLen = _cards.Count;
        Card tCard;
        for ( int i = 0; i < iLen; i++ )
        {
            tCard = _cards[i];
            if ( tCard.Rank == pRank && tCard.Suit == pSuit )
            {
                return i;
            }
        }

        return -1;
    }

    public List<Card> Cards
    {
        get
        {
            return _cards;
        }

        set
        {
            _cards = value;
        }
    }

    public List<Card> AllCards
    {
        get
        {
            return _allCards;
        }

        set
        {
            _allCards = value;
        }
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
}

public enum DebugHands
{
    NormalHandDeal,
    CommaDelimitedHand
}


