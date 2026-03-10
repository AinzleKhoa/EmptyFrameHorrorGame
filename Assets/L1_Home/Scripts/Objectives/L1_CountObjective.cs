using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class L1_CountObjective : ActObjectiveBase
{
    [Header("Goal Settings")]
    [SerializeField] private GameSignal _countSignal;
    [SerializeField] private int _goalAmount = 1;

    private int _currentProgress = 0;

    public override bool IsReached => _currentProgress >= _goalAmount;
    public override int CurrentProgress => _currentProgress;
    public override int GoalAmount => _goalAmount;

    public override void OnObjectiveStarted()
    {
        Reset();
    }

    public override void OnSignalRaised(GameSignal signal)
    {
        if (IsReached) return;

        if (signal == _countSignal)
        {
            _currentProgress++;
            Debug.Log($"{_objectiveName} progress: {_currentProgress}/{_goalAmount}");
        }
    }

    public override void Reset()
    {
        _currentProgress = 0;
    }

    public override List<GameSignal> GetAllRequiredSignals()
    {
        List<GameSignal> signals = new List<GameSignal>();
        if (_countSignal != null) signals.Add(_countSignal);
        return signals;
    }
}