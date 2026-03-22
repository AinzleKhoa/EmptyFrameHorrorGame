using UnityEngine;

public class L2_StudentCardManager : MonoBehaviour
{
    public static L2_StudentCardManager Instance; // Singleton để gọi ở mọi nơi cực nhanh

    [Header("Status")]
    public int CardCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void AddCard()
    {
        CardCount++;
        // Mượn tạm cái bảng mô tả vật phẩm của bạn để thông báo lên màn hình!
        GameBroadcast.OnUpdateItemDescription?.Invoke($"<color=yellow>Student ID Collected!</color>\nTotal IDs: {CardCount}");
    }

    // Hàm này sau này Quái vật và Bảng danh dự sẽ gọi để xài thẻ
    public bool ConsumeCard()
    {
        if (CardCount > 0)
        {
            CardCount--;
            GameBroadcast.OnUpdateItemDescription?.Invoke($"<color=red>-1 Student ID</color>\nRemaining IDs: {CardCount}");
            return true; // Trả về true tức là xài thành công (cứu 1 mạng)
        }
        return false; // Hết thẻ rồi!
    }
}