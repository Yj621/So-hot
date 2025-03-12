using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Fire : MonoBehaviourPun
{
    public static Fire Instance;
    public bool isOnFire = true; //불이 켜져 있는지 확인
    public bool isOnGround = false; //불이 바닥에 있는지 여부 확인
    public float timer = 5f; // 불이 바닥에서 유지되는 시간


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (photonView.IsMine)
        {
            //불의 상태를 모든 클라이언트에서 동기화
            photonView.RPC("SyncFireState", RpcTarget.AllBuffered, isOnFire, isOnGround, timer);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("Finish") && isOnFire)
        {
            GameManager.Instance.photonView.RPC("GameClear", RpcTarget.All);
        }
        else if (other.CompareTag("Water") && isOnFire)
        {
            photonView.RPC("FireOff", RpcTarget.AllBuffered); //불 끄기 동기화
        }
        else if (other.CompareTag("Ground"))
        {
            photonView.RPC("SetGroundState", RpcTarget.AllBuffered, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("Ground"))
        {
            photonView.RPC("SetGroundState", RpcTarget.AllBuffered, false);
        }
    }

    [PunRPC]
    void FireOff()
    {
        if (!isOnFire) return; //중복 호출 방지

        isOnFire = false;
        isOnGround = false; //바닥에 있을 필요 없음
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if (!isOnFire)
        {
            GameManager.Instance.isOnFire = false;
        }
        if (isOnGround)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                photonView.RPC("FireOff", RpcTarget.AllBuffered);
                photonView.RPC("ResetFire", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    void SetGroundState(bool state)
    {
        isOnGround = state;
        if (state) timer = 5f; // 바닥에 떨어졌으면 타이머 리셋
    }

    [PunRPC]
    void ResetFire()
    {
        isOnFire = true;
        isOnGround = false;
        timer = 5f;
        gameObject.transform.position = GameManager.Instance.fireSavePoint.position;
    }

    [PunRPC]
    void SyncFireState(bool fireState, bool groundState, float fireTimer)
    {
        isOnFire = fireState;
        isOnGround = groundState;
        timer = fireTimer;
    }


    [PunRPC]
    public void RPC_SetHeldState(int catcherPhotonViewID)
    {
        // 잡은 플레이어의 PhotonView를 찾습니다.
        PhotonView playerPV = PhotonView.Find(catcherPhotonViewID);
        if (playerPV != null)
        {
            // 플레이어 오브젝트의 자식 중 holdPoint 이름의 트랜스폼을 찾습니다.
            Transform holdPoint = playerPV.transform.Find("FireCatchTransform");
            if (holdPoint != null)
            {
                // 모든 클라이언트에서 부모 변경 및 위치 업데이트
                transform.SetParent(holdPoint);
                transform.position = holdPoint.position;
            }
            else
            {
                Debug.LogWarning("HoldPoint를 찾지 못했습니다. 플레이어 오브젝트에 HoldPoint가 존재하는지 확인하세요.");
            }
        }
        else
        {
            Debug.LogWarning("플레이어 PhotonView를 찾지 못했습니다.");
        }

        // Rigidbody를 kinematic으로 설정하여 물리 영향 제거
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }
}
