using UnityEngine;
using System; // Needed for Guid

public class WorldCardPickup : MonoBehaviour, IInteractable
{
    [Header("Persistence")]
    // We make this read-only in the Inspector so you don't accidentally break it
    [Tooltip("Auto-generated unique ID.")]
    public string pickupID; 

    [Header("Data")]
    public CardBase cardToGive; 

    [Header("Visuals")]
    public float rotateSpeed = 50f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    private Vector3 startPos;

    // --- AUTOMATION MAGIC ---
    // This function runs inside the Unity Editor whenever you change something
    // or place the object in the scene.
    void OnValidate()
    {
        // If the ID is empty, generate a new random one
        if (string.IsNullOrEmpty(pickupID))
        {
            GenerateNewID();
        }
    }

    // Right-click context menu to force a new ID if you copy-pasted and want to be sure
    [ContextMenu("Generate New ID")]
    private void GenerateNewID()
    {
        pickupID = Guid.NewGuid().ToString();
    }
    // ------------------------

    void Start()
    {
        // 1. Check Memory: Have we picked this up before?
        if (GameState.I != null && !string.IsNullOrEmpty(pickupID))
        {
            if (GameState.I.HasKeyItem(pickupID))
            {
                Destroy(gameObject); 
                return;
            }
        }

        startPos = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void Interact(GameObject player)
    {
        if (DeckService.I != null && cardToGive != null)
        {
            DeckService.I.CollectNewCard(cardToGive);

            if (GameState.I != null && !string.IsNullOrEmpty(pickupID))
            {
                GameState.I.AddKeyItem(pickupID);
            }

            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Cannot pickup: DeckService or Card Data is missing!");
        }
    }
}