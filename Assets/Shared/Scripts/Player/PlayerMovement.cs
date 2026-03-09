using UnityEngine;
using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 _moveInput; // Stores WASD movement
    private bool _isSprintingInput = false;


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

    [Header("HUD & Stamina settings")]
    [SerializeField] private float _maxStamina = 100f;
    [SerializeField] private float _drainRate = 20f;   // How fast stamina drops
    [SerializeField] private float _regenRate = 15f;   // How fast it comes back
    private float _currentStamina;

    [Header("Out of Breath Settings")]
    [SerializeField] private float _exhaustedSpeed = 2f;      // Very slow movement
    [SerializeField] private float _recoveryThreshold = 40f; // Must reach certain percent to run again
    public bool isExhausted = false;

    [Header("Upgrade Status")]
    public bool isSilenced = false; // Set by Upgrade

    // Internal States (Other scripts like HeadBob or UI can read these)
    [HideInInspector] public bool isWalking, isSprinting, isCrouched, isGrounded;
    private bool _isUsingCamera = false; // Local state updated by events

    private Vector3 originalScale;

    public void OnMovement(InputValue value) => _moveInput = value.Get<Vector2>();

    public void OnSprint(InputValue value) => _isSprintingInput = value.isPressed;

    public void OnJump()
    {
        if (_enableJump && isGrounded) Jump();
    }

    public void OnCrouch()
    {
        if (_enableCrouch) ToggleCrouch();
    }

    private void FixedUpdate()
    {
        CheckGround();
        MovePlayer();
        HandleNoiseEmission(); // Moved from Update for better physics sync
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        originalScale = transform.localScale;
        _currentStamina = _maxStamina; // Start full
    }

    // --- NEW EVENT SYSTEM LOGIC ---
    private void OnEnable()
    {
        // Subscribe to the camera toggle event
        GameBroadcast.OnCameraToggle += HandleCameraStateChange;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        GameBroadcast.OnCameraToggle -= HandleCameraStateChange;
    }

    // Handles the signal from the camera
    private void HandleCameraStateChange(bool isOnCamera)
    {
        _isUsingCamera = isOnCamera;
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

        // Calculate how much we are shrinking
        float heightDifference = originalScale.y - _crouchHeight;

        if (isCrouched)
        {
            // 1. Shrink the player
            transform.localScale = new Vector3(originalScale.x, _crouchHeight, originalScale.z);

            // 2. Lower the position so the feet stay on the floor 
            // We move down by half the difference because scaling happens from the center
            transform.position -= new Vector3(0, heightDifference / 2f, 0);
        }
        else
        {
            // 1. Return to full size
            transform.localScale = originalScale;

            // 2. Raise the position back up
            transform.position += new Vector3(0, heightDifference / 2f, 0);
        }
    }

    private void Jump()
    {
        rb.AddForce(0f, _jumpPower, 0f, ForceMode.Impulse);
        isGrounded = false;
    }

    private void MovePlayer()
    {
        // 2. Check if we are moving
        isWalking = _moveInput.sqrMagnitude > 0.01f && isGrounded;
        float currentSpeed;

        if (_isUsingCamera)
        {
            isSprinting = false;
            currentSpeed = _speedOnCamera;
        }
        else
        {
            // Use the stored message value instead of Keyboard.current
            isSprinting = _isSprintingInput && isWalking && !isCrouched && !isExhausted;

            if (isExhausted) currentSpeed = _exhaustedSpeed;
            else currentSpeed = isSprinting ? _sprintSpeed : _walkSpeed;

            StaminaLogic(_moveInput);

            if (isCrouched) currentSpeed *= _crouchSpeedReduction;
        }

        // 4. Calculate Direction (Relative to where the player is facing)
        Vector3 moveDir = (transform.forward * _moveInput.y) + (transform.right * _moveInput.x);
        Vector3 targetVelocity = moveDir.normalized * currentSpeed;

        // 5. Apply Force using linearVelocity (Version 1.0.2 Fix)
        Vector3 velocity = rb.linearVelocity;
        Vector3 velocityChange = targetVelocity - velocity;

        // Only affect X and Z axis, leave Y (Gravity) alone
        velocityChange.x = Mathf.Clamp(velocityChange.x, -_maxVelocityChange, _maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -_maxVelocityChange, _maxVelocityChange);
        velocityChange.y = 0;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void HandleNoiseEmission()
    {
        // Crouching makes no noise at all, so we check that first
        if (isCrouched)
        {
            GameBroadcast.OnPlayerMovementState?.Invoke("Crouched");
            return;
        }

        // If we have the silence upgrade, we always broadcast "Idle" (no noise)
        if (isSilenced)
        {
            GameBroadcast.OnPlayerMovementState?.Invoke("Idle");
            return;
        }

        // Get the horizontal speed (ignoring falling/jumping speed)
        float horizontalSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

        // Define thresholds based on your speeds
        // _walkSpeed is 4.5f, _sprintSpeed is 8f
        bool isMakingRunningNoise = horizontalSpeed > 5.5f;
        bool isMakingWalkingNoise = horizontalSpeed > 0.1f && !isMakingRunningNoise;

        // We only send a signal if the player is physically moving
        if (isMakingRunningNoise)
        {
            GameBroadcast.OnPlayerMovementState?.Invoke("Sprinting");
        }
        else if (isMakingWalkingNoise)
        {
            GameBroadcast.OnPlayerMovementState?.Invoke("Walking");
        }
        else
        {
            GameBroadcast.OnPlayerMovementState?.Invoke("Idle");
        }
    }

    private void StaminaLogic(Vector2 input)
    {
        // Handle Draining and Regeneration
        if (isSprinting && input.sqrMagnitude > 0.01f && !isExhausted) _currentStamina -= _drainRate * Time.deltaTime;
        else _currentStamina += _regenRate * Time.deltaTime;

        // Logic for entering/exiting Exhaustion
        if (_currentStamina <= 0 && !isExhausted) isExhausted = true;
        if (isExhausted && _currentStamina >= _recoveryThreshold) isExhausted = false;

        // Player must rest until threshold to stop being exhausted
        if (isExhausted && _currentStamina >= _recoveryThreshold)
        {
            isExhausted = false;
        }

        // Keep stamina between 0 and Max
        _currentStamina = Mathf.Clamp(_currentStamina, 0, _maxStamina);

        // Update HUD bar
        GameBroadcast.OnStaminaUpdate?.Invoke(_currentStamina, _maxStamina);
    }

    // UPGRADE LOGIC ===============================================================
    public void UpgradeMaxStamina(float amount)
    {
        _maxStamina += amount;
        _currentStamina = _maxStamina; // Fill it up on upgrade
        Debug.Log($"<color=green>Stamina Upgraded!</color> New Max: {_maxStamina}");
    }

    public void UpgradeRegenRate(float amount)
    {
        _regenRate += amount;
        Debug.Log($"<color=green>Regen Upgraded!</color> New Rate: {_regenRate}");
    }

    public void UnlockSilentFeet()
    {
        isSilenced = true;
        Debug.Log("<color=cyan>Silence Unlocked:</color> Player is now completely silent.");
    }
}