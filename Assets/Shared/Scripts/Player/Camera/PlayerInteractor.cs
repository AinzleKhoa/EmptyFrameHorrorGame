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
                    GameBroadcast.OnInteractionPromptReq?.Invoke(interactable.PromptMessage, true);

                    if (Keyboard.current.eKey.wasPressedThisFrame)
                    {
                        // 1. Do the base interaction (Show text/Pick up)
                        interactable.Interact(this);

                        // 2. Automatically check for a Signal (Optional)
                        if (hit.collider.TryGetComponent<SignalTrigger>(out var signal))
                        {
                            signal.RaiseSignal();
                        }
                    }
                    return;
                }
            }

            // If we look away or hit something non-interactable, hide the prompt
            // BROADCAST: Request the HUD to hide the prompt
            GameBroadcast.OnInteractionPromptReq?.Invoke("", false);
        }
    }
}