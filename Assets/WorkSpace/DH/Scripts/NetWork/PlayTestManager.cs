using KJ.CameraSystem;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayTestManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private string spawnPrefabName;

    private GameObject player; //현재 클라이언트가 물고 있는 플레이어


    //포톤 서버 연결
    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    //게임이 포톤 마스터 서버에 무사 접속 시 실행
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);
    }

    //방에 입장했을때 호출되는 메서드
    public override void OnJoinedRoom()
    {
        player = PhotonNetwork.Instantiate(spawnPrefabName, spawnPosition.position, Quaternion.identity);
        Camera.main.GetComponent<CameraController>().PlayerBody = player.transform;
    }
}
