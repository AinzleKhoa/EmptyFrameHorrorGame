using UnityEngine;

public sealed class MMCandleFlicker : MonoBehaviour
{
    [SerializeField] private Light targetLight;
    [SerializeField] private float minIntensity = 0.8f;
    [SerializeField] private float maxIntensity = 1.2f;
    [SerializeField] private float smoothing = 0.1f;

    private float _targetIntensity;
    private float _velocity;

    void Start()
    {
        if (targetLight != null) _targetIntensity = targetLight.intensity;
    }

    void Update()
    {
        if (targetLight == null) return;

        if (Mathf.Abs(targetLight.intensity - _targetIntensity) < 0.01f)
        {
            _targetIntensity = Random.Range(minIntensity, maxIntensity);
        }

        targetLight.intensity = Mathf.SmoothDamp(
            targetLight.intensity,
            _targetIntensity,
            ref _velocity,
            smoothing
        );
    }
}