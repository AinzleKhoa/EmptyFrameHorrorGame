using UnityEngine;

[RequireComponent(typeof(UpgradeInteractable))]
public class CameraViewfinderUpgrade : MonoBehaviour, IUpgradeEffect
{
    [Header("Upgrade Identity")]
    [SerializeField] private string _upgradeName = "Spectral Lens";
    [TextArea]
    [SerializeField] private string _upgradeLore = "Reveals item outlines through the viewfinder.";

    public void ExecuteUpgrade(PickableItem target)
    {
        // 1. Update Lore
        if (target.TryGetComponent<ItemData>(out var data))
        {
            data.AddUpgrade($"{_upgradeName}: {_upgradeLore}");
            GameBroadcast.OnUpdateItemDescription?.Invoke(data.FullDescription);
        }

        // 2. Unlock the feature on the camera
        if (target.TryGetComponent<PolaroidCamera>(out var camera))
        {
            camera.UnlockSpectralLens(true);
        }

        Debug.Log($"<color=purple>Lens Integrated:</color> {_upgradeName}");
    }
}