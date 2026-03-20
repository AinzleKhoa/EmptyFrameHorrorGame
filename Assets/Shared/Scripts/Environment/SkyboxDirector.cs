using UnityEngine;

public class SkyboxDirector : MonoBehaviour
{
    [Header("Skybox Settings")]
    [SerializeField] private Material _targetSkybox;
    [SerializeField] private Color _fogColor = Color.black;
    [SerializeField] private float _fogDensity = 0.02f;

    [Header("Ambient Lighting")]
    [SerializeField] private Color _ambientColor = Color.gray;

    public void ApplySkybox()
    {
        if (_targetSkybox != null) RenderSettings.skybox = _targetSkybox;

        RenderSettings.fog = true;
        RenderSettings.fogColor = _fogColor;
        RenderSettings.fogDensity = _fogDensity;
        RenderSettings.ambientLight = _ambientColor;

        DynamicGI.UpdateEnvironment();
        Debug.Log($"<color=cyan>SkyboxDirector:</color> Environment updated.");
    }
}