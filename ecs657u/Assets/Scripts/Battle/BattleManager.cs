using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [Header("Party & Enemy")]
    public List<BattleCharacter> party;    // 3 entries in scene
    public EnemySimple enemy;

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

    
    Queue<CardBase> drawPile = new();
    List<CardBase> discard = new();

    int turnIndex = 0;
    bool playerPhase = true;

    
    Dictionary<BattleCharacter, (int bonus, int turns)> flatAtkBonus = new();

    void Awake()
    {
        if (drawSkipButton)
        {
            drawSkipButton.onClick.RemoveAllListeners();           // ← ensure only one listener
            drawSkipButton.onClick.AddListener(DrawAndSkip);
        }
        if (returnButton)
        {
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(() => SceneManager.LoadScene("Main(prototype)") ); // change to main later
        }
    }


    void Start()
    {
        // Nameplates
        foreach (var ch in party)
            nameplateHUD.Register(ch.transform, ch.Health, ch.displayName);
        Debug.Log($"Enemy Health: {enemy.Health}, CurrentHP: {enemy.Health?.CurrentHP}, MaxHP: {enemy.Health?.MaxHP}");
        nameplateHUD.Register(enemy.transform, enemy.Health, "Enemy");
        

        // Camera focus: active char vs enemy
        battleCamera.SetFocus(party[0].transform, enemy.transform);

        // Enemy HP text
        enemy.Health.OnHealthChanged += (_, __) => RefreshEnemyHP();
        // enemy.Health.OnDeath += OnEnemyDeath;
        RefreshEnemyHP();

        // Deck
        BuildAndShuffleDeck();
        DealStartingHand(5);

        StartPlayerPhase();
    }

    void BuildAndShuffleDeck()
    {
        var list = DeckService.I ? DeckService.I.GetDeckCopy() : new List<CardBase>();
        if (list.Count == 0) Debug.LogWarning("DeckService has no deck; using any scene-starting deck if assigned via inspector.");

        var clonedList = new List<CardBase>();
        foreach (var card in list)
        {
            clonedList.Add(Object.Instantiate(card));
        }

        list = clonedList;
        // Shuffle:
        for (int i = list.Count - 1; i > 0; i--)
        { int j = Random.Range(0, i + 1); (list[i], list[j]) = (list[j], list[i]); }
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
        
        yield return new WaitForSeconds(0.25f);

        // pick target: taunting > lowest HP alive
        BattleCharacter target = null;
        foreach (var ch in party) if (ch.isTaunting && ch.Health.CurrentHP > 0) { target = ch; break; }
        if (target == null)
        {
            int bestHP = int.MaxValue;
            foreach (var ch in party)
                if (ch.Health.CurrentHP > 0 && ch.Health.CurrentHP < bestHP) { bestHP = ch.Health.CurrentHP; target = ch; }
        }

        if (target != null) target.ReceiveDamage(enemy.attackDamage);

        yield return new WaitForSeconds(0.2f);
        CheckDefeat();
        if (resultPanel.activeSelf) yield break;

        // round tick
        foreach (var ch in party) ch.TickEndOfRound();

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
            // quick-and-dirty: bump actor.baseAttack temporarily
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
        // DrawToHand();  keep hand topped if possible

        if (enemy.Health.CurrentHP > 0)
            NextPartyOrEnemy();
        isBusy = false;
    }

    void DrawAndSkip()
    {
        
        if (!playerPhase || isBusy)
        {
            
            return;
        }
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

    public void RefreshNameplates() { /* the HUD updates via health events; keep for future */ }

    public void TagFlatAttackBonus(BattleCharacter who, int bonus, int turns)
    {
        flatAtkBonus[who] = (bonus, turns);
    }

    void RefreshEnemyHP()
    {
        if (enemyHPText) enemyHPText.text = $"Enemy HP: {enemy.Health.CurrentHP}/{enemy.Health.MaxHP}";
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
