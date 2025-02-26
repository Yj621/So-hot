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
<<<<<<< HEAD
=======
    [Serializable]
    public class Slot
    {
        public SortedSet<int> slot;
    }

    [Serializable]
    public class SerializableSlot
    {
        public List<int> slot;
    }

>>>>>>> 86c93f92 ([FIX] Lobby)
    public class NetWorkManager : MonoBehaviourPunCallbacks
    {
        [Header("----- 스폰될 프리팹 이름 -----")]
        [SerializeField] private string spawnPrefabName;

        [Header("----- 플레이어가 들어갈 슬롯 -----")]
        [SerializeField] private List<Transform> playerSlots = new List<Transform>(4); //초기용량 4

        [Header("----- UI 관련 -----")]
        [SerializeField] private TextMeshProUGUI roomNameText;
        [SerializeField] private TextMeshProUGUI playerNumText;
        [SerializeField] private GameObject startButtonObj;
        [SerializeField] private GameObject readyButtonObj;
        [SerializeField] private Button exitButton;

<<<<<<< HEAD
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
=======
        private static NetWorkManager instance;

        private PhotonView pv; //포톤 뷰

        private Hashtable ht; //커스텀 프로퍼티 캐싱

        //플레이어 개인이 소유하는 본인 변수들
        private int playerNumber;
        private bool isReady;
        private int ActorNumber;
        private string ActorNumberString;
        private GameObject player;
        private LobbyPlayer playerSetting;
        private Button startButton;
        private Button readyButton;

        private Slot emptyPlayer = new Slot(); //플레이어 슬롯

        public static NetWorkManager Instance => instance;
        public List<Transform> PlayerSlots => playerSlots;

        void Awake()
        {
            instance = this;

            RoomInitSetting(); //방 초기 포톤 설정
            RoomUIInitSetting(); //방 UI 초기 설정
            PlayerInitSetting(); //플레이어 초기 설정
>>>>>>> 86c93f92 ([FIX] Lobby)
        }

        public bool master() => PhotonNetwork.LocalPlayer.IsMasterClient;

<<<<<<< HEAD
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

=======
        //마스터 바뀔 때
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            Debug.Log("LobbyScene - 마스터 변경");

            isReady = false;
            playerSetting.SetMasterTextRPC();
            SetStartButton();
        }


        #region 1.방 초기세팅
        private void RoomInitSetting()
        {
            //포톤 뷰
            pv = GetComponent<PhotonView>();

            PhotonNetwork.SendRate = 40; //포톤이 서버와 통신하는 빈도
            PhotonNetwork.SerializationRate = 20; //객체 상태 업데이트 빈도(트랜스폼, etc...)

            PhotonNetwork.AutomaticallySyncScene = true; //모든 클라이언트와 함께 씬 이동

            ht = PhotonNetwork.CurrentRoom.CustomProperties; //커스텀 프로퍼티 캐싱
        }

        private void RoomUIInitSetting()
        {
            startButton = startButtonObj.GetComponent<Button>();
            readyButton = readyButtonObj.GetComponent<Button>();

            startButton.onClick.AddListener(GameStart);
            readyButton.onClick.AddListener(SetReady);
            exitButton.onClick.AddListener(LeaveRoom);

            roomNameText.text = string.Format("{0}", PhotonNetwork.CurrentRoom.Name); ///방 이름 설정

            RoomRenewal(); //방 정보 갱신
        }

        private void PlayerInitSetting()
        {
            //플레이어 생성 후 변수 초기화
            player = PhotonNetwork.Instantiate(spawnPrefabName, Vector3.zero, Quaternion.identity);
            playerSetting = player.GetComponent<LobbyPlayer>();

            ActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            ActorNumberString = ActorNumber.ToString();

            isReady = true;
            SetReady();

            if (master())
            {
                playerSetting.SetMasterTextRPC();
                startButtonObj.SetActive(true);
                readyButtonObj.SetActive(false);
                startButton.interactable = false;
                emptyPlayer.slot = new SortedSet<int> { 0, 1, 2, 3 };
            }
            else
            {
                //마스터가 아닐 경우 emptyPlayer.slot을 새로 받아와서 동기화
                ConvertJsonToEmptyPlayerSlot();

                playerSetting.SetClientTextRPC();
                startButtonObj.SetActive(false);
                readyButtonObj.SetActive(true);
            }

            //플레이어의 본인 슬롯을 할당
            playerNumber = emptyPlayer.slot.Min;
            emptyPlayer.slot.Remove(playerNumber);
            playerSetting.SetPlayerSlotRPC(playerNumber, RpcTarget.All);
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { ActorNumberString, playerNumber } });

            //빈 슬롯 할당 후 커스텀 프로퍼티 바꾸기
            ConvertEmptyPlayerSlotToJson();

            //본인이므로 이름을 빨간색으로 만들어 줌
            playerSetting.SetNickNameColor(Color.red);

            //모든 클라이언트에게 본인의 닉네임을 표시
            playerSetting.SetNickNameRPC(PhotonNetwork.NickName, RpcTarget.All);

        }
        #endregion


        //본인이 방을 떠날 때
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        private void ConvertJsonToEmptyPlayerSlot()
        {
            object JSonData;
            ht.TryGetValue("EmptyPlayerSlot", out JSonData);

            SerializableSlot emptySlot = JsonUtility.FromJson<SerializableSlot>((string)JSonData);
            emptyPlayer.slot = new SortedSet<int>(emptySlot.slot);

            emptySlot.slot.Clear();
        }

        private void ConvertEmptyPlayerSlotToJson()
        {
            SerializableSlot emptySlot = new SerializableSlot();
            emptySlot.slot = new List<int>(emptyPlayer.slot);

            string ConvertJson = JsonUtility.ToJson(emptySlot);
            ht["EmptyPlayerSlot"] = ConvertJson;
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

            emptySlot.slot.Clear();
        }

        //방에서 완전히 떠난 뒤 실행
>>>>>>> 86c93f92 ([FIX] Lobby)
        public override void OnLeftRoom()
        {
            PhotonNetwork.AutomaticallySyncScene = false;

            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("StartScene");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"LobbyScene - {newPlayer.NickName} 입장 (ActorNumber: {newPlayer.ActorNumber})");

<<<<<<< HEAD
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
=======
            RoomRenewal();

            playerSetting.SetPlayerSlotRPC(playerNumber, newPlayer);
            playerSetting.SetNickNameRPC(PhotonNetwork.NickName, newPlayer);
            if(master())
            {
                playerSetting.SetMasterTextRPC(newPlayer);
            }
            else
            {
                playerSetting.SetClientTextRPC(newPlayer);
            }
>>>>>>> 86c93f92 ([FIX] Lobby)
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

            RoomRenewal();
<<<<<<< HEAD
            UserRenewal();
=======

            ConvertJsonToEmptyPlayerSlot();
            emptyPlayer.slot.Add((int)otherPlayer.CustomProperties[otherPlayer.ActorNumber.ToString()]);
            ConvertEmptyPlayerSlotToJson();

            otherPlayer.CustomProperties.Clear();

            if (master())
            {
                startButtonObj.SetActive(true);
                readyButtonObj.SetActive(false);
            }

            ht.Remove(otherPlayer.ActorNumber.ToString());
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
>>>>>>> 86c93f92 ([FIX] Lobby)
        }

        public void RoomRenewal()
        {
            Debug.Log("LobbyScene - 방 정보 갱신");
<<<<<<< HEAD
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
=======
            playerNumText.text = string.Format("{0} / {1}", PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers); //전체 플레이어 수

>>>>>>> 86c93f92 ([FIX] Lobby)
        }

        public void GameStart()
        {
            PhotonNetwork.LoadLevel("PlayScene");
        }

        public void SetReady()
        {
<<<<<<< HEAD
            object num;
            isReady = !isReady;

            // 여기서 Ready 상태를 각 플레이어의 슬롯(즉, CustomProperties["PlayerSlot"])을 기반으로 RPC 호출
            pv.RPC("SetAllReadyState", RpcTarget.All, (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerSlot"], isReady);
=======
            //레디 변경
            isReady = !isReady;

            //커스텀 프로퍼티에도 적용
            ht[ActorNumberString] = isReady;
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

            //레디 UI 상태 변환
            playerSetting.SetReadytRPC(isReady);
>>>>>>> 86c93f92 ([FIX] Lobby)

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

<<<<<<< HEAD
        [PunRPC]
        public void SetAllReadyState(int index, bool isTrue)
        {
            playerUIs[index].SetReady(isTrue);
        }

        [PunRPC]
        public void SetStartButton()
        {
            if ((int)ht["ReadyPlayer"] > 1)
=======
        //스타트 버튼 상태 갱신 - 마스터에서만 실행
        [PunRPC]
        public void SetStartButton()
        {
            Debug.Log("LobbyScene - 스타트 버튼 상태 갱신");

            int count = 1;

            //마스터인 본인 제외
            foreach (int actorNumber in PhotonNetwork.CurrentRoom.Players.Keys)
            {
                if (ActorNumber != actorNumber)
                {
                    if ((bool)ht[actorNumber.ToString()]) count++;
                }
            }
            
            if (count == 2)
            {
>>>>>>> 86c93f92 ([FIX] Lobby)
                startButton.interactable = true;
            else
                startButton.interactable = false;
<<<<<<< HEAD
=======
            }
>>>>>>> 86c93f92 ([FIX] Lobby)
        }
    }
}