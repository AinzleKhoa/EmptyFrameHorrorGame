using TMPro;
using UnityEngine;

public class L1H_CleanupHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text _cleanProgressText;
    [SerializeField] private TMP_Text _timeText;

    private void OnEnable()
    {
        L1H_CleanupHUDSignals.OnCleanProgressChanged += UpdateCleanProgress;
        L1H_CleanupHUDSignals.OnTimeChanged += UpdateTime;
    }

    private void OnDisable()
    {
        L1H_CleanupHUDSignals.OnCleanProgressChanged -= UpdateCleanProgress;
        L1H_CleanupHUDSignals.OnTimeChanged -= UpdateTime;
    }

    private void UpdateCleanProgress(int current, int goal)
    {
        if (_cleanProgressText == null) return;
        _cleanProgressText.text = $"Cleaned: {current}/{goal}";
    }

    private void UpdateTime(float remaining)
    {
        if (_timeText == null) return;
        _timeText.text = $"Time: {Mathf.CeilToInt(remaining)}s";
    }
}