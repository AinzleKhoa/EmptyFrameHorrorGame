using UnityEngine;

public class ReadableItem : MonoBehaviour, IInteractable
{
    [Header("UI Message")]
    [SerializeField] private string _promptMessage = "Press [E] to Read";
    public string PromptMessage => _promptMessage;

    [Header("Content")]
    [TextArea]
    [SerializeField] private string _content;

    public void Interact(PlayerInteractor interactor)
    {
        GameBroadcast.OnShowReadableContent?.Invoke(_content, true);
        GameBroadcast.isPlayerFreezed?.Invoke(true); // Freeze player while reading
        // Always raise signal, it will be null if no signal trigger existed.
        if (TryGetComponent<SignalTrigger>(out var signal))
        {
            signal.RaiseSignal();
        }
    }
}