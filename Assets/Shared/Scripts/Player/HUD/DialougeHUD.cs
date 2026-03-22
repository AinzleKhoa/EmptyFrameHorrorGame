using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CanvasGroup))]
public class DialougeHUD : BaseHUD
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _textDisplay;

    [Header("Settings")]
    [SerializeField] private float _typingSpeed = 0.05f;
    [SerializeField] private GameSignal _onDialogueFinishedSignal;

    private List<string> _currentDialogueLines;
    private int _lineIndex;
    private bool _isTyping;
    private bool _isActive;
    private Coroutine _typingRoutine;

    private void Awake()
    {
        // Ensure we have the reference
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
        HideUI();
    }

    public void OnSkip()
    {
        if (!_isActive || !IsValid) return;

        Debug.Log("Dialogue fully skipped via Input Message");
        EndDialogue();
    }

    // Broadcast Message: Triggered when 'E' is pressed in UI Map
    public void OnClose()
    {
        if (!_isActive || !IsValid) return;

        if (_isTyping)
        {
            // Instant skip typing
            StopAllCoroutines();
            _textDisplay.text = _currentDialogueLines[_lineIndex];
            _isTyping = false;
        }
        else
        {
            NextLine();
        }
    }

    // Called by StoryDirector via UnityEvent
    public void StartDialogue(DialogueData data)
    {
        if (data == null || data.Lines.Count == 0 || !IsValid) return;

        _isActive = true;
        _currentDialogueLines = data.Lines;
        _lineIndex = 0;

        // 1. Switch to UI State via your switch-case controller
        GameBroadcast.OnInputStateChange?.Invoke("UI");

        // 2. Show UI via Alpha
        ShowUI();
        _typingRoutine = StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        _isTyping = true;
        _textDisplay.text = "";

        foreach (char c in _currentDialogueLines[_lineIndex].ToCharArray())
        {
            // Safety check inside the loop in case of sudden destruction
            if (!IsValid) yield break;
            _textDisplay.text += c;
            yield return new WaitForSeconds(_typingSpeed);
        }

        _isTyping = false;
    }

    private void FinishLineInstantly()
    {
        if (_typingRoutine != null) StopCoroutine(_typingRoutine);

        _textDisplay.text = _currentDialogueLines[_lineIndex];
        _isTyping = false;
    }

    private void NextLine()
    {
        if (_lineIndex < _currentDialogueLines.Count - 1)
        {
            _lineIndex++;
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        _isActive = false;
        // Clean up any running typing
        if (_typingRoutine != null) StopCoroutine(_typingRoutine);

        HideUI();

        // 1. Switch back to Gameplay state
        GameBroadcast.OnInputStateChange?.Invoke("Player");

        // 2. Signal the StoryDirector
        _onDialogueFinishedSignal?.Raise();
    }

    private void ShowUI()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

    private void HideUI()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    // SAFEGUARD: If the scene changes while someone is talking, 
    // we must stop the typing routine and clear the state.
    protected override void OnDisable()
    {
        if (_typingRoutine != null) StopCoroutine(_typingRoutine);
        _isActive = false;
        _isTyping = false;
    }

    // Since this script is called manually by StartDialogue, 
    // we don't need to subscribe to GameBroadcast here.
    protected override void OnEnable() { }
}