using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Prime31.ZestKit; 

public class StateDeal : BaseState, IState
{
    Controller _context;
    Type _nextStateType;

    public StateDeal ( System.Object pContext, FiniteStateMachine pFSM ) : base( pContext, pFSM )
    {
        _context = _contextObject as Controller;
    }

    public override IEnumerable Execute ()
    {
        // Decrement bankroll by Ante amount
        _context.ChangeBankroll( -_context.ante );

        // Wait for deal to complete before enabling Draw/Fold buttons
        foreach ( var cur in DealHand() )
        {
            yield return cur;
        }
        
        // Enable Draw and Fold buttons
        EnableUIButtons();

        // After deal choose cards to keep. 
        // When user is done they will advance the FSM which sets 
        // _nextState for use in the call to SwitchState below
        IEnumerator tEnumerator = SelectCards().GetEnumerator();
        while( _stateInternalState == StateInternalStates.Execute  )
        {
            tEnumerator.MoveNext();

            yield return null;
        }
      
        _FSM.OnStateExitComplete();
    }

    private IEnumerable DealHand ()
    {
        Deck tDeck = GetFullDeck().Shuffle();
        
        /////////////
        // Player & Dealer interlaced/alternating deal
        /////////////
        
        if( _context.playerHandType == DebugHands.CommaDelimitedHand )
        {
            _context.playerHand = _context.deck.DealHand( _context.playerHandType, _context.playerHandString );
        }
        else
        {
            _context.playerHand = _context.deck.DealHand( _context.playerHandType );
        }

        if ( _context.dealerHandType == DebugHands.CommaDelimitedHand )
        {
            _context.dealerHand = _context.deck.DealHand( _context.dealerHandType, _context.dealerHandString );
        }
        else
        {
            _context.dealerHand = _context.deck.DealHand( _context.dealerHandType );
        }
        // Debug.LogFormat("Player Dealt Hand: {0}", _context.playerHand);
        // Debug.LogFormat( "Dealer Dealt Hand: {0} ", _context.dealerHand );

        Card tCard;
        int iLen;
        int i;
        List<Card> tCards = _context.playerHand.Cards;
        iLen = tCards.Count;
        for (i = 0; i < iLen; i++)
        {
            tCard = _context.playerHand.Cards[i];
            GameObject tCardGO = tCard.InstantiatedGO;
            tCardGO.transform.parent = _context.table.transform;
            tCardGO.transform.rotation = _context.cardRotationFaceDown;
            tCardGO.SetActive( true );

            Vector3 tPositionTarget = _context.defaultCardLocalPosition + new Vector3( Controller.cardWidth * i, -Controller.cardHeight * 2, 0 );

            // Set position to pre-tween location
            tCardGO.transform.localPosition = _context.deckLocalPosition;
            tCardGO.transform.rotation = _context.cardRotationFaceDown;

            tCardGO.transform.ZKlocalPositionTo( tPositionTarget, Controller.animationTimeSlow ).setEaseType( EaseType.QuintOut ).start();
            tCardGO.transform.ZKlocalRotationTo( _context.cardRotationFaceUp, Controller.animationTimeSlow ).setEaseType( EaseType.QuintOut ).start();

            // Cascade entry
            yield return new WaitForSeconds( Controller.animationWaitSlow );

            // Alternate deal cards with Dealer
            tCard = _context.dealerHand.Cards[ i ];
            tCardGO = tCard.InstantiatedGO;
            tCardGO.transform.parent = _context.table.transform;
            tCardGO.transform.rotation = _context.cardRotationFaceDown;
            tCardGO.SetActive( true );

            tPositionTarget = _context.defaultCardLocalPosition + new Vector3( Controller.cardWidth * i, 0, 0 );

            // Set position to pre-tween location
            tCardGO.transform.localPosition = _context.deckLocalPosition;

            tCardGO.transform.ZKlocalPositionTo( tPositionTarget, Controller.animationTimeSlow ).setEaseType( EaseType.QuintOut ).start();

            // Cascade entry
            yield return new WaitForSeconds( Controller.animationWaitSlow );
        }
        
        _context.textFieldConsole.GetComponent<Text>().text = _context.textInstuctionsDiscard;
    }

    private IEnumerable SelectCards ()
    {
        while( true )
        {
            if (Input.GetMouseButtonDown( 0 ))
            {
                Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
                RaycastHit hit;
                bool tHit = Physics.Raycast( ray, out hit );
                if ( tHit )
                {
                    if ( hit.collider.transform.tag == "card" )
                    {
                        // verify the player clicked a card in the player hand
                        Card tCard = _context.playerHand.SearchForCardByTransform( hit.collider.transform );
                        if (tCard != null)
                        {
                            // Clicked Card is in the Player's Hand
                            _context.playerHand.ChooseCard( tCard );

                            float tNewX = _context.defaultCardLocalPosition.x + ( _context.playerHand.ChosenCards.Count - 1 ) * Controller.cardWidth;
                            float tNewY = _context.defaultCardLocalPosition.y - Controller.cardHeight * 3;
                            float tNewZ = _context.defaultCardLocalPosition.z;
                            Vector3 tPositionTarget = new Vector3( tNewX, tNewY, tNewZ );

                            hit.collider.transform.ZKlocalPositionTo( tPositionTarget, Controller.animationTimeSlow ).setEaseType( EaseType.QuintOut ).start();
                        }
                    }
                }
            }

            yield return null;
        }
    }

    private void EnableUIButtons( bool pEnable = true )
    {
        if( pEnable )
        {
            _context.buttonDraw.GetComponent<Button>().onClick.AddListener( OnDrawClicked );
            _context.buttonFold.GetComponent<Button>().onClick.AddListener( OnFoldClicked );
        }
        else
        {
            _context.buttonDraw.GetComponent<Button>().onClick.RemoveAllListeners();
            _context.buttonFold.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void OnDrawClicked ()
    {
        _context.textFieldConsole.GetComponent<Text>().text = "";

        _context.ValidateBetEntry();

        _context.ChangeBankroll( -_context.betThisHand );

        _FSM.SetNextState( typeof( StateDraw ), true );
    }

    public override void BeginExit()
    {
        _context.textFieldConsole.GetComponent<Text>().text = "";

        EnableUIButtons( false );
    }

    public void OnFoldClicked ()
    {
        _FSM.SetNextState( typeof( StateConcludeGame ), true );
    }

    public void ReUpBankroll ()
    {
        if (_context.bankroll <= _context.minimumBet + _context.ante)
        {
            _context.bankroll = _context.refreshBankroll;
        }
    }

    private Deck GetFullDeck()
    {
        if ( _context.deck == null )
        {
            _context.deck = new Deck( _context.defaultCardScale );
        }

        return _context.deck.Refresh( _context.gameType );
    }

}
