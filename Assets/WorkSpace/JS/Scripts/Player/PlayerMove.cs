using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;

    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isThrowingReady;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 땅에 닿았는지 확인
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 이동 입력 받기
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // 이동 속도 설정
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isMoving = moveX != 0 || moveZ != 0;
        float speed = isRunning ? runSpeed : walkSpeed;
        controller.Move(move * speed * Time.deltaTime);

        // 애니메이션 설정
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isGrounded);

        // 점프 처리
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        // 공중 상태 감지
        bool isFalling = !isGrounded && velocity.y < 0;
        animator.SetBool("isFalling", isFalling);

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("ThrowReady");
            isThrowingReady = true;
        }

        if (Input.GetMouseButtonUp(1) && isThrowingReady)
        {
            animator.ResetTrigger("ThrowReady");
            animator.SetTrigger("Any");// Trigger 초기화
            isThrowingReady = false;
        }


        // Throw 애니메이션 실행 (ThrowReady 상태일 때만 가능)
        if (Input.GetMouseButtonDown(0) && isThrowingReady)
        {
            animator.SetTrigger("Throw");
            isThrowingReady = false;
        }
    }
}

