using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PolaroidCamera : MonoBehaviour
{
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
    [SerializeField] private AudioClip _photoShutter;
    [SerializeField] private AudioClip _viewfinderOnClip;
    [SerializeField] private AudioClip _viewfinderOffClip;

    [Header("Capture Settings")]
    [SerializeField] private float _cooldown = 2f;
    private float _lastPhotoTime;

    [Header("Camera Rules Settings")]
    [SerializeField] private MeshRenderer _cameraMesh;
    [Header("Zoom Settings")]
    [SerializeField] private float _zoomFOV = 40f;

    [Header("Spectral Vision")]

    [Header("References")]
    [SerializeField] private Camera _mainCam; // Drag your Main Camera here
    [SerializeField] private PlayerCamera _playerCam;
    private bool _isOnCamera = false;

    private void Start()
    {
        _isOnCamera = false;
        if (_playerCam != null) _playerCam.fovOverride = 0;
        // Initial broadcast to make sure everything starts "Normal"
        GameEvents.OnCameraToggle?.Invoke(false);
    }

    private void OnDisable()
    {
        // Reset everything if the camera is put away or dropped
        _isOnCamera = false;
        if (_playerCam != null) _playerCam.fovOverride = 0;
        // Broadcast that camera is away
        GameEvents.OnCameraToggle?.Invoke(false);
    }

    private void Update()
    {
        // Only run if the camera is held
        if (transform.parent == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= _lastPhotoTime + _cooldown)
        {
            TakePhoto();
        }

        HandleViewfinder(); // Hold not Toggle
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
            GameEvents.OnCameraToggle?.Invoke(_isOnCamera);

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
        _lastPhotoTime = Time.time;
        Debug.Log("Photo Captured!");

        if (_audioSource != null && _photoShutter != null)
        {
            _audioSource.PlayOneShot(_photoShutter);
        }

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