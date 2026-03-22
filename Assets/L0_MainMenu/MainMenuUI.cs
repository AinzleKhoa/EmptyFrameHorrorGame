using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenuUI : MonoBehaviour
{
    [Header("Hierarchy Containers")]
    [SerializeField] private CanvasGroup _mainMenuCanvas;
    [SerializeField] private CanvasGroup _levelSelectCanvas;

    [Header("Buttons to Control")]
    [SerializeField] private Button _btnContinue;
    [Header("Level Select Buttons")]
    // Drag your buttons directly into these slots in the Inspector
    [SerializeField] private Button _btnLvl1;
    [SerializeField] private Button _btnLvl2;
    [SerializeField] private Button _btnLvl3;
    [SerializeField] private Button _btnLvl4;
    [SerializeField] private Button _btnLvl5;

    [Header("Settings")]
    [SerializeField] private float _fadeSpeed = 5f;

    [System.Serializable]
    public class LevelButtonMapping
    {
        public string sceneName;
        public Button button;
    }

    public void SetupUI(GameSaveData data)
    {
        // 1. Disable Continue button if the player is still on the first level and hasn't progressed
        if (_btnContinue != null)
        {
            // Continue is available if they've moved past the first level 
            // OR if you want them to be able to continue the first level, check if the file exists.
            _btnContinue.interactable = data.unlockedLevels.Count > 1 || data.lastLevelName != "L1_Home";
        }

        // 2. Level Unlocking Logic (Direct check)
        // We use the exact scene names you provided earlier
        SetButtonState(_btnLvl1, "L1_Home", data);
        SetButtonState(_btnLvl2, "L2_School", data);
        SetButtonState(_btnLvl3, "L3_Hospital", data);
        SetButtonState(_btnLvl4, "L4_Maze", data);
        SetButtonState(_btnLvl5, "L5_Park", data);

        // 3. Initial state
        SetCanvasState(_mainMenuCanvas, true);
        SetCanvasState(_levelSelectCanvas, false);
    }

    private void SetButtonState(Button btn, string sceneName, GameSaveData data)
    {
        if (btn == null) return;

        // Does the save file contain this level name?
        bool isUnlocked = data.unlockedLevels.Contains(sceneName);

        // Turn the button interaction on or off
        btn.interactable = isUnlocked;

        // OPTIONAL: Make it look "Disabled" visually by changing alpha
        // If your button has a CanvasGroup, this makes it look grey/faded
        if (btn.TryGetComponent(out CanvasGroup group))
        {
            group.alpha = isUnlocked ? 1f : 0.3f;
        }
    }

    public void PlayTransition(bool openingLevelSelect)
    {
        StopAllCoroutines();
        StartCoroutine(TransitionRoutine(openingLevelSelect));
    }

    private IEnumerator TransitionRoutine(bool openingLevelSelect)
    {
        CanvasGroup toHide = openingLevelSelect ? _mainMenuCanvas : _levelSelectCanvas;
        CanvasGroup toShow = openingLevelSelect ? _levelSelectCanvas : _mainMenuCanvas;

        yield return StartCoroutine(Fade(toHide, 0f));
        toHide.gameObject.SetActive(false);

        toShow.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(toShow, 1f));
    }

    private IEnumerator Fade(CanvasGroup cg, float target)
    {
        while (!Mathf.Approximately(cg.alpha, target))
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, target, _fadeSpeed * Time.deltaTime);
            yield return null;
        }
        cg.interactable = (target > 0.5f);
        cg.blocksRaycasts = (target > 0.5f);
    }

    private void SetCanvasState(CanvasGroup cg, bool active)
    {
        cg.alpha = active ? 1f : 0f;
        cg.interactable = active;
        cg.blocksRaycasts = active;
        cg.gameObject.SetActive(active);
    }
}