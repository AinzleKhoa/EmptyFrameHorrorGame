using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoryDirector : MonoBehaviour
{
    [Header("Level Flow")]
    [SerializeField] private List<Act> _storyActs = new List<Act>();
    [System.Serializable]
    private class Act
    {
        public string ActName;

        [SerializeReference, SerializeReferenceDropdown]
        public List<ActObjectiveBase> Objectives = new List<ActObjectiveBase>();

        public UnityEvent OnActStarted;
        public UnityEvent OnActFinished;

        [HideInInspector] public bool IsActive;

        public bool AllObjectivesComplete()
        {
            foreach (var obj in Objectives) if (!obj.IsReached) return false;
            return true;
        }
    }
    private int _currentActIndex = 0;

    private void OnEnable()
    {
        foreach (var act in _storyActs)
            foreach (var obj in act.Objectives)
                foreach (var sig in obj.GetAllRequiredSignals())
                    sig?.RegisterListener(this);

        if (_storyActs.Count > 0) StartCoroutine(DelayedStart());
    }

    private System.Collections.IEnumerator DelayedStart()
    {
        yield return null; // Wait one frame for UI to initialize
        StartAct(0);
    }

    private void OnDisable()
    {
        foreach (var act in _storyActs)
            foreach (var obj in act.Objectives)
                foreach (var sig in obj.GetAllRequiredSignals())
                    sig?.UnregisterListener(this);
    }

    private void Update()
    {
        if (_currentActIndex >= _storyActs.Count) return;
        Act currentAct = _storyActs[_currentActIndex];
        if (!currentAct.IsActive) return;

        foreach (var obj in currentAct.Objectives)
        {
            obj.OnUpdate(Time.deltaTime);
        }

        if (currentAct.AllObjectivesComplete()) FinishAct();
    }

    // Main entry for all signals
    public void OnEventRaised(GameSignal signal)
    {
        // Save where we are before we start
        int indexBeforeProcessing = _currentActIndex;

        Act currentAct = _storyActs[_currentActIndex];
        if (!currentAct.IsActive) return;

        foreach (var obj in currentAct.Objectives)
        {
            obj.OnSignalRaised(signal);

            // If an objective called JumpToAct(), the index has changed. We stop.
            if (_currentActIndex != indexBeforeProcessing) return;
        }

        UpdateHUD(currentAct);
        // If we are still in the same act and everything is done, move forward linearly.
        if (currentAct.AllObjectivesComplete()) FinishAct();
    }

    // Call externally from special objectives like branching ones to jump to a specific act
    public void JumpToAct(string actName)
    {
        Debug.Log($"Director: Jumping to act -> {actName}");

        // 1. End current act properly
        _storyActs[_currentActIndex].IsActive = false;
        _storyActs[_currentActIndex].OnActFinished.Invoke();

        // 2. Find the index of the act by its name
        int targetIndex = _storyActs.FindIndex(a => a.ActName == actName);

        if (targetIndex != -1)
        {
            _currentActIndex = targetIndex;
            StartAct(_currentActIndex);
        }
    }

    private void StartAct(int index)
    {
        Act act = _storyActs[index];
        act.IsActive = true;

        foreach (var obj in act.Objectives) obj.OnObjectiveStarted();

        UpdateHUD(act);
        act.OnActStarted.Invoke();
    }

    private void UpdateHUD(Act act)
    {
        // find the first objective that isn't done yet and show its text
        foreach (var obj in act.Objectives)
        {
            if (!obj.IsReached)
            {
                GameBroadcast.OnObjectiveUpdateHUD?.Invoke(obj.Description);
                break;
            }
        }
    }

    private void FinishAct()
    {
        _storyActs[_currentActIndex].IsActive = false;
        _storyActs[_currentActIndex].OnActFinished.Invoke();
        _currentActIndex++;
        if (_currentActIndex < _storyActs.Count) StartAct(_currentActIndex);
    }
}