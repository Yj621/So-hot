using UnityEngine;


namespace JW.PlatformSystem
{
    public class CloudPlatform : MonoBehaviour
    {
        [SerializeField] float bounceForce = 15f; //점프 힘

        void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.CompareTag("Player"))
            {
                Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

                if(playerRb != null)    
                {
                    //기존 Y 속도 제거 후 위로 힘 추가
                    Vector3 velocity = playerRb.linearVelocity;
                    velocity.y = 0; //기존 y 속도 제거
                    playerRb.linearVelocity = velocity;

                    playerRb.AddForce(Vector3.up * bounceForce, ForceMode.VelocityChange);
                }
            }
        }
    }
}