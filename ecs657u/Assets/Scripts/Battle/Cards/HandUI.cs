using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    public Transform handRoot;           // a Horizontal Layout Group container
    public GameObject cardButtonPrefab;  // the prefab above
    public int handLimit = 5;

    readonly List<CardBase> hand = new();
    public IReadOnlyList<CardBase> Cards => hand;

    public void Clear()
    {
        foreach (Transform t in handRoot) Destroy(t.gameObject);
        hand.Clear();
    }

    public void AddCard(CardBase card, System.Action<CardBase> onClicked)
    {
        if (hand.Count >= handLimit) return;
        hand.Add(card);

        var go = Instantiate(cardButtonPrefab, handRoot);
        go.name = $"Card_{card.Title}";
        (go.transform as RectTransform).localScale = Vector3.one;

        var view = go.GetComponent<CardView>();
        if (!view) { Debug.LogError("CardButton prefab missing CardView (Legacy)."); return; }

        view.Bind(card, () => onClicked?.Invoke(card));
    }

    public void Remove(CardBase c)
    {
        int i = hand.IndexOf(c);
        if (i < 0) return;
        hand.RemoveAt(i);
        Destroy(handRoot.GetChild(i).gameObject);
    }
}
