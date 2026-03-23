using UnityEngine;

public class L1H_SnapSpot : MonoBehaviour
{
    [Header("Identify which item can snap here")]
    [SerializeField] private string spotId = "blanket"; // ví dụ: blanket, pillow

    public string SpotId => spotId;
}