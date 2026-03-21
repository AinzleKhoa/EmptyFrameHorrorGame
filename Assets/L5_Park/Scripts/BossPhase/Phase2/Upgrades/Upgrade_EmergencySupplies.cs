using UnityEngine;

public class Upgrade_EmergencySupplies : MonoBehaviour, IUpgradeEffect
{
    [SerializeField] private string _upgradeName = "Emergency Supplies";
    [TextArea][SerializeField] private string _upgradeLore = "Increases the active candle count from 4 to 8 per spawn cycle.";

    public void ExecuteUpgrade(PickableItem target)
    {
        // 1. Apply functional effect to the Manager
        if (L5_CandleManager.Instance != null)
        {
            L5_CandleManager.Instance.UnlockEmergencySupplies();
        }

        // 2. Record the upgrade in PlayerData for status tracking
        if (target.TryGetComponent<ItemData>(out var data))
        {
            data.AddUpgrade($"{_upgradeName}: {_upgradeLore}");
        }

        Debug.Log($"<color=green>Upgrade Applied:</color> {_upgradeName} is now active.");
    }
}