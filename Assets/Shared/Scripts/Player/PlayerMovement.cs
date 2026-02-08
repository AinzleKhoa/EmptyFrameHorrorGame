using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movement Speeds")]
    [SerializeField] private float _walkSpeed = 4.5f;
    [SerializeField] private float _sprintSpeed = 8f;
    [SerializeField] private float _maxVelocityChange = 10f;
    [SerializeField] private float _speedOnCamera = 3f;


    [Header("Jump & Crouch Settings")]
    [SerializeField] private float _jumpPower = 5f;
    [SerializeField] private float _crouchHeight = 0.75f;
    [SerializeField] private float _crouchSpeedReduction = 0.5f;
    [SerializeField] private bool _enableJump = false;
    [SerializeField] private bool _enableCrouch = true;

    // Internal States (Other scripts like HeadBob or UI can read these)
    [HideInInspector] public bool isWalking, isSprinting, isCrouched, isGrounded;
    [HideInInspector] public bool isOnCamera = false;

    private Vector3 originalScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        originalScale = transform.localScale;
    }

    private void Update()
    {
        // 1. Check ground
        CheckGround();
        // 2. Handle Jump Input
        if (_enableJump && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            Jump();
        }
        // 3. Handle Crouch Input
        if (_enableCrouch && Keyboard.current.leftCtrlKey.wasPressedThisFrame)
        {
            ToggleCrouch();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }


    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * 0.5f), transform.position.z);
        // Cast a ray sightly below player's feet
        isGrounded = Physics.Raycast(origin, Vector3.down, 0.75f);
    }

    private void ToggleCrouch()
    {
        isCrouched = !isCrouched;
        transform.localScale = isCrouched
                                ? new Vector3(originalScale.x, _crouchHeight, originalScale.z)
                                : originalScale;
    }

    private void Jump()
    {
        rb.AddForce(0f, _jumpPower, 0f, ForceMode.Impulse);
        isGrounded = false;
    }

    private void MovePlayer()
    {
        // 1. Get Input (Modern Input System)
        float horizontal = (Keyboard.current.dKey.isPressed ? 1f : 0f) - (Keyboard.current.aKey.isPressed ? 1f : 0f);
        float vertical = (Keyboard.current.wKey.isPressed ? 1f : 0f) - (Keyboard.current.sKey.isPressed ? 1f : 0f);

        Vector2 input = new Vector2(horizontal, vertical);

        // 2. Check if we are moving
        isWalking = input.sqrMagnitude > 0.01f && isGrounded;
        float currentSpeed = _walkSpeed;

        if (isOnCamera)
        {
            // 1. Force sprinting off
            isSprinting = false;

            // 2. Slow the player down
            currentSpeed = _speedOnCamera;
        }
        else
        {
            bool canSprint = Keyboard.current.leftShiftKey.isPressed && isWalking && !isCrouched;
            isSprinting = canSprint;

            currentSpeed = isSprinting ? _sprintSpeed : _walkSpeed;
            if (isCrouched) currentSpeed *= _crouchSpeedReduction;
        }

        // 4. Calculate Direction (Relative to where the player is facing)
        Vector3 moveDir = (transform.forward * input.y) + (transform.right * input.x);
        Vector3 targetVelocity = moveDir.normalized * currentSpeed;

        // 5. Apply Force using linearVelocity (Version 1.0.2 Fix)
        Vector3 velocity = rb.linearVelocity;
        Vector3 velocityChange = (targetVelocity - velocity);

        // Only affect X and Z axis, leave Y (Gravity) alone
        velocityChange.x = Mathf.Clamp(velocityChange.x, -_maxVelocityChange, _maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -_maxVelocityChange, _maxVelocityChange);
        velocityChange.y = 0;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }
}