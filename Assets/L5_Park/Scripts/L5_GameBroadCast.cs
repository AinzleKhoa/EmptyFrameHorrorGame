using System;

public static class L5_GameBroadcast
{
    // Tells the Director/Shadow: "Is the shadow currently hunting?"
    public static Action<bool> OnL5ShadowSpawned;

    // Tells the HUD: "Current progress bar is at X%"
    public static Action<float> OnL5FragmentProgress;

    // Tells the Director: "A single fragment just hit 100%"
    public static Action OnL5FragmentFinished;

    // NEW: Boolean to track if player has the Echo Location tool
    public static Action<bool> OnL5EchoUpgradeUnlocked;
}