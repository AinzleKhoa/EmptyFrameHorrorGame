using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;

public class L2_MonsterAI : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Transform _player;

    [Header("Hide & Seek Settings")]
    public float SightDistance = 15f;
    public float WalkSpeed = 2.5f;
    public float RunSpeed = 6.0f;
    public LayerMask ObstacleMask;
    public float PatrolRadius = 35f;

    [Header("Animation")]
    public Animator MonsterAnimator;

    [Header("Audio Sources")]
    public AudioSource IdleAudioSource;
    public AudioSource ChaseAudioSource;
    public AudioSource StunAudioSource;

    private float _idleTimer;
    private float MinIdleTime = 8f;
    private float MaxIdleTime = 15f;

    [Header("Stun Settings")]
    public float StunDuration = 4.0f;
    private bool _isStunned = false;

    private Vector3 _lastKnownPosition;
    private bool _isChasing = false;
    private float _chaseTimer = 0f;
    private float _maxChaseTime = 4.0f;
    private float _pathUpdateTimer = 0f;

    private float _stuckTimer = 0f;
    private float _stuckThresholdTime = 1.5f;

    private bool _alreadyProcessedHitThisStun = false;
    private float _lastHitTime = 0f;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;
        if (MonsterAnimator == null) MonsterAnimator = GetComponentInChildren<Animator>();

        _idleTimer = Random.Range(MinIdleTime, MaxIdleTime);
        GotoRandomPoint();
    }

    private void Update()
    {
        if (_player == null || _agent == null || !_agent.isOnNavMesh || _isStunned) return;

        if (MonsterAnimator != null) MonsterAnimator.SetFloat("Speed", _agent.velocity.magnitude);

        float targetSpeed = _isChasing ? RunSpeed : WalkSpeed;
        if (_agent.hasPath && _agent.remainingDistance > 0.5f)
        {
            Vector3 targetDir = _agent.steeringTarget - transform.position;
            targetDir.y = 0;
            if (targetDir.sqrMagnitude > 0.1f)
            {
                float angle = Vector3.Angle(transform.forward, targetDir);
                if (angle > 40f) _agent.speed = Mathf.Lerp(_agent.speed, 0.5f, Time.deltaTime * 10f);
                else _agent.speed = Mathf.Lerp(_agent.speed, targetSpeed, Time.deltaTime * 4f);
            }
        }
        else { _agent.speed = targetSpeed; }

        if (_agent.hasPath && _agent.remainingDistance > 1f)
        {
            if (_agent.velocity.magnitude < 0.2f && _agent.speed > 1f)
            {
                _stuckTimer += Time.deltaTime;
                if (_stuckTimer >= _stuckThresholdTime)
                {
                    _stuckTimer = 0f;
                    _isChasing = false;
                    _agent.ResetPath();
                    Vector3 randomEscapeDir = Random.insideUnitSphere * 2.5f + transform.position;
                    if (NavMesh.SamplePosition(randomEscapeDir, out NavMeshHit escapeHit, 4f, NavMesh.AllAreas))
                    {
                        _agent.Warp(escapeHit.position);
                    }
                    GotoRandomPoint();
                }
            }
            else { _stuckTimer = 0f; }
        }
        else { _stuckTimer = 0f; }

        float distToPlayer = Vector3.Distance(transform.position, _player.position);
        bool canSeePlayer = false;
        if (distToPlayer <= SightDistance)
        {
            Vector3 eyePos = transform.position + Vector3.up * 1.5f;
            Vector3 targetPos = _player.position + Vector3.up * 1.0f;
            Vector3 dirToPlayer = (targetPos - eyePos).normalized;
            if (!Physics.Raycast(eyePos, dirToPlayer, distToPlayer, ObstacleMask))
            {
                canSeePlayer = true;
            }
        }

        if (canSeePlayer)
        {
            _isChasing = true;
            _chaseTimer = _maxChaseTime;
            _lastKnownPosition = _player.position;
            _pathUpdateTimer -= Time.deltaTime;
            if (_pathUpdateTimer <= 0f)
            {
                _agent.isStopped = false;
                _agent.SetDestination(_player.position);
                _pathUpdateTimer = 0.2f;
            }
            if (MonsterAnimator) MonsterAnimator.SetBool("IsChasing", true);
            PlayAudio(ChaseAudioSource);
        }
        else if (_isChasing)
        {
            _chaseTimer -= Time.deltaTime;
            _pathUpdateTimer -= Time.deltaTime;
            if (_pathUpdateTimer <= 0f)
            {
                _agent.SetDestination(_lastKnownPosition);
                _pathUpdateTimer = 0.2f;
            }
            if (_chaseTimer <= 0f || (!_agent.pathPending && _agent.remainingDistance < 1f))
            {
                _isChasing = false;
                if (MonsterAnimator) MonsterAnimator.SetBool("IsChasing", false);
            }
        }
        else
        {
            if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            {
                GotoRandomPoint();
                if (MonsterAnimator) MonsterAnimator.SetBool("IsChasing", false);
            }
            _idleTimer -= Time.deltaTime;
            if (_idleTimer <= 0f)
            {
                PlayAudio(IdleAudioSource);
                _idleTimer = Random.Range(MinIdleTime, MaxIdleTime);
            }
        }
    }

    private void GotoRandomPoint()
    {
        if (_agent == null || !_agent.isOnNavMesh) return;
        Vector3 randomDir = Random.insideUnitSphere * PatrolRadius + transform.position;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, PatrolRadius, NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            _agent.CalculatePath(hit.position, path);
            if (path.status == NavMeshPathStatus.PathComplete) _agent.SetDestination(hit.position);
        }
    }

    public void ApplyFlashStun()
    {
        if (_isStunned) return;
        StartCoroutine(StunRoutine());
    }

    private IEnumerator StunRoutine()
    {
        _isStunned = true;
        _alreadyProcessedHitThisStun = true;

        // 👉 Ép con quái đứng im tại chỗ 100%
        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
        }

        if (MonsterAnimator) MonsterAnimator.SetTrigger("Stun");
        PlayAudio(StunAudioSource);

        // Đợi đủ 4 giây sếp chạy
        yield return new WaitForSeconds(StunDuration);

        _isStunned = false;
        _alreadyProcessedHitThisStun = false;

        // Quái tỉnh lại tiếp tục đi tuần
        if (_agent != null && _agent.isOnNavMesh) _agent.isStopped = false;
        _isChasing = false;
        GotoRandomPoint();
    }

    public void ForceInvestigate(Vector3 targetPosition, float customChaseTime = -1f)
    {
        if (_isStunned || _agent == null || !_agent.isOnNavMesh) return;
        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            _agent.CalculatePath(hit.position, path);
            if (path.status == NavMeshPathStatus.PathComplete || path.status == NavMeshPathStatus.PathPartial)
            {
                _isChasing = true;
                _chaseTimer = customChaseTime > 0 ? customChaseTime : (_maxChaseTime * 2f);
                _lastKnownPosition = hit.position;
                _agent.SetDestination(hit.position);
                if (MonsterAnimator) MonsterAnimator.SetBool("IsChasing", true);
                PlayAudio(ChaseAudioSource);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Đang choáng thì đứng im, không làm gì cả
        if (_isStunned || _alreadyProcessedHitThisStun) return;

        if (other.CompareTag("Player"))
        {
            // Chống lỗi va chạm liên hoàn (đợi ít nhất 4.5 giây mới bị chạm tiếp)
            if (Time.time - _lastHitTime < (StunDuration + 0.5f)) return;
            _lastHitTime = Time.time;

            L2_StudentCardManager cardManager = L2_StudentCardManager.Instance;
            if (cardManager == null) cardManager = FindObjectOfType<L2_StudentCardManager>();

            // NẾU CÒN THẺ -> TRỪ 1 THẺ VÀ ĐỨNG IM CHỜ SẾP CHẠY
            if (cardManager != null && cardManager.ConsumeCard())
            {
                _alreadyProcessedHitThisStun = true;
                GameBroadcast.OnUpdateItemDescription?.Invoke("<color=yellow>-1 STUDENT CARD! RUN!</color>");
                ApplyFlashStun();
            }
            // NẾU HẾT THẺ -> GAME OVER
            else
            {
                GameBroadcast.OnUpdateItemDescription?.Invoke("<color=red>GAME OVER!</color>");
                GameFlowController gameFlow = FindObjectOfType<GameFlowController>();
                if (gameFlow != null) gameFlow.KillPlayer();
                else StartCoroutine(GameOverRoutine());
            }
        }
    }

    private IEnumerator GameOverRoutine()
    {
        if (_agent != null && _agent.isOnNavMesh) _agent.isStopped = true;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void PlayAudio(AudioSource sourceToPlay)
    {
        if (sourceToPlay != null && !sourceToPlay.isPlaying) sourceToPlay.Play();
    }
}