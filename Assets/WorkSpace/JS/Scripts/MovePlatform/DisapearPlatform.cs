using Photon.Pun;
using System.Collections;
using UnityEngine;

public class DisapearPlatform : MonoBehaviourPunCallbacks
{
    private Collider platformCollider;
    private MeshRenderer platformRenderer;
    private bool isActive = true;  // 현재 발판 상태

    void Start()
    {
        platformCollider = GetComponent<Collider>();
        platformRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 밟았을 때
        if (other.CompareTag("Player") && isActive)
        {
            photonView.RPC("StartDisappear", RpcTarget.All);
        }
    }

    [PunRPC]
    void StartDisappear()
    {
        if (isActive)
        {
            isActive = false;
            StartCoroutine(DisappearCoroutine());
        }
    }

    IEnumerator DisappearCoroutine()
    {
        yield return new WaitForSeconds(2f); // 2초 대기
        SetPlatformActive(false);

        yield return new WaitForSeconds(5f); // 5초 후 재생성
        SetPlatformActive(true);
    }

    void SetPlatformActive(bool state)
    {
        photonView.RPC("SyncPlatformState", RpcTarget.All, state);
    }

    [PunRPC]
    void SyncPlatformState(bool state)
    {
        isActive = state;
        platformCollider.enabled = state;
        platformRenderer.enabled = state;
    }
}
