using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using YJ.Network;
using YJ.UIManager;
using static TotalMultiManager;
using Cursor = UnityEngine.Cursor;

namespace JS.PlayerMove
{
    public class PlayerMove : MonoBehaviourPunCallbacks
    {
        public CharacterController controller;
        public Animator animator;
        public PlayerLook playerLook;
        private new PhotonView photonView;
        public PlayerThrowGuide ptg;
        private int playerNumber;

        [Header("이동 설정")]
        public float walkSpeed = 15f;
        public float runSpeed = 25f;
        public float jumpHeight = 10f;
        private float AwalkSpeed = 15f;
        private float ArunSpeed = 25f;
        private float AjumpHeight = 10f;
        public float slowSpeed = 7f;
        public float slowRun = 15f;
        public float slowJumpPower = 6f;
        public float gravity = -52.3f;
        private bool isSlow = false;
        private Vector2 moveInput = Vector2.zero; // 입력값 저장
        private Vector3 velocity;
        private bool isGrounded;
        private bool isRunning = false;
        public GameObject RunObject;

        [Header("던지기 설정")]
        public Transform holdPoint;
        private GameObject heldObject;
        public bool isThrowingReady;
        private float throwCooldownTimer = 0f;
        private bool isThrowing = false;
        public float minThrowForce = 3f;
        public float maxThrowForce = 20f;
        public float throwCooldown = 1f;
        private bool wasOverHeat = false;


        [Header("불 관련")]
        private bool CatchingFire = false;
        public float HotIncrease = 2f;
        public float HotDecrease = 1f;
        public GameObject SmokeObject;

        [Header("죽음, 고스트 관련")]
        public bool isDie = false;
        public bool isGhost = false;
        public GameObject[] OriginalOb;
        public GameObject Ghost;
        public GameObject BloodObject;

        private CinemachineCamera camera;

        [Header("아이템 관련")]
        public Coroutine unlimitRunCoroutine;
        public Coroutine gaugeStopCoroutine;
        public List<GameObject> effectList;
        private Inventory inventory;
        public bool saveLife = false;
        private bool invincible = false;

        public Transform playerGroup;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
        }

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            ptg = GetComponent<PlayerThrowGuide>();
            UIManager.Instance.UpdateStaminaUI();
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

            for (int i = 0; i < OriginalOb.Length; i++)
            {
                OriginalOb[i].SetActive(true);
            }
            Ghost.SetActive(false);
            SmokeObject.SetActive(false);
            BloodObject.SetActive(false);

            playerNumber = (int)GetTag(PhotonNetwork.LocalPlayer, "Number");
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
                SoundManager.Instance.PlayLoopSound(SoundManager.AudioType.HotGauge);
                UIManager.Instance.IncreaseHeat(HotIncrease);
            }
            else
            {
                UIManager.Instance.DecreaseHeat(HotDecrease);
                SoundManager.Instance.StopLoopSound(SoundManager.AudioType.HotGauge);
            }

            if (isThrowingReady)
            {
                Vector3 throwDirection = Camera.main.transform.forward;
                float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, UIManager.Instance.currentThrow / UIManager.Instance.maxThrow);
                ptg.DrawThrowGuide(throwDirection, throwForce);
                UIManager.Instance.IncreaseCharge();
            }
            else
            {
                UIManager.Instance.ResetThrow();
            }

            if (UIManager.Instance.IsOverheated() && !wasOverHeat)
            {
                wasOverHeat = true;
                photonView.RPC("SetSmokeEffect", RpcTarget.AllViaServer, true);
                if (heldObject != null)  // 손에 불이 있을 때만 처리
                {
                    Vector3 throwPosition = heldObject.transform.position;
                    Vector3 throwDirection;
                    float throwForce;

                    SoundManager.Instance.PlaySound(SoundManager.AudioType.PlayerHot);

                    isThrowing = true;

                    if (isThrowingReady) // 차징 중이었으면 던지기
                    {
                        photonView.RPC("PlayThrowAnimation", RpcTarget.AllViaServer);
                        throwDirection = Camera.main.transform.forward;
                        throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, UIManager.Instance.currentThrow / UIManager.Instance.maxThrow);
                    }
                    else // 차징 안 했으면 바닥에 떨어뜨리기
                    {
                        photonView.RPC("PlayAnyAnimation", RpcTarget.AllViaServer);
                        throwDirection = Vector3.down;
                        throwForce = minThrowForce;
                    }
                    int heldObjectViewID = heldObject.GetComponent<PhotonView>().ViewID;
                    photonView.RPC("ThrowObjectRPC", RpcTarget.AllViaServer, heldObjectViewID, throwPosition, throwDirection, throwForce);
                    isThrowingReady = false;
                }

                StartCoroutine(WaitForHeatGaugeReset());
            }

            IEnumerator WaitForHeatGaugeReset()
            {
                // heatGauge가 0이 아닐 동안 매 프레임 대기
                while (UIManager.Instance.heatGauge > 0)
                {
                    yield return null;
                }
                wasOverHeat = false;
                photonView.RPC("SetSmokeEffect", RpcTarget.AllViaServer, false);
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
            if (!photonView.IsMine) return;
            if (isThrowing) return;

            if (!wasOverHeat) // 과열 시 잡을 수 없음
            {
                if (wasOverHeat) return;
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
                    ItemManager.Instance.photonView.RPC("ItemEffectOff", RpcTarget.AllViaServer, photonView.ViewID, 1);
                    StartCoroutine(InvincibilityTimer()); // 무적 타이머 시작
                    saveLife = false; // 죽음 면제 효과는 1회만 사용
                    return;
                }
                photonView.RPC("SetDieState", RpcTarget.AllViaServer);
            }

            if (other.CompareTag("Mud"))
            {
                SetSlow(true, slowSpeed, slowRun, slowJumpPower);
            }

            if (other.CompareTag("Water"))
            {
                GameManager.Instance.PlayerRespawn(playerNumber);
            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Mud"))
            {
                SetSlow(false);
            }
        }

        public void SetSlow(bool slow, float newSpeed = 0f, float newRun = 0f, float newJumpPower = 0f)
        {
            if (slow)
            {
                walkSpeed = newSpeed;
                runSpeed = newRun;
                jumpHeight = newJumpPower;
                isSlow = true;
            }
            else
            {
                walkSpeed = AwalkSpeed;
                runSpeed = ArunSpeed;
                jumpHeight = AjumpHeight;
                isSlow = false;
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
            GameManager.Instance.deadPlayers[playerNumber] = true;
            animator.Play("Die");
            photonView.RPC("SetBloodEffect", RpcTarget.AllViaServer, true);
            yield return new WaitForSeconds(2f);
            photonView.RPC("SetBloodEffect", RpcTarget.AllViaServer, false);
            isDie = false;
            isGhost = true;
            inventory.InitInventory();
            for (int i = 0; i < OriginalOb.Length; i++)
            {
                OriginalOb[i].SetActive(false);
            }
            SetLayerUpwards(gameObject, "Ghost");
            Ghost.SetActive(true);
            if (photonView.IsMine)
            {
                UIManager.Instance.TimerStart();
            }
            yield return new WaitForSeconds(15f);
            for (int i = 0; i < OriginalOb.Length; i++)
            {
                OriginalOb[i].SetActive(true);
            }
            SetLayerUpwards(gameObject, "Default");
            if (photonView.IsMine)
            {
                UIManager.Instance.TimerEnd();
            }
            Ghost.SetActive(false);
            isGhost = false;
            GameManager.Instance.deadPlayers[playerNumber] = false;
        }

        void SetLayerUpwards(GameObject obj, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);

            // 현재 오브젝트부터 부모까지 모든 레이어 변경
            Transform parent = obj.transform;
            while (parent != null)
            {
                parent.gameObject.layer = layer;
                parent = parent.parent;  // 부모로 이동
            }
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
            if (Cursor.visible) return;
            if (isDie) return;
            moveInput = input;
        }

        public void Jump()
        {
            if (Cursor.visible) return;
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
            if (Cursor.visible) return;
            if (!isDie || !isGhost)
            {
                inventory.UseItem();
            }
        }

        public void SetRunning()
        {
            if (Cursor.visible) return;
            if (!isGrounded || isGhost) return;
            isRunning = true;
            photonView.RPC("SetRunEffect", RpcTarget.AllViaServer, true);
        }

        public void StopRunning()
        {
            isRunning = false;
            photonView.RPC("SetRunEffect", RpcTarget.AllViaServer, false);
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

        public void ItemEffectOn(int idx)
        {
            if (effectList[idx] != null)
            {
                effectList[idx].SetActive(true);
            }
        }

        public void ItemEffectOff(int idx)
        {
            if (effectList[idx] != null)
            {
                effectList[idx].SetActive(false);
            }
        }

        private void CatchObject(GameObject obj)
        {
            if (!photonView.IsMine) return;
            if (wasOverHeat) return;

            photonView.RPC("SetCatchingFire", RpcTarget.AllViaServer, true);
            photonView.RPC("PlayCatchAnimation", RpcTarget.AllViaServer);

            PhotonView objPhotonView = obj.GetComponent<PhotonView>();
            if (objPhotonView != null && !objPhotonView.IsMine)
            {
                objPhotonView.RequestOwnership();
            }

            heldObject = obj;
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            // 오브젝트를 holdPoint의 자식으로 설정
            objPhotonView.RPC("RPC_SetHeldState", RpcTarget.AllViaServer, photonView.ViewID);
        }

        public void ReleaseThrow()
        {
            Debug.Log("ReleaseThrow");
            if (isThrowingReady && heldObject != null)
            {
                ptg.OffThrowGuide();
                Vector3 throwPosition = heldObject.transform.position; // 던지기 시작 위치
                Vector3 throwDirection = Camera.main.transform.forward; // 던지는 방향 (플레이어 시점)
                float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, UIManager.Instance.currentThrow / UIManager.Instance.maxThrow);

                SoundManager.Instance.PlaySound(SoundManager.AudioType.PlayerThrow);

                int heldObjectViewID = heldObject.GetComponent<PhotonView>().ViewID;
                photonView.RPC("ThrowObjectRPC", RpcTarget.AllViaServer, heldObjectViewID, throwPosition, throwDirection, throwForce);

                isThrowingReady = false;
                isThrowing = true;
                photonView.RPC("PlayThrowAnimation", RpcTarget.AllViaServer);
                photonView.RPC("SetCatchingFire", RpcTarget.AllViaServer, false);
            }
        }

        [PunRPC]
        void ThrowObjectRPC(int heldObjectViewID, Vector3 throwPosition, Vector3 throwDirection, float throwForce)
        {
            // 전달받은 ViewID를 이용해 물체를 찾음
            PhotonView objPhotonView = PhotonView.Find(heldObjectViewID);
            if (objPhotonView == null) return;
            GameObject obj = objPhotonView.gameObject;
            obj.transform.parent = null;

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 던지기 시작 위치로 설정 후 물리 적용
                obj.transform.position = throwPosition;
                rb.isKinematic = false;
                rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            }

            // 만약 현재 소유한 클라이언트라면, 마스터에게 소유권을 이전
            if (objPhotonView.IsMine)
            {
                objPhotonView.TransferOwnership(PhotonNetwork.MasterClient);
            }

            // 물체의 부모 해제
            obj.transform.parent = null;

            // 각 클라이언트에서 보유중인 heldObject가 해당 물체라면 null로 초기화
            if (heldObject == obj)
            {
                heldObject = null;
            }
            CatchingFire = false;
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
            SoundManager.Instance.PlaySound(SoundManager.AudioType.PlayerManDie);

            StartCoroutine(DieAndBeGhost());
        }

        public void SetPlayerParentRPC()
        {
            photonView.RPC("SetPlayerParent", RpcTarget.AllViaServer);
        }

        [PunRPC]
        private void SetPlayerParent()
        {
            transform.parent.SetParent(playerGroup);
        }

        [PunRPC]
        void SetCatchingFire(bool state)
        {
            CatchingFire = state;
        }

        [PunRPC]
        void SetSmokeEffect(bool isActive)
        {
            SmokeObject.SetActive(isActive);
        }

        [PunRPC]
        void SetBloodEffect(bool isActive)
        {
            BloodObject.SetActive(isActive);
        }

        [PunRPC]
        void SetRunEffect(bool isActive)
        {
            RunObject.SetActive(isActive);
        }
    }
}