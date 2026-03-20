using UnityEngine;

[RequireComponent(typeof(SignalTrigger))] // Ensures the signal is there
public class Phase1Director : MonoBehaviour
{
    [Header("Goal Settings")]
    [SerializeField] private int _fragmentsRequired = 2;
    private int _fragmentsCompleted = 0;

    private SignalTrigger _completionSignal;

    private void Awake()
    {
        _completionSignal = GetComponent<SignalTrigger>();
    }

    private void OnEnable()
    {
        L5_GameBroadcast.OnL5FragmentFinished += HandleFragmentFinished;
    }

    private void OnDisable()
    {
        L5_GameBroadcast.OnL5FragmentFinished -= HandleFragmentFinished;
    }

    private void HandleFragmentFinished()
    {
        _fragmentsCompleted++;
        Debug.Log($"Director: Progress {_fragmentsCompleted}/{_fragmentsRequired}");

        if (_fragmentsCompleted >= _fragmentsRequired)
        {
            CompletePhase();
        }
    }

    private void CompletePhase()
    {
        Debug.Log("<color=cyan>Phase 1 Complete. Sending signal to StoryDirector...</color>");

        if (_completionSignal != null)
        {
            _completionSignal.RaiseSignal(); // This hits your StoryDirector
        }
    }
}