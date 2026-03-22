using UnityEngine;
using TMPro;

public class ItemDescriptionHUD : BaseHUD // Inherit from BaseHUD
{
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private TextMeshProUGUI _displayText;

    // BaseHUD overrides handle the event subscription safely
    protected override void OnEnable() => GameBroadcast.OnUpdateItemDescription += UpdateUI;
    protected override void OnDisable() => GameBroadcast.OnUpdateItemDescription -= UpdateUI;

    private void Awake()
    {
        // Safety check for initialization
        if (_group != null) _group.alpha = 0f;
    }

    private void UpdateUI(string fullContent)
    {
        // 1. Safety check: Is the HUD object still alive?
        if (!IsValid) return;

        // 2. Component check: Is the CanvasGroup still in memory?
        if (_group == null) return;

        if (string.IsNullOrEmpty(fullContent))
        {
            _group.alpha = 0f;
            return;
        }

        // 3. Component check: Is the TextMeshProUGUI still in memory?
        if (_displayText != null)
        {
            _displayText.text = fullContent;
            _group.alpha = 1f;
        }
    }
}