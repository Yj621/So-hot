using UnityEngine;
using Photon.Pun;

namespace KJ.CameraSystem
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform target; // 카메라가 따라갈 플레이어
        [SerializeField] private Vector3 offset = new Vector3(0, 2, -4); // 카메라 위치 오프셋 (살짝 위, 뒤쪽)
        [SerializeField] private float followSpeed = 5f; // 플레이어 따라가는 속도
        [SerializeField] private float rotationSpeed = 5f; // 카메라 회전 속도
        [SerializeField] private float mouseSensitivity = 2.0f; // 마우스 감도
        private float pitch = 0.0f; // 위아래 각도
        private float yaw = 0.0f; // 좌우 회전값
        private float minPitch = -30f; // 위아래 최소 각도
        private float maxPitch = 60f; // 위아래 최대 각도
        private Transform playerBody; // 플레이어 본체 회전용
        private CharacterController controller; // 플레이어 이동 컨트롤러

        [SerializeField] private float moveSpeed = 5f; // 플레이어 이동 속도

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 고정
            Cursor.visible = false; // 마우스 숨김

            // 포톤을 이용해 로컬 플레이어 찾기
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                PhotonView pv = player.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)
                {
                    target = player.transform;
                    playerBody = player.transform;
                    controller = player.GetComponent<CharacterController>(); // 플레이어 이동 제어할 컨트롤러 가져오기
                    break;
                }
            }
        }

        private void Update()
        {
            if (target == null) return;

            HandleMouseLook(); // 마우스 입력 처리
            HandleMovement(); // 플레이어 이동 처리
        }

        private void LateUpdate()
        {
            if (target == null) return;

            RotateCamera(); // 카메라 회전 처리
            MoveCamera(); // 카메라 이동 처리
        }

        private void HandleMouseLook()
        {
            // 마우스 움직임을 감지하여 자동으로 카메라를 회전
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            yaw += mouseX; // 좌우 회전 적용
            pitch -= mouseY; // 위아래 반전 (반대로 적용)
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch); // 위아래 각도 제한
        }

        private void RotateCamera()
        {
            // 카메라의 회전값 적용
            Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // 플레이어 방향도 자동으로 변경
            if (playerBody != null)
            {
                Quaternion playerTargetRotation = Quaternion.Euler(0, yaw, 0);
                playerBody.rotation = Quaternion.Slerp(playerBody.rotation, playerTargetRotation, Time.deltaTime * rotationSpeed);
            }
        }

        private void MoveCamera()
        {
            // 목표 위치 = 플레이어 위치 + 회전된 오프셋
            Vector3 targetPosition = target.position + transform.rotation * offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        }

        private void HandleMovement()
        {
            if (controller == null) return;

            float horizontal = Input.GetAxis("Horizontal"); // A, D 키 (좌우)
            float vertical = Input.GetAxis("Vertical"); // W, S 키 (앞뒤)

            Vector3 moveDirection = transform.forward * vertical + transform.right * horizontal;
            moveDirection.y = 0; // 점프 기능이 없으므로 y축 이동 제거

            if (moveDirection.magnitude > 0.1f) // 입력이 있을 때만 이동
            {
                controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
            }
        }
    }
}
