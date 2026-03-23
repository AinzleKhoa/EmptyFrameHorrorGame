using UnityEngine;

public class L1H_SinkInteractable : MonoBehaviour, IInteractable
{
    [Header("Prompt")]
    [SerializeField] private string _prompt = "Press 'E' to Check the Sink";

    [Header("Optional Effects")]
    [SerializeField] private ParticleSystem _waterEffect;
    [SerializeField] private AudioSource _dripAudio;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _interactSfx;

    private bool _used;
    private bool _canInteract;
    private SignalTrigger _signalTrigger;

    public string PromptMessage => (!_canInteract || _used) ? "" : _prompt;

    private void Awake()
    {
        _signalTrigger = GetComponent<SignalTrigger>();
        _canInteract = false;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!_canInteract) return;
        if (_used) return;

        _used = true;

        if (_waterEffect != null)
            _waterEffect.Stop();

        if (_dripAudio != null)
            _dripAudio.Stop();

        if (_audioSource != null && _interactSfx != null)
            _audioSource.PlayOneShot(_interactSfx);

        if (_signalTrigger != null)
            _signalTrigger.RaiseSignal();
        else
            Debug.LogWarning($"[L1] {name} has no SignalTrigger attached.");
    }

    public void EnableInteract()
    {
        _canInteract = true;
    }

    public void DisableInteract()
    {
        _canInteract = false;
    }

    public void ResetInteractable()
    {
        _used = false;
    }
}