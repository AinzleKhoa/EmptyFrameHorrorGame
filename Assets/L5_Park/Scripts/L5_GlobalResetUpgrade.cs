using UnityEngine;

public class L5_GlobalResetUpgrade : MonoBehaviour
{
    private void Awake()
    {
        // 1. Reset the Candle/Safezone Upgrades
        L5_CandleManager.GlobalResetUpgrades();

        // 2. Clear the "Memory" of the old Boss and Shadow
        // This forces the Managers to spawn new ones instead of looking for dead ones
        L5_TheOverexposedManager.ResetSharedBoss();
        L5_ShadowManager.ResetSharedShadow();

        Debug.Log("<color=white>Level Initialized:</color> Upgrades and Monster references wiped clean.");
    }
}