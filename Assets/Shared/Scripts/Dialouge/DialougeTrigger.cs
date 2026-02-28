using UnityEngine;

// This is used to trigger dialogue from the DialogueManager because the StoryDirector can't directly call methods with parameters.
public class DialogueTrigger : MonoBehaviour
{
    [Header("Connections")]
    // 1. Drag your DialogueManager here in the Inspector
    [SerializeField] private DialogueManager _dialogueManager;

    [Header("The Dialogue")]
    // 2. Type your dialogue lines here in the Inspector
    [SerializeField] private DialogueData _data;

    // --- THE MAGIC METHOD ---
    // Because this method has NO parameters, it WILL show up in the image_4 dropdown.
    public void TriggerDialogue()
    {
        if (_dialogueManager != null && _data != null)
        {
            // This script acts as the bridge, passing the data to the manager.
            _dialogueManager.StartDialogue(_data);
        }
    }
}