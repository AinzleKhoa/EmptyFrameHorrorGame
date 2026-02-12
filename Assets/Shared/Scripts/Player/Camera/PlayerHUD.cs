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

    [Header("Counter Text")]
    [SerializeField] private TextMeshProUGUI _hudCounterText;
    [SerializeField] private TextMeshProUGUI _hudStageNameText;

    [Header("HUD Management")]
    [SerializeField] private GameObject _normalStateGroup;
    [SerializeField] private GameObject _cameraStateGroup;

    private void OnEnable()
    {
        // Listening for the PlayerMovement's stamina broadcast
        GameEvents.OnStaminaUpdate += UpdateStamina;

        // Listening for the PolaroidCamera's state broadcast
        GameEvents.OnCameraToggle += HandleCameraStateChange;

        // Listening for the Inventory's slot updates
        GameEvents.OnInventorySlotUpdate += UpdateInventorySlotImage;
        GameEvents.OnSlotSelected += SetSelectedSlot;

        // Listening for the Progression Manager's updates
        GameEvents.OnFragmentUpdate += UpdateFragmentProgression;
        GameEvents.OnStageNameUpdate += UpdateStageName;

        // Listening for Interactor's prompt requests
        GameEvents.OnInteractionPromptReq += SetInteractionPrompt;
    }

    private void OnDisable()
    {
        // Always unsubscribe to prevent "Ghost" functions running in the background
        GameEvents.OnStaminaUpdate -= UpdateStamina;
        GameEvents.OnCameraToggle -= HandleCameraStateChange;
        GameEvents.OnInventorySlotUpdate -= UpdateInventorySlotImage;
        GameEvents.OnSlotSelected -= SetSelectedSlot;
        GameEvents.OnFragmentUpdate -= UpdateFragmentProgression;
        GameEvents.OnStageNameUpdate -= UpdateStageName;
        GameEvents.OnInteractionPromptReq -= SetInteractionPrompt;
    }

    private void Awake()
    {
        _sprintModule.alpha = 0f; // Hide on initalization
    }

    private void Update()
    {
        // Smoothly slide the highlight to the selected slot
        _selectionHighlight.position = Vector3.Lerp(_selectionHighlight.position, _targetPos, Time.deltaTime * _transitionSpeed);
    }

    private void HandleCameraStateChange(bool isOnCamera)
    {
        if (isOnCamera)
            EnableCameraState();
        else
            EnableNormalState();
    }

    // --- HUD STATE ---
    public void EnableNormalState()
    {
        if (_normalStateGroup != null) _normalStateGroup.SetActive(true);
        if (_cameraStateGroup != null) _cameraStateGroup.SetActive(false);
    }

    public void EnableCameraState()
    {
        if (_normalStateGroup != null) _normalStateGroup.SetActive(false);
        if (_cameraStateGroup != null) _cameraStateGroup.SetActive(true);
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
    // --- INTERACTION PROMPT LOGIC ---
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

    public void UpdateFragmentProgression(int _currentCount, int _totalRequired)
    {
        if (_hudCounterText != null) _hudCounterText.text = $"{_currentCount}/{_totalRequired}";
    }

    public void UpdateStageName(string stageName)
    {
        if (_hudStageNameText != null) _hudStageNameText.text = stageName;
    }
}