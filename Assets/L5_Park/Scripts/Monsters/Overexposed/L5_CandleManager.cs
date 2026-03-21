using UnityEngine;
using System.Collections.Generic;

public class L5_CandleManager : MonoBehaviour
{
    public static L5_CandleManager Instance { get; private set; }

    [Header("Candle Groups")]
    [SerializeField] private List<GameObject> _groupA; // First 4 candles
    [SerializeField] private List<GameObject> _groupB; // Second 4 candles

    [Header("Upgrade Status (View Only)")]
    private static bool _emergencySupplies = false;
    private static bool _litFlame = false;

    public bool IsLitFlameUnlocked => _litFlame;

    private List<GameObject> _allCandles = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
        // Combine for easy cleanup later
        _allCandles.AddRange(_groupA);
        _allCandles.AddRange(_groupB);

        // Ensure everything starts OFF
        DeactivateAll();
    }

    public void SpawnCandleBatch()
    {
        // If Emergency Supplies is on, just turn them ALL on.
        if (_emergencySupplies)
        {
            ActivateGroup(_allCandles);
            return;
        }

        // Otherwise, pick a random group (A or B)
        if (Random.value > 0.5f) ActivateGroup(_groupA);
        else ActivateGroup(_groupB);
    }

    private void ActivateGroup(List<GameObject> group)
    {
        foreach (GameObject candle in group)
        {
            candle.SetActive(true);

            // If Lit Flame upgrade is unlocked, tell the candle to ignite immediately
            if (_litFlame)
            {
                if (candle.TryGetComponent<CandleInteractable>(out var c))
                {
                    c.SetLit(true); // Assuming your candle script has a public Ignite/SetLit method
                }
            }
        }
    }

    public void DeactivateAll()
    {
        foreach (GameObject candle in _allCandles)
        {
            if (candle != null)
            {
                // Reset the internal logic so it's not "already lit" next time
                if (candle.TryGetComponent<CandleInteractable>(out var c))
                {
                    c.SetLit(false);
                }
                candle.SetActive(false);
            }
        }
        Debug.Log("<color=orange>Candle Manager:</color> All candles cleared.");
    }

    // --- Upgrade Hooks ---
    public void UnlockEmergencySupplies() => _emergencySupplies = true;
    public void UnlockLitFlame() => _litFlame = true;

    // Call this only at the very start of the level, not every phase
    public static void GlobalResetUpgrades()
    {
        _emergencySupplies = false;
        _litFlame = false;
    }
}