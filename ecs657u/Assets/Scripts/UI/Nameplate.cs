using UnityEngine;
using UnityEngine.UI;

public class Nameplate : MonoBehaviour
{
    public Text label;
    public Transform target;
    public Vector3 offset = new Vector3(0, 1.8f, 0);
    public bool alwaysOnTop = true; // Helps the UI stay visible even if clipping through walls

    Camera cam;
    Canvas canvas;
    Vector3 screenPos;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        // Ensure the UI knows which camera to look at
        if (canvas) canvas.worldCamera = Camera.main;
    }

    void OnEnable()
    {
        cam = Camera.main;
        // Sorting order 500 keeps this on top of most other world objects
        if (canvas && !canvas.overrideSorting) { canvas.overrideSorting = true; canvas.sortingOrder = 500; }
        
        // Safety check: World-space canvases often need very small scales (0.01) to look right
        if (Mathf.Abs(transform.localScale.x) < 1e-4f) transform.localScale = Vector3.one * 0.01f;
    }

    // Initialize nameplate and subscribe to health updates automatically
    public void Set(string name, Health hp)
    {
        UpdateText(name, hp.CurrentHP, hp.MaxHP);
        // Using a lambda means we don't need a separate method for the event listener
        hp.OnHealthChanged += (cur, max) => UpdateText(name, cur, max);
    }

    void UpdateText(string name, int cur, int max)
    {
        if (label) label.text = $"{name}  HP: {cur}/{max}";
    }

    // LateUpdate is best for UI following players to avoid "jittery" movement
    void LateUpdate()
    {
        if (!target) return;
        if (!cam) { cam = Camera.main; if (canvas) canvas.worldCamera = cam; }

        // Positioning & Billboarding (Making the UI face the camera)
        transform.position = target.position + offset;
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);

        if (alwaysOnTop)
        {
            // Simple check to hide the nameplate if the enemy is behind the player
            screenPos = cam.WorldToViewportPoint(transform.position);
            bool behind = screenPos.z < 0f;
            if (gameObject.activeSelf == behind) gameObject.SetActive(!behind);
        }
    }
}