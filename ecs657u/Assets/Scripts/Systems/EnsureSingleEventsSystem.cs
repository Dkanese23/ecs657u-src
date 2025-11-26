using UnityEngine;
using UnityEngine.EventSystems;

public class EnsureSingleEventSystem : MonoBehaviour
{
    void Awake()
    {
        var systems = FindObjectsOfType<EventSystem>();
        if (systems.Length > 1)
            Destroy(gameObject); // drop the newer duplicate
    }
}
