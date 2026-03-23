using UnityEngine;

public class L1H_KitchenRadio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource _audioSource;

    [Header("Clips")]
    [SerializeField] private AudioClip _staticClip;
    [SerializeField] private AudioClip _voiceClip;

    [Header("Timing")]
    [SerializeField] private float _voiceDelayAfterSink = 1.2f;

    [Header("Play Once")]
    [SerializeField] private bool _playStaticOnlyOnce = true;
    [SerializeField] private bool _playVoiceOnlyOnce = true;

    private bool _hasPlayedStatic;
    private bool _hasPlayedVoice;

    private void Awake()
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        if (_audioSource != null)
        {
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
        }
    }

    public void PlayStatic()
    {
        if (_audioSource == null || _staticClip == null) return;
        if (_playStaticOnlyOnce && _hasPlayedStatic) return;

        _audioSource.Stop();
        _audioSource.PlayOneShot(_staticClip);
        _hasPlayedStatic = true;
    }

    public void PlayVoice()
    {
        if (_audioSource == null || _voiceClip == null) return;
        if (_playVoiceOnlyOnce && _hasPlayedVoice) return;

        _audioSource.Stop();
        _audioSource.PlayOneShot(_voiceClip);
        _hasPlayedVoice = true;
    }

    public void PlayVoiceDelayed()
    {
        if (!gameObject.activeInHierarchy) return;

        CancelInvoke(nameof(PlayVoice));
        Invoke(nameof(PlayVoice), _voiceDelayAfterSink);
    }

    public void StopRadio()
    {
        if (_audioSource != null)
            _audioSource.Stop();
    }

    public void ResetRadio()
    {
        CancelInvoke(nameof(PlayVoice));
        _hasPlayedStatic = false;
        _hasPlayedVoice = false;

        if (_audioSource != null)
            _audioSource.Stop();
    }
}