using UnityEngine;

public class L1H_CountdownSignalTimer : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float _timeLimitSeconds = 120f;
    [SerializeField] private bool _startOnStart = true;

    [Header("Signal")]
    [SerializeField] private GameSignal _timeUpSignal;

    private float _remainingTime;
    private bool _isRunning;
    private bool _hasRaisedTimeUp;

    public float RemainingTime => _remainingTime;
    public bool IsRunning => _isRunning;

    private void Start()
    {
        ResetTimer();

        if (_startOnStart)
            StartTimer();
    }

    private void Update()
    {
        if (!_isRunning || _hasRaisedTimeUp) return;

        _remainingTime -= Time.deltaTime;

        if (_remainingTime <= 0f)
        {
            _remainingTime = 0f;
            _isRunning = false;
            _hasRaisedTimeUp = true;

            Debug.Log("[L1 Timer] Time Up");

            if (_timeUpSignal != null)
                _timeUpSignal.Raise();
            else
                Debug.LogWarning("[L1 Timer] No time-up signal assigned.");
        }
    }

    public void StartTimer()
    {
        _isRunning = true;
    }

    public void StopTimer()
    {
        _isRunning = false;
    }

    public void ResetTimer()
    {
        _remainingTime = _timeLimitSeconds;
        _isRunning = false;
        _hasRaisedTimeUp = false;
    }
}