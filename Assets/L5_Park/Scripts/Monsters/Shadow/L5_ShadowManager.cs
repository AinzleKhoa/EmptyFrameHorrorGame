using UnityEngine;
using System.Collections.Generic;

public class L5_ShadowManager : MonoBehaviour
{
    [SerializeField] private L5_ShadowMonster _shadowPrefab;
    private static L5_ShadowMonster _sharedShadow; // Static so all phase managers share ONE monster
    [SerializeField] private List<Transform> _spawnPoints;

    [Header("Difficulty Settings")]
    [SerializeField] private float _shadowDamageTime = 10f;
    [SerializeField] private float _minSpawnInterval = 20f;
    [SerializeField] private float _maxSpawnInterval = 40f;

    private float _currentSpawnTimer;
    private L5_ShadowMonster _activeShadow;

    private void OnEnable() => L5_ShadowMonster.OnShadowBanished += PrepareNextSpawn;
    private void OnDisable() => L5_ShadowMonster.OnShadowBanished -= PrepareNextSpawn;

    private void Start()
    {
        // THE FIX: Only instantiate if a shadow doesn't exist in the scene yet
        if (_sharedShadow == null)
        {
            _sharedShadow = Instantiate(_shadowPrefab);
            Debug.Log("<color=cyan>Manager:</color> Global Shadow Created.");
        }
        // Link this specific phase manager to the shared monster
        _activeShadow = _sharedShadow;
        _activeShadow.SetDamageInterval(_shadowDamageTime);
        PrepareNextSpawn(); // Starts the very first countdown
        Debug.Log("<color=cyan>Manager:</color> Initialized.");
    }

    private void Update()
    {
        // Only count down if shadow is NOT here AND timer hasn't hit 0 yet
        if (!_activeShadow.IsManifested && _currentSpawnTimer > 0)
        {
            _currentSpawnTimer -= Time.deltaTime;

            if (_currentSpawnTimer <= 0)
            {
                _currentSpawnTimer = 0; // Lock it at zero
                SpawnShadow();
            }
        }
    }

    private void SpawnShadow()
    {
        if (_spawnPoints.Count == 0) return;
        int randomIndex = Random.Range(0, _spawnPoints.Count);

        // Refresh the stat in case you changed it in the inspector during play
        _activeShadow.SetDamageInterval(_shadowDamageTime);
        _activeShadow.Manifest(_spawnPoints[randomIndex].position);

        // NOTE: We do NOT call PrepareNextSpawn here. 
        // The timer stays at 0 while the shadow is active.
        Debug.Log($"<color=blue>Shadow SPAWNED.</color> Manager waiting for banishment...");
    }

    private void PrepareNextSpawn()
    {
        _currentSpawnTimer = Random.Range(_minSpawnInterval, _maxSpawnInterval);
        Debug.Log($"<color=white>Manager:</color> Countdown STARTED: {_currentSpawnTimer:F1}s");
    }

    public void CompletePhase()
    {
        // 1. Stop this script from running any more Update logic
        this.enabled = false;

        // 2. Clean up the monster
        if (_activeShadow != null)
        {
            // Force it to hide/stop damage immediately
            _activeShadow.TakeFlash();
            Destroy(_activeShadow.gameObject, 0.5f);
        }

        Debug.Log("<color=green>Phase 1 Manager:</color> Shutdown complete. Shadow destroyed.");
    }

    public static void ResetSharedShadow() => _sharedShadow = null;
}