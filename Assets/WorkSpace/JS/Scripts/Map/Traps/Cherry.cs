using UnityEngine;
using Photon.Pun; // 포톤 네트워크 관련 네임스페이스 추가

public class Cherry : MonoBehaviourPunCallbacks
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // PhotonView가 존재하는지 확인 후, 소유자가 맞을 때만 삭제
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
