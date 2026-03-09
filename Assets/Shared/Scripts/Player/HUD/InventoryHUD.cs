using UnityEngine;
using UnityEngine.UI;

public class InventoryHUD : MonoBehaviour
{
    [Header("Inventory Container")]
    [SerializeField] private Image[] _inventoryItemIcon;

    [Header("Selection Highlight")]
    [SerializeField] private RectTransform _selectionHighlight;
    [SerializeField] private RectTransform[] _slotPositions;
    [SerializeField] private float _transitionSpeed = 10f;

    private Vector3 _targetPos;

    private void OnEnable()
    {
        GameBroadcast.OnInventorySlotUpdate += UpdateInventorySlotImage;
        GameBroadcast.OnSlotSelected += SetSelectedSlot;
    }

    private void OnDisable()
    {
        // Unsubscribing is essential for modular scripts to prevent "ghost" errors
        GameBroadcast.OnInventorySlotUpdate -= UpdateInventorySlotImage;
        GameBroadcast.OnSlotSelected -= SetSelectedSlot;
    }

    private void Update()
    {
        // Smoothly slide the highlight to the selected slot (Original Logic)
        _selectionHighlight.position = Vector3.Lerp(_selectionHighlight.position, _targetPos, Time.deltaTime * _transitionSpeed);
    }

    // --- ORIGINAL WORKING LOGIC RESTORED ---
    private void SetSelectedSlot(int index)
    {
        if (index >= 0 && index < _slotPositions.Length)
        {
            _targetPos = _slotPositions[index].position;
            _selectionHighlight.gameObject.SetActive(true); // This was the missing line!
        }
    }

    private void UpdateInventorySlotImage(int index, Sprite icon)
    {
        if (index < _inventoryItemIcon.Length)
        {
            _inventoryItemIcon[index].sprite = icon;
            _inventoryItemIcon[index].color = Color.white;
        }
    }
}