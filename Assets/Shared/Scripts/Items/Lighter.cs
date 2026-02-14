using UnityEngine;
using UnityEngine.InputSystem;

public class Lighter : MonoBehaviour
{
    [Header("Lighter Visuals")]
    [SerializeField] private ParticleSystem _flameParticles;
    [Header("Point Light")]
    [SerializeField] private Light _pointLight;
    [SerializeField] private float _pointIntensity = 5f;
    [SerializeField] private float _pointRange = 3f;
    [SerializeField] private Color _flashColor = Color.yellow;
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _igniteClip;

    private bool _isLit = false;
    // Always ensure we start and end in a "Safe" state
    private void Start() => ResetLogic();
    private void OnDisable() => ResetLogic();

    private void ResetLogic()
    {
        _isLit = false;
        if (_flameParticles != null) _flameParticles.Stop();
        if (_pointLight != null) _pointLight.enabled = false;
        GameEvents.OnLighterToggle?.Invoke(false);
    }

    private void Update()
    {
        // Only run if is held
        if (transform.parent == null || !transform.parent.CompareTag("HandSocket")) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            ToggleLighter();
        }
    }

    private void ToggleLighter()
    {
        _isLit = !_isLit;

        // Visuals
        if (_isLit)
        {
            if (_flameParticles != null) _flameParticles.Play();
            if (_pointLight != null)
            {
                _pointLight.enabled = true;
                _pointLight.intensity = _pointIntensity;
                _pointLight.range = _pointRange;
                _pointLight.color = _flashColor;
            }
            if (_audioSource != null && _igniteClip != null)
            {
                _audioSource.PlayOneShot(_igniteClip);
            }
        }
        else
        {
            if (_flameParticles != null) _flameParticles.Stop();
            if (_pointLight != null) _pointLight.enabled = false;
        }

        // Broadcast the change
        GameEvents.OnLighterToggle?.Invoke(_isLit);
    }
}