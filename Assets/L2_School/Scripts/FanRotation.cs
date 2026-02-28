using UnityEngine;

public partial class FanRotate : MonoBehaviour
{
    public float rotateSpeed = 300f; // Tốc độ quay, bạn có thể chỉnh ở Inspector

    void Update()
    {
        // Xoay quanh trục Y (hoặc Z tùy theo hướng model của bạn)
        transform.Rotate(0,0,rotateSpeed * Time.deltaTime);
    }
}