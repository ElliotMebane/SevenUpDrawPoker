using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Text;

public class Controller : MonoBehaviour
{
    // Constants
    public const float cardWidth = 0.04f;
    public const float cardHeight = 0.055f;
    public const float animationWaitSlow = 0.2f;
    public const float animationWaitFast = 0.1f;
    public const float animationTimeQuick = 0.35f;
    public const float animationTimeSlow = 0.7f;

    // Config
    public int ante;
    public int minimumBet;
    public int maximumBet;
    public int startBankroll;
    public int refreshBankroll;
    public DebugHands dealerHandType;
    public DebugHands playerHandType;
    // No data entry validation is performed on string entries. 
    // Look out for spaces (use none), including trailing spaces. 
    public string playerHandString = "KD,KC,KS,10D,10C";
    public string dealerHandString = "AH,KH,QH,JH,9H";

    // Properties Hidden from Inspector
    // No apparent speed difference between Properties and Fields (See Dunstan comparison post), 
    // but prefer fields to limit unnecessary code bloat
    // Also, I use the Controller for Model responsibilities for simplicity since we can use the Inspector
    // for property configuration. For a larger game a better approach might be a Singleeton Model or Scriptable Object.
    [HideInInspector]
    public int bankroll;
    [HideInInspector]
    public GameType gameType = GameType.draw;
    [HideInInspector]
    public Vector3 defaultCardLocalPosition;
    [HideInInspector]
    public Quaternion cardRotationFaceDown;
    [HideInInspector]
    public Quaternion cardRotationFaceUp;
    [HideInInspector]
    public Vector3 defaultCardScale;
    [HideInInspector]
    public Vector3 deckLocalPosition;
    [HideInInspector]
    public int betThisHand;
    [HideInInspector]
    public GameObject table;
    [HideInInspector]
    public GameObject dealerHandPositionPlaceholder;
    [HideInInspector]
    public Hand playerHand;
    [HideInInspector]
    public Hand dealerHand;
    [HideInInspector]
    public Deck deck;
    [HideInInspector]
    public string drawPokerInstructions = "";
    [HideInInspector]
    public string sevenUpDrawPokerInstructions = "";
    [HideInInspector]
    public string textInstuctionsDiscard = "Click your Hold cards then\nplace your Bet and Draw!";

    // Fields
    private FiniteStateMachine _FSM;
    
    // UI
    public GameObject textFieldTitle;
    public GameObject buttonChangeGame;
    public GameObject buttonDeal;
    public GameObject buttonBet;
    public GameObject textFieldBankroll;
    public GameObject textFieldBetAmount;
    public GameObject buttonDraw;
    public GameObject buttonFold;
    public GameObject buttonShowdown;
    public GameObject buttonClear;
    public GameObject textFieldBetInput;
    public GameObject textFieldConsole;

    void Start ()
    {
        // Save the transform data of the visual-placement card
        defaultCardLocalPosition = dealerHandPositionPlaceholder.transform.localPosition;
        cardRotationFaceUp = Quaternion.Euler( 0, 180f, 0 );
        cardRotationFaceDown = Quaternion.identity;
        GameObject tPositioningCard = dealerHandPositionPlaceholder.transform.gameObject;
        Destroy( tPositioningCard );

        defaultCardScale = dealerHandPositionPlaceholder.transform.localScale;
        deckLocalPosition = defaultCardLocalPosition + new Vector3( cardWidth * 5, cardHeight * 1.2f, 0 );

        // Update Deal button with configurable Ante amount
        buttonDeal.transform.Find( "Text" ).GetComponent<Text>().text = string.Format( "Deal (${0} Ante)", ante );

        bankroll = startBankroll;
        betThisHand = minimumBet;
        textFieldBetInput.GetComponent<InputField>().text = minimumBet.ToString();

        StringBuilder tInstructions = new StringBuilder();
        tInstructions.AppendLine("PAYOFF MULTIPLIER TABLE");
        tInstructions.AppendLine("——————————————");
        tInstructions.AppendLine("Straight Flush: 50");
        tInstructions.AppendLine("Four of a Kind: 25");
        tInstructions.AppendLine("Full House: 9");
        tInstructions.AppendLine("Flush: 5");
        tInstructions.AppendLine("Straight: 4");
        tInstructions.AppendLine("Three of a Kind: 3");
        tInstructions.AppendLine("Two Pair: 2");
        tInstructions.AppendLine("Pair: 1");
        drawPokerInstructions = tInstructions.ToString();

        // SEVEN-UP
        tInstructions = new StringBuilder();
        tInstructions.AppendLine( "PAYOFF MULTIPLIER TABLE" );
        tInstructions.AppendLine( "——————————————" );
        tInstructions.AppendLine( "Straight Flush: 50" );
        tInstructions.AppendLine( "Four of a Kind: 25" );
        tInstructions.AppendLine( "Flush: 9" );
        tInstructions.AppendLine( "Full House: 5" );
        tInstructions.AppendLine( "Straight: 4" );
        tInstructions.AppendLine( "Three of a Kind: 3" );
        tInstructions.AppendLine( "Two Pair: 2" );
        tInstructions.AppendLine( "Pair: 1" );
        sevenUpDrawPokerInstructions = tInstructions.ToString();

        List<Type> tStateTypes = new List<Type>() { typeof(StateOpen),
                                                    typeof(StateDeal),
                                                    typeof(StateDraw), 
                                                    typeof(StateShowdown),
                                                    typeof(StateConcludeGame), };
        _FSM = new FiniteStateMachine( this, tStateTypes );
        _FSM.SetNextState( typeof(StateOpen), false );
        _FSM.OnStateExitComplete(); // relays call to SwitchState.
        _FSM.Start();
    }

    // Method shared by multiple States
    public void ChangeBankroll ( int pInt )
    {
        bankroll += pInt;
        UpdateBankrollDisplay();
    }

    // Method shared by multiple States
    public void UpdateBankrollDisplay ()
    {
        textFieldBankroll.GetComponent<Text>().text = "$" + bankroll.ToString();
    }

    // Method shared by multiple States
    public void ValidateBetEntry ()
    {
        int tParsed;
        if ( Int32.TryParse( textFieldBetInput.GetComponent<InputField>().text, out tParsed ) )
        {
            if (tParsed < minimumBet)
            {
                betThisHand = minimumBet;
            }
            else if (tParsed > bankroll || tParsed > maximumBet)
            {
                betThisHand = Math.Min( maximumBet, bankroll );
            }
            else
            {
                betThisHand = tParsed;
            }
        }
        else
        {
            betThisHand = minimumBet;
        }

        textFieldBetInput.GetComponent<InputField>().text = betThisHand.ToString();
    }
    
}

public enum GameType
{
    draw,
    sevenUp
}
