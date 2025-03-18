using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JW.PlatformSystem
{
    public class CloudPlatform : MonoBehaviour
    {
        [SerializeField] private float moveDistance = 2f; // 위로 올라가는 최대 거리
        [SerializeField] private float moveSpeed = 1f; // 움직이는 속도
        [SerializeField] private float waitTime = 1f; // 위에서 머무는 시간
        private Vector3 originalPosition;
        private HashSet<Transform> fireObjects = new HashSet<Transform>(); // Fire 오브젝트 관리

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
                Vector3 prevPosition = transform.position; // 이전 위치 저장
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                Vector3 moveDelta = transform.position - prevPosition; // 이동한 거리 계산

                // Fire 오브젝트만 직접 이동
                foreach (var fire in fireObjects)
                {
                    fire.position += moveDelta;
                }

                yield return null;
            }
        }

        // ✅ 플레이어는 부모 설정
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.transform.SetParent(transform);
            }
            else if (other.CompareTag("Fire"))
            {
                fireObjects.Add(other.transform); // Fire는 직접 이동 처리
            }
        }

        // ✅ 플레이어는 부모 해제, Fire는 리스트에서 제거
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.transform.SetParent(null);
            }
            else if (other.CompareTag("Fire"))
            {
                fireObjects.Remove(other.transform);
            }
        }
    }
}
