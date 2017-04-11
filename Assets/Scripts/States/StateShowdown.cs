using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Prime31.ZestKit; 

public class StateShowdown : BaseState, IState
{
    Controller _context;
    Type _nextStateType;

    public StateShowdown ( System.Object pContext, FiniteStateMachine pFSM ) : base( pContext, pFSM )
    {
        _context = _contextObject as Controller;
    }
    public override IEnumerable Execute ()
    {
        _context.playerHand.SetFinalHand();
        _context.dealerHand.SetFinalHand();

        // Animate cards into place
        foreach (var cur in RevealCards())
        {
            yield return cur;
        }

        FindWinner();

        // Enable Clear button
        EnableUIButtons();

        // Wait for user's next input
        while (_stateInternalState == StateInternalStates.Execute)
        {
            yield return null;
        }

        _FSM.OnStateExitComplete();
    }

    private void FindWinner ()
    {
        string tShowdownOutcome = "Showdown Outcome";
        int tComparedHands = Hand.CompareHands( _context.dealerHand, _context.playerHand, _context.gameType );
        if (tComparedHands == 0)
        {
            // Tie
            // Refund bet and ante to player
            int tRefund = _context.betThisHand + _context.ante;
            _context.ChangeBankroll( tRefund );
            tShowdownOutcome = string.Format( "Tie!\nDealer: {0}\nYou: {1}\nRefund: ${2}", _context.dealerHand, _context.playerHand, tRefund );
        }
        else if (tComparedHands == -1)
        {
            tShowdownOutcome = string.Format( "Dealer wins!\nDealer: {0}\nYou: {1}", _context.dealerHand, _context.playerHand );
        }
        else if (tComparedHands == 1)
        {
            int tWinAmount = HandleWin();
            string tHandRank = _context.playerHand.GetHighestMadeRankedHand().SId;
            int tMultiplier = _context.playerHand.GetHighestMadeRankedHand().PayoffMultiplier;
            tShowdownOutcome = string.Format( "You win!\n{2} Multiplier: {3}\nYour Bet: ${4}\nYou Win: ${5}\nDealer: {0}\nYou: {1}", _context.dealerHand, _context.playerHand, tHandRank, tMultiplier, _context.betThisHand, tWinAmount );
        }

        _context.textFieldConsole.GetComponent<Text>().text = tShowdownOutcome;
    }

    private int HandleWin ()
    {
        int tWinAmount = _context.betThisHand * _context.playerHand.GetHighestMadeRankedHand().PayoffMultiplier;
        _context.ChangeBankroll( tWinAmount );

        return tWinAmount;
    }

    private void EnableUIButtons ( bool pEnable = true )
    {
        if (pEnable)
        {
            _context.buttonClear.GetComponent<Button>().onClick.AddListener( OnClearClicked );
        }
        else
        {
            _context.buttonClear.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    private void OnClearClicked ()
    {
        _context.textFieldConsole.GetComponent<Text>().text = "";

        _FSM.SetNextState( typeof( StateConcludeGame ), true );
    }

    private IEnumerable RevealCards ()
    {
        List<Card> tDealerHandCards = _context.dealerHand.Cards;

        // Reveal Hand Cards
        int i;
        int iLen = tDealerHandCards.Count;
        Card tCard;
        for ( i = 0; i < iLen; i++ )
        {
            tCard = tDealerHandCards[i];
            
            tCard.InstantiatedGO.transform.ZKlocalRotationTo( _context.cardRotationFaceUp, Controller.animationTimeQuick ).setEaseType( EaseType.QuintOut ).start();

            yield return new WaitForSeconds( Controller.animationWaitFast );
        }

        // Now Reveal the discard stack
        iLen = _context.dealerHand.DiscardCards.Count;
        for ( i = 0; i < iLen; i++ )
        {
            tCard = _context.dealerHand.DiscardCards[i];
            
            tCard.InstantiatedGO.transform.ZKlocalRotationTo( _context.cardRotationFaceUp, Controller.animationTimeQuick ).setEaseType( EaseType.QuintOut ).start();

            yield return new WaitForSeconds( Controller.animationWaitFast );
        }
    }

    public override void BeginExit ()
    {
        _context.textFieldConsole.GetComponent<Text>().text = "";

        EnableUIButtons( false );
    }

}
