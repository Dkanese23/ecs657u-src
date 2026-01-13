using UnityEngine;

public class MusicStartOffset : MonoBehaviour
{
    public AudioSource musicSource;
    public float startTimeInSeconds = 15.0f; // Example: Start 15 seconds in

    void Start()
    {
        if (musicSource != null)
        {
            // Jump to the specific timestamp
            musicSource.time = startTimeInSeconds;
            
            // Now play the music
            musicSource.Play();
        }
    }
}