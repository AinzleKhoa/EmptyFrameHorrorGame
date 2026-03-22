using UnityEngine;

public class HospitalLockableDoor : MonoBehaviour
{
    [SerializeField] private DoorInteract _doorInteract;
    [SerializeField] private Collider _extraBlocker;
    [SerializeField] private bool _lockedOnStart = true;

    private bool _isLocked;

    private void Awake()
    {
        if (_doorInteract == null) _doorInteract = GetComponent<DoorInteract>();
        SetLocked(_lockedOnStart);
    }

    public void LockDoor() => SetLocked(true);
    public void UnlockDoor() => SetLocked(false);

    public void SetLocked(bool value)
    {
        _isLocked = value;

        if (_doorInteract != null)
            _doorInteract.enabled = !value;

        if (_extraBlocker != null)
            _extraBlocker.enabled = value;
    }

    public bool IsLocked() => _isLocked;
}
