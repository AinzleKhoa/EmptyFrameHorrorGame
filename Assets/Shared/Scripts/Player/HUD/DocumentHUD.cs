using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DocumentHUD : BaseHUD
{
    [SerializeField] private CanvasGroup _module;
    [SerializeField] private TextMeshProUGUI _contentText;
    [SerializeField] private TextMeshProUGUI _closeGuidance;

    [Header("Scrolling")]
    [SerializeField] private ScrollRect _scrollRect;

    private bool _isOpen = false;

    private void HandleShow(string content, bool shouldShow)
    {
        // Use IsValid check from BaseHUD
        if (!IsValid) return;

        if (shouldShow) OpenDocument(content);
        else if (_isOpen) CloseDocument();
    }
    private void OpenDocument(string content)
    {
        if (!IsValid || _module == null) return;

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
        // If the object was destroyed mid-frame, don't execute
        if (!IsValid || !_isOpen) return;
        CloseDocument();
    }

    public void CloseDocument()
    {
        _isOpen = false;
        // Safety check before accessing component
        if (_module != null)
        {
            _module.alpha = 0f;
            _module.blocksRaycasts = false;
            _module.interactable = false;
        }

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

    // Standard BaseHUD overrides for event safety
    protected override void OnEnable() => GameBroadcast.OnShowReadableContent += HandleShow;

    protected override void OnDisable()
    {
        GameBroadcast.OnShowReadableContent -= HandleShow;

        // SAFEGUARD: If the scene is destroyed while the document is open,
        // we reset the state so the next scene starts clean.
        if (_isOpen)
        {
            _isOpen = false;
            // We don't invoke OnInputStateChange here because the Broadcast 
            // system might already be shutting down during a scene load.
        }
    }
}