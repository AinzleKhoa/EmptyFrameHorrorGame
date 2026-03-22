using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScreenController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image _backgroundDisplay;
    [SerializeField] private Slider _loadingBar;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private TextMeshProUGUI _levelName;

    private void Start()
    {
        // 1. Load background image by name
        _backgroundDisplay.sprite = Resources.Load<Sprite>(LoadingData.SceneToLoad);

        // 2. Set the Story Title based on the Scene Name
        if (_levelName != null)
        {
            _levelName.text = GetStoryTitle(LoadingData.SceneToLoad);
        }

        // 3. Start the loading
        StartCoroutine(LoadLevelAsync());
    }

    private IEnumerator LoadLevelAsync()
    {
        // Start loading the scene in the background
        AsyncOperation operation = SceneManager.LoadSceneAsync(LoadingData.SceneToLoad);

        // Prevent the scene from showing until it's 100% ready
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // operation.progress goes from 0 to 0.9. 
            // 0.9 means the scene is fully loaded but not "active" yet.
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (_loadingBar != null) _loadingBar.value = progress;
            if (_loadingText != null) _loadingText.text = $"Loading... {(progress * 100):F0}%";

            // If it's 90% loaded, we can finally activate it
            if (operation.progress >= 0.9f)
            {
                // You can add a "Press Any Key" prompt here if you want
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private string GetStoryTitle(string sceneName)
    {
        // Simple mapping: Code Name -> Story Title
        switch (sceneName)
        {
            case "L1_Home": return "Level 1: Once my Home";
            case "L2_School": return "Level 2: School of Whispers";
            case "L3_Hospital": return "Level 3: The Silent Hospital";
            case "L4_Maze": return "Level 4: Maze of Memories";
            case "L5_Park": return "Level 5: The last Memory";
            case "MainMenu": return "Main Menu";
            default: return "Untitled Level"; // Fallback
        }
    }
}