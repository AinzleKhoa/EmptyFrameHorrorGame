using UnityEngine;

public class L1H_CountdownTimer : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float _timeLimitSeconds = 120f;

    [Header("References")]
    [SerializeField] private L1H_CleanupObjectiveManager _objective;

    private float _remaining;
    private bool _running = true;

    private void Start()
    {
        _remaining = _timeLimitSeconds;
        L1H_CleanupEvents.OnTimeChanged?.Invoke(_remaining);
    }

    private void OnEnable()
    {
        L1H_CleanupEvents.OnWin += StopTimer;
        L1H_CleanupEvents.OnLose += StopTimer;
    }

    private void OnDisable()
    {
        L1H_CleanupEvents.OnWin -= StopTimer;
        L1H_CleanupEvents.OnLose -= StopTimer;
    }

    private void Update()
    {
        if (!_running) return;

        _remaining -= Time.deltaTime;
        if (_remaining < 0f) _remaining = 0f;

        L1H_CleanupEvents.OnTimeChanged?.Invoke(_remaining);

        if (_remaining <= 0f)
        {
            _running = false;
            if (_objective != null) _objective.Lose();
            else FindFirstObjectByType<L1H_CleanupObjectiveManager>()?.Lose();
        }
    }

    private void StopTimer() => _running = false;
}