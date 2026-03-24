using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class HospitalRoomMonster : MonoBehaviour
{
    [Header("Required scene references")]
    [SerializeField] private GameObject _monsterObject;
    public GameObject MonsterModel => _monsterObject;

    [SerializeField] private Animator _animator;
    [SerializeField] private Collider _roomTrigger;

    [Header("Vị trí quái")]
    [SerializeField] private Transform _anchorPoint;

    [Header("Encounter rules")]
    [SerializeField] private float _attackDelay = 3f;

    [Header("UI & Game Over")]
    [SerializeField] private float _jumpscareDuration = 2.5f;
    // Đã xóa UI Game Over riêng lẻ để dùng hệ thống chung của dự án

    [Header("Hiệu ứng Choáng (Stun Effects)")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _stunnedClip;
    [SerializeField] private ParticleSystem _stunParticles;
    [SerializeField] private string _stunAnimatorTrigger = "isStunned";

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
        if (_monsterObject == null) return;

        if (_animator == null) _animator = _monsterObject.GetComponent<Animator>();

        NavMeshAgent agent = _monsterObject.GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        Vector3 spawnPos = (_anchorPoint != null) ? _anchorPoint.position : _monsterObject.transform.position;
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
    }

    private void Update()
    {
        if (!_isInitialized || _monsterObject == null || _isDeadSequence) return;

        if (currentState != MonsterLocalState.Stunned)
        {
            _monsterObject.transform.position = _finalLockedPosition;
            _monsterObject.transform.rotation = _finalLockedRotation;

            if (_playerInsideRoom && _player != null)
            {
                Vector3 direction = (_player.position - _monsterObject.transform.position).normalized;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    _monsterObject.transform.rotation = Quaternion.Slerp(_monsterObject.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 2f);
                }
            }
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
        Debug.Log("<color=yellow>Player đã VÀO phòng!</color>");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInsideRoom = false;
        CancelAttackRoutine();
        Debug.Log("<color=yellow>Player đã RA KHỎI phòng!</color>");
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

        if (_fpsController != null) _fpsController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (_animator != null)
        {
            _animator.SetTrigger("isJumpscare");
        }

        StartCoroutine(ShowGameOverRoutine());
    }

    private IEnumerator ShowGameOverRoutine()
    {
        // Đợi quái hù xong (chạy hết animation Jumpscare)
        yield return new WaitForSeconds(_jumpscareDuration);

        // GỌI HỆ THỐNG GAME OVER CHUNG CỦA DỰ ÁN (Mở Menu Retry/Exit)
        GameBroadcast.OnHUDStateChanged?.Invoke(HUDState.GameOver);
    }

    public void TakeDamage(float duration)
    {
        if (currentState == MonsterLocalState.Stunned) return;

        currentState = MonsterLocalState.Stunned;
        CancelAttackRoutine();

        if (_animator != null && !string.IsNullOrEmpty(_stunAnimatorTrigger))
        {
            _animator.SetTrigger(_stunAnimatorTrigger);
        }

        if (_stunParticles != null)
        {
            _stunParticles.Play();
        }

        if (_audioSource != null && _stunnedClip != null)
        {
            _audioSource.PlayOneShot(_stunnedClip);
        }

        Debug.Log($"<color=magenta>QUÁI ĐÃ BỊ CHOÁNG {duration} GIÂY VÀ PHÁT HIỆU ỨNG!</color>");

        Invoke("RecoverFromStun", duration);
    }

    private void RecoverFromStun()
    {
        currentState = MonsterLocalState.Idle;
        Debug.Log("<color=red>QUÁI ĐÃ TỈNH LẠI!</color>");
    }
}