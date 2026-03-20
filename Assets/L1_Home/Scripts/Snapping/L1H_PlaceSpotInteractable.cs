using UnityEngine;

public class L1H_PlaceSpotInteractable : MonoBehaviour, IInteractable
{
    [Header("Match with itemId")]
    [SerializeField] private string spotId = "pillow";

    [Header("Prompt")]
    [SerializeField] private string _placePrompt = "Press 'E' to Place";

    [Header("State")]
    [SerializeField] private bool _canInteractAtStart = true;

    private bool _canInteract;

    public string SpotId => spotId;

    private void Awake()
    {
        _canInteract = _canInteractAtStart;
    }

    public string PromptMessage
    {
        get
        {
            if (!_canInteract) return "";
            if (!L1H_InteractRules.CanInteractWithPlaceSpot(spotId)) return "";

            return _placePrompt;
        }
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!_canInteract) return;
        if (!L1H_InteractRules.CanInteractWithPlaceSpot(spotId)) return;

        var held = L1H_HeldRegistry.CurrentHeld;
        if (held == null) return;

        held.PlaceOn(transform);
    }

    public void EnableInteract() => _canInteract = true;
    public void DisableInteract() => _canInteract = false;
}