using UnityEngine;

public class Card
{
    private string _rank;
    private int _numericRank;
    private int _aceLowNumericRank;
    private string _suit;
    private string _suitFullName;
    private UnityEngine.Object _loadedResource;
    private GameObject _instantiatedGO;

    public Card( string pRank, string pSuit, string pSuitFullName, GameObject pLoadedResource, Vector3 pScale )
	{
        _rank = pRank;
        _suit = pSuit;
        _suitFullName = pSuitFullName;
        _loadedResource = pLoadedResource;

        _instantiatedGO = GameObject.Instantiate(_loadedResource) as GameObject;
        _instantiatedGO.AddComponent<BoxCollider>();
        _instantiatedGO.transform.tag = "card";
        _instantiatedGO.transform.name = ToString();
        _instantiatedGO.transform.localScale = pScale;
        _instantiatedGO.transform.localRotation = Quaternion.Euler( 0, 180f, 0 );

        _numericRank = GetNumericRank();
        _aceLowNumericRank = GetAceLowNumericRank();
    }

    public int GetNumericRank()
    {
        int tNumericRank = 0;
        int iLen;
        iLen = Deck.ranks.Length;
        for ( int i = 0; i < iLen; i++ )
        {
            if ( _rank == Deck.ranks[i] )
            {
                tNumericRank = i;
            }
        }

        return tNumericRank;
    }

    public int GetAceLowNumericRank()
    {
        int tAceLowNumericRank = 0;
        int iLen = Deck.ranksAceLow.Length;
        for ( int i = 0; i < iLen; i++ )
        {
            if ( _rank == Deck.ranksAceLow[i] )
            {
                tAceLowNumericRank = i;
            }
        }

        return tAceLowNumericRank;
    }

    public void ConfigureInstantiatedGO( GameObject pGO, Vector3 pScale )
    {
        _instantiatedGO = pGO;
        _instantiatedGO.AddComponent<BoxCollider>();
        _instantiatedGO.transform.tag = "card";
        _instantiatedGO.transform.name = ToString();
        _instantiatedGO.transform.localScale = pScale;
        _instantiatedGO.transform.localRotation = Quaternion.Euler( 0, 180f, 0 );
    }

    public string Rank
    {
        get { return _rank; }
    }

    public int NumericRank
    {
        get { return _numericRank; }
    }

    public int AceLowNumericRank
    {
        get { return _aceLowNumericRank; }
    }

    public string Suit
    {
        get { return _suit; }
    }

    public string SuitFullName
    {
        get { return _suitFullName; }
    }

    public UnityEngine.Object LoadedResource
    {
        get { return _loadedResource; }
    }

    public GameObject InstantiatedGO
    {
        get { return _instantiatedGO; }
        set { _instantiatedGO = value; }
    }

    public override string ToString()
    {
        return ( _rank + _suit );
    }
}
