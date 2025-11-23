using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public partial class BattleManager : MonoBehaviour
{
    [Header("Party & Enemy")]
    public List<BattleCharacter> party;    // 3 entries in scene
    public EnemyBase enemy;                // now using EnemyBase

    [Header("UI")]
    public HandUI handUI;
    public Button drawSkipButton;
    public bool isBusy;
    public Text enemyHPText;
    public GameObject resultPanel;
    public GameObject handPanel;
    public Text resultText;
    public GameObject handArea;
    public Button returnButton;

    [Header("HUD")]
    public NameplateHUD nameplateHUD;

    [Header("Camera")]
    public BattleCamera battleCamera;

    // Deck state
    Queue<CardBase> drawPile = new();
    List<CardBase> discard = new();

    int turnIndex = 0;
    bool playerPhase = true;

    // Optional: quick buff bookkeeping for Rally
    Dictionary<BattleCharacter, (int bonus, int turns)> flatAtkBonus = new();

    void Awake()
    {
        if (drawSkipButton)
        {
            drawSkipButton.onClick.RemoveAllListeners();
            drawSkipButton.onClick.AddListener(DrawAndSkip);
        }
        if (returnButton)
        {
            returnButton.onClick.RemoveAllListeners();
            // TODO: change to final main scene name
            returnButton.onClick.AddListener(() => SceneManager.LoadScene("Main(prototype)"));
        }
    }

    void Start()
    {
        // Initialise enemy AI (important!)
        if (enemy != null)
            enemy.Initialize(this);

        // Nameplates
        foreach (var ch in party)
            nameplateHUD.Register(ch.transform, ch.Health, ch.displayName);

        Debug.Log($"Enemy Health: {enemy.Health}, CurrentHP: {enemy.Health?.CurrentHP}, MaxHP: {enemy.Health?.MaxHP}");
        nameplateHUD.Register(enemy.transform, enemy.Health, "Enemy");

        // Camera focus: active char vs enemy
        battleCamera.SetFocus(party[0].transform, enemy.transform);

        // Enemy HP text
        enemy.Health.OnHealthChanged += (_, __) => RefreshEnemyHP();
        RefreshEnemyHP();

        // Deck
        BuildAndShuffleDeck();
        DealStartingHand(5);

        StartPlayerPhase();
    }

    void BuildAndShuffleDeck()
    {
        var list = DeckService.I ? DeckService.I.GetDeckCopy() : new List<CardBase>();
        if (list.Count == 0)
            Debug.LogWarning("DeckService has no deck; using any scene-starting deck if assigned via inspector.");

        // clone ScriptableObject instances so we don't mutate the assets
        var clonedList = new List<CardBase>();
        foreach (var card in list)
            clonedList.Add(Object.Instantiate(card));

        list = clonedList;

        // Shuffle
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }

        foreach (var c in list) drawPile.Enqueue(c);
    }

    void DealStartingHand(int n)
    {
        handUI.Clear();
        for (int i = 0; i < n; i++) DrawToHand();
    }

    void DrawToHand()
    {
        if (drawPile.Count == 0 && discard.Count > 0)
        {
            // reshuffle discard
            for (int i = discard.Count - 1; i >= 0; i--)
                drawPile.Enqueue(discard[i]);
            discard.Clear();
        }

        if (drawPile.Count == 0) return;
        var card = drawPile.Dequeue();
        handUI.AddCard(card, OnCardClicked);
    }

    void StartPlayerPhase()
    {
        playerPhase = true;
        turnIndex = 0;
        FocusActive();
        drawSkipButton.interactable = true;
    }

    void FocusActive()
    {
        var active = party[turnIndex];
        if (active.Health.CurrentHP <= 0)
        {
            // skip to next
            NextPartyOrEnemy();
            return;
        }

        battleCamera.SetFocus(active.transform, enemy.transform);
        nameplateHUD.Highlight(active.transform);
    }

    void NextPartyOrEnemy()
    {
        turnIndex++;

        if (turnIndex >= party.Count)
        {
            playerPhase = false;
            drawSkipButton.interactable = false;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            FocusActive();
            drawSkipButton.interactable = true;
        }
    }

    IEnumerator EnemyTurn()
    {
        
        if (enemy == null)
        {
            Debug.LogWarning("[BM] EnemyTurn called but enemy is null.");
            StartPlayerPhase();
            yield break;
        }

        Debug.Log($"[BM] EnemyTurn start. Enemy HP: {enemy.Health.CurrentHP}/{enemy.Health.MaxHP}");

        // dead enemy = no turn
        if (enemy.Health.CurrentHP <= 0)
        {
            Debug.Log("[BM] Enemy is dead, skipping enemy turn.");
            StartPlayerPhase();
            yield break;
        }
        
        Debug.Log($"[BM] Party count: {(party == null ? 0 : party.Count)}");
        if (party != null)
        {
            for (int i = 0; i < party.Count; i++)
            {
                var ch = party[i];
                Debug.Log($"[BM] party[{i}] = {(ch ? ch.displayName : "null")}  hp={(ch && ch.Health != null ? ch.Health.CurrentHP : -1)}/{(ch && ch.Health != null ? ch.Health.MaxHP : -1)}");
                Debug.Log($"[BM] party[{i}] scene='{(ch ? ch.gameObject.scene.name : "<null>")}' object={ch?.name}");
            }
        }


        // Let the enemy AI choose its move
        enemy.PlanNextAction(party);
        

        // Execute that move (Heal / Power Up / Dark Bolt)
        yield return StartCoroutine(enemy.ExecuteTurn(party));

        // After enemy has acted, re-check defeat
        CheckDefeat();
        if (resultPanel != null && resultPanel.activeSelf)
        {
            Debug.Log("[BM] Party defeated after enemy action.");
            yield break;
        }

        // End-of-round ticks (taunt, buffs, etc.)
        foreach (var ch in party)
            if (ch != null) ch.TickEndOfRound();

        Debug.Log("[BM] Enemy turn ended, starting player phase.");
        StartPlayerPhase();
    }


    void OnCardClicked(CardBase c)
    {
        if (!playerPhase || isBusy) return;
        isBusy = true;

        var actor = party[turnIndex];
        var ctx = new BattleContext { BM = this, Actor = actor, Target = actor, Enemy = enemy };

        // Apply flat bonus if active
        if (flatAtkBonus.TryGetValue(actor, out var b) && b.turns > 0 && c.School == CardSchool.Physical)
        {
            
            actor.baseAttack += b.bonus;
            c.Play(ctx);
            actor.baseAttack -= b.bonus;
            flatAtkBonus[actor] = (b.bonus, b.turns - 1);
            if (flatAtkBonus[actor].turns <= 0) flatAtkBonus.Remove(actor);
        }
        else
        {
            c.Play(ctx);
        }

        handUI.Remove(c);
        discard.Add(c);
        // no auto-draw here – Draw & Skip handles drawing mid-battle

        if (enemy.Health.CurrentHP > 0)
            NextPartyOrEnemy();

        isBusy = false;
    }

    void DrawAndSkip()
    {
        if (!playerPhase || isBusy) return;

        drawSkipButton.interactable = false;
        isBusy = true;

        TryDrawOne();
        NextPartyOrEnemy();

        isBusy = false;
    }

    void TryDrawOne()
    {
        if (drawPile.Count == 0 && discard.Count > 0)
        {
            // reshuffle discard into draw
            for (int i = discard.Count - 1; i >= 0; i--)
                drawPile.Enqueue(discard[i]);
            discard.Clear();
        }

        if (drawPile.Count == 0) return;   // nothing to draw
        var card = drawPile.Dequeue();
        handUI.AddCard(card, OnCardClicked);
    }

    public void DamageEnemy(int amount)
    {
        enemy.Health.TakeDamage(amount);
        RefreshEnemyHP();

        if (enemy.Health.CurrentHP <= 0)
            OnEnemyDeath();
    }

    public void RefreshNameplates()
    {
        // HUD auto-updates HP via events; keep for future badges/overlays
    }

    public void TagFlatAttackBonus(BattleCharacter who, int bonus, int turns)
    {
        flatAtkBonus[who] = (bonus, turns);
    }

    void RefreshEnemyHP()
    {
        if (enemyHPText)
            enemyHPText.text = $"Enemy HP: {enemy.Health.CurrentHP}/{enemy.Health.MaxHP}";
    }

    void OnEnemyDeath()
    {
        resultPanel.SetActive(true);
        resultText.text = "Victory!";

        handArea.SetActive(false);
        handPanel.SetActive(false);
    }

    void CheckDefeat()
    {
        bool allDown = true;
        foreach (var ch in party)
            if (ch.Health.CurrentHP > 0) { allDown = false; break; }

        if (allDown)
            ShowDefeat();
    }

    void ShowDefeat()
    {
        // stop input
        playerPhase = false;
        drawSkipButton.interactable = false;

        // show UI
        resultPanel.SetActive(true);
        resultText.text = "Defeat!";

        handArea.SetActive(false);
        handPanel.SetActive(false);
    }
}
