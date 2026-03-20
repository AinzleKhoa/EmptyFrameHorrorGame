using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PolaroidCamera : MonoBehaviour
{
    [Header("Item Name: PolaroidCamera")]

    [Space(15)]

    [Header("Point Light Viewfinder")]
    [SerializeField] private Light _pointLightViewfinder;
    [SerializeField] private float _pointIntensityViewfinder = 5f;
    [SerializeField] private float _pointRangeViewfinder = 5f;

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

    [Header("Camera Rules Settings")]
    [SerializeField] private float _photoCooldown = 2.0f;
    [SerializeField] private MeshRenderer _cameraMesh;
    [Header("Zoom Settings")]
    [SerializeField] private float _zoomFOV = 40f;
    [Space(15)]


    [Header("UPGRADE")]

    [Header("Lens Upgrade Settings")]
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private float _lensScanRange = 20f;
    private bool _hasSpectralLens = false;


    private ItemCooldown _cooldown;
    private Camera _mainCam;
    private PlayerCamera _playerCam;

    [Header("Flash Settings")]
    public float flashRange = 8f;
    public float flashAngle = 60f; // The width of the camera's view
    public LayerMask monsterLayer; // Set this to the layer your monster is on
    private bool _isOnCamera = false;

    private void Start()
    {
        // Automatic Player Search
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            _playerCam = playerObj.GetComponentInChildren<PlayerCamera>();
        }

        if (_mainCam == null) _mainCam = Camera.main;

        ResetLogic();
    }

    private void OnDisable() => ResetLogic();
    private void OnEnable() => ResetLogic();

    private void ResetLogic()
    {
        // Reset everything if the camera is put away or dropped
        _isOnCamera = false;
        if (_playerCam != null) _playerCam.fovOverride = 0;
        // Broadcast that camera is away
        GameBroadcast.OnCameraToggle?.Invoke(false);
        StopAllCoroutines();
        // TURN OFF ALL LIGHTS
        if (_pointLight != null) _pointLight.enabled = false;
        if (_spotLight != null) _spotLight.enabled = false;
        if (_pointLightViewfinder != null) _pointLightViewfinder.enabled = false;
    }

    private void Awake()
    {
        _cooldown = GetComponent<ItemCooldown>();

        if (_cooldown != null)
        {
            _cooldown.setBaseCooldown(_photoCooldown);
        }

        if (_pointLight != null)
        {
            _pointLight.enabled = false;
            _pointLight.intensity = _pointIntensity;
            _pointLight.range = _pointRange;
            _pointLight.color = _flashColor;
        }

        if (_pointLightViewfinder != null)
        {
            _pointLightViewfinder.enabled = false;
            _pointLightViewfinder.intensity = _pointIntensityViewfinder;
            _pointLightViewfinder.range = _pointRangeViewfinder;
            _pointLightViewfinder.color = _flashColor;
        }

        if (_spotLight != null)
        {
            _spotLight.enabled = false;
            _spotLight.intensity = _spotIntensity;
            _spotLight.range = _spotRange;
            _spotLight.color = _flashColor;
        }
    }

    private void Update()
    {
        if (transform.parent == null || !transform.parent.CompareTag("HandSocket")) return;

        if (_isOnCamera && _hasSpectralLens)
        {
            HandleSpectralLens();
        }
    }

    public void OnPrimaryAction()
    {
        // Check if the component exists on this object
        ItemCooldown cooldown = GetComponent<ItemCooldown>();

        if (cooldown != null)
        {
            // Follow Cooldown Rules
            if (cooldown.IsReady)
            {
                cooldown.Use();
                TakePhoto();
            }
            else
            {
                // Play 'Click' sound because it's still charging
                if (_audioSource != null && _clickSound != null)
                    _audioSource.PlayOneShot(_clickSound);
            }
        }
        else
        {
            // No Cooldown Component found? Shoot freely!
            TakePhoto();
        }
    }

    public void OnSecondaryAction()
    {
        // Toggle the viewfinder/zoom
        HandleViewfinder();
    }

    private void HandleViewfinder()
    {
        // Both Enable & Disable in one method
        _isOnCamera = !_isOnCamera; // Flip the state (True -> False / False -> True)

        // Play the appropriate sound for the new state
        PlayViewfinderSound(_isOnCamera ? _viewfinderOnClip : _viewfinderOffClip);

        if (_pointLightViewfinder != null)
        {
            _pointLightViewfinder.enabled = _isOnCamera ? true : false;
        }

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

    private void PlayViewfinderSound(AudioClip clip)
    {
        if (_audioSource != null && clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    private void TakePhoto()
    {
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
                    if (hit.collider == monster || hit.transform.CompareTag("Monster"))
                    {
                        Debug.Log($"<color=green>SUCCESS:</color> Captured {monster.name} in frame.");
                        // Check for TheOverexposed
                        if (monster.TryGetComponent<TheOverexposed>(out var oe))
                        {
                            oe.TakeDamage();
                        }
                        // Check for Shadow
                        if (monster.TryGetComponent<L5_ShadowMonster>(out var shadow))
                        {
                            shadow.TakeFlash();
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
        // RESET intensities before enabling
        if (_pointLight != null) _pointLight.intensity = _pointIntensity;
        if (_spotLight != null) _spotLight.intensity = _spotIntensity;

        // Now enable them
        if (_pointLight != null) _pointLight.enabled = true;
        if (_spotLight != null) _spotLight.enabled = true;

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

    public void UnlockSpectralLens(bool status) => _hasSpectralLens = status;

    private void HandleSpectralLens()
    {
        if (!_hasSpectralLens || !_isOnCamera) return;

        // INCREASE THIS RANGE to 200 to match your request
        Collider[] items = Physics.OverlapSphere(transform.position, 200f, _interactableLayer);

        foreach (var item in items)
        {
            Vector3 screenPoint = _mainCam.WorldToViewportPoint(item.bounds.center);
            bool inFrame = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

            if (inFrame)
            {
                if (item.TryGetComponent<OutlineManager>(out var manager))
                {
                    manager.SignalLensPresence();
                }
            }
        }
    }
}