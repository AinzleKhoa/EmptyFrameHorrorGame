using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform player;
    public float height = 40f;

    void LateUpdate()
    {
        if (!player) return;

        Vector3 pos = player.position;
        pos.y = height;
        transform.position = pos;

        // Map không xoay, player icon xoay
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
