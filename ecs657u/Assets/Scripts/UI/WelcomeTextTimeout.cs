using UnityEngine;

public class WelcomeTextTimeout : MonoBehaviour
{
    public float timeoutSeconds = 5f;
    float timer;

    void Awake()
    {
        // If we've shown it before, remove immediately
        if (PlayerPrefs.GetInt("WelcomeShown", 0) == 1)
        {
            Destroy(gameObject);
        }
        else
        {
            // Mark as shown for next time
            PlayerPrefs.SetInt("WelcomeShown", 1);
            PlayerPrefs.Save();
        }
    }

    void Update()
    {
        timer += Time.unscaledDeltaTime; // unaffected by timescale
        if (timer >= timeoutSeconds)
            Destroy(gameObject);
    }
}
