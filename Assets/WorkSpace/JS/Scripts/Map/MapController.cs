using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MapController : MonoBehaviourPunCallbacks
{
    public Transform[] RockFallPoints;
    public GameObject RockPrefab;
    public float RockSpeed = 5f;
    public float RockDuration = 5f;
    public float RockSpwanTime = 5f;

    public BoxCollider[] CherryCollider;
    public GameObject CherryPrefab;
    public float DropInterval = 0.1f;
    private int CurrentCherry = 0;

    public BoxCollider[] BamBooCollider;
    public GameObject BamBooPrefab;
    public float ShootInterval = 0.1f;
    public float BamBooSpeed = 5f;
    public float BamBooDuration = 5f;

    // 마스터 클라이언트만 오브젝트 생성
    IEnumerator CheckMasterClient()
    {
        yield return new WaitForSeconds(5f);

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpwanRockRoutine());
            StartCoroutine(SpwanCherryRoutine());
            StartCoroutine(SpwanBamBooRoutine());
        }
    }

    void Start()
    {
        StartCoroutine(CheckMasterClient());
    }

    private IEnumerator SpwanRockRoutine()
    {
        while (true)
        {
            RockFall();
            yield return new WaitForSeconds(RockSpwanTime);
        }
    }

    private IEnumerator SpwanCherryRoutine()
    {
        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                SpwanCherry();
                yield return new WaitForSeconds(DropInterval);
            }
        }
    }

    private IEnumerator SpwanBamBooRoutine()
    {
        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                SpwanBamBoo();
                yield return new WaitForSeconds(ShootInterval);
            }
        }
    }

    // 바위 생성
    void RockFall()
    {
        for (int i = 0; i < RockFallPoints.Length; i++)
        {
            GameObject rock = PhotonNetwork.Instantiate(RockPrefab.name, RockFallPoints[i].position, Quaternion.identity);
            Rigidbody rb = rock.GetComponent<Rigidbody>();

            rb.linearVelocity = Vector3.down * RockSpeed;

            StartCoroutine(DestroyAfterTime(rock, RockDuration));
        }
    }

    // 체리 생성 (삭제 로직 없음, Cherry.cs에서 자동 삭제)
    void SpwanCherry()
    {
        for (int i = 0; i < CherryCollider.Length; i++)
        {
            Vector3 spawnPosition = GetPoint(CherryCollider[i]);
            PhotonNetwork.Instantiate(CherryPrefab.name, spawnPosition, Quaternion.identity);
            CurrentCherry++;
        }
    }

    // 대나무 생성
    void SpwanBamBoo()
    {
        for (int i = 0; i < BamBooCollider.Length; i++)
        {
            Vector3 spawnPosition = GetRandomPointFromCollider(BamBooCollider[i]);
            Quaternion rotation = Quaternion.Euler(0, 0, 90);
            GameObject bamBoo = PhotonNetwork.Instantiate(BamBooPrefab.name, spawnPosition, rotation);
            Rigidbody rb = bamBoo.GetComponent<Rigidbody>();

            Vector3 direction = (i == 0) ? Vector3.left : Vector3.right;
            rb.linearVelocity = direction * BamBooSpeed;

            StartCoroutine(DestroyAfterTime(bamBoo, BamBooDuration));
        }
    }

    // 일정 시간이 지나면 네트워크 오브젝트 삭제
    IEnumerator DestroyAfterTime(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);

        if (obj != null && obj.GetComponent<PhotonView>().IsMine)
        {
            PhotonNetwork.Destroy(obj);
        }
    }

    // 체리 스폰 위치 계산
    Vector3 GetPoint(BoxCollider collider)
    {
        Vector3 areaSize = collider.bounds.size;
        Vector3 areaCenter = collider.bounds.center;

        float randomX = Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2);
        float randomY = areaCenter.y;
        float randomZ = Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2);

        return new Vector3(randomX, randomY, randomZ);
    }

    // 대나무 스폰 위치 계산
    Vector3 GetRandomPointFromCollider(BoxCollider collider)
    {
        Vector3 areaSize = collider.bounds.size;
        Vector3 areaCenter = collider.bounds.center;

        float randomX = areaCenter.x;
        float randomY = Random.Range(areaCenter.y - areaSize.y / 2, areaCenter.y + areaSize.y / 2);
        float randomZ = Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2);

        return new Vector3(randomX, randomY, randomZ);
    }
}
