using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Prime31.ZestKit; 

public class StateDraw : BaseState, IState
{
    Controller _context;
    Type _nextStateType;

    public StateDraw ( System.Object pContext, FiniteStateMachine pFSM ) : base( pContext, pFSM )
    {
        _context = _contextObject as Controller;
    }

    public override IEnumerable Execute ()
    {
        // Animate cards into place
        foreach ( var cur in DrawCards() )
        {
            yield return cur;
        }

        // Enable Draw and Fold buttons
        EnableUIButtons();

        // Wait for user input
        while (_stateInternalState == StateInternalStates.Execute)
        {
            yield return null;
        }
        
        _FSM.OnStateExitComplete();
    }

    private void EnableUIButtons ( bool pEnable = true )
    {
        if ( pEnable )
        {
            _context.buttonShowdown.GetComponent<Button>().onClick.AddListener( OnShowdownClicked );
        }
        else
        {
            _context.buttonShowdown.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void OnShowdownClicked ()
    {
        _context.textFieldConsole.GetComponent<Text>().text = "";

        _FSM.SetNextState( typeof( StateShowdown ), true );
    }

    private IEnumerable DrawCards ()
    {
        ////////////////
        // Player Draw 
        ////////////////

        int i;
        int iLen;
        Card tCard;
        float tNewX;
        float tNewY;
        float tNewZ;

        // Set and display discard info
        _context.playerHand.SetDiscardCards();

        // Draw new cards to fill hand
        List<Card> tPlayerChosenCards = _context.playerHand.ChosenCards;
        iLen = 5;
        for ( i = tPlayerChosenCards.Count; i < iLen; i++ )
        {
            tCard = _context.playerHand.AddCardToChosenCards( _context.deck.DealCard() );

            GameObject tCardGO = tCard.InstantiatedGO;
            tCardGO.transform.parent = _context.table.transform;
            
            tNewX = _context.defaultCardLocalPosition.x + ( _context.playerHand.ChosenCards.Count - 1 ) * Controller.cardWidth;
            tNewY = _context.defaultCardLocalPosition.y - Controller.cardHeight * 3;
            tNewZ = _context.defaultCardLocalPosition.z;
            Vector3 tPositionTarget = new Vector3( tNewX, tNewY, tNewZ );

            // Set position and rotation to pre-tween location
            tCardGO.transform.localPosition = _context.deckLocalPosition;
            tCardGO.transform.rotation = _context.cardRotationFaceDown;

            // Cascade entry    
            tCardGO.SetActive( true );
            tCard.InstantiatedGO.transform.ZKlocalPositionTo( tPositionTarget, Controller.animationTimeSlow ).setEaseType( EaseType.QuintOut ).start();
            tCard.InstantiatedGO.transform.ZKlocalRotationTo( _context.cardRotationFaceUp, Controller.animationTimeSlow ).setEaseType( EaseType.QuintOut ).start();

            yield return new WaitForSeconds( Controller.animationWaitSlow );
        }

        ////////////////
        // Player Move Discards
        ////////////////

        // Move discarded cards the left
        iLen = _context.playerHand.DiscardCards.Count;  
        for ( i = 0; i < iLen; i++ )
        {
            tCard = _context.playerHand.DiscardCards[ i ];

            tNewX = _context.defaultCardLocalPosition.x + Controller.cardWidth * i;
            tNewY = tCard.InstantiatedGO.transform.localPosition.y;
            tNewZ = tCard.InstantiatedGO.transform.localPosition.z;
            Vector3 tPositionTarget = new Vector3( tNewX, tNewY, tNewZ );

            tCard.InstantiatedGO.transform.ZKlocalPositionTo( tPositionTarget, Controller.animationTimeQuick ).setEaseType( EaseType.QuintOut ).start();

            yield return new WaitForSeconds( Controller.animationWaitSlow );
        }

        ////////////////
        // Dealer Draw
        ////////////////

        // Choose any Made cards, similar to the way player chose cards one by one
        BaseRankedHand tDealerRankedHand = _context.dealerHand.GetHighestMadeRankedHand();
        List<Card> tDealerHandCards = _context.dealerHand.Cards;

        // If a hand was specified for the dealer in the Inspector
        // keep all cards so it can be tested against the player's chosen hand
        List<Card> tDealerMadeList;
        if ( _context.dealerHandType == DebugHands.NormalHandDeal )
        {
            tDealerMadeList = tDealerRankedHand.GetMadeList();
        }
        else
        {
            tDealerMadeList = new List<Card>();
            foreach( Card tDealerCard in _context.dealerHand.Cards )
            {
                tDealerMadeList.Add( tDealerCard );
            }
        }

        iLen = tDealerMadeList.Count;
        for ( i = 0; i < iLen; i++ )
        {
            // Simulated Clicked Card is in the Dealer's Hand
            tCard = tDealerMadeList[i];
            if ( tCard != null )
            {
                _context.dealerHand.ChooseCard( tCard );

                tNewX = _context.defaultCardLocalPosition.x + ( _context.dealerHand.ChosenCards.Count - 1 ) * Controller.cardWidth;
                tNewY = _context.defaultCardLocalPosition.y - Controller.cardHeight * 1;
                tNewZ = _context.defaultCardLocalPosition.z;

                tCard.InstantiatedGO.transform.ZKlocalPositionTo( new Vector3( tNewX, tNewY, tNewZ ), Controller.animationTimeQuick ).setEaseType( EaseType.QuintOut ).start();

                yield return new WaitForSeconds( Controller.animationWaitSlow );
            }
        }

        ////////////////
        // Dealer Draw
        ////////////////

        // Choose new cards and animate into place
        List<Card> tDealerChosenCards = _context.dealerHand.ChosenCards;
        iLen = 5;
        for ( i = tDealerChosenCards.Count; i < iLen; i++ )
        {
            tCard = _context.dealerHand.AddCardToChosenCards( _context.deck.DealCard() );

            GameObject tCardGO = tCard.InstantiatedGO;
            tCardGO.transform.parent = _context.table.transform;

            tNewX = _context.defaultCardLocalPosition.x + ( _context.dealerHand.ChosenCards.Count - 1 ) * Controller.cardWidth;
            tNewY = _context.defaultCardLocalPosition.y - Controller.cardHeight;
            tNewZ = _context.defaultCardLocalPosition.z;
            Vector3 tPositionTarget = new Vector3( tNewX, tNewY, tNewZ );

            // Set position and rotation to pre-tween location
            tCardGO.transform.localPosition = _context.deckLocalPosition;
            tCardGO.transform.rotation = _context.cardRotationFaceDown;

            tCardGO.SetActive( true );

            tCard.InstantiatedGO.transform.ZKlocalPositionTo( tPositionTarget, Controller.animationTimeSlow ).setEaseType( EaseType.QuintOut ).start();

            // Cascade entry
            if ( i < iLen - 1 )
            {
                yield return new WaitForSeconds( Controller.animationWaitSlow );
            }
        }

        ////////////////
        // Dealer Move Discard Cards
        ////////////////

        // Now finalize discard choices and move un-chosen cards to the discard stack
        _context.dealerHand.SetDiscardCards();

        iLen = _context.dealerHand.DiscardCards.Count;
        for ( i = 0; i < iLen; i++ )
        {
            tCard = _context.dealerHand.DiscardCards[i];
    
            tNewX = _context.defaultCardLocalPosition.x + Controller.cardWidth * i;
            tNewY = tCard.InstantiatedGO.transform.localPosition.y;
            tNewZ = tCard.InstantiatedGO.transform.localPosition.z;
            Vector3 tPositionTarget = new Vector3( tNewX, tNewY, tNewZ );

            tCard.InstantiatedGO.transform.ZKlocalPositionTo( tPositionTarget, Controller.animationTimeQuick ).setEaseType( EaseType.QuintOut ).start();

            yield return new WaitForSeconds( Controller.animationWaitFast );
        }
    }

    public override void BeginExit ()
    {
        _context.textFieldConsole.GetComponent<Text>().text = "";

        EnableUIButtons( false );
    }
}
