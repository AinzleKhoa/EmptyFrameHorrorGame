using UnityEngine;
using System; // Required for Action

public class L5_ShadowMonster : MonoBehaviour
{
    public static L5_ShadowMonster Instance { get; private set; }
    public static Action OnShadowBanished; // The signal for the Manager

    public enum ShadowState { Idle, Manifested }
    [SerializeField] private ShadowState _currentState = ShadowState.Idle;
    public bool IsManifested => _currentState == ShadowState.Manifested;

    private float _timeToDamage = 30f;
    private float _dwellTimer;
    private Renderer[] _renderers;
    private Collider[] _colliders;

    private void Awake()
    {
        Instance = this;
        _renderers = GetComponentsInChildren<Renderer>();
        _colliders = GetComponentsInChildren<Collider>();
    }

    private void Start() => SetState(ShadowState.Idle);

    private void Update()
    {
        if (_currentState == ShadowState.Manifested)
        {
            _dwellTimer += Time.deltaTime;
            if (_dwellTimer >= _timeToDamage)
            {
                Debug.Log("<color=red>Shadow Drained Health!</color>");
                _dwellTimer = 0;
            }
        }
    }

    public void Manifest(Vector3 position)
    {
        transform.position = position;
        SetState(ShadowState.Manifested);
    }

    public void TakeFlash()
    {
        if (_currentState == ShadowState.Manifested)
        {
            SetState(ShadowState.Idle);
            // --- THE TRIGGER ---
            OnShadowBanished?.Invoke();
            Debug.Log("<color=orange>Shadow Banished!</color>");
        }
    }

    private void SetState(ShadowState newState)
    {
        _currentState = newState;
        bool active = (newState == ShadowState.Manifested);
        foreach (var r in _renderers) r.enabled = active;
        foreach (var c in _colliders) c.enabled = active;
        L5_GameBroadcast.OnL5ShadowSpawned?.Invoke(active);
        _dwellTimer = 0;
    }

    public void SetDamageInterval(float newTime)
    {
        _timeToDamage = newTime;
    }
}