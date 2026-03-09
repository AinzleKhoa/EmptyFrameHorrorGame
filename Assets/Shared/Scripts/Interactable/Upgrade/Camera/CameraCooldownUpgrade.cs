using UnityEngine;

[RequireComponent(typeof(UpgradeInteractable))]
public class CameraCooldownUpgrade : MonoBehaviour, IUpgradeEffect
{
    [Header("Settings")]
    [Tooltip("2 will halve the time, 3 will cut it to a third.")]
    [SerializeField] private float _divisionFactor = 2f;

    [Header("Upgrade Identity")]
    [SerializeField] private string _upgradeName = "Industrial Battery";
    [TextArea]
    [SerializeField] private string _upgradeLore = "Cooldown reduced to 15s.";

    public void ExecuteUpgrade(PickableItem target)
    {
        // 1. Update Lore
        if (target.TryGetComponent<ItemData>(out var data))
        {
            data.AddUpgrade($"{_upgradeName}: {_upgradeLore}");
            GameBroadcast.OnUpdateItemDescription?.Invoke(data.FullDescription);
        }

        // 2. Change the math on the Cooldown component
        if (target.TryGetComponent<ItemCooldown>(out var cooldown))
        {
            cooldown.ReduceCooldownByFactor(_divisionFactor);
        }

        Debug.Log($"<color=yellow>Upgrade Applied:</color> {_upgradeName} on {target.gameObject.name}");
    }
}