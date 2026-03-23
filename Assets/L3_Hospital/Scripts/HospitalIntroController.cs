using UnityEngine;

public class HospitalIntroController : MonoBehaviour
{
    [Header("Movement gate")]
    [SerializeField] private Collider[] _blockersToKeepEnabledBeforeIntroDialogue;

    [Header("Optional guide paper")]
    [TextArea(3, 8)]
    [SerializeField] private string _guidePaperText =
        "MỤC TIÊU MÀN 3:\n- Thu thập đủ 3 trang nhật ký.\n- Dùng Camera để làm choáng quái trong 20 giây.\n- Có thể mở lại giấy này nếu quên cách chơi.\n\nGợi ý: Chúng sợ đèn flash. Nếu sợ bóng tối, hãy nhớ mang theo đèn pin.";

    [SerializeField] private bool _showGuidePaperWhenIntroEnds = true;

    [Header("Optional objective text")]
    [SerializeField] private string _firstObjective = "Nói chuyện với Shadow và chuẩn bị vật phẩm.";
    [SerializeField] private string _afterDialogueObjective = "Tìm 3 trang nhật ký trong bệnh viện.";

    private bool _introFinished;

    private void Start()
    {
        SetBlockers(true);
        if (!string.IsNullOrWhiteSpace(_firstObjective))
            GameBroadcast.OnObjectiveUpdateHUD?.Invoke(_firstObjective);
    }

    public void HandleIntroDialogueFinished()
    {
        if (_introFinished) return;
        _introFinished = true;

        SetBlockers(false);

        if (_showGuidePaperWhenIntroEnds)
        {
            GameBroadcast.OnShowReadableContent?.Invoke(_guidePaperText, true);
        }

        if (!string.IsNullOrWhiteSpace(_afterDialogueObjective))
            GameBroadcast.OnObjectiveUpdateHUD?.Invoke(_afterDialogueObjective);
    }

    private void SetBlockers(bool value)
    {
        if (_blockersToKeepEnabledBeforeIntroDialogue == null) return;
        foreach (var c in _blockersToKeepEnabledBeforeIntroDialogue)
        {
            if (c != null) c.enabled = value;
        }
    }
}
