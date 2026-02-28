using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _textDisplay;
    [SerializeField] private GameObject _continuePrompt;

    [Header("Settings")]
    [SerializeField] private float _typingSpeed = 0.05f;
    [SerializeField] private GameSignal _onDialogueFinishedSignal;

    private List<string> _currentDialogueLines;
    private int _lineIndex;
    private bool _isTyping;
    private bool _isActive;

    private void Start()
    {
        _dialoguePanel.SetActive(false);
    }

    // Called by StoryDirector via UnityEvent
    public void StartDialogue(DialogueData data)
    {
        if (data == null || data.Lines.Count == 0) return;

        _isActive = true;
        _currentDialogueLines = data.Lines;
        _lineIndex = 0;

        // 1. Block Player
        GameBroadcast.isPlayerFreezed?.Invoke(true); // Broadcast to freeze player interactions

        // 2. Show UI
        _dialoguePanel.SetActive(true);
        StartCoroutine(TypeLine());
    }

    private void Update()
    {
        if (!_isActive) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (_isTyping)
            {
                // Instant skip typing
                StopAllCoroutines();
                _textDisplay.text = _currentDialogueLines[_lineIndex];
                _isTyping = false;
                _continuePrompt.SetActive(true);
            }
            else
            {
                NextLine();
            }
        }
    }

    private IEnumerator TypeLine()
    {
        _isTyping = true;
        _continuePrompt.SetActive(false);
        _textDisplay.text = "";

        foreach (char c in _currentDialogueLines[_lineIndex].ToCharArray())
        {
            _textDisplay.text += c;
            yield return new WaitForSeconds(_typingSpeed);
        }

        _isTyping = false;
        _continuePrompt.SetActive(true);
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
        _dialoguePanel.SetActive(false);

        // 1. Unblock Player
        GameBroadcast.isPlayerFreezed?.Invoke(false); // Broadcast to unfreeze player interactions

        // 2. Tell the StoryDirector we are done
        _onDialogueFinishedSignal?.Raise();
    }
}