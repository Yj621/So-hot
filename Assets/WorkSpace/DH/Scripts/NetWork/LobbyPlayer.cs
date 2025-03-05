using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;

namespace Donghyun.Network
{
    public class LobbyPlayer : MonoBehaviour
    {
        [SerializeField] private List<GameObject> Character = new List<GameObject> (4);
        [SerializeField] private GameObject readyText;
        [SerializeField] private GameObject masterText;
        [SerializeField] private TextMeshProUGUI nameText;

        private GameObject curCharacter;
        private PhotonView pv;

        private void Awake()
        {
            curCharacter = Character[0];
            pv = GetComponent<PhotonView>();
        }

        //닉네임 색 바꾸는 함수
        public void SetNickNameColor(Color color)
        {
            nameText.color = color;
        }
        public void SetReadytRPC(bool isReady) => pv.RPC("SetReady", RpcTarget.AllBufferedViaServer, isReady);
        public void SetClientTextRPC() => pv.RPC("SetClientText", RpcTarget.AllBufferedViaServer);
        public void SetClientTextRPC(Player player) =>  pv.RPC("SetClientText", player);
        public void SetMasterTextRPC() => pv.RPC("SetMasterText", RpcTarget.AllBufferedViaServer);
        public void SetMasterTextRPC(Player player) => pv.RPC("SetMasterText", player);
        public void SetNickNameRPC(string name, RpcTarget target) => pv.RPC("SetNickName", target, name);
        public void SetNickNameRPC(string name, Player player) => pv.RPC("SetNickName", player, name);
        public void SetPlayerSlotRPC(int playerNumber, RpcTarget target) => pv.RPC("SetPlayerSlot", target, playerNumber);
        public void SetPlayerSlotRPC(int playerNumber, Player player) => pv.RPC("SetPlayerSlot", player, playerNumber);
        public void SetCharacterRPC(int index) => pv.RPC("SetCharacter", RpcTarget.AllBufferedViaServer, index);

        /// <summary>
        /// 캐릭터를 변경
        /// </summary>
        /// <param name="index"></param>
        [PunRPC]
        private void SetCharacter(int index)
        {
            curCharacter.SetActive(false);
            curCharacter = Character[index];
            curCharacter.SetActive(true);
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

