using System.Collections.Generic;
using UnityEngine;

public class DeckService : MonoBehaviour
{
    public static DeckService I { get; private set; }
    public DeckData currentDeck;

    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public List<CardBase> GetDeckCopy()
    {
        
        return currentDeck ? new List<CardBase>(currentDeck.cards) : new List<CardBase>();
    }
}
