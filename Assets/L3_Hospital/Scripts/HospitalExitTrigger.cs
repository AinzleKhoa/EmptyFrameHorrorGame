using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HospitalExitTrigger : MonoBehaviour
{
    [SerializeField] private string _nextSceneName = "L4_Maze";
    private bool _used;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_used) return;
        if (!other.CompareTag("Player")) return;

        _used = true;
        GameBroadcast.OnLevelTransitionStarted?.Invoke(_nextSceneName);
    }
}
