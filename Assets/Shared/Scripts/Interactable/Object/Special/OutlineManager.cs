using UnityEngine;

[RequireComponent(typeof(Outline))]
public class OutlineManager : MonoBehaviour
{
    [SerializeField] private float _detectionRange = 8f;
    private Transform _player;
    private Outline _outline;
    private bool _isSeen;

    // NEW: Master control flag
    private bool _canShowOutline = true;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _player = GameObject.FindWithTag("Player")?.transform;
        if (_outline) _outline.enabled = false;
    }

    // Public method to lock/unlock the  functionality
    public void SetOutlineActive(bool isActive)
    {
        _canShowOutline = isActive;

        // If we are disabling it, force the outline off immediately
        if (!_canShowOutline && _outline != null)
        {
            _outline.enabled = false;
        }
    }

    public void SignalLensPresence() => _isSeen = true;

    private void LateUpdate()
    {
        if (!_outline || !_player || !_canShowOutline) return;

        // Logic only runs if _canShowOutline is true
        float dist = Vector3.Distance(transform.position, _player.position);
        _outline.enabled = (dist <= _detectionRange) || _isSeen;

        _isSeen = false; // Reset for next frame
    }
}