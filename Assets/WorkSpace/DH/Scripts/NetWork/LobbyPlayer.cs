using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;

namespace Donghyun.Network
{
    public class LobbyPlayer : MonoBehaviour
    {
        [SerializeField] private GameObject readyText;
        [SerializeField] private GameObject masterText;
        [SerializeField] private TextMeshProUGUI nameText;

        private PhotonView pv;

        private void Awake()
        {
            pv = GetComponent<PhotonView>();
        }

        //닉네임 색 바꾸는 함수
        public void SetNickNameColor(Color color)
        {
            nameText.color = color;
        }

        public void SetReadytRPC(bool isReady)
        {
            pv.RPC("SetReady", RpcTarget.All, isReady);
        }

        public void SetClientTextRPC()
        {
            pv.RPC("SetClientText", RpcTarget.All);
        }
        public void SetClientTextRPC(Player player)
        {
            pv.RPC("SetClientText", player);
        }

        public void SetMasterTextRPC()
        {
            pv.RPC("SetMasterText", RpcTarget.All);
        }

        public void SetMasterTextRPC(Player player)
        {
            pv.RPC("SetMasterText", player);
        }

        public void SetNickNameRPC(string name, RpcTarget target)
        {
            pv.RPC("SetNickName", target, name);
        }
        public void SetNickNameRPC(string name, Player player)
        {
            pv.RPC("SetNickName", player, name);
        }

        public void SetPlayerSlotRPC(int playerNumber, RpcTarget target)
        {
            pv.RPC("SetPlayerSlot", target, playerNumber);
        }

        public void SetPlayerSlotRPC(int playerNumber, Player player)
        {
            pv.RPC("SetPlayerSlot", player, playerNumber);
        }

        /// <summary>
        /// 플레이어의 이름 UI를 업데이트
        /// </summary>
        /// <param name="name">플레이어 이름</param>
        [PunRPC]
        private void SetNickName(string name)
        {
            nameText.text = name;
        }

        [PunRPC]
        private void SetPlayerSlot(int playerNumber)
        {
            transform.SetParent(NetWorkManager.Instance.PlayerSlots[playerNumber], false);
        }

        [PunRPC]
        private void SetClientText()
        {
            readyText.SetActive(false);
            masterText.SetActive(false);
        }

        [PunRPC]
        private void SetMasterText()
        {
            readyText.SetActive(false);
            masterText.SetActive(true);
        }

        [PunRPC]
        private void SetReady(bool isReady)
        {
            readyText.SetActive(isReady);
        }
    }

}

