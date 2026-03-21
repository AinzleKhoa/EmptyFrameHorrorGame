using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputStateController : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    private void OnEnable() => GameBroadcast.OnInputStateChange += SwitchState;
    private void OnDisable() => GameBroadcast.OnInputStateChange -= SwitchState;

    public void SwitchState(string stateName)
    {
        // 1. Switch the actual Action Map
        _playerInput.SwitchCurrentActionMap(stateName);

        switch (stateName)
        {
            case "Player":
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case "UI": // Both Menu and Documents use this
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }

        Debug.Log($"Input System switched to: {stateName}");
    }
}