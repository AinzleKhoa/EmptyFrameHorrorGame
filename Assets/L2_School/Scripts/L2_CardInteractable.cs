using UnityEngine;

public class L2_CardInteractable : InteractableBase
{
    public override string PromptMessage => "Press [E] to Collect Student ID";

    public override void Interact(PlayerInteractor interactor)
    {
        // 1. Cộng vào bộ đếm
        if (L2_StudentCardManager.Instance != null)
        {
            L2_StudentCardManager.Instance.AddCard();
        }

        // 2. Biến mất khỏi map
        Destroy(gameObject);

        // 3. Vẫn gọi base để kích hoạt SignalTrigger nếu có
        base.Interact(interactor);
    }
}