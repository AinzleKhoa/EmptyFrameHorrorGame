using UnityEngine;

public class L3_PhotoProgression : MonoBehaviour
{
    // 👉 VŨ KHÍ BÍ MẬT: Khai báo Instance để các bức ảnh gọi trực tiếp
    public static L3_PhotoProgression Instance;

    [Header("Tiến trình Màn 3")]
    public int PhotosCollected = 0;
    public int TotalPhotosNeeded = 5;

    [Header("Hệ thống Dịch chuyển")]
    [Tooltip("Kéo điểm đến (Sảnh) vào đây!")]
    public Transform LobbySpawnPoint;

    private GameObject _player;

    private void Awake()
    {
        Instance = this; // Bật cờ đánh dấu vị trí lãnh đạo
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null) Debug.LogWarning("MÀN 3: Không tìm thấy Player!");

        GameBroadcast.OnObjectiveUpdateHUD?.Invoke($"Find {TotalPhotosNeeded} photos.");
    }

    public void OnPhotoPickedUp()
    {
        PhotosCollected++;
        Debug.Log("Đã nhặt ảnh số: " + PhotosCollected);

        if (PhotosCollected < TotalPhotosNeeded)
        {
            GameBroadcast.OnObjectiveUpdateHUD?.Invoke($"Photos collected: {PhotosCollected}/{TotalPhotosNeeded}");
        }
        else
        {
            GameBroadcast.OnObjectiveUpdateHUD?.Invoke("All photos found! Returning to the lobby...");
            TeleportPlayerToLobby();
        }
    }

    private void TeleportPlayerToLobby()
    {
        if (_player != null && LobbySpawnPoint != null)
        {
            CharacterController cc = _player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            _player.transform.position = LobbySpawnPoint.position;
            _player.transform.rotation = LobbySpawnPoint.rotation;

            if (cc != null) cc.enabled = true;
            Debug.Log("Đã dịch chuyển về Sảnh!");
        }
    }
}