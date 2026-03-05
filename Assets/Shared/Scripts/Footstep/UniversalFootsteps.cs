using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class UniversalFootsteps : MonoBehaviour
{
    [Header("Movement References")]
    private CharacterController _playerController;
    private NavMeshAgent _monsterAgent;

    [Header("Surface Mapping")]
    [SerializeField] private List<FootstepData> _surfaceData;
    [SerializeField] private FootstepData _fallbackData;

    [Header("Distance Settings")]
    [SerializeField] private float _strideLength = 2.0f;     // How many meters per step
    [SerializeField] private float _minSpeedThreshold = 0.5f;

    [Header("Raycast Settings")]
    [SerializeField] private float _rayDistance = 1.2f;
    [SerializeField] private LayerMask _groundLayer;

    private AudioSource _audioSource;
    private float _distanceAccumulator;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        // It looks for a Player controller OR a Monster agent automatically
        _playerController = GetComponent<CharacterController>();
        _monsterAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        float currentSpeed = GetVelocityMagnitude();

        // Only count distance if moving and grounded
        if (currentSpeed > _minSpeedThreshold && IsGrounded())
        {
            _distanceAccumulator += currentSpeed * Time.deltaTime;

            if (_distanceAccumulator >= _strideLength)
            {
                PlayMaterialBasedSound(currentSpeed);
                _distanceAccumulator = 0;
            }
        }
    }

    private float GetVelocityMagnitude()
    {
        if (_playerController != null)
            return new Vector3(_playerController.velocity.x, 0, _playerController.velocity.z).magnitude;

        if (_monsterAgent != null)
            return _monsterAgent.velocity.magnitude;

        return 0f;
    }

    private bool IsGrounded()
    {
        if (_playerController != null) return _playerController.isGrounded;
        return true; // NavMeshAgents are technically always "grounded"
    }

    private void PlayMaterialBasedSound(float speed)
    {
        // Added a tiny offset (up * 0.1f) so the ray doesn't start inside the floor
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, _rayDistance, _groundLayer))
        {
            FootstepData data = _surfaceData.Find(d => d.materialTag == hit.collider.tag);
            FootstepData finalData = (data != null) ? data : _fallbackData;

            if (finalData != null && finalData.clips.Length > 0)
            {
                PlayRandomClip(finalData.clips, speed);
            }
        }
    }

    private void PlayRandomClip(AudioClip[] clips, float speed)
    {
        int index = Random.Range(0, clips.Length);

        // --- PRO UPDATES ---
        // 1. Intensity: Map speed (0 to 8) to a 0.0 to 1.0 range
        float intensity = Mathf.InverseLerp(_minSpeedThreshold, 8f, speed);

        // 2. Dynamic Volume: Sneaking is quiet, sprinting is LOUD
        _audioSource.volume = Mathf.Lerp(0.3f, 1.0f, intensity);

        // 3. Dynamic Pitch: Higher speed feels more "energetic"
        _audioSource.pitch = Mathf.Lerp(0.85f, 1.15f, intensity) + Random.Range(-0.05f, 0.05f);

        _audioSource.PlayOneShot(clips[index]);
    }
}