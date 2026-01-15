using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameplateHUD : MonoBehaviour
{
    [System.Serializable] public class NP
    {
        public Transform root;            // character root
        public Transform anchor;          // head/explicit anchor
        public Renderer[] renderers;      // for bounds fallback
        public Health health;
        public string displayName;
        public float extraY = 0.25f;      // extra gap above head
        public RectTransform ui;
        public Image bg;
        public Text  label;
    }

    public string anchorChildName = "NameplateAnchor";   // optional child to place on head
    public RectTransform container;                      // defaults to this canvas root
    public GameObject itemPrefab;

    readonly Dictionary<Transform, NP> map = new();
    RectTransform canvasRect;
    Camera cam;

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

        // Build UI
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

        // Find best anchor: explicit child → humanoid head → bounds top
        np.anchor = FindBestAnchor(world);

        if (np.label) np.label.text = $"{name}  HP: {hp.CurrentHP}/{hp.MaxHP}";
        if (hp) hp.OnHealthChanged += (cur, max) =>
        {
            if (np.label) np.label.text = $"{name}  HP: {cur}/{max}";
        };

        map[world] = np;

        var button = go.GetComponent<Button>() ?? go.AddComponent<Button>();
        button.interactable = false;
        var ch = world.GetComponent<BattleCharacter>();
        entries.Add(new Entry { ch = ch, btn = button });
    }

    Transform FindBestAnchor(Transform t)
    {
        // 1) explicit child
        var child = t.Find(anchorChildName);
        if (child) return child;

        // 2) humanoid head
        var anim = t.GetComponentInChildren<Animator>();
        if (anim && anim.isHuman)
        {
            var head = anim.GetBoneTransform(HumanBodyBones.Head);
            if (head) return head;
        }

        // 3) fallback: create a temporary anchor at bounds top
        var go = new GameObject("GeneratedNameplateAnchor");
        go.transform.SetParent(t, false);
        go.transform.position = BoundsTopWorld(t);
        return go.transform;
    }

    Vector3 BoundsTopWorld(Transform root)
    {
        var rends = root.GetComponentsInChildren<Renderer>(true);
        if (rends.Length == 0) return root.position + Vector3.up * 2f; // generic
        var b = new Bounds(rends[0].bounds.center, Vector3.zero);
        for (int i = 0; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);
        var p = b.center; p.y = b.max.y;
        return p;
    }

    public void Highlight(Transform world)
    {
        foreach (var kv in map)
            if (kv.Value.bg) kv.Value.bg.color = new Color(1f, 1f, 1f, 0.25f);

        if (world != null && map.TryGetValue(world, out var np) && np.bg)
            np.bg.color = new Color(0f, 1f, 1f, 0.75f);
    }

    void LateUpdate()
    {
        if (!cam) cam = Camera.main;
        foreach (var np in map.Values)
        {
            if (!np.root || !np.ui) continue;

            // World point directly above head
            Vector3 headPos = np.anchor ? np.anchor.position : BoundsTopWorld(np.root);
            headPos += Vector3.up * np.extraY;

            Vector3 sp = cam.WorldToScreenPoint(headPos);
            bool behind = sp.z < 0f;
            np.ui.gameObject.SetActive(!behind);
            if (behind) continue;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, sp, null, out var local);
            np.ui.anchoredPosition = local;
        }
    }

    public void EnableAllyClicks(BattleManager manager)
    {
        foreach (var e in entries)
        {
            bool isParty = (e.ch != null) && manager.party.Contains(e.ch);
            e.btn.onClick.RemoveAllListeners();
            e.btn.interactable = isParty;
            if (isParty)
            {
                var local = e;
                e.btn.onClick.AddListener(() => manager.SelectAllyTarget(local.ch));
            }
        }
    }

    public void DisableAllyClicks()
    {
        foreach (var e in entries)
        {
            e.btn.onClick.RemoveAllListeners();
            e.btn.interactable = false;
        }
    }
}
