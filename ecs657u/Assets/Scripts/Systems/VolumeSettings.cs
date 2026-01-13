using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer mainMixer; // Drag your 'MainMixer' asset here

    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Set sliders to current mixer values on start
        // (Optional logic, usually you load from PlayerPrefs here)
        masterSlider.value = GetSliderValue("MasterVol");
        musicSlider.value = GetSliderValue("MusicVol");
        sfxSlider.value = GetSliderValue("SFXVol");
    }

    public void SetMasterVolume(float sliderValue)
    {
        // Logarithmic conversion
        mainMixer.SetFloat("MasterVol", Mathf.Log10(sliderValue) * 20);
    }

    public void SetMusicVolume(float sliderValue)
    {
        mainMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
    }

    public void SetSFXVolume(float sliderValue)
    {
        mainMixer.SetFloat("SFXVol", Mathf.Log10(sliderValue) * 20);
    }
    
    // Helper to get current volume back to slider value (Decibel to Linear)
    private float GetSliderValue(string paramName)
    {
        float value;
        bool result = mainMixer.GetFloat(paramName, out value);
        if(result)
        {
            return Mathf.Pow(10, value / 20);
        }
        return 1f;
    }
}