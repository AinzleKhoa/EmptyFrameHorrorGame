using UnityEngine;
using UnityEngine.Events;

public class L2_ShadowNPC : InteractableBase
{
    public static bool PlayerHasPencil = false;

    [Header("Giai đoạn 1: Thoại tìm Bút Chì")]
    [TextArea(3, 5)]
    public string MissingItemMessage = "TheShadow: I dropped it... I dropped my pencil... I can't finish the test without it.\nMinh: Calm down. I'll find it for you.";

    [TextArea(2, 3)]
    public string SuccessMessage = "TheShadow: Thank you... but it's too late. The bell has rung.";

    [Header("Giai đoạn 2: Thoại Kết Thúc (Đủ 4 mảnh)")]
    [TextArea(2, 4)]
    public string[] FinalEndingMessages = new string[] {
        "Minh: I've collected all 4 fragments... What are you?",
        "TheShadow: I am the truth you are always running from."
    };

    public UnityEvent OnItemDelivered;
    public UnityEvent OnFinalDialogueFinished;

    private int _currentDialogueIndex = 0;
    public override string PromptMessage => "Press [E] to Talk";

    // --- BIẾN CHO HỆ THỐNG GIAO DIỆN TỰ CHẾ ---
    private string _currentDialogueText = "";
    private float _dialogueTimer = 0f;

    public override void Interact(PlayerInteractor interactor)
    {
        L2_MazeProgression mazeManager = FindObjectOfType<L2_MazeProgression>();
        int currentFragments = (mazeManager != null) ? mazeManager.FragmentsCollected : 0;

        if (currentFragments >= 4)
        {
            if (_currentDialogueIndex < FinalEndingMessages.Length)
            {
                ShowIndependentDialogue(FinalEndingMessages[_currentDialogueIndex]);
                _currentDialogueIndex++;
            }
            else
            {
                _currentDialogueText = ""; // Ẩn thoại
                OnFinalDialogueFinished?.Invoke();
                if (mazeManager != null) mazeManager.ActivateFragment5();
            }
        }
        else
        {
            if (PlayerHasPencil)
            {
                ShowIndependentDialogue(SuccessMessage);
                OnItemDelivered?.Invoke();
                if (mazeManager != null) mazeManager.ActivateInitialFragments();
                Invoke("DisableNPC", 4f); // Tạm chờ 4 giây cho Minh đọc chữ rồi mới tắt NPC
            }
            else
            {
                ShowIndependentDialogue(MissingItemMessage);
            }
            base.Interact(interactor);
        }
    }

    private void DisableNPC() { gameObject.SetActive(false); }

    private void ShowIndependentDialogue(string text)
    {
        _currentDialogueText = text;
        _dialogueTimer = 6f; // Hiện chữ trong 6 giây rồi tự tắt
    }

    private void Update()
    {
        if (_dialogueTimer > 0)
        {
            _dialogueTimer -= Time.deltaTime;
            if (_dialogueTimer <= 0) _currentDialogueText = "";
        }
    }

    // 👉 ĐÂY LÀ CHÂN ÁI: Tự vẽ khung thoại siêu đẹp không cần mượn đồ của team
    private void OnGUI()
    {
        if (string.IsNullOrEmpty(_currentDialogueText)) return;

        int boxWidth = 800;
        int boxHeight = 150;
        Rect boxRect = new Rect(Screen.width / 2 - boxWidth / 2, Screen.height - 200, boxWidth, boxHeight);

        // Nền đen mờ siêu ngầu
        Texture2D blackTex = new Texture2D(1, 1);
        blackTex.SetPixel(0, 0, new Color(0, 0, 0, 0.8f));
        blackTex.Apply();
        GUI.DrawTexture(boxRect, blackTex);

        // Định dạng chữ
        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.white;
        style.wordWrap = true;
        style.alignment = TextAnchor.UpperLeft;
        style.padding = new RectOffset(20, 20, 20, 20);

        GUI.Label(boxRect, _currentDialogueText, style);
    }
}