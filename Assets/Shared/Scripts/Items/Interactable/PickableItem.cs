using UnityEngine;

public class PickableItem : MonoBehaviour, IInteractable
{
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

    public void Interact(PlayerInteractor interactor)
    {
        if (interactor.IsHandFull) return;
        // Tell interactor we are the held item
        interactor.SetHeldItem(this);
        // 1. Physics & Collision logic
        if (_rb) _rb.isKinematic = true;
        if (_col) _col.enabled = false; // Prevents Minh from "tripping" on the item
        // 2. Parenting & Positioning
        transform.SetParent(interactor.HandSocket);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(_heldRotationOffset);

        Debug.Log($"{gameObject.name} Picked Up!");
    }

    public void Drop()
    {
        // 1. Detach
        transform.SetParent(null);
        // 2. Re-enable Collisions
        if (_col) _col.enabled = true;
        // 3. Re-enable Physics & Add "Toss"
        if (_rb)
        {
            _rb.isKinematic = false;
            _rb.AddForce(transform.forward * 2f + Vector3.up * 0.5f, ForceMode.Impulse);
        }

        Debug.Log($"{gameObject.name} Dropped!");
    }
}