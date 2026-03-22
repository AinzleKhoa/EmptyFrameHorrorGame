using UnityEngine;
using UnityEngine.UI;

public class InventoryHUD : BaseHUD
{
    [Header("Inventory Container")]
    [SerializeField] private Image[] _inventoryItemIcon;

    [Header("Selection Highlight")]
    [SerializeField] private RectTransform _selectionHighlight;
    [SerializeField] private RectTransform[] _slotPositions;
    [SerializeField] private float _transitionSpeed = 10f;

    private Vector3 _targetPos;

    protected override void OnEnable()
    {
        GameBroadcast.OnInventorySlotUpdate += UpdateInventorySlotImage;
        GameBroadcast.OnSlotSelected += SetSelectedSlot;
    }

    protected override void OnDisable()
    {
        // Unsubscribing prevents "ghost" calls from the static GameBroadcast
        GameBroadcast.OnInventorySlotUpdate -= UpdateInventorySlotImage;
        GameBroadcast.OnSlotSelected -= SetSelectedSlot;
    }

    private void Update()
    {
        // Safety check: Don't run logic if the HUD or highlight is being destroyed
        if (!IsValid || _selectionHighlight == null) return;

        // Smoothly slide the highlight to the selected slot (Original Logic)
        _selectionHighlight.position = Vector3.Lerp(_selectionHighlight.position, _targetPos, Time.deltaTime * _transitionSpeed);
    }

    // --- ORIGINAL WORKING LOGIC RESTORED ---
    private void SetSelectedSlot(int index)
    {
        if (!IsValid) return;

        if (index >= 0 && index < _slotPositions.Length)
        {
            // Ensure the slot position reference still exists
            if (_slotPositions[index] == null) return;

            _targetPos = _slotPositions[index].position;

            if (_selectionHighlight != null)
                _selectionHighlight.gameObject.SetActive(true);
        }
    }

    private void UpdateInventorySlotImage(int index, Sprite icon)
    {
        if (!IsValid) return;

        if (index >= 0 && index < _inventoryItemIcon.Length)
        {
            // Safeguard against missing array elements
            if (_inventoryItemIcon[index] == null) return;

            _inventoryItemIcon[index].sprite = icon;
            _inventoryItemIcon[index].color = Color.white;
        }
    }
}