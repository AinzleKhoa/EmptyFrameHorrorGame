using UnityEngine;

public class ShadowStaticLogic : MonoBehaviour
{
    [SerializeField] private float _maxDistance = 20f;

    [Range(0f, 1f)]
    [SerializeField] private float _maxVolume = 0.5f; // Set your desired "loudest" volume here

    [SerializeField] private AudioClip _staticSoundFile;
    [SerializeField] private AudioSource _staticAudioSource;
    private float _audioMultiplier = 1.0f;

    private void Start()
    {
        if (_staticAudioSource != null && _staticSoundFile != null)
        {
            _staticAudioSource.clip = _staticSoundFile;
            _staticAudioSource.loop = true;
        }
    }

    private void Update()
    {
        var shadow = L5_ShadowMonster.Instance;
        if (!shadow.IsManifested)
        {
            _staticAudioSource.Stop();
            return;
        }

        // --- THE FIX: Physically move the speaker to the Shadow's location ---
        // This makes 3D Spatial Panning work!
        _staticAudioSource.transform.position = shadow.transform.position;

        float dist = Vector3.Distance(transform.position, shadow.transform.position);

        if (dist < _maxDistance)
        {
            // Calculate base intensity (0 to 1)
            float intensity = 1f - (dist / _maxDistance);

            if (!_staticAudioSource.isPlaying) _staticAudioSource.Play();

            // Multiply intensity by your custom _maxVolume
            _staticAudioSource.volume = intensity * _maxVolume * _audioMultiplier;
        }
        else
        {
            if (_staticAudioSource.isPlaying) _staticAudioSource.Stop();
        }
    }
}