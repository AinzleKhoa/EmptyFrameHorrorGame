using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private string _mainMenuSceneName = "MainMenu";

    public void BackToMenu()
    {
        // 1. Unpause the game engine
        Time.timeScale = 1f;

        // 2. Use the bridge to go to the Menu via Loading Screen
        LoadWithScreen(_mainMenuSceneName);

        Debug.Log("Returning to Main Menu...");
    }

    public void RestartLevel()
    {
        // 1. Unpause the game engine
        Time.timeScale = 1f;

        // 2. Get current scene name and pass it to the loading screen
        string currentScene = SceneManager.GetActiveScene().name;
        LoadWithScreen(currentScene);

        Debug.Log($"Restarting {currentScene} via Loading Screen...");
    }

    // This is our unified transition method
    private void LoadWithScreen(string targetScene)
    {
        LoadingData.SceneToLoad = targetScene;
        SceneManager.LoadScene("Loading");
    }
}