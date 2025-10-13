using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookSensitivity = 1.5f;
    CharacterController cc;
    Vector2 moveInput, lookInput;
    float yaw;

    void Awake() => cc = GetComponent<CharacterController>();

    public void OnMove(InputValue v) => moveInput = v.Get<Vector2>();
    public void OnLook(InputValue v) => lookInput = v.Get<Vector2>();

    void Update()
    {
        // Move
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = transform.TransformDirection(move) * moveSpeed;
        cc.SimpleMove(move);

        // Look (yaw only for placeholder)
        yaw += lookInput.x * lookSensitivity;
        transform.rotation = Quaternion.Euler(0, yaw, 0);
    }
}

