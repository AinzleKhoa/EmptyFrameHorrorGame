using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class L3_PolaroidCamera : MonoBehaviour
{
    [Header("Item Name: L3_PolaroidCamera")]
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
    public float flashRange = 15f; // Đã tăng tầm xa để dễ trúng hơn
    public float flashAngle = 60f;
    public LayerMask monsterLayer;

    [Header("L3 Custom Settings")]
    public float stunDurationM3 = 20f;

    private bool _isOnCamera = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            _playerCam = playerObj.GetComponentInChildren<PlayerCamera>();
        }

        if (_mainCam == null) _mainCam = Camera.main;
        if (_mainCam == null) _mainCam = FindObjectOfType<Camera>();

        if (_cooldown != null)
        {
            _cooldown.SetBaseCooldown(_photoCooldown);
        }

        ResetLogic();
    }

    private void Awake()
    {
        _cooldown = GetComponent<ItemCooldown>();
    }

    private void OnDisable() => ResetLogic();
    private void OnEnable() => ResetLogic();

    private void ResetLogic()
    {
        _isOnCamera = false;
        if (_playerCam != null) _playerCam.fovOverride = 0;

        GameBroadcast.OnCameraToggle?.Invoke(false);
        StopAllCoroutines();

        if (_pointLight != null) _pointLight.enabled = false;
        if (_spotLight != null) _spotLight.enabled = false;
        if (_pointLightViewfinder != null) _pointLightViewfinder.enabled = false;
    }

    private void Update()
    {
        if (transform.parent == null || !transform.parent.CompareTag("HandSocket")) return;
        if (_isOnCamera && _hasSpectralLens) HandleSpectralLens();
    }

    private void HandleSpectralLens() { }

    public void OnPrimaryAction()
    {
        if (_cooldown != null)
        {
            if (_cooldown.IsReady)
            {
                _cooldown.Use();
                TakePhoto();
            }
            else if (_audioSource != null && _clickSound != null)
            {
                _audioSource.PlayOneShot(_clickSound);
            }
        }
        else TakePhoto();
    }

    public void OnSecondaryAction()
    {
        _isOnCamera = !_isOnCamera;
        if (_audioSource != null) _audioSource.PlayOneShot(_isOnCamera ? _viewfinderOnClip : _viewfinderOffClip);
        if (_pointLightViewfinder != null) _pointLightViewfinder.enabled = _isOnCamera;

        GameBroadcast.OnCameraToggle?.Invoke(_isOnCamera);
        if (_cameraMesh != null) _cameraMesh.enabled = !_isOnCamera;
        if (_playerCam != null) _playerCam.fovOverride = _isOnCamera ? _zoomFOV : 0;

        if (_mainCam != null)
        {
            int spectralLayer = LayerMask.NameToLayer("Spectral");
            if (_isOnCamera) _mainCam.cullingMask |= 1 << spectralLayer;
            else _mainCam.cullingMask &= ~(1 << spectralLayer);
        }
    }

    private void TakePhoto()
    {
        if (_mainCam == null) _mainCam = Camera.main;
        if (_mainCam == null) _mainCam = FindObjectOfType<Camera>();
        if (_mainCam == null) return;

        Debug.Log("<color=cyan>Chụp ảnh!</color>");
        if (_audioSource != null && _photoShutter != null) _audioSource.PlayOneShot(_photoShutter);
        GameBroadcast.OnItemStatusUpdate?.Invoke("PolaroidCamera", 0f);

        Collider[] hitMonsters = Physics.OverlapSphere(transform.position, flashRange, monsterLayer);

        foreach (var monster in hitMonsters)
        {
            Vector3 screenPoint = _mainCam.WorldToViewportPoint(monster.bounds.center);
            bool inFrame = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

            if (inFrame)
            {
                // GỌI TRỰC TIẾP QUÁI THE OVEREXPOSED CỦA MÀN 4
                TheOverexposed aiMonster = monster.GetComponent<TheOverexposed>();
                if (aiMonster == null) aiMonster = monster.GetComponentInParent<TheOverexposed>();

                if (aiMonster != null)
                {
                    // Đổi trạng thái sang Stunned
                    aiMonster.currentState = TheOverexposed.MonsterState.Stunned;

                    // Gửi tin nhắn ép nó chạy hàm choáng (nếu có sẵn trong code của bạn)
                    aiMonster.SendMessage("TakeDamage", stunDurationM3, SendMessageOptions.DontRequireReceiver);
                    aiMonster.SendMessage("Stun", stunDurationM3, SendMessageOptions.DontRequireReceiver);

                    Debug.Log($"<color=green>Đã chụp trúng TheOverexposed!</color>");
                }
            }
        }

        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        if (_spotLight != null)
        {
            _spotLight.enabled = true;
            _spotLight.intensity = _spotIntensity;
            _spotLight.range = _spotRange;
            _spotLight.color = _flashColor;
        }
        yield return new WaitForSeconds(_flashDuration);
        if (_spotLight != null) _spotLight.enabled = false;
    }
}