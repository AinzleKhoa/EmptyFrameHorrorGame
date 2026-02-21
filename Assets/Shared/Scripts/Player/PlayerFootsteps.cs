using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private AudioSource _audioSource;

    [Header("Surface Mapping")]
    [SerializeField] private List<FootstepData> _surfaceData; // Drag your L1-L5 Data assets here
    [SerializeField] private FootstepData _fallbackData;    // Sound to play if no tag is found

    [Header("Raycast Settings")]
    [SerializeField] private float _rayDistance = 1.5f;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Interval Settings")]
    [SerializeField] private float _baseStepInterval = 0.8f;
    [SerializeField] private float _sprintStepInterval = 0.3f;

    private float _stepTimer;

    private void Update()
    {
        if (_movement == null) return;

        // Only run timer if moving and on the ground, or crouched
        if (_movement.isWalking && _movement.isGrounded && !_movement.isCrouched)
        {
            HandleFootsteps();
        }
        else
        {
            _stepTimer = 0;
        }
    }

    private void HandleFootsteps()
    {
        // we force the timer down to trigger a sound sooner.
        if (_movement.isSprinting && _stepTimer > _sprintStepInterval)
        {
            _stepTimer = 0; // Force an immediate step when starting a sprint
        }

        _stepTimer -= Time.deltaTime;

        if (_stepTimer <= 0)
        {
            PlayMaterialBasedSound();

            // Calculate next interval
            float interval = _baseStepInterval;
            if (_movement.isSprinting) interval = _sprintStepInterval;

            _stepTimer = interval;
        }
    }

    private void PlayMaterialBasedSound()
    {
        // 1. Raycast straight down from the player
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _rayDistance, _groundLayer))
        {
            string currentTag = hit.collider.tag;

            // 2. Find the ScriptableObject that matches this Tag
            FootstepData data = _surfaceData.Find(d => d.materialTag == currentTag);

            if (data != null && data.clips.Length > 0)
            {
                PlayRandomClip(data.clips);
            }
            else if (_fallbackData != null)
            {
                PlayRandomClip(_fallbackData.clips);
            }
        }
    }

    private void PlayRandomClip(AudioClip[] clips)
    {
        int index = Random.Range(0, clips.Length);
        // Add a random pitch between 0.9 and 1.1
        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.PlayOneShot(clips[index]);
    }
}