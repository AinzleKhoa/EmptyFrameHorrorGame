using UnityEngine;
using UnityEngine.InputSystem;

public class GameFlowController : MonoBehaviour
{
    private bool _isPaused = false;

    // Called by PlayerInput (Map: Player, Action: Pause)
    public void OnPause(InputValue value) => PauseGame();

    // Called by PlayerInput (Map: UI, Action: Resume)
    // public void OnResume(InputValue value) => ResumeGame();

    private void Start()
    {
        // Ensure the game starts unpaused and in the Player map
        ResumeGame();
    }

    public void PauseGame()
    {
        if (_isPaused) return;
        _isPaused = true;

        Time.timeScale = 0f;
        GameBroadcast.OnInputStateChange?.Invoke("UI");   // Unlocks Mouse/Switches Map
        GameBroadcast.OnPauseMenuToggle?.Invoke(true);    // Shows the Menu Graphics
    }

    public void ResumeGame()
    {
        if (!_isPaused) return;
        _isPaused = false;

        Time.timeScale = 1f;
        GameBroadcast.OnInputStateChange?.Invoke("Player"); // Locks Mouse/Switches Map
        GameBroadcast.OnPauseMenuToggle?.Invoke(false);    // Hides the Menu Graphics
    }
}