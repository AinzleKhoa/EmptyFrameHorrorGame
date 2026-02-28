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

    private bool _isHeld;
    private bool _isSnapped;

    private Rigidbody _rb;
    private Collider _col;
    private Transform _originalParent;

    // cache all colliders in player so we can ignore collision while held
    private Collider[] _playerColliders;

    // ✅ expose for spot script
    public string ItemId => itemId;
    public bool IsSnapped => _isSnapped;

    public string PromptMessage
    {
        get
        {
            if (_isSnapped) return "";
            return _isHeld ? placePrompt : pickPrompt;
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
        _originalParent = transform.parent;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (_isSnapped) return;

        if (!_isHeld)
        {
            // PICK UP
            Transform anchor = ResolveHoldAnchor(interactor);
            if (anchor == null) return;

            _isHeld = true;
            L1H_HeldRegistry.SetHeld(this); // ✅ NOW HELD

            transform.SetParent(anchor);
            transform.localPosition = holdLocalPos;
            transform.localRotation = Quaternion.Euler(holdLocalEuler);

            // make it kinematic so it doesn't fight physics
            SetPhysics(false);

            // keep collider ON for raycast, but ignore collision with player
            CachePlayerColliders(interactor);
            IgnorePlayerCollisions(true);
        }
        else
        {
            // (Optional fallback) PLACE by looking at item itself
            // Bạn có thể giữ hoặc xoá đoạn này. Giữ lại thì tiện test.
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

    // ✅ Spot gọi hàm này để đặt
    public void PlaceOn(Transform spot)
    {
        if (_isSnapped) return;

        // restore collision with player
        IgnorePlayerCollisions(false);

        transform.SetParent(spot);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        SetPhysics(false);

        _isHeld = false;
        _isSnapped = true;

        // 🔥 1) TẮT collider của spot
        var spotCol = spot.GetComponent<Collider>();
        if (spotCol != null)
            spotCol.enabled = false;

        // 🔥 2) BẬT lại physics cho gối (tắt kinematic)
        if (_rb != null)
        {
            // 🎯 Chỉ áp dụng cho ghế
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

        // Giữ collider của gối bật để nó nằm lên giường
        if (_col != null)
            _col.enabled = true;

        L1H_HeldRegistry.ClearHeld(this);
        L1H_CleanupObjectiveManager.ReportOneCleaned();
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
        L1H_HeldRegistry.ClearHeld(this); // ✅ no longer held

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

        // DO NOT disable collider here (need it for raycast while held)
        // Collider is disabled only when placed (PlaceOn)
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