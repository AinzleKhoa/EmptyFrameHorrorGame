using UnityEngine;

public class L2_FragmentSpy : MonoBehaviour
{
    private bool _isShuttingDown = false;

    // Hàm này chạy khi bạn tắt game (để tránh lỗi)
    private void OnApplicationQuit()
    {
        _isShuttingDown = true;
    }

    // Hàm này tự động chạy ngay khoảnh khắc Object này bị xóa (bị người chơi nhặt)
    private void OnDestroy()
    {
        // Kiểm tra để chắc chắn đây là do người chơi nhặt, chứ không phải do đang chuyển cảnh hay tắt game
        if (_isShuttingDown || !gameObject.scene.isLoaded) return;

        // Báo cáo cho ông trùm!
        L2_MazeProgression manager = FindObjectOfType<L2_MazeProgression>();
        if (manager != null)
        {
            manager.OnFragmentPickedUp();
        }
    }
}