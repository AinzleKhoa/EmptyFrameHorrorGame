using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _interactDistance = 3f;
    [SerializeField] private LayerMask _interactLayer;
    private IInteractable _currentInteractable; // Store what we are looking at

    // Local state to track if we should block interaction
    private bool _isOnCamera = false;

    private void OnEnable()
    {
        // Start listening for camera toggle
        GameBroadcast.OnCameraToggle += HandleCameraToggle;
    }

    private void OnDisable()
    {
        // Stop listening
        GameBroadcast.OnCameraToggle -= HandleCameraToggle;
    }
    private void HandleCameraToggle(bool isOnCamera)
    {
        _isOnCamera = isOnCamera;
    }

    public void OnInteract()
    {
        // If we are looking at something valid, interact with it
        if (_currentInteractable != null)
        {
            _currentInteractable.Interact(this);
        }
    }

    private void Update()
    {
        // Default state: no interaction
        _currentInteractable = null;
        GameBroadcast.OnInteractionPromptReq?.Invoke("", false);

        // We still need the Raycast in Update to show the "Press E" prompt
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _interactDistance, _interactLayer))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                _currentInteractable = interactable;

                // BROADCAST: Show the prompt on screen
                GameBroadcast.OnInteractionPromptReq?.Invoke(interactable.PromptMessage, true);
            }
        }
    }
}
