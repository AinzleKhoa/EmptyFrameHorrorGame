using UnityEngine;
using UnityEngine.UI;
using TMPro; // Highly recommended for future-proof text

public class PlayerHUD : MonoBehaviour
{
    [Header("Modules")]
    [SerializeField] private CanvasGroup _sprintModule; // The SprintBar object
    [SerializeField] private Image _staminaFill;       // The Stamina object
    [SerializeField] private Image _crosshair;         // The Reticle/Crosshair object

    [Header("Interaction")]
    [SerializeField] private TextMeshProUGUI _promptText; // Drag 'InteractionPrompt' object here

    [Header("Inventory Container")]
    [SerializeField] private Image[] _inventoryItemIcon; // Drag your 3 ItemIcons here

    [Header("Selection Highlight")]
    [SerializeField] private RectTransform _selectionHighlight; // Drag the 'SelectionHighlight' here
    [SerializeField] private RectTransform[] _slotPositions;    // Drag Slot_1, Slot_2, Slot_3 here
    [SerializeField] private float _transitionSpeed = 10f;
    private Vector3 _targetPos;

    private void Awake()
    {
        _sprintModule.alpha = 0f; // Hide on initalization
    }

    private void Update()
    {
        // Smoothly slide the highlight to the selected slot
        _selectionHighlight.position = Vector3.Lerp(_selectionHighlight.position, _targetPos, Time.deltaTime * _transitionSpeed);
    }

    // --- STAMINA / SPRINT BAR LOGIC ---
    public void UpdateStamina(float currentStamina, float maxStamina)
    {
        if (_sprintModule == null || _staminaFill == null) return;

        // Calculate fill amount (0 to 1)
        float fillAmount = currentStamina / maxStamina;
        _staminaFill.fillAmount = fillAmount;

        // Show bar if stamina not full, hide it if 100%
        if (fillAmount < 1.0f)
        {
            _sprintModule.alpha = 1f; // Visible
        }
        else
        {
            _sprintModule.alpha = 0f; // Hide
        }
    }
    // --- INTERACTION LOGIC ---
    public void SetInteractionPrompt(string message, bool canInteract)
    {
        if (_promptText == null) return;

        if (canInteract)
        {
            _promptText.text = message;
            _promptText.gameObject.SetActive(true);
            _crosshair.color = Color.yellow;
        }
        else
        {
            _promptText.gameObject.SetActive(false);
            _crosshair.color = Color.white;
        }
    }

    // --- INVENTORY LOGIC ---
    public void UpdateInventorySlotImage(int index, Sprite icon)
    {
        if (index < _inventoryItemIcon.Length)
        {
            _inventoryItemIcon[index].sprite = icon;
            _inventoryItemIcon[index].color = Color.white; // Show it
        }
    }

    public void SetSelectedSlot(int index)
    {
        if (index >= 0 && index < _slotPositions.Length)
        {
            _targetPos = _slotPositions[index].position;
            _selectionHighlight.gameObject.SetActive(true);
        }
    }
}