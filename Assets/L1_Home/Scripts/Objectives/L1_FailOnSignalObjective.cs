using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class L1_FailOnSignalObjective : ActObjectiveBase
{
    [Header("Fail Settings")]
    [SerializeField] private GameSignal _failSignal;
    [SerializeField] private string _targetLoseActName = "LoseAct";

    private bool _isTriggered;

    public override bool IsReached => _isTriggered;
    public override int CurrentProgress => _isTriggered ? 1 : 0;
    public override int GoalAmount => 1;

    public override void OnObjectiveStarted()
    {
        Reset();
    }

    public override void OnSignalRaised(GameSignal signal)
    {
        if (_isTriggered) return;

        if (signal == _failSignal)
        {
            _isTriggered = true;

            StoryDirector director = Object.FindFirstObjectByType<StoryDirector>();
            if (director != null)
            {
                Debug.Log($"[L1_FailOnSignalObjective] Jump to act: {_targetLoseActName}");
                director.JumpToAct(_targetLoseActName);
            }
            else
            {
                Debug.LogWarning("[L1_FailOnSignalObjective] No StoryDirector found.");
            }
        }
    }

    public override void Reset()
    {
        _isTriggered = false;
    }

    public override List<GameSignal> GetAllRequiredSignals()
    {
        List<GameSignal> signals = new List<GameSignal>();
        if (_failSignal != null) signals.Add(_failSignal);
        return signals;
    }
}