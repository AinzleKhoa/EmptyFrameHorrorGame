using UnityEngine;

public class Upgrade_LuminousStabilizer : MonoBehaviour, IUpgradeEffect
{
    [Header("Upgrade Identity")]
    [SerializeField] private string _upgradeName = "Luminous Stabilizer";
    [TextArea]
    [SerializeField] private string _upgradeLore = "Spawn a lit candle on the bench, makes it a permanent safe zone. Stun The Overexposed on touch.";

    [Header("Targeting")]
    [SerializeField] private GameObject _candle;

    public void ExecuteUpgrade(PickableItem target)
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return;

        // 1. Record the upgrade in PlayerData
        if (playerObj.TryGetComponent<PlayerData>(out var data))
        {
            data.AddPermanentUpgrade($"{_upgradeName}: {_upgradeLore}");
        }

        // 2. THE FIX: Just turn it on and force it to be lit
        if (_candle != null)
        {
            _candle.SetActive(true); // 1. Turn the object on

            if (_candle.TryGetComponent<CandleInteractable>(out var interactable))
            {
                interactable.SetLit(true); // 2. Set Lit to true
            }

            Debug.Log($"<color=yellow>Safezone Created:</color> {_candle.name} is now active and lit.");
        }
        else
        {
            Debug.LogError("Luminous Stabilizer: No _candle object assigned!");
        }

        // Deactivate the upgrade item (the one you clicked on)
        gameObject.SetActive(false);
    }
}