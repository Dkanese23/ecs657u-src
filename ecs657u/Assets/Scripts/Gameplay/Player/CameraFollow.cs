using UnityEngine;

public class CameraCollision_Rigged : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask collisionLayers = 1; // Default to 'Default' layer
    public float collisionBuffer = 0.2f;  // Small gap from wall
    public float sphereRadius = 0.2f;     // Size of the camera "head"

    // We store the initial distance (e.g., -4) to know where we WANT to be
    private float defaultDistance;
    private Vector3 defaultLocalPos;

    void Awake()
    {
        // Remember where the camera was placed in the editor
        defaultLocalPos = transform.localPosition;
        defaultDistance = defaultLocalPos.magnitude;
    }

    void LateUpdate()
    {
        // 1. Calculate the direction from the Pivot (Parent) to the Camera
        // Since this script is on the Camera, 'transform.parent' is the Pivot.
        if (transform.parent == null) return;
        
        Vector3 parentPos = transform.parent.position;
        Vector3 desiredPos = transform.parent.TransformPoint(defaultLocalPos);
        Vector3 direction = desiredPos - parentPos;
        float targetDist = direction.magnitude;

        // 2. Check for walls between Pivot and Desired Camera Position
        RaycastHit hit;
        if (Physics.SphereCast(parentPos, sphereRadius, direction.normalized, out hit, defaultDistance, collisionLayers))
        {
            // Hit a wall! Calculate new distance
            // We clamp it so it doesn't zoom inside the player's head (min 0.5f)
            float hitDist = Mathf.Max(hit.distance - collisionBuffer, 0.5f);
            
            // Move camera to the hit point locally
            transform.localPosition = defaultLocalPos.normalized * hitDist;
        }
        else
        {
            // No wall, return to default position smoothly
            // (Using a fast Lerp here helps smooth out tiny jagged wall edges)
            transform.localPosition = Vector3.Lerp(transform.localPosition, defaultLocalPos, Time.deltaTime * 10f);
        }
    }

    // Visualize the camera "Head" size
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}