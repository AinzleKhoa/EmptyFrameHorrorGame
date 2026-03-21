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

    private void OnEnable()
    {
        GameBroadcast.OnLighterToggle += SetLighterState;

        // 2. THE FIX: Check the current state immediately upon spawning
        _playerHasLitLighter = Lighter.IsLitGlobal;

        // it must physically force itself to be unlit first.
        _isAlreadyLit = false;
        if (_candleFlame != null) _candleFlame.SetActive(false);
        if (_pointLight != null) _pointLight.enabled = false;

        // Check the Manager's upgrade status immediately
        if (L5_CandleManager.Instance != null && L5_CandleManager.Instance.IsLitFlameUnlocked)
        {
            SetLit(true);
            Debug.Log("<color=yellow>Candle:</color> Auto-ignited via Lit Flame upgrade.");
        }
    }
    private void OnDisable()
    {
        GameBroadcast.OnLighterToggle -= SetLighterState;
        // Safety: ensure it's unlit when it goes away
        _isAlreadyLit = false;
    }

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

    // Add this so the Manager can force the candle to be lit or unlit
    public void SetLit(bool lit)
    {
        _isAlreadyLit = lit;
        if (_candleFlame != null) _candleFlame.SetActive(lit);
        if (_pointLight != null) _pointLight.enabled = lit;
    }
}