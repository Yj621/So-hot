using UnityEngine;

    public class PlatformMover : MonoBehaviour
    {


        [SerializeField] Vector3 moveOffset = new Vector3(10f, 0f, 0f); // 이동할 거리 및 방향
        [SerializeField] float speed = 1f; // 이동 속도

        private Vector3 startPos;
        private Vector3 endPos;

        void Awake()
        {
            startPos = transform.position; // 시작 위치를 현재 위치로 설정
            endPos = startPos + moveOffset; // 목표 위치를 상대적으로 설정
        }

        void FixedUpdate()
        {
            float t = Mathf.PingPong(Time.time * speed, 1f); // 0과 1 사이 반복
            transform.position = Vector3.Lerp(startPos, endPos, t); // 두 위치 간의 부드러운 이동
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.transform.SetParent(transform);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.transform.SetParent(null);
            }
        }

    }
