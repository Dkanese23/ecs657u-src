using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameplateHUD : MonoBehaviour
{
    [System.Serializable] public class NP
    {
        public Transform world;
        public Health health;
        public string displayName;
        public Vector3 offset = new(0, 2f, 0);
        public RectTransform ui;
        public Image bg;   // optional highlight
        public Text  label;
    }

    public RectTransform container;        // leave empty to use canvas root
    public GameObject itemPrefab;          // Panel + Text (Legacy)

    readonly Dictionary<Transform, NP> map = new();
    RectTransform canvasRect;
    Camera cam;
    NP highlighted;
    class Entry { public BattleCharacter ch; public UnityEngine.UI.Button btn; }
    List<Entry> entries = new();
    BattleManager bm;

    void Awake()
    {
        canvasRect = GetComponent<RectTransform>();
        if (!container) container = canvasRect;
        cam = Camera.main;
    }


    public void Register(Transform world, Health hp, string name, Vector3? worldOffset = null)
    {
        if (!world || !itemPrefab) return;

        var go = Instantiate(itemPrefab, container);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f); // center
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 210f);

        var graphic = go.GetComponent<Graphic>();
        if (!graphic) graphic = go.AddComponent<Image>();
        graphic.raycastTarget = true;

        go.transform.SetAsLastSibling();

        var np = new NP
        {
            world = world,
            health = hp,
            displayName = name,
            offset = worldOffset ?? new Vector3(0, 2f, 0),
            ui = rt,
            bg = go.GetComponent<Image>(),
            label = go.GetComponentInChildren<Text>()
        };

        if (np.label) np.label.text = $"{name}  HP: {hp.CurrentHP}/{hp.MaxHP}";
        if (hp) hp.OnHealthChanged += (cur, max) =>
        {
            if (np.label) np.label.text = $"{name}  HP: {cur}/{max}";
        };

        map[world] = np;

        // inside NameplateHUD.Register (after creating 'go', 'rt', 'np' and map[world]=np)
        var button = go.GetComponent<Button>();
        if (!button) button = go.AddComponent<Button>();
        button.interactable = false; // default off

        var ch = world.GetComponent<BattleCharacter>();
        entries.Add(new Entry { ch = ch, btn = button });  // may be null for enemy

    }

    public void Highlight(Transform world)
    {
        // clear previous
        if (highlighted != null && highlighted.bg) highlighted.bg.color = new Color(0, 0, 0, 0.4f);

        if (world != null && map.TryGetValue(world, out var np))
        {
            highlighted = np;
            if (np.bg) np.bg.color = new Color(0.1f, 0.5f, 1f, 0.55f); // bluish highlight
        }
        else highlighted = null;
    }

    void LateUpdate()
    {
        if (!cam) cam = Camera.main;

        foreach (var np in map.Values)
        {
            if (!np.world || !np.ui) continue;

            Vector3 sp = cam.WorldToScreenPoint(np.world.position + np.offset);
            bool behind = sp.z < 0f;
            np.ui.gameObject.SetActive(!behind);
            if (behind) continue;

            // Convert to anchored position in Overlay canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, sp, null, out var local);
            np.ui.anchoredPosition = local;
        }
    }

    public void EnableAllyClicks(BattleManager manager)
    {
        bm = manager;
        foreach (var e in entries)
        {
            bool isPartyMember = (e.ch != null) && bm.party.Contains(e.ch);
            e.btn.onClick.RemoveAllListeners();
            if (isPartyMember)
            {
                var local = e; // capture
                e.btn.onClick.AddListener(() => bm.SelectAllyTarget(local.ch));
                e.btn.interactable = true;
            }
            else
            {
                e.btn.interactable = false; // enemy (no clicks)
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
        bm = null;
    }
}