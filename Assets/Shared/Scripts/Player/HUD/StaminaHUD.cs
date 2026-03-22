using UnityEngine;
using UnityEngine.UI;

public class StaminaHUD : BaseHUD // Inherit from BaseHUD
{
    [SerializeField] private CanvasGroup _sprintModule;
    [SerializeField] private Image _staminaFill;

    private void Awake()
    {
        // Safety check for initialization
        if (_sprintModule != null) _sprintModule.alpha = 0f;
    }

    // Standard BaseHUD overrides
    protected override void OnEnable() => GameBroadcast.OnStaminaUpdate += UpdateStamina;

    protected override void OnDisable() => GameBroadcast.OnStaminaUpdate -= UpdateStamina;

    private void UpdateStamina(float current, float max)
    {
        // 1. Universal Safeguard: Is the HUD object alive?
        if (!IsValid) return;

        // 2. Component Safeguard: Ensure the references haven't been destroyed
        if (_sprintModule == null || _staminaFill == null) return;

        float fill = current / max;

        // Update the visual fill
        _staminaFill.fillAmount = fill;

        // Show the module if stamina is used, hide if full
        _sprintModule.alpha = (fill < 1f) ? 1f : 0f;
    }
}