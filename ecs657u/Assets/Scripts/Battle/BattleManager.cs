using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    public Health player;
    public Health enemy;

    [Header("UI")]
    public Text playerHPText;
    public Text enemyHPText;
    public Button attackButton;
    public Button endTurnButton;
    public GameObject resultPanel;
    public Text resultText;
    public Button returnButton;

    enum Turn { Player, Enemy }
    Turn turn;

    void Awake()
    {
        
        attackButton.onClick.AddListener(PlayerAttack);
        endTurnButton.onClick.AddListener(EndPlayerTurn);
        returnButton.onClick.AddListener(() => SceneManager.LoadScene("Main"));

        
        player.OnDeath += OnPlayerDeath;
        enemy.OnDeath += OnEnemyDeath;
        player.OnHealthChanged += (_, __) => RefreshHP();
        enemy.OnHealthChanged += (_, __) => RefreshHP();
    }

    void Start()
    {
    
    Time.timeScale = 1f;
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible   = true;

    
    if (FindObjectOfType<EventSystem>() == null)
    {
        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    RefreshHP();
    SetTurn(Turn.Player);
    Debug.Log("[BattleManager] Ready. Player turn.");
    }

    void SetTurn(Turn t)
    {
        turn = t;
        bool playerTurn = (turn == Turn.Player);
        attackButton.interactable = playerTurn;
        endTurnButton.interactable = playerTurn;

        if (!playerTurn)
            StartCoroutine(EnemyTurnRoutine());
    }

    void PlayerAttack()
    {
        if (turn != Turn.Player) return;
        Debug.Log("[Battle] Player attacks for 5.");
        enemy.TakeDamage(5);
    }

    void EndPlayerTurn()
    {
        if (turn != Turn.Player) return;
        Debug.Log("[Battle] Player ends turn.");
        SetTurn(Turn.Enemy);
    }

    IEnumerator EnemyTurnRoutine()
    {
        Debug.Log("[Battle] Enemy turn.");
        yield return new WaitForSeconds(0.4f);
        if (enemy.CurrentHP > 0)
        {
            Debug.Log("[Battle] Enemy attacks for 3.");
            player.TakeDamage(3);
        }
        yield return new WaitForSeconds(0.2f);
        if (player.CurrentHP > 0) SetTurn(Turn.Player);
    }

    void OnPlayerDeath()  => ShowResult("Defeat!");
    void OnEnemyDeath()   => ShowResult("Victory!");

    void ShowResult(string text)
    {
        attackButton.interactable = false;
        endTurnButton.interactable = false;
        resultPanel.SetActive(true);
        resultText.text = text;
        Debug.Log("[Battle] " + text);
    }

    void RefreshHP()
    {
        if (playerHPText) playerHPText.text = $"HP: {player.CurrentHP}";
        if (enemyHPText)  enemyHPText.text  = $"Enemy HP: {enemy.CurrentHP}";
    }
}
