using UnityEngine;

public class PlayerStaminaUpgrade : MonoBehaviour, IUpgradeEffect
{
    [Header("Stamina Settings")]
    [SerializeField] private float _extraMaxStamina = 100f;
    [SerializeField] private float _extraRegenRate = 15f;
    [Header("Upgrade Identity")]
    [SerializeField] private string _upgradeName = "Adrenaline Shot";
    [SerializeField] private string _upgradeLore = "Permanent Stamina Boost.";

    public void ExecuteUpgrade(PickableItem target)
    {
        GameObject playerObj = GameObject.FindWithTag("Player");

        // 1. FIXED: Look for PlayerData (not ItemData)
        if (playerObj.TryGetComponent<PlayerData>(out var data))
        {
            // 2. FIXED: Call the correct method name 'AddPermanentUpgrade'
            data.AddPermanentUpgrade($"{_upgradeName}: {_upgradeLore}");
        }

        if (playerObj.TryGetComponent<PlayerMovement>(out var movement))
        {
            movement.UpgradeMaxStamina(_extraMaxStamina);
            movement.UpgradeRegenRate(_extraRegenRate);
        }
    }
}