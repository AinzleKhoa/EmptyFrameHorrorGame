using UnityEngine;

public class HospitalEscapeController : MonoBehaviour
{
    [Header("Required progression")]
    [SerializeField] private FragmentProgressionCounter _fragmentCounter;

    [Header("After all 3 fragments")]
    [SerializeField] private Light[] _lightsToTurnRed;
    [SerializeField] private GameObject[] _objectsToEnableWhenAllFragmentsCollected;
    [SerializeField] private GameObject[] _objectsToDisableWhenAllFragmentsCollected;
    [SerializeField] private string _objectiveAfterAllFragments = "Tìm Shadow ở cuối hành lang.";

    [Header("After final Shadow dialogue")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _teleportBackToLobbyPoint;
    [SerializeField] private Collider[] _stairsBlockersToEnable;
    [SerializeField] private Collider[] _frontDoorBlockersToDisable;
    [SerializeField] private GameObject[] _objectsToEnableAfterFinalDialogue;
    [SerializeField] private GameObject[] _objectsToDisableAfterFinalDialogue;
    [SerializeField] private string _finalEscapeObjective = "Chạy ra cửa chính và thoát khỏi bệnh viện.";

    [Header("Optional final chaser")]
    [SerializeField] private GameObject _finalMonsterToActivate;

    private bool _allFragmentsTriggered;
    private bool _finalDialogueHandled;

    private void Awake()
    {
        if (_player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) _player = playerObj.transform;
        }
    }

    private void OnEnable()
    {
        GameBroadcast.OnAllFragmentsCollected += HandleAllFragmentsCollected;
    }

    private void OnDisable()
    {
        GameBroadcast.OnAllFragmentsCollected -= HandleAllFragmentsCollected;
    }

    private void HandleAllFragmentsCollected(bool state)
    {
        if (!state || _allFragmentsTriggered) return;
        _allFragmentsTriggered = true;

        for (int i = 0; i < _lightsToTurnRed.Length; i++)
        {
            if (_lightsToTurnRed[i] == null) continue;
            _lightsToTurnRed[i].color = Color.red;
            _lightsToTurnRed[i].intensity = Mathf.Max(_lightsToTurnRed[i].intensity, 1.5f);
        }

        SetActive(_objectsToEnableWhenAllFragmentsCollected, true);
        SetActive(_objectsToDisableWhenAllFragmentsCollected, false);

        if (!string.IsNullOrWhiteSpace(_objectiveAfterAllFragments))
            GameBroadcast.OnObjectiveUpdateHUD?.Invoke(_objectiveAfterAllFragments);
    }

    public void HandleFinalShadowDialogueFinished()
    {
        if (_finalDialogueHandled) return;
        _finalDialogueHandled = true;

        if (_player != null && _teleportBackToLobbyPoint != null)
        {
            _player.position = _teleportBackToLobbyPoint.position;
            _player.rotation = _teleportBackToLobbyPoint.rotation;
        }

        SetColliderState(_stairsBlockersToEnable, true);
        SetColliderState(_frontDoorBlockersToDisable, false);
        SetActive(_objectsToEnableAfterFinalDialogue, true);
        SetActive(_objectsToDisableAfterFinalDialogue, false);

        if (_finalMonsterToActivate != null)
            _finalMonsterToActivate.SetActive(true);

        if (!string.IsNullOrWhiteSpace(_finalEscapeObjective))
            GameBroadcast.OnObjectiveUpdateHUD?.Invoke(_finalEscapeObjective);
    }

    private void SetActive(GameObject[] targets, bool active)
    {
        if (targets == null) return;
        foreach (var obj in targets)
        {
            if (obj != null) obj.SetActive(active);
        }
    }

    private void SetColliderState(Collider[] targets, bool enabled)
    {
        if (targets == null) return;
        foreach (var c in targets)
        {
            if (c != null) c.enabled = enabled;
        }
    }
}
