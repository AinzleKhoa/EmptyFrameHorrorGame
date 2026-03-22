using UnityEngine;

public class L1H_FragmentProgressionCounter : MonoBehaviour
{
    [Header("Stage Settings")]
    [SerializeField] private int _totalRequired = 3;
    [SerializeField] private string _stageTitle = "Once my Home";

    [Header("Signal")]
    [SerializeField] private GameSignal _allFragmentsCollectedSignal;

    private int _currentCount;
    private bool _completed;

    public bool HasAllFragments => _currentCount >= _totalRequired;

    private void Start()
    {
        GameBroadcast.OnStageNameUpdateHUD?.Invoke(_stageTitle);
        GameBroadcast.OnProgressionCountUpdateHUD?.Invoke(_currentCount, _totalRequired);
    }

    public void AddFragment()
    {
        if (_completed) return;

        _currentCount++;

        if (_currentCount > _totalRequired)
            _currentCount = _totalRequired;

        GameBroadcast.OnProgressionCountUpdateHUD?.Invoke(_currentCount, _totalRequired);
        GameBroadcast.OnFragmentCollected?.Invoke(_currentCount);

        Debug.Log($"Photo Fragment Progress: {_currentCount}/{_totalRequired}");

        if (_currentCount >= _totalRequired)
        {
            _completed = true;

            GameBroadcast.OnAllFragmentsCollected?.Invoke(true);

            if (_allFragmentsCollectedSignal != null)
            {
                Debug.Log($"Signal Raised: {_allFragmentsCollectedSignal.name}");
                _allFragmentsCollectedSignal.Raise();
            }
        }
    }
}