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
    public GameObject CherryHitEffect; // 🍒 체리 피격 이펙트

    public BoxCollider[] BamBooCollider;
    public GameObject BamBooPrefab;
    public float ShootInterval = 0.1f;
    public float BamBooSpeed = 5f;
    public float BamBooDuration = 5f;
    public GameObject HitEffect;  // 피격 이펙트 프리팹

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

    void RockFall()
    {
        foreach (Transform point in RockFallPoints)
        {
            GameObject rock = PhotonNetwork.Instantiate(RockPrefab.name, point.position, Quaternion.identity);
            Rigidbody rb = rock.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.down * RockSpeed;
            }

            StartCoroutine(DestroyAfterTime(rock, RockDuration));
        }
    }

    void SpwanCherry()
    {
        foreach (BoxCollider collider in CherryCollider)
        {
            Vector3 spawnPosition = GetPoint(collider);
            PhotonNetwork.Instantiate(CherryPrefab.name, spawnPosition, Quaternion.identity);
        }
    }

    void SpwanBamBoo()
    {
        for (int i = 0; i < BamBooCollider.Length; i++)
        {
            Vector3 spawnPosition = GetRandomPointFromCollider(BamBooCollider[i]);
            Quaternion rotation = Quaternion.Euler(0, 0, 90);

            // 모든 클라이언트에서 실행하도록 RPC 호출
            photonView.RPC("RpcSpwanBamBoo", RpcTarget.AllBuffered, spawnPosition, rotation);
        }
    }


    [PunRPC]
    void RpcSpwanBamBoo(Vector3 position, Quaternion rotation)
    {
        GameObject bamBoo = PhotonNetwork.Instantiate(BamBooPrefab.name, position, rotation);
        Rigidbody rb = bamBoo.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction = position.x < 0 ? Vector3.right : Vector3.left;
            rb.linearVelocity = direction * BamBooSpeed;
        }

        // 죽창이 플레이어를 맞췄을 때의 처리만 남김
        BamBooProjectile bamBooScript = bamBoo.AddComponent<BamBooProjectile>();
        bamBooScript.Setup(this, HitEffect);

        StartCoroutine(DestroyAfterTime(bamBoo, BamBooDuration));
    }

    [PunRPC]
    void RpcSpwanCherry(Vector3 position, Quaternion rotation)
    {
        GameObject cherry = PhotonNetwork.Instantiate(CherryPrefab.name, position, rotation);

        // ✅ 체리에 피격 이펙트 적용
        CherryProjectile cherryScript = cherry.AddComponent<CherryProjectile>();
        cherryScript.Setup(this, CherryHitEffect); // 🍒 체리 피격 이펙트 전달
    }


    IEnumerator DestroyAfterTime(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (obj != null && obj.GetComponent<PhotonView>().IsMine)
        {
            PhotonNetwork.Destroy(obj);
        }
    }

    Vector3 GetPoint(BoxCollider collider)
    {
        Vector3 areaSize = collider.bounds.size;
        Vector3 areaCenter = collider.bounds.center;

        float randomX = Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2);
        float randomY = areaCenter.y;
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
