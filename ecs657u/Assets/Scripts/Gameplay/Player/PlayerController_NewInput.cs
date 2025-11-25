using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;  // needed for EventSystem


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController_NewInput : MonoBehaviour
{

    [Header("UI")]
    public GameObject inventoryUI; // drag your Inventory/Deck panel root here
    public GameObject rebindingPanel;
    public GameObject pausePanel;
    public GameObject contrastVolume;

    public GameObject settingsPanel;

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
        
        // Disable all panels on start
        inventoryUI?.SetActive(false);
        rebindingPanel?.SetActive(false);
        pausePanel?.SetActive(false);
        settingsPanel?.SetActive(false); // <-- NEW
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

    void OnEnable() { interactAction?.Enable(); }
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

    // ##################################################################
    // #region UI MANAGEMENT
    // ##################################################################

    // --- INPUT ACTION HANDLERS ---

    public void OnToggleInventory(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        ToggleInventory();
    }

    public void OnToggleRebinding(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        ToggleRebinding();
    }

    // NEW: Handler for the "TogglePause" action
    public void OnTogglePause(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        TogglePause();
    }


    // --- TOGGLE LOGIC (Called by Input Actions) ---

    void ToggleInventory()
    {
        if (!inventoryUI) return; 
        
        bool desiredState = !inventoryUI.activeSelf;

        // Close all other panels
        if (desiredState == true)
        {
            rebindingPanel?.SetActive(false);
            pausePanel?.SetActive(false);
            settingsPanel?.SetActive(false); // <-- UPDATED
        }
        
        inventoryUI.SetActive(desiredState);
        SetPlayerControl(!desiredState);
    }

    void ToggleRebinding()
    {
        if (!rebindingPanel) return; 

        bool desiredState = !rebindingPanel.activeSelf;
        
        // Close all other panels
        if (desiredState == true)
        {
            inventoryUI?.SetActive(false);
            pausePanel?.SetActive(false);
            settingsPanel?.SetActive(false);  // <-- UPDATED
        }
        
        rebindingPanel.SetActive(desiredState);
        SetPlayerControl(!desiredState);
    }

    // NEW: Logic for toggling the pause menu
    void TogglePause()
    {
        if (!pausePanel) return;

        bool desiredState = !pausePanel.activeSelf;

        // Close all other panels
        if (desiredState == true)
        {
            inventoryUI?.SetActive(false);
            rebindingPanel?.SetActive(false);
        }

        pausePanel.SetActive(desiredState);
        SetPlayerControl(!desiredState);
    }

    // --- PUBLIC UI BUTTON FUNCTIONS ---
    // NEW: These functions are for your UI Buttons to call from OnClick()

    /// <summary>
    /// Closes all UI panels and returns to the game.
    /// Call this from your "Resume" button.
    /// </summary>
    public void ResumeGame()
    {
        inventoryUI?.SetActive(false);
        rebindingPanel?.SetActive(false);
        pausePanel?.SetActive(false);
        settingsPanel?.SetActive(false); 
        
        SetPlayerControl(true);
    }

    /// <summary>
    /// Shows the Inventory panel from another UI panel (e.g., Pause Menu).
    /// </summary>
    public void ShowInventoryPanel()
    {
        inventoryUI?.SetActive(true);
        rebindingPanel?.SetActive(false);
        pausePanel?.SetActive(false);
        settingsPanel?.SetActive(false); 
        
        // We are still in a UI, so player control remains OFF
        SetPlayerControl(false); 
    }

    /// <summary>
    /// Shows the Rebinding panel from another UI panel (e.g., Pause Menu).
    /// </summary>
    public void ShowRebindingPanel()
    {
        inventoryUI?.SetActive(false);
        rebindingPanel?.SetActive(true);
        pausePanel?.SetActive(false);
        settingsPanel?.SetActive(false); 

        // We are still in a UI, so player control remains OFF
        SetPlayerControl(false);
    }
    
    /// <summary>
    /// Shows the Pause panel from another UI panel (e.g., a "Back" button).
    /// </summary>
    public void ShowPausePanel()
    {
        inventoryUI?.SetActive(false);
        rebindingPanel?.SetActive(false);
        pausePanel?.SetActive(true);
        settingsPanel?.SetActive(false); 

        // We are still in a UI, so player control remains OFF
        SetPlayerControl(false);
    }

    public void ShowSettingsPanel()
    {
        inventoryUI?.SetActive(false);
        rebindingPanel?.SetActive(false);
        pausePanel?.SetActive(false);
        settingsPanel?.SetActive(true); 

        // We are still in a UI, so player control remains OFF
        SetPlayerControl(false);
    }

        public void ToggleHighContrastMode()
    {
        if (contrastVolume.activeSelf)
        {
            contrastVolume?.SetActive(false);
        }
        else
        {
            contrastVolume?.SetActive(true);
        }

        // We are still in a UI, so player control remains OFF
        SetPlayerControl(false); 
    }

    // --- CORE CONTROL FUNCTION ---

    /// <summary>
    /// Enables or disables player movement and camera control.
    /// </summary>
    /// <param name="hasControl">True to give control, False to take it away (for UI)</param>
    void SetPlayerControl(bool hasControl)
    {
        // This stops Update/LateUpdate, freezing movement and camera
        enabled = hasControl; 
        
        // Handle cursor
        Cursor.lockState = hasControl ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !hasControl;

        // Just in case, stop movement when UI opens
        if (!hasControl)
        {
            moveInput = Vector2.zero;
            // Check if cc is not null before calling SimpleMove
            if(cc != null) 
            {
                cc.SimpleMove(Vector3.zero); // Stop any residual movement
            }
        }
    }
    // #endregion
}