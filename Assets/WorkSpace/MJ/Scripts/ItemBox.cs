using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;
using Photon.Pun;
using JS.PlayerMove;
using Photon.Realtime;
namespace MJ.Item.ItemBox
{
    public class ItemBox : MonoBehaviourPun
    {
        [SerializeField] ItemData[] items; //아이템 전체 목록

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                int idx = Random.Range(0, items.Length);
                other.GetComponent<Inventory>().GetItem(items[idx]);
                photonView.RPC("DestroyObject", RpcTarget.All);
            }
        }

        [PunRPC]
        void DestroyObject()
        {
            Destroy(gameObject);
        }
    }
}
