using UnityEngine;

public class L5_ShatteredFragmentInteractable : InteractableBase, IHoldable
{
    public override string PromptMessage => _isLocked ? "The Shadow is jamming the memory..." : $"Hold [E]\nFixing Fragment:\n<size=120%>{Mathf.RoundToInt(_currentProgress)}%</size>";

    [Header("Progress Settings")]
    [SerializeField] private float _fillSpeed = 20f;
    private float _currentProgress = 0f;
    private bool _isComplete = false;
    private bool _isLocked = false;

    // The reference to your Special Signal script
    private SpecialSignalTrigger _specialSignal;

    protected override void Awake()
    {
        // Call parent Awake to setup Outlines
        base.Awake();

        // Search for the Special Signal script on this same object
        _specialSignal = GetComponent<SpecialSignalTrigger>();
    }

    private void OnEnable() => L5_GameBroadcast.OnL5ShadowSpawned += SetLockStatus;
    private void OnDisable() => L5_GameBroadcast.OnL5ShadowSpawned -= SetLockStatus;

    private void SetLockStatus(bool shouldLock) => _isLocked = shouldLock;

    public void OnInteractHold(PlayerInteractor player)
    {
        if (_isComplete || _isLocked) return;

        _currentProgress += _fillSpeed * Time.deltaTime;
        _currentProgress = Mathf.Clamp(_currentProgress, 0, 100);

        // Keep the progress bar updated (HUD usually needs this)
        L5_GameBroadcast.OnL5FragmentProgress?.Invoke(_currentProgress);

        if (_currentProgress >= 100f && !_isComplete)
        {
            CompleteFragment();
        }
    }

    private void CompleteFragment()
    {
        _isComplete = true;

        // Visual feedback
        if (_outline != null) _outline.OutlineColor = Color.green;

        // --- THE CHANGE ---
        // Instead of a global broadcast, we trigger the specific Signal Script
        if (_specialSignal != null)
        {
            _specialSignal.RaiseSignal();
        }
        else
        {
            Debug.LogError($"<color=red>{gameObject.name}:</color> Missing SpecialSignalTrigger component!");
        }

        Debug.Log($"<color=yellow>{gameObject.name} Restored!</color> Signal sent.");

        // NEW: Reset and hide the object after a delay so the player can see the success
        ResetAndDeactivate();
    }

    // NEW METHOD: Resets the state and deactivates the object for reuse in later phases
    public void ResetAndDeactivate()
    {
        _currentProgress = 0f;
        _isComplete = false;

        if (_outline != null) _outline.OutlineColor = Color.red;

        // Deactivate the object so your Director can reactivate it for Phase 2 or 3
        gameObject.SetActive(false);

        Debug.Log($"<color=cyan>{gameObject.name}:</color> Reset and hidden for next phase.");
    }
}