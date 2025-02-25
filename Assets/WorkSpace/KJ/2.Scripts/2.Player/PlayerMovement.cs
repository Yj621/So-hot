using UnityEngine;
using Photon.Pun;
using System.Collections;

namespace KJ.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviourPun
    {
        [Header("이동 설정")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 8f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("스태미나 설정")]
        public bool runLimit;
        [SerializeField] private float maxStamina = 100f;
        private float currentStamina;
        [SerializeField] private float staminaDrainRate = 10f;

        [Header("UI 설정")]
        [SerializeField] private KJ.UI.StaminaUIController staminaUI; // 스태미나 UI 연동

        private Rigidbody rb;
        private bool isGrounded;
        private float currentSpeed;
        private Vector3 moveDirection;
        private Animator animator;
        private bool isJumping = false;

        public bool IsGrounded => isGrounded;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            currentSpeed = walkSpeed;
            currentStamina = maxStamina;
            animator = GetComponent<Animator>();

            UpdateStaminaUI(); // UI 초기값 설정
        }

        void Update()
        {
            if (!photonView.IsMine) return;

            HandleMovementInput();
            CheckGround();

            if (moveDirection.magnitude >= 0.1f)
            {
                RotateCharacter();
            }

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
            {
                Jump();
            }
        }

        void FixedUpdate()
        {
            if (!photonView.IsMine) return;
            Move();
        }

        private void HandleMovementInput()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            moveDirection = (cameraForward * moveZ + cameraRight * moveX).normalized;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (!runLimit)
                {
                    currentSpeed = runSpeed;
                }
                else if (currentStamina > 0)
                {
                    currentSpeed = runSpeed;
                    DrainStamina();
                }
                else
                {
                    currentSpeed = walkSpeed;
                }
            }
            else
            {
                currentSpeed = walkSpeed;
                RecoverStamina();
            }

            float speedNormalized = moveDirection.magnitude > 0.1f ? GetCurrentSpeedNormalized() : 0f;
            animator.SetFloat("Speed", speedNormalized);
        }

        private void Move()
        {
            if (moveDirection.magnitude >= 0.1f)
            {
                Vector3 moveVelocity = moveDirection * currentSpeed;
                rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
            }
        }

        private void RotateCharacter()
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        private void Jump()
        {
            if (isJumping) return;

            isJumping = true;
            isGrounded = false;
            animator.SetTrigger("Jump");
        }

        public void OnJumpStart()
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }

        private void CheckGround()
        {
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

        private void DrainStamina()
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina < 0)
            {
                currentStamina = 0;
            }
            UpdateStaminaUI(); // UI 업데이트
        }

        private void RecoverStamina()
        {
            currentStamina += staminaDrainRate * Time.deltaTime;
            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
            UpdateStaminaUI(); // UI 업데이트
        }

        private void UpdateStaminaUI()
        {
            if (staminaUI != null)
            {
                staminaUI.UpdateStaminaUI(currentStamina, maxStamina);
            }
        }

        public float GetCurrentSpeedNormalized()
        {
            return currentSpeed / runSpeed;
        }
    }
}
