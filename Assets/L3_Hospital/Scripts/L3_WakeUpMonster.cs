using UnityEngine;

[RequireComponent(typeof(Collider))]
public class L3_WakeUpMonster : MonoBehaviour
{
    [Header("Kéo con quái TheOverexposed vào đây")]
    public GameObject monsterToWakeUp;

    private void Start()
    {
        // Đảm bảo trigger này là dạng xuyên thấu (để Player đi qua được)
        GetComponent<Collider>().isTrigger = true;

        // Tự động tắt con quái ngay khi game vừa bắt đầu để nó đứng im
        if (monsterToWakeUp != null)
        {
            monsterToWakeUp.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Khi Player chạm vào cửa -> Bật con quái lên -> Tự hủy cái cửa (trigger)
        if (other.CompareTag("Player") && monsterToWakeUp != null)
        {
            monsterToWakeUp.SetActive(true);
            Debug.Log("<color=yellow>QUÁI ĐÃ THỨC GIẤC VÀ BẮT ĐẦU DI CHUYỂN!</color>");

            Destroy(gameObject);
        }
    }
}