using UnityEngine;

public class L3_PhotoPickup : InteractableBase
{
    public override string PromptMessage => "Press [E] to Pick Up Photo";

    public override void Interact(PlayerInteractor interactor)
    {
        // 1. Chạy thẳng lên văn phòng ông Quản lý nộp ảnh (Không qua trung gian Signal)
        if (L3_PhotoProgression.Instance != null)
        {
            L3_PhotoProgression.Instance.OnPhotoPickedUp();
        }
        else
        {
            Debug.LogError("LỖI: Chưa có script L3_PhotoProgression trên Scene!");
        }

        // 2. Nộp xong thì tự hủy bức ảnh 3D
        Destroy(gameObject);

        base.Interact(interactor);
    }
}