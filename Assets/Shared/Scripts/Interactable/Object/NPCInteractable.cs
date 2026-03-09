using UnityEngine;

public class NPCInteractable : InteractableBase
{
    [Header("UI Message")]
    [SerializeField] private string _promptMessage = "Press [E] to Talk";
    public override string PromptMessage => _promptMessage;

    public override void Interact(PlayerInteractor interactor)
    {
        // Call the Base logic
        base.Interact(interactor);
    }

    public void setPromptMessage(string newMessage)
    {
        _promptMessage = newMessage;
    }
}