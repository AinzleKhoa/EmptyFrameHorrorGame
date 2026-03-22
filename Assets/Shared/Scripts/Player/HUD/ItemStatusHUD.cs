using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemStatusHUD : BaseHUD // Inherit from BaseHUD
{
    [SerializeField] private CanvasGroup _module;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _fill;
    [SerializeField] private TextMeshProUGUI _statusText;

    private string _currentEquippedName;

    protected override void OnEnable()
    {
        GameBroadcast.OnItemEquipped += HandleEquipped;
        GameBroadcast.OnItemStatusUpdate += HandleStatus;
    }

    protected override void OnDisable()
    {
        GameBroadcast.OnItemEquipped -= HandleEquipped;
        GameBroadcast.OnItemStatusUpdate -= HandleStatus;
    }

    private void HandleEquipped(string name, Sprite icon)
    {
        // 1. Universal Safeguard
        if (!IsValid) return;

        _currentEquippedName = name;

        // 2. Player Search Safeguard: During scene load, FindWithTag might fail
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        var inventory = player.GetComponentInChildren<PlayerInventory>();
        if (inventory == null) return;

        var heldItem = inventory.GetCurrentlyEquippedItem();

        // 3. Component Safeguard
        bool shouldShow = heldItem != null && heldItem.GetComponent<ItemCooldown>() != null;

        if (_module != null)
        {
            _module.alpha = shouldShow ? 1f : 0f;
        }

        if (shouldShow && _icon != null)
        {
            _icon.sprite = icon;
        }
    }

    private void HandleStatus(string itemName, float progress)
    {
        // 1. Universal Safeguard
        if (!IsValid) return;

        // 2. Component Safeguard: Ensure the CanvasGroup still exists
        if (_module == null) return;

        // FILTER: If the message is from a different item, ignore it!
        if (itemName != _currentEquippedName || _module.alpha <= 0) return;

        // 3. UI Element Safeguards
        if (_fill != null)
        {
            _fill.fillAmount = progress;
            bool isReady = progress >= 1f;
            _fill.color = isReady ? Color.green : Color.red;
        }

        if (_statusText != null)
        {
            bool isReady = progress >= 1f;
            _statusText.color = isReady ? Color.green : Color.red;
            _statusText.text = isReady ? "READY" : "CHARGING...";
        }
    }
}