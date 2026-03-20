using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class L0_SwitchDirectorObjective : ActObjectiveBase
{
    [Header("Director Handoff")]
    [SerializeField] private StoryDirector _nextDirector;

    [SerializeField] private bool _disableSelf = true;

    private bool _isReached = false;
    public override bool IsReached => _isReached;

    public override void OnObjectiveStarted()
    {
        if (_nextDirector == null)
        {
            Debug.LogError("Handoff Failed! Assign Director!");
            return;
        }

        _isReached = true;
        Debug.Log($"<color=green>SWITCHING DIRECTOR!:</color> Enable {_nextDirector.name}");

        _nextDirector.gameObject.SetActive(true);

        // DEACTIVATE THE CURRENT DIRECTOR TO PREVENT CONFLICT
        if (_disableSelf)
        {
            var allDirectors = Object.FindObjectsByType<StoryDirector>(FindObjectsSortMode.None);
            foreach (var d in allDirectors)
            {
                if (d != _nextDirector)
                {
                    Debug.Log($"<color=red>KẾT THÚC CHƯƠNG CŨ:</color> Đang tắt {d.name}");
                    d.gameObject.SetActive(false);
                }
            }
        }
    }
    public override void Reset() => _isReached = false;
    public override int CurrentProgress => _isReached ? 1 : 0;
    public override int GoalAmount => 1;
    public override List<GameSignal> GetAllRequiredSignals() => new List<GameSignal>();
}