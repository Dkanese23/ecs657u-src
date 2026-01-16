using UnityEngine;
using UnityEngine.EventSystems;

// Provides reactive visual feedback when the player hovers over a card
public class CardHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    public float hoverScale = 1.15f; // The scale multiplier when focused
    public float speed = 15f;        // The interpolation speed for the transition
    
    private Vector3 originalScale = Vector3.one;
    private Vector3 targetScale = Vector3.one;

    private void Start()
    {
        // Initialises the base scale to ensure a clean return point
        originalScale = Vector3.one;
        targetScale = originalScale;
    }

    private void Update()
    {
        // Smoothly interpolates the scale to create a professional 'pop' effect
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
    }

    // Triggered by Unity's Event System when the mouse enters the card's bounds
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;
        
        // UX Polish: Moves the card to the front of the rendering order.
        // This ensures the enlarged card appears above its neighbours in a hand
        transform.SetAsLastSibling(); 
    }

    // Triggered when the mouse leaves the card's bounds
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }
}