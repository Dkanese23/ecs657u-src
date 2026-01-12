using UnityEngine;
using UnityEngine.EventSystems;

public class CardHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    public float hoverScale = 1.15f;
    public float speed = 15f;
    
    // We use a local offset so it doesn't break the Grid Layout
    private Vector3 originalScale = Vector3.one;
    private Vector3 targetScale = Vector3.one;

    private void Start()
    {
        // Ensure we start at normal size
        originalScale = Vector3.one;
        targetScale = originalScale;
    }

    private void Update()
    {
        // Smoothly scale the visual part only
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;
        
        // This moves the card to the FRONT of the UI 
        // without changing its position in the Layout Group
        transform.SetAsLastSibling(); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }
}