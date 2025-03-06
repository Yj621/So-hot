using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("캐릭터 회전 속도")]
    [Range(0, 3)]
    public float rotationSmoothTime = 0.1f;

    private Camera camera;
    private float rotationVelocity = 0;

    public GameObject playerCameraRoot;

    private void Awake()
    {
        camera = Camera.main;
    }

    public void Rotate()
    {
        // 대상이 될 카메라의 y축 각도
        float targetRotation = camera.transform.eulerAngles.y;
        // 현재 캐릭터의 y축 각도를 SmoothDampAngle로 대상 각도로 회전시켜준다.
        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);
        // 구해진 rotation을 Quaternion.Euler에 y축 각도로 넣어주고 transform.rotation에 적용
        transform.rotation = Quaternion.Euler(0, rotation, 0);
    }
}