using UnityEngine;

// Prevents the camera from clipping through environment geometry during overworld exploration
public class CameraCollision_Rigged : MonoBehaviour
{
    [Header("Collision Settings")]
    public LayerMask collisionLayers = 1; // Default to 'Default' layer
    public float collisionBuffer = 0.2f;  // Gap maintained between the camera and the wall surface
    public float sphereRadius = 0.2f;     // Radius of the SphereCast to detect wide collisions

    // Stores initial placement data to determine the 'ideal' viewing distance
    private float defaultDistance;
    private Vector3 defaultLocalPos;

    void Awake()
    {
        // Captures the editor-set position to use as the baseline for recovery logic
        defaultLocalPos = transform.localPosition;
        defaultDistance = defaultLocalPos.magnitude;
    }

    // LateUpdate ensures the player has moved before we calculate collision
    void LateUpdate()
    {
        // Safety check to ensure the camera is correctly parented to a pivot point
        if (transform.parent == null) return;
        
        Vector3 parentPos = transform.parent.position;
        Vector3 desiredPos = transform.parent.TransformPoint(defaultLocalPos);
        Vector3 direction = desiredPos - parentPos;

        // 2. Raycast/SphereCast logic: Probes for obstacles between pivot and camera
        RaycastHit hit;
        
        // Using SphereCast instead of Raycast provides a 'thicker' check to prevent corner clipping
        if (Physics.SphereCast(parentPos, sphereRadius, direction.normalized, out hit, defaultDistance, collisionLayers))
        {
            // Collision detected: reposition camera to the point of impact
            // Clamped at 0.5m to prevent the camera from entering the player's character model
            float hitDist = Mathf.Max(hit.distance - collisionBuffer, 0.5f);
            
            transform.localPosition = defaultLocalPos.normalized * hitDist;
        }
        else
        {
            // Clear view: return smoothly to the default local position using interpolation
            transform.localPosition = Vector3.Lerp(transform.localPosition, defaultLocalPos, Time.deltaTime * 10f);
        }
    }

    // Debugging tool to visualise the collision volume in the Unity Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}