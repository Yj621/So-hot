using UnityEngine;
using Photon.Pun;
using System.Collections;

public class Cherry : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        GetComponent<AudioSource>().Play();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            GetComponent<AudioSource>().Play();

            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
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
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Debug.LogError($"[Cherry] 소유권 변경 실패! 삭제 불가: {gameObject.name}");
        }
    }
}
