using UnityEngine;

public class L1H_PlaceSpotInteractable : MonoBehaviour, IInteractable
{
    [Header("Match with itemId")]
    [SerializeField] private string spotId = "pillow";

    [Header("Prompt")]
    [SerializeField] private string _placePrompt = "Press 'E' to Place";

    public string SpotId => spotId;

    public string PromptMessage
    {
        get
        {
            var held = L1H_HeldRegistry.CurrentHeld;
            if (held == null) return "";               // không cầm gì -> không hiện
            if (held.IsSnapped) return "";
            if (held.ItemId != spotId) return "";      // cầm sai đồ -> không hiện
            return _placePrompt;
        }
    }

    public void Interact(PlayerInteractor interactor)
    {
        var held = L1H_HeldRegistry.CurrentHeld;
        if (held == null) return;
        if (held.IsSnapped) return;
        if (held.ItemId != spotId) return;

        held.PlaceOn(transform);
    }
}