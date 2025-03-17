using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Trap : MonoBehaviourPunCallbacks
{
    public Rigidbody[] spikeRigidbodies; // 가시의 Rigidbody 배열
    public Collider trapCollider; // 트랩 감지용 콜라이더
    public float upwardForce = 20.0f; // 🔴 가시 속도 증가 (기존 20 → 30)
    public float moveDownSpeed = 5.0f;  // 🔴 더 빠르게 내려가도록 설정
    public float triggerDelay = 0f; // 트리거 후 가시 발동 대기 시간
    public float reloadTime = 1.5f; // 🔴 트랩 재사용 시간을 줄여 반응 속도 개선
    public float spikeDelay = 0.005f; // 🔴 가시가 올라오기 전 딜레이 최소화

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

                // 🔥 즉시 가시를 올림 (물리 엔진의 영향을 최소화)
                foreach (Rigidbody rb in spikeRigidbodies)
                {
                    rb.isKinematic = false; // 🔴 물리 적용 활성화
                    rb.useGravity = false; // 🔴 중력 영향 제거
                    rb.linearVelocity = Vector3.up * 5.0f; // 🔴 즉각적인 상승
                    rb.transform.position += new Vector3(0, 0.01f, 0); // 🔴 즉시 위로 이동
                }

                // 🔴 코루틴으로 원래대로 돌아가게 함
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
            rb.linearVelocity = Vector3.up * 30.0f; // 🔴 즉시 속도 부여
        }

        yield return new WaitForSeconds(0.2f); // 🔴 가시가 올라온 후 유지 시간

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
        float duration = 0.7f / moveDownSpeed; // 🔴 더 빠르게 내려오도록 조정

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
}
