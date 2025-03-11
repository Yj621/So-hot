using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;
using YJ.UIManager;

namespace JS.PlayerMove
{
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

        public Coroutine unlimitRunCoroutine;
        public Coroutine gaugeStopCoroutine;

        public List<GameObject> effectList;
        private Inventory inventory;
        public bool saveLife = false;
        private bool invincible = false;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            photonView = GetComponent<PhotonView>();
            UIManager.Instance.UpdateStaminaUI();
            originalMaterial = CharacterRenderer.material;
            StartCoroutine(FindInventoryWithDelay());

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
            if (!photonView.IsMine) return;

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

                }
                else
                {
                    UIManager.Instance.DrainStamina();
                }

                if (UIManager.Instance.currentStamina == 0)
                {
                    isRunning = false;
                }
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
                wasOverHeat = true;
                if (heldObject != null)  // 손에 불이 있을 때만 처리
                {
                    Vector3 throwPosition = heldObject.transform.position;
                    Vector3 throwDirection;
                    float throwForce;

                    isThrowing = true;

                    if (isThrowingReady) // 차징 중이었으면 던지기
                    {
                        throwDirection = Camera.main.transform.forward;
                        throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, UIManager.Instance.currentThrow / UIManager.Instance.maxThrow);
                    }
                    else // 차징 안 했으면 바닥에 떨어뜨리기
                    {
                        photonView.RPC("PlayAnyAnimation", RpcTarget.AllViaServer);
                        throwDirection = Vector3.down;
                        throwForce = minThrowForce;
                    }
                    photonView.RPC("ThrowObjectRPC", RpcTarget.AllViaServer, throwPosition, throwDirection, throwForce);
                    isThrowingReady = false;
                }
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

            if (isDie && !isGhost)
            {
                StartCoroutine(DieAndBeGhost());
            }

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
        private void OnTriggerEnter(Collider other)
        {
            if (isThrowing) return;

            if (!wasOverHeat) // 과열 시 잡을 수 없음
            {
                if (other.CompareTag("Fire") && !isDie && !isGhost)
                {
                    CatchObject(other.gameObject);
                }
            }

            if (other.CompareTag("Trap") && !isDie && !isGhost)
            {
                if (invincible)
                {
                    Debug.Log("무적 상태: 데미지 없음");
                    return;
                }

                if (saveLife)
                {
                    Debug.Log("죽음 면제 발동! 5초간 무적");
                    ItemManager.Instance.photonView.RPC("ItemEffectOff", RpcTarget.All, photonView.ViewID, 1);
                    StartCoroutine(InvincibilityTimer()); // 무적 타이머 시작
                    saveLife = false; // 죽음 면제 효과는 1회만 사용
                    return;
                }
                photonView.RPC("SetDieState", RpcTarget.AllBuffered);
            }

        }

        private IEnumerator InvincibilityTimer()
        {
            invincible = true;
            yield return new WaitForSeconds(5f);
            invincible = false;
            Debug.Log("무적 해제됨");
        }

        IEnumerator DieAndBeGhost()
        {
            animator.Play("Die");
            yield return new WaitForSeconds(2f);
            isDie = false;
            isGhost = true;
            CharacterRenderer.material = OverrideMaterial;
            gameObject.layer = LayerMask.NameToLayer("Ghost");
            yield return new WaitForSeconds(15f);
            CharacterRenderer.material = originalMaterial;
            gameObject.layer = LayerMask.NameToLayer("Default");
            isGhost = false;
        }

        IEnumerator FindInventoryWithDelay()
        {
            yield return new WaitForSeconds(5f); // 0.5초 정도 기다리기 (네트워크 동기화 시간 확보)
            inventory = FindAnyObjectByType<Inventory>();

            if (inventory == null)
            {
                Debug.LogError("Inventory 객체를 찾지 못함!");
            }
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

        public void UseItem()
        {
            inventory.UseItem();
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
            Debug.Log("StartThrow");
            if (heldObject != null)
            {
                photonView.RPC("PlayThrowReadyAnimation", RpcTarget.AllViaServer);
                isThrowingReady = true;
            }
        }

        public void ReleaseThrow()
        {
            if (isThrowingReady && heldObject != null)
            {
                Vector3 throwPosition = heldObject.transform.position; // 던지기 시작 위치
                Vector3 throwDirection = Camera.main.transform.forward; // 던지는 방향 (플레이어 시점)
                float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, UIManager.Instance.currentThrow / UIManager.Instance.maxThrow);

                photonView.RPC("RequestThrowObject", RpcTarget.MasterClient, throwDirection);

                isThrowingReady = false;
                isThrowing = true;
            }
        }

        [PunRPC]
        void RequestThrowObject(Vector3 throwDirection, PhotonMessageInfo info)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            if (heldObject == null) return;

            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(throwDirection * maxThrowForce, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
            }

            Vector3 newPos = heldObject.transform.position;
            Vector3 newVel = rb.linearVelocity;
            Vector3 newRot = heldObject.transform.eulerAngles;

            photonView.RPC("SyncThrownObject", RpcTarget.Others, newPos, newVel, newRot);

            // 소유권 해제
            PhotonView objPhotonView = heldObject.GetComponent<PhotonView>();
            if (objPhotonView != null && objPhotonView.IsMine)
            {
                objPhotonView.TransferOwnership(PhotonNetwork.MasterClient); // 마스터 클라이언트에게 돌려주기
            }

            heldObject.transform.parent = null;
            heldObject = null;
            CatchingFire = false;

            photonView.RPC("PlayThrowAnimation", RpcTarget.AllViaServer);
        }

        [PunRPC]
        void SyncThrownObject(Vector3 position, Vector3 velocity, Vector3 rotation)
        {
            if (heldObject == null) return;

            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                heldObject.transform.position = position;
                rb.linearVelocity = velocity;
                heldObject.transform.eulerAngles = rotation;
            }
        }

        [PunRPC]
        void PlayThrowReadyAnimation()
        {
            animator.SetBool("ThrowReady", true);
        }

        [PunRPC]
        void PlayThrowAnimation()
        {
            animator.SetBool("ThrowReady", false);
            animator.SetTrigger("Throw");
        }

        [PunRPC]
        void PlayCatchAnimation()
        {
            Debug.Log("재생");
            animator.SetTrigger("Catch");
        }

        [PunRPC]
        void PlayAnyAnimation()
        {
            animator.SetTrigger("Any");
        }

        [PunRPC]
        void SetDieState()
        {
            if (isDie) return; // 이미 죽었으면 실행 안 함.

            isDie = true;
            StartCoroutine(DieAndBeGhost());
        }

        private void CatchObject(GameObject obj)
        {
            if (!photonView.IsMine) return; // 본인만 실행
            CatchingFire = true;
            photonView.RPC("PlayCatchAnimation", RpcTarget.AllViaServer);

            // 물체의 PhotonView 가져오기
            PhotonView objPhotonView = obj.GetComponent<PhotonView>();
            if (objPhotonView != null && !objPhotonView.IsMine)
            {
                objPhotonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }

            heldObject = obj;
            obj.GetComponent<Rigidbody>().isKinematic = true;
            obj.transform.position = holdPoint.position;
            obj.transform.parent = holdPoint;
        }

    }
}