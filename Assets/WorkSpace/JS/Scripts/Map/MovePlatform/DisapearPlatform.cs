using Photon.Pun;
using System.Collections;
using UnityEngine;

public class DisappearPlatform : MonoBehaviourPunCallbacks
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
        if (other.CompareTag("Player") && isActive)
        {
            Debug.Log("플레이어가 발판을 밟음");
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
        yield return new WaitForSeconds(2f); // 2초 대기 후 사라짐
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
