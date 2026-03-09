using UnityEngine;

[RequireComponent(typeof(Outline))]
public class OutlineManager : MonoBehaviour
{
    [SerializeField] private float _detectionRange = 8f;
    private Transform _player;
    private Outline _outline;
    private bool _isSeen;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _player = GameObject.FindWithTag("Player")?.transform;
        if (_outline) _outline.enabled = false;
    }

    public void SignalLensPresence() => _isSeen = true;

    private void LateUpdate()
    {
        if (!_outline || !_player) return;

        // One simple line: Distance check OR Camera signal
        float dist = Vector3.Distance(transform.position, _player.position);
        _outline.enabled = (dist <= _detectionRange) || _isSeen;

        _isSeen = false; // Reset for next frame
    }
}