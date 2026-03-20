using UnityEngine;

public class LightingDirector : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Light _sunLight; // Kéo Directional Light vào đây

    [Header("Light Settings")]
    [SerializeField] private Color _lightColor = Color.white;
    [SerializeField] private float _intensity = 1f;
    [SerializeField] private Vector3 _rotation = new Vector3(50, -30, 0);

    public void ApplyLight()
    {
        if (_sunLight == null) return;

        _sunLight.color = _lightColor;
        _sunLight.intensity = _intensity;

        // Thay đổi góc quay của mặt trời (Góc nắng)
        _sunLight.transform.localEulerAngles = _rotation;

        Debug.Log($"<color=orange>LightingDirector:</color> Sun settings applied.");
    }
}