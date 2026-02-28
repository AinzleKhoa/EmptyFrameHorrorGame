using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class L0_MultiBranchObjective : ActObjectiveBase
{
    [Header("Branching Configuration")]
    [Tooltip("The signal received from the world (e.g., Sig_Bench)")]
    [SerializeField] private List<GameSignal> _triggerSignals = new List<GameSignal>();

    [Tooltip("The name of the Act to jump to (MUST match exactly)")]
    [SerializeField] private List<string> _targetActNames = new List<string>();

    private bool _isReached = false;
    public override bool IsReached => _isReached;

    public override void OnSignalRaised(GameSignal signal)
    {
        if (_isReached) return;

        // Loop through the signals list
        for (int i = 0; i < _triggerSignals.Count; i++)
        {
            if (signal == _triggerSignals[i])
            {
                _isReached = true;

                // Safety check: Make sure we have a matching act name for this index
                if (i < _targetActNames.Count)
                {
                    string target = _targetActNames[i];
                    Object.FindFirstObjectByType<StoryDirector>().JumpToAct(target);
                    Debug.Log($"MultiBranch: Signal {signal.name} matched index {i}. Jumping to {target}");
                }

                break;
            }
        }
    }

    public override List<GameSignal> GetAllRequiredSignals()
    {
        // Return the simple list of signals
        return _triggerSignals ?? new List<GameSignal>();
    }

    // --- Housekeeping ---
    public override void OnObjectiveStarted() => Reset();
    public override void Reset() => _isReached = false;
    public override void OnUpdate(float deltaTime) { }
    public override int CurrentProgress => _isReached ? 1 : 0;
    public override int GoalAmount => 1;
}