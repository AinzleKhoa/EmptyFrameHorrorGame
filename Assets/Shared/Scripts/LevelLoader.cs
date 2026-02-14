using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _transitionDelay = 2.0f; // Time for the fade-out

    private void OnEnable()
    {
        GameEvents.OnLevelTransitionStarted += StartTransition;
    }

    private void OnDisable()
    {
        GameEvents.OnLevelTransitionStarted -= StartTransition;
    }

    private void StartTransition(string sceneName)
    {
        // 1. Tell everyone to stop (Broadcast "CameraToggle true" to disable movement/interaction)
        GameEvents.OnCameraToggle?.Invoke(true);

        // 2. Start the loading process
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        Debug.Log($"Transitioning to {sceneName}...");

        // Here you would tell your HUD to "Fade to Black"
        // GameEvents.OnFadeRequest?.Invoke(true); 

        yield return new WaitForSeconds(_transitionDelay);

        // Actually change the scene
        SceneManager.LoadScene(sceneName);
    }
}