using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

namespace Donghyun.Network
{
    public class NetWorkManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private List<GameObject> players = new List<GameObject>();
        [SerializeField] private TextMeshProUGUI roomNameText;
        [SerializeField] private TextMeshProUGUI playerNumText;
        [SerializeField] private GameObject startButtonObj;
        [SerializeField] private GameObject readyButtonObj;
        [SerializeField] private Button exitButton;

        private PhotonView pv;
        private List<PlayerUI> playerUIs = new List<PlayerUI>();
        private Button startButton;
        private Button readyButton;
        private bool isReady = false;
        private Hashtable ht;

        void Awake()
        {
            pv = GetComponent<PhotonView>();

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] != null)
                {
                    PlayerUI playerUI = players[i].GetComponent<PlayerUI>();
                    if (playerUI != null)
                        playerUIs.Add(playerUI);
                    else
                    {
                        Debug.LogError($"[NetWorkManager] players[{i}]에 PlayerUI 컴포넌트가 없습니다!");
                        playerUIs.Add(null);
                    }
                }
                else
                {
                    Debug.LogError($"[NetWorkManager] players[{i}]가 null입니다!");
                    playerUIs.Add(null);
                }
            }

            PhotonNetwork.SendRate = 40;
            PhotonNetwork.SerializationRate = 20;

            startButton = startButtonObj.GetComponent<Button>();
            readyButton = readyButtonObj.GetComponent<Button>();

            PhotonNetwork.AutomaticallySyncScene = true;

            if (master())
            {
                isReady = true;
                startButtonObj.SetActive(true);
                readyButtonObj.SetActive(false);
                startButton.interactable = false;
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "ReadyPlayer", 1 } });
            }
            else
            {
                startButtonObj.SetActive(false);
                readyButtonObj.SetActive(true);
            }

            ht = PhotonNetwork.CurrentRoom.CustomProperties;

            startButton.onClick.AddListener(GameStart);
            readyButton.onClick.AddListener(SetReady);
            exitButton.onClick.AddListener(LeaveRoom);

            roomNameText.text = string.Format("{0}", PhotonNetwork.CurrentRoom.Name);

            RoomRenewal();
            UserRenewal();
        }

        public bool master() => PhotonNetwork.LocalPlayer.IsMasterClient;

        public void LeaveRoom()
        {
            object num;
            if (isReady)
            {
                ht.TryGetValue("ReadyPlayer", out num);
                ht["ReadyPlayer"] = (int)num - 1;
                PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
            }
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("StartScene");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"LobbyScene - {newPlayer.NickName} 입장 (ActorNumber: {newPlayer.ActorNumber})");

            // PlayerSlot이 없다면 빈 슬롯을 할당
            if (!newPlayer.CustomProperties.ContainsKey("PlayerSlot"))
            {
                HashSet<int> usedSlots = new HashSet<int>();
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if (player.CustomProperties.ContainsKey("PlayerSlot"))
                        usedSlots.Add((int)player.CustomProperties["PlayerSlot"]);
                }
                for (int i = 0; i < 4; i++)
                {
                    if (!usedSlots.Contains(i))
                    {
                        Hashtable props = new Hashtable();
                        props["PlayerSlot"] = i;
                        newPlayer.SetCustomProperties(props);
                        Debug.Log($"[NetWorkManager] {newPlayer.NickName}에게 PlayerSlot {i} 할당됨.");
                        break;
                    }
                }
            }
            int slotIndex = (int)newPlayer.CustomProperties["PlayerSlot"];
            SetPlayerUI(playerUIs[slotIndex], newPlayer.NickName, newPlayer.IsMasterClient);
            players[slotIndex].SetActive(true);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"LobbyScene - {otherPlayer.NickName} 퇴장 (ActorNumber: {otherPlayer.ActorNumber})");

            if (!otherPlayer.CustomProperties.ContainsKey("PlayerSlot"))
            {
                Debug.LogError($"[NetWorkManager] {otherPlayer.NickName}의 PlayerSlot 정보가 없습니다!");
                return;
            }

            int slotIndex = (int)otherPlayer.CustomProperties["PlayerSlot"];
            players[slotIndex].SetActive(false);

            // 해당 슬롯을 빈 슬롯(-1)으로 표시
            Hashtable props = new Hashtable();
            props["PlayerSlot"] = -1;
            otherPlayer.SetCustomProperties(props);

            pv.RPC("SetStartButton", RpcTarget.MasterClient);
            RoomRenewal();
            UserRenewal();
        }

        public void RoomRenewal()
        {
            Debug.Log("LobbyScene - 방 정보 갱신");
            playerNumText.text = string.Format("{0} / {1}", PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);
        }

        public void UserRenewal()
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey("PlayerSlot"))
                {
                    int slot = (int)player.CustomProperties["PlayerSlot"];
                    SetPlayerUI(playerUIs[slot], player.NickName, player.IsMasterClient);
                    players[slot].SetActive(true);
                }
            }
        }

        public void SetPlayerUI(PlayerUI ui, string name, bool isMaster)
        {
            if (ui == null)
            {
                Debug.LogError($"[NetWorkManager] SetPlayerUI() 호출 실패: PlayerUI가 null입니다! (name: {name}, isMaster: {isMaster})");
                return;
            }
            ui.SetNickname(name);
            if (name == PhotonNetwork.LocalPlayer.NickName)
                ui.SetNickNameColor(Color.red);
            else
                ui.SetNickNameColor(Color.white);
            if (isMaster)
                ui.SetMaster();
            else
                ui.SetClient();
        }

        public void GameStart()
        {
            PhotonNetwork.LoadLevel("PlayScene");
        }

        public void SetReady()
        {
            object num;
            isReady = !isReady;

            // 여기서 Ready 상태를 각 플레이어의 슬롯(즉, CustomProperties["PlayerSlot"])을 기반으로 RPC 호출
            pv.RPC("SetAllReadyState", RpcTarget.All, (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerSlot"], isReady);

            if (isReady)
            {
                ht.TryGetValue("ReadyPlayer", out num);
                ht["ReadyPlayer"] = (int)num + 1;
            }
            else
            {
                ht.TryGetValue("ReadyPlayer", out num);
                ht["ReadyPlayer"] = (int)num - 1;
            }
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
            pv.RPC("SetStartButton", RpcTarget.MasterClient);
        }

        [PunRPC]
        public void SetAllReadyState(int index, bool isTrue)
        {
            playerUIs[index].SetReady(isTrue);
        }

        [PunRPC]
        public void SetStartButton()
        {
            if ((int)ht["ReadyPlayer"] > 1)
                startButton.interactable = true;
            else
                startButton.interactable = false;
        }
    }
}