using UnityEngine;

public class ReadableItem : InteractableBase
{
    [Header("UI Message")]
    [SerializeField] private string _promptMessage = "Press [E] to Read";
    public override string PromptMessage => _promptMessage;

    [Header("Content")]
    [TextArea]
    [SerializeField] private string _content;

    public override void Interact(PlayerInteractor interactor)
    {
        GameBroadcast.OnShowReadableContent?.Invoke(_content, true);
        // Call the Base logic
        base.Interact(interactor);
    }
}