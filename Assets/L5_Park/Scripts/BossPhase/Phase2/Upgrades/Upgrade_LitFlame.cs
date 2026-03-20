using UnityEngine;

public class Upgrade_LitFlame : MonoBehaviour, IUpgradeEffect
{
    [SerializeField] private string _upgradeName = "Lit Flame";
    [TextArea][SerializeField] private string _upgradeLore = "Candles are pre-ignited upon manifestation, requiring no manual lighting.";

    public void ExecuteUpgrade(PickableItem target)
    {
        // 1. Apply functional effect to the Manager
        if (L5_CandleManager.Instance != null)
        {
            L5_CandleManager.Instance.UnlockLitFlame();
        }

        // 2. Record the upgrade in PlayerData for status tracking
        if (target.TryGetComponent<ItemData>(out var data))
        {
            data.AddUpgrade($"{_upgradeName}: {_upgradeLore}");
        }

        Debug.Log($"<color=green>Upgrade Applied:</color> {_upgradeName} is now active.");
    }
}