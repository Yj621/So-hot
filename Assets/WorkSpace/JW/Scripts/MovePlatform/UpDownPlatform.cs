using UnityEditor.Timeline;
using UnityEngine;

public class UpDownPlatform : MonoBehaviour
{
    [SerializeField] float minY = 21.91f; //최소값
    [SerializeField] float maxY = 50.11f;  //최대값
    [SerializeField] float speed = 2f;  //이동 속도

    private float distance;

    void Start()
    {
        distance = maxY - minY; //이동 거리 계산
    }

    
    void FixedUpdate()
    {
        float newY = Mathf.PingPong(Time.time * speed, distance)+ minY;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
