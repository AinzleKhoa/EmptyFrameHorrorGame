using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class L0_CountObjective : ActObjectiveBase
{
    [Header("Goal Settings")]
    [SerializeField] private GameSignal _interactionSignal;
    [SerializeField] private int _targetCount = 1; // You can set this to 2 in the Inspector

    private int _currentCount = 0;

    // The logic now uses the target count you set in the Inspector
    public override bool IsReached => _currentCount >= _targetCount;

    public override void OnSignalRaised(GameSignal signal)
    {
        if (signal == _interactionSignal)
        {
            _currentCount++;
            Debug.Log($"{_objectiveName} progress: {_currentCount}/{_targetCount}");
        }
    }

    public override void Reset() => _currentCount = 0;

    public override List<GameSignal> GetAllRequiredSignals()
    {
        List<GameSignal> signals = new List<GameSignal>();
        // If we only have one signal, simplified it
        if (_interactionSignal != null) signals.Add(_interactionSignal);
        return signals;
    }

    // If your StoryDirector still asks for these, implement them locally
    public override int CurrentProgress => _currentCount;
    public override int GoalAmount => _targetCount;
}