using UnityEngine;

public class FragmentProgressionCounter : MonoBehaviour
{
    [Header("Stage Settings")]
    [Tooltip("Set to 0 or -1 for stages with no fragments (like Stage 5)")]
    [SerializeField] private int _totalRequired = 3;
    [SerializeField] private string _stageTitle;
    private int _currentCount = 0;

    private void Start()
    {
        UpdateHUD();
        GameBroadcast.OnStageNameUpdateHUD?.Invoke(_stageTitle);

        // If this is a no-fragment stage, we DON'T trigger OnAllFragmentsCollected.
        // The LevelCompleteManager will wait for a manual trigger instead.
    }

    public void AddFragmentProgress()
    {
        // Safety: If no fragments are required, this function shouldn't do anything
        if (_totalRequired <= 0) return;
        if (_currentCount >= _totalRequired) return;

        _currentCount++;
        UpdateHUD();

        if (_currentCount >= _totalRequired)
            GameBroadcast.OnAllFragmentsCollected?.Invoke(true);

        Debug.Log($"Fragment Progress: {_currentCount}/{_totalRequired}");
    }

    private void UpdateHUD()
    {
        // If total required is 0 or less, we send a signal for "?? / ??"
        // We can use -1 as a magic number that the HUD script recognizes
        if (_totalRequired <= 0)
        {
            // Sending -1, -1 tells the HUD to show "?? / ??"
            GameBroadcast.OnProgressionCountUpdateHUD?.Invoke(-1, -1);
            GameBroadcast.OnFragmentCollected?.Invoke(0);
        }
        else
        {
            GameBroadcast.OnProgressionCountUpdateHUD?.Invoke(_currentCount, _totalRequired);
            GameBroadcast.OnFragmentCollected?.Invoke(_currentCount);
        }
    }
}