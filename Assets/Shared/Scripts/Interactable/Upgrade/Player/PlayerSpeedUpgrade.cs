using UnityEngine;
using System.Reflection; // Bắt buộc phải có để dùng Reflection

public class PlayerSpeedUpgrade : MonoBehaviour, IUpgradeEffect
{
    [Header("Upgrade Identity")]
    [SerializeField] private string _upgradeName = "Overclocked Servos";
    [SerializeField] private string _upgradeLore = "Can thiệp vào hệ thống motor để tăng tốc độ di chuyển.";

    [Header("Values")]
    [SerializeField] private float _walkBoost = 2f;
    [SerializeField] private float _sprintBoost = 3f;

    public void ExecuteUpgrade(PickableItem target)
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return;

        // 1. Lưu vào PlayerData (Giống script cũ của bạn)
        if (playerObj.TryGetComponent<PlayerData>(out var data))
        {
            data.AddPermanentUpgrade($"{_upgradeName}: {_upgradeLore}");
        }

        // 2. Dùng Reflection để "hack" vào PlayerMovement
        if (playerObj.TryGetComponent<PlayerMovement>(out var movement))
        {
            ApplySpeedHack(movement);
        }
    }

    private void ApplySpeedHack(PlayerMovement movement)
    {
        // Lấy thông tin về các biến private từ class PlayerMovement
        FieldInfo walkField = typeof(PlayerMovement).GetField("_walkSpeed", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo sprintField = typeof(PlayerMovement).GetField("_sprintSpeed", BindingFlags.Instance | BindingFlags.NonPublic);

        if (walkField != null && sprintField != null)
        {
            // Đọc giá trị hiện tại
            float currentWalk = (float)walkField.GetValue(movement);
            float currentSprint = (float)sprintField.GetValue(movement);

            // Ghi đè giá trị mới mà không cần hàm public
            walkField.SetValue(movement, currentWalk + _walkBoost);
            sprintField.SetValue(movement, currentSprint + _sprintBoost);

            Debug.Log($"<color=cyan>Reflection Upgrade:</color> Walk {currentWalk + _walkBoost}, Sprint {currentSprint + _sprintBoost}");
        }
    }
}