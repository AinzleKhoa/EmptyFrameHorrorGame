using UnityEngine;

public class L1H_GrandmaDoorProgression : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private L1H_DoorInteractable _doorInteractable;
    [SerializeField] private L1H_FragmentProgressionCounter _fragmentCounter;

    [Header("Signal")]
    [SerializeField] private GameSignal _grandmaRoomOpenedSignal;

    private bool _wasUnlocked;
    private bool _signalRaised;

    private void Start()
    {
        if (_doorInteractable == null)
            _doorInteractable = GetComponent<L1H_DoorInteractable>();

        if (_doorInteractable != null)
            _doorInteractable.SetLocked(true);
    }

    private void Update()
    {
        if (_doorInteractable == null || _fragmentCounter == null)
            return;

        // Đủ 3 mảnh thì mở khóa đúng 1 lần
        if (!_wasUnlocked && _fragmentCounter.HasAllFragments)
        {
            _wasUnlocked = true;
            _doorInteractable.SetLocked(false);
            Debug.Log("Grandma room door unlocked.");
        }

        // Khi cửa đã thật sự mở thì bắn signal đúng 1 lần
        if (!_signalRaised && _doorInteractable.IsOpen)
        {
            _signalRaised = true;

            if (_grandmaRoomOpenedSignal != null)
            {
                Debug.Log($"Signal Raised: {_grandmaRoomOpenedSignal.name}");
                _grandmaRoomOpenedSignal.Raise();
            }
        }
    }
}