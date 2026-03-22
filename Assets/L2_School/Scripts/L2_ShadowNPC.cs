using UnityEngine;
using UnityEngine.Events;

public class L2_ShadowNPC : InteractableBase
{
    public static bool PlayerHasPencil = false;

    [Header("Giai đoạn 1: Thoại tìm Bút Chì")]
    [TextArea(3, 5)]
    public string MissingItemMessage = "TheShadow: I dropped it... I dropped my pencil... I can't finish the test without it. The Monitor is coming for me!\nMinh: Calm down. I'll find it for you.";

    [TextArea(2, 3)]
    public string SuccessMessage = "TheShadow: Thank you... but it's too late. The bell has rung.";

    [Header("Giai đoạn 2: Thoại Kết Thúc (Đủ 4 mảnh)")]
    [Tooltip("Mỗi Element là 1 lần bấm phím E")]
    [TextArea(2, 4)]
    public string[] FinalEndingMessages = new string[] {
        "Minh: I've collected all 4 fragments... What the hell are you?",
        "TheShadow: I am not a monster. I am the truth you are always running from."
    };

    [Header("Twist Events (Chuông, Mở cửa, Thả quái)")]
    public UnityEvent OnItemDelivered;

    [Header("Sự Kiện Kết Thúc Game")]
    public UnityEvent OnFinalDialogueFinished;

    private int _currentDialogueIndex = 0;

    public override string PromptMessage => "Press [E] to Talk";

    public override void Interact(PlayerInteractor interactor)
    {
        L2_MazeProgression mazeManager = FindObjectOfType<L2_MazeProgression>();
        int currentFragments = (mazeManager != null) ? mazeManager.FragmentsCollected : 0;

        // --- GIAI ĐOẠN 2: LÚC KẾT THÚC (4 Mảnh) ---
        if (currentFragments >= 4)
        {
            if (_currentDialogueIndex < FinalEndingMessages.Length)
            {
                GameBroadcast.OnUpdateItemDescription?.Invoke($"<color=#00FFFF>{FinalEndingMessages[_currentDialogueIndex]}</color>");
                _currentDialogueIndex++;
            }
            else
            {
                GameBroadcast.OnUpdateItemDescription?.Invoke("");
                OnFinalDialogueFinished?.Invoke();

                // 👉 ĐÃ FIX: Nói xong thì hiện ra mảnh vỡ thứ 5 để end game!
                if (mazeManager != null) mazeManager.ActivateFragment5();
            }
        }

        // --- GIAI ĐOẠN 1: LÚC MỚI VÀO GAME (0-3 Mảnh) ---
        else
        {
            if (PlayerHasPencil)
            {
                GameBroadcast.OnUpdateItemDescription?.Invoke($"<color=#00FF00>{SuccessMessage}</color>");
                OnItemDelivered?.Invoke();

                // 👉 ĐÃ FIX: Trả bút chì xong thì 4 mảnh vỡ mới chịu hiện hình
                if (mazeManager != null) mazeManager.ActivateInitialFragments();

                gameObject.SetActive(false);
            }
            else
            {
                GameBroadcast.OnUpdateItemDescription?.Invoke($"<color=#FFD700>{MissingItemMessage}</color>");
            }

            base.Interact(interactor);
        }
    }
}