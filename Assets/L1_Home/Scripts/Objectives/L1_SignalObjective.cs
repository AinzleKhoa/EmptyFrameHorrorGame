using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class L1_SignalObjective : ActObjectiveBase
{
    [Header("Signal Settings")]
    [SerializeField] private GameSignal _targetSignal;

    private bool _isReached;

    public override bool IsReached => _isReached;
    public override int CurrentProgress => _isReached ? 1 : 0;
    public override int GoalAmount => 1;

    public override void OnObjectiveStarted()
    {
        Reset();
    }

    public override void OnSignalRaised(GameSignal signal)
    {
        if (_isReached) return;

        if (signal == _targetSignal)
        {
            _isReached = true;
            Debug.Log($"{_objectiveName} completed");
        }
    }

    public override void Reset()
    {
        _isReached = false;
    }

    public override List<GameSignal> GetAllRequiredSignals()
    {
        List<GameSignal> signals = new List<GameSignal>();
        if (_targetSignal != null) signals.Add(_targetSignal);
        return signals;
    }
}