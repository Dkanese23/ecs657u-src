using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class LoadBattleOnTrigger : MonoBehaviour
{
    [SerializeField] string battleSceneName = "Battle";

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        if (!TryGetComponent<Rigidbody>(out var rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(battleSceneName, LoadSceneMode.Single);
        }
    }
}

