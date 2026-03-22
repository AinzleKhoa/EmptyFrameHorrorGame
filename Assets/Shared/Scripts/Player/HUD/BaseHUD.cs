using UnityEngine;

public abstract class BaseHUD : MonoBehaviour
{
    // A universal check to see if this UI is "Alive" and "Valid"
    protected bool IsValid => this != null && gameObject != null && gameObject.activeInHierarchy;

    // Force every HUD to clean up after itself
    protected abstract void OnEnable();
    protected abstract void OnDisable();

    // Utility: A safe way to check CanvasGroups without crashing
    protected bool IsVisible(CanvasGroup group)
    {
        if (group == null) return false;
        return group.alpha > 0;
    }
}