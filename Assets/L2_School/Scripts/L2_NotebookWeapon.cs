using UnityEngine;
using UnityEngine.InputSystem; // Phải giữ cái này

public class L2_NotebookWeapon : MonoBehaviour
{
    [Header("Cài đặt Vũ Khí")]
    public int MaxUses = 6;
    public float CooldownTime = 15f;
    public float ThrowRange = 25f;

    [Header("Âm thanh & Hiệu ứng")]
    public AudioSource ThrowSound;

    private int _currentUses;
    private float _cooldownTimer = 0f;

    private void Start()
    {
        _currentUses = MaxUses;
    }

    private void Update()
    {
        // 1. Đếm ngược thời gian hồi chiêu
        if (_cooldownTimer > 0) _cooldownTimer -= Time.deltaTime;

        // 2. Kiểm tra chuột bằng cách đọc trực tiếp từ thiết bị (Chuẩn New Input System)
        if (Mouse.current == null) return;

        // Bấm CHUỘT PHẢI (giữ) để Nhắm
        bool isAiming = Mouse.current.rightButton.isPressed;

        // Bấm CHUỘT TRÁI (vừa nhấn xong) để Ném
        bool pressedThrow = Mouse.current.leftButton.wasPressedThisFrame;

        // 3. Logic: Phải GIỮ chuột phải VÀ BẤM chuột trái mới ném
        if (isAiming && pressedThrow)
        {
            TryThrowNotebook();
        }
    }

    private void TryThrowNotebook()
    {
        if (_currentUses <= 0)
        {
            GameBroadcast.OnUpdateItemDescription?.Invoke("<color=red>Out of Notebooks!</color>");
            return;
        }

        if (_cooldownTimer > 0)
        {
            GameBroadcast.OnUpdateItemDescription?.Invoke($"<color=yellow>Recharging: {_cooldownTimer:F1}s</color>");
            return;
        }

        // Bắt đầu ném
        _currentUses--;
        _cooldownTimer = CooldownTime;

        if (ThrowSound != null) ThrowSound.Play();
        GameBroadcast.OnUpdateItemDescription?.Invoke($"Book Thrown! ({_currentUses} left)");

        // Bắn tia Raycast
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, ThrowRange))
        {
            // Tìm con quái ở vật thể vừa trúng
            L2_MonsterAI monster = hit.collider.GetComponentInParent<L2_MonsterAI>();
            if (monster != null)
            {
                monster.ApplyFlashStun(); // Gọi hàm làm choáng của quái
                GameBroadcast.OnUpdateItemDescription?.Invoke("<color=green>MONSTER STUNNED!</color>");
            }
        }
    }
}