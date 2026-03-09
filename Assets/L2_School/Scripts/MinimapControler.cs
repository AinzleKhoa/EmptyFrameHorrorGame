using UnityEngine;
using UnityEngine.InputSystem; // Thư viện cực kỳ quan trọng cho Input mới

public class MinimapController : MonoBehaviour
{
    [Header("Kéo cái UI_Minimap vào đây:")]
    public GameObject minimapUI;

    void Start()
    {
        // Tắt minimap lúc mới vào game
        if (minimapUI != null)
        {
            minimapUI.SetActive(false);
        }
    }

    void Update()
    {
        // Kiểm tra xem bàn phím có đang cắm không và phím M có được bấm không (Cách mới)
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
        {
            if (minimapUI != null)
            {
                // Đảo ngược trạng thái
                minimapUI.SetActive(!minimapUI.activeSelf);
            }
        }
    }
}