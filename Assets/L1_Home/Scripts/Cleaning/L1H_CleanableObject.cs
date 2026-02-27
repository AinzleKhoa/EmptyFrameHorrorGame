using UnityEngine;

public class L1H_CleanableObject : MonoBehaviour, IInteractable
{
    [Header("Prompt")]
    [SerializeField] private string _prompt = "Press 'E' to Clean";

    [Header("Optional")]
    [Tooltip("Nếu muốn tắt riêng mesh/VFX thay vì tắt cả object.")]
    [SerializeField] private GameObject _disableTarget;

    private bool _cleaned;

    public string PromptMessage => _cleaned ? "" : _prompt;

    public void Interact(PlayerInteractor interactor)
    {
        if (_cleaned) return;

        _cleaned = true;

        if (_disableTarget != null) _disableTarget.SetActive(false);
        else gameObject.SetActive(false);

        L1H_CleanupObjectiveManager.ReportOneCleaned();
    }
}