using UnityEngine;

// Provides a smooth third-person camera following the player during overworld exploration
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new(0f, 2f, -4f);
    public float followSmooth = 10f;

    [Header("Accessibility")]
    [Tooltip("If enabled, camera snaps instantly to target to assist users with motion sensitivity")]
    public bool reduceMotion;

    // LateUpdate is the industry standard for cameras to ensure the target has finished moving
    void LateUpdate()
    {
        if (!target) return;

        // Determine smoothing based on accessibility settings
        float s = reduceMotion ? 0f : followSmooth;

        // Calculate the desired position relative to the target's current rotation
        Vector3 desired = target.position + target.rotation * offset;

        if (s <= 0f) 
        {
            // Instant snap for accessibility or testing
            transform.position = desired;
        }
        else 
        {
            // Linear interpolation (Lerp) for high-quality, polished movement
            transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * s);
        }
        
        // Ensure the camera always faces the target point
        transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
    }
}