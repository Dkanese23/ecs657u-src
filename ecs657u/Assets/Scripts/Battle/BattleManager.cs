using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Central controller for turn-based combat, managing the game loop and UI synchronisation
public partial class BattleManager : MonoBehaviour
{
    [Header("Party & Enemy")]
    public List<BattleCharacter> party;    
    public EnemyBase enemy;                

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

    // Deck state: Utilises a Queue for efficient 'Top of the Deck' card drawing
    Queue<CardBase> drawPile = new();
    List<CardBase> discard = new();

    int turnIndex = 0;
    bool playerPhase = true;
    CardBase pendingCard;
    bool waitingForAllyTarget;
    bool handInputLocked;

    // Tracks temporary stat modifiers applied during combat turns
    Dictionary<BattleCharacter, (int bonus, int turns)> flatAtkBonus = new();

    // Initialises the enemy and hooks into health events for UI reactivity
    public void AttachEnemy(EnemyBase e)
    {
        enemy = e;
        enemy.Initialize(this);

        if (enemyHPText)
        {
            // Observer Pattern: Updates UI automatically when health values change
            enemy.Health.OnHealthChanged -= (_, __) => RefreshEnemyHP(); 
            enemy.Health.OnHealthChanged += (_, __) => RefreshEnemyHP();
            RefreshEnemyHP();
        }

        if (nameplateHUD) nameplateHUD.Register(enemy.transform, enemy.Health, enemy.enemyName);
    }

    void Awake()
    {
        // Initialises persistent UI listeners for scene transitions
        if (drawSkipButton)
        {
            drawSkipButton.onClick.RemoveAllListeners();
            drawSkipButton.onClick.AddListener(DrawAndSkip);
        }
        if (returnButton)
        {
            returnButton.onClick.AddListener(() => 
            {
                string sceneToLoad = "Main(prototype)"; 
                if (GameState.I != null && !string.IsNullOrEmpty(GameState.I.lastScene))
                {
                    sceneToLoad = GameState.I.lastScene;
                }
                SceneManager.LoadScene(sceneToLoad);
            });
        }
    }

    void Start()
    {
        // Registers all participants with the HUD and framing camera
        if (nameplateHUD)
        {
            foreach (var ch in party)
                if (ch) nameplateHUD.Register(ch.transform, ch.Health, ch.displayName);
        }

        if (enemy != null) AttachEnemy(enemy);

        var cam = FindObjectOfType<BattleFramingCamera>();
        if (cam)
        {
            var partyTs = new List<Transform>();
            foreach (var ch in party) if (ch) partyTs.Add(ch.transform);
            cam.SetPartyAndEnemy(partyTs, enemy ? enemy.transform : null);
        }

        // Prepares the deck and initial hand
        BuildAndShuffleDeck();
        DealStartingHand(5);

        // Subscribes to death events to trigger specific animations
        foreach (var ch in party)
        {
            var c = ch; 
            ch.Health.OnDeath += () => { c.GetComponent<BattleAnim>()?.PlayDie(); };
        }
        if (enemy) enemy.Health.OnDeath += () => { enemy.GetComponent<BattleAnim>()?.PlayDie(); };

        ShowHowToThenStart();
    }

    // Instantiates cards from the DeckService to prevent mutating source assets
    void BuildAndShuffleDeck()
    {
        var list = DeckService.I ? DeckService.I.GetDeckCopy() : new List<CardBase>();
        var clonedList = new List<CardBase>();
        foreach (var card in list)
            clonedList.Add(Object.Instantiate(card));

        // Fisher-Yates Shuffle Algorithm for unbiased card distribution
        for (int i = clonedList.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (clonedList[i], clonedList[j]) = (clonedList[j], clonedList[i]);
        }

        foreach (var c in clonedList) drawPile.Enqueue(c);
    }

    // Coroutine managing the automated Enemy turn logic
    IEnumerator EnemyTurn()
    {
        if (enemy == null || enemy.Health.CurrentHP <= 0)
        {
            StartPlayerPhase();
            yield break;
        }

        // AI Decision Making Phase
        enemy.PlanNextAction(party);
        yield return StartCoroutine(enemy.ExecuteTurn(party));

        CheckDefeat();
        if (resultPanel != null && resultPanel.activeSelf) yield break;

        // Processes end-of-round logic for status effects
        foreach (var ch in party)
            if (ch != null) ch.TickEndOfRound();

        StartPlayerPhase();
    }

    // Processes card selection and enters targeting mode if required
    void OnCardClicked(CardBase c)
    {
        if (!playerPhase || isBusy || handInputLocked) return;

        if (c.Targeting == CardBase.TargetingType.Ally || c.Targeting == CardBase.TargetingType.SelfOrAlly)
        {
            pendingCard = c;
            waitingForAllyTarget = true;
            handInputLocked = true;
            nameplateHUD.EnableAllyClicks(this);
            return;
        }

        PlayCardNow(c, target: null);
    }

    // Executes the logical effect of a card and advances the turn index
    void PlayCardNow(CardBase c, BattleCharacter target)
    {
        isBusy = true;
        var actor = party[turnIndex];
        var ctx = new BattleContext { BM = this, Actor = actor, Target = target, Enemy = enemy };

        c.Play(ctx);
        handUI.Remove(c);
        discard.Add(c);

        if (enemy.Health.CurrentHP > 0) NextPartyOrEnemy();
        isBusy = false;
    }

    // Updates global GameState upon victory and manages progression items
    void OnEnemyDeath()
    {
        if (GameState.I != null)
        {
            if (!string.IsNullOrEmpty(GameState.I.currentEnemyId))
                GameState.I.MarkEnemyDefeated(GameState.I.currentEnemyId);

            GameState.I.AddKeyItem("key_fragment");
            GameState.I.ClearEncounter();
            GameState.I.pendingRespawn = true; 
        }

        resultPanel.SetActive(true);
        resultText.text = "Victory! Well done.";
        handArea.SetActive(false);
        handPanel.SetActive(false);
    }
}