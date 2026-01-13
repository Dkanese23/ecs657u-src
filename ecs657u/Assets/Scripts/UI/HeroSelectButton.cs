using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroSelectButton : MonoBehaviour
{
    public HeroData heroData;
    public string overworldSceneName = "Overworld";

    public void SelectHero()
    {
        SelectedHero.chosenHero = heroData;
        SceneFader.Instance.TransitionToScene("Main(prototype)");
    }
}
