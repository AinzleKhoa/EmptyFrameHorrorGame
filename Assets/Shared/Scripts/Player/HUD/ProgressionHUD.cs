using UnityEngine;
using TMPro;

public class ProgressionHUD : BaseHUD // Inherit from BaseHUD
{
    [SerializeField] private TextMeshProUGUI _hudObjectiveText;
    [SerializeField] private TextMeshProUGUI _hudProgressionCountText;
    [SerializeField] private TextMeshProUGUI _hudStageNameText;

    protected override void OnEnable()
    {
        // Use named methods instead of lambdas so we can unsubscribe!
        GameBroadcast.OnProgressionCountUpdateHUD += UpdateProgressionCount;
        GameBroadcast.OnStageNameUpdateHUD += UpdateStageName;
        GameBroadcast.OnObjectiveUpdateHUD += UpdateObjective;
    }

    protected override void OnDisable()
    {
        // CRITICAL: Clean up the references when the scene changes
        GameBroadcast.OnProgressionCountUpdateHUD -= UpdateProgressionCount;
        GameBroadcast.OnStageNameUpdateHUD -= UpdateStageName;
        GameBroadcast.OnObjectiveUpdateHUD -= UpdateObjective;
    }

    private void UpdateProgressionCount(int current, int total)
    {
        if (!IsValid || _hudProgressionCountText == null) return;

        // Called from FragmentProgressionCounter, if -1 -1 => A stage requires no fragment collecting
        if (current == -1 && total == -1)
        {
            _hudProgressionCountText.text = "?/?";
        }
        else
        {
            _hudProgressionCountText.text = $"{current}/{total}";
        }
    }

    private void UpdateStageName(string name)
    {
        if (!IsValid || _hudStageNameText == null) return;
        _hudStageNameText.text = name;
    }

    private void UpdateObjective(string obj)
    {
        if (!IsValid || _hudObjectiveText == null) return;
        _hudObjectiveText.text = obj;
    }
}