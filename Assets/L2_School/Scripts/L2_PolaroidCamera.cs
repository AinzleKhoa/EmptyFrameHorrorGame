using UnityEngine;
using System.Collections;

public class L2_PolaroidCamera : MonoBehaviour
{
    [Header("Settings")]
    public float FlashRange = 12f;
    public float CooldownTime = 3f;
    private bool _isReady = true;

    void Update()
    {
        // Bấm chuột trái để chụp
        if (Input.GetMouseButtonDown(0) && _isReady)
        {
            StartCoroutine(PerformFlash());
        }
    }

    private IEnumerator PerformFlash()
    {
        _isReady = false;
        GameBroadcast.OnUpdateItemDescription?.Invoke("FLASH!");

        // Tìm quái vật
        L2_MonsterAI[] monsters = FindObjectsOfType<L2_MonsterAI>();
        foreach (var monster in monsters)
        {
            // Kiểm tra khoảng cách
            if (Vector3.Distance(transform.position, monster.transform.position) <= FlashRange)
            {
                // Lóa mắt quái
                monster.ApplyFlashStun();
            }
        }

        yield return new WaitForSeconds(CooldownTime);
        _isReady = true;
        GameBroadcast.OnUpdateItemDescription?.Invoke("Camera Ready!");
    }
}