using UnityEngine;

public class UpDownPlatform : MonoBehaviour
{
    [SerializeField] private float moveRange = 5f; // 위아래 이동 범위
    [SerializeField] private float speed = 2f;  // 이동 속도

    private float startY;

    void Start()
    {
        startY = transform.position.y; // 초기 위치 저장
    }

    void FixedUpdate()
    {
        float newY = Mathf.PingPong(Time.time * speed, moveRange * 2) + (startY - moveRange);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
