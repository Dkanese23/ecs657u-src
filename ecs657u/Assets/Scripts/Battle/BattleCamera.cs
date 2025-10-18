using UnityEngine;

/// Simple orbit/zoom camera that frames the active unit and the enemy.
/// No Input System dependency; optional legacy mouse/scroll support.
[RequireComponent(typeof(Camera))]
public class BattleCamera : MonoBehaviour
{
    public Transform focusA;              // active party member
    public Transform focusB;              // enemy
    public Vector3 offset = new(0, 2.0f, 0);  // elevate look point
    public float orbitDegreesPerSec = 15f;
    public float manualOrbitSensitivity = 120f;   // hold RMB + move
    public float minDistance = 6f, maxDistance = 14f;
    public float zoomSpeed = 6f;
    public float followSmooth = 10f;

    float yaw;
    float targetDist = 10f;
    float dist;
    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        dist = targetDist = Mathf.Clamp(targetDist, minDistance, maxDistance);
    }

    public void SetFocus(Transform a, Transform b)
    {
        focusA = a; focusB = b;
    }

    Vector3 GetFocusPoint()
    {
        if (focusA && focusB) return Vector3.Lerp(focusA.position, focusB.position, 0.5f) + offset;
        if (focusA) return focusA.position + offset;
        if (focusB) return focusB.position + offset;
        return offset;
    }

    void Update()
    {
        // Optional legacy input (safe even if you use new Input System elsewhere)
        if (Input.GetMouseButton(1))
            yaw += Input.GetAxis("Mouse X") * manualOrbitSensitivity * Time.deltaTime;
        else
            yaw += orbitDegreesPerSec * Time.deltaTime;

        float scroll = Input.mouseScrollDelta.y; // wheel zoom
        if (Mathf.Abs(scroll) > 0.01f)
            targetDist = Mathf.Clamp(targetDist - scroll * zoomSpeed, minDistance, maxDistance);
    }

    void LateUpdate()
    {
        dist = Mathf.Lerp(dist, targetDist, Time.deltaTime * 10f);

        Vector3 focus = GetFocusPoint();
        Quaternion rot = Quaternion.Euler(15f, yaw, 0f); // slight pitch down for clarity
        Vector3 desiredPos = focus + rot * new Vector3(0, 0, -dist);

        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSmooth);
        transform.rotation = Quaternion.LookRotation(focus - transform.position, Vector3.up);
    }
}
