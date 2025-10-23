using UnityEngine;

public interface IInteractable
{
    void Interact(GameObject interactor);
}

public class CluePickup : MonoBehaviour, IInteractable
{
    [SerializeField] private string clueId;

    public void Interact(GameObject interactor)
    {
        ClueLog.Instance.RegisterClue(clueId);
        Destroy(gameObject);
    }
}
