using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player; // Kéo Player thả vào đây
    public float height = 30f;

    void LateUpdate()
    {
        if (player != null)
        {
            // Camera chỉ bay theo Player nhưng KHÔNG bị xoay
            transform.position = new Vector3(player.position.x, height, player.position.z);
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}