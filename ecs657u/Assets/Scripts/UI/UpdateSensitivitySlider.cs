using UnityEngine;
using UnityEngine.UI;

public class UpdateSensitivitySlider : MonoBehaviour
{
    public PlayerController_NewInput player; 
    private Slider mySlider;

    void OnEnable()
    {
        mySlider = GetComponent<Slider>();
        if(player != null && mySlider != null)
        {
            mySlider.value = player.lookSensitivity;
        }
    }
}