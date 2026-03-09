using UnityEngine;

public class PlayerHUDManager : MonoBehaviour
{
    [Header("HUD Management")]
    [SerializeField] private GameObject _normalStateGroup;
    [SerializeField] private GameObject _cameraStateGroup;

    private void OnEnable() => GameBroadcast.OnCameraToggle += HandleCameraStateChange;
    private void OnDisable() => GameBroadcast.OnCameraToggle -= HandleCameraStateChange;

    private void HandleCameraStateChange(bool isOnCamera)
    {
        if (_normalStateGroup != null) _normalStateGroup.SetActive(!isOnCamera);
        if (_cameraStateGroup != null) _cameraStateGroup.SetActive(isOnCamera);
    }
}