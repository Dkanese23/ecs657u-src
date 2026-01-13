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
    
    [Header("The Story")]
    [TextArea(3, 10)]
    public string[] storyPages; 

    private int currentPage = 0;
    private bool isTyping = false; 
    private Coroutine typeRoutine; 
    public AudioSource typingAudio;

    void Start()
    {
        // FIX 1: Assign the coroutine to the variable immediately
        typeRoutine = StartCoroutine(TypeStory(storyPages[currentPage]));
    }

    public void OnClickNext()
    {
        if (isTyping)
        {
            // FIX 2: Safety check to prevent errors if routine is missing
            if(typeRoutine != null) 
            {
                StopCoroutine(typeRoutine);
            }
            
            storyText.text = storyPages[currentPage];
            isTyping = false;
            return;
        }

        currentPage++;

        if (currentPage < storyPages.Length)
        {
            typeRoutine = StartCoroutine(TypeStory(storyPages[currentPage]));
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    IEnumerator TypeStory(string pageText)
    {
        isTyping = true;
        storyText.text = ""; 

        int charCount = 0; // Counter for sound logic

        foreach (char letter in pageText.ToCharArray())
        {
            storyText.text += letter;
            charCount++;

            // FIX 3: Changed % 3 to % 7 (plays sound every 7th letter)
            // You can increase this number to make it even quieter (e.g. 10)
            if (typingAudio != null && charCount % 4 == 0)
            {
                // Randomize pitch slightly to make it sound less robotic
                typingAudio.pitch = Random.Range(0.9f, 1.1f);
                typingAudio.PlayOneShot(typingAudio.clip);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}