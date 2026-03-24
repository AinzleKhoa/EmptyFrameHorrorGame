using UnityEngine;

public class L3_FinalShadowNPC : InteractableBase
{
    [Header("Câu thoại cuối cùng")]
    [TextArea(3, 5)]
    public string FinalMessage = "Shadow: You found them... But it's too late! IT IS HERE. RUN TO THE EXIT!";

    [Header("Bộ điều khiển tẩu thoát")]
    [Tooltip("Kéo L3_EscapeController vào đây")]
    public HospitalEscapeController EscapeController;

    public override string PromptMessage => "Press [E] to Talk to Shadow";

    public override void Interact(PlayerInteractor interactor)
    {
        // 1. Hiện câu thoại cảnh báo
        GameBroadcast.OnUpdateItemDescription?.Invoke($"<color=#FF0000>{FinalMessage}</color>");

        // 2. Kích hoạt Đèn đỏ, thả quái, mở khóa cửa chính!
        if (EscapeController != null)
        {
            EscapeController.HandleFinalShadowDialogueFinished();
        }

        // 3. Xóa NPC này đi (tạo cảm giác Shadow đã biến mất)
        Destroy(gameObject, 2f);

        base.Interact(interactor);
    }
}