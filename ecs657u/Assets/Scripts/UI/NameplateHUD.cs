using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameplateHUD : MonoBehaviour
{
    // NP (Nameplate) holds the data for each individual UI element
    [System.Serializable] public class NP
    {
        public Transform root;         
        public Transform anchor;       
        public Renderer[] renderers;   
        public Health health;
        public string displayName;
        public float extraY = 0.25f;   
        public RectTransform ui;
        public Image bg;
        public Text label;
    }

    public string anchorChildName = "NameplateAnchor"; 
    public RectTransform container; // The UI parent in the hierarchy
    public GameObject itemPrefab;

    readonly Dictionary<Transform, NP> map = new();
    RectTransform canvasRect;
    Camera cam;

    // Helper class to manage clickable buttons for allies
    class Entry { public BattleCharacter ch; public Button btn; }
    readonly List<Entry> entries = new();

    void Awake()
    {
        canvasRect = GetComponent<RectTransform>();
        if (!container) container = canvasRect;
        cam = Camera.main;
    }

    public void Register(Transform world, Health hp, string name, Vector3? _unused = null)
    {
        if (!world || !itemPrefab) return;

        // Build the physical UI element on the Canvas
        var go = Instantiate(itemPrefab, container);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);

        var np = new NP
        {
            root = world,
            health = hp,
            displayName = name,
            ui = rt,
            bg = go.GetComponent<Image>(),
            label = go.GetComponentInChildren<Text>(),
            renderers = world.GetComponentsInChildren<Renderer>(true),
        };

        // Logic to find where above the character the nameplate should float
        np.anchor = FindBestAnchor(world);

        // Update text initially and subscribe to future health changes
        if (np.label) np.label.text = $"{name}  HP: {hp.CurrentHP}/{hp.MaxHP}";
        if (hp) hp.OnHealthChanged += (cur, max) =>
        {
            if (np.label) np.label.text = $"{name}  HP: {cur}/{max}";
        };

        map[world] = np;

        // Setup the button for targeting logic
        var button = go.GetComponent<Button>() ?? go.AddComponent<Button>();
        button.interactable = false;
        var ch = world.GetComponent<BattleCharacter>();
        entries.Add(new Entry { ch = ch, btn = button });
    }

    // Logic to find the "Top" of a character regardless of their shape
    Transform FindBestAnchor(Transform t)
    {
        var child = t.Find(anchorChildName);
        if (child) return child;

        var anim = t.GetComponentInChildren<Animator>();
        if (anim && anim.isHuman)
        {
            var head = anim.GetBoneTransform(HumanBodyBones.Head);
            if (head) return head;
        }

        // Final fallback: Calculate the highest point of all meshes (renderers)
        var go = new GameObject("GeneratedNameplateAnchor");
        go.transform.SetParent(t, false);
        go.transform.position = BoundsTopWorld(t);
        return go.transform;
    }

    void LateUpdate()
    {
        if (!cam) cam = Camera.main;
        foreach (var np in map.Values)
        {
            if (!np.root || !np.ui) continue;

            // Project 3D point to 2D screen space
            Vector3 headPos = np.anchor ? np.anchor.position : BoundsTopWorld(np.root);
            headPos += Vector3.up * np.extraY;

            Vector3 sp = cam.WorldToScreenPoint(headPos);
            bool behind = sp.z < 0f;
            np.ui.gameObject.SetActive(!behind);
            if (behind) continue;

            // Update UI position on the canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, sp, null, out var local);
            np.ui.anchoredPosition = local;
        }
    }
}