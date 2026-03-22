using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class L2_Objective_WaitSignal : ActObjectiveBase
{
    [SerializeField] private GameSignal _requiredSignal;
    private bool _isReached = false;

    public override bool IsReached => _isReached;
    public override int CurrentProgress => _isReached ? 1 : 0;
    public override int GoalAmount => 1;

    public override void Reset() => _isReached = false;

    // Lắng nghe tín hiệu phát ra từ GameBroadcast/SignalTrigger
    public override void OnSignalRaised(GameSignal signal)
    {
        if (signal == _requiredSignal && !_isReached)
        {
            _isReached = true;
            Debug.Log($"[L2 Story] Đã hoàn thành nhiệm vụ: {_objectiveName}");
        }
    }

    // Báo cho Director biết đang chờ tín hiệu nào
    public override List<GameSignal> GetAllRequiredSignals()
    {
        return new List<GameSignal>() { _requiredSignal };
    }
}