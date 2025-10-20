// DeckService.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public class DeckService : MonoBehaviour
{
    public static DeckService I { get; private set; }
    public DeckData currentDeck;

    public event Action OnDeckChanged;

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

    public void AddCard(CardBase card)
    {
        if (currentDeck == null || card == null) return;
        currentDeck.cards.Add(card);
        OnDeckChanged?.Invoke();
    }

    public void RemoveCard(CardBase card)
    {
        if (currentDeck == null || card == null) return;
        currentDeck.cards.Remove(card);
        OnDeckChanged?.Invoke();
    }
}
