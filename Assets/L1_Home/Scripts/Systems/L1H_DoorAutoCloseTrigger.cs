using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class L1H_DoorAutoCloseTrigger : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private string _playerTag = "Player";

    [Header("Door")]
    [SerializeField] private L1H_DoorInteractable _door;
    [SerializeField] private bool _lockDoorAfterClose = true;
    [SerializeField] private float _closeWaitTime = 0.35f;

    [Header("Signal")]
    [SerializeField] private float _signalDelayAfterLock = 0.05f;

    [Header("Options")]
    [SerializeField] private bool _triggerOnlyOnce = true;

    private bool _triggered;
    private bool _sequenceRunning;
    private SignalTrigger _signalTrigger;
    private BoxCollider _boxCollider;

    private void Awake()
    {
        _signalTrigger = GetComponent<SignalTrigger>();
        _boxCollider = GetComponent<BoxCollider>();

        if (_boxCollider != null)
            _boxCollider.isTrigger = true;
    }

    private void OnEnable()
    {
        CheckPlayerAlreadyInside();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other)) return;
        TriggerNow();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsPlayer(other)) return;
        TriggerNow();
    }

    private bool IsPlayer(Collider other)
    {
        return other != null && other.transform.root.CompareTag(_playerTag);
    }

    private void CheckPlayerAlreadyInside()
    {
        if (_boxCollider == null) return;
        if (_triggerOnlyOnce && _triggered) return;

        Vector3 worldCenter = transform.TransformPoint(_boxCollider.center);
        Vector3 halfExtents = Vector3.Scale(_boxCollider.size * 0.5f, transform.lossyScale);

        Collider[] hits = Physics.OverlapBox(
            worldCenter,
            halfExtents,
            transform.rotation,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < hits.Length; i++)
        {
            if (IsPlayer(hits[i]))
            {
                TriggerNow();
                return;
            }
        }
    }

    private void TriggerNow()
    {
        if (_sequenceRunning) return;
        if (_triggerOnlyOnce && _triggered) return;

        _triggered = true;
        _sequenceRunning = true;

        StartCoroutine(CloseDoorThenLockThenSignal());
    }

    private IEnumerator CloseDoorThenLockThenSignal()
    {
        if (_door != null)
        {
            Debug.Log("[L1] Force closing door...");
            _door.ForceCloseDoor();
        }
        else
        {
            Debug.LogWarning("[L1] Door reference is missing on DoorAutoCloseTrigger.");
        }

        yield return new WaitForSeconds(_closeWaitTime);

        if (_door != null && _lockDoorAfterClose)
        {
            Debug.Log("[L1] Locking door...");
            _door.SetLocked(true);
        }

        yield return new WaitForSeconds(_signalDelayAfterLock);

        if (_signalTrigger != null)
        {
            Debug.Log("[L1] Raising entered-room signal...");
            _signalTrigger.RaiseSignal();
        }
        else
        {
            Debug.LogWarning("[L1] Door trigger has no SignalTrigger attached.");
        }

        _sequenceRunning = false;
    }

    public void ResetTrigger()
    {
        _triggered = false;
        _sequenceRunning = false;
        StopAllCoroutines();
    }
}