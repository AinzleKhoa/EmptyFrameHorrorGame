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
        GameBroadcast.OnInputStateChange?.Invoke("UI");
        GameBroadcast.OnHUDStateChanged?.Invoke(HUDState.Pause);
    }

    public void ResumeGame()
    {
        if (!_isPaused) return;
        _isPaused = false;

        Time.timeScale = 1f;
        GameBroadcast.OnInputStateChange?.Invoke("Player"); // Locks Mouse/Switches Map
        GameBroadcast.OnHUDStateChanged?.Invoke(HUDState.Gameplay);    // Hides the Menu Graphics
    }

    public void KillPlayer()
    {
        if (_isPaused) ResumeGame(); // Safety: don't double-dip states

        Time.timeScale = 0f;
        GameBroadcast.OnInputStateChange?.Invoke("UI");
        GameBroadcast.OnHUDStateChanged?.Invoke(HUDState.GameOver);
    }

    public void GoodEnd()
    {
        Time.timeScale = 0f;
        GameBroadcast.OnInputStateChange?.Invoke("UI");
        GameBroadcast.OnHUDStateChanged?.Invoke(HUDState.GoodEnd);
    }

    public void BadEnd()
    {
        Time.timeScale = 0f;
        GameBroadcast.OnInputStateChange?.Invoke("UI");
        GameBroadcast.OnHUDStateChanged?.Invoke(HUDState.BadEnd);
    }
}