using UnityEngine;

public class Totem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Tell the manager we found one!
            ShamanTrialManager.Instance.CollectTotem();
            Destroy(gameObject); // Remove from scene
        }
    }
}