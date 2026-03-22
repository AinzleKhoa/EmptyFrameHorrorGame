using UnityEngine;

public class PlayerHUDManager : BaseHUD
{
    [Header("HUD Groups")]
    [SerializeField] private GameObject _normalStateGroup;
    [SerializeField] private GameObject _cameraStateGroup;
    [SerializeField] private GameObject _pauseStateGroup;
    [SerializeField] private GameObject _gameOverStateGroup;

    private bool _isCurrentlyOnCamera = false;

    protected override void OnEnable()
    {
        GameBroadcast.OnHUDStateChanged += HandleHUDStateChange;
        GameBroadcast.OnCameraToggle += (isOn) => { _isCurrentlyOnCamera = isOn; UpdateGameplayLayer(); };
    }

    protected override void OnDisable()
    {
        GameBroadcast.OnHUDStateChanged -= HandleHUDStateChange;
    }

    private void HandleHUDStateChange(HUDState newState)
    {
        if (!IsValid) return;

        // 1. Hide EVERYTHING first (Reset to clean slate)
        _normalStateGroup.SetActive(false);
        _cameraStateGroup.SetActive(false);
        _pauseStateGroup.SetActive(false);
        _gameOverStateGroup.SetActive(false);

        // 2. Turn on only what we need
        switch (newState)
        {
            case HUDState.Gameplay:
                UpdateGameplayLayer(); // Logic to choose between Normal or Camera
                break;

            case HUDState.Pause:
                _pauseStateGroup.SetActive(true);
                break;

            case HUDState.GameOver:
                _gameOverStateGroup.SetActive(true);
                break;
        }
    }

    private void UpdateGameplayLayer()
    {
        // CRITICAL: If the scene is changing or this object is destroyed, STOP.
        if (!IsValid) return;

        if (_normalStateGroup != null)
            _normalStateGroup.SetActive(!_isCurrentlyOnCamera);

        if (_cameraStateGroup != null)
            _cameraStateGroup.SetActive(_isCurrentlyOnCamera);
    }
}