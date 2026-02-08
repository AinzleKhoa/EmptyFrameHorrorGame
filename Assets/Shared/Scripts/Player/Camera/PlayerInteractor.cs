using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _interactDistance = 3f; // How far the player can reach
    [SerializeField] private LayerMask _interactLayer;     // Set this to a 'Interactable' layer for performance

    [Header("References")]
    public Transform HandSocket;
    [SerializeField] private PlayerHUD _hud;

    private IInteractable _currentlyHeldItem; // Reference to the INTERFACE
    public bool IsHandFull => _currentlyHeldItem != null;

    private void Update()
    {
        // Drop Logic - Using 'Q' key
        if (Keyboard.current.qKey.wasPressedThisFrame && _currentlyHeldItem != null)
        {
            // We tell the interface to drop, it handles the details
            if (_currentlyHeldItem is PickableItem pickable) pickable.Drop();
            _currentlyHeldItem = null;
        }

        Ray ray = new Ray(transform.position, transform.forward); // Fire a raycast
        if (Physics.Raycast(ray, out RaycastHit hit, _interactDistance, _interactLayer)) // If an Item with 'Interactable' Layer, perform raycast
        {
            if (hit.collider.TryGetComponent(out IInteractable currentlyRaycastItem)) // If an Item with 'Interactable.cs' Script, enable logic
            {
                _hud.SetInteractionPrompt(currentlyRaycastItem.PromptMessage, true);

                // PickUp Logic - Using 'E' key
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    currentlyRaycastItem.Interact(this);
                }
                return;
            }
        }
        // If we hit nothing, hide the prompt
        _hud.SetInteractionPrompt("", false);
    }

    public void SetHeldItem(IInteractable item) => _currentlyHeldItem = item;
}