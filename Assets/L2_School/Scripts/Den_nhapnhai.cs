using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour
{
    private Light lightSource;
    public float waitTime = 5f; // Thời gian chờ giữa các lần chớp

    void Start()
    {
        lightSource = GetComponent<Light>();
        // Bắt đầu vòng lặp chớp đèn
        StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // Đợi 5 giây
            yield return new WaitForSeconds(waitTime);

            // Chớp 3 cái
            for (int i = 0; i < 3; i++)
            {
                lightSource.enabled = false; // Tắt đèn
                yield return new WaitForSeconds(0.1f); // Tắt trong 0.1s
                lightSource.enabled = true;  // Bật đèn
                yield return new WaitForSeconds(0.1f); // Bật trong 0.1s
            }
        }
    }
}