using UnityEngine;

public class L1H_SnappableItem : MonoBehaviour, IInteractable
{
    [Header("ID must match SnapSpot / PlaceSpot")]
    [SerializeField] private string itemId = "blanket";

    [Header("Prompts")]
    [SerializeField] private string pickPrompt = "Press 'E' to Pick Up";
    [SerializeField] private string placePrompt = "Press 'E' to Place";

    [Header("Hold behavior")]
    [SerializeField] private Transform holdAnchor;
    [SerializeField] private Vector3 holdLocalPos = new Vector3(0.3f, -0.2f, 0.7f);
    [SerializeField] private Vector3 holdLocalEuler = new Vector3(0f, 0f, 0f);

    [Header("Snap settings (fallback if you still place by looking at item)")]
    [SerializeField] private float snapDistance = 1.2f;

    [Tooltip("Disable physics when holding/snapped")]
    [SerializeField] private bool disablePhysics = true;

    [Header("State")]
    [SerializeField] private bool _canInteractAtStart = true;

    private bool _isHeld;
    private bool _isSnapped;
    private bool _canInteract;

    private Rigidbody _rb;
    private Collider _col;
    private Transform _originalParent;
    private Collider[] _playerColliders;
    private SignalTrigger _signalTrigger;

    public string ItemId => itemId;
    public bool IsSnapped => _isSnapped;

    public string PromptMessage
    {
        get
        {
            if (!_canInteract) return "";
            if (_isSnapped) return "";
            if (!L1H_InteractRules.CanInteractWithThisSnappable(this)) return "";

            return _isHeld ? placePrompt : pickPrompt;
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
        _originalParent = transform.parent;
        _signalTrigger = GetComponent<SignalTrigger>();
        _canInteract = _canInteractAtStart;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!_canInteract) return;
        if (_isSnapped) return;
        if (!L1H_InteractRules.CanInteractWithThisSnappable(this)) return;


        // Nếu chưa cầm món này, nhưng đang cầm món khác rồi -> không cho nhặt
        if (!_isHeld && L1H_HeldRegistry.CurrentHeld != null && L1H_HeldRegistry.CurrentHeld != this)
            return;

        if (!_isHeld)
        {
            Transform anchor = ResolveHoldAnchor(interactor);
            if (anchor == null) return;

            _isHeld = true;
            L1H_HeldRegistry.SetHeld(this);

            transform.SetParent(anchor);
            transform.localPosition = holdLocalPos;
            transform.localRotation = Quaternion.Euler(holdLocalEuler);

            SetPhysics(false);

            CachePlayerColliders(interactor);
            IgnorePlayerCollisions(true);
        }
        else
        {
            L1H_SnapSpot spot = FindClosestSpot();
            if (spot != null && spot.SpotId == itemId)
            {
                PlaceOn(spot.transform);
            }
            else
            {
                DropToWorld();
            }
        }
    }

    public void PlaceOn(Transform spot)
    {
        if (!_canInteract) return;
        if (_isSnapped) return;

        IgnorePlayerCollisions(false);

        transform.SetParent(spot);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        SetPhysics(false);

        _isHeld = false;
        _isSnapped = true;

        var spotCol = spot.GetComponent<Collider>();
        if (spotCol != null)
            spotCol.enabled = false;

        if (_rb != null)
        {
            if (itemId == "chair")
            {
                transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            }

            _rb.isKinematic = false;
            _rb.useGravity = true;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.constraints = RigidbodyConstraints.FreezeRotationX
                            | RigidbodyConstraints.FreezeRotationZ;
        }

        if (_col != null)
            _col.enabled = true;

        L1H_HeldRegistry.ClearHeld(this);

        if (_signalTrigger != null)
            _signalTrigger.RaiseSignal();
        else
            Debug.LogWarning($"[L1 Snap] {name} has no SignalTrigger attached.");
    }

    public void EnableInteract()
    {
        _canInteract = true;
    }

    public void DisableInteract()
    {
        _canInteract = false;
    }

    private Transform ResolveHoldAnchor(PlayerInteractor interactor)
    {
        if (holdAnchor != null) return holdAnchor;

        Camera cam = interactor.GetComponentInChildren<Camera>();
        if (cam != null) return cam.transform;

        return interactor.transform;
    }

    private L1H_SnapSpot FindClosestSpot()
    {
        L1H_SnapSpot[] spots = FindObjectsByType<L1H_SnapSpot>(FindObjectsSortMode.None);
        if (spots == null || spots.Length == 0) return null;

        float best = float.MaxValue;
        L1H_SnapSpot bestSpot = null;

        Vector3 p = transform.position;

        for (int i = 0; i < spots.Length; i++)
        {
            float d = Vector3.Distance(p, spots[i].transform.position);
            if (d < best)
            {
                best = d;
                bestSpot = spots[i];
            }
        }

        if (bestSpot != null && best <= snapDistance) return bestSpot;
        return null;
    }

    private void DropToWorld()
    {
        _isHeld = false;

        IgnorePlayerCollisions(false);
        L1H_HeldRegistry.ClearHeld(this);

        transform.SetParent(_originalParent);
        SetPhysics(true);

        if (_rb != null)
        {
            _rb.AddForce(transform.forward * 1.5f + Vector3.up * 0.5f, ForceMode.Impulse);
        }
    }

    private void SetPhysics(bool enabled)
    {
        if (!disablePhysics) return;
        if (_rb != null) _rb.isKinematic = !enabled;
    }

    private void CachePlayerColliders(PlayerInteractor interactor)
    {
        if (interactor == null) return;

        if (_playerColliders == null || _playerColliders.Length == 0)
        {
            _playerColliders = interactor.transform.root.GetComponentsInChildren<Collider>();
        }
    }

    private void IgnorePlayerCollisions(bool ignore)
    {
        if (_col == null) return;
        if (_playerColliders == null) return;

        for (int i = 0; i < _playerColliders.Length; i++)
        {
            var pc = _playerColliders[i];
            if (pc == null) continue;
            if (pc == _col) continue;

            Physics.IgnoreCollision(_col, pc, ignore);
        }
    }
}