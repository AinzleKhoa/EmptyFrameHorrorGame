using UnityEngine;
using System.Reflection;

public class Upgrade_FisheyeOptic : MonoBehaviour, IUpgradeEffect
{
    [Header("Upgrade Identity")]
    [SerializeField] private string _upgradeName = "Fisheye Optic";
    [TextArea][SerializeField] private string _upgradeLore = "Eliminates lens weight and triple detection reach.";

    [Header("Movement Settings")]
    [Tooltip("1.0 = 100% movement speed while aiming.")]
    [SerializeField] private float _speedWhileAiming = 1.0f;

    [Header("Camera Settings")]
    [SerializeField] private float _fovMultiplier = 1.25f;

    [Header("Shadow Settings")]
    [SerializeField] private float _shadowRangeMultiplier = 2.0f;

    public void ExecuteUpgrade(PickableItem target)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        // 1. SPEED: Remove or adjust the penalty
        if (player.TryGetComponent<PlayerMovement>(out var movement))
            SetField(movement, "_speedOnCamera", _speedWhileAiming, false);

        // 2. POLAROID: Expand FOV & Light
        if (target != null && target.TryGetComponent<PolaroidCamera>(out var camera))
        {
            SetField(camera, "_zoomFOV", _fovMultiplier, true);
        }

        // 3. RANGE: Multiply the detection range on the Shadow
        L5_OutlineShadowManager manager = Object.FindFirstObjectByType<L5_OutlineShadowManager>();
        if (manager != null)
        {
            SetField(manager, "_detectionRange", _shadowRangeMultiplier, true);
        }

        if (target.TryGetComponent<ItemData>(out var data))
        {
            data.AddUpgrade($"{_upgradeName}: {_upgradeLore}");
            GameBroadcast.OnUpdateItemDescription?.Invoke(data.FullDescription);
        }

        Debug.Log($"<color=cyan>{_upgradeName}:</color> Installed. Range x{_shadowRangeMultiplier}, Speed set to {_speedWhileAiming}.");
    }

    private void SetField(object obj, string name, object val, bool mult)
    {
        var f = obj.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        if (f == null) return;

        if (mult) f.SetValue(obj, (float)f.GetValue(obj) * (float)val);
        else f.SetValue(obj, val);
    }
}