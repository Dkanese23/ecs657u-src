using UnityEngine;
using UnityEngine.InputSystem;

// Basic Player Controller using the New Input System for movement and rotation
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float lookSensitivity = 1.5f;

    // Internal references and state variables
    CharacterController cc;
    Vector2 moveInput, lookInput;
    float yaw;

    // Caches the CharacterController component on startup for performance optimisation
    void Awake() => cc = GetComponent<CharacterController>();

    // Input Message Handlers: Automatically called by the PlayerInput component
    public void OnMove(InputValue v) => moveInput = v.Get<Vector2>();
    public void OnLook(InputValue v) => lookInput = v.Get<Vector2>();

    void Update()
    {
        // 1. Locomotion Logic
        // Convert 2D input into a 3D movement vector relative to the player's facing direction
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = transform.TransformDirection(move) * moveSpeed;
        
        // SimpleMove handles gravity automatically, which is ideal for flat overworld navigation
        cc.SimpleMove(move);

        // 2. Rotation Logic (Horizontal Look)
        // Add horizontal look input to rotate the player character's body
        yaw += lookInput.x * lookSensitivity;
        transform.rotation = Quaternion.Euler(0, yaw, 0);
    }
}