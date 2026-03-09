using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _promptText;
    [SerializeField] private Image _crosshair;

    private void OnEnable() => GameBroadcast.OnInteractionPromptReq += SetInteractionPrompt;
    private void OnDisable() => GameBroadcast.OnInteractionPromptReq -= SetInteractionPrompt;

    private void SetInteractionPrompt(string message, bool canInteract)
    {
        if (_promptText == null) return;

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