using UnityEngine;

public class GoBack : MonoBehaviour
{
    [SerializeField] private float moveDuration = 1f;    // 이동 시간
    [SerializeField] private AnimationCurve arcCurve;    // 포물선 곡선

    private Vector3 returnPosition = new Vector3(-131f, 0.5f, -1013f);  // 돌아갈 기본 위치

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.isKinematic = true;  // 떨림 방지
                StartCoroutine(MovePlayerToTarget(collision.transform, playerRb));
            }
        }
    }

    System.Collections.IEnumerator MovePlayerToTarget(Transform player, Rigidbody playerRb)
    {
        Vector3 startPos = player.position;
        float elapsed = 0f;

        // 포물선의 최대 높이를 설정 (필요시 조정)
        float maxHeight = 7f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            // 수평 이동 (Lerp)
            Vector3 horizontalMove = Vector3.Lerp(startPos, returnPosition, t);

            // 포물선 높이 계산 (arcCurve를 사용하여 극적인 변화)
            float height = arcCurve != null ? arcCurve.Evaluate(t) * maxHeight : Mathf.Sin(Mathf.PI * t) * maxHeight;

            // 최종 위치 적용
            player.position = new Vector3(horizontalMove.x, horizontalMove.y + height, horizontalMove.z);

            yield return null;
        }

        playerRb.isKinematic = false; // 이동 후 물리 재활성화
    }
}
