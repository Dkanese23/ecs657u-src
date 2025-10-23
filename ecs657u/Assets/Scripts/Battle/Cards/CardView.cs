using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public Text title;
    public Text desc;
    public Button btn;

    public void Bind(CardBase card, System.Action onClick)
    {
        title.text = card.Title;
        desc.text  = card.Description;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => onClick?.Invoke());
    }
}
