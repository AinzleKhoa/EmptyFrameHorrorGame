using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MonsterStateHUD : MonoBehaviour
{
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private TextMeshProUGUI _displayText;

    private List<string> _activeTraits = new List<string>();

    private void Awake()
    {
        _group.alpha = 0f;
        _displayText.text = "";
    }

    // This is called by the Monster
    public void UpdateThreatDisplay(int count, string newTrait)
    {
        // 1. Safety check: If the monster sends empty text, hide the HUD
        if (string.IsNullOrEmpty(newTrait))
        {
            _group.alpha = 0f;
            return;
        }

        // 2. Add to our active list if we don't have it yet
        if (!_activeTraits.Contains(newTrait))
        {
            _activeTraits.Add(newTrait);
        }

        // 3. Render
        string header = $"<color=#FF4500><b>MONSTER THREAT: {count}/5</b></color>";
        string traitList = "";

        foreach (var t in _activeTraits)
        {
            traitList += $"\n• {t}";
        }

        _displayText.text = $"{header}{traitList}";
        _group.alpha = 1f;
    }
}