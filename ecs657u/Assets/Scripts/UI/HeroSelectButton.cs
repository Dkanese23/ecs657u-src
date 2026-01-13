using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroSelectButton : MonoBehaviour
{
    public HeroData heroData;
    public string overworldSceneName = "Main(prototype)"; // replace with your actual scene name

    public void SelectHero()
    {
        // Assign the chosen hero
        SelectedHero.chosenHero = heroData;

        // Directly load the next scene without fancy fading
        SceneManager.LoadScene(overworldSceneName);
    }
}
