using UnityEngine;

public class MiniMapPlayerIcon : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        if (!player) return;

        float angle = -player.eulerAngles.y;


        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
