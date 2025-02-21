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
            //포톤 뷰
            pv = GetComponent<PhotonView>();

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] != null)
                {
                    PlayerUI playerUI = players[i].GetComponent<PlayerUI>();
                    if (playerUI != null)
                    {
                        playerUIs.Add(playerUI);
                    }
                    else
                    {
                        Debug.LogError($"[NetWorkManager] players[{i}]에 PlayerUI 컴포넌트가 없습니다!");
                        playerUIs.Add(null); // null을 추가하여 리스트 크기 유지
                    }
                }
                else
                {
                    Debug.LogError($"[NetWorkManager] players[{i}]가 null입니다!");
                    playerUIs.Add(null);
                }
            }

                PhotonNetwork.SendRate = 40; //포톤이 서버와 통신하는 빈도
            PhotonNetwork.SerializationRate = 20; //객체 상태 업데이트 빈도(트랜스폼, etc...)

            startButton = startButtonObj.GetComponent<Button>();
            readyButton = readyButtonObj.GetComponent<Button>();

            PhotonNetwork.AutomaticallySyncScene = true; //모든 클라이언트와 함께 씬 이동

            //마스터 클라이언트
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

            roomNameText.text = string.Format("{0}", PhotonNetwork.CurrentRoom.Name); ///벙 이름 설정

            RoomRenewal();
            UserRenewal();
        }

        /// 마스터 권한
        public bool master() => PhotonNetwork.LocalPlayer.IsMasterClient;

        //본인이 방을 떠날 때
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

        public override void OnDisconnected(DisconnectCause cause)
        {
            
        }

        //방에서 완전히 떠난 뒤 실행
        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("StartScene");
        }

        //누군가 방을 들어올때
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("LobbyScene - 인원 입장");

            RoomRenewal();
            SetPlayerUI(playerUIs[newPlayer.ActorNumber-1], newPlayer.NickName, newPlayer.IsMasterClient);
            players[newPlayer.ActorNumber - 1].SetActive(true);
        }

        //누군가 방을 떠날때
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("LobbyScene - 인원 퇴장");

            pv.RPC("SetStartButton", RpcTarget.MasterClient);
            RoomRenewal();
            UserRenewal();

            players[otherPlayer.ActorNumber-1].SetActive(false);

            if (master())
            {
                startButtonObj.SetActive(true);
                readyButtonObj.SetActive(false);
            }
        }


        //방 정보 갱신
        public void RoomRenewal() 
        {
            Debug.Log("LobbyScene - 방 정보 갱신");
            playerNumText.text = string.Format("{0} / {1}", PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers); //전체 플레이어 수
        }

        //유저 정보 갱신
        public void UserRenewal()
        {
            foreach (int i in PhotonNetwork.CurrentRoom.Players.Keys)
            {
                Debug.Log(PhotonNetwork.CurrentRoom.Players[i].ActorNumber);
                int index = PhotonNetwork.CurrentRoom.Players[i].ActorNumber;

                SetPlayerUI(playerUIs[index-1], PhotonNetwork.CurrentRoom.Players[index].NickName, PhotonNetwork.CurrentRoom.Players[index].IsMasterClient);

                players[index-1].SetActive(true);
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


            //본인은 빨간색
            if(name == PhotonNetwork.LocalPlayer.NickName)
            {
                ui.SetNickNameColor(Color.red);
            }
            else{
                ui.SetNickNameColor(Color.white);
            }

            if (isMaster)
            {
                ui.SetMaster();
            }
            else
            {
                ui.SetClient();
            }
        }



        //게임 시작 버튼
        public void GameStart()
        {
            PhotonNetwork.LoadLevel("PlayScene");
        }

        //대기 상태 갱신
        public void SetReady()
        {
            object num;
            isReady = !isReady;

            //대기 상태 변경
            pv.RPC("SetAllReadyState", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber - 1, isReady);

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

            //스타트 버튼 상태 갱신
            pv.RPC("SetStartButton", RpcTarget.MasterClient);
        }

        //모든 클라이언트의 특정 플레이어의 준비 상태 갱신
        [PunRPC]
        public void SetAllReadyState(int index, bool isTrue)
        {
            playerUIs[index].SetReady(isTrue);
        }

        //스타트 버튼 상태 갱신
        [PunRPC]
        public void SetStartButton()
        {
            if((int)ht["ReadyPlayer"] > 1)
            {
                startButton.interactable = true;
            }
            else
            {
                startButton.interactable = false;
            }
        }
    }
}
