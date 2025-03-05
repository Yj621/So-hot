using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform playerBody;  // 캐릭터 본체 (좌우 회전)
    public Vector3 cameraTransform; // 카메라 (위아래 회전)

    public float mouseSensitivity = 3f; // 마우스 감도
    private float xRotation = 0f; // 위아래 회전 값


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 고정 (ESC로 해제 가능)
    }

    void Update()
    {
        RotateView();
    }

    void RotateView()
    {
        // 마우스 입력값 가져오기
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 좌우 회전 (캐릭터 본체 회전)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
