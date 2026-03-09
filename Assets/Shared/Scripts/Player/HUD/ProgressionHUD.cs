using UnityEngine;
using TMPro;

public class ProgressionHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hudObjectiveText;
    [SerializeField] private TextMeshProUGUI _hudProgressionCountText;
    [SerializeField] private TextMeshProUGUI _hudStageNameText;

    private void OnEnable()
    {
        GameBroadcast.OnProgressionCountUpdateHUD += (current, total) => _hudProgressionCountText.text = $"{current}/{total}";
        GameBroadcast.OnStageNameUpdateHUD += (name) => _hudStageNameText.text = name;
        GameBroadcast.OnObjectiveUpdateHUD += (obj) => _hudObjectiveText.text = obj;
    }
}