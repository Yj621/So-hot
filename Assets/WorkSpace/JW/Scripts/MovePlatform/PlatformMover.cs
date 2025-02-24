using UnityEngine;


namespace JW.PlatformSystem
{
    public class PlatformMover : MonoBehaviour
    {
        [SerializeField] Vector3 startPos = new Vector3(-159.54f, 64.82f, -1042.46f); // 시작 위치
        [SerializeField] Vector3 endPos = new Vector3(-140.14f, 50.56f, -1040.51f); // 목표 위치
        [SerializeField] float speed = 10; //이동 속도 

     void Start()
        {
            transform.position = startPos;
        }

    
        void FixedUpdate()
        {
            float t = Mathf.PingPong(Time.time * speed, 1f);    //0과 1 사이 값을 반복적으로 생성
            transform.position = Vector3.Lerp(startPos, endPos,t);  // 두 위치 간의 부드러운 이동
        }
    }
}