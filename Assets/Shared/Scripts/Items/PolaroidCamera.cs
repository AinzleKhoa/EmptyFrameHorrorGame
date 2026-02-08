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
    [SerializeField] private PlayerMovement _movement;

    [Header("HUD Management")]
    [SerializeField] private GameObject _normalStateGroup;
    [SerializeField] private GameObject _cameraStateGroup;

    [Header("Zoom Settings")]
    [SerializeField] private PlayerCamera _playerCam;
    [SerializeField] private float _zoomFOV = 40f;

    [Header("Spectral Vision")]
    [SerializeField] private Camera _mainCam; // Drag your Main Camera here

    private bool _isAiming = false;

    private void Start()
    {
        _isAiming = false;
        if (_playerCam != null) _playerCam.fovOverride = 0;
        if (_movement != null) _movement.isOnCamera = false;
        if (_normalStateGroup != null) _normalStateGroup.SetActive(true);
        if (_cameraStateGroup != null) _cameraStateGroup.SetActive(false);
    }

    private void OnDisable()
    {
        // Reset everything if the camera is put away or dropped
        _isAiming = false;
        if (_playerCam != null) _playerCam.fovOverride = 0;
        if (_movement != null) _movement.isOnCamera = false;
        if (_normalStateGroup != null) _normalStateGroup.SetActive(true);
        if (_cameraStateGroup != null) _cameraStateGroup.SetActive(false);
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
        // Check if the button was clicked (not held)
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            _isAiming = !_isAiming; // Flip the state (True -> False / False -> True)

            // Play the appropriate sound for the new state
            PlayViewfinderSound(_isAiming ? _viewfinderOnClip : _viewfinderOffClip);

            // --- 1. HUD Toggle ---
            if (_normalStateGroup != null) _normalStateGroup.SetActive(!_isAiming);
            if (_cameraStateGroup != null) _cameraStateGroup.SetActive(_isAiming);

            // --- 2. Hide/Show Camera Model ---
            if (_cameraMesh != null) _cameraMesh.enabled = !_isAiming;

            // --- 3. Modify Movement & FOV ---
            if (_movement != null) _movement.isOnCamera = _isAiming;

            if (_playerCam != null)
            {
                _playerCam.fovOverride = _isAiming ? _zoomFOV : 0;
            }

            // --- 4. Spectral Vision ---
            if (_mainCam != null)
            {
                int spectralLayer = LayerMask.NameToLayer("Spectral");
                if (_isAiming)
                    _mainCam.cullingMask |= (1 << spectralLayer);
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