using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _handSocket;      // Drag 'HandSocket' here
    [SerializeField] private Transform _inventoryStorage; // Drag 'InventoryStorage' here

    [Header("Storage")]
    [SerializeField] private Sprite _defaultEmptyIcon;
    private PickableItem[] _slots = new PickableItem[3];
    private int _currentSlotIndex = 0;

    // --- INPUT MESSAGES ---
    public void OnSlot1() => SwapToSlot(0);
    public void OnSlot2() => SwapToSlot(1);
    public void OnSlot3() => SwapToSlot(2);

    public void OnDrop() => DropCurrentItem();

    // --- LOGIC ---
    public void AddItem(PickableItem item)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null)
            {
                _slots[i] = item;

                // TELL THE ITEM: Stop being a physical object in the world
                item.SetPhysics(false);
                item.HideOutline();

                if (i == _currentSlotIndex) Equip(item);
                else Store(item);

                // BROADCAST: Tell the HUD to show this new icon
                // Get Identity from ItemData for the HUD Icon
                if (item.TryGetComponent<ItemData>(out var data))
                {
                    GameBroadcast.OnInventorySlotUpdate?.Invoke(i, data.ItemIcon);
                }
                return;
            }
        }
    }

    private void SwapToSlot(int newIndex)
    {
        if (newIndex == _currentSlotIndex) return;

        // 1. Put the item Minh is currently holding AWAY first
        if (_slots[_currentSlotIndex] != null)
        {
            Store(_slots[_currentSlotIndex]);
        }

        // 2. Update the index to the new slot
        _currentSlotIndex = newIndex;

        // 3. Take the item in the new slot and put it in Minh's HAND
        if (_slots[_currentSlotIndex] != null)
        {
            Equip(_slots[_currentSlotIndex]);
        }
        else
        {
            // If the new slot is empty, hide the description ---
            // Assuming ItemData is on the same object as PlayerMovement (the parent of Inventory)
            ShowPlayerStatus();

            GameBroadcast.OnItemEquipped?.Invoke("None", null);
        }

        // BROADCAST: Tell the HUD to move the selection highlight
        GameBroadcast.OnSlotSelected?.Invoke(_currentSlotIndex);
    }

    private void Equip(PickableItem item)
    {
        item.transform.SetParent(_handSocket);
        item.gameObject.SetActive(true);
        // Use the item's own logic for its specific rotation
        item.ApplyHandTransform();

        // Broadcast Name, Icon, and Description from ItemData
        if (item.TryGetComponent<ItemData>(out var data))
        {
            GameBroadcast.OnUpdateItemDescription?.Invoke(data.FullDescription);
            GameBroadcast.OnItemEquipped?.Invoke(data.ItemName, data.ItemIcon);
        }
    }

    private void Store(PickableItem item)
    {
        item.transform.SetParent(_inventoryStorage);
        item.gameObject.SetActive(false);
    }

    public void DropCurrentItem()
    {
        if (_slots[_currentSlotIndex] == null) return;

        PickableItem item = _slots[_currentSlotIndex];
        item.transform.SetParent(null);
        item.SetPhysics(true);
        item.ShowOutline();

        _slots[_currentSlotIndex] = null;
        GameBroadcast.OnInventorySlotUpdate?.Invoke(_currentSlotIndex, _defaultEmptyIcon);
        PlayerData pData = GetComponentInParent<PlayerData>();
        ShowPlayerStatus();

        GameBroadcast.OnItemEquipped?.Invoke("None", null);
    }

    public PickableItem GetItemByName(string itemName)
    {
        foreach (var item in _slots)
        {
            // Search through the ItemData component of each slotted item
            if (item != null && item.TryGetComponent<ItemData>(out var data))
            {
                if (data.ItemName == itemName) return item;
            }
        }
        return null;
    }

    // Returns the item in the current active slot
    public PickableItem GetCurrentlyEquippedItem()
    {
        return _slots[_currentSlotIndex];
    }

    // Simple helper to show the player's own status when no item is equipped
    private void ShowPlayerStatus()
    {
        PlayerData pData = GetComponentInParent<PlayerData>();
        if (pData != null)
            GameBroadcast.OnUpdateItemDescription?.Invoke(pData.FullStatusDescription);
        else
            GameBroadcast.OnUpdateItemDescription?.Invoke("");
    }
}