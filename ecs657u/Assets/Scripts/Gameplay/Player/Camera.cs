using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new(0f, 2f, -4f);
    public float followSmooth = 10f;

    public bool reduceMotion;

    void LateUpdate()
    {
    if (!target) return;
    float s = reduceMotion ? 0f : followSmooth;
        Vector3 desired = target.position + target.rotation * offset;
        if (s <= 0f) transform.position = desired;
        else transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * s);
        
        transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
    
    
    }
}
