using UnityEngine;

public class FragmentInteractable : MonoBehaviour, IInteractable
{
    [Header("UI Message")]
    [SerializeField] private string _promptMessage = "Press [E] to Collect Fragment";
    public string PromptMessage => _promptMessage;

    [Header("Reference")]
    private FragmentProgressionCounter _manager;

    private void Start()
    {
        // Automatically find the manager in the scene when the game starts
        _manager = FindFirstObjectByType<FragmentProgressionCounter>();

        if (_manager == null)
        {
            Debug.LogError($"Progression Manager missing from the scene! {gameObject.name} won't work.");
        }
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (_manager != null)
        {
            _manager.AddFragmentProgress();

            Destroy(gameObject);

            // Always raise signal, it will be null if no signal trigger existed.
            if (TryGetComponent<SignalTrigger>(out var signal))
            {
                signal.RaiseSignal();
            }
        }
    }
}