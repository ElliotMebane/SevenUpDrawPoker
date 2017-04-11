using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StateConcludeGame : BaseState, IState
{
    Controller _context;

    public StateConcludeGame ( System.Object pContext, FiniteStateMachine pFSM ) : base( pContext, pFSM )
    {
        _context = _contextObject as Controller;
    }

    public override IEnumerable Execute ()
    {
        ClearTable();

        _FSM.SetNextState( typeof( StateOpen ), true );

        _FSM.OnStateExitComplete();

        yield return null;
    }

    private void ClearTable ()
    {
        _context.textFieldBetInput.GetComponent<InputField>().text = "";
        _context.textFieldConsole.GetComponent<Text>().text = "";

        int i;
        int iLen = _context.deck.AllCards.Count;
        List<Card> tCards = _context.deck.AllCards;
        for ( i = 0; i < iLen; i++ )
        {
            Card tCard = tCards[i];
            tCard.InstantiatedGO.transform.localPosition = _context.defaultCardLocalPosition;
            tCard.InstantiatedGO.transform.parent = null;
            tCard.InstantiatedGO.SetActive( false );
        }
    }

    public override void BeginExit ()
    {
        _context.textFieldConsole.GetComponent<Text>().text = "";
    }

}
