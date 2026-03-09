using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem; // Highly recommended for future-proof text

public class PlayerHUD : MonoBehaviour
{
    [Header("Sprint Modules")]
    [SerializeField] private CanvasGroup _sprintModule; // The SprintBar object
    [SerializeField] private Image _staminaFill;       // The Stamina object
    [SerializeField] private Image _crosshair;         // The Reticle/Crosshair

    [Header("Item Status Modules")]
    [SerializeField] private CanvasGroup _itemStatusModule; // The ItemStatus object
    [SerializeField] private Image _itemStatusIcon;  // The ItemStatusIcon object
    [SerializeField] private TextMeshProUGUI _statusText; // Drag the 'Charging...' text here
    [SerializeField] private Image _itemStatusFill;

    [Header("Interaction")]
    [SerializeField] private TextMeshProUGUI _promptText; // Drag 'InteractionPrompt' object here

    [Header("Inventory Container")]
    [SerializeField] private Image[] _inventoryItemIcon; // Drag your 3 ItemIcons here

    [Header("Selection Highlight")]
    [SerializeField] private RectTransform _selectionHighlight; // Drag the 'SelectionHighlight' here
    [SerializeField] private RectTransform[] _slotPositions;    // Drag Slot_1, Slot_2, Slot_3 here
    [SerializeField] private float _transitionSpeed = 10f;
    private Vector3 _targetPos;

    [Header("HUD Text (Objective, Stage Name, etc)")]
    [SerializeField] private TextMeshProUGUI _hudObjectiveText;
    [SerializeField] private TextMeshProUGUI _hudProgressionCountText;
    [SerializeField] private TextMeshProUGUI _hudStageNameText;

    [Header("HUD Management")]
    [SerializeField] private GameObject _normalStateGroup;
    [SerializeField] private GameObject _cameraStateGroup;

    [Header("Readable Item Display")]
    [SerializeField] private GameObject _readableContentPanel; // A panel to show readable content
    [SerializeField] private TextMeshProUGUI _readableContentText; // Text component to display the content
    [SerializeField] private TextMeshProUGUI _readableCloseTextGuidance;

    private void OnEnable()
    {
        // Listening for the PlayerMovement's stamina broadcast
        GameBroadcast.OnStaminaUpdate += UpdateStamina;

        // Listening for the PolaroidCamera's state broadcast
        GameBroadcast.OnCameraToggle += HandleCameraStateChange;

        // Listening for the Inventory's slot updates
        GameBroadcast.OnInventorySlotUpdate += UpdateInventorySlotImage;
        GameBroadcast.OnSlotSelected += SetSelectedSlot;

        // Listening for the Progression Manager's updates
        GameBroadcast.OnProgressionCountUpdateHUD += UpdateProgressionCountHUD;
        GameBroadcast.OnStageNameUpdateHUD += UpdateStageNameHUD;
        GameBroadcast.OnObjectiveUpdateHUD += UpdateObjectiveHUD;

        // Listening for Interactor's prompt requests
        GameBroadcast.OnInteractionPromptReq += SetInteractionPrompt;

        // Listening for Item status updates
        GameBroadcast.OnItemEquipped += HandleItemEquipped;
        GameBroadcast.OnItemStatusUpdate += HandleItemStatusUpdate;

        // Listening for Readable Item content display
        GameBroadcast.OnShowReadableContent += HandleShowReadableContent;
    }

    private void OnDisable()
    {
        // Always unsubscribe to prevent "Ghost" functions running in the background
        GameBroadcast.OnStaminaUpdate -= UpdateStamina;
        GameBroadcast.OnCameraToggle -= HandleCameraStateChange;
        GameBroadcast.OnInventorySlotUpdate -= UpdateInventorySlotImage;
        GameBroadcast.OnSlotSelected -= SetSelectedSlot;
        GameBroadcast.OnProgressionCountUpdateHUD -= UpdateProgressionCountHUD;
        GameBroadcast.OnStageNameUpdateHUD -= UpdateStageNameHUD;
        GameBroadcast.OnObjectiveUpdateHUD -= UpdateObjectiveHUD;
        GameBroadcast.OnInteractionPromptReq -= SetInteractionPrompt;
        GameBroadcast.OnItemEquipped -= HandleItemEquipped;
        GameBroadcast.OnItemStatusUpdate -= HandleItemStatusUpdate;
    }

    private void Awake()
    {
        _sprintModule.alpha = 0f; // Hide on initalization
        _itemStatusModule.alpha = 0f; // Hide on initalization
    }

    private void Update()
    {
        // Smoothly slide the highlight to the selected slot
        _selectionHighlight.position = Vector3.Lerp(_selectionHighlight.position, _targetPos, Time.deltaTime * _transitionSpeed);

        // Readable content can also be closed by pressing E
        if (_readableContentPanel.activeSelf && Keyboard.current.eKey.wasPressedThisFrame)
        {
            CloseReadable();
        }
    }

    private void HandleCameraStateChange(bool isOnCamera)
    {
        if (isOnCamera)
            EnableCameraState();
        else
            EnableNormalState();
    }

    // --- HUD STATE ---
    private void EnableNormalState()
    {
        if (_normalStateGroup != null) _normalStateGroup.SetActive(true);
        if (_cameraStateGroup != null) _cameraStateGroup.SetActive(false);
    }

    private void EnableCameraState()
    {
        if (_normalStateGroup != null) _normalStateGroup.SetActive(false);
        if (_cameraStateGroup != null) _cameraStateGroup.SetActive(true);
    }

    // --- STAMINA / SPRINT BAR LOGIC ---
    private void UpdateStamina(float currentStamina, float maxStamina)
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
    private void SetInteractionPrompt(string message, bool canInteract)
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
    private void UpdateInventorySlotImage(int index, Sprite icon)
    {
        if (index < _inventoryItemIcon.Length)
        {
            _inventoryItemIcon[index].sprite = icon;
            _inventoryItemIcon[index].color = Color.white; // Show it
        }
    }

    private void SetSelectedSlot(int index)
    {
        if (index >= 0 && index < _slotPositions.Length)
        {
            _targetPos = _slotPositions[index].position;
            _selectionHighlight.gameObject.SetActive(true);
        }
    }

    // --- ITEM STATUS LOGIC ---
    private void HandleItemEquipped(string itemName, Sprite itemStatusIcon) // Interact with PlayerInventory through PickableItem
    {
        if (_itemStatusModule == null || _itemStatusIcon == null) return;

        // Only show for the camera for now, can expand later
        if (itemName != "PolaroidCamera")
        {
            _itemStatusModule.alpha = 0f;
            _itemStatusIcon.sprite = null;
        }
        else
        {
            _itemStatusModule.alpha = 1f; // Show module
            _itemStatusIcon.sprite = itemStatusIcon;
        }
    }

    private void HandleItemStatusUpdate(string itemName, float progress)
    {
        if (_itemStatusModule == null || _statusText == null) return;
        if (itemName != "PolaroidCamera") return;

        _itemStatusFill.fillAmount = progress;

        if (progress >= 1f)
        {
            _statusText.text = "READY";
            _statusText.color = Color.green; // Optional: Visual cue
            _itemStatusFill.color = Color.green;
        }
        else
        {
            _statusText.text = "CHARGING...";
            _statusText.color = Color.red;
            _itemStatusFill.color = Color.red;
        }
    }

    // --- PROGRESSION HUD LOGIC ---
    private void UpdateProgressionCountHUD(int _currentCount, int _totalRequired)
    {
        if (_hudProgressionCountText != null) _hudProgressionCountText.text = $"{_currentCount}/{_totalRequired}";
    }

    private void UpdateStageNameHUD(string stageName)
    {
        if (_hudStageNameText != null) _hudStageNameText.text = stageName;
    }

    private void UpdateObjectiveHUD(string objective)
    {
        if (_hudObjectiveText != null) _hudObjectiveText.text = objective;
    }

    // --- READABLE ITEM LOGIC ---
    private void HandleShowReadableContent(string content, bool shouldShow)
    {
        if (shouldShow)
        {
            _readableContentText.text = content;
            _readableContentPanel.SetActive(true);
            _readableCloseTextGuidance.text = "Press [E] to Close";
        }
    }

    private void CloseReadable()
    {
        _readableContentPanel.SetActive(false);
        _readableContentText.text = "";
        GameBroadcast.isPlayerFreezed?.Invoke(false); // Unfreeze player when done
    }
}