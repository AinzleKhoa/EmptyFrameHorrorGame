using TMPro;
using UnityEngine;

public class L1H_CleanupHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _progressText;

    private void OnEnable()
    {
        L1H_CleanupEvents.OnTimeChanged += UpdateTime;
        L1H_CleanupEvents.OnProgressChanged += UpdateProgress;
    }

    private void OnDisable()
    {
        L1H_CleanupEvents.OnTimeChanged -= UpdateTime;
        L1H_CleanupEvents.OnProgressChanged -= UpdateProgress;
    }

    private void UpdateTime(float remaining)
    {
        if (_timeText == null) return;
        _timeText.text = $"Time: {Mathf.CeilToInt(remaining)}s";
    }

    private void UpdateProgress(int total, int cleaned)
    {
        if (_progressText == null) return;
        _progressText.text = $"Cleaned: {cleaned}/{total}";
    }
}