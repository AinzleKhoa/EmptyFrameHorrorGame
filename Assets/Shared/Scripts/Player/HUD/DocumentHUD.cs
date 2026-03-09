using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DocumentHUD : MonoBehaviour
{
    [SerializeField] private CanvasGroup _module;
    [SerializeField] private TextMeshProUGUI _contentText;
    [SerializeField] private TextMeshProUGUI _closeGuidance;

    [Header("Scrolling")]
    [SerializeField] private ScrollRect _scrollRect;

    private bool _isOpen = false;

    private void OnEnable() => GameBroadcast.OnShowReadableContent += HandleShow;
    private void OnDisable() => GameBroadcast.OnShowReadableContent -= HandleShow;

    private void HandleShow(string content, bool shouldShow)
    {
        if (shouldShow) OpenDocument(content);
    }

    private void OpenDocument(string content)
    {
        _isOpen = true;
        _contentText.text = content;
        _module.alpha = 1f;
        _module.blocksRaycasts = true;
        _module.interactable = true;
        _closeGuidance.text = "Press [E] to Close";

        // BROADCAST: This triggers the Action Map switch and unlocks the cursor
        GameBroadcast.OnInputStateChange?.Invoke("UI");

        ResetScrollPosition();
    }

    // TRIGGERED BY PLAYER INPUT: The "UI" Map has a "Close" action bound to [E]
    public void OnClose()
    {
        if (_isOpen) CloseDocument();
    }

    public void CloseDocument()
    {
        _isOpen = false;
        _module.alpha = 0f;
        _module.blocksRaycasts = false;
        _module.interactable = false;

        // BROADCAST: This switches back to "Player" map and re-locks the cursor
        GameBroadcast.OnInputStateChange?.Invoke("Player");
    }

    private void ResetScrollPosition()
    {
        if (_scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            _scrollRect.verticalNormalizedPosition = 1f;
        }
    }
}