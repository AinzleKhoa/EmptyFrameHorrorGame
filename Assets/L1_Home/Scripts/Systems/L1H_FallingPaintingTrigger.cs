using UnityEngine;

public class L1H_FallingPaintingTrigger : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private string _playerTag = "Player";

    [Header("Painting")]
    [SerializeField] private Rigidbody _paintingRb;
    [SerializeField] private bool _triggerOnlyOnce = true;

    [Header("Optional Push")]
    [SerializeField] private Vector3 _pushDirection = new Vector3(0.2f, 0f, -0.1f);
    [SerializeField] private float _pushForce = 1.5f;

    [Header("Optional Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _fallSfx;

    private bool _triggered;

    private void Start()
    {
        if (_paintingRb != null)
        {
            _paintingRb.isKinematic = true;
            _paintingRb.useGravity = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggerOnlyOnce && _triggered) return;
        if (!other.transform.root.CompareTag(_playerTag)) return;

        _triggered = true;

        if (_paintingRb != null)
        {
            _paintingRb.isKinematic = false;

            Vector3 dir = _pushDirection.normalized;
            if (dir.sqrMagnitude > 0.001f)
                _paintingRb.AddForce(dir * _pushForce, ForceMode.Impulse);
        }

        if (_audioSource != null && _fallSfx != null)
            _audioSource.PlayOneShot(_fallSfx);

        Debug.Log("Painting fell down.");

        if (_triggerOnlyOnce)
            gameObject.SetActive(false);
    }
}