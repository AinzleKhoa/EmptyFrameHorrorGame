using UnityEngine;

public class L1H_WallLightSwitchInteractable : MonoBehaviour, IInteractable
{
    [Header("Prompt")]
    [SerializeField] private string _prompt = "Press 'E' to Turn On the Light";

    [Header("Light Flicker Controller")]
    [SerializeField] private L1H_LightFlickerController _lightFlickerController;

    [Header("Optional Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _switchSfx;

    private bool _isOn;
    private SignalTrigger _signalTrigger;

    public string PromptMessage => _isOn ? "" : _prompt;

    private void Awake()
    {
        _signalTrigger = GetComponent<SignalTrigger>();

        if (_lightFlickerController != null)
            _lightFlickerController.TurnOffCompletely();
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (_isOn) return;

        _isOn = true;

        if (_lightFlickerController != null)
            _lightFlickerController.TurnOnWithFlicker();

        if (_audioSource != null && _switchSfx != null)
            _audioSource.PlayOneShot(_switchSfx);

        if (_signalTrigger != null)
            _signalTrigger.RaiseSignal();
        else
            Debug.LogWarning($"[L1] {name} has no SignalTrigger attached.");
    }
}