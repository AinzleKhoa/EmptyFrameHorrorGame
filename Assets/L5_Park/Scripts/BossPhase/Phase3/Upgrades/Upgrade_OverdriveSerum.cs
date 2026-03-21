using UnityEngine;
using System.Reflection; // Required for Reflection logic

public class Upgrade_OverdriveSerum : MonoBehaviour, IUpgradeEffect
{
    [Header("Upgrade Identity")]
    [SerializeField] private string _upgradeName = "Overdrive Serum";
    [TextArea]
    [SerializeField] private string _upgradeLore = "Permanently increases walking and sprinting velocity, stamina capacity and regeneration rate.";

    [Header("Speed Boost Settings")]
    [SerializeField] private float _walkBoost = 11f;
    [SerializeField] private float _sprintBoost = 2f;

    [Header("Stamina Settings")]
    [SerializeField] private float _extraMaxStamina = 100f;
    [SerializeField] private float _extraRegenRate = 15f;

    public void ExecuteUpgrade(PickableItem target)
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return;

        // 1. Record the permanent upgrade in PlayerData
        if (playerObj.TryGetComponent<PlayerData>(out var data))
        {
            data.AddPermanentUpgrade($"{_upgradeName}: {_upgradeLore}");
        }

        // 2. Apply enhancements to PlayerMovement
        if (playerObj.TryGetComponent<PlayerMovement>(out var movement))
        {
            // Apply Speed via Reflection
            ApplySpeedHack(movement);

            // Apply Stamina via existing public methods
            movement.UpgradeMaxStamina(_extraMaxStamina);
            movement.UpgradeRegenRate(_extraRegenRate);
        }

        // Deactivate the syringe after use
        gameObject.SetActive(false);
    }

    private void ApplySpeedHack(PlayerMovement movement)
    {
        // Target the private fields in your PlayerMovement script
        FieldInfo walkField = typeof(PlayerMovement).GetField("_walkSpeed", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo sprintField = typeof(PlayerMovement).GetField("_sprintSpeed", BindingFlags.Instance | BindingFlags.NonPublic);

        if (walkField != null && sprintField != null)
        {
            // Get current values
            float currentWalk = (float)walkField.GetValue(movement);
            float currentSprint = (float)sprintField.GetValue(movement);

            // Set new values
            walkField.SetValue(movement, currentWalk + _walkBoost);
            sprintField.SetValue(movement, currentSprint + _sprintBoost);

            Debug.Log($"<color=cyan>Vascular Overdrive:</color> System saturation complete. Speed and Stamina increased.");
        }
    }
}