using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteManager : MonoBehaviour
{
    public enum LevelName { L1_Home, L2_School, L3_Hospital, L4_Maze, L5_Park, MainMenu }

    [Header("Transition Settings")]
    [SerializeField] private LevelName _targetLevel; // Now a dropdown menu!

    [Header("Behavior")]
    [SerializeField] private bool _autoCompleteOnFragments = true;

    private void OnEnable()
    {
        // Listen for the broadcast
        GameBroadcast.OnAllFragmentsCollected += HandleFragmentsBroadcast;
    }

    private void OnDisable()
    {
        // Safety: Always unsubscribe
        GameBroadcast.OnAllFragmentsCollected -= HandleFragmentsBroadcast;
    }

    private void HandleFragmentsBroadcast(bool isDone)
    {
        // Only trigger if this level is set to finish when fragments are done
        if (isDone && _autoCompleteOnFragments)
        {
            CompleteLevel();
        }
    }

    /// <summary>
    /// This can be called by UI buttons, Story Directors, or other scripts.
    /// </summary>
    public void CompleteLevel()
    {
        string levelString = _targetLevel.ToString();
        Debug.Log($"<color=cyan>Level Manager:</color> Completing level. Target: {levelString}");

        // 1. Load Data
        GameSaveData data = SaveSystem.Load();

        // FIX: Prevent Duplicate Names
        // Only add if the list DOES NOT already contain this name
        if (!data.unlockedLevels.Contains(levelString))
        {
            data.unlockedLevels.Add(levelString);
            Debug.Log($"Added {levelString} to unlocked list.");
        }

        // Always update the last level for the 'Continue' button
        data.lastLevelName = levelString;

        // 3. Save to File
        SaveSystem.Save(data);

        // 4. Scene Transition
        LoadLevelWithScreen(levelString);
    }

    public void LoadLevelWithScreen(string targetScene)
    {
        // 1. Pack the data
        LoadingData.SceneToLoad = targetScene;

        // 2. Go to the Loading scene
        SceneManager.LoadScene("Loading");
    }
}