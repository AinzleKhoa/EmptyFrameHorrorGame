using UnityEngine;

public class L2_DialogueSwapper : MonoBehaviour
{
    [Header("Kho Kịch Bản Của NPC Này")]
    public ScriptableObject KichBan_Lan1; // Kéo file thoại lúc mới gặp vào đây
    public ScriptableObject KichBan_Ending; // Kéo file thoại nhặt 4 mảnh vào đây

    [Header("Kết nối với Quản lý Mê cung")]
    public L2_MazeProgression MazeManager; // Kéo cái cục SceneProgressionManager vào đây

    // ========================================================
    // CHÚ Ý: ĐỔI TÊN 'TenScriptSharedCuaBan' THÀNH TÊN SCRIPT
    // TƯƠNG TÁC SHARED MÀ BẠN ĐANG GẮN TRÊN CON NPC
    // ========================================================
    // private TenScriptSharedCuaBan _sharedScript; 

    private void Start()
    {
        // Lấy cái script Shared đang nằm trên người con NPC này
        // _sharedScript = GetComponent<TenScriptSharedCuaBan>();

        // Tự động tìm hệ thống quản lý mảnh vỡ nếu bạn quên kéo
        if (MazeManager == null)
        {
            MazeManager = FindObjectOfType<L2_MazeProgression>();
        }
    }

    private void Update()
    {
        // Kiểm tra xem đã có script Shared và Quản lý Mê cung chưa
        // if (_sharedScript == null || MazeManager == null) return;

        // LUẬT TRÁO KỊCH BẢN:
        if (MazeManager.FragmentsCollected >= 4)
        {
            // Nhét kịch bản Ending vào tay script Shared
            // _sharedScript.dialogueData = KichBan_Ending; 
        }
        else
        {
            // Nhét kịch bản Lần 1 vào tay script Shared
            // _sharedScript.dialogueData = KichBan_Lan1;
        }
    }
}