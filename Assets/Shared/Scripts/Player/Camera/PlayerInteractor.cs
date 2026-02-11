using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _interactDistance = 3f;
    [SerializeField] private LayerMask _interactLayer;

    [Header("References")]
    [SerializeField] private PlayerHUD _hud;

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _interactDistance, _interactLayer))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                // Show the specific message from the item (e.g., "Press E to Pick Up")
                _hud.SetInteractionPrompt(interactable.PromptMessage, true);

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    interactable.Interact(this);
                }
                return;
            }
        }

        // If we look away or hit something non-interactable, hide the prompt
        _hud.SetInteractionPrompt("", false);
    }
}