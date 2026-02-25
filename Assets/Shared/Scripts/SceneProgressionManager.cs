using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SceneProgressionManager : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent OnAllCollectedLocal;

    [Header("Stage Settings")]
    [SerializeField] private string _stageName = "Default Stage";
    [SerializeField] private int _totalRequired = 3;
    private int _currentCount = 0;

    [Header("Stage Transition")]
    [SerializeField] private string _nextSceneName;

    private void Start()
    {
        GameBroadcast.OnFragmentUpdate?.Invoke(_currentCount, _totalRequired);
        GameBroadcast.OnStageNameUpdate?.Invoke(_stageName);
    }

    public void AddFragmentProgress()
    {
        _currentCount++;
        GameBroadcast.OnFragmentUpdate?.Invoke(_currentCount, _totalRequired);
        if (_currentCount >= _totalRequired) ObjectiveComplete();
    }
    private void ObjectiveComplete()
    {
        // 1. Trigger local logic (e.g., a door opening sound in this level)
        Debug.Log("Objective Complete!");
        OnAllCollectedLocal?.Invoke();

        // 2. Trigger global logic (e.g., update UI, enable next level transition)
        GameBroadcast.OnLevelTransitionStarted?.Invoke(_nextSceneName);
    }
}