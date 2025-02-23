using UnityEngine;
using Photon.Pun;

namespace KJ.Player
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

        [Header("스태미나 설정")]
        public bool runLimit;   // 달릴 시 스태미나 제한을 받는지 여부
        [SerializeField] private float maxStamina = 100f;   // 최대 스태미나
        private float currentStamina;   // 현재 스태미나
        [SerializeField] private float staminaDrainRate = 10f;   // 초당 스태미나 감소량

        private Rigidbody rb; // Rigidbody 컴포넌트 참조
        private bool isGrounded; // 플레이어가 지면에 있는지 여부
        private float currentSpeed; // 현재 이동 속도 (걷기 또는 달리기 속도 반영)
        private Vector3 moveDirection; // 이동 방향 벡터
        private Animator animator;
        private bool isJumping = false;   // 점프 상태를 추적하는 변수 추가

        public bool IsGrounded => isGrounded;

        void Start()
        {
            rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
            rb.freezeRotation = true; // 회전 고정 (물리적인 회전 방지, 넘어지지 않도록 설정)
            currentSpeed = walkSpeed; // 기본 속도를 걷기 속도로 설정
            currentStamina = maxStamina;
            animator = GetComponent<Animator>();
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
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
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

            // Shift 키를 누르면 달리기 시도
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (!runLimit)   // 아이템 사용 상태 : 무조건 달리기 가능
                {
                    currentSpeed = runSpeed;
                }
                else if (currentStamina > 0)   // 기본 상태 : 스태미나가 남아 있을 때만 가능
                {
                    currentSpeed = runSpeed;
                    DrainStamina();
                }
                else
                {
                    currentSpeed = walkSpeed;   // 스태미나가 없으면 걷기
                }
            }
            else
            {
                currentSpeed = walkSpeed;   // 기본 이동 속도 유지

                currentStamina += staminaDrainRate * Time.deltaTime;
                if (currentStamina > maxStamina)
                {
                    currentStamina = maxStamina;
                }
            }

            // 애니메이션 속도 적용
            float speedNormalized = moveDirection.magnitude > 0.1f ? GetCurrentSpeedNormalized() : 0f;
            animator.SetFloat("Speed", speedNormalized);
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
            isJumping = true; // 점프 상태 시작

            // 점프 애니메이션과 점프 동작을 동시에 실행
            animator.SetTrigger("Jump");
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
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
                isJumping = false; // 착지하면 점프 상태 해제
            }
            else
            {
                isGrounded = false;
            }

            // 애니메이터에 isGrounded 상태 전달
            animator.SetBool("isGrounded", isGrounded);
        }

        private void DrainStamina()
        {
            if (currentStamina <= 0)
            {
                currentStamina = 0;   // 스태미나가 0 이하로 내려가지 않도록 제한
            }
            else
            {
                currentStamina -= staminaDrainRate * Time.deltaTime;
            }
        }

        public float GetCurrentSpeedNormalized()
        {
            return currentSpeed / runSpeed;
        }
    }
}
