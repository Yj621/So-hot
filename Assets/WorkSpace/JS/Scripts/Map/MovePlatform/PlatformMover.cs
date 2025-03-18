using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    [SerializeField] private Vector3 baseMoveOffset = new Vector3(10f, 0f, 0f); // 기본 이동 거리
    [SerializeField] private float baseSpeed = 1f; // 기본 이동 속도

    private Vector3 startPos;
    private Vector3 endPos;
    private float speed;
    private float randomOffset;

    void Awake()
    {
        startPos = transform.position;

        // 각 발판마다 이동 거리와 속도를 랜덤하게 조정
        Vector3 randomMoveOffset = baseMoveOffset + new Vector3(
            Random.Range(-2f, 2f), // X축 이동 범위 조정
            Random.Range(-0.5f, 0.5f), // Y축 이동 범위 조정 (약간의 높낮이 변화)
            Random.Range(-2f, 2f)  // Z축 이동 범위 조정
        );

        endPos = startPos + randomMoveOffset;

        // 이동 속도도 약간 랜덤하게 설정
        speed = baseSpeed * Random.Range(0.8f, 1.2f);

        // 각 발판의 움직임을 랜덤한 시간 차이로 시작하도록 설정
        randomOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void FixedUpdate()
    {
        float t = Mathf.PingPong(Time.time * speed + randomOffset, 1f);
        transform.position = Vector3.Lerp(startPos, endPos, t);
    }

    void OnTriggerEnter(Collider other)
    {
        // 플레이어나 불(Fire)이 발판에 올라오면 부모로 설정
        if (other.CompareTag("Player") || other.CompareTag("Fire"))
        {
            other.transform.SetParent(transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 플레이어나 불(Fire)이 발판에서 내려가면 부모 해제
        if (other.CompareTag("Player") || other.CompareTag("Fire"))
        {
            other.transform.SetParent(null);
        }
    }
}
