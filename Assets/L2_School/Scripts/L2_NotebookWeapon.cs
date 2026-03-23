using UnityEngine;
using UnityEngine.InputSystem;

public class L2_NotebookWeapon : MonoBehaviour
{
    [Header("Trạng thái")]
    public bool HasNotebook = false;

    [Header("Liên kết vật phẩm")]
    public GameObject NotebookOnTable;

    [Header("Cài đặt Vũ Khí")]
    public int MaxUses = 6;
    public float CooldownTime = 15f;
    public float ThrowRange = 25f;

    [Header("Âm thanh & Hiệu ứng")]
    public AudioSource ThrowSound;

    private int _currentUses;
    private float _cooldownTimer = 0f;

    private LineRenderer _aimLaser;
    private string _independentUIText = ""; // Biến chứa text riêng biệt
    private float _textClearTimer = 0f;

    private void Start()
    {
        _currentUses = MaxUses;

        _aimLaser = gameObject.AddComponent<LineRenderer>();
        _aimLaser.startWidth = 0.02f;
        _aimLaser.endWidth = 0.02f;
        _aimLaser.material = new Material(Shader.Find("Sprites/Default"));
        _aimLaser.startColor = Color.red;
        _aimLaser.endColor = new Color(1f, 0f, 0f, 0f);
        _aimLaser.enabled = false;
    }

    private void Update()
    {
        if (!HasNotebook)
        {
            if (NotebookOnTable != null && !NotebookOnTable.activeInHierarchy) PickUpNotebook();
            return;
        }

        // Tự động xóa chữ trên màn hình sau 2 giây nếu không làm gì
        if (_textClearTimer > 0)
        {
            _textClearTimer -= Time.deltaTime;
            if (_textClearTimer <= 0) _independentUIText = "";
        }

        if (_cooldownTimer > 0) _cooldownTimer -= Time.deltaTime;
        if (Mouse.current == null) return;

        bool isAiming = Mouse.current.rightButton.isPressed;
        bool pressedThrow = Mouse.current.leftButton.wasPressedThisFrame;

        if (isAiming)
        {
            _textClearTimer = 0f; // Đang ngắm thì cấm xóa chữ

            if (_cooldownTimer > 0)
            {
                _independentUIText = $"Recharging: {_cooldownTimer:F1}s...";
                _aimLaser.enabled = false;
            }
            else if (_currentUses <= 0)
            {
                _independentUIText = "Out of Notebooks!";
                _aimLaser.enabled = false;
            }
            else
            {
                _independentUIText = $"AIMING... (Left Click to Throw | {_currentUses}/{MaxUses} left)";
                DrawAimLaser();

                if (pressedThrow) TryThrowNotebook();
            }
        }
        else
        {
            // Nhả chuột phát là xóa tia ngắm và dọn dẹp chữ ngay lập tức
            if (_independentUIText.StartsWith("AIMING") || _independentUIText.StartsWith("Recharging"))
            {
                _independentUIText = "";
            }
            _aimLaser.enabled = false;
        }
    }

    public void PickUpNotebook()
    {
        HasNotebook = true;
        _currentUses = MaxUses;
        _independentUIText = "Equipped Notebook!";
        _textClearTimer = 2f;
    }

    private void TryThrowNotebook()
    {
        _currentUses--;
        _cooldownTimer = CooldownTime;
        _aimLaser.enabled = false;

        if (ThrowSound != null) ThrowSound.Play();

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, ThrowRange))
        {
            L2_MonsterAI monster = hit.collider.GetComponentInParent<L2_MonsterAI>();
            if (monster != null)
            {
                monster.ApplyFlashStun();
                _independentUIText = "MONSTER STUNNED!";
                _textClearTimer = 2f;
            }
        }
    }

    private void DrawAimLaser()
    {
        _aimLaser.enabled = true;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 startPos = Camera.main.transform.position + Camera.main.transform.forward * 0.5f + Camera.main.transform.right * 0.3f - Camera.main.transform.up * 0.2f;
        _aimLaser.SetPosition(0, startPos);

        if (Physics.Raycast(ray, out RaycastHit hit, ThrowRange))
            _aimLaser.SetPosition(1, hit.point);
        else
            _aimLaser.SetPosition(1, ray.GetPoint(ThrowRange));
    }

    // 👉 ĐÂY LÀ PHÉP THUẬT: Tự vẽ UI riêng bằng code thuần mà không mượn đồ của team
    private void OnGUI()
    {
        if (string.IsNullOrEmpty(_independentUIText)) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;

        // Chọn màu tùy trạng thái
        if (_independentUIText.Contains("AIMING")) style.normal.textColor = Color.yellow;
        else if (_independentUIText.Contains("Recharging")) style.normal.textColor = Color.gray;
        else if (_independentUIText.Contains("Out") || _independentUIText.Contains("STUNNED")) style.normal.textColor = Color.red;
        else style.normal.textColor = Color.green;

        // Vẽ cái hộp chứa chữ ở chính giữa cạnh dưới màn hình (trên khung thoại một tí)
        Rect rect = new Rect(Screen.width / 2 - 200, Screen.height - 250, 400, 50);

        // Đổ bóng đen cho chữ dễ đọc trên nền tối
        GUIStyle shadowStyle = new GUIStyle(style);
        shadowStyle.normal.textColor = Color.black;
        GUI.Label(new Rect(rect.x + 2, rect.y + 2, rect.width, rect.height), _independentUIText, shadowStyle);

        // Chữ chính
        GUI.Label(rect, _independentUIText, style);
    }
}