using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

// Handles smooth visual transitions between game scenes
public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;
    public Image fadeImage;
    public float fadeSpeed = 1.5f;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        } else {
            Destroy(gameObject);
        }
    }

    // Call this from other scripts instead of SceneManager.LoadScene()
    public void TransitionToScene(string sceneName) {
        StartCoroutine(FadeOutAndIn(sceneName));
    }

    // Coroutine allows for time-based logic without blocking the main thread
    IEnumerator FadeOutAndIn(string sceneName) {
        float alpha = 0;
        
        // 1. Fade out to black
        while (alpha < 1) {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null; // Wait for the next frame
        }

        // 2. Load the scene asynchronously in the background
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone) yield return null;

        // 3. Fade back in to the new scene
        while (alpha > 0) {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}