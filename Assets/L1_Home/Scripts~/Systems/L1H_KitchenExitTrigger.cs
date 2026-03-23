using UnityEngine;

public class L1H_KitchenExitTrigger : MonoBehaviour
{
    [SerializeField] private SignalTrigger _signalTrigger;
    [SerializeField] private bool _triggerOnlyOnce = true;

    private bool _hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggerOnlyOnce && _hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        _hasTriggered = true;

        if (_signalTrigger != null)
            _signalTrigger.RaiseSignal();
        else
            Debug.LogWarning($"[L1] {name} has no SignalTrigger assigned.");
    }
}