using System.Collections.Generic;
using UnityEngine;

public class ClueLog : MonoBehaviour
{
    public static ClueLog Instance { get; private set; }
    HashSet<string> clues = new();
    [SerializeField] int cluesToWin = 3;

    void Awake(){ if (Instance && Instance!=this) Destroy(gameObject); else { Instance=this; DontDestroyOnLoad(gameObject);} }

    public void RegisterClue(string id)
    {
        if (clues.Add(id))
            Debug.Log($"Clue added: {id} ({clues.Count}/{cluesToWin})");
        if (clues.Count >= cluesToWin)
            Debug.Log("All clues found! You win!");
    }
}

