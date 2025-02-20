using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private static string userNickName;
    private const int MaxNicknameLenght = 8;

    [Header("--- Panel ---")]
    public GameObject startUI;
    public GameObject nickNameUI;
    public GameObject roomListUI;
    public GameObject createRoomUI;
    public GameObject settingUI;
    public GameObject QuitUI;

    [Header("--- Text ---")]

    [Header("--- InputField ---")]
    public TMP_InputField nickNameInput;
    public TMP_InputField roomNameInput;

    [Header("--- Room List ---")]
    public GameObject roomListContent;


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

}
