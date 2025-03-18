using UnityEngine;
using System.Collections;

namespace JW.PlatformSystem
{
    public class CloudPlatform : MonoBehaviour
    {
        [SerializeField] private float moveDistance = 2f; // 위로 올라가는 최대 거리
        [SerializeField] private float moveSpeed = 1f; // 움직이는 속도
        [SerializeField] private float waitTime = 1f; // 위에서 머무는 시간
        private Vector3 originalPosition;

        private void Start()
        {
            originalPosition = transform.position;
            StartCoroutine(MovePlatform());
        }

        private IEnumerator MovePlatform()
        {
            while (true)
            {
                // 랜덤 딜레이 적용 (각 구름마다 다르게 움직이도록)
                yield return new WaitForSeconds(Random.Range(0f, 2f));

                // 위로 이동
                yield return MoveToPosition(originalPosition + Vector3.up * moveDistance);

                // 머무는 시간
                yield return new WaitForSeconds(waitTime);

                // 아래로 이동 (원래 위치로 복귀)
                yield return MoveToPosition(originalPosition);
            }
        }

        private IEnumerator MoveToPosition(Vector3 targetPosition)
        {
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        // ✅ 플레이어나 불(Fire)이 올라오면 발판의 자식으로 설정
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Fire"))
            {
                other.transform.SetParent(transform);
            }
        }

        // ✅ 플레이어나 불(Fire)이 떠나면 부모 해제
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Fire"))
            {
                other.transform.SetParent(null);
            }
        }
    }
}
