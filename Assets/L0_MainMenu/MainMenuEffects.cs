using UnityEngine;

public sealed class MainMenuEffects : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera menuCamera;
    [SerializeField] private Light lanternLight;
    [SerializeField] private Light monsterLight;

    [Header("Camera Sway Settings")]
    [SerializeField] private float swaySpeed = 0.5f;
    [SerializeField] private float swayAmount = 0.05f;

    [Header("Lantern Flicker Settings")]
    [SerializeField] private float minIntensity = 0.8f;
    [SerializeField] private float maxIntensity = 1.2f;
    [SerializeField] private float flickerSmoothing = 0.1f;
    [Header("Sudden Stutter Settings")]
    [Range(0, 100)]
    [SerializeField] private float stutterChance = 1f; // % chance per frame to stutter
    [SerializeField] private float minStutterTime = 0.05f;
    [SerializeField] private float maxStutterTime = 0.2f;

    private Vector3 _cameraStartPos;
    private float _targetIntensity;
    private float _flickerVelocity;
    private bool _isStuttering = false;

    void Start()
    {
        if (menuCamera != null)
            _cameraStartPos = menuCamera.transform.localPosition;
    }

    void Update()
    {
        HandleCameraSway();
        HandleLightFlicker();
        if (!_isStuttering)
        {
            CheckForStutter();
        }
    }

    private void HandleCameraSway()
    {
        if (menuCamera == null) return;

        // Using PerlinNoise for a natural, "handheld" feel
        float x = (Mathf.PerlinNoise(Time.time * swaySpeed, 0) - 0.5f) * 2 * swayAmount;
        float y = (Mathf.PerlinNoise(0, Time.time * swaySpeed) - 0.5f) * 2 * swayAmount;

        menuCamera.transform.localPosition = _cameraStartPos + new Vector3(x, y, 0);
    }

    private void HandleLightFlicker()
    {
        if (lanternLight == null) return;

        // SmoothDamp makes the flicker look like a flame reacting to wind, 
        // rather than a strobe light.
        if (Mathf.Abs(lanternLight.intensity - _targetIntensity) < 0.01f)
        {
            _targetIntensity = Random.Range(minIntensity, maxIntensity);
        }

        lanternLight.intensity = Mathf.SmoothDamp(
            lanternLight.intensity,
            _targetIntensity,
            ref _flickerVelocity,
            flickerSmoothing
        );
    }

    private void CheckForStutter()
    {
        // Randomly trigger a light cutout
        if (Random.Range(0f, 100f) < stutterChance)
        {
            StartCoroutine(StutterRoutine());
        }
    }

    private System.Collections.IEnumerator StutterRoutine()
    {
        _isStuttering = true;
        float originalIntensity = monsterLight.intensity;

        // Turn light off
        monsterLight.intensity = 0f;

        // Wait a random short time
        yield return new WaitForSeconds(Random.Range(minStutterTime, maxStutterTime));

        // Snap it back on (or add a second quick flash for a 'broken' feel)
        monsterLight.intensity = originalIntensity;
        _isStuttering = false;
    }
}