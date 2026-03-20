using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _interactDistance = 3f;
    [SerializeField] private LayerMask _blockerLayers;
    [SerializeField] private int _interactableLayerIndex = 7;

    [Header("Input")]
    [SerializeField] private PlayerInput _playerInput; // Drag your PlayerInput component here
    private InputAction _interactAction;

    private IInteractable _currentInteractable;

    private void Awake()
    {
        // Find the "Interact" action inside your Input Asset
        _interactAction = _playerInput.actions["Interact"];
    }

    // This handles the "One-Tap" (Pickup, open door)
    public void OnInteract(InputValue value)
    {
        if (value.isPressed && _currentInteractable != null)
        {
            _currentInteractable.Interact(this);
        }
    }

    private void Update()
    {
        _currentInteractable = null;
        GameBroadcast.OnInteractionPromptReq?.Invoke("", false);

        // Check if the button is CURRENTLY being held down
        bool isHolding = _interactAction.IsPressed();

        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _interactDistance, _blockerLayers))
        {
            if (hit.collider.gameObject.layer == _interactableLayerIndex)
            {
                if (hit.collider.TryGetComponent(out IInteractable interactable))
                {
                    _currentInteractable = interactable;
                    GameBroadcast.OnInteractionPromptReq?.Invoke(interactable.PromptMessage, true);

                    // Check for the IHoldable interface
                    if (isHolding && interactable is IHoldable holdable)
                    {
                        holdable.OnInteractHold(this);
                    }
                }
            }
        }
    }
}