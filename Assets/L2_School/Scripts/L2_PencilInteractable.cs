using UnityEngine;

public class L2_PencilInteractable : InteractableBase
{
    public override string PromptMessage => "Press [E] to Pick Up Broken Pencil";

    public override void Interact(PlayerInteractor interactor)
    {
        // 1. Báo cho con NPC Bóng Đen biết là "Tao nhặt được bút rồi nhé!"
        L2_ShadowNPC.PlayerHasPencil = true;

        // 2. Hiện thông báo lên màn hình
        GameBroadcast.OnUpdateItemDescription?.Invoke("Collected: Broken Pencil");

        // 3. Biến mất khỏi bản đồ
        Destroy(gameObject);

        // 4. Kích hoạt Signal để hoàn thành Act 2 trong StoryDirector
        base.Interact(interactor);
    }
}