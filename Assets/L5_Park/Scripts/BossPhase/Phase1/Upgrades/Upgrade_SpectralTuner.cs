using UnityEngine;

public class Upgrade_SpectralTuner : MonoBehaviour, IUpgradeEffect
{
    [SerializeField] private string _upgradeName = "Spectral Tuner";
    [TextArea][SerializeField] private string _upgradeLore = "Allows the naked eye to perceive spectral silhouettes.";

    public void ExecuteUpgrade(PickableItem target)
    {
        // Simple, clean, and works even if no shadow is spawned yet!
        L5_OutlineShadowManager.IsPassiveEnabled = true;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return;
        if (playerObj.TryGetComponent<PlayerData>(out var data))
        {
            data.AddPermanentUpgrade($"{_upgradeName}: {_upgradeLore}");
        }

        Debug.Log("<color=purple>Upgrade Applied:</color> Spectral Vision unlocked.");
    }
}