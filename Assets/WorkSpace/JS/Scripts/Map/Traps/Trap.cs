using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Trap : MonoBehaviourPunCallbacks
{
    public Rigidbody[] spikeRigidbodies; // 자식 가시들의 Rigidbody 배열
    public float upwardForce = 10.0f; // 위로 밀어 올리는 힘
    public float moveDownSpeed = 2.0f;  // 다시 내려가는 속도
    public float triggerDelay = 0f; // 트리거 후 가시 발동 대기 시간
    public float reloadTime = 3.0f; // 트랩이 재사용 가능해지는 시간

    private Vector3[] originalPositions;
    private bool isActivated = false;
    private bool playerInside = false;

    private void Start()
    {
        if (photonView == null)
        {
            Debug.LogError("PhotonView가 없습니다! Trap 오브젝트에 PhotonView 컴포넌트를 추가하세요.");
        }

        // 모든 가시의 원래 위치 저장
        originalPositions = new Vector3[spikeRigidbodies.Length];
        for (int i = 0; i < spikeRigidbodies.Length; i++)
        {
            originalPositions[i] = spikeRigidbodies[i].transform.localPosition;
            spikeRigidbodies[i].useGravity = true;   // 중력 사용
            spikeRigidbodies[i].isKinematic = true;  // 기본적으로 정지 상태 유지
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true; // 플레이어가 범위 내에 있음을 기록
            if (!isActivated && PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("StartActivateTrap", RpcTarget.All);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false; // 플레이어가 범위에서 나감
        }
    }

    [PunRPC]
    private void StartActivateTrap()
    {
        StartCoroutine(ActivateTrap());
    }

    private IEnumerator ActivateTrap()
    {
        isActivated = true;
        yield return new WaitForSeconds(triggerDelay);

        // 1. 모든 가시를 위로 밀어 올리기
        foreach (Rigidbody rb in spikeRigidbodies)
        {
            rb.isKinematic = false; // 물리 활성화
            rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
        }

        // 2. 일정 시간 후 부드럽게 내려가기
        yield return new WaitForSeconds(0.5f); // 살짝 떠오른 후 내려가기 시작
        StartCoroutine(SmoothlyMoveSpikesDown());

        // 3. 5초 후 다시 활성화 가능
        yield return new WaitForSeconds(reloadTime);
        isActivated = false;

        // 4. 플레이어가 아직 트랩 범위 내에 있으면 다시 발동
        if (playerInside)
        {
            photonView.RPC("StartActivateTrap", RpcTarget.All);
        }
    }

    private IEnumerator SmoothlyMoveSpikesDown()
    {
        float elapsedTime = 0f;
        float duration = 1.5f / moveDownSpeed; // 부드럽게 내려오는 시간 조절

        Vector3[] startPositions = new Vector3[spikeRigidbodies.Length];
        for (int i = 0; i < spikeRigidbodies.Length; i++)
        {
            startPositions[i] = spikeRigidbodies[i].transform.localPosition;
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            for (int i = 0; i < spikeRigidbodies.Length; i++)
            {
                spikeRigidbodies[i].transform.localPosition = Vector3.Lerp(startPositions[i], originalPositions[i], progress);
            }

            yield return null;
        }

        // 최종적으로 위치를 정확히 원래 위치로 설정
        for (int i = 0; i < spikeRigidbodies.Length; i++)
        {
            spikeRigidbodies[i].transform.localPosition = originalPositions[i];
            spikeRigidbodies[i].isKinematic = true; // 다시 정지
        }
    }
}
