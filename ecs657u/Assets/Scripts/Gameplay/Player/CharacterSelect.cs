using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    // Called when player selects Male
    public void SelectMale()
    {
        PlayerPrefs.SetString("HeroGender", "Male"); // Save choice
        SceneManager.LoadScene("OverworldScene"); // Load overworld
    }

    // Called when player selects Female
    public void SelectFemale()
    {
        PlayerPrefs.SetString("HeroGender", "Female");
        SceneManager.LoadScene("OverworldScene");
    }

    // Called when player selects Other
    public void SelectOther()
    {
        PlayerPrefs.SetString("HeroGender", "Other");
        SceneManager.LoadScene("OverworldScene");
    }
}
