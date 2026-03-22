using UnityEngine;
using System.Collections;

public sealed class LightStutter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light targetLight;

    [Header("Settings")]
    [SerializeField] private float flashIntensity = 1.5f;
    [Range(0, 100)]
    [SerializeField] private float chancePerFrame = 0.2f; // Keep this very low!

    [Header("Duration")]
    [SerializeField] private float minFlashTime = 0.05f;
    [SerializeField] private float maxFlashTime = 0.15f;

    private bool _isFlashing = false;

    void Start()
    {
        // Ensure the light is off when the game starts
        if (targetLight != null)
        {
            targetLight.intensity = 0f;
            targetLight.enabled = true; // Keep the component on, just hide the light
        }
    }

    void Update()
    {
        if (targetLight == null || _isFlashing) return;

        // Check for a random "blip" on
        if (Random.Range(0f, 100f) < chancePerFrame)
        {
            StartCoroutine(FlashRoutine());
        }
    }

    private IEnumerator FlashRoutine()
    {
        _isFlashing = true;

        // Sudden ON
        targetLight.intensity = flashIntensity;

        // Wait a tiny fraction of a second (the "glimpse")
        yield return new WaitForSeconds(Random.Range(minFlashTime, maxFlashTime));

        // Back to OFF
        targetLight.intensity = 0f;

        // Small cooldown so it doesn't flicker like a strobe light
        yield return new WaitForSeconds(0.1f);

        _isFlashing = false;
    }
}