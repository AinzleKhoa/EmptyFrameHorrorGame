using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class HospitalRoomMonster : MonoBehaviour
{
    [Header("Required scene references")]
    [SerializeField] private GameObject _monsterObject; // Kéo Object quái vào đây
    [SerializeField] private Animator _animator;
    [SerializeField] private Collider _roomTrigger;

    [Header("Vị trí quái")]
    [SerializeField] private Transform _anchorPoint;

    [Header("Encounter rules")]
    [SerializeField] private float _attackDelay = 3f;
    [SerializeField] private float _stunDurationForThisStage = 20f;

    [Header("UI & Game Over")]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private float _jumpscareDuration = 2.5f;

    private Transform _player;
    private FPSController _fpsController;
    private Coroutine _attackRoutine;
    private bool _playerInsideRoom;
    private bool _isDeadSequence;

    private Vector3 _finalLockedPosition;
    private Quaternion _finalLockedRotation;
    private bool _isInitialized = false;

    public enum MonsterLocalState { Idle, Stunned }
    public MonsterLocalState currentState = MonsterLocalState.Idle;

    private void Awake()
    {
        if (_roomTrigger == null) _roomTrigger = GetComponent<Collider>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
            _fpsController = playerObj.GetComponent<FPSController>();
        }
    }

    private void Start()
    {
        if (_monsterObject == null)
        {
            Debug.LogError("Chưa kéo Object Quái vào ô Monster Object của " + gameObject.name);
            return;
        }

        // 1. TẮT NAVMESH ĐỂ CHỐNG XUYÊN TƯỜNG
        NavMeshAgent agent = _monsterObject.GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        // 2. ÉP CHẠM ĐẤT CHỐNG LƠ LỬNG
        Vector3 spawnPos = (_anchorPoint != null) ? _anchorPoint.position : _monsterObject.transform.position;
        // Bắn tia xuống đất 5m để tìm sàn
        if (Physics.Raycast(spawnPos + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 5f))
        {
            _finalLockedPosition = hit.point;
        }
        else
        {
            _finalLockedPosition = spawnPos;
        }

        _monsterObject.transform.position = _finalLockedPosition;
        _finalLockedRotation = (_anchorPoint != null) ? _anchorPoint.rotation : _monsterObject.transform.rotation;
        _monsterObject.transform.rotation = _finalLockedRotation;

        _isInitialized = true;
        if (_gameOverPanel != null) _gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (!_isInitialized || _monsterObject == null || _isDeadSequence) return;

        // KHÓA VỊ TRÍ TUYỆT ĐỐI TRONG UPDATE
        if (currentState != MonsterLocalState.Stunned)
        {
            _monsterObject.transform.position = _finalLockedPosition;
            _monsterObject.transform.rotation = _finalLockedRotation;
        }

        HandleAttackLogic();
    }

    private void HandleAttackLogic()
    {
        if (!_playerInsideRoom || _isDeadSequence || currentState == MonsterLocalState.Stunned)
        {
            CancelAttackRoutine();
            return;
        }

        if (_attackRoutine == null)
        {
            _attackRoutine = StartCoroutine(AttackCountdownRoutine());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || _isDeadSequence) return;
        _playerInsideRoom = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInsideRoom = false;
        CancelAttackRoutine();
    }

    private IEnumerator AttackCountdownRoutine()
    {
        float timer = _attackDelay;
        while (timer > 0f)
        {
            if (!_playerInsideRoom || _isDeadSequence || currentState == MonsterLocalState.Stunned) yield break;
            timer -= Time.deltaTime;
            yield return null;
        }
        _attackRoutine = null;
        TriggerPlayerCaught();
    }

    private void CancelAttackRoutine()
    {
        if (_attackRoutine != null) { StopCoroutine(_attackRoutine); _attackRoutine = null; }
    }

    private void TriggerPlayerCaught()
    {
        if (_isDeadSequence) return;
        _isDeadSequence = true;

        // KHÓA NGƯỜI CHƠI TRỰC TIẾP
        if (_fpsController != null) _fpsController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (_animator == null) _animator = _monsterObject.GetComponent<Animator>();
        if (_animator != null)
        {
            _animator.SetTrigger("isJumpscare");
        }

        StartCoroutine(ShowGameOverRoutine());
    }

    private IEnumerator ShowGameOverRoutine()
    {
        yield return new WaitForSeconds(_jumpscareDuration);
        if (_gameOverPanel != null) _gameOverPanel.SetActive(true);
        else SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RetryGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void TakeDamage(float duration)
    {
        currentState = MonsterLocalState.Stunned;
        CancelAttackRoutine();
        Invoke("RecoverFromStun", duration);
    }

    private void RecoverFromStun() => currentState = MonsterLocalState.Idle;
}