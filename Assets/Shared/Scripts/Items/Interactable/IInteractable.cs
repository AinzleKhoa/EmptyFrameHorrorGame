using UnityEngine;

public interface IInteractable
{
    string PromptMessage { get; }
    void Interact(PlayerInteractor interactor);
}