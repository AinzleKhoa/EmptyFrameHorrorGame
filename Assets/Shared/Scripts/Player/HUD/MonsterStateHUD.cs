using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MonsterStateHUD : BaseHUD // Inherit from BaseHUD
{
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private TextMeshProUGUI _displayText;

    private List<string> _activeTraits = new List<string>();

    private void Awake()
    {
        // Safety check for initialization
        if (_group != null) _group.alpha = 0f;
        if (_displayText != null) _displayText.text = "";
    }

    // Standard BaseHUD overrides 
    // Even if not using GameBroadcast here, we must implement them.
    protected override void OnEnable() { }
    protected override void OnDisable()
    {
        _activeTraits.Clear(); // Clear traits on disable for scene restarts
    }

    // This is called by the Monster
    public void UpdateThreatDisplay(int count, string newTrait)
    {
        // 1. Universal Safeguard: Is the HUD alive?
        if (!IsValid) return;

        // 2. Component Safeguard
        if (_group == null || _displayText == null) return;

        // 3. Logic check: If the monster sends empty text, hide the HUD
        if (string.IsNullOrEmpty(newTrait))
        {
            _group.alpha = 0f;
            return;
        }

        // 4. Add to our active list if we don't have it yet
        if (!_activeTraits.Contains(newTrait))
        {
            _activeTraits.Add(newTrait);
        }

        // 5. Render string construction
        string header = $"<color=#FF4500><b>MONSTER THREAT: {count}/5</b></color>";
        string traitList = "";

        foreach (var t in _activeTraits)
        {
            traitList += $"\n• {t}";
        }

        // 6. Final safety check before setting text
        if (_displayText != null)
        {
            _displayText.text = $"{header}{traitList}";
            _group.alpha = 1f;
        }
    }
}