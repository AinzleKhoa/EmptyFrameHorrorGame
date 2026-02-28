using System;

public static class L1H_CleanupEvents
{
    // total, cleaned
    public static Action<int, int> OnProgressChanged;

    // remaining seconds
    public static Action<float> OnTimeChanged;

    public static Action OnWin;
    public static Action OnLose;
}