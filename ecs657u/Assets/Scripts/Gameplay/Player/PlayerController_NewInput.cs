using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;  // needed for EventSystem


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController_NewInput : MonoBehaviour
{

    [Header("UI")]
    public GameObject inventoryUI; // drag your Inventory/Deck panel root here
    bool uiOpen = false;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Look")]
    public Transform camPivot;            // assign CamPivot here
    public Camera playerCamera;           // assign Main Camera (child of CamPivot)
    public float lookSensitivity = 0.1f;
    public float minPitch = -80f, maxPitch = 80f;
    public bool invertY = false;

    [Header("Interact (3rd-person)")]
    public Transform interactOrigin;
    public float interactRange = 3.0f;
    public float interactRadius = 0.45f;
    public LayerMask interactMask = ~0;

    CharacterController cc;
    PlayerInput playerInput;
    Vector2 moveInput;
    float yaw, pitch;

    InputAction interactAction;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        if (!playerCamera) playerCamera = Camera.main;

        
        if (!camPivot)
        {
            var t = transform.Find("CameraRig/CamPivot");
            if (t) camPivot = t;
        }

        if (playerInput && playerInput.actions != null)
            interactAction = playerInput.actions["Interact"];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        // ignore movement if pointer over UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = ctx.ReadValue<Vector2>();
        if (ctx.canceled) moveInput = Vector2.zero;
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        // ignore camera input if pointer over UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2 d = ctx.ReadValue<Vector2>();
        yaw += d.x * lookSensitivity;

        float vy = (invertY ? d.y : -d.y) * lookSensitivity;
        pitch = Mathf.Clamp(pitch + vy, minPitch, maxPitch);
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) TryInteractFromPlayer();
    }

    void OnEnable()  { interactAction?.Enable(); }
    void OnDisable() { interactAction?.Disable(); }

    void Update()
    {
        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y) * moveSpeed;
        cc.SimpleMove(move);

        if (interactAction != null && interactAction.WasPressedThisFrame())
            TryInteractFromPlayer();
    }

    void LateUpdate()
    {
        // Apply yaw on the player body
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        // Apply pitch on the pivot, not the camera
        if (camPivot)
            camPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        else
            Debug.LogWarning("camPivot not assigned/found. Create CameraRig/CamPivot under the player and assign it.");
    }

    
    void TryInteractFromPlayer()
    {
        Vector3 origin = interactOrigin
            ? interactOrigin.position
            : transform.position + Vector3.up * 1.3f;

        origin += transform.forward * 0.05f;

        Ray ray = new Ray(origin, transform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(
            ray, interactRadius, interactRange, interactMask, QueryTriggerInteraction.Collide
        );
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            if (!hit.transform) continue;
            if (hit.transform.IsChildOf(transform)) continue;

            var interactable = hit.transform.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(gameObject);
                Debug.Log($"[Interact] {hit.collider.name} -> Interact()");
                return;
            }
        }

        Debug.Log("[Interact] Nothing interactable in front.");
    }

    void OnDrawGizmosSelected()
    {
        Vector3 origin = interactOrigin
            ? interactOrigin.position
            : transform.position + Vector3.up * 1.3f;
        origin += transform.forward * 0.05f;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(origin, interactRadius);
        Gizmos.DrawLine(origin, origin + transform.forward * interactRange);
        Gizmos.DrawWireSphere(origin + transform.forward * interactRange, interactRadius);
    }


    public void OnToggleInventory(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        ToggleInventory();
    }

    void ToggleInventory()
    {
        uiOpen = !uiOpen;

        if (inventoryUI)
            inventoryUI.SetActive(uiOpen);

        // Cursor handling
        Cursor.lockState = uiOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = uiOpen;

        // Optional: stop movement/camera when UI open
        enabled = !uiOpen;  // disables this script while UI is open
    }


}
