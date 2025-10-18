using UnityEngine;
using UnityEngine.UI;

public class Nameplate : MonoBehaviour
{
    public Text label;                 // Legacy Text
    public Transform target;
    public Vector3 offset = new Vector3(0, 1.8f, 0);
    public bool alwaysOnTop = true;    // set true to ignore depth occlusion (via screen-space fallback)

    Camera cam;
    Canvas canvas;
    Vector3 screenPos;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (canvas) canvas.worldCamera = Camera.main;
    }

    void OnEnable()
    {
        cam = Camera.main;
        if (canvas && !canvas.overrideSorting) { canvas.overrideSorting = true; canvas.sortingOrder = 500; }
        // Make sure scale is sane even if prefab was messed up
        if (Mathf.Abs(transform.localScale.x) < 1e-4f) transform.localScale = Vector3.one * 0.01f;
    }

    public void Set(string name, Health hp)
    {
        UpdateText(name, hp.CurrentHP, hp.MaxHP);
        hp.OnHealthChanged += (cur, max) => UpdateText(name, cur, max);
    }

    void UpdateText(string name, int cur, int max)
    {
        if (label) label.text = $"{name}  HP: {cur}/{max}";
    }

    void LateUpdate()
    {
        if (!target) return;
        if (!cam) { cam = Camera.main; if (canvas) canvas.worldCamera = cam; }

        // Face camera
        transform.position = target.position + offset;
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);

        if (alwaysOnTop)
        {
            // If behind camera, hide
            screenPos = cam.WorldToViewportPoint(transform.position);
            bool behind = screenPos.z < 0f;
            gameObject.SetActive(!behind);
        }
    }
}
