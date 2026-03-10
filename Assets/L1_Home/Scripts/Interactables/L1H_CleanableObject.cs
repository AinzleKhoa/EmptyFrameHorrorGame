using UnityEngine;

public class L1H_CleanableObject : MonoBehaviour, IInteractable
{
    [Header("Prompt")]
    [SerializeField] private string _prompt = "Press 'E' to Clean";

    [Header("Visual")]
    [SerializeField] private GameObject _disableTarget;

    private bool _isCleaned;
    private SignalTrigger _signalTrigger;

    public string PromptMessage => _isCleaned ? "" : _prompt;

    private void Awake()
    {
        _signalTrigger = GetComponent<SignalTrigger>();
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (_isCleaned) return;

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
}