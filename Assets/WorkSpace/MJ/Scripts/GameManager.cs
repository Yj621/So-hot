using UnityEngine;
using Photon.Pun;
using KJ.Player;
using UnityEngine.UIElements;
using System.Collections;
using Unity.VisualScripting;
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

    Inventory player1Inventory;
    Inventory player2Inventory;
    Inventory player3Inventory;
    Inventory player4Inventory;

    public GameObject startPos; //시작 지점
    public Vector3 savePoint; //현재 저장된 플레이어 세이브 포인트의 위치 정보(세이브 포인트에 trigger되면 본 필드값이 수정되어야 함)
    public Vector3 fireSavePoint; //현재 저장된 fire 세이브 포인트의 위치 정보(세이브 포인트에 trigger되면 본 필드값이 수정되어야 함)


    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this; 
        }
        else 
        { 
            Destroy(gameObject); 
        }

        //fireSavePoint = Fire.Instance.firstFirePos;

        //photonView.RPC("InitPlayerPos",RpcTarget.All);
 
    }

    //플레이어 전체를 각각의 변수(player1~4)에 등록시키는 함수
    public void RegisterPlayer(PlayerController player)
    {
        if (player1 == null)
        {
            player1 = player;
            player1Dead = player1.GetComponent<PlayerState>().isDead;
            player1Inventory = player1.GetComponent<Inventory>();
        }
        else if (player2 == null)
        {
            player2 = player;
            player2Dead = player2.GetComponent<PlayerState>().isDead;
            player2Inventory = player2.GetComponent<Inventory>();
        }
        else if (player3 == null)
        {
            player3 = player;
            player3Dead = player3.GetComponent<PlayerState>().isDead;
            player3Inventory = player3.GetComponent<Inventory>();
        }
        else if (player4 == null)
        {
            player4 = player;
            player4Dead = player4.GetComponent<PlayerState>().isDead;
            player4Inventory = player4.GetComponent<Inventory>();
        }
    }

    [PunRPC]
    void InitPlayerPos()
    {
        //TO-DO: LobbyScene에서 GameScene으로 캐릭터가 어떤 형태에서 넘어오는지 파악 필요 (각 플레이어 스폰 위치값 미세 조정 필요)
        player1.gameObject.transform.position = startPos.transform.position;
        player2.gameObject.transform.position = startPos.transform.position;
        player3.gameObject.transform.position = startPos.transform.position;
        player4.gameObject.transform.position = startPos.transform.position;
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

        player1Inventory.InitInventory();
        player2Inventory.InitInventory();
        player3Inventory.InitInventory();
        player4Inventory.InitInventory();
       
        //TO-DO: 세이브 포인트 위치가 명확해지면 수정 (각 플레이어 리스폰 위치값 미세 조정 필요)
        player1.gameObject.transform.position = savePoint;
        player2.gameObject.transform.position = savePoint;
        player3.gameObject.transform.position = savePoint;
        player4.gameObject.transform.position = savePoint;

    }

    [PunRPC]
    void PlayerDie(int playerViewID)
    {
        GameObject playerObj = PhotonView.Find(playerViewID).gameObject;
        PlayerState playerState = playerObj.GetComponent<PlayerState>();
        playerState.Die();

        //playerViewID가 맞는 Player를 찾아 GameManager에서도 사망 처리
        if (playerObj == player1.gameObject) player1Dead = true;
        else if (playerObj == player2.gameObject) player2Dead = true;
        else if (playerObj == player3.gameObject) player3Dead = true;
        else if (playerObj == player4.gameObject) player4Dead = true;

        if (player1Dead && player2Dead && player3Dead && player4Dead)
        {
            photonView.RPC("AllPlayerRespawn", RpcTarget.All);
        }
        else
        {
            photonView.RPC("PlayerResurrection", RpcTarget.All, playerViewID);
        }

    }

    [PunRPC]
    void PlayerResurrection(int playerViewID)
    {

        GameObject playerObj = PhotonView.Find(playerViewID).gameObject;
        PlayerState playerState = playerObj.GetComponent<PlayerState>();
        Inventory playerInven = playerObj.GetComponent<Inventory>();
        playerInven.InitInventory();
        playerState.StartCoroutine(playerState.Revive());
    }


}
