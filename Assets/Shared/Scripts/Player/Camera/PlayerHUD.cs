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

    // --- SPRINT LOGIC ---
    public void UpdateSprint(float current, float max, bool isMoving)
    {
        _staminaFill.fillAmount = current / max;
        // Fades in when moving or tired, fades out when still and full
        float targetAlpha = (current < max || isMoving) ? 1f : 0f;
        _sprintModule.alpha = Mathf.MoveTowards(_sprintModule.alpha, targetAlpha, Time.deltaTime * 2f);
    }

    // --- INTERACTION LOGIC ---
    public void SetInteractionPrompt(string message, bool canInteract)
    {
        if (_promptText == null) return;

        if (canInteract)
        {
            _promptText.text = message;
            _promptText.gameObject.SetActive(true);

            // Subtle visual feedback: change crosshair color
            _crosshair.color = Color.yellow;
        }
        else
        {
            _promptText.gameObject.SetActive(false);
            _crosshair.color = Color.white;
        }
    }
}