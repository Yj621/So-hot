using UnityEngine;
using System.Collections.Generic;

public class PlatformMover : MonoBehaviour
{
    [SerializeField] private Vector3 baseMoveOffset = new Vector3(10f, 0f, 0f);
    [SerializeField] private float baseSpeed = 1f;

    private Vector3 startPos;
    private Vector3 endPos;
    private float speed;
    private float randomOffset;

    private HashSet<Transform> fireObjects = new HashSet<Transform>();

    void Awake()
    {
        startPos = transform.position;

        Vector3 randomMoveOffset = baseMoveOffset + new Vector3(
            Random.Range(-2f, 2f),
            Random.Range(-0.5f, 0.5f),
            Random.Range(-2f, 2f)
        );

        endPos = startPos + randomMoveOffset;
        speed = baseSpeed * Random.Range(0.8f, 1.2f);
        randomOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void FixedUpdate()
    {
        float t = Mathf.PingPong(Time.time * speed + randomOffset, 1f);
        Vector3 newPos = Vector3.Lerp(startPos, endPos, t);
        Vector3 moveDelta = newPos - transform.position; // 이동한 거리 계산

        transform.position = newPos;

        // Fire 오브젝트만 직접 이동 처리
        foreach (var fire in fireObjects)
        {
            fire.position += moveDelta;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform); // 플레이어는 부모 설정
        }
        else if (other.CompareTag("Fire"))
        {
            fireObjects.Add(other.transform); // Fire는 직접 이동 처리
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null); // 플레이어는 발판에서 내려가면 부모 해제
        }
        else if (other.CompareTag("Fire"))
        {
            fireObjects.Remove(other.transform); // Fire 리스트에서 제거
        }
    }
}
