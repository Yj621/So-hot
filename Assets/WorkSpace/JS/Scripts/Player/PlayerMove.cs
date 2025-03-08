using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Voice;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;
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
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
        UIManager.Instance.UpdateStaminaUI();
        originalMaterial = CharacterRenderer.material;

        // 내 캐릭터만 카메라 설정
        if (photonView.IsMine)
        {
            CinemachineCamera camera = FindFirstObjectByType<CinemachineCamera>();

            if (camera != null)
            {
                camera.Follow = transform;
                camera.LookAt = transform;
            }
        }
    }

    private void Update()
    {
        if (! photonView.IsMine) return;

        if (isDie)
            {
                moveInput = Vector2.zero;
                velocity = Vector3.zero;

                animator.SetBool("isMoving", false);
                animator.SetBool("isRunning", false);

                StartCoroutine(DieAndBeGhost());
                return;
            }

            playerLook.Rotate();
            float rotationY = transform.rotation.eulerAngles.y;

            // 중력 적용
            isGrounded = controller.isGrounded;
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            velocity.y += gravity * Time.deltaTime;

            // 이동 벡터 생성
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            float speed = isRunning ? runSpeed : walkSpeed;
            controller.Move((move * speed + velocity) * Time.deltaTime);

            // 애니메이션 설정
            animator.SetBool("isMoving", moveInput != Vector2.zero);
            animator.SetBool("isRunning", isRunning);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isGhost", isGhost);
            animator.SetBool("isFalling", !isGrounded && velocity.y < 0);

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

            // 스테미너 시스템 처리
            if (isRunning)
            {
                UIManager.Instance.ActiveStamina();
                if (!UIManager.Instance.runLimit)
                {
                    UIManager.Instance.DrainStamina();
                }
            }
            if (UIManager.Instance.currentStamina == 0)
            {
                isRunning = false;
            }
            if (!isRunning)
            {
                UIManager.Instance.RecoverStamina();
            }

            // 과열 시스템 처리
            if (CatchingFire)
            {
                UIManager.Instance.IncreaseHeat(HotIncrease);
            }
            else
            {
                UIManager.Instance.DecreaseHeat(HotDecrease);
            }

            if (isThrowingReady)
            {
                UIManager.Instance.IncreaseCharge();
            }
            else
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

            if (UIManager.Instance.heatGauge == 0 && wasOverHeat)
            {
                wasOverHeat = false;
            }


            // 네트워크 동기화 (velocity.y 제외)
            photonView.RPC("SyncState", RpcTarget.Others, moveInput, isRunning, isThrowingReady, isDie, isGhost, rotationY, transform.position.x, transform.position.y, transform.position.z);
        
    }

    [PunRPC]
    void SyncState(Vector2 input, bool running, bool throwingReady, bool die, bool ghost, float rotationY, float posX, float posY, float posZ)
    {
        if (photonView.IsMine) return;

        moveInput = input;
        isRunning = running;
        isThrowingReady = throwingReady;
        isDie = die;
        isGhost = ghost;
        

        animator.SetBool("isMoving", moveInput != Vector2.zero);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isGhost", isGhost);

        if (!photonView.IsMine)
        {
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationY, ref playerLook.rotationVelocity, playerLook.rotationSmoothTime);
            // 구해진 rotation을 Quaternion.Euler에 y축 각도로 넣어주고 transform.rotation에 적용
            transform.rotation = Quaternion.Euler(0, rotation, 0);

            transform.position = new Vector3(posX, posY, posZ);
        }
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

    public void Move(Vector2 input)
    {
        if (isDie) return;
        moveInput = input;
    }

    public void Jump()
    {
        if (isDie) return;
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (!isGhost)
                animator.SetTrigger("Jump");
        }
    }

    public void SetRunning()
    {
        if (!isGrounded || isGhost) return;
        isRunning = true;
    }

    public void StopRunning()
    {
        isRunning = false;
    }

    public void StartThrow()
    {
        if (heldObject != null)
        {
            animator.SetTrigger("ThrowReady");
            isThrowingReady = true;
        }
    }

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

    private void CatchObject(GameObject obj)
    {
        heldObject = obj;
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.transform.position = holdPoint.position;
        obj.transform.parent = holdPoint;
    }

    private void ThrowObject()
    {
        if (heldObject == null) return;

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(Camera.main.transform.forward * maxThrowForce, ForceMode.Impulse);
        }

        heldObject.transform.parent = null;
        heldObject = null;
        CatchingFire = false;
    }
}
