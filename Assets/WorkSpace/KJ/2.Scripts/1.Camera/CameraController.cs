using UnityEngine;
using Photon.Pun;

namespace KJ.CameraSystem
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform playerBody; // 플레이어 캐릭터
        [SerializeField] private Vector3 offset = new Vector3(0, 2, -4); // 카메라 오프셋
        [SerializeField] private float followSpeed = 10f; // 카메라 따라가는 속도
        [SerializeField] private float mouseSensitivity = 3.5f; // 마우스 감도
        [SerializeField] private float moveSpeed = 6.5f; // 플레이어 이동 속도

        private float pitch = 0.0f;
        private float yaw = 0.0f;
        private const float minPitch = -10f;
        private const float maxPitch = 45f;

        private CharacterController controller;
        
        public Transform PlayerBody
        {
            get
            {
                return playerBody;
            }
            set
            {
                playerBody = value;
            }
        }

        private void Start()
        {
           // Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;

            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                PhotonView pv = player.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)
                {
                    playerBody = player.transform;
                    controller = player.GetComponent<CharacterController>();
                    break;
                }
            }
        }

        private void Update()
        {
            if (playerBody == null) return;

            HandleMouseLook();
            HandleMovement();
        }

        private void LateUpdate()
        {
            if (playerBody == null) return;

            MoveCamera(); // 카메라가 플레이어를 부드럽게 따라가도록 설정
        }

        private void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            // 카메라 회전 (플레이어는 회전하지 않음)
            transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        }

        private void MoveCamera()
        {
            // 카메라를 플레이어 위치 + 오프셋으로 이동 (부드럽게 따라가도록 Lerp 적용)
            Vector3 targetPosition = playerBody.position + transform.rotation * offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        }

        private void HandleMovement()
        {
            if (controller == null) return;

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // 카메라가 바라보는 방향 기준으로 이동
            Vector3 moveDirection = transform.forward * vertical + transform.right * horizontal;
            moveDirection.y = 0;

            if (moveDirection.magnitude > 0.1f)
            {
                controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
            }
        }
    }
}
