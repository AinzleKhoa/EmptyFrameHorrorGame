using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PolaroidCamera : MonoBehaviour
{
    [Header("Item Name: PolaroidCamera")]

    [Space(15)]

    [Header("Polaroid Camera Settings")]
    [SerializeField] private float _cameraShotCooldown = 30f;
    [SerializeField] private bool _isCameraStateEnabled = false; // This is the "Viewfinder On" state

    [Header("Point Light (Glow)")]
    [SerializeField] private Light _pointLight;
    [SerializeField] private float _pointIntensity = 5f;
    [SerializeField] private float _pointRange = 3f;

    [Header("Spot Light (Forward Flash)")]
    [SerializeField] private Light _spotLight;
    [SerializeField] private float _spotIntensity = 40f;
    [SerializeField] private float _spotRange = 20f;

    [Header("General Flash Settings")]
    [SerializeField] private Color _flashColor = new Color(0.9f, 0.95f, 1f);
    [SerializeField] private float _flashDuration = 0.05f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private AudioClip _photoShutter;
    [SerializeField] private AudioClip _viewfinderOnClip;
    [SerializeField] private AudioClip _viewfinderOffClip;
    private float _lastPhotoTime;

    [Header("Camera Rules Settings")]
    [SerializeField] private MeshRenderer _cameraMesh;
    [Header("Zoom Settings")]
    [SerializeField] private float _zoomFOV = 40f;

    [Header("References")]
    [SerializeField] private Camera _mainCam; // Drag your Main Camera here
    [SerializeField] private PlayerCamera _playerCam;

    [Header("Flash Settings")]
    public float flashRange = 8f;
    public float flashAngle = 60f; // The width of the camera's view
    public LayerMask monsterLayer; // Set this to the layer your monster is on
    private bool _isOnCamera = false;

    private void Start()
    {
        // Mark the last shot as "negative thirty seconds ago" 
        // This makes (Time.time - _lastPhotoTime) >= _cooldown true immediately.
        _lastPhotoTime = -_cameraShotCooldown;

        ResetLogic();
    }
    private void OnDisable() => ResetLogic();

    private void ResetLogic()
    {
        // Reset everything if the camera is put away or dropped
        _isOnCamera = false;
        if (_playerCam != null) _playerCam.fovOverride = 0;
        // Broadcast that camera is away
        GameBroadcast.OnCameraToggle?.Invoke(false);
    }

    private void Update()
    {
        // Only run if the camera is held
        if (transform.parent == null || !transform.parent.CompareTag("HandSocket")) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Time.time >= _lastPhotoTime + _cameraShotCooldown)
            {
                TakePhoto();
            }
            else
            {
                Debug.Log("Camera is still recharging...");
                if (_audioSource != null && _clickSound != null)
                {
                    _audioSource.PlayOneShot(_clickSound);
                }
            }
        }

        if (_isCameraStateEnabled)
            HandleViewfinder();
    }

    // Interact with PlayerItemStatusHUD by sending cooldown progress
    private void LateUpdate()
    {
        // Calculate the percentage of the cooldown (0 to 1)
        float timePassed = Time.time - _lastPhotoTime;
        float progress = Mathf.Clamp01(timePassed / _cameraShotCooldown);

        // Update the HUD so the player sees the bar filling up
        GameBroadcast.OnItemStatusUpdate?.Invoke("PolaroidCamera", progress);
    }

    private void HandleViewfinder()
    {
        // Both Enable & Disable in one method
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            _isOnCamera = !_isOnCamera; // Flip the state (True -> False / False -> True)

            // Play the appropriate sound for the new state
            PlayViewfinderSound(_isOnCamera ? _viewfinderOnClip : _viewfinderOffClip);

            // --- 1. HUD Toggle ---
            // This tells the HUD, Movement, and Interactor what to do!
            GameBroadcast.OnCameraToggle?.Invoke(_isOnCamera);

            // --- 2. Hide/Show Camera Model ---
            if (_cameraMesh != null) _cameraMesh.enabled = !_isOnCamera;

            // --- 3. Camera FOV ---
            if (_playerCam != null)
            {
                _playerCam.fovOverride = _isOnCamera ? _zoomFOV : 0;
            }

            // --- 4. Spectral Vision ---
            if (_mainCam != null)
            {
                int spectralLayer = LayerMask.NameToLayer("Spectral");
                if (_isOnCamera)
                    _mainCam.cullingMask |= 1 << spectralLayer;
                else
                    _mainCam.cullingMask &= ~(1 << spectralLayer);
            }
        }
    }

    private void PlayViewfinderSound(AudioClip clip)
    {
        if (_audioSource != null && clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    private void TakePhoto()
    {
        _lastPhotoTime = Time.time; // Mark the time of the shot immediately
        Debug.Log("Photo Captured!");

        // Audio Feedback
        if (_audioSource != null && _photoShutter != null)
        {
            _audioSource.PlayOneShot(_photoShutter);
        }

        // We send 0 to represent the start of the cooldown (fully empty)
        GameBroadcast.OnItemStatusUpdate?.Invoke("PolaroidCamera", 0f);

        // Efficient Detection: Only look at objects on the Monster Layer
        Collider[] hitMonsters = Physics.OverlapSphere(transform.position, flashRange, monsterLayer);

        foreach (var monster in hitMonsters)
        {
            // Viewport Check: Is the monster actually appearing inside the camera frame?
            // screenPoint.z > 0 ensures it's in front of the camera, not behind.
            Vector3 screenPoint = _mainCam.WorldToViewportPoint(monster.bounds.center);
            bool inFrame = screenPoint.z > 0 &&
                           screenPoint.x > 0.1f && screenPoint.x < 0.9f &&
                           screenPoint.y > 0.1f && screenPoint.y < 0.9f;

            if (inFrame)
            {
                // Occlusion Check: Ensure no walls/obstacles are blocking the light
                Vector3 dir = (monster.bounds.center - transform.position).normalized;
                RaycastHit hit;

                // We hit everything (Default + Monster) to see if a wall is in between
                if (Physics.Raycast(transform.position, dir, out hit, flashRange))
                {
                    // If the first thing we hit is the monster, it's a success
                    if (hit.collider == monster || hit.transform.CompareTag("Monster_OE"))
                    {
                        if (monster.TryGetComponent<TheOverexposed>(out var ai))
                        {
                            ai.TakeDamage(); // Trigger the Stun
                            Debug.Log("<color=green>Flash Impact Success on: </color>" + monster.name);
                        }
                    }
                }
            }
        }

        // Visual Flash Effect
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // 1. Initial Burst
        if (_pointLight != null)
        {
            _pointLight.enabled = true;
            _pointLight.intensity = _pointIntensity;
            _pointLight.range = _pointRange;
            _pointLight.color = _flashColor;
        }

        if (_spotLight != null)
        {
            _spotLight.enabled = true;
            _spotLight.intensity = _spotIntensity;
            _spotLight.range = _spotRange;
            _spotLight.color = _flashColor;
        }

        yield return new WaitForSeconds(_flashDuration);

        // 2. Smooth Fade Out
        float elapsed = 0;
        float fadeDuration = 0.2f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            if (_pointLight != null) _pointLight.intensity = Mathf.Lerp(_pointIntensity, 0, t);
            if (_spotLight != null) _spotLight.intensity = Mathf.Lerp(_spotIntensity, 0, t);

            yield return null;
        }

        // 3. Final Cleanup
        if (_pointLight != null) _pointLight.enabled = false;
        if (_spotLight != null) _spotLight.enabled = false;
    }
}