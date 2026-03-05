using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _interactDistance = 3f;
    [SerializeField] private LayerMask _interactLayer;
    private bool _isPlayerFreezed = false; // Local state to track if player is frozen

    // Local state to track if we should block interaction
    private bool _isOnCamera = false;

    private void OnEnable()
    {
        // Start listening for camera toggle
        GameBroadcast.OnCameraToggle += HandleCameraToggle;
        GameBroadcast.isPlayerFreezed += HandlePlayerFreeze;
    }

    private void OnDisable()
    {
        // Stop listening
        GameBroadcast.OnCameraToggle -= HandleCameraToggle;
        GameBroadcast.isPlayerFreezed -= HandlePlayerFreeze;
    }

    private void HandlePlayerFreeze(bool isFreezed)
    {
        _isPlayerFreezed = isFreezed;
    }

    private void HandleCameraToggle(bool isOnCamera)
    {
        _isOnCamera = isOnCamera;
    }

    private void Update()
    {
        // If we look away or hit something non-interactable, hide the prompt
        GameBroadcast.OnInteractionPromptReq?.Invoke("", false);

        if (_isPlayerFreezed) return; // If player is frozen, skip interaction checks

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
                }
                return;
            }
        }
    }
}