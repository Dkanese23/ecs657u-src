using UnityEngine;
using UnityEngine.UI;

// Manages the visual display of card data within a UI container
public class CardView : MonoBehaviour
{
    public Text title;
    public Text desc;
    public Button btn;

    // Maps card properties to UI elements and assigns the click functionality
    public void Bind(CardBase card, System.Action onClick)
    {
        // Update text fields with card metadata for player clarity
        title.text = card.Title;
        desc.text  = card.Description;

        // Clear existing listeners to ensure the button performs the correct action
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => onClick?.Invoke());
    }
}