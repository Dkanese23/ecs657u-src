using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;
    public Image fadeImage;
    public float fadeSpeed = 1.5f;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps fader alive between scenes
        } else {
            Destroy(gameObject);
        }
    }

    public void TransitionToScene(string sceneName) {
        StartCoroutine(FadeOutAndIn(sceneName));
    }

    IEnumerator FadeOutAndIn(string sceneName) {
        // Fade to Black
        float alpha = 0;
        while (alpha < 1) {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Load Scene in the background
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone) yield return null;

        // Fade to Clear
        while (alpha > 0) {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}