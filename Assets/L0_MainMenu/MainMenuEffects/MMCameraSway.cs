using UnityEngine;

public sealed class MMCameraSway : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float amount = 0.05f;

    private Vector3 _startPos;

    void Start()
    {
        if (targetCamera != null)
            _startPos = targetCamera.transform.localPosition;
    }

    void Update()
    {
        if (targetCamera == null) return;

        float x = (Mathf.PerlinNoise(Time.time * speed, 0) - 0.5f) * 2 * amount;
        float y = (Mathf.PerlinNoise(0, Time.time * speed) - 0.5f) * 2 * amount;

        targetCamera.transform.localPosition = _startPos + new Vector3(x, y, 0);
    }
}