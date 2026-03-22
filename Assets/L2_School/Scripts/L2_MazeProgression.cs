using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.AI;
using UnityEngine.SceneManagement; // Thêm thư viện này để chuyển màn

public class L2_MazeProgression : MonoBehaviour
{
    [Header("Tiến trình")]
    public int FragmentsCollected = 0;
    public int TotalFragments = 4; // Vẫn để 4 vì mảnh 5 là ngoại lệ

    [Header("Quản lý Mảnh Vỡ (Fragments)")]
    [Tooltip("Kéo 4 mảnh đầu tiên vào đây để nó tự động ẩn/hiện")]
    public GameObject[] Fragments_1_to_4;
    [Tooltip("Kéo mảnh thứ 5 (mảnh kết thúc) vào đây")]
    public GameObject Fragment_5;
    [Tooltip("Tên của màn chơi tiếp theo (Map 3)")]
    public string NextSceneName = "L3_TênMapCủaBạn";

    [Header("Quái Vật & NPC")]
    public GameObject MonsterPrefab;
    public Transform SpawnPointClone;

    [Header("NPC Shadow (LÚC KẾT THÚC)")]
    public GameObject ShadowNPC;
    public Transform ShadowSpawnPointOnField;

    [Header("Hiệu ứng & Âm thanh")]
    public AudioSource YellAudioSource;
    public GameObject FlashBangEffect;

    [Header("Hệ thống Truy Sát (Mảnh 3)")]
    public TMP_Text CountdownText;
    private bool _isHuntCycleActive = false;
    private Transform _playerTransform;

    private float currentWalkSpeed = 3.5f;
    private float currentRunSpeed = 7.0f;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _playerTransform = playerObj.transform;
        if (CountdownText != null) CountdownText.text = "";

        // TẮT HẾT CÁC MẢNH VỠ KHI MỚI VÀO GAME
        foreach (var f in Fragments_1_to_4) { if (f != null) f.SetActive(false); }
        if (Fragment_5 != null) Fragment_5.SetActive(false);
    }

    // Hàm này sẽ được con Shadow gọi khi Minh trả bút chì
    public void ActivateInitialFragments()
    {
        foreach (var f in Fragments_1_to_4) { if (f != null) f.SetActive(true); }
        Debug.Log("Đã kích hoạt 4 mảnh vỡ đầu tiên!");
    }

    // Hàm này sẽ được gọi khi Shadow nói xong câu cuối cùng
    public void ActivateFragment5()
    {
        if (Fragment_5 != null) Fragment_5.SetActive(true);
        GameBroadcast.OnUpdateItemDescription?.Invoke("<color=yellow>A new fragment has appeared...</color>");
    }

    public void OnFragmentPickedUp()
    {
        FragmentsCollected++;
        Debug.Log("Fragment collected: " + FragmentsCollected);

        switch (FragmentsCollected)
        {
            case 1:
                GameBroadcast.OnUpdateItemDescription?.Invoke("It's getting faster...");
                currentWalkSpeed += 1.5f;
                currentRunSpeed += 2.0f;
                SyncAllMonsters();
                break;

            case 2:
                GameBroadcast.OnUpdateItemDescription?.Invoke("I hear... more footsteps!");
                L2_MonsterAI originalMonster = FindObjectOfType<L2_MonsterAI>();
                if (originalMonster != null && SpawnPointClone != null)
                {
                    GameObject clone = Instantiate(originalMonster.gameObject, SpawnPointClone.position, Quaternion.identity);
                    clone.SetActive(true);
                    clone.name = "TheOverexposed_Clone";

                    L2_MonsterAI cloneAI = clone.GetComponent<L2_MonsterAI>();
                    SyncAllMonsters();

                    if (_playerTransform != null)
                        StartCoroutine(DelayForceInvestigate(cloneAI, _playerTransform.position));
                }
                break;

            case 3:
                GameBroadcast.OnUpdateItemDescription?.Invoke("<color=red>THEY CAN SMELL YOU!</color>");
                if (YellAudioSource) YellAudioSource.Play();

                currentWalkSpeed += 1.0f;
                currentRunSpeed += 1.5f;
                SyncAllMonsters();

                _isHuntCycleActive = true;
                StartCoroutine(HuntCycleRoutine());
                break;

            case 4:
                GameBroadcast.OnUpdateItemDescription?.Invoke("Everything... has stopped.");
                _isHuntCycleActive = false;
                if (CountdownText != null) CountdownText.text = "";
                StartCoroutine(EndMazeSequence());
                break;

            case 5:
                // NHẶT MẢNH THỨ 5 -> QUA MÀN!
                GameBroadcast.OnUpdateItemDescription?.Invoke("Escaping the maze...");
                SceneManager.LoadScene(NextSceneName);
                break;
        }
    }

    private IEnumerator DelayForceInvestigate(L2_MonsterAI cloneAI, Vector3 targetPos)
    {
        yield return new WaitUntil(() => cloneAI != null && cloneAI.GetComponent<NavMeshAgent>() != null && cloneAI.GetComponent<NavMeshAgent>().isOnNavMesh);
        cloneAI.ForceInvestigate(targetPos, 45f);
    }

    private void SyncAllMonsters()
    {
        L2_MonsterAI[] monsters = FindObjectsOfType<L2_MonsterAI>();
        foreach (var m in monsters)
        {
            if (m != null)
            {
                m.WalkSpeed = currentWalkSpeed;
                m.RunSpeed = currentRunSpeed;
                m.PatrolRadius = 80f;
            }
        }
    }

    private IEnumerator HuntCycleRoutine()
    {
        while (_isHuntCycleActive)
        {
            if (CountdownText != null) CountdownText.color = Color.white;
            for (int i = 7; i > 0; i--)
            {
                if (!_isHuntCycleActive) yield break;
                if (CountdownText != null) CountdownText.text = i.ToString();
                yield return new WaitForSeconds(1f);
            }

            if (CountdownText != null) CountdownText.color = Color.red;
            for (int i = 3; i > 0; i--)
            {
                if (!_isHuntCycleActive) yield break;
                if (CountdownText != null) CountdownText.text = i.ToString();

                L2_MonsterAI[] monstersOnMap = FindObjectsOfType<L2_MonsterAI>();
                foreach (var monster in monstersOnMap)
                {
                    if (monster != null && _playerTransform != null)
                        monster.ForceInvestigate(_playerTransform.position);
                }
                yield return new WaitForSeconds(1f);
            }

            if (CountdownText != null) CountdownText.text = "";
            for (int i = 0; i < 10; i++)
            {
                if (!_isHuntCycleActive) yield break;
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private IEnumerator EndMazeSequence()
    {
        if (FlashBangEffect) FlashBangEffect.SetActive(true);

        yield return null;
        L2_MonsterAI[] monstersOnMap = FindObjectsOfType<L2_MonsterAI>();
        foreach (var monster in monstersOnMap)
        {
            if (monster != null) Destroy(monster.gameObject);
        }

        yield return new WaitForSeconds(1.5f);
        if (FlashBangEffect) FlashBangEffect.SetActive(false);

        if (ShadowNPC != null && ShadowSpawnPointOnField != null)
        {
            GameObject newShadow = Instantiate(ShadowNPC, ShadowSpawnPointOnField.position, ShadowSpawnPointOnField.rotation);
            newShadow.SetActive(true);
        }
    }
}