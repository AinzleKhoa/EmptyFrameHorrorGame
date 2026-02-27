using UnityEngine;

public class L1H_CleanupObjectiveManager : MonoBehaviour
{
    [Header("Objective")]
    [SerializeField] private bool _autoCountInScene = true;
    [SerializeField] private int _totalToClean = 10;

    private static L1H_CleanupObjectiveManager _instance;

    private int _cleaned;
    private bool _ended;

    private void Awake()
    {
        _instance = this;

        if (_autoCountInScene)
        {
            _totalToClean = FindObjectsByType<L1H_CleanableObject>(FindObjectsSortMode.None).Length;
        }

        _cleaned = 0;
        _ended = false;

        L1H_CleanupEvents.OnProgressChanged?.Invoke(_totalToClean, _cleaned);
    }

    public static void ReportOneCleaned()
    {
        if (_instance == null) return;
        _instance.HandleCleaned();
    }

    private void HandleCleaned()
    {
        if (_ended) return;

        _cleaned++;
        L1H_CleanupEvents.OnProgressChanged?.Invoke(_totalToClean, _cleaned);

        if (_cleaned >= _totalToClean)
        {
            _ended = true;
            L1H_CleanupEvents.OnWin?.Invoke();
            Debug.Log("[L1H] WIN: Room cleaned!");
        }
    }

    public void Lose()
    {
        if (_ended) return;

        _ended = true;
        L1H_CleanupEvents.OnLose?.Invoke();
        Debug.Log("[L1H] LOSE: Time up!");
    }
}