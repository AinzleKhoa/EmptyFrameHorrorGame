using UnityEngine;

public class ItemCooldown : MonoBehaviour
{
    [SerializeField] private float _baseCooldown = 30f;
    private float _currentCooldown;
    private float _lastUseTime;

    // Helper for the Camera to check if it can click
    public bool IsReady => Time.time >= _lastUseTime + _currentCooldown;

    // Helper for the HUD
    public float Progress => Mathf.Clamp01((Time.time - _lastUseTime) / _currentCooldown);

    private void Awake()
    {
        _currentCooldown = _baseCooldown;
        // Start ready (negative time trick)
        _lastUseTime = -_currentCooldown;
    }

    public void Use() => _lastUseTime = Time.time;

    // Mathematical reduction (30 / 2 = 15)
    public void ReduceCooldownByFactor(float divisor)
    {
        _currentCooldown = Mathf.Max(1f, _currentCooldown / divisor);
        Debug.Log($"<color=cyan>Cooldown Factor Applied!</color> New: {_currentCooldown}s");
    }

    private void LateUpdate()
    {
        // Sole source of HUD updates for this item
        ItemData data = GetComponent<ItemData>();
        string itemName = (data != null) ? data.ItemName : gameObject.name;

        GameBroadcast.OnItemStatusUpdate?.Invoke(itemName, Progress);
    }

    public void setBaseCooldown(float cooldown)
    {
        _baseCooldown = cooldown;
        SyncCooldown(cooldown);
    }

    private void SyncCooldown(float value)
    {
        _currentCooldown = value;
        if (!IsReady) _lastUseTime = Time.time - _currentCooldown;
    }
}