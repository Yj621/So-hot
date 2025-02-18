using UnityEngine;
using Photon.Pun; // Photon 네트워크 기능을 사용하기 위해 추가

namespace KJ.PlayerMovement
{
    // Rigidbody를 필요로 하는 컴포넌트임을 명시하여, 실수로 제거되지 않도록 함
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviourPun
    {
        [Header("이동 설정")]
        [SerializeField] private float walkSpeed = 5f; // 기본 이동 속도 (걷기 속도)
        [SerializeField] private float runSpeed = 8f; // 달리기 속도 (Shift 키를 누를 때 적용)
        [SerializeField] private float jumpForce = 5f; // 점프의 힘 (Y축 방향으로 가해지는 힘)
        [SerializeField] private LayerMask groundLayer; // 지면 감지를 위한 레이어 (바닥 체크에 사용)
        [SerializeField] private float rotationSpeed = 10f; // 회전 속도

        private Rigidbody rb; // Rigidbody 컴포넌트 참조
        private bool isGrounded; // 플레이어가 지면에 있는지 여부
        private float currentSpeed; // 현재 이동 속도 (걷기 또는 달리기 속도 반영)
        private Vector3 moveDirection; // 이동 방향 벡터

        void Start()
        {
            rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
            rb.freezeRotation = true; // 회전 고정 (물리적인 회전 방지, 넘어지지 않도록 설정)
            currentSpeed = walkSpeed; // 기본 속도를 걷기 속도로 설정
        }

        void Update()
        {
            // 포톤 네트워크에서 본인의 캐릭터만 조작할 수 있도록 제한
            if (!photonView.IsMine) return;

            HandleMovementInput(); // 이동 입력 처리
            CheckGround(); // 지면 체크 (플레이어가 땅에 있는지 확인)

            if (moveDirection.magnitude >= 0.1f) // 이동 입력이 있을 때만 회전 적용
            {
                RotateCharacter(); // 이동 방향에 따라 캐릭터 회전
            }

            // 점프 입력 처리 (스페이스바를 누르고 지면에 있을 때만 점프 가능)
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                Jump();
            }
        }

        void FixedUpdate()
        {
            // 포톤 네트워크에서 본인의 캐릭터만 조작할 수 있도록 제한
            if (!photonView.IsMine) return;

            Move(); // 이동 처리 (물리 기반 이동 적용)
        }

        /// <summary>
        /// 이동 입력을 감지하여 이동 방향과 속도를 설정하는 함수
        /// </summary>
        private void HandleMovementInput()
        {
            float moveX = Input.GetAxis("Horizontal"); // 좌우(A, D 또는 화살표 좌우) 입력 감지
            float moveZ = Input.GetAxis("Vertical"); // 앞뒤(W, S 또는 화살표 상하) 입력 감지

            Vector3 cameraForward = Camera.main.transform.forward; // 카메라의 정면 벡터
            Vector3 cameraRight = Camera.main.transform.right; // 카메라의 오른쪽 벡터
            cameraForward.y = 0; // Y축 회전 방지 (카메라 기울기 무시)
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            moveDirection = (cameraForward * moveZ + cameraRight * moveX).normalized; // 카메라 기준 이동 방향 계산

            // 왼쪽 Shift 키를 누르면 달리기 속도 적용, 그렇지 않으면 걷기 속도 유지
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        }

        /// <summary>
        /// Rigidbody를 활용하여 이동 처리하는 함수
        /// </summary>
        private void Move()
        {
            if (moveDirection.magnitude >= 0.1f) // 이동 입력이 존재하는 경우만 실행
            {
                Vector3 moveVelocity = moveDirection * currentSpeed;
                rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z); // Y축 속도는 기존 값을 유지하여 점프 유지
            }
        }

        /// <summary>
        /// 플레이어가 이동 방향을 바라보도록 회전하는 함수
        /// </summary>
        private void RotateCharacter()
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection); // 목표 회전 방향 설정
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // 부드러운 회전 적용
        }

        /// <summary>
        /// 플레이어가 점프하는 함수 (Y축 방향으로 힘을 가함)
        /// </summary>
        private void Jump()
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 순간적인 힘(Impulse)로 점프 적용
        }

        /// <summary>
        /// 플레이어가 지면에 있는지 체크하는 함수 (Raycast 사용)
        /// </summary>
        private void CheckGround()
        {
            RaycastHit hit;
            float rayLength = 1.1f; // 바닥 감지 거리 설정 (캐릭터의 높이에 맞게 조정 필요)

            // 바닥을 향해 Raycast를 쏘고, groundLayer에 맞닿아 있으면 지면에 있다고 판정
            if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, groundLayer))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }
    }
}
