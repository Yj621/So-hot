using UnityEngine;
using Photon.Pun;
using System.Collections;

public class Cherry : MonoBehaviourPunCallbacks
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log($"[Cherry] Ground 충돌 감지: {gameObject.name}");

            if (photonView.IsMine)
            {
                Debug.Log($"[Cherry] PhotonNetwork.Destroy 호출: {gameObject.name}");
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                Debug.Log($"[Cherry] 소유권 없음, 소유권 요청 중...: {gameObject.name}");
                photonView.RequestOwnership(); // 💡 소유권 요청
                StartCoroutine(DestroyAfterOwnership());
            }
        }
    }

    private IEnumerator DestroyAfterOwnership()
    {
        float timer = 0f;
        while (!photonView.IsMine && timer < 2f) // 최대 2초 동안 소유권을 기다림
        {
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        if (photonView.IsMine)
        {
            Debug.Log($"[Cherry] 소유권 변경 완료, 삭제 진행: {gameObject.name}");
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Debug.LogError($"[Cherry] 소유권 변경 실패! 삭제 불가: {gameObject.name}");
        }
    }
}
