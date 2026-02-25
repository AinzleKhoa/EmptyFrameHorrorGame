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

    private void Update()
    {
        // 2. New Input System syntax
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SwapToSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SwapToSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SwapToSlot(2);

        // Q to Drop
        if (Keyboard.current.qKey.wasPressedThisFrame) DropCurrentItem();
    }
    public void AddItem(PickableItem item)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null)
            {
                _slots[i] = item;

                // TELL THE ITEM: Stop being a physical object in the world
                item.SetPhysics(false);

                if (i == _currentSlotIndex) Equip(item);
                else Store(item);

                // BROADCAST: Tell the HUD to show this new icon
                GameBroadcast.OnInventorySlotUpdate?.Invoke(i, item.ItemIcon);
                return;
            }
        }
        // For later prompt inventory full
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

        // BROADCAST: Tell the HUD to move the selection highlight
        GameBroadcast.OnSlotSelected?.Invoke(_currentSlotIndex);
    }

    private void Equip(PickableItem item)
    {
        item.transform.SetParent(_handSocket);
        item.gameObject.SetActive(true);
        // Use the item's own logic for its specific rotation
        item.ApplyHandTransform();

        // BROADCAST: Tell the HUD what item is now equipped (for status effects, etc)
        GameBroadcast.OnItemEquipped?.Invoke(item.ItemName, item.ItemIcon);
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

        _slots[_currentSlotIndex] = null;

        // BROADCAST: Tell the HUD to show the empty icon for this slot
        GameBroadcast.OnInventorySlotUpdate?.Invoke(_currentSlotIndex, _defaultEmptyIcon);
        // BROADCAST: Tell the HUD that nothing is equipped (for status effects, etc)
        if (item.ItemName == "PolaroidCamera")
        {
            GameBroadcast.OnItemEquipped?.Invoke("None", null);
        }
    }
}