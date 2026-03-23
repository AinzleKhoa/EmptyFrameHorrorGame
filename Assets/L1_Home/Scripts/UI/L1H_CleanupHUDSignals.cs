using System;

public static class L1H_CleanupHUDSignals
{
    // current, goal
    public static Action<int, int> OnCleanProgressChanged;

    // remaining time
    public static Action<float> OnTimeChanged;
}