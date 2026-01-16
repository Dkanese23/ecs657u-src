// CardItem.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Controls the visual representation and interaction of an individual card within the UI
public class CardItem : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descText;
    public Button button;
    public TMP_Text buttonLabel;

    CardBase boundCard; // The specific card data currently linked to this UI element
    System.Action<CardBase> onClick; // Delegate to handle the logic when the player selects this card

    // Initialises the UI components with card data and sets up the click listener
    public void Bind(CardBase card, string label, System.Action<CardBase> clickAction)
    {
        boundCard = card;
        onClick = clickAction;

        // Map the card metadata to the TextMeshPro components
        titleText.text = card.Title;
        descText.text = card.Description;
        buttonLabel.text = label;

        // Reset and assign the button functionality to prevent duplicate triggers
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(boundCard));
    }
}