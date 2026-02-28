using System.Collections;
using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [Header("Prompt")]
    [SerializeField] private string _openPrompt = "Press 'E' to Open Door";
    [SerializeField] private string _closePrompt = "Press 'E' to Close Door";
    [SerializeField] private string _lockedPrompt = "The door is locked";

    [Header("Door Settings")]
    [SerializeField] private bool _openInward = true;
    [SerializeField] private bool _isLocked = false;
    [SerializeField] private float _openAngle = 90f;
    [SerializeField] private float _openDuration = 2.2f; // seconds

    [Tooltip("Local axis to rotate around (usually Y for a hinge door)")]
    [SerializeField] private Vector3 _localAxis = new Vector3(0, 1, 0);

    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _openSfx;
    [SerializeField] private AudioClip _closeSfx;
    [SerializeField] private AudioClip _lockedSfx;

    private bool _isOpen = false;
    private bool _isMoving = false;

    private Quaternion _closedRot;
    private Quaternion _openRot;

    public string PromptMessage
    {
        get
        {
            if (_isLocked) return _lockedPrompt;
            return _isOpen ? _closePrompt : _openPrompt;
        }
    }

    private void Awake()
    {
        _closedRot = transform.localRotation;
        float angle = _openInward ? -_openAngle : _openAngle;
        _openRot = _closedRot * Quaternion.AngleAxis(angle, _localAxis.normalized);
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (_isMoving) return;

        if (_isLocked)
        {
            if (_audioSource && _lockedSfx) _audioSource.PlayOneShot(_lockedSfx);
            return;
        }

        StartCoroutine(RotateDoor(!_isOpen));
    }

    private IEnumerator RotateDoor(bool open)
    {
        _isMoving = true;

        Quaternion start = transform.localRotation;
        Quaternion target = open ? _openRot : _closedRot;

        if (_audioSource)
        {
            var clip = open ? _openSfx : _closeSfx;
            if (clip) _audioSource.PlayOneShot(clip);
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, _openDuration);
            transform.localRotation = Quaternion.Slerp(start, target, t);
            yield return null;
        }

        transform.localRotation = target;
        _isOpen = open;
        _isMoving = false;
    }

    public void SetLocked(bool locked) => _isLocked = locked;
}