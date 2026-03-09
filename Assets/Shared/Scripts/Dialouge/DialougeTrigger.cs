using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("The Dialogue")]
    [SerializeField] private DialogueData _data;

    private DialougeHUD _dialogueHUD;

    private void Start()
    {
        // 1. Find the Player object
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            // 2. Look through children to find the HUD (since you moved it to the Player)
            _dialogueHUD = player.GetComponentInChildren<DialougeHUD>();
        }

        // Safety check to alert you in the console if something is missing
        if (_dialogueHUD == null)
        {
            Debug.LogError($"<color=red>DialogueTrigger on {gameObject.name} could not find DialougeHUD on the Player!</color>");
        }
    }

    // --- THE MAGIC METHOD ---
    public void TriggerDialogue()
    {
        if (_dialogueHUD != null && _data != null)
        {
            // Pass the data to the HUD on the player
            _dialogueHUD.StartDialogue(_data);
        }
    }
}