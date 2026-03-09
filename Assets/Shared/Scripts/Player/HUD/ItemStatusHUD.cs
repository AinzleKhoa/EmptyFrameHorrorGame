using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemStatusHUD : MonoBehaviour
{
    [SerializeField] private CanvasGroup _module;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _fill;
    [SerializeField] private TextMeshProUGUI _statusText;

    private string _currentEquippedName;

    private void OnEnable()
    {
        GameBroadcast.OnItemEquipped += HandleEquipped;
        GameBroadcast.OnItemStatusUpdate += HandleStatus;
    }

    private void HandleEquipped(string name, Sprite icon)
    {
        _currentEquippedName = name;

        // Get the item from your inventory helper
        var inventory = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerInventory>();
        var heldItem = inventory.GetCurrentlyEquippedItem();

        // Does it have a cooldown script?
        bool shouldShow = heldItem != null && heldItem.GetComponent<ItemCooldown>() != null;

        // Apply to the HUD
        _module.alpha = shouldShow ? 1f : 0f;

        if (shouldShow)
        {
            _icon.sprite = icon;
        }
    }

    private void HandleStatus(string itemName, float progress)
    {
        // FILTER: If the message is from a different item, ignore it!
        if (itemName != _currentEquippedName || _module.alpha <= 0) return;

        _fill.fillAmount = progress;

        // Visual Logic
        bool isReady = progress >= 1f;
        _fill.color = isReady ? Color.green : Color.red;
        _statusText.color = isReady ? Color.green : Color.red;
        _statusText.text = isReady ? "READY" : "CHARGING...";
    }
}