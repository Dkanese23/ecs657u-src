using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new(0f, 2f, -4f);
    public float followSmooth = 10f;
    public bool reduceMotion;

    [Header("Collision Settings")]
    public bool enableCollision = true;
    public LayerMask collisionLayers = -1;  // Which layers block camera
    public float collisionBuffer = 0.2f;    // Distance from walls
    public float collisionCheckRadius = 0.3f; // Sphere cast size

    void LateUpdate()
    {
        if (!target) return;

        float s = reduceMotion ? 0f : followSmooth;
        Vector3 desiredPosition = target.position + target.rotation * offset;

        // Handle collision if enabled
        Vector3 finalPosition = desiredPosition;
        if (enableCollision)
        {
            finalPosition = HandleCollision(target.position, desiredPosition);
        }

        // Smooth movement
        if (s <= 0f)
            transform.position = finalPosition;
        else
            transform.position = Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * s);

        // Look at target
        transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
    }

    Vector3 HandleCollision(Vector3 fromPosition, Vector3 toPosition)
    {
        Vector3 direction = toPosition - fromPosition;
        float desiredDistance = direction.magnitude;

        // Use SphereCast to check for collisions (better than Raycast for camera)
        RaycastHit hit;
        if (Physics.SphereCast(
            fromPosition,
            collisionCheckRadius,
            direction.normalized,
            out hit,
            desiredDistance,
            collisionLayers))
        {
            // Hit something - pull camera closer
            float safeDistance = Mathf.Max(hit.distance - collisionBuffer, 0.5f);
            return fromPosition + direction.normalized * safeDistance;
        }

        // No collision - use desired position
        return toPosition;
    }

    // Visualize collision detection in Scene view
    void OnDrawGizmosSelected()
    {
        if (!enableCollision || !target) return;

        Vector3 desiredPos = target.position + target.rotation * offset;
        Vector3 direction = desiredPos - target.position;

        // Draw line from target to camera
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(target.position, transform.position);

        // Draw desired position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(desiredPos, collisionCheckRadius);

        // Draw collision sphere at current position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, collisionCheckRadius);
    }
}