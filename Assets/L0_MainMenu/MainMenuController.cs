using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private MainMenuUI _ui; // Reference to our visual script
    [SerializeField] private string _firstLevelName = "L1_Home";

    private GameSaveData _currentData;

    private void Start()
    {
        // Global environment setup
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Load Data
        _currentData = SaveSystem.Load();

        // Tell UI to refresh itself based on this data
        if (_ui != null) _ui.SetupUI(_currentData);
    }

    public void NewGame()
    {
        // 1. Reset progress
        SaveSystem.DeleteSave();
        _currentData = new GameSaveData();
        SaveSystem.Save(_currentData);

        // 2. Load the first level via the Loading Screen
        LoadWithScreen(_firstLevelName);
    }

    public void ContinueGame()
    {
        _currentData = SaveSystem.Load();

        // 3. Continue the last saved level via the Loading Screen
        LoadWithScreen(_currentData.lastLevelName);
    }

    public void LoadLevel(string sceneName)
    {
        // THE GATEKEEPER: Check if the list actually contains this level
        if (_currentData != null && _currentData.unlockedLevels.Contains(sceneName))
        {
            Debug.Log($"<color=green>SUCCESS:</color> Loading {sceneName} via Loading Screen.");
            LoadWithScreen(sceneName);
        }
        else
        {
            // This will trigger if the level is locked
            Debug.LogWarning($"<color=red>ACCESS DENIED:</color> {sceneName} is not unlocked yet!");
        }
    }

    private void LoadWithScreen(string targetScene)
    {
        LoadingData.SceneToLoad = targetScene;
        SceneManager.LoadScene("Loading");
    }

    public void Quit() => Application.Quit();

    [ContextMenu("Debug Print Save Data")]
    public void DebugPrintSave()
    {
        GameSaveData data = SaveSystem.Load();
        Debug.Log($"Last Level: {data.lastLevelName}");
        Debug.Log($"Unlocked Levels: {string.Join(", ", data.unlockedLevels)}");
    }
}