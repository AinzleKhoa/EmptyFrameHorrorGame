using UnityEngine;

public class L1H_PhotoFragmentInteractable : MonoBehaviour, IInteractable
{
    [Header("Prompt")]
    [SerializeField] private string _promptMessage = "Press [E] to Pick Up";
    public string PromptMessage => _promptMessage;

    [Header("Fragment Progress")]
    [SerializeField] private L1H_FragmentProgressionCounter _fragmentCounter;

    [Header("Optional")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _pickupSfx;
    [SerializeField] private GameObject _objectToDisable;

    private bool _collected;

    public void Interact(PlayerInteractor interactor)
    {
        if (_collected) return;

        _collected = true;

        if (_fragmentCounter != null)
            _fragmentCounter.AddFragment();
        else
            Debug.LogWarning($"{name}: FragmentCounter is missing.");

        if (_audioSource != null && _pickupSfx != null)
            _audioSource.PlayOneShot(_pickupSfx);

        if (_objectToDisable != null)
            _objectToDisable.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}