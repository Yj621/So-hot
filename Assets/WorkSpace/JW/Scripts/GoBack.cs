using UnityEngine;

public class GoBack : MonoBehaviour
{
    public float bounceForce = 10f;    // 위로 튀는 힘
    public float backwardForce = 5f;   // 뒤로 밀려나는 힘
    

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // 플레이어가 바라보는 반대 방향 + 위쪽
                Vector3 bounceDirection = (-collision.transform.forward + Vector3.up).normalized;

                playerRb.linearVelocity = Vector3.zero; // 기존 속도 제거
                playerRb.AddForce(bounceDirection * bounceForce, ForceMode.Impulse); // 튀기기
                playerRb.AddForce(-collision.transform.forward * backwardForce, ForceMode.Impulse); // 뒤로 밀기
            }
        }
    }
}
