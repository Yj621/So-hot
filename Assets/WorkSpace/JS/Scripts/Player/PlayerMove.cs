using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using YJ.UIManager;

public class PlayerMove : MonoBehaviourPunCallbacks
{
    public CharacterController controller;
    public Animator animator;
    public PlayerLook playerLook;
    private new PhotonView photonView;

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
    public Renderer CharacterRenderer;
    public Material OverrideMaterial;
    private Material originalMaterial;

    private bool wasOverHeat = false;

    private CinemachineCamera camera;

    private void Start()
    {
        camera = FindFirstObjectByType<CinemachineCamera>();

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
        UIManager.Instance.UpdateStaminaUI();
        originalMaterial = CharacterRenderer.material;
        camera.Follow = gameObject.transform;
        camera.LookAt = gameObject.transform;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if (isDie)
        {
            // 이동 및 속도 정지
            moveInput = Vector2.zero;
            velocity = Vector3.zero;

            animator.SetBool("isMoving", false);
            animator.SetBool("isRunning", false);
         
            StartCoroutine(DieAndBeGhost());
            return; // 모든 동작 차단
        }

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

        if (isThrowingReady == true)
        {
            UIManager.Instance.IncreaseCharge();
        }

        if (isThrowingReady == false)
        {
            UIManager.Instance.ResetThrow();
        }

        if (UIManager.Instance.IsOverheated() && !wasOverHeat)
        {
            if (isThrowingReady)
            {
                ReleaseThrow();
            }
            else
            {
                ThrowObject();
                isThrowing = true;
                animator.SetTrigger("Any");
            }
            wasOverHeat = true;
        }

        if ((UIManager.Instance.heatGauge == 0) && wasOverHeat)
        {
            wasOverHeat = false;
        }
        
        photonView.RPC("SyncState", RpcTarget.Others, moveInput, isRunning, isThrowingReady, isDie, isGhost);
    }

    [PunRPC]
    void SyncState(Vector2 input, bool running, bool throwingReady, bool die, bool ghost)
    {
        moveInput = input;
        isRunning = running;
        isThrowingReady = throwingReady;
        isDie = die;
        isGhost = ghost;
    }

    IEnumerator DieAndBeGhost()
    {
        animator.Play("Die");
        yield return new WaitForSeconds(2f);
        isDie = false;
        isGhost = true;
        CharacterRenderer.material = OverrideMaterial;
        yield return new WaitForSeconds(15f);
        CharacterRenderer.material = originalMaterial;
        isGhost = false;
    }

    // 이동 입력 (PlayerInput에서 호출)
    public void Move(Vector2 input)
    {
        if (isDie) return;
        moveInput = input;
    }

    // 점프 처리 (PlayerInput에서 호출)
    public void Jump()
    {
        if (isDie) return;
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if(!isGhost)
            animator.SetTrigger("Jump");
        }
    }

    [PunRPC]
    void JumpRPC()
    {
        animator.SetTrigger("Jump");
    }

    // 달리기 토글 (PlayerInput에서 호출)
    public void SetRunning()
    {
        if (isGhost) return;
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
    [PunRPC]
    void ThrowReadyRPC()
    {
        animator.SetTrigger("ThrowReady");
    }

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

        if (other.CompareTag("Fire") && !isDie && !isGhost)
        {
            if (!wasOverHeat) // 과열 시 잡을 수 없음
            {
                animator.SetTrigger("Catch");
                CatchingFire = true;
                CatchObject(other.gameObject);
            }
        }

        if (other.CompareTag("Trap") && !isDie && !isGhost)
        {
            isDie = true;
        }
    }

    private void CatchObject(GameObject obj)
    {
        if (CatchingFire && !wasOverHeat)
        {
            heldObject = obj;
            obj.GetComponent<Rigidbody>().isKinematic = true;
            obj.transform.position = holdPoint.position;
            obj.transform.parent = holdPoint;
        }
    }

    private void ThrowObject()
    {
        if (heldObject != null)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;

                Vector3 throwDirection = Camera.main.transform.forward;

                float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, UIManager.Instance.currentThrow / UIManager.Instance.maxThrow);
                rb.AddForce(throwDirection * throwForce, ForceMode.Impulse); // 카메라 방향으로 던지기
            }

            heldObject.transform.parent = null;
            heldObject = null;
            CatchingFire = false;
        }
    }

    [PunRPC]
    void ThrowRPC()
    {
        animator.SetTrigger("Throw");
    }
}
