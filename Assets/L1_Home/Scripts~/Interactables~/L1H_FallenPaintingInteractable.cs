using UnityEngine;

public class L1H_FallenPaintingInteractable : MonoBehaviour, IInteractable
{
    [Header("Prompt")]
    [SerializeField] private string _promptMessage = "Press [E] to Inspect";
    [SerializeField] private string _notReadyPrompt = "";
    [SerializeField] private string _donePrompt = "";

    [Header("Requirement")]
    [SerializeField] private L1H_FragmentProgressionCounter _fragmentCounter;
    [SerializeField] private Rigidbody _paintingRb;

    [Header("Photo Visual")]
    [SerializeField] private GameObject _hiddenPhotoObject;

    [Header("Signal")]
    [SerializeField] private GameSignal _photoPlacedSignal;

    [Header("Optional Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _inspectSfx;

    private bool _revealed;

    public string PromptMessage
    {
        get
        {
            if (_revealed) return _donePrompt;
            if (!CanInspectYet()) return _notReadyPrompt;
            return _promptMessage;
        }
    }

    private void Start()
    {
        if (_hiddenPhotoObject != null)
            _hiddenPhotoObject.SetActive(false);
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (_revealed) return;
        if (!CanInspectYet()) return;

        _revealed = true;

        if (_hiddenPhotoObject != null)
            _hiddenPhotoObject.SetActive(true);

        if (_audioSource != null && _inspectSfx != null)
            _audioSource.PlayOneShot(_inspectSfx);

        if (_photoPlacedSignal != null)
        {
            Debug.Log($"Signal Raised: {_photoPlacedSignal.name}");
            _photoPlacedSignal.Raise();
        }

        var levelManager = FindFirstObjectByType<LevelCompleteManager>();
        if (levelManager != null)
        {
            levelManager.CompleteLevel();
        }
        else
        {
            Debug.LogWarning("LevelCompleteManager not found.");
        }
    }

    private bool CanInspectYet()
    {
        bool hasFragments = _fragmentCounter != null && _fragmentCounter.HasAllFragments;
        bool paintingHasFallen = _paintingRb != null && !_paintingRb.isKinematic;

        return hasFragments && paintingHasFallen;
    }
}