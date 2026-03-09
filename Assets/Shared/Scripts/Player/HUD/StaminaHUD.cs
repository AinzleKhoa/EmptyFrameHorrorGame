using UnityEngine;
using UnityEngine.UI;

public class StaminaHUD : MonoBehaviour
{
    [SerializeField] private CanvasGroup _sprintModule;
    [SerializeField] private Image _staminaFill;

    private void Awake() => _sprintModule.alpha = 0f; // Original initialization

    private void OnEnable() => GameBroadcast.OnStaminaUpdate += UpdateStamina;

    private void UpdateStamina(float current, float max)
    {
        float fill = current / max;
        _staminaFill.fillAmount = fill;
        _sprintModule.alpha = (fill < 1f) ? 1f : 0f;
    }
}