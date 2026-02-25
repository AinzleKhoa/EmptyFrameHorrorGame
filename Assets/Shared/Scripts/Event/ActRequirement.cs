using UnityEngine;

[System.Serializable]
public abstract class ActRequirement
{
    public abstract bool IsSatisfied { get; }
    public abstract void Evaluate(GameSignal signal);
    public abstract void Reset();
}

// A specific requirement for counting signals (e.g., "Find 2 Memories")
[System.Serializable]
public class SignalCountRequirement : ActRequirement
{
    public GameSignal TargetSignal;
    public int GoalCount = 1;
    private int _currentCount = 0;

    public override bool IsSatisfied => _currentCount >= GoalCount;

    public override void Evaluate(GameSignal signal)
    {
        if (signal == TargetSignal)
        {
            _currentCount++;
            Debug.Log($"Requirement Progress: {_currentCount}/{GoalCount}");
        }
    }

    public override void Reset() => _currentCount = 0;
}