using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [Header("UI Message")]
    [SerializeField] private string _promptMessage = "Press [E] to Talk";
    public string PromptMessage => _promptMessage;

    public void Interact(PlayerInteractor interactor)
    {
        // This will only trigger the SignalTrigger for StoryDirector for now.
    }

    public void setPromptMessage(string newMessage)
    {
        _promptMessage = newMessage;
    }
}