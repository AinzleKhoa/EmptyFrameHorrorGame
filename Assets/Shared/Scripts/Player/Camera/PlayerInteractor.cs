using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _interactDistance = 3f;
    [SerializeField] private LayerMask _interactLayer;

    // Local state to track if we should block interaction
    private bool _isOnCamera = false;

    private void OnEnable()
    {
        // Start listening for camera toggle
        GameEvents.OnCameraToggle += HandleCameraToggle;
    }

    private void OnDisable()
    {
        // Stop listening
        GameEvents.OnCameraToggle -= HandleCameraToggle;
    }

    private void HandleCameraToggle(bool isOnCamera)
    {
        _isOnCamera = isOnCamera;
    }

    private void Update()
    {
        if (!_isOnCamera)
        {
            Ray ray = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, _interactDistance, _interactLayer))
            {
                if (hit.collider.TryGetComponent(out IInteractable interactable))
                {
                    // BROADCAST: Request the HUD to show the prompt
                    GameEvents.OnInteractionPromptReq?.Invoke(interactable.PromptMessage, true);

                    if (Keyboard.current.eKey.wasPressedThisFrame)
                    {
                        interactable.Interact(this);
                    }
                    return;
                }
            }

            // If we look away or hit something non-interactable, hide the prompt
            // BROADCAST: Request the HUD to hide the prompt
            GameEvents.OnInteractionPromptReq?.Invoke("", false);
        }
    }
}