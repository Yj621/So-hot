using UnityEngine;
using Photon.Pun;
using JS.PlayerMove;
namespace MJ.Item.ItemBox
{
    public class ItemBox : MonoBehaviourPun
    {
        [SerializeField] ItemData[] items; //아이템 전체 목록

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            PhotonView playerView = other.GetComponent<PhotonView>();
            PlayerMove playerMove = other.GetComponent<PlayerMove>();

            if (playerView != null && playerView.IsMine)
            {
                if (!playerMove.isDie && !playerMove.isGhost)
                {
                    int idx = Random.Range(0, items.Length);
                    other.GetComponent<Inventory>().GetItem(items[idx]);

                    photonView.RPC("DestroyObject", RpcTarget.All);
                }
            }
        }

        [PunRPC]
        void DestroyObject()
        {
            Destroy(gameObject);
        }
    }
}
