using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace YJ.Network
{

    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        private static string userNickName;
        private const int MaxNicknameLenght = 8;


        [Header("--- Panel ---")]
        [SerializeField] private GameObject startUI;
        [SerializeField] private GameObject nickNameUI;
        [SerializeField] private GameObject roomListUI;
        [SerializeField] private GameObject createRoomUI;

        [Header("--- Text ---")]
        [SerializeField] private TextMeshProUGUI nickNameStateText;
        [SerializeField] private TextMeshProUGUI roomListNickName;

        [Header("--- InputField ---")]
        [SerializeField] private TMP_InputField nickNameInput;
        [SerializeField] private TMP_InputField roomNameInput;

        [Header("--- Room List ---")]
        [SerializeField] private GameObject roomListContent;
        [SerializeField] private GameObject roomListPrefab;


        void Start()
        {
            //포톤 연결 설정
            PhotonNetwork.ConnectUsingSettings();
        }

        //마스터 서버 연결시
        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }

        //로비에 성공적으로 입장하면 호출되는 메서드 (StartPanel이 나타남)
        public override void OnJoinedLobby()
        {
            startUI.SetActive(true);
        }

        /// <summary>
        /// NickNameUI 관련 함수
        /// 닉네임 확정 버튼
        /// </summary>
        public void OnClickNameConfirm()
        {
            //입력한 텍스트에서 공백 제거
            string inputNickName = nickNameInput.text.Trim();

            //입력한 닉네임이 비어있다면
            if (string.IsNullOrEmpty(inputNickName))
            {
                nickNameStateText.text = "이름이 비었다.";
                return;
            }

            if (inputNickName.Length > MaxNicknameLenght)
            {
                inputNickName = inputNickName.Substring(0, MaxNicknameLenght);
                nickNameInput.text = inputNickName; //입력 필드 갱신
                nickNameStateText.text = $"이름은 8글자까지!";
            }

            userNickName = inputNickName;
            PhotonNetwork.NickName = userNickName;
            nickNameUI.SetActive(false);
            roomListUI.SetActive(true);

            roomListNickName.text = userNickName;
            Debug.Log($"roomListNickName.text : " + roomListNickName);
        }


        /// <summary>
        /// RoomList UI 관련 함수
        /// 방목록을 업데이트하는 메서드
        /// </summary>
        public void UpdateRoomList(List<RoomInfo> rooms)
        {
            foreach (Transform child in roomListContent.transform)
            {
                Destroy(child.gameObject);
            }

            // 방 리스트 프리팹 연동
            foreach (RoomInfo room in rooms)
            {
                GameObject roomItem = Instantiate(roomListPrefab, roomListContent.transform);
                TextMeshProUGUI roomNameText = roomItem.transform.Find("RoomName_Text").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI roomPlayerCountText = roomItem.transform.Find("Count_Text").GetComponent<TextMeshProUGUI>();

                //방 이름과 인원수 표시
                roomNameText.text = room.Name;
                roomPlayerCountText.text = $"{room.PlayerCount} / {room.MaxPlayers}";

                Button joinButton = roomItem.transform.Find("RoomJoin_Button").GetComponent<Button>();
                joinButton.onClick.AddListener(() => JoinRoom(room.Name));

            }
        }


        // 로비에 있을때 방 목록이 갱신될 때 호출되는 콜백
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            UpdateRoomList(roomList); // 방 목록 업데이트
        }

        /// <summary>
        /// RoomList UI 관련 함수
        /// 사용자가 Room UI에서 방을 생성할때 호출되는 메서드
        /// </summary>
        public void OnClickRoomCreate()
        {
            createRoomUI.SetActive(true);
            roomListUI.SetActive(false);
        }


        // 방에 참가하는 메서드
        public void JoinRoom(string roomName)
        {
            if (PhotonNetwork.JoinRoom(roomName)) // 방 참가 시도
            {
                Debug.Log($"Trying to join room: {roomName}");

            }
            else
            {
                Debug.LogError($"방에 참가하지 못했습니다.: {roomName}");
            }
        }

        /// <summary>
        /// CreateRoom UI 관련 함수
        /// CreateRoom UI에서 방을 생성할때 호출되는 메서드
        /// </summary>
        public void OnClickCreateConfirm()
        {
            RoomOptions options = new RoomOptions();
            string roomName = roomNameInput.text.Trim();

            if (string.IsNullOrEmpty(roomName))
            {
                string[] randomTitels = { "앗! 뜨거뜨거", "손에 불난다~", "님만 오면 고", "우가우가우가", "매너 게임해요~" };

                roomName = randomTitels[Random.Range(0, randomTitels.Length)];
                Debug.Log($"랜덤 방 제목 생성: {roomName}");
            }
            options.MaxPlayers = 4;

            PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
            createRoomUI.SetActive(false);

        }

        //방에 입장했을때 호출되는 메서드
        public override void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevel("LobbyScene");
        }

    }
}