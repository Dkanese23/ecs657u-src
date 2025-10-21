// CardItem.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardItem : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descText;
    public Button button;
    public TMP_Text buttonLabel;

    CardBase boundCard;
    System.Action<CardBase> onClick;

    public void Bind(CardBase card, string label, System.Action<CardBase> clickAction)
    {
        boundCard = card;
        onClick = clickAction;

        titleText.text = card.Title;
        descText.text = card.Description;
        buttonLabel.text = label;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(boundCard));
    }
}
