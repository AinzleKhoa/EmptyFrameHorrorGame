using UnityEngine;
using System.Collections.Generic;

public class L5_ShadowManager : MonoBehaviour
{
    [SerializeField] private L5_ShadowMonster _shadowPrefab;
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private float _minSpawnInterval = 20f;
    [SerializeField] private float _maxSpawnInterval = 40f;

    private float _currentSpawnTimer;
    private L5_ShadowMonster _activeShadow;

    private void OnEnable() => L5_ShadowMonster.OnShadowBanished += PrepareNextSpawn;
    private void OnDisable() => L5_ShadowMonster.OnShadowBanished -= PrepareNextSpawn;

    private void Start()
    {
        _activeShadow = Instantiate(_shadowPrefab);
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
        }

        Debug.Log("<color=green>Phase 1 Manager:</color> Shutdown complete. Shadow destroyed.");
    }
}