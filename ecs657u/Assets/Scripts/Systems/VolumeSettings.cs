using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// Connects UI sliders to the AudioMixer using logarithmic decibel scaling
public class VolumeSettings : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer mainMixer; 

    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Sync sliders with mixer state on menu open
        masterSlider.value = GetSliderValue("MasterVol");
        musicSlider.value = GetSliderValue("MusicVol");
        sfxSlider.value = GetSliderValue("SFXVol");
    }

    // Standard volume formula: dB = 20 * log10(LinearValue)
    public void SetMasterVolume(float sliderValue)
    {
        // We use a small clamping or ensure slider min is 0.0001 to avoid log10(0) errors
        mainMixer.SetFloat("MasterVol", Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20);
    }

    public void SetMusicVolume(float sliderValue)
    {
        mainMixer.SetFloat("MusicVol", Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20);
    }

    public void SetSFXVolume(float sliderValue)
    {
        mainMixer.SetFloat("SFXVol", Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20);
    }
    
    // Decibel to Linear conversion for UI syncing
    private float GetSliderValue(string paramName)
    {
        float value;
        if(mainMixer.GetFloat(paramName, out value))
        {
            return Mathf.Pow(10, value / 20);
        }
        return 1f;
    }
}