using UnityEngine;
using System; // Needed for Guid

public class CluePickup : MonoBehaviour, IInteractable
{
    [Header("Persistence")]
    [Tooltip("Auto-generated unique ID. Do not edit manually.")]
    public string clueId; 

    [Header("Content")]
    [TextArea(3, 10)]
    public string popupDescription; // Text to show in the UI

    [Header("Visuals")]
    public float rotateSpeed = 50f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    private Vector3 startPos;

    // --- AUTOMATION MAGIC ---
    void OnValidate()
    {
        if (string.IsNullOrEmpty(clueId)) 
        {
            GenerateNewID();
        }
    }

    [ContextMenu("Generate New ID")]
    private void GenerateNewID()
    {
        clueId = Guid.NewGuid().ToString();
    }
    // ------------------------

    void Start()
    {
        // 1. Check Memory: Did we already find this clue?
        if (GameState.I != null && !string.IsNullOrEmpty(clueId))
        {
            if (GameState.I.HasKeyItem(clueId))
            {
                Destroy(gameObject); // Already found -> remove it
                return;
            }
        }

        // 2. Initialize Visuals (Capture starting position for bobbing)
        startPos = transform.position;
    }

    // New Update Loop for Visuals
    void Update()
    {
        // Rotate
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        
        // Bob up and down
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void Interact(GameObject interactor)
    {
        // 1. Register in Journal
        if (ClueLog.Instance != null)
        {
            ClueLog.Instance.RegisterClue(clueId);
        }

        // 2. Show the Popup UI
        if (CluePopupUI.Instance != null)
        {
            CluePopupUI.Instance.ShowClue(popupDescription);
        }

        // 3. SAVE MEMORY: Mark this ID as collected
        if (GameState.I != null && !string.IsNullOrEmpty(clueId))
        {
            GameState.I.AddKeyItem(clueId);
        }

        // 4. Destroy object
        Destroy(gameObject);
    }
}