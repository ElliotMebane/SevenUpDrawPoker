﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateInitialize : BaseState
{
    Controller _context;
    Type _nextStateType;

	public StateInitialize ()
    {
        // empty
    }

    public override void Init( FiniteStateMachine pFSM )
    {
        base.Init( pFSM );

        _context = _FSMContext as Controller;
    }

    public override IEnumerable Execute ()
    {
        // SEVEN-UP
        SetGameType( GameType.sevenUp );

        _context.ValidateBetEntry();

        ReUpBankroll();
        _context.UpdateBankrollDisplay();

        EnableUIButtons();

        while ( _stateInternalState == StateInternalStates.Execute )
        {
            yield return null;
        }

        _FSM.OnStateExitComplete();
    }

    public override void BeginExit()
    {
        _context.textFieldConsole.GetComponent<Text>().text = "";

        EnableUIButtons( false );
    }

    public void OnDealClicked ()
    {
        _FSM.SetNextState( typeof( StateDeal ), true );
    }

    /// <summary>
    /// SEVEN-UP
    /// Use to set game type to either Draw Poker or Seven-Up Draw Poker. 
    /// Not currently in use. This sample is a Seven-Up Draw Poker game only.
    /// Additional requirements for switching to Draw Poker include switching the Payoff Multipliers,  
    /// the instructions for Flush/Full House hand rank difference, and payoff values
    /// </summary>
    /// <param name="pGameType"></param>
    public void SetGameType( GameType pGameType )
    {
        switch ( pGameType )
        {
            case GameType.sevenUp:
                {
                    _context.gameType = GameType.sevenUp;

                    Deck.ranks = Deck.sevenUpRanks;
                    Deck.ranksAceLow = Deck.sevenUpRanksAceLow;

                    _context.textFieldTitle.GetComponent<Text>().text = "Seven-Up Draw Poker";
                    _context.textFieldConsole.GetComponent<Text>().text = _context.sevenUpDrawPokerInstructions;
                }
                break;
            case GameType.draw:
                {
                    _context.gameType = GameType.draw;

                    Deck.ranks = Deck.drawRanks;
                    Deck.ranksAceLow = Deck.drawRanksAceLow;

                    _context.textFieldTitle.GetComponent<Text>().text = "Draw Poker";
                    _context.textFieldConsole.GetComponent<Text>().text = _context.drawPokerInstructions;
                }
                break;
            default:
                break;
        }
    }
    
    public void ReUpBankroll ()
    {
        if ( _context.bankroll <= _context.minimumBet + _context.ante )
        {
            _context.bankroll = _context.refreshBankroll;
        }
    }

    private void EnableUIButtons ( bool pEnable = true )
    {
        if (pEnable)
        {
            _context.buttonDeal.GetComponent<Button>().onClick.AddListener( OnDealClicked );
        }
        else
        {
            _context.buttonDeal.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

}
