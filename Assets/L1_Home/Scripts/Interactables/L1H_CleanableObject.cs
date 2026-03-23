using UnityEngine;

public class L1H_CleanableObject : MonoBehaviour, IInteractable
{
    [Header("Prompt")]
    [SerializeField] private string _prompt = "Press 'E' to Clean";

    [Header("Visual")]
    [SerializeField] private GameObject _disableTarget;

    [Header("State")]
    [SerializeField] private bool _canInteractAtStart = true;

    private bool _isCleaned;
    private bool _canInteract;
    private SignalTrigger _signalTrigger;

    public string PromptMessage
    {
        get
        {
            if (!_canInteract || _isCleaned) return "";
            if (!L1H_InteractRules.CanInteractWithGeneralObject()) return "";
            return _prompt;
        }
    }

    private void Awake()
    {
        _signalTrigger = GetComponent<SignalTrigger>();
        _canInteract = _canInteractAtStart;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!_canInteract) return;
        if (_isCleaned) return;
        if (!L1H_InteractRules.CanInteractWithGeneralObject()) return;

        _isCleaned = true;

        if (_disableTarget != null)
            _disableTarget.SetActive(false);
        else
            gameObject.SetActive(false);

        if (_signalTrigger != null)
            _signalTrigger.RaiseSignal();
        else
            Debug.LogWarning($"[L1] {name} has no SignalTrigger attached.");
    }

    public void EnableInteract() => _canInteract = true;
    public void DisableInteract() => _canInteract = false;
    public void ResetCleanable() => _isCleaned = false;
}