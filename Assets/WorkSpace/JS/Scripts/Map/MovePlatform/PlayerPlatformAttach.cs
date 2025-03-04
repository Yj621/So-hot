using UnityEngine;


namespace JW.PlatformSystem
{
    public class PlayerPlatformAttach : MonoBehaviour
    {
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                transform.SetParent(collision.transform);  // 자식으로 추가
            }
        }
        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                transform.SetParent(null);  // 플랫폼에서 분리
            }
        }
    }
}