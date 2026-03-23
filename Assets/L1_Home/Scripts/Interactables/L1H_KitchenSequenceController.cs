using System.Collections;
using UnityEngine;

public class L1_KitchenSequenceController : MonoBehaviour
{
    [Header("Gameplay References")]
    [SerializeField] private L1H_SinkInteractable _sinkInteractable;
    [SerializeField] private GameObject _exitTriggerObject;
    [SerializeField] private L1H_LightFlickerController _kitchenLightController;

    [Header("Ambient Audio")]
    [SerializeField] private AudioSource _dripAudio;
    [SerializeField] private AudioSource _radioAudio;

    [Header("Radio Clip")]
    [SerializeField] private AudioClip _radioStaticClip;

    [Header("Radio Delays")]
    [SerializeField] private float _lightRadioDelay = 1f;
    [SerializeField] private float _sinkRadioDelay = 1f;
    [SerializeField] private float _exitRadioDelay = 0.2f;

    private Coroutine _radioCoroutine;

    public void OnKitchenStart()
    {
        if (_sinkInteractable != null)
            _sinkInteractable.DisableInteract();

        if (_exitTriggerObject != null)
            _exitTriggerObject.SetActive(false);

        if (_dripAudio != null && !_dripAudio.isPlaying)
            _dripAudio.Play();

        if (_radioAudio != null)
            _radioAudio.Stop();

        StopPendingRadio();
    }

    public void OnLightTurnedOn()
    {
        if (_sinkInteractable != null)
            _sinkInteractable.EnableInteract();

        PlayRadioOneShotDelayed(_lightRadioDelay);
    }

    public void OnSinkChecked()
    {
        if (_dripAudio != null)
            _dripAudio.Stop();

        if (_exitTriggerObject != null)
            _exitTriggerObject.SetActive(true);

        PlayRadioOneShotDelayed(_sinkRadioDelay);
    }

    public void OnKitchenExited()
    {
        if (_dripAudio != null)
            _dripAudio.Stop();

        if (_kitchenLightController != null)
            _kitchenLightController.FlickerAndTurnOff();

        PlayRadioOneShotDelayed(_exitRadioDelay);
    }

    public void StopAllKitchenAudio()
    {
        StopPendingRadio();

        if (_dripAudio != null)
            _dripAudio.Stop();

        if (_radioAudio != null)
            _radioAudio.Stop();
    }

    private void PlayRadioOneShotDelayed(float delay)
    {
        if (_radioAudio == null || _radioStaticClip == null) return;

        StopPendingRadio();
        _radioCoroutine = StartCoroutine(PlayRadioAfterDelay(delay));
    }

    private IEnumerator PlayRadioAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (_radioAudio != null && _radioStaticClip != null)
            _radioAudio.PlayOneShot(_radioStaticClip);

        _radioCoroutine = null;
    }

    private void StopPendingRadio()
    {
        if (_radioCoroutine != null)
        {
            StopCoroutine(_radioCoroutine);
            _radioCoroutine = null;
        }
    }
}