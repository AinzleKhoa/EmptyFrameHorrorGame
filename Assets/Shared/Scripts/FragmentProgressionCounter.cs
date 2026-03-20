using UnityEngine;

public class FragmentProgressionCounter : MonoBehaviour
{
    [Header("Stage Settings")]
    [SerializeField] private int _totalRequired = 3;
    [SerializeField] private string _stageTitle;
    private int _currentCount = 0;

    private void Start()
    {
        GameBroadcast.OnProgressionCountUpdateHUD?.Invoke(_currentCount, _totalRequired);
        GameBroadcast.OnFragmentCollected?.Invoke(_currentCount);
        GameBroadcast.OnStageNameUpdateHUD?.Invoke(_stageTitle);
    }

    public void AddFragmentProgress()
    {
        if (_currentCount >= _totalRequired) return;

        _currentCount++;
        GameBroadcast.OnProgressionCountUpdateHUD?.Invoke(_currentCount, _totalRequired);
        GameBroadcast.OnFragmentCollected?.Invoke(_currentCount);

        if (_currentCount >= _totalRequired) GameBroadcast.OnAllFragmentsCollected?.Invoke(true);
        Debug.Log($"Fragment Progress: {_currentCount}/{_totalRequired}");
    }
}