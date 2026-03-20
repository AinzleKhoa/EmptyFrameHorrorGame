using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class TheOverexposed : MonoBehaviour
{
    public enum MonsterState { Roaming, Investigating, Alerting, Chasing, Stunned, Stalking }

    [Header("Status")]
    public MonsterState currentState = MonsterState.Roaming;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _footstepSource; // New: Dedicated for feet
    [SerializeField] private AudioSource _screamSource;   // New: Dedicated for screams
    [SerializeField] private AudioSource _musicSource;    // For Chase Music

    [Header("Audio Clips")]
    [SerializeField] private AudioClip _screamClip;
    [SerializeField] private AudioClip _stunnedClip;
    [SerializeField] private AudioClip _investigateClip;
    [SerializeField] private AudioClip _chaseMusic;
    [SerializeField] private AudioClip[] _footstepClips;

    [Header("Footstep Settings")]
    [SerializeField] private float _footstepDistance = 4.0f; // Play every 4 meters
    private Vector3 _lastStepPosition;

    [Header("Navigation & Memory")]
    [SerializeField] private Transform[] _roomWaypoints;
    private Vector3 _investigationPoint;
    private Queue<int> _waypointHistory = new Queue<int>();
    [SerializeField] private float _minTravelDistance = 20f;
    [SerializeField] private float _waitTime = 1f;
    [SerializeField] private int _memorySize = 3;
    private float _idleTimeElapsed;

    [Header("Speeds")]
    [SerializeField] private float _walkSpeed = 4f;
    [SerializeField] private float _runSpeed = 6f;
    [SerializeField] private float _chaseSpeed = 10f;

    [Header("Vision & Hearing Settings")]
    [SerializeField] private float _viewAngle = 60f;
    [SerializeField] private float _detectionRange = 20f;
    private string _currentPlayerMovementState = "Idle";
    private float _noiseCheckTimer;

    [Header("Jumpscare Setup")]
    [SerializeField] private Camera _jumpscareCamera;

    [Header("HUD: Threat Naming (Index 0 = 1st Fragment)")]
    [SerializeField] private List<string> _threatLevelNames = new List<string>();
    private MonsterStateHUD _monsterHUD;

    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;

    private bool _isStalkingPlayer = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SetNextDestination();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            _monsterHUD = playerObj.GetComponentInChildren<MonsterStateHUD>();
        }
    }

    private void HandleMusic()
    {
        // If the monster is chasing, make sure the music is playing
        if (currentState == MonsterState.Chasing || currentState == MonsterState.Investigating || currentState == MonsterState.Stalking)
        {
            if (!_musicSource.isPlaying)
            {
                _musicSource.clip = _chaseMusic;
                _musicSource.Play();
            }
        }
        else // If not chasing, stop the music
        {
            if (_musicSource.isPlaying)
            {
                _musicSource.Stop();
            }
        }
    }

    private void OnEnable()
    {
        GameBroadcast.OnPlayerMovementState += (state) => _currentPlayerMovementState = state;
        GameBroadcast.OnFragmentCollected += HandleProgressionUpdate; // Listen here
    }

    private void OnDisable()
    {
        GameBroadcast.OnPlayerMovementState -= (state) => _currentPlayerMovementState = state;
        GameBroadcast.OnFragmentCollected -= HandleProgressionUpdate;
    }

    void Update()
    {
        // SAFETY CHECK: If the agent is disabled (e.g., during jumpscare), skip all logic
        if (!agent.enabled) return;

        // 1. Always Sync Animation
        anim.SetFloat("Speed", agent.velocity.magnitude);

        HandleMusic();
        HandleFootsteps();

        // 2. High-Priority State Lock (Screaming or Stunned = No thinking)
        if (currentState == MonsterState.Alerting || currentState == MonsterState.Stunned) return;

        // 3. Perception (Inputs that change the Enum)
        if (currentState == MonsterState.Chasing) // Only check if the monster is chasing
        {
            CheckForLightExposure();
        }
        CheckForPlayerVisual();

        // PROGRESSION LOGIC ====
        // If we are at 4/5 fragments, force the state to Stalking
        // Only force Stalking if the monster is currently Roaming or Investigating.
        if (_isStalkingPlayer)
        {
            if (currentState == MonsterState.Roaming || currentState == MonsterState.Investigating)
            {
                currentState = MonsterState.Stalking;
            }
        }
        else if (currentState == MonsterState.Roaming)
        {
            ListenForNoise();
        }

        // 4. State Execution (The Switch)
        switch (currentState)
        {
            case MonsterState.Roaming: ExecuteRoaming(); break;
            case MonsterState.Investigating: ExecuteInvestigating(); break;
            case MonsterState.Chasing: ExecuteChasing(); break;
            case MonsterState.Stalking: ExecuteStalking(); break;
        }
    }

    private void HandleFootsteps()
    {
        // 1. Check if moving
        if (agent.velocity.magnitude > 0.5f)
        {
            // 2. High-Priority Guard
            if (currentState == MonsterState.Alerting || currentState == MonsterState.Stunned) return;

            float distMoved = Vector3.Distance(transform.position, _lastStepPosition);

            if (distMoved >= _footstepDistance)
            {
                PlayFootstep();
                _lastStepPosition = transform.position;
            }
        }
    }

    private void PlayFootstep()
    {
        if (_footstepClips.Length == 0) return;

        AudioClip clip = _footstepClips[Random.Range(0, _footstepClips.Length)];
        float speedPercent = Mathf.InverseLerp(_walkSpeed, _chaseSpeed, agent.velocity.magnitude);

        // Pitch shift for feet
        _footstepSource.pitch = Mathf.Lerp(0.85f, 1.15f, speedPercent);

        // Play on the FOOTSTEP source at a lower volume
        _footstepSource.PlayOneShot(clip);
    }

    #region State Executions

    private void ExecuteRoaming()
    {
        agent.speed = _walkSpeed;
        if (ReachedDestination())
        {
            HandleWaitAndRepath(MonsterState.Roaming);
        }
    }

    private void ExecuteInvestigating()
    {
        agent.speed = _runSpeed;

        float dist = Vector3.Distance(transform.position, player.position);

        // When investigating, if player is sprinting, immediately update the destination to chase them dynamically
        if (_currentPlayerMovementState == "Sprinting" || dist < _detectionRange)
        {
            _investigationPoint = player.position;
            agent.SetDestination(_investigationPoint);
        }

        if (ReachedDestination())
        {
            // After checking the noise, wait a bit then go back to roaming
            HandleWaitAndRepath(MonsterState.Roaming);
        }
    }

    private void ExecuteStalking()
    {
        agent.speed = _runSpeed;

        // Continuously track the player's transform directly
        agent.SetDestination(player.position);

        // Keep the animation moving
        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void ExecuteChasing()
    {
        agent.speed = _chaseSpeed;
        agent.SetDestination(player.position);

        float dist = Vector3.Distance(transform.position, player.position);

        // Lose interest if player gets too far
        if (dist > _detectionRange * 1.5f)
        {
            currentState = MonsterState.Roaming;
            _musicSource.Stop();
        }
    }

    #endregion

    #region Perception Logic

    private void CheckForPlayerVisual()
    {
        if (currentState == MonsterState.Alerting || currentState == MonsterState.Chasing || currentState == MonsterState.Stunned) return;

        // 1. Calculate the range based on player's movement state (Crouching reduces it by 50%)
        float effectiveRange = (_currentPlayerMovementState == "Crouched") ? (_detectionRange * 0.5f) : _detectionRange;

        // 2. Check distance between monster and player
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // Assure the monster is close enough
        if (distToPlayer < effectiveRange)
        {
            // Aim for the chest
            Vector3 targetPoint = player.position + Vector3.up * 0.7f; // Player's chest height
            Vector3 eyeLevel = transform.position + Vector3.up * 1.6f; // Monster's "eyes"
            Vector3 direction = (targetPoint - eyeLevel).normalized;

            if (Vector3.Angle(transform.forward, direction) < _viewAngle)
            {
                int layerMask = ~LayerMask.GetMask("Monster");

                // 3. RAYCAST LENGTH MUST MATCH EFFECTIVE RANGE
                // This prevents the "eyes" from seeing further than the current bubble.
                if (Physics.Raycast(eyeLevel, direction, out RaycastHit hit, effectiveRange, layerMask))
                {
                    bool hitPlayer = hit.transform.CompareTag("Player");
                    if (hitPlayer)
                    {
                        StartCoroutine(AlertScream());
                    }
                }
            }
        }
    }

    private void CheckForLightExposure()
    {
        if (currentState == MonsterState.Stunned) return;

        // 1. Scan for any colliders near the monster's chest
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + Vector3.up, 3f);

        foreach (var hit in hitColliders)
        {
            // 2. Look for the script on the hit object OR any of its parents
            CandleInteractable candle = hit.GetComponentInParent<CandleInteractable>();

            if (candle != null && candle.IsLit)
            {
                Debug.Log("<color=yellow>Monster hit by light from: </color>" + hit.name);

                // 3. Trigger the stun
                TakeDamage(3f);

                // 4. DESTROY the candle so it cannot be used again
                Destroy(candle.gameObject);

                break;
            }
        }
    }

    private void ListenForNoise()
    {
        if (_currentPlayerMovementState == "Idle" || _currentPlayerMovementState == "Crouched") return;

        _noiseCheckTimer += Time.deltaTime;
        if (_noiseCheckTimer >= 0.5f)
        {
            _noiseCheckTimer = 0;
            float dist = Vector3.Distance(transform.position, player.position);

            // Range cap for hearing
            if (dist > _detectionRange) return;

            float baseChance = (_currentPlayerMovementState == "Sprinting") ? 0.5f : 0.05f;

            // 4. THE SMOOTH CURVE (InverseLerp + Power)
            // 20m = 0.0 (Silent), 0m = 1.0 (Maximum volume)
            float proximityFactor = 1f - (dist / 20f);

            // We square the factor (proximityFactor * proximityFactor) 
            // This makes the chance increase SLOWLY at first, then FAST when close.
            float curveFactor = Mathf.Pow(proximityFactor, 2);

            // Calculate final chance (Base + up to 90% bonus at 0m)
            float finalChance = baseChance + (curveFactor * 0.9f);

            // 5. THE ROLL
            if (Random.value < finalChance)
            {
                if (_investigateClip != null)
                {
                    _screamSource.PlayOneShot(_investigateClip);
                }
                TriggerInvestigation();
            }
        }
    }

    private void TriggerInvestigation()
    {
        currentState = MonsterState.Investigating;
        _investigationPoint = player.position;
        agent.SetDestination(_investigationPoint);
    }

    #endregion

    #region Helper & Navigation Methods

    private void HandleWaitAndRepath(MonsterState nextState)
    {
        _idleTimeElapsed += Time.deltaTime;
        if (_idleTimeElapsed >= _waitTime)
        {
            _idleTimeElapsed = 0;
            currentState = nextState;
            SetNextDestination();
        }
    }

    private bool ReachedDestination()
    {
        // Check if the agent is active and on the NavMesh before asking for distance
        if (!agent.isOnNavMesh || !agent.isActiveAndEnabled) return false;

        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    private void SetNextDestination()
    {
        if (_roomWaypoints.Length < _memorySize + 1) return;

        int targetIndex = -1;

        // Memory & Distance Filtering
        if (targetIndex == -1 || _waypointHistory.Contains(targetIndex))
        {
            for (int i = 0; i < 15; i++)
            {
                int rnd = Random.Range(0, _roomWaypoints.Length);
                float d = Vector3.Distance(transform.position, _roomWaypoints[rnd].position);
                if (!_waypointHistory.Contains(rnd) && d >= _minTravelDistance)
                {
                    targetIndex = rnd;
                    break;
                }
            }
        }

        // Emergency Fallback
        if (targetIndex == -1)
        {
            for (int i = 0; i < _roomWaypoints.Length; i++)
            {
                if (!_waypointHistory.Contains(i)) { targetIndex = i; break; }
            }
        }

        _waypointHistory.Enqueue(targetIndex);
        if (_waypointHistory.Count > _memorySize) _waypointHistory.Dequeue();
        agent.SetDestination(_roomWaypoints[targetIndex].position);
    }

    public void TakeDamage(float stunDuration = 8f)
    {
        if (currentState == MonsterState.Stunned) return;

        Debug.Log("Monster hit by camera! Interrupting current state: " + currentState);

        StopAllCoroutines();
        agent.isStopped = true;

        StartCoroutine(StunnedRoutine(stunDuration));
    }

    #endregion

    #region Coroutines

    IEnumerator AlertScream()
    {
        currentState = MonsterState.Alerting;
        agent.isStopped = true;

        if (_screamClip != null)
        {
            _screamSource.PlayOneShot(_screamClip);
        }

        anim.SetTrigger("Scream");
        yield return new WaitForSeconds(2f);
        agent.isStopped = false;
        currentState = MonsterState.Chasing;
    }

    IEnumerator StunnedRoutine(float duration)
    {
        currentState = MonsterState.Stunned;
        agent.isStopped = true;
        anim.SetTrigger("isHit");

        if (_stunnedClip != null)
        {
            _screamSource.Stop();
            _screamSource.PlayOneShot(_stunnedClip);
        }

        yield return new WaitForSeconds(duration);
        anim.SetTrigger("isRecovered");
        yield return new WaitForSeconds(2f);
        agent.isStopped = false;

        // PROGRESSION LOGIC ======
        // If at 4/5 fragments collected stage, monster will always scream when get up, indicated it is looking for player again.
        if (_isStalkingPlayer)
        {
            currentState = MonsterState.Stalking;

            // Play the investigate scream immediately as it gets back up
            if (_investigateClip != null)
            {
                _screamSource.PlayOneShot(_investigateClip);
            }

            Debug.Log("<color=red>Monster recovered and is immediately Stalking again!</color>");
        }
        else
        {
            // Normal behavior for Stages 0-3
            currentState = MonsterState.Roaming;
            SetNextDestination();
        }
    }

    #endregion

    private void OnDrawGizmos()
    {
        if (player == null) return;

        // 1. Calculate the real-time range
        float currentRange = (_currentPlayerMovementState == "Crouched") ? (_detectionRange * 0.5f) : _detectionRange;
        Gizmos.color = (_currentPlayerMovementState == "Crouched") ? Color.cyan : Color.yellow;
        Vector3 eyePos = transform.position + Vector3.up * 1.6f;

        // 2. Draw the Vision Cone
        Vector3 left = Quaternion.Euler(0, -_viewAngle, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, _viewAngle, 0) * transform.forward;

        Gizmos.DrawRay(eyePos, left * currentRange);
        Gizmos.DrawRay(eyePos, right * currentRange);
        Gizmos.DrawLine(eyePos + left * currentRange, eyePos + right * currentRange);

        // 3. Draw the "Sight Line" ONLY if the monster is looking at you
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist < currentRange)
        {
            // Draw a simple sphere at the player's chest to show where the monster is aiming
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position + Vector3.up * 0.7f, 0.3f);
        }

        // Draw a circle on the floor for the total range
        Gizmos.DrawWireSphere(transform.position, currentRange);

        // Draw a line that changes from Yellow (Safe) to Red (Danger) 
        // based on the actual hearing chance
        float d = Vector3.Distance(transform.position, player.position);
        if (d < 20f && _currentPlayerMovementState != "Crouched")
        {
            float intensity = Mathf.Pow(1f - (d / 20f), 2);
            Gizmos.color = Color.Lerp(Color.yellow, Color.red, intensity);
            Gizmos.DrawLine(transform.position + Vector3.up * 1.5f, player.position + Vector3.up * 1.5f);
        }

        // --- SENSOR SURROUNDING DEBUGGER ---
        // Add this to the end of your OnDrawGizmos method
        Gizmos.color = Color.white;
        Vector3 sensorPos = transform.position + Vector3.up;
        Gizmos.DrawWireSphere(sensorPos, 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Prevent multiple jumpscares from firing
            if (currentState == MonsterState.Stunned) return;

            InitiateJumpscare();
        }
    }

    private void InitiateJumpscare()
    {
        Debug.Log("<color=red>JUMPSCARE:</color> Caught the player!");

        // Stop the monster and the player logic
        agent.isStopped = true;
        agent.enabled = false; // Stop navigation entirely

        GameBroadcast.isPlayerFreezed?.Invoke(true);

        // Look at player immediately
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        // Play Jumpscare Animation
        anim.SetTrigger("isJumpscare");

        // Play Jumpscare Sound
        if (_screamSource != null && _screamClip != null)
        {
            _screamSource.pitch = 0.7f; // Make it deeper and scarier
            _screamSource.PlayOneShot(_screamClip);
        }

        Camera mainCam = player.GetComponentInChildren<Camera>();

        if (mainCam == null)
        {
            mainCam = Camera.main;
        }

        // Perform the swap logic
        if (mainCam != null && _jumpscareCamera != null)
        {
            // Turn on the "Movie" camera
            _jumpscareCamera.enabled = true;
            _jumpscareCamera.targetDisplay = 0;

            Debug.Log("Cameras Swapped: Player should now see Jumpscare View.");
        }
        else
        {
            Debug.LogError("Jumpscare Failed: Cameras are not assigned in the Inspector!");
        }

        // 3. Play the visuals and sounds
        anim.SetTrigger("Jumpscare");
        _screamSource.PlayOneShot(_screamClip);

        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        yield return new WaitForSeconds(2.5f);
        // Add your SceneManager.LoadScene here or show your Game Over UI
        Debug.Log("Reloading Level...");
    }

    // Progression Logic
    private void HandleProgressionUpdate(int count)
    {
        switch (count)
        {
            case 1: // 1/5: Walk speed increase
                _walkSpeed += 1f;
                Debug.Log("<color=orange>Monster: Walking Faster...</color>");
                break;

            case 2: // 2/5: Always in running state
                _walkSpeed = _runSpeed; // Forces Roaming to be as fast as Investigating
                Debug.Log("<color=orange>Monster: No longer walking. Only running.</color>");
                break;

            case 3: // 3/5: Even faster
                _chaseSpeed += 2f;
                _walkSpeed += 2f;
                _runSpeed += 2f;
                Debug.Log("<color=red>Monster: Lethal speeds reached.</color>");
                break;

            case 4: // 4/5: Always know your location
                _isStalkingPlayer = true;
                if (_investigateClip != null)
                    _screamSource.PlayOneShot(_investigateClip);
                Debug.Log("<color=purple>Monster: I SEE YOU EVERYWHERE.</color>");
                break;
        }

        if (_monsterHUD != null)
        {
            int index = count - 1;

            // Check if we actually have text for this level in the Inspector
            if (_threatLevelNames != null && index >= 0 && index < _threatLevelNames.Count)
            {
                _monsterHUD.UpdateThreatDisplay(count, _threatLevelNames[index]);
            }
            else
            {
                // If the list is empty or missing this index, hide the HUD
                _monsterHUD.UpdateThreatDisplay(count, "");
            }
        }
    }
}