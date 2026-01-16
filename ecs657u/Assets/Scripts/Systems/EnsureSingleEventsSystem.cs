using UnityEngine;
using UnityEngine.EventSystems;

// Ensures only one EventSystem exists to prevent UI input conflicts
public class EnsureSingleEventSystem : MonoBehaviour
{
    void Awake()
    {
        // Find all active EventSystems in the current scene
        var systems = FindObjectsOfType<EventSystem>();
        
        // If more than one is detected, this object is a redundant duplicate
        if (systems.Length > 1)
        {
            Debug.Log("[UI Management] Duplicate EventSystem detected and removed to prevent input lag.");
            Destroy(gameObject); 
        }
    }
}