using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerHUDManager : MonoBehaviour
{
    [Header("HUD Groups")]
    [SerializeField] private GameObject _normalStateGroup;
    [SerializeField] private GameObject _cameraStateGroup;
    [SerializeField] private GameObject _menuStateGroup; // Add your Menu here

    private bool _isCurrentlyOnCamera = false;

    private void OnEnable()
    {
        GameBroadcast.OnCameraToggle += HandleCameraStateChange;
        GameBroadcast.OnPauseMenuToggle += ShowMenu; // Listens for specific Pause events
    }

    private void OnDisable()
    {
        GameBroadcast.OnCameraToggle -= HandleCameraStateChange;
        GameBroadcast.OnPauseMenuToggle -= ShowMenu;
    }

    // This handles switching between Normal and Camera during gameplay
    private void HandleCameraStateChange(bool isOnCamera)
    {
        _isCurrentlyOnCamera = isOnCamera;
        UpdateGameplayUI();
    }

    private void ShowMenu(bool isMenuOpen)
    {
        _menuStateGroup.SetActive(isMenuOpen);

        if (isMenuOpen)
        {
            _normalStateGroup.SetActive(false);
            _cameraStateGroup.SetActive(false);
        }
        else
        {
            UpdateGameplayUI();
        }
    }

    private void UpdateGameplayUI()
    {
        _normalStateGroup.SetActive(!_isCurrentlyOnCamera);
        _cameraStateGroup.SetActive(_isCurrentlyOnCamera);
    }
}