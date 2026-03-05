using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [Header("UI Message")]
    [SerializeField] private string _promptMessage = "Press [E] to Talk";
    public string PromptMessage => _promptMessage;

    public void Interact(PlayerInteractor interactor)
    {
        // Always raise signal, it will be null if no signal trigger existed.
        if (TryGetComponent<SignalTrigger>(out var signal))
        {
            signal.RaiseSignal();
        }
    }

    public void setPromptMessage(string newMessage)
    {
        _promptMessage = newMessage;
    }
}