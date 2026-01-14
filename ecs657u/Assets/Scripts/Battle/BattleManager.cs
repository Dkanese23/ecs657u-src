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
    // public BattleCamera battleCamera;

    // Deck state
    Queue<CardBase> drawPile = new();
    List<CardBase> discard = new();

    int turnIndex = 0;
    bool playerPhase = true;
    CardBase pendingCard;
    bool waitingForAllyTarget;
    bool handInputLocked;

    // Optional: quick buff bookkeeping for Rally
    Dictionary<BattleCharacter, (int bonus, int turns)> flatAtkBonus = new();

    public void AttachEnemy(EnemyBase e)
    {
        enemy = e;
        enemy.Initialize(this);

        // hook HP UI once here
        if (enemyHPText)
        {
            enemy.Health.OnHealthChanged -= (_, __) => RefreshEnemyHP(); // defensive
            enemy.Health.OnHealthChanged += (_, __) => RefreshEnemyHP();
            RefreshEnemyHP();
        }
    

        // nameplate & camera refresh
        if (nameplateHUD) nameplateHUD.Register(enemy.transform, enemy.Health, enemy.enemyName);
        // if (party != null && party.Count > 0 && battleCamera)
        //     battleCamera.SetFocus(party[0].transform, enemy.transform);
    }


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
        // Party nameplates first
        if (nameplateHUD)
        {
            foreach (var ch in party)
                if (ch) nameplateHUD.Register(ch.transform, ch.Health, ch.displayName);
        }

        
        if (enemy != null) AttachEnemy(enemy);
        // Example: party is List<BattleCharacter>, enemy is EnemyBase
        var cam = FindObjectOfType<BattleFramingCamera>();
        if (cam)
        {
            var partyTs = new List<Transform>();
            foreach (var ch in party) if (ch) partyTs.Add(ch.transform);
            cam.SetPartyAndEnemy(partyTs, enemy ? enemy.transform : null);
        }

        // Deck
        BuildAndShuffleDeck();
        DealStartingHand(5);

        // BattleManager.Start() after you build party & enemy
        foreach (var ch in party)
        {
            var c = ch; // capture
            ch.Health.OnDeath += () => {
                c.GetComponent<BattleAnim>()?.PlayDie();
            };
        }
        enemy.Health.OnDeath += () => {
            enemy.GetComponent<BattleAnim>()?.PlayDie();
        };


        ShowHowToThenStart();
    }

    void ShowHowToThenStart()
    {
        // Show only once
        if (PlayerPrefs.GetInt("CardHowToSeen", 0) == 0)
        {
            var msg = "Pick a card for each hero in turn. Attack scales with Strength, support with Agility, magic with Intelligence. Click a card, then click a target if needed; or press Draw to skip and draw a new card.";
            BattleHowToUI.Show(msg, () =>
            {
                PlayerPrefs.SetInt("CardHowToSeen", 1);
                PlayerPrefs.Save();
                StartPlayerPhase();
            });
        }
        else
        {
            StartPlayerPhase();
        }
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

        if (active.Health.CurrentHP <= 0) { NextPartyOrEnemy(); return; }

        // if (battleCamera)
        //     battleCamera.SetFocus(active.transform, enemy ? enemy.transform : active.transform);

        if (nameplateHUD)
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
        if (!playerPhase || isBusy || handInputLocked) return;

        // If this card needs an ally target, enter targeting mode
        if (c.Targeting == CardBase.TargetingType.Ally || c.Targeting == CardBase.TargetingType.SelfOrAlly)
        {
            pendingCard = c;
            waitingForAllyTarget = true;
            handInputLocked = true;
            // handUI.HighlightCard(c);                 // optional: visually show it’s selected
            nameplateHUD.EnableAllyClicks(this);     // (see below) make nameplates clickable
            LogAction("Select an ally to target (or press ESC to cancel).");
            return;
        }

        // otherwise: play immediately with self/none target
        PlayCardNow(c, target: null);
    }

    void PlayCardNow(CardBase c, BattleCharacter target)
    {
        isBusy = true;

        var actor = party[turnIndex];
        var ctx = new BattleContext { BM = this, Actor = actor, Target = target, Enemy = enemy };

        c.Play(ctx);

        handUI.Remove(c);
        discard.Add(c);

        if (enemy.Health.CurrentHP > 0)
            NextPartyOrEnemy();
        isBusy = false;
    }

    public void SelectAllyTarget(BattleCharacter chosen)
    {
        if (!waitingForAllyTarget || pendingCard == null) return;

        // If card allows self or ally, chosen may be same as actor; if "Ally" only, you can enforce different target here
        var actor = party[turnIndex];
        if (pendingCard.Targeting == CardBase.TargetingType.Ally && chosen == actor)
        {
            LogAction("This card must target another ally.");
            return;
        }

        // play and clear state
        var c = pendingCard;
        pendingCard = null;
        waitingForAllyTarget = false;
        handInputLocked = false;
        nameplateHUD.DisableAllyClicks();
        // handUI.ClearHighlight();
        PlayCardNow(c, chosen);
    }

    public void CancelTargeting()
    {
        if (!waitingForAllyTarget) return;
        waitingForAllyTarget = false;
        handInputLocked = false;
        pendingCard = null;
        nameplateHUD.DisableAllyClicks();
        // handUI.ClearHighlight();
        LogAction("Target selection canceled.");
    }

    void Update()
    {
    if (waitingForAllyTarget && Input.GetKeyDown(KeyCode.Escape))
        CancelTargeting();
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
        enemy.GetComponent<BattleAnim>()?.PlayHit();
        RefreshEnemyHP();

        if (enemy.Health.CurrentHP <= 0)
        {
            enemy.GetComponent<BattleAnim>()?.PlayDie();
            OnEnemyDeath();
        }
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
        if (GameState.I != null)
        {
            // mark defeated
            if (!string.IsNullOrEmpty(GameState.I.currentEnemyId))
                GameState.I.MarkEnemyDefeated(GameState.I.currentEnemyId);

            // grant key item (example)
            GameState.I.AddKeyItem("key_fragment"); // change per enemy type/ID if needed

            // clear encounter
            GameState.I.ClearEncounter();

            // ensure we restore to last checkpoint (optional: you may want to continue ahead instead)
            GameState.I.pendingRespawn = true; // use checkpoint even on win (optional behavior)
        }

        resultPanel.SetActive(true);
        resultText.text = "Victory! You received a key.";

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

        if (GameState.I != null)
        {
            // Do NOT mark defeated; enemy stays in overworld
            // Just flag respawn to checkpoint
            GameState.I.pendingRespawn = true;
            GameState.I.ClearEncounter();
        }
        // show UI
        resultPanel.SetActive(true);
        resultText.text = "Defeat!";

        handArea.SetActive(false);
        handPanel.SetActive(false);
    }
}
