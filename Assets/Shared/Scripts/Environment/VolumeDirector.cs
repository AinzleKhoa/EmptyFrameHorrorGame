using UnityEngine.Rendering;
using UnityEngine;

public class VolumeDirector : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Volume _globalVolume; // Volume chính trong scene

    [Header("Profile Settings")]
    [SerializeField] private VolumeProfile _targetProfile;

    public void ApplyVolume()
    {
        if (_globalVolume == null || _targetProfile == null) return;

        // Thay đổi toàn bộ Profile của Post-Processing
        _globalVolume.profile = _targetProfile;

        Debug.Log($"<color=magenta>VolumeDirector:</color> Post-Processing profile swapped.");
    }
}