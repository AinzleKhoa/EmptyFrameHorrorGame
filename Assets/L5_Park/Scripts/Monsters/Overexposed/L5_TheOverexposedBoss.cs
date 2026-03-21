using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class L5_TheOverexposedBoss : MonoBehaviour
{
    // --- NEW: For the Manager to track him ---
    public static Action OnBossBanished;
    public bool IsManifested => gameObject.activeSelf;

    public enum MonsterState { Alerting, Chasing, Stunned }

    [Header("Status")]
    public MonsterState currentState = MonsterState.Alerting;

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

    private float _chaseSpeed = 10f;

    [Header("Jumpscare Setup")]
    [SerializeField] private Camera _jumpscareCamera;

    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;

    // --- NEW: Reference to the collider ---
    private Collider _myCollider;

    private Vector3 _lockPos;
    // 2. Add LateUpdate below your Update method
    private void LateUpdate()
    {
        // If the monster is stunned, we FORCE his position back to the anchor point
        // This happens AFTER animations and NavMesh try to move him.
        if (currentState == MonsterState.Stunned)
        {
            transform.position = _lockPos;
            agent.velocity = Vector3.zero;
        }
    }

    private void Awake()
    {
        // Get components in Awake to ensure they are ready for OnEnable
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        _myCollider = GetComponent<Collider>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    private void OnEnable()
    {
        // Stop any leftover logic from the last time he was active
        StopAllCoroutines();

        // --- NEW: Safety - Disable collider on spawn ---
        if (_myCollider != null) _myCollider.enabled = false;

        // Reset state to Alerting every single time he appears/reactivates
        currentState = MonsterState.Alerting;

        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
            agent.speed = _chaseSpeed;
        }

        // Start the scream loop immediately upon appearing
        StartCoroutine(AlertScream());
    }

    private void HandleMusic()
    {
        // If the monster is chasing, make sure the music is playing
        if (currentState == MonsterState.Chasing)
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
    void Update()
    {
        // SAFETY CHECK: If the agent is disabled (e.g., during jumpscare), skip all logic
        if (!agent.enabled) return;

        // 1. Always Sync Animation
        anim.SetFloat("Speed", agent.velocity.magnitude);

        HandleMusic();

        // 2. High-Priority State Lock (Screaming or Stunned = No thinking)
        if (currentState == MonsterState.Alerting || currentState == MonsterState.Stunned) return;

        // Always Chase
        if (currentState == MonsterState.Chasing)
        {
            CheckForLightExposure();
            ExecuteChasing();
        }
    }

    public void ForceDespawn()
    {
        Banish();
    }

    // --- NEW: Method for the Manager to call ---
    public void Manifest(Vector3 pos)
    {

        // If Awake hasn't run yet, we manually find the agent now
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        // Safety check: if it's STILL null, the component is missing from the prefab
        if (agent == null)
        {
            Debug.LogError($"<color=red>FATAL:</color> NavMeshAgent missing on {gameObject.name}!");
            return;
        }
        agent.enabled = false; // Disable to move him without NavMesh errors
        transform.position = pos;
        agent.enabled = true;

        gameObject.SetActive(true); // OnEnable handles the rest!
    }

    private void Banish()
    {
        Debug.Log("<color=red>Boss Banished by Light!</color>");
        OnBossBanished?.Invoke(); // Tell Manager to start countdown
        gameObject.SetActive(false); // He "dies" and turns off
    }

    #region State Executions
    private void ExecuteChasing()
    {
        agent.speed = _chaseSpeed;
        agent.isStopped = false; // Ensure he isn't stuck
        agent.SetDestination(player.position);
    }

    #endregion

    #region Perception Logic

    private void CheckForLightExposure()
    {
        if (currentState == MonsterState.Stunned) return;

        // 1. Scan for any colliders near the monster's chest
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + Vector3.up, 3.5f);

        foreach (var hit in hitColliders)
        {
            // 2. Look for the script on the hit object OR any of its parents
            CandleInteractable candle = hit.GetComponentInParent<CandleInteractable>();

            // 2. Check for the Candle OR check for the LightSource tag
            if ((candle != null && candle.IsLit) || hit.CompareTag("LightSource"))
            {
                Debug.Log("<color=yellow>Monster hit by light from: </color>" + hit.name);

                // 3. Trigger the banish
                TakeDamage();
                break;
            }
        }
    }

    #endregion

    #region Helper & Navigation Methods

    public void TakeDamage()
    {
        if (currentState == MonsterState.Stunned) return;

        // --- NEW: Disable collider immediately when hit ---
        if (_myCollider != null) _myCollider.enabled = false;

        Debug.Log("Monster hit by camera! Interrupting current state: " + currentState);

        // CAPTURE the exact spot where he was hit
        _lockPos = transform.position;

        // Switch state IMMEDIATELY so LateUpdate starts locking him
        currentState = MonsterState.Stunned;

        StopAllCoroutines();

        // Standard stops for safety
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.ResetPath();

        StartCoroutine(StunnedRoutine());
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

        // --- THE FIX: Only enable collider when Chasing starts ---
        if (_myCollider != null) _myCollider.enabled = true;

        agent.isStopped = false;
        currentState = MonsterState.Chasing;
    }

    IEnumerator StunnedRoutine()
    {
        currentState = MonsterState.Stunned;
        agent.isStopped = true;
        anim.SetTrigger("isHit");

        if (_stunnedClip != null)
        {
            _screamSource.Stop();
            _screamSource.PlayOneShot(_stunnedClip);
        }

        yield return new WaitForSeconds(2f);
        agent.isStopped = false;

        Banish();

        // Always scream after stunned
        // StartCoroutine(AlertScream());
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, 3f); // Stun radius
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

    public void SetChaseSpeed(float newSpeed)
    {
        _chaseSpeed = newSpeed;
        if (agent != null) agent.speed = _chaseSpeed;
    }
}