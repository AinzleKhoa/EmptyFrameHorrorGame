using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class MouseSensitivitySlider : MonoBehaviour
{
    private Slider _slider;
    private const string SensKey = "MouseSensitivity";

    void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    // This runs EVERY time the Pause Menu or Settings Menu is opened
    void OnEnable()
    {
        // 1. Load the value (Default to 1.0)
        float savedValue = PlayerPrefs.GetFloat(SensKey, 1.0f);

        // 2. Force the slider UI to match the saved value
        _slider.value = savedValue;

        Debug.Log($"<color=green>Slider Initialized to: {savedValue}</color>");
    }

    // IMPORTANT: Link this to the Slider's "On Value Changed" in the Inspector
    public void HandleSliderChange(float newValue)
    {
        // 1. Update the PlayerPrefs
        PlayerPrefs.SetFloat(SensKey, newValue);

        // 2. Force write to disk immediately
        PlayerPrefs.Save();

        Debug.Log($"<color=yellow>Sensitivity SAVED to PlayerPrefs: {newValue}</color>");
    }
}