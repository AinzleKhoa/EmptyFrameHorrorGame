using UnityEngine;
using UnityEngine.Events;

public class L3_ShadowNPC : InteractableBase
{
    public static bool PlayerHasPencil = false;

    [Header("NPC Dialogues")]
    [TextArea(3, 5)]
    public string MissingItemMessage = "TheShadow: I dropped it... I dropped my pencil... I can't finish the test without it. The Monitor is coming for me!\nMinh: Calm down. I'll find it for you.";

    [TextArea(2, 3)]
    public string SuccessMessage = "TheShadow: Thank you... but it's too late. The bell has rung.";

    [Header("Twist Events (Chuông, Mở cửa, Thả quái)")]
    public UnityEvent OnItemDelivered;

    public override string PromptMessage => "Press [E] to Talk";

    public override void Interact(PlayerInteractor interactor)
    {
        if (PlayerHasPencil)
        {
            // Trả bút -> Báo thành công, kích hoạt Twist
            GameBroadcast.OnUpdateItemDescription?.Invoke($"<color=#00FF00>{SuccessMessage}</color>");
            OnItemDelivered?.Invoke();
            Destroy(gameObject, 2f);
        }
        else
        {
            // Chưa có bút -> Hiện hội thoại
            GameBroadcast.OnUpdateItemDescription?.Invoke($"<color=#FFD700>{MissingItemMessage}</color>");
        }

        // Gọi Signal (Sig2_L2_TalkToShadow) để báo cho StoryDirector chuyển sang Act tìm bút chì
        base.Interact(interactor);
    }
}