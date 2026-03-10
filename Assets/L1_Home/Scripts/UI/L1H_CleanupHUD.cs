using TMPro;
using UnityEngine;

public class L1H_CleanupHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text _objectiveText;
    [SerializeField] private TMP_Text _timeText;

    [Header("Optional")]
    [SerializeField] private L1H_CountdownSignalTimer _timer;

    private void OnEnable()
    {
        GameBroadcast.OnObjectiveUpdateHUD += UpdateObjective;
    }

    private void OnDisable()
    {
        GameBroadcast.OnObjectiveUpdateHUD -= UpdateObjective;
    }

    private void Update()
    {
        if (_timeText != null && _timer != null)
        {
            _timeText.text = $"Time: {Mathf.CeilToInt(_timer.RemainingTime)}s";
        }
    }

    private void UpdateObjective(string text)
    {
        if (_objectiveText != null)
        {
            _objectiveText.text = text;
        }
    }
}