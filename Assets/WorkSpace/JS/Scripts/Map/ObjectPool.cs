using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab; // 풀링할 프리팹
    public int poolSize = 10; // 미리 생성할 개수
    private Queue<GameObject> objectPool = new Queue<GameObject>();

    void Start()
    {
        // 미리 오브젝트 생성해서 큐에 저장
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = PhotonNetwork.Instantiate(prefab.name, Vector3.zero, Quaternion.identity);
            obj.SetActive(false); // 비활성화해서 대기 상태로 둠
            objectPool.Enqueue(obj);
        }
    }

    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        if (objectPool.Count > 0)
        {
            GameObject obj = objectPool.Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true); // 활성화해서 사용
            return obj;
        }
        else
        {
            // 풀에 남은 오브젝트가 없으면 새로 생성
            GameObject newObj = PhotonNetwork.Instantiate(prefab.name, position, rotation);
            return newObj;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }
}
