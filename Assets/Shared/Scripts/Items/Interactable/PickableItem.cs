using UnityEngine;

public class PickableItem : MonoBehaviour, IInteractable
{
    [Header("Item Data")]
    public string ItemName = "New Item";
    public Sprite ItemIcon;

    [Header("UI Message")]
    [SerializeField] private string _promptMessage = "Press 'E' to Pick Up";
    public string PromptMessage => _promptMessage;

    [Header("Hand Transform Settings")]
    [SerializeField] private Vector3 _heldPositionOffset;
    [SerializeField] private Vector3 _heldRotationOffset;

    [Tooltip("Scale Multiplier: 1 = same as floor, 2 = double size, 0.5 = half size")]
    [SerializeField] private float _handScaleMultiplier = 1.0f; // Simplified to a single float

    private Rigidbody _rb;
    private Collider _col;
    private Vector3 _originalScale;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();

        // Save the tiny scale it has when it's sitting in the world
        _originalScale = transform.localScale;
    }

    public void SetPhysics(bool isEnabled)
    {
        if (_rb) _rb.isKinematic = !isEnabled;
        if (_col) _col.enabled = isEnabled;

        if (isEnabled)
        {
            // Reset to that tiny original scale when dropped
            transform.localScale = _originalScale;
            _rb.AddForce(transform.forward * 2f + Vector3.up * 1f, ForceMode.Impulse);
        }
    }

    public void Interact(PlayerInteractor interactor)
    {
        PlayerInventory inventory = interactor.GetComponentInParent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddItem(this);
        }
    }

    public void ApplyHandTransform()
    {
        transform.localPosition = _heldPositionOffset;
        transform.localRotation = Quaternion.Euler(_heldRotationOffset);

        // CALCULATED SCALE: Original size (e.g. 0.01) * Multiplier (e.g. 2.0)
        transform.localScale = _originalScale * _handScaleMultiplier;
    }
}