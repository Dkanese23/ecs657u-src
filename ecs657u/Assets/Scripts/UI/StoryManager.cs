using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections; 

public class StoryManager : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI storyText;
    public GameObject nextButton; 

    [Header("Settings")]
    public string sceneToLoad = "MainScene";
    public float typingSpeed = 0.04f; 
    
    [Header("The Story Content")]
    [TextArea(3, 10)] // Makes it easier to write long paragraphs in the Inspector
    public string[] storyPages; 

    private int currentPage = 0;
    private bool isTyping = false; 
    private Coroutine typeRoutine; 
    public AudioSource typingAudio;

    void Start()
    {
        // Begin the first page immediately on load
        typeRoutine = StartCoroutine(TypeStory(storyPages[currentPage]));
    }

    public void OnClickNext()
    {
        // UX Logic: If still typing, finish the page instantly
        if (isTyping)
        {
            if(typeRoutine != null) StopCoroutine(typeRoutine);
            
            storyText.text = storyPages[currentPage];
            isTyping = false;
            return;
        }

        currentPage++;

        // Either move to the next page or transition to the main game
        if (currentPage < storyPages.Length)
        {
            typeRoutine = StartCoroutine(TypeStory(storyPages[currentPage]));
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    // Handles the character-by-character typing logic
    IEnumerator TypeStory(string pageText)
    {
        isTyping = true;
        storyText.text = ""; 

        int charCount = 0;

        foreach (char letter in pageText.ToCharArray())
        {
            storyText.text += letter;
            charCount++;

            // Audio Logic: Play sound every few letters to avoid "noise fatigue"
            if (typingAudio != null && charCount % 4 == 0)
            {
                typingAudio.pitch = Random.Range(0.9f, 1.1f);
                typingAudio.PlayOneShot(typingAudio.clip);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}