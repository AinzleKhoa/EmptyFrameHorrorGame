using UnityEngine;

public class L3_PhotoProgression : MonoBehaviour
{
    public static L3_PhotoProgression Instance;

    [Header("Tiến trình Màn 3")]
    public int PhotosCollected = 0;
    public int TotalPhotosNeeded = 5;

    [Header("Hệ thống Dịch chuyển")]
    [Tooltip("Kéo điểm đến (Sảnh) vào đây!")]
    public Transform LobbySpawnPoint;

    [Header("NPC Shadow Cuối Cùng")]
    [Tooltip("Kéo NPC Shadow ở Sảnh vào đây")]
    public GameObject FinalShadowNPC;

    private GameObject _player;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null) Debug.LogWarning("MÀN 3: Không tìm thấy Player!");

        GameBroadcast.OnObjectiveUpdateHUD?.Invoke($"Find {TotalPhotosNeeded} photo fragments.");

        // Tắt NPC Shadow cuối cùng lúc mới vào game
        if (FinalShadowNPC != null) FinalShadowNPC.SetActive(false);
    }

    public void OnPhotoPickedUp()
    {
        PhotosCollected++;
        Debug.Log("Đã nhặt ảnh số: " + PhotosCollected);

        if (PhotosCollected < TotalPhotosNeeded)
        {
            GameBroadcast.OnObjectiveUpdateHUD?.Invoke($"Fragments collected: {PhotosCollected}/{TotalPhotosNeeded}");
        }
        else
        {
            // --- KHI ĐỦ 5 MẢNH ---
            GameBroadcast.OnObjectiveUpdateHUD?.Invoke("Talk to Shadow in the Lobby.");

            // Hiện NPC Shadow lên để người chơi ra nói chuyện
            if (FinalShadowNPC != null) FinalShadowNPC.SetActive(true);

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