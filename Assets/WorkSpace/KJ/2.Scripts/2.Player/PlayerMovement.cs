using UnityEngine;
using Photon.Pun;
using System.Collections;
using YJ.UIManager;

namespace KJ.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviourPun
    {
        [Header("이동 설정")]
        [SerializeField] private float walkSpeed = 5f; // 걷기 속도
        [SerializeField] private float runSpeed = 8f; // 달리기 속도
        [SerializeField] private float jumpForce = 5f; // 점프 시 힘
        [SerializeField] private LayerMask groundLayer; // 지면 판별을 위한 레이어
        [SerializeField] private float rotationSpeed = 10f; // 회전 속도


        private Rigidbody rb; // Rigidbody 컴포넌트
        private bool isGrounded; // 지면 여부 확인
        private float currentSpeed; // 현재 속도
        private Vector3 moveDirection; // 이동 방향 벡터
        private Animator animator; // 애니메이터 컴포넌트
        private bool isJumping = false; // 점프 여부

        public bool IsGrounded => isGrounded;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true; // 물리 회전 방지
            currentSpeed = walkSpeed;
            animator = GetComponent<Animator>();

            UIManager.Instance.UpdateStaminaUI(); // UI 초기값 설정
        }

        void Update()
        {
            if (!photonView.IsMine) return; // 본인 캐릭터만 조작 가능

            HandleMovementInput(); // 이동 입력 처리
            CheckGround(); // 지면 체크

            if (moveDirection.magnitude >= 0.1f)
            {
                RotateCharacter(); // 캐릭터 회전
            }

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
            {
                Jump(); // 점프 실행
            }
        }

        void FixedUpdate()
        {
            if (!photonView.IsMine) return;
            Move(); // 물리 이동 처리
        }

        private void HandleMovementInput()
        {
            // 입력 값을 받아 이동 방향 설정
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            // 카메라 방향을 기준으로 이동 방향 설정
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            moveDirection = (cameraForward * moveZ + cameraRight * moveX).normalized;

            // 스프린트(달리기) 처리
            if (Input.GetKey(KeyCode.LeftShift))
            {
                //스태미나 게이지 활성화
                UIManager.Instance.ActiveStamina();

                if (!UIManager.Instance.runLimit)
                {
                    currentSpeed = runSpeed;
                }
                else if (UIManager.Instance.currentStamina > 0)
                {
                    currentSpeed = runSpeed;
                    UIManager.Instance.DrainStamina(); // 스태미나 감소
                    Debug.Log("대쉬중");
                }
                else
                {
                    currentSpeed = walkSpeed;
                }
            }
            else
            {
                //스태미나 게이지 비활성화
                UIManager.Instance.DeactiveStamina();

                currentSpeed = walkSpeed;
                UIManager.Instance.RecoverStamina(); // 스태미나 회복
            }

            float speedNormalized = moveDirection.magnitude > 0.1f ? GetCurrentSpeedNormalized() : 0f;
            animator.SetFloat("Speed", speedNormalized); // 애니메이션 속도 설정
        }

        private void Move()
        {
            if (moveDirection.magnitude >= 0.1f)
            {
                Vector3 moveVelocity = moveDirection * currentSpeed;
                rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z); // 물리 이동 적용
            }
        }

        private void RotateCharacter()
        {
            // 이동 방향으로 부드럽게 회전
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        private void Jump()
        {
            if (isJumping) return;

            isJumping = true;
            isGrounded = false;
            animator.SetTrigger("Jump"); // 점프 애니메이션 실행
        }

        public void OnJumpStart()
        {
            // 점프 시 Y축 속도 초기화 후 점프력 적용
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }

        private void CheckGround()
        {
            // 지면 체크를 위한 Raycast
            RaycastHit hit;
            float rayLength = 1.1f;

            if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, groundLayer))
            {
                isGrounded = true;
                isJumping = false;
                animator.SetBool("isGrounded", true);
            }
            else
            {
                isGrounded = false;
                animator.SetBool("isGrounded", false);
            }
        }

        public void RecoverFullStamina()
        {
            UIManager.Instance.currentStamina = UIManager.Instance.maxStamina;
            UIManager.Instance.UpdateStaminaUI(); // UI 업데이트
        }

        public float GetCurrentSpeedNormalized()
        {
            return currentSpeed / runSpeed; // 현재 속도를 최대 달리기 속도로 정규화하여 반환
        }
    }
}