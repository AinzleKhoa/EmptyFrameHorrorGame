using UnityEngine;

public class L2_StudentCardManager : MonoBehaviour
{
    public static L2_StudentCardManager Instance;

    [Header("Status")]
    public int CardCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void AddCard()
    {
        CardCount++;
        GameBroadcast.OnUpdateItemDescription?.Invoke($"<color=#00FF00>+1 Student ID!</color> (Total: {CardCount})");

        // In ra Console để sếp dễ kiểm tra
        Debug.Log("ĐÃ NHẶT THẺ! Số lượng hiện tại: " + CardCount);
    }

    public bool ConsumeCard()
    {
        if (CardCount > 0)
        {
            CardCount--;
            GameBroadcast.OnUpdateItemDescription?.Invoke($"<color=yellow>Monster Attack! -1 ID!</color> (Left: {CardCount})");

            // In ra Console khi bị quái vả
            Debug.Log("QUÁI CẮN! Đã dùng 1 thẻ cứu mạng. Thẻ còn lại: " + CardCount);
            return true;
        }
        return false;
    }
}