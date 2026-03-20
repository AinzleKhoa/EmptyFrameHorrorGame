using UnityEngine;

public class UpgradeInteractable : InteractableBase
{
    [Header("Targeting")]
    [SerializeField] private string _targetItemName = "PolaroidCamera";
    // [SerializeField] private string _upgradeID = "Battery_V1";

    [Header("UI Message")]
    [SerializeField] private string _readyPrompt = "Press [E] to Upgrade";
    [SerializeField] private string _missingPrompt = "Requires Polaroid Camera";
    public override string PromptMessage
    {
        get
        {
            if (_targetItemName == "Player") return _readyPrompt; // If no item is required, always show ready prompt

            // Simple check: Find the player and check their inventory
            var inv = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerInventory>();

            if (inv != null && inv.GetItemByName(_targetItemName) != null)
            {
                return _readyPrompt;
            }

            return _missingPrompt;
        }
    }

    public override void Interact(PlayerInteractor interactor)
    {
        base.Interact(interactor);

        PlayerInventory inventory = interactor.GetComponentInParent<PlayerInventory>();

        // 1. Handle Player-Targeted Upgrades
        if (_targetItemName == "Player")
        {
            foreach (var effect in GetComponents<IUpgradeEffect>())
            {
                effect.ExecuteUpgrade(null); // Runs your Stamina/Speed logic
            }

            // Refresh HUD immediately with PlayerData
            if (inventory.GetCurrentlyEquippedItem() == null)
            {
                PlayerData pData = interactor.GetComponentInParent<PlayerData>();
                if (pData != null) GameBroadcast.OnUpdateItemDescription?.Invoke(pData.FullStatusDescription);
            }

            gameObject.SetActive(false);
            return;
        }

        // 2. Handle Item-Targeted Upgrades
        PickableItem target = inventory?.GetItemByName(_targetItemName);
        if (target != null)
        {
            foreach (var effect in GetComponents<IUpgradeEffect>())
            {
                effect.ExecuteUpgrade(target);
            }

            // REFRESH HUD: Since we are holding the item we just upgraded, refresh its lore
            if (target.TryGetComponent<ItemData>(out var itemData))
            {
                GameBroadcast.OnUpdateItemDescription?.Invoke(itemData.FullDescription);
            }

            gameObject.SetActive(false);
        }
    }
}