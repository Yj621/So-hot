using ExitGames.Client.Photon;
using System.Collections;
using UnityEngine;
using YJ.UIManager;

public class PlayerMove : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;
    public PlayerLook playerLook;

    [Header("이동 설정")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isRunning = false;

    [Header("던지기 설정")]
    public Transform holdPoint;
    private GameObject heldObject;
    public bool isThrowingReady;
    private float throwCooldownTimer = 0f;
    private bool isThrowing = false;

    public float minThrowForce = 3f;
    public float maxThrowForce = 20f;
    public float throwCooldown = 1f;

    private Vector2 moveInput = Vector2.zero; // 입력값 저장

    private bool CatchingFire = false;
    public float HotIncrease = 2f;
    public float HotDecrease = 1f;

    public bool isDie = false;
    public bool isGhost = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        UIManager.Instance.UpdateStaminaUI();
    }

    private void Update()
    {
        playerLook.Rotate();
        
        // 중력 적용
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 이동 벡터 생성
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

      
        // 이동 속도 적용
        float speed = isRunning ? runSpeed : walkSpeed;
        controller.Move(move * speed * Time.deltaTime);

        // 애니메이션 설정
        bool isMoving = moveInput != Vector2.zero;
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isGhost", isGhost);

        // 공중 상태 감지
        bool isFalling = !isGrounded && velocity.y < 0;
        animator.SetBool("isFalling", isFalling);

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 던지기 쿨타임 처리
        if (isThrowing)
        {
            throwCooldownTimer += Time.deltaTime;
            if (throwCooldownTimer >= throwCooldown)
            {
                isThrowing = false;
                throwCooldownTimer = 0f;
            }
        }

        if(isRunning == true)
        {
            UIManager.Instance.ActiveStamina();
            if (!UIManager.Instance.runLimit)
            {

            }
            else
            {
                UIManager.Instance.DrainStamina();
            }
        }

        if(UIManager.Instance.currentStamina == 0)
        {
            isRunning = false;
        }

        if(isRunning == false)
        {
            UIManager.Instance.RecoverStamina();
        }

        if(CatchingFire == true)
        {
           UIManager.Instance.IncreaseHeat(HotIncrease);
        }
        if(CatchingFire == false)
        {
            UIManager.Instance.DecreaseHeat(HotDecrease);
        }

        if(isThrowingReady == true)
        {
            UIManager.Instance.IncreaseCharge();
        }

        if (isThrowingReady == false)
        {
            UIManager.Instance.ResetThrow();
        }

        if(isDie == true)
        {
            StartCoroutine(DieAndBeGhost());
        }
    }

    IEnumerator DieAndBeGhost()
    {
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(2f);

        isGhost = true;

        if (isGhost)
        {

        }
    }

    // 이동 입력 (PlayerInput에서 호출)
    public void Move(Vector2 input)
    {
        moveInput = input;
    }

    // 점프 처리 (PlayerInput에서 호출)
    public void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }
    }

    // 달리기 토글 (PlayerInput에서 호출)
    public void SetRunning()
    {
        if (isGrounded)
        {
           isRunning = true;
        }
    }

    public void StopRunning()
    {
        isRunning = false;
    }

    // 던지기 시작 (PlayerInput에서 호출)
    public void StartThrow()
    {
        if (heldObject != null)
        {
            animator.SetTrigger("ThrowReady");
            isThrowingReady = true;
        }
    }

    // 던지기 충전 (PlayerInput에서 호출)
   

    // 던지기 실행 (PlayerInput에서 호출)
    public void ReleaseThrow()
    {
        if (isThrowingReady && heldObject != null)
        {
            ThrowObject();
            animator.SetTrigger("Throw");
            isThrowingReady = false;
            isThrowing = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isThrowing) return;

        if (other.CompareTag("Fire"))
        {
            animator.SetTrigger("Catch");
            CatchObject(other.gameObject);
            CatchingFire = true;
        }
    }

    private void CatchObject(GameObject obj)
    {
        heldObject = obj;
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.transform.position = holdPoint.position;
        obj.transform.parent = holdPoint;
    }

    private void ThrowObject()
    {
        if (heldObject != null)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;

                // 던질 방향을 카메라 시야 기준으로 변경
                Vector3 throwDirection = Camera.main.transform.forward;  // 카메라의 앞 방향 사용

                float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, UIManager.Instance.currentThrow / UIManager.Instance.maxThrow);
                rb.AddForce(throwDirection * throwForce, ForceMode.Impulse); // 카메라 방향으로 던지기
            }

            heldObject.transform.parent = null;
            heldObject = null;
            CatchingFire = false;
        }
    }
}
