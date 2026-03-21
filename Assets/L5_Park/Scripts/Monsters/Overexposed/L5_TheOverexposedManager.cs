using UnityEngine;
using System.Collections.Generic;

public class L5_TheOverexposedManager : MonoBehaviour
{
    [SerializeField] private L5_TheOverexposedBoss _bossPrefab;
    private static L5_TheOverexposedBoss _sharedBoss;
    [SerializeField] private List<Transform> _spawnPoints;

    [Header("Difficulty Settings")]
    [SerializeField] private float _bossChaseSpeed = 7.5f;
    [SerializeField] private float _minSpawnInterval = 25f;
    [SerializeField] private float _maxSpawnInterval = 30f;

    private float _currentSpawnTimer;
    private L5_TheOverexposedBoss _activeBoss;

    // Listen for the Boss's specific banishment signal
    private void OnEnable() => L5_TheOverexposedBoss.OnBossBanished += PrepareNextSpawn;
    private void OnDisable() => L5_TheOverexposedBoss.OnBossBanished -= PrepareNextSpawn;

    private void Start()
    {
        if (_sharedBoss == null)
        {
            _sharedBoss = Instantiate(_bossPrefab);
            // Hide it immediately so the timer can take over
            _sharedBoss.gameObject.SetActive(false);
            Debug.Log("<color=red>Manager:</color> Global Boss Created.");
        }
        // 3. Link this specific phase manager to the shared boss
        _activeBoss = _sharedBoss;

        // Push speed early
        _activeBoss.SetChaseSpeed(_bossChaseSpeed);

        PrepareNextSpawn();
        Debug.Log("<color=yellow>Overexposed Manager:</color> Initialized.");
    }

    private void Update()
    {
        // Only count down if Boss is NOT manifested
        if (!_activeBoss.IsManifested && _currentSpawnTimer > 0)
        {
            _currentSpawnTimer -= Time.deltaTime;

            if (_currentSpawnTimer <= 0)
            {
                _currentSpawnTimer = 0;
                SpawnBoss();
            }
        }
    }

    private void SpawnBoss()
    {
        if (_spawnPoints.Count == 0) return;
        int randomIndex = Random.Range(0, _spawnPoints.Count);


        // Apply speed right before he manifests
        _activeBoss.SetChaseSpeed(_bossChaseSpeed);
        _activeBoss.Manifest(_spawnPoints[randomIndex].position);

        // --- ADDED: Spawn the candles when the boss appears ---
        if (L5_CandleManager.Instance != null)
            L5_CandleManager.Instance.SpawnCandleBatch();

        Debug.Log($"<color=red>BOSS SPAWNED:</color> Overexposed at {_spawnPoints[randomIndex].name}");
    }

    private void PrepareNextSpawn()
    {
        // --- ADDED: Clear any leftover candles when the boss is banished ---
        if (L5_CandleManager.Instance != null)
            L5_CandleManager.Instance.DeactivateAll();

        _currentSpawnTimer = Random.Range(_minSpawnInterval, _maxSpawnInterval);
        Debug.Log($"<color=white>Overexposed Manager:</color> Next appearance in {_currentSpawnTimer:F1}s");
    }

    public void CompletePhase()
    {
        this.enabled = false;
        if (_activeBoss != null)
        {
            _activeBoss.ForceDespawn();
            Destroy(_activeBoss.gameObject, 0.5f);
        }
    }

    public static void ResetSharedBoss() => _sharedBoss = null;
}