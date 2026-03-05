using UnityEngine;

public class CandleInteractable : MonoBehaviour, IInteractable
{
    [Header("Visuals")]
    [SerializeField] private GameObject _candleFlame;
    [Header("Point Light")]
    [SerializeField] private Light _pointLight;
    [SerializeField] private float _pointIntensity = 4f;
    [SerializeField] private float _pointRange = 10f;
    [SerializeField] private Color _pointColor = Color.yellow;

    private bool _playerHasLitLighter = false;
    private bool _isAlreadyLit = false;

    // The Interactor reads this every frame
    public string PromptMessage
    {
        get
        {
            if (_isAlreadyLit) return "";
            return _playerHasLitLighter ? "Press 'E' to Light Candle" : "You need a lit lighter";
        }
    }

    private void OnEnable() => GameBroadcast.OnLighterToggle += SetLighterState;
    private void OnDisable() => GameBroadcast.OnLighterToggle -= SetLighterState;

    // Listening for interaction from the player
    private void SetLighterState(bool isLit) => _playerHasLitLighter = isLit;

    public void Interact(PlayerInteractor interactor)
    {
        if (_playerHasLitLighter && !_isAlreadyLit)
        {
            _isAlreadyLit = true;
            if (_candleFlame != null)
            {
                _candleFlame.SetActive(true);
            }
            if (_pointLight != null)
            {
                _pointLight.enabled = true;
                _pointLight.intensity = _pointIntensity;
                _pointLight.range = _pointRange;
                _pointLight.color = _pointColor;
            }
            // Optional: Trigger a local event for the level designer
            Debug.Log("Candle lit! Surrounding area illuminated.");

            // Always raise signal, it will be null if no signal trigger existed.
            if (TryGetComponent<SignalTrigger>(out var signal))
            {
                signal.RaiseSignal();
            }
        }
    }
}