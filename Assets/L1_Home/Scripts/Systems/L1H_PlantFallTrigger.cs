using UnityEngine;

public class L1H_PlantFallTrigger : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private string _playerTag = "Player";

    [Header("Plant")]
    [SerializeField] private Rigidbody _plantRb;
    [SerializeField] private Vector3 _pushDirection = new Vector3(1f, 0f, 0.2f);
    [SerializeField] private float _pushForce = 2.5f;

    [Header("Optional Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _fallSfx;

    [Header("Options")]
    [SerializeField] private bool _triggerOnlyOnce = true;

    private bool _triggered;

    private void Start()
    {
        if (_plantRb != null)
        {
            _plantRb.isKinematic = true;
            _plantRb.useGravity = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggerOnlyOnce && _triggered) return;
        if (!other.transform.root.CompareTag(_playerTag)) return;

        _triggered = true;

        if (_plantRb != null)
        {
            _plantRb.isKinematic = false;
            _plantRb.AddForce(_pushDirection.normalized * _pushForce, ForceMode.Impulse);
        }

        if (_audioSource != null && _fallSfx != null)
            _audioSource.PlayOneShot(_fallSfx);
    }
}