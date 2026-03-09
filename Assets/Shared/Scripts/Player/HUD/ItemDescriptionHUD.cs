using UnityEngine;
using TMPro;

public class ItemDescriptionHUD : MonoBehaviour
{
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private TextMeshProUGUI _displayText;

    private void OnEnable() => GameBroadcast.OnUpdateItemDescription += UpdateUI;
    private void OnDisable() => GameBroadcast.OnUpdateItemDescription -= UpdateUI;

    private void Awake() => _group.alpha = 0f; // Start hidden

    private void UpdateUI(string fullContent)
    {
        if (string.IsNullOrEmpty(fullContent))
        {
            _group.alpha = 0f;
            return;
        }

        _displayText.text = fullContent;
        _group.alpha = 1f;
    }
}