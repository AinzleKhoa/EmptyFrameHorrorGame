using UnityEngine;

public static class L1H_HeldRegistry
{
    public static L1H_SnappableItem CurrentHeld { get; private set; }

    public static void SetHeld(L1H_SnappableItem item)
    {
        CurrentHeld = item;
    }

    public static void ClearHeld(L1H_SnappableItem item)
    {
        if (CurrentHeld == item)
            CurrentHeld = null;
    }
}