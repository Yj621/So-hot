using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;
using static TotalMultiManager;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace Donghyun.Network
{
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

        [Header("----- 씬 이동 관련 -----")]
        [SerializeField] private string nextScene;
        [SerializeField] private string preScene;

        private static NetWorkManager instance;

        private PhotonView pv; //포톤 뷰

        private Hashtable ht; //커스텀 프로퍼티 캐싱

        //플레이어 개인이 소유하는 본인 변수들
        private int playerNumber;
        private bool isReady;
        private GameObject player;
        private LobbyPlayer playerSetting;
        private Button startButton;
        private Button readyButton;

        private Slot emptyPlayer = new Slot(); //플레이어 슬롯

        public static NetWorkManager Instance => instance;
        public List<Transform> PlayerSlots => playerSlots;

        void Awake()
        {
            SetTag("LoadLobby", true);

            instance = this;

            RoomInitSetting(); //방 초기 포톤 설정
        }
        void Start()
        {
            RoomUIInitSetting(); //방 UI 초기 설정
            PlayerInitSetting(); //플레이어 초기 설정
        }

        //마스터 바뀔 때
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            Debug.Log("LobbyScene - 마스터 변경");

            if(master())
            {
                MasterSetting();
            }
        }

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

        private void MasterSetting()
        {
            isReady = true;
            SetTag("isReady", isReady);

            startButton.interactable = false;
            playerSetting.SetMasterTextRPC();
            startButtonObj.SetActive(true);
            readyButtonObj.SetActive(false);
            StartCoroutine(SetStartButton());
        }

        private void PlayerInitSetting()
        {
            Debug.Log("플레이어 생성 로직 실행");

            //플레이어 생성 후 변수 초기화
            player = PhotonNetwork.Instantiate(spawnPrefabName, Vector3.zero, Quaternion.identity);

            //레디매니저에 해당 플레이어 넘겨줌
            ReadyManager.Instance.SetPlayer(player);
            ReadyManager.Instance.ResetSelection();

            playerSetting = player.GetComponent<LobbyPlayer>();
        
            if (master())
            {
                MasterSetting();
                if (HasTag("Number"))
                {
                    ConvertJsonToEmptyPlayerSlot();
                }
                else
                {
                    emptyPlayer.slot = new SortedSet<int> { 0, 1, 2, 3 };
                }
            }
            else
            {
                isReady = false;
                SetTag("isReady", isReady);

                //마스터가 아닐 경우 emptyPlayer.slot을 새로 받아와서 동기화
                ConvertJsonToEmptyPlayerSlot();

                playerSetting.SetClientTextRPC();
                startButtonObj.SetActive(false);
                readyButtonObj.SetActive(true);
            }


            //플레이어의 본인 슬롯을 할당
            if (!HasTag("Number"))
            {
                playerNumber = emptyPlayer.slot.Min;
                Debug.Log("슬롯" + emptyPlayer.slot.Min);
                SetTag("Number", playerNumber);
                emptyPlayer.slot.Remove(playerNumber);

                //빈 슬롯 할당 후 커스텀 프로퍼티 바꾸기
                ConvertEmptyPlayerSlotToJson();
            }
            else
            {
                playerNumber = (int)GetTag(PhotonNetwork.LocalPlayer, "Number");
            }
            
            playerSetting.SetPlayerSlotRPC(playerNumber, RpcTarget.AllBufferedViaServer);

            //본인이므로 이름을 빨간색으로 만들어 줌
            playerSetting.SetNickNameColor(Color.red);

            //모든 클라이언트에게 본인의 닉네임을 표시
            playerSetting.SetNickNameRPC(PhotonNetwork.NickName, RpcTarget.AllBufferedViaServer);

        }

        //본인이 방을 떠날 때
        public void LeaveRoom()
        {
            PhotonNetwork.LocalPlayer.CustomProperties.Clear();
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
        public override void OnLeftRoom()
        {
            Debug.Log("LobbyScene - 인원 퇴장 (본인)");

            PhotonNetwork.AutomaticallySyncScene = false;

            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(preScene);
        }

        //누군가 방을 들어올때
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("LobbyScene - 인원 입장");

            RoomRenewal();
        }

        //누군가 방을 떠날때
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("LobbyScene - 인원 퇴장");

            RoomRenewal();

            ConvertJsonToEmptyPlayerSlot();
            emptyPlayer.slot.Add((int)GetTag(otherPlayer, "Number"));
            ConvertEmptyPlayerSlotToJson();

            otherPlayer.CustomProperties.Clear();

            pv.RPC("SetStartButton", RpcTarget.MasterClient);
        }

        //방 정보 갱신
        public void RoomRenewal() 
        {
            Debug.Log("LobbyScene - 방 정보 갱신");
            playerNumText.text = string.Format("{0} / {1}", PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers); //전체 플레이어 수

        }

        //게임 시작 버튼
        public void GameStart()
        {
            StartCoroutine(GameStartRoutine());
        }

        //게임 실행시 코루틴
        private IEnumerator GameStartRoutine()
        {
            Debug.Log("게임 시작");

            ReadyManager.Instance.SetPlayerInfoRPC();
            PhotonNetwork.CurrentRoom.IsOpen = false;  // 방을 닫아 새로운 플레이어가 못 들어오게 함
            PhotonNetwork.CurrentRoom.IsVisible = false; // 로비에서 방이 보이지 않도록 설정
            startButton.interactable = false;

            yield return HasPlayerInfo();

            PhotonNetwork.LoadLevel(nextScene);
        }

        //플레이어 정보가 모두에게 세팅되어있는가 판별
        private IEnumerator HasPlayerInfo()
        {
            while (!AllhasTag("HasInfo")) { yield return null; }
        }

        //대기 상태 갱신
        public void SetReady()
        {
            //레디 변경
            isReady = !isReady;

            //커스텀 프로퍼티에도 적용
            SetTag("isReady", isReady);

            //레디 UI 상태 변환
            playerSetting.SetReadytRPC(isReady);

            //스타트 버튼 활성화 판별
            pv.RPC("SetStartButton", RpcTarget.MasterClient);
        }

        //모두 레디 태그를 가지고 있는지 판별
        private IEnumerator HasReadyRoutine()
        {
            while (!AllhasTag("isReady")) { yield return null; }
        }

        //스타트 버튼 상태 갱신 - 마스터에서만 실행
        [PunRPC]
        private IEnumerator SetStartButton()
        {
            yield return HasReadyRoutine();

            Debug.Log("LobbyScene - 스타트 버튼 상태 갱신");

            int count = 0;

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if ((bool)GetTag(PhotonNetwork.PlayerList[i], "isReady") == true)
                {
                    Debug.Log(String.Format("LobbyScene - {0}번 레디", i));
                    count++;
                }
            }

            startButton.interactable = (count >= 1);
        }
    }
}
