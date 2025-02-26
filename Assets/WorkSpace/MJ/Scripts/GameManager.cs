using UnityEngine;
using Photon.Pun;
using KJ.Player;
using UnityEngine.UIElements;
public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance;

    PlayerController player1;
    PlayerController player2;
    PlayerController player3;
    PlayerController player4;

    bool player1Dead;
    bool player2Dead;
    bool player3Dead;
    bool player4Dead;

    public Vector3 savePoint; //현재 저장된 플레이어 세이브 포인트의 위치 정보(세이브 포인트에 trigger되면 본 필드값이 수정되어야 함)
    public Vector3 fireSavePoint; //현재 저장된 fire 세이브 포인트의 위치 정보(세이브 포인트에 trigger되면 본 필드값이 수정되어야 함)


    void Start()
    {
        if (Instance == null) 
        {
            Instance = this; 
        }
        else 
        { 
            Destroy(Instance); 
        }

        //플레이어 사망 여부 캐싱
        player1Dead = player1.GetComponent<PlayerState>().isDead;
        player2Dead = player2.GetComponent<PlayerState>().isDead;
        player3Dead = player3.GetComponent<PlayerState>().isDead;
        player4Dead = player4.GetComponent<PlayerState>().isDead;

        fireSavePoint = Fire.Instance.firstFirePos;
        //TO-DO: savePoint = (게임 시작 시, 플레이어가 처음 로드되는 위치) 
        photonView.RPC("AllPlayerRespawn", RpcTarget.All);
 
    }

    //플레이어 전체를 각각의 변수(player1~4)에 등록시키는 함수
    public void RegisterPlayer(PlayerController player)
    {
        if (player1 == null) player1 = player;
        else if (player2 == null) player2 = player;
        else if (player3 == null) player3 = player;
        else if (player4 == null) player4 = player;
    }

    [PunRPC]
    public void GameClear()
    {
        //TO-DO: 게임 클리어 연출이 명확해지면 수정
        Debug.Log("게임 클리어");
    }

    [PunRPC]
    void AllPlayerRespawn()
    {
        Fire.Instance.isOnFire = true;
        Fire.Instance.gameObject.transform.position = fireSavePoint; 

        //TO-DO: 세이브 포인트 위치가 명확해지면 수정 (각 플레이어 리스폰 위치값 미세 조정 필요)
        player1.gameObject.transform.position = savePoint;
        player2.gameObject.transform.position = savePoint;
        player3.gameObject.transform.position = savePoint;
        player4.gameObject.transform.position = savePoint;

    }

    [PunRPC]
    void PlayerDie(int playerViewID)
    {
        /*
        1.플레이어는 자신의 PlayerState에서 isDead를 변경한 후, PlayerDie() RPC 호출
        2.플레이어 전체 사망 여부를 우선 체크하여, true일 경우에 AllPlayerRespawn() RPC 호출
        3.1명의 플레이어라도 살아있는 경우, 파라미터로 받아온 PlayerViewID를 갖는 플레이어의 사망 처리 진행
        */

        if (player1Dead && player2Dead && player3Dead && player4Dead)
        {
            photonView.RPC("AllPlayerRespawn", RpcTarget.All);
        }
        else
        {
            //TO-DO: playerViewID에 맞는 플레이어의 사망 처리 (이 블럭에서) 진행
            //TO-DO: PlayerResurrection() RpcTarget은 All이 맞는가?
            photonView.RPC("PlayerResurrection", RpcTarget.All, playerViewID);
        }

    }

    [PunRPC]
    void PlayerResurrection(int playerViewID)
    {

        //TO-DO: playerViewID에 맞는 플레이어의 부활 처리
    }


}
