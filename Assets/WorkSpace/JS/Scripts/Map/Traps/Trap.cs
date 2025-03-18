using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Trap : MonoBehaviourPunCallbacks
{
    public Rigidbody[] spikeRigidbodies; // 가시의 Rigidbody 배열
    public Collider trapCollider; // 트랩 감지용 콜라이더
    public float upwardForce = 30.0f; // 🔴 가시 속도 증가
    public float moveDownSpeed = 5.0f; // 🔴 내려가는 속도 조정
    public float triggerDelay = 0f; // 트리거 후 가시 발동 대기 시간
    public float reloadTime = 1.2f; // 🔴 트랩 재사용 시간 단축
    public float spikeDelay = 0.002f; // 🔴 가시가 올라오기 전 딜레이 최소화

    private Vector3[] originalPositions;
    private bool isActivated = false;
    private bool playerInside = false;

    private void Start()
    {
        if (photonView == null)
        {
            Debug.LogError("PhotonView가 없습니다! Trap 오브젝트에 PhotonView 컴포넌트를 추가하세요.");
        }

        // 원래 위치 저장
        originalPositions = new Vector3[spikeRigidbodies.Length];
        for (int i = 0; i < spikeRigidbodies.Length; i++)
        {
            originalPositions[i] = spikeRigidbodies[i].transform.localPosition;
            spikeRigidbodies[i].useGravity = false;
            spikeRigidbodies[i].isKinematic = true;

            // 🔴 Trap의 콜라이더와 가시 간 충돌 방지
            if (trapCollider != null)
            {
                Physics.IgnoreCollision(trapCollider, spikeRigidbodies[i].GetComponent<Collider>());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            if (!isActivated)
            {
                isActivated = true;

                foreach (Rigidbody rb in spikeRigidbodies)
                {
                    rb.isKinematic = false;
                    rb.useGravity = false;

                    // 🔴 속도를 조정하여 즉시 반응 + 너무 높이 올라가지 않도록 조정
                    rb.linearVelocity = Vector3.up * 20f; // 기존 8.0f → 20.0f로 수정

                    // 🔴 LimitSpikeHeight() 실행 → 가시가 올라갈 최대 높이 설정
                    StartCoroutine(LimitSpikeHeight(rb));
                }

                StartCoroutine(ActivateTrap());
                photonView.RPC("StartActivateTrap", RpcTarget.Others);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
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

        // 🔥 가시를 즉시 위로 올리기
        foreach (Rigidbody rb in spikeRigidbodies)
        {
            SoundManager.Instance.PlaySound(SoundManager.AudioType.Spikes);
            rb.isKinematic = false;
            rb.useGravity = false;

            // 🔴 즉시 반응하도록 속도를 높이고, 너무 높이 올라가지 않도록 조정
            rb.linearVelocity = Vector3.up * 50.0f; // 기존 100.0f → 50.0f로 조정
        }

        yield return new WaitForSeconds(0.08f); // 🔴 가시가 올라온 후 유지 시간 증가

        StartCoroutine(SmoothlyMoveSpikesDown());

        yield return new WaitForSeconds(reloadTime);
        isActivated = false;

        if (playerInside)
        {
            StartCoroutine(ActivateTrap());
        }
    }

    private IEnumerator SmoothlyMoveSpikesDown()
    {
        float elapsedTime = 0f;
        float duration = 0.5f / moveDownSpeed; // 🔴 더 빠르게 내려오도록 조정

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

        // 원래 위치로 복구 및 물리 비활성화
        for (int i = 0; i < spikeRigidbodies.Length; i++)
        {
            spikeRigidbodies[i].transform.localPosition = originalPositions[i];
            spikeRigidbodies[i].isKinematic = true;
        }
    }

    private IEnumerator LimitSpikeHeight(Rigidbody rb)
    {
        float maxHeight = originalPositions[0].y + 0.1f; // 🔴 최대 높이 제한 (원래 위치 + 0.4)

        while (rb.transform.position.y < maxHeight)
        {
            yield return null; // 매 프레임 체크
        }

        // 🔴 최대 높이에 도달하면 속도 멈추기
        rb.linearVelocity = Vector3.zero;
        rb.transform.position = new Vector3(rb.transform.position.x, maxHeight, rb.transform.position.z);
    }
}
