using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoryDirector : MonoBehaviour
{
    [System.Serializable]
    private class Act
    {
        public string ActName;
        // Now using a list of specific requirements
        public List<SignalCountRequirement> Requirements = new List<SignalCountRequirement>();
        public UnityEvent OnActStarted;  // Logic for when the act begins
        public UnityEvent OnActFinished; // Logic for when the act ends

        [HideInInspector] public bool IsActive;
        public bool AllRequirementsMet()
        {
            foreach (var req in Requirements)
            {
                if (!req.IsSatisfied) return false;
            }
            return true;
        }
    }

    [Header("Level Flow")]
    [SerializeField] private List<Act> _storyActs = new List<Act>();
    private int _currentActIndex = 0;

    private void OnEnable()
    {
        // Register the Director to every signal used in any requirement
        foreach (var act in _storyActs)
            foreach (var req in act.Requirements)
                req.TargetSignal?.RegisterListener(this);

        if (_storyActs.Count > 0) StartAct(0);
    }

    private void OnDisable()
    {
        foreach (var act in _storyActs)
            foreach (var req in act.Requirements)
                req.TargetSignal?.UnregisterListener(this);
    }

    // This is the core "Switchboard" logic
    public void OnEventRaised(GameSignal signal)
    {
        Act currentAct = _storyActs[_currentActIndex];
        if (!currentAct.IsActive) return;

        // Pass the signal to all requirements in the current act
        foreach (var req in currentAct.Requirements)
        {
            req.Evaluate(signal);
        }

        // Check if the Act can finish
        if (currentAct.AllRequirementsMet())
        {
            FinishAct();
        }
    }

    private void StartAct(int index)
    {
        if (index >= _storyActs.Count) return;
        _storyActs[index].IsActive = true;
        _storyActs[index].OnActStarted.Invoke();
    }

    private void FinishAct()
    {
        _storyActs[_currentActIndex].IsActive = false;
        _storyActs[_currentActIndex].OnActFinished.Invoke();

        _currentActIndex++;
        if (_currentActIndex < _storyActs.Count) StartAct(_currentActIndex);
    }
}