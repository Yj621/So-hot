using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ClearTrigger : MonoBehaviourPunCallbacks
{
    private void OnTriggerEnter(Collider other)
    {
        // Fire 태그가 있는 오브젝트가 닿으면 모든 클라이언트에게 씬 전환 신호 전송
        if (other.CompareTag("Fire"))
        {
            photonView.RPC("LoadEndingScene", RpcTarget.All);
        }
    }

    [PunRPC]
    void LoadEndingScene()
    {
        // 모든 클라이언트가 엔딩씬으로 전환
        SceneManager.LoadScene("EndingScene");
    }
}
