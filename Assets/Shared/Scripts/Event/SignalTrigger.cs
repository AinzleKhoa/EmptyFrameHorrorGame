using UnityEngine;

public class SignalTrigger : MonoBehaviour
{
    [SerializeField] private GameSignal _signalToRaise;

    // Put this on any GameObject that should raise a signal when interacted with.
    public void RaiseSignal()
    {
        if (_signalToRaise != null)
        {
            Debug.Log($"Signal Raised: {_signalToRaise.name}");
            _signalToRaise.Raise();
        }
    }

    public void SelfDestruct()
    {
        Destroy(this);
    }
}