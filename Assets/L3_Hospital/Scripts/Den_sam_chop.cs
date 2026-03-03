
using UnityEngine;
using System.Collections;

public class Den_sam_chop : MonoBehaviour
{
    public Light lightningLight;
    public AudioSource tiengSam;
    public float thoiGianChoNhoNhat = 5f;
    public float thoiGianChoLonNhat = 15f;

    void Start()
    {
        if (lightningLight != null)
            lightningLight.intensity = 0;

        StartCoroutine(VongLapSamChop());
    }

    IEnumerator VongLapSamChop()
    {
        while (true)
        {
            // Chờ ngẫu nhiên từ 5 đến 15 giây
            yield return new WaitForSeconds(Random.Range(thoiGianChoNhoNhat, thoiGianChoLonNhat));

            // Bắt đầu chớp sáng
            StartCoroutine(ChopNhay());
        }
    }

    IEnumerator ChopNhay()
    {
        // Phát tiếng sấm
        if (tiengSam != null) tiengSam.Play();
        // Nháy lần 1 (Sáng rực)
        lightningLight.intensity = Random.Range(3f, 6f);
        yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        lightningLight.intensity = 0;

        // Nghỉ một nhịp cực ngắn
        yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));

        // Nháy lần 2 (Sáng vừa, tạo cảm giác chớp kép)
        lightningLight.intensity = Random.Range(1f, 3f);
        yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        lightningLight.intensity = 0;
    }
}