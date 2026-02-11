using UnityEngine;

public class PickableItem : MonoBehaviour, IInteractable
{
    [Header("Item Data")]
    public string ItemName = "New Item";
    public Sprite ItemIcon;

    [Header("UI Message")]
    [SerializeField] private string _promptMessage = "Press 'E' to Pick Up";
    public string PromptMessage => _promptMessage;

    [Header("Holding Settings")]
    [SerializeField] private Vector3 _heldRotationOffset; // The custom rotation for the hand

    private Rigidbody _rb;
    private Collider _col;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
    }

    public void SetPhysics(bool isEnabled)
    {
        if (_rb) _rb.isKinematic = !isEnabled;
        if (_col) _col.enabled = isEnabled;

        if (isEnabled) // Add a little toss when dropped
        {
            _rb.AddForce(transform.forward * 2f + Vector3.up * 1f, ForceMode.Impulse);
        }
    }

    public void Interact(PlayerInteractor interactor)
    {
        // 1. Get the Inventory component from the player
        PlayerInventory inventory = interactor.GetComponentInParent<PlayerInventory>();

        if (inventory != null)
        {
            // 2. Try to add the item to the inventory
            inventory.AddItem(this);
        }
    }

    // Helper for the Inventory to position the item correctly in the hand
    public void ApplyHandTransform()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(_heldRotationOffset);
    }
}