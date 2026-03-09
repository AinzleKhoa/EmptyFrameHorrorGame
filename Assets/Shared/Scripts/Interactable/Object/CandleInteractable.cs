using UnityEngine;

public class CandleInteractable : InteractableBase
{
    [Header("Visuals")]
    [SerializeField] private GameObject _candleFlame;
    [Header("Point Light")]
    [SerializeField] private Light _pointLight;

    private bool _playerHasLitLighter = false;
    private bool _isAlreadyLit = false;

    // The Interactor reads this every frame
    public override string PromptMessage
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

    public override void Interact(PlayerInteractor interactor)
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
            }
            // Optional: Trigger a local event for the level designer
            Debug.Log("Candle lit! Surrounding area illuminated.");

            // Call the Base logic
            base.Interact(interactor);
        }
    }

    public bool IsLit => _isAlreadyLit;
}