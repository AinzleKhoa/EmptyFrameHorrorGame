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

        // 2. Handle Cursor Logic based on the state
        switch (stateName)
        {
            case "Player":
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case "UI":
            case "Menu":
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;

            default:
                Debug.LogWarning($"Input state '{stateName}' not recognized!");
                break;
        }

        Debug.Log($"Input System switched to: {stateName}");
    }
}