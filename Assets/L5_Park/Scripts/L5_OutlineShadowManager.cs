using UnityEngine;

[RequireComponent(typeof(Outline))]
public class L5_OutlineShadowManager : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float _detectionRange = 10f;
    [SerializeField] private float _defaultWidth = 5f;

    private Transform _player;
    private Outline _outline;

    public static bool IsPassiveEnabled = false;
    private bool _isPlayerOnCamera = false;
    private bool _isInitialized = false;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _player = GameObject.FindWithTag("Player")?.transform;

        // FORCE OFF immediately
        if (_outline) _outline.enabled = false;
    }

    private void OnEnable()
    {
        GameBroadcast.OnCameraToggle += SetCameraState;

        // Ensure it starts OFF every time it manifests
        _isInitialized = false;
        if (_outline) _outline.enabled = false;

        // Wait for the "Teleport" to finish
        Invoke(nameof(EnableLogic), 0.15f); // Increased slightly for safety
    }

    private void OnDisable()
    {
        GameBroadcast.OnCameraToggle -= SetCameraState;
        CancelInvoke();
        if (_outline) _outline.enabled = false;
    }

    private void EnableLogic()
    {
        if (_outline) _outline.enabled = true;
        _isInitialized = true;
    }

    private void SetCameraState(bool isOn) => _isPlayerOnCamera = isOn;

    private void LateUpdate()
    {
        // If not initialized or component is missing, do nothing
        if (!_isInitialized || !_outline || !_player) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        bool inRange = dist <= _detectionRange;

        bool reveal = inRange && (IsPassiveEnabled || _isPlayerOnCamera);

        // Instead of setting width to 0, just toggle the component
        // This is much "safer" and prevents rendering bugs
        _outline.enabled = reveal;

        if (reveal) _outline.OutlineWidth = _defaultWidth;
    }
}