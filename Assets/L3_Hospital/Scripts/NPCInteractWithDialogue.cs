using UnityEngine;

public class NPCInteractWithDialogue : NPCInteractable
{
    [Header("Dialogue Trigger Source")]
    [SerializeField] private DialogueTrigger _dialogueTrigger;

    protected virtual void Start()
    {
        // Nếu chưa kéo tay trong Inspector thì tự tìm
        if (_dialogueTrigger == null)
        {
            // Tìm trên chính object này trước
            _dialogueTrigger = GetComponent<DialogueTrigger>();

            // Nếu chưa có thì tìm trong children
            if (_dialogueTrigger == null)
            {
                _dialogueTrigger = GetComponentInChildren<DialogueTrigger>();
            }
        }

        if (_dialogueTrigger == null)
        {
            Debug.LogWarning(
                $"<color=yellow>{nameof(NPCInteractWithDialogue)} on {gameObject.name} could not find DialogueTrigger.</color>"
            );
        }
    }

    public override void Interact(PlayerInteractor interactor)
    {
        // Chạy logic interact gốc trước
        base.Interact(interactor);

        // Sau đó gọi dialogue
        if (_dialogueTrigger != null)
        {
            _dialogueTrigger.TriggerDialogue();
        }
        else
        {
            Debug.LogWarning(
                $"<color=yellow>{nameof(NPCInteractWithDialogue)} on {gameObject.name} has no DialogueTrigger assigned or found.</color>"
            );
        }
    }
}