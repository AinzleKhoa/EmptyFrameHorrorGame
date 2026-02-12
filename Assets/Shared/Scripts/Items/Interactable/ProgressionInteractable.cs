using UnityEngine;

public class ProgressionInteractable : MonoBehaviour, IInteractable
{
    [Header("UI Message")]
    [SerializeField] private string _promptMessage = "Press 'E' to Interact";
    public string PromptMessage => _promptMessage;

    [Header("Reference")]
    private SceneProgressionManager _manager;

    private void Start()
    {
        // Automatically find the manager in the scene when the game starts
        _manager = Object.FindFirstObjectByType<SceneProgressionManager>();

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
        }
    }
}