using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionHUD : BaseHUD
{
    [SerializeField] private TextMeshProUGUI _promptText;
    [SerializeField] private Image _crosshair;

    protected override void OnEnable() => GameBroadcast.OnInteractionPromptReq += SetInteractionPrompt;

    protected override void OnDisable() => GameBroadcast.OnInteractionPromptReq -= SetInteractionPrompt;

    private void SetInteractionPrompt(string message, bool canInteract)
    {
        // 1. Safety check: Is the HUD object still alive in the scene?
        if (!IsValid) return;

        // 2. Component check: Are the UI elements still valid?
        if (_promptText == null || _crosshair == null) return;

        _promptText.gameObject.SetActive(canInteract);
        if (canInteract)
        {
            _promptText.text = message;
            _crosshair.color = Color.yellow;
        }
        else
        {
            _crosshair.color = Color.white;
        }
    }
}