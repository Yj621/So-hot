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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpwanRockRoutine());
            StartCoroutine(SpwanCherryRoutine());
            StartCoroutine(SpwanBamBooRoutine());
        }
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


    void RockFall()
    {
        for (int i = 0; i < RockFallPoints.Length; i++)
        {
            GameObject rock = PhotonNetwork.Instantiate(RockPrefab.name, RockFallPoints[i].position, Quaternion.identity);
            Rigidbody rb = rock.GetComponent<Rigidbody>();

            rb.linearVelocity = Vector3.down * RockSpeed;

            Destroy(rock, RockDuration);
        }
    }

    

    void SpwanCherry()
    {
        for (int i = 0; i < CherryCollider.Length; i++)
        {
            Vector3 spawnPosition = GetPoint(CherryCollider[i]);
            PhotonNetwork.Instantiate(CherryPrefab.name, spawnPosition, Quaternion.identity);
            CurrentCherry++; // 생성된 아이템 개수 증가
        }
    }

    void SpwanBamBoo()
    {
        for (int i = 0; i < BamBooCollider.Length; i++)
        {
            Vector3 spawnPosition = GetRandomPointFromCollider(BamBooCollider[i]); // 해당 Collider에서 랜덤 위치 가져오기
            Quaternion rotation = Quaternion.Euler(0, 0, 90);
            GameObject bamBoo = PhotonNetwork.Instantiate(BamBooPrefab.name, spawnPosition, rotation);
            Rigidbody rb = bamBoo.GetComponent<Rigidbody>();

            // 왼쪽이면 Vector3.left, 오른쪽이면 Vector3.right
            Vector3 direction = (i == 0) ? Vector3.left : Vector3.right;
            rb.linearVelocity = direction * BamBooSpeed;

            Destroy(bamBoo, BamBooDuration);
        }
    }

        Vector3 GetPoint(BoxCollider collider)
    {
        Vector3 areaSize = collider.bounds.size;
        Vector3 areaCenter = collider.bounds.center;

        float randomX = Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2);
        float randomY = areaCenter.y; // dropHeight 대신 Collider 높이 기준
        float randomZ = Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2);

        return new Vector3(randomX, randomY, randomZ);
    }

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
