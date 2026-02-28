using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
    [Header("Item Name: Flashlight")]

    [Space(15)]

    [Header("Spot Light")]
    [SerializeField] private Light _spotLight;
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _flashOnClip;
    [SerializeField] private AudioClip _flashOffClip;

    private bool _isLit = false;
    // Always ensure we start and end in a "Safe" state
    private void Start() => ResetLogic();
    private void OnDisable() => ResetLogic();

    private void ResetLogic()
    {
        _isLit = false;
        if (_spotLight != null) _spotLight.enabled = false;
        GameBroadcast.OnFlashlightToggle?.Invoke(false);
    }

    private void Update()
    {
        // Only run if is held
        if (transform.parent == null || !transform.parent.CompareTag("HandSocket")) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            ToggleFlashlight();
        }
    }

    private void ToggleFlashlight()
    {
        _isLit = !_isLit;

        // Visuals
        if (_isLit)
        {
            if (_spotLight != null)
            {
                _spotLight.enabled = true;
            }
            if (_audioSource != null && _flashOnClip != null)
            {
                _audioSource.PlayOneShot(_flashOnClip);
            }
        }
        else
        {
            if (_audioSource != null && _flashOffClip != null)
            {
                _audioSource.PlayOneShot(_flashOffClip);
            }
            if (_spotLight != null) _spotLight.enabled = false;
        }

        // Broadcast the change
        GameBroadcast.OnFlashlightToggle?.Invoke(_isLit);
    }
}