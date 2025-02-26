using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using System;

namespace Donghyun.Network
{
    [Serializable]
    public struct PlayerInfo
    {
        public int playerNumber;
        public bool isReady;
        public PlayerInfo(int _playerNumber, bool _isReady)
        {
            this.playerNumber = _playerNumber;
            this.isReady = _isReady;
        }
    }

    [Serializable]
    public class Slot
    {
        public List<int> slot = UnityEngine.Pool.ListPool<int>.Get();
    }

    public class NetWorkManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private List<GameObject> players = new List<GameObject>();
        [SerializeField] private TextMeshProUGUI roomNameText;
        [SerializeField] private TextMeshProUGUI playerNumText;
        [SerializeField] private GameObject startButtonObj;
        [SerializeField] private GameObject readyButtonObj;
        [SerializeField] private Button exitButton;

        private PhotonView pv; //포톤 뷰

        private Button startButton;
        private Button readyButton;

        PlayerInfo playerInfo;
        private Hashtable ht;

        private List<PlayerUI> playerUIs = new List<PlayerUI>();
            
        private Slot emptyPlayer = new Slot();

        void Awake()
        {
            //포톤 뷰
            pv = GetComponent<PhotonView>();

            for (int  i = 0; i < players.Count; i++)
            {
                playerUIs.Add(players[i].GetComponent<PlayerUI>());
            }

            PhotonNetwork.SendRate = 40; //포톤이 서버와 통신하는 빈도
            PhotonNetwork.SerializationRate = 20; //객체 상태 업데이트 빈도(트랜스폼, etc...)

            startButton = startButtonObj.GetComponent<Button>();
            readyButton = readyButtonObj.GetComponent<Button>();

            PhotonNetwork.AutomaticallySyncScene = true; //모든 클라이언트와 함께 씬 이동

            ht = PhotonNetwork.CurrentRoom.CustomProperties;

            //마스터 클라이언트
            if (master())
            {
                startButtonObj.SetActive(true);
                readyButtonObj.SetActive(false);
                startButton.interactable = false;

                //빈 플레이어 슬롯에 리스트 풀 할당
                emptyPlayer.slot = UnityEngine.Pool.ListPool<int>.Get();

                //Awake에서 마스터 클라이언트라는 건 방을 만든 사람이라는 뜻. 그러므로 방의 초기값을 지정해준다
                playerInfo = new PlayerInfo(0, false);
                ConvertPlayerInfoToJson();

                for (int i = 1; i < 4; ++i)
                {
                    emptyPlayer.slot.Add(i);
                }
            }
            else
            {
                ConvertJsonToEmptyPlayerSlot(); //EmptyPlayerSlot 초기화

                playerInfo = new PlayerInfo(emptyPlayer.slot[0], false);

                ConvertPlayerInfoToJson(); //playerInfo 추가

                emptyPlayer.slot.RemoveAt(0);

                startButtonObj.SetActive(false);
                readyButtonObj.SetActive(true);
            }
            ConvertEmptyPlayerSlotToJson(); //EmptyPlayerSlot를 Json화하여 커스텀 프로퍼티에 삽입

            playerUIs[playerInfo.playerNumber].SetNickNameColor(Color.red);

            startButton.onClick.AddListener(GameStart);
            readyButton.onClick.AddListener(SetReady);
            exitButton.onClick.AddListener(LeaveRoom);

            roomNameText.text = string.Format("{0}", PhotonNetwork.CurrentRoom.Name); ///방 이름 설정

            RoomRenewal();
            UserRenewal();
        }

        /// 마스터 권한
        public bool master() => PhotonNetwork.LocalPlayer.IsMasterClient;

        //마스터 바뀔 때
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            playerInfo.isReady = false;
            ConvertPlayerInfoToJson();
            SetStartButton();
        }

        //본인이 방을 떠날 때
        public void LeaveRoom()
        {
            ConvertJsonToEmptyPlayerSlot();
            emptyPlayer.slot.Add(playerInfo.playerNumber);
            ConvertEmptyPlayerSlotToJson();

            //리스트 풀 회수
            UnityEngine.Pool.ListPool<int>.Release(emptyPlayer.slot);

            ht.Remove(PhotonNetwork.LocalPlayer.ActorNumber.ToString());

            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LeaveRoom();
        }


        private PlayerInfo ConvertJsonToPlayerInfo(int ActorNumber)
        {
            object JsonData;
            ht.TryGetValue(ActorNumber.ToString(), out JsonData);
            return JsonUtility.FromJson<PlayerInfo>((string)JsonData);
        }

        private void ConvertPlayerInfoToJson()
        {
            string ConvertJson = JsonUtility.ToJson(playerInfo);
            ht[PhotonNetwork.LocalPlayer.ActorNumber.ToString()] = ConvertJson;
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
        }

        private void ConvertJsonToEmptyPlayerSlot()
        {
            object JSonData;
            ht.TryGetValue("EmptyPlayerSlot", out JSonData);
            emptyPlayer = JsonUtility.FromJson<Slot>((string)JSonData);
            emptyPlayer.slot.Sort();
        }

        private void ConvertEmptyPlayerSlotToJson()
        {
            emptyPlayer.slot.Sort();
            string ConvertJson = JsonUtility.ToJson(emptyPlayer);
            ht["EmptyPlayerSlot"] = ConvertJson;
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
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

            int playerNumber = emptyPlayer.slot[0];
            SetPlayerUI(playerUIs[playerNumber], newPlayer.NickName, newPlayer.IsMasterClient);
            players[playerNumber].SetActive(true);
        }

        //누군가 방을 떠날때
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("LobbyScene - 인원 퇴장");

            pv.RPC("SetStartButton", RpcTarget.MasterClient);
            RoomRenewal();
            UserRenewal();

            PlayerInfo info = ConvertJsonToPlayerInfo(otherPlayer.ActorNumber);
            players[info.playerNumber].SetActive(false);

            if (master())
            {
                startButtonObj.SetActive(true);
                readyButtonObj.SetActive(false);
            }

            ht.Remove(otherPlayer.ActorNumber);
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
            foreach (int ActorNumber in PhotonNetwork.CurrentRoom.Players.Keys)
            {
                PlayerInfo info = ConvertJsonToPlayerInfo(ActorNumber);
                int i = info.playerNumber;

                SetPlayerUI(playerUIs[i], PhotonNetwork.CurrentRoom.Players[ActorNumber].NickName, PhotonNetwork.CurrentRoom.Players[ActorNumber].IsMasterClient);

                players[i].SetActive(true);
            }
        }

        public void SetPlayerUI(PlayerUI ui, string name, bool isMaster)
        {
            ui.SetNickname(name);

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
            playerInfo.isReady = !playerInfo.isReady;
            ConvertPlayerInfoToJson();

            //레디 UI 상태 변환
            pv.RPC("SetReadyUI", RpcTarget.All, playerInfo.playerNumber, playerInfo.isReady);

            //스타트 버튼 활성화 판별
            pv.RPC("SetStartButton", RpcTarget.MasterClient);
        }

        //레디 UI 상태 변환
        [PunRPC]
        public void SetReadyUI(int index, bool isTrue)
        {
            playerUIs[index].SetReady(isTrue);
        }

        //스타트 버튼 상태 갱신
        [PunRPC]
        public void SetStartButton()
        {
            List<PlayerInfo> playerInfoGroup = UnityEngine.Pool.ListPool<PlayerInfo>.Get();

            //마스터인 본인 제외
            foreach (int ActorNumber in PhotonNetwork.CurrentRoom.Players.Keys)
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber != ActorNumber)
                {
                    Debug.Log(ActorNumber);
                    PlayerInfo info = ConvertJsonToPlayerInfo(ActorNumber);
                    playerInfoGroup.Add(info);
                }
            }
            
            bool allReady = true;
            foreach (PlayerInfo info in playerInfoGroup)
            {
                allReady = allReady & info.isReady;
            }

            if (allReady && playerInfoGroup.Count >= 1)
            {
                startButton.interactable = true;
            }
            else
            {
                startButton.interactable = false;
            }

            UnityEngine.Pool.ListPool<PlayerInfo>.Release(playerInfoGroup);
        }
    }
}
