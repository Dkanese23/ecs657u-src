using UnityEngine;

public class DeckUI : MonoBehaviour
{
    public Transform contentRoot;
    public GameObject cardItemPrefab;

    void OnEnable()
    {
        if (DeckService.I)
            DeckService.I.OnDeckChanged += Refresh;

        Refresh();
    }

    void OnDisable()
    {
        if (DeckService.I)
            DeckService.I.OnDeckChanged -= Refresh;
    }

    void Refresh()
    {
        if (DeckService.I == null) return;

        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        foreach (var card in DeckService.I.GetDeckCopy())
        {
            var go = Instantiate(cardItemPrefab, contentRoot);
            var ci = go.GetComponent<CardItem>();
            ci.Bind(card, "Remove", (c) => DeckService.I.RemoveCard(c));
        }
    }

}
