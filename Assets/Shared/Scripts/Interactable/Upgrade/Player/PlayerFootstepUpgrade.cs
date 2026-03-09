using UnityEngine;

public class PlayerFootstepUpgrade : MonoBehaviour, IUpgradeEffect
{
    [Header("Upgrade Identity")]
    [SerializeField] private string _upgradeName = "Soft-Soled Boots";
    [SerializeField] private string _upgradeLore = "Running and walking no longer emits noise.";

    public void ExecuteUpgrade(PickableItem target)
    {
        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
        {
            // 1. Update PlayerData Lore
            if (playerObj.TryGetComponent<PlayerData>(out var data))
            {
                data.AddPermanentUpgrade($"{_upgradeName}: {_upgradeLore}");
            }

            // 2. Unlock the silence in Movement
            if (playerObj.TryGetComponent<PlayerMovement>(out var movement))
            {
                movement.UnlockSilentFeet();
            }

            Debug.Log($"<color=cyan>Upgrade Applied:</color> {_upgradeName}");
        }
    }
}