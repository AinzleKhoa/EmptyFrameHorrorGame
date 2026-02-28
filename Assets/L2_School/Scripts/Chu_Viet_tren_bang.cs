using UnityEngine;
using TMPro; // Bắt buộc phải có dòng này để dùng TextMeshPro
using System.Collections;

public class ChalkBoardWriter : MonoBehaviour
{
    public TMP_Text bangDenText; // Kéo cái Text (TMP) vào đây
    public string[] noiDung; // Danh sách các câu muốn viết
    public float tocDoViet = 0.1f; // Tốc độ hiện từng chữ (hiệu ứng đánh máy)
    public float thoiGianCho = 3f; // Chờ 3 giây giữa các câu

    void Start()
    {
        // Tự động tìm component Text nếu quên kéo vào
        if (bangDenText == null)
            bangDenText = GetComponentInChildren<TMP_Text>();

        StartCoroutine(QuyTrinhVietBang());
    }

    IEnumerator QuyTrinhVietBang()
    {
        // Vòng lặp vô tận
        while (true)
        {
            foreach (string cau in noiDung)
            {
                // Xóa bảng
                bangDenText.text = "";

                // Hiệu ứng viết từng chữ (Typewriter effect) - Nhìn rất kinh dị!
                foreach (char chuCai in cau.ToCharArray())
                {
                    bangDenText.text += chuCai;
                    yield return new WaitForSeconds(tocDoViet);
                }

                // Đợi 3 giây sau khi viết xong để người chơi đọc
                yield return new WaitForSeconds(thoiGianCho);
            }
        }
    }
}