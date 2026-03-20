using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public class PlayerCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private Transform _joint;
    [SerializeField] private Camera _cam;

    [Header("Rotation")]
    [SerializeField] private float _mouseSensitivity = 1f;
    [SerializeField] private float _maxLookAngle = 80f;
    private float _yaw, _pitch;

    [Header("FOV Settings")]
    [SerializeField] private float _baseFOV = 50f;
    [SerializeField] private float _sprintFOV = 60f;
    [SerializeField] private float _exhaustedFOV = 30f;
    [SerializeField] private float _fovStepTime = 10f;
    [HideInInspector] public float fovOverride = 0; // For FOV changer like Polaroid Camera right-click
    [HideInInspector] public float fovCamera = 0; // For FOV changer like Polaroid Camera right-click

    [Header("Head Bob")]
    [SerializeField] private bool _enableHeadBob = true;
    [SerializeField] private float _bobSpeed = 10f;
    [SerializeField] private Vector3 _bobAmount = new Vector3(.15f, 0.04f, 0f);
    private Vector3 _jointOriginalPos;
    private float _timer = 0;

    [Header("Idle Bob (Breathing)")]
    [SerializeField] private bool _enableIdleBob = true;
    [SerializeField] private float _idleBobSpeed = 2f;
    [SerializeField] private Vector3 _idleBobAmount = new Vector3(0.02f, 0.02f, 0f);

    private Vector2 _lookInput;

    public void OnLook(InputValue value)
    {
        _lookInput = value.Get<Vector2>();
    }

    private void Start()
    {
        _jointOriginalPos = _joint.localPosition;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleRotation();
        HandleFOV();
        if (_enableHeadBob) HandleHeadBob();
    }

    private void HandleRotation()
    {
        // Change 0.1f to a smaller number for more precise control
        float sensitivityMultiplier = 0.05f;

        float mouseX = _lookInput.x * _mouseSensitivity * sensitivityMultiplier;
        float mouseY = _lookInput.y * _mouseSensitivity * sensitivityMultiplier;

        _yaw = transform.root.localEulerAngles.y + mouseX;
        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, -_maxLookAngle, _maxLookAngle);

        transform.root.localEulerAngles = new Vector3(0, _yaw, 0);
        transform.localEulerAngles = new Vector3(_pitch, 0, 0);
    }

    private void HandleFOV()
    {
        float targetFOV;

        // Override FOV if set by any item
        if (fovOverride > 0)
        {
            targetFOV = fovOverride;
        }
        else if (fovCamera > 0)
        {
            targetFOV = fovCamera;
        }
        // Exhausted (From Movement Script)
        else if (_movement != null && _movement.isExhausted)
        {
            targetFOV = _exhaustedFOV;
        }
        // Sprinting (From Movement Script)
        else if (_movement != null && _movement.isSprinting)
        {
            targetFOV = _sprintFOV;
        }
        // Default: Normal View
        else
        {
            targetFOV = _baseFOV;
        }

        _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, targetFOV, _fovStepTime * Time.deltaTime);
    }

    private void HandleHeadBob()
    {
        if (_movement == null) return;

        float currentBobSpeed;
        Vector3 currentBobAmount;

        if (_movement.isWalking)
        {
            // Calculate walking/sprinting speeds
            float multiplier = _movement.isSprinting ? 1.5f : (_movement.isCrouched ? 0.5f : 1f);
            currentBobSpeed = _bobSpeed * multiplier;
            currentBobAmount = _bobAmount;
        }
        else
        {
            // Idle/Breathing state
            currentBobSpeed = _idleBobSpeed;
            currentBobAmount = _idleBobAmount;

            if (!_enableIdleBob)
            {
                _timer = 0;
                _joint.localPosition = Vector3.Lerp(_joint.localPosition, _jointOriginalPos, Time.deltaTime * _bobSpeed);
                return;
            }
        }
        // Keep the timer running constantly for a smooth transition
        _timer += Time.deltaTime * currentBobSpeed;
        float sinTimer = Mathf.Sin(_timer);
        // 1. Force xPos to 0 to stop the side-to-side nausea
        float xPos = 0f;
        // 2. Use a standard Sine wave for Y to get a smooth, natural up-down cycle
        float yPos = sinTimer * currentBobAmount.y;
        // Apply only to the Y axis
        _joint.localPosition = _jointOriginalPos + new Vector3(xPos, yPos, 0);
    }
}