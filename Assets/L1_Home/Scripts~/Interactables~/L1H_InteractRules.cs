public static class L1H_InteractRules
{
    public static bool IsHoldingAnySnapItem()
    {
        return L1H_HeldRegistry.CurrentHeld != null;
    }

    public static bool CanInteractWithGeneralObject()
    {
        // Nếu đang cầm đồ snap thì khóa các object thường
        return L1H_HeldRegistry.CurrentHeld == null;
    }

    public static bool CanInteractWithPlaceSpot(string spotId)
    {
        var held = L1H_HeldRegistry.CurrentHeld;
        if (held == null) return false;

        return !held.IsSnapped && held.ItemId == spotId;
    }

    public static bool CanInteractWithThisSnappable(L1H_SnappableItem item)
    {
        var held = L1H_HeldRegistry.CurrentHeld;

        // Không cầm gì -> được nhặt item này
        if (held == null) return true;

        // Chỉ cho phép tương tác với chính item đang cầm
        return held == item;
    }
}