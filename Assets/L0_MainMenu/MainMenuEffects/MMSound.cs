using UnityEngine;
using System.Collections;

public sealed class MMSound : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;

    void Start()
    {
        if (_audioSource != null)
        {
            _audioSource.clip = _audioClip;
            _audioSource.Play();
        }
    }
}