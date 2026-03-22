using UnityEngine;

public class L2_CardPickup : InteractableBase
{
    public override string PromptMessage => "Press [E] to Pick Up Student ID";

    public override void Interact(PlayerInteractor interactor)
    {
        // 👉 ĐÃ FIX TỐI THƯỢNG: Trực tiếp nhét thẻ vào tay ông Quản Lý! Chắc chắn 100% tăng điểm.
        if (L2_StudentCardManager.Instance != null)
        {
            L2_StudentCardManager.Instance.AddCard();
        }
        else
        {
            Debug.LogError("LỖI: Chưa có Manager_StudentCard trên Scene!");
        }

        // Nhặt xong thì tự hủy cái thẻ rớt trên mặt đất
        Destroy(gameObject);

        base.Interact(interactor);
    }
}