using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;  
using TMPro; 
using UnityEngine.UI; 

// Advanced Player Controller using the New Input System for movement, camera, and UI management
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController_NewInput : MonoBehaviour
{
    [Header("UI & Accessibility")]
    public GameObject inventoryUI; 
    public GameObject rebindingPanel;
    public GameObject pausePanel;
    public GameObject contrastVolume; // Accessibility: High Contrast Volume
    public GameObject controlsOverlay;
    public Text overlayButtonText;
    public Text contrastButtonText;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Camera Control")]
    public Transform camPivot;            
    public Camera playerCamera;           
    public float lookSensitivity = 0.1f;
    public float minPitch = -80f, maxPitch = 80f;
    public bool invertY = false;

    [Header("Interaction System")]
    public Transform interactOrigin;
    public float interactRange = 3.0f;
    public float interactRadius = 0.45f;
    public LayerMask interactMask = ~0;

    [Header("Animation")]
    public Animator animator;

    CharacterController cc;
    PlayerInput playerInput;
    Vector2 moveInput;
    float yaw, pitch;
    InputAction interactAction;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        // Dynamic reference fetching to ensure the prefab works in any scene
        if (!playerCamera) playerCamera = Camera.main;
        if (!camPivot)
        {
            var t = transform.Find("CameraRig/CamPivot");
            if (t) camPivot = t;
        }

        if (playerInput && playerInput.actions != null)
            interactAction = playerInput.actions["Interact"];

        // State Initialisation: Default to locked cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        ResumeGame(); // Ensure all UI is closed on start
        lookSensitivity = PlayerPrefs.GetFloat("LookSensitivity", 0.09f);
    }

    //  INPUT ACTION CALLBACKS 

    public void OnMove(InputAction.CallbackContext ctx)
    {
        // Prevents character movement while interacting with UI menus
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            moveInput = Vector2.zero;
            return;
        }
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2 d = ctx.ReadValue<Vector2>();
        yaw += d.x * lookSensitivity;

        float vy = (invertY ? d.y : -d.y) * lookSensitivity;
        pitch = Mathf.Clamp(pitch + vy, minPitch, maxPitch);
    }

    void Update()
    {
        // Physics-based movement using CharacterController for consistent collision
        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y) * moveSpeed;
        cc.SimpleMove(move);

        // Syncs movement magnitude to Animator for seamless locomotion
        if (animator != null)
            animator.SetFloat("Speed", moveInput.magnitude);
    }

    void LateUpdate()
    {
        // Rotates the player body for movement and the pivot for camera pitch
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (camPivot)
            camPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    //  INTERACTION SYSTEM 

    // Uses a SphereCastAll to detect interactable objects with a generous buffer
    void TryInteractFromPlayer()
    {
        Vector3 origin = interactOrigin ? interactOrigin.position : transform.position + Vector3.up * 1.3f;
        Ray ray = new Ray(origin, transform.forward);
        
        RaycastHit[] hits = Physics.SphereCastAll(ray, interactRadius, interactRange, interactMask);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            var interactable = hit.transform.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(gameObject); // Polymorphic interaction
                return;
            }
        }
    }

    //  UI & STATE MANAGEMENT 

    // Global state switcher: Handles cursor locking and script execution
    void SetPlayerControl(bool hasControl)
    {
        enabled = hasControl; // Disables Update/LateUpdate to freeze movement
        Cursor.lockState = hasControl ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !hasControl;

        if (!hasControl && cc != null)
            cc.SimpleMove(Vector3.zero);
    }

    // Logic for returning to gameplay from any menu state
    public void ResumeGame()
    {
        inventoryUI?.SetActive(false);
        rebindingPanel?.SetActive(false);
        pausePanel?.SetActive(false);
        SetPlayerControl(true);
    }
}