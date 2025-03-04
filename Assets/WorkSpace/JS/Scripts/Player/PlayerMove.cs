using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public CharacterController controller;

    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 땅에 닿았는지 확인
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 입력 받기
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 이동 방향 설정
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // 걷기 / 달리기 속도 설정
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        controller.Move(move * speed * Time.deltaTime);

        // 점프
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
